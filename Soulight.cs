// Soulight.cs - LED color control via LightProtocol commands.
//
// Loads Beelight.exe via reflection, calls LProtocolCtrl methods
// to generate encrypted wire-format packets, sends via serial.
//
// Build:
//   C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /out:Soulight.exe /reference:System.Drawing.dll Soulight.cs
//
// Usage:
//   Soulight.exe 255 0 255         (purple, 30s)
//   Soulight.exe 255 0 255 60      (purple, 60s)
//   Soulight.exe                   (interactive)

using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Threading;

class Soulight
{
    static readonly string BDir = @"C:\Program Files (x86)\Beelight\Beelight V3.0";
    static readonly string Port = "COM7";
    static readonly int Baud = 500000;

    static MethodInfo mColor, mBright, mSwitch, mWorkMode, mFrame;
    static Type tWkMode, tCmd, tAttr;
    static byte[] hbPkt;

    static Assembly Resolve(object s, ResolveEventArgs a)
    {
        string p = Path.Combine(BDir, a.Name.Split(',')[0] + ".dll");
        return File.Exists(p) ? Assembly.LoadFrom(p) : null;
    }

    static bool Init()
    {
        AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        string exe = Path.Combine(BDir, "Beelight.exe");
        if (!File.Exists(exe)) { Console.WriteLine("Beelight.exe not found"); return false; }

        Assembly asm = Assembly.LoadFrom(exe);
        Type[] types = asm.GetTypes();
        BindingFlags f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        Type lpCtrl = null, lpBase = null;
        foreach (Type t in types)
        {
            if (t.Name == "LProtocolCtrl") lpCtrl = t;
            else if (t.Name == "LProtocolBase") lpBase = t;
            else if (t.Name == "LP_WK_MODE") tWkMode = t;
            else if (t.Name == "LP_CMD") tCmd = t;
            else if (t.Name == "LP_ATTR") tAttr = t;
        }
        if (lpCtrl == null) { Console.WriteLine("LProtocolCtrl not found"); return false; }

        mColor = lpCtrl.GetMethod("GenColorPackage", f);
        mBright = lpCtrl.GetMethod("GenBrightPackage", f);
        mSwitch = lpCtrl.GetMethod("GenSwitchPackage", f);
        if (lpBase != null) mFrame = lpBase.GetMethod("GenFramePackage", f);

        foreach (MethodInfo m in lpCtrl.GetMethods(f))
            if (m.Name == "GenWorkModePackage" && m.GetParameters().Length == 2)
            { mWorkMode = m; break; }

        if (mColor == null) { Console.WriteLine("GenColorPackage not found"); return false; }

        // Pre-generate heartbeat
        if (mFrame != null && tAttr != null && tCmd != null)
        {
            try { hbPkt = (byte[])mFrame.Invoke(null, new object[] { Enum.ToObject(tAttr, 0), Enum.ToObject(tCmd, 0), new byte[0] }); }
            catch { }
        }
        return true;
    }

    static void Write(SerialPort p, byte[] d)
    {
        if (d != null && p.IsOpen) p.Write(d, 0, d.Length);
    }

    static void SendColor(int r, int g, int b, int dur)
    {
        Color c = Color.FromArgb(r, g, b);
        SerialPort port = new SerialPort(Port, Baud);
        port.DtrEnable = true;
        port.RtsEnable = true;
        port.ReadTimeout = 100;
        port.WriteTimeout = 500;

        try
        {
            port.Open();
            Thread.Sleep(300);
            try { port.ReadExisting(); } catch { }

            // Phase 1: Heartbeat burst
            for (int i = 0; i < 5; i++) { Write(port, hbPkt); Thread.Sleep(50); }
            Thread.Sleep(200);
            try { port.ReadExisting(); } catch { }

            // Phase 2: Switch ON + PC mode
            Write(port, mSwitch != null ? (byte[])mSwitch.Invoke(null, new object[] { true, (byte)0 }) : null);
            Thread.Sleep(50);
            if (mWorkMode != null && tWkMode != null)
            {
                try { Write(port, (byte[])mWorkMode.Invoke(null, new object[] { Enum.ToObject(tWkMode, 0), (byte)0 })); }
                catch { }
            }
            Thread.Sleep(50);

            // Phase 3: Color + Brightness loop
            DateTime start = DateTime.Now;
            int n = 0;
            while ((DateTime.Now - start).TotalSeconds < dur)
            {
                // Brightness MAX before each color (controller resets dimmer)
                try { Write(port, (byte[])mBright.Invoke(null, new object[] { 255, (byte)0 })); }
                catch { }
                Thread.Sleep(5);

                // Color packet (unique nonce each call)
                Write(port, (byte[])mColor.Invoke(null, new object[] { c, (byte)0 }));
                n++;
                Thread.Sleep(60);

                // Heartbeat every 10 packets
                if (n % 10 == 0) { Write(port, hbPkt); Thread.Sleep(5); }
            }
            Console.WriteLine("  Sent {0} packets, {1:F1}s", n, (DateTime.Now - start).TotalSeconds);
        }
        catch (Exception ex) { Console.WriteLine("  Serial error: {0}", ex.Message); }
        finally { if (port.IsOpen) port.Close(); }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Soulight - Beelight LED Color Control");
        Console.Write("Loading Beelight assembly... ");
        if (!Init()) return;
        Console.WriteLine("OK");

        if (args.Length >= 3)
        {
            // Single-shot mode
            int r = int.Parse(args[0]);
            int g = int.Parse(args[1]);
            int b = int.Parse(args[2]);
            int dur = args.Length >= 4 ? int.Parse(args[3]) : 30;
            SendColor(r, g, b, dur);
        }
        else
        {
            // Interactive mode
            Console.WriteLine("Interactive mode. Enter: R G B [seconds]  (q to quit)");
            Console.WriteLine("Examples: 255 0 255    0 255 0 60    q");
            while (true)
            {
                Console.Write("\nRGB> ");
                string line = Console.ReadLine();
                if (line == null || line.Trim().ToLower() == "q") break;

                string[] parts = line.Trim().Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) { Console.WriteLine("  Format: R G B [seconds]"); continue; }

                int r, g, b, dur = 30;
                if (!int.TryParse(parts[0], out r) || !int.TryParse(parts[1], out g) || !int.TryParse(parts[2], out b))
                { Console.WriteLine("  R, G, B must be 0-255"); continue; }
                if (parts.Length >= 4) int.TryParse(parts[3], out dur);

                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));

                SendColor(r, g, b, dur);
            }
        }
        Console.WriteLine("Done.");
    }
}
