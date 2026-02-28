using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Dmo;
using NAudio.Dmo.Effect;
using NAudio.Dsp;
using NAudio.FileFormats.Wav;
using NAudio.MediaFoundation;
using NAudio.Mixer;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.Asio;
using NAudio.Wave.Compression;
using NAudio.Wave.SampleProviders;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: InternalsVisibleTo("NAudioTests")]
[assembly: AssemblyCompany("Mark Heath & Contributors")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("© Mark Heath 2020")]
[assembly: AssemblyDescription("NAudio, an audio library for .NET")]
[assembly: AssemblyFileVersion("1.10.0.0")]
[assembly: AssemblyInformationalVersion("1.10.0")]
[assembly: AssemblyProduct("NAudio")]
[assembly: AssemblyTitle("NAudio")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("1.10.0.0")]
[module: UnverifiableCode]
namespace NAudio
{
	public enum Manufacturers
	{
		Microsoft = 1,
		Creative = 2,
		Mediavision = 3,
		Fujitsu = 4,
		Artisoft = 20,
		TurtleBeach = 21,
		Ibm = 22,
		Vocaltec = 23,
		Roland = 24,
		DspSolutions = 25,
		Nec = 26,
		Ati = 27,
		Wanglabs = 28,
		Tandy = 29,
		Voyetra = 30,
		Antex = 31,
		IclPS = 32,
		Intel = 33,
		Gravis = 34,
		Val = 35,
		Interactive = 36,
		Yamaha = 37,
		Everex = 38,
		Echo = 39,
		Sierra = 40,
		Cat = 41,
		Apps = 42,
		DspGroup = 43,
		Melabs = 44,
		ComputerFriends = 45,
		Ess = 46,
		Audiofile = 47,
		Motorola = 48,
		Canopus = 49,
		Epson = 50,
		Truevision = 51,
		Aztech = 52,
		Videologic = 53,
		Scalacs = 54,
		Korg = 55,
		Apt = 56,
		Ics = 57,
		Iteratedsys = 58,
		Metheus = 59,
		Logitech = 60,
		Winnov = 61,
		Ncr = 62,
		Exan = 63,
		Ast = 64,
		Willowpond = 65,
		Sonicfoundry = 66,
		Vitec = 67,
		Moscom = 68,
		Siliconsoft = 69,
		Supermac = 73,
		Audiopt = 74,
		Speechcomp = 76,
		Ahead = 77,
		Dolby = 78,
		Oki = 79,
		Auravision = 80,
		Olivetti = 81,
		Iomagic = 82,
		Matsushita = 83,
		Controlres = 84,
		Xebec = 85,
		Newmedia = 86,
		Nms = 87,
		Lyrrus = 88,
		Compusic = 89,
		Opti = 90,
		Adlacc = 91,
		Compaq = 92,
		Dialogic = 93,
		Insoft = 94,
		Mptus = 95,
		Weitek = 96,
		LernoutAndHauspie = 97,
		Qciar = 98,
		Apple = 99,
		Digital = 100,
		Motu = 101,
		Workbit = 102,
		Ositech = 103,
		Miro = 104,
		Cirruslogic = 105,
		Isolution = 106,
		Horizons = 107,
		Concepts = 108,
		Vtg = 109,
		Radius = 110,
		Rockwell = 111,
		Xyz = 112,
		Opcode = 113,
		Voxware = 114,
		NorthernTelecom = 115,
		Apicom = 116,
		Grande = 117,
		Addx = 118,
		Wildcat = 119,
		Rhetorex = 120,
		Brooktree = 121,
		Ensoniq = 125,
		Fast = 126,
		Nvidia = 127,
		Oksori = 128,
		Diacoustics = 129,
		Gulbransen = 130,
		KayElemetrics = 131,
		Crystal = 132,
		SplashStudios = 133,
		Quarterdeck = 134,
		Tdk = 135,
		DigitalAudioLabs = 136,
		Seersys = 137,
		Picturetel = 138,
		AttMicroelectronics = 139,
		Osprey = 140,
		Mediatrix = 141,
		Soundesigns = 142,
		Aldigital = 143,
		SpectrumSignalProcessing = 144,
		Ecs = 145,
		Amd = 146,
		Coredynamics = 147,
		Canam = 148,
		Softsound = 149,
		Norris = 150,
		Ddd = 151,
		Euphonics = 152,
		Precept = 153,
		CrystalNet = 154,
		Chromatic = 155,
		Voiceinfo = 156,
		Viennasys = 157,
		Connectix = 158,
		Gadgetlabs = 159,
		Frontier = 160,
		Viona = 161,
		Casio = 162,
		Diamondmm = 163,
		S3 = 164,
		FraunhoferIis = 172
	}
	public class MmException : Exception
	{
		private MmResult result;

		private string function;

		public MmResult Result => result;

		public MmException(MmResult result, string function)
			: base(ErrorMessage(result, function))
		{
			this.result = result;
			this.function = function;
		}

		private static string ErrorMessage(MmResult result, string function)
		{
			return $"{result} calling {function}";
		}

		public static void Try(MmResult result, string function)
		{
			if (result != 0)
			{
				throw new MmException(result, function);
			}
		}
	}
	public enum MmResult
	{
		NoError = 0,
		UnspecifiedError = 1,
		BadDeviceId = 2,
		NotEnabled = 3,
		AlreadyAllocated = 4,
		InvalidHandle = 5,
		NoDriver = 6,
		MemoryAllocationError = 7,
		NotSupported = 8,
		BadErrorNumber = 9,
		InvalidFlag = 10,
		InvalidParameter = 11,
		HandleBusy = 12,
		InvalidAlias = 13,
		BadRegistryDatabase = 14,
		RegistryKeyNotFound = 15,
		RegistryReadError = 16,
		RegistryWriteError = 17,
		RegistryDeleteError = 18,
		RegistryValueNotFound = 19,
		NoDriverCallback = 20,
		MoreData = 21,
		WaveBadFormat = 32,
		WaveStillPlaying = 33,
		WaveHeaderUnprepared = 34,
		WaveSync = 35,
		AcmNotPossible = 512,
		AcmBusy = 513,
		AcmHeaderUnprepared = 514,
		AcmCancelled = 515,
		MixerInvalidLine = 1024,
		MixerInvalidControl = 1025,
		MixerInvalidValue = 1026
	}
}
namespace NAudio.Utils
{
	public static class BufferHelpers
	{
		public static byte[] Ensure(byte[] buffer, int bytesRequired)
		{
			if (buffer == null || buffer.Length < bytesRequired)
			{
				buffer = new byte[bytesRequired];
			}
			return buffer;
		}

		public static float[] Ensure(float[] buffer, int samplesRequired)
		{
			if (buffer == null || buffer.Length < samplesRequired)
			{
				buffer = new float[samplesRequired];
			}
			return buffer;
		}
	}
	public static class ByteArrayExtensions
	{
		public static bool IsEntirelyNull(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static string DescribeAsHex(byte[] buffer, string separator, int bytesPerLine)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (byte b in buffer)
			{
				stringBuilder.AppendFormat("{0:X2}{1}", b, separator);
				if (++num % bytesPerLine == 0)
				{
					stringBuilder.Append("\r\n");
				}
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		public static string DecodeAsString(byte[] buffer, int offset, int length, Encoding encoding)
		{
			for (int i = 0; i < length; i++)
			{
				if (buffer[offset + i] == 0)
				{
					length = i;
				}
			}
			return encoding.GetString(buffer, offset, length);
		}

		public static byte[] Concat(params byte[][] byteArrays)
		{
			int num = 0;
			byte[][] array = byteArrays;
			foreach (byte[] array2 in array)
			{
				num += array2.Length;
			}
			if (num <= 0)
			{
				return new byte[0];
			}
			byte[] array3 = new byte[num];
			int num2 = 0;
			array = byteArrays;
			foreach (byte[] array4 in array)
			{
				Array.Copy(array4, 0, array3, num2, array4.Length);
				num2 += array4.Length;
			}
			return array3;
		}
	}
	public class ByteEncoding : Encoding
	{
		public static readonly ByteEncoding Instance = new ByteEncoding();

		private ByteEncoding()
		{
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return count;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			for (int i = 0; i < charCount; i++)
			{
				bytes[byteIndex + i] = (byte)chars[charIndex + i];
			}
			return charCount;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (bytes[index + i] == 0)
				{
					return i;
				}
			}
			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			for (int i = 0; i < byteCount; i++)
			{
				byte b = bytes[byteIndex + i];
				if (b == 0)
				{
					return i;
				}
				chars[charIndex + i] = (char)b;
			}
			return byteCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
		}

		public override int GetMaxByteCount(int charCount)
		{
			return charCount;
		}
	}
	public class ChunkIdentifier
	{
		public static int ChunkIdentifierToInt32(string s)
		{
			if (s.Length != 4)
			{
				throw new ArgumentException("Must be a four character string");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			if (bytes.Length != 4)
			{
				throw new ArgumentException("Must encode to exactly four bytes");
			}
			return BitConverter.ToInt32(bytes, 0);
		}
	}
	public class CircularBuffer
	{
		private readonly byte[] buffer;

		private readonly object lockObject;

		private int writePosition;

		private int readPosition;

		private int byteCount;

		public int MaxLength => buffer.Length;

		public int Count
		{
			get
			{
				lock (lockObject)
				{
					return byteCount;
				}
			}
		}

		public CircularBuffer(int size)
		{
			buffer = new byte[size];
			lockObject = new object();
		}

		public int Write(byte[] data, int offset, int count)
		{
			lock (lockObject)
			{
				int num = 0;
				if (count > buffer.Length - byteCount)
				{
					count = buffer.Length - byteCount;
				}
				int num2 = Math.Min(buffer.Length - writePosition, count);
				Array.Copy(data, offset, buffer, writePosition, num2);
				writePosition += num2;
				writePosition %= buffer.Length;
				num += num2;
				if (num < count)
				{
					Array.Copy(data, offset + num, buffer, writePosition, count - num);
					writePosition += count - num;
					num = count;
				}
				byteCount += num;
				return num;
			}
		}

		public int Read(byte[] data, int offset, int count)
		{
			lock (lockObject)
			{
				if (count > byteCount)
				{
					count = byteCount;
				}
				int num = 0;
				int num2 = Math.Min(buffer.Length - readPosition, count);
				Array.Copy(buffer, readPosition, data, offset, num2);
				num += num2;
				readPosition += num2;
				readPosition %= buffer.Length;
				if (num < count)
				{
					Array.Copy(buffer, readPosition, data, offset + num, count - num);
					readPosition += count - num;
					num = count;
				}
				byteCount -= num;
				return num;
			}
		}

		public void Reset()
		{
			lock (lockObject)
			{
				ResetInner();
			}
		}

		private void ResetInner()
		{
			byteCount = 0;
			readPosition = 0;
			writePosition = 0;
		}

		public void Advance(int count)
		{
			lock (lockObject)
			{
				if (count >= byteCount)
				{
					ResetInner();
					return;
				}
				byteCount -= count;
				readPosition += count;
				readPosition %= MaxLength;
			}
		}
	}
	public class Decibels
	{
		private const double LOG_2_DB = 8.685889638065037;

		private const double DB_2_LOG = 0.11512925464970228;

		public static double LinearToDecibels(double lin)
		{
			return Math.Log(lin) * 8.685889638065037;
		}

		public static double DecibelsToLinear(double dB)
		{
			return Math.Exp(dB * 0.11512925464970228);
		}
	}
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldDescriptionAttribute : Attribute
	{
		public string Description { get; }

		public FieldDescriptionAttribute(string description)
		{
			Description = description;
		}

		public override string ToString()
		{
			return Description;
		}
	}
	public static class FieldDescriptionHelper
	{
		public static string Describe(Type t, Guid guid)
		{
			FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!fieldInfo.IsPublic || !fieldInfo.IsStatic || fieldInfo.FieldType != typeof(Guid) || !((Guid)fieldInfo.GetValue(null) == guid))
				{
					continue;
				}
				object[] customAttributes = fieldInfo.GetCustomAttributes(inherit: false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					if (customAttributes[j] is FieldDescriptionAttribute fieldDescriptionAttribute)
					{
						return fieldDescriptionAttribute.Description;
					}
				}
				return fieldInfo.Name;
			}
			return guid.ToString();
		}
	}
	public static class HResult
	{
		public const int S_OK = 0;

		public const int S_FALSE = 1;

		public const int E_INVALIDARG = -2147483645;

		private const int FACILITY_AAF = 18;

		private const int FACILITY_ACS = 20;

		private const int FACILITY_BACKGROUNDCOPY = 32;

		private const int FACILITY_CERT = 11;

		private const int FACILITY_COMPLUS = 17;

		private const int FACILITY_CONFIGURATION = 33;

		private const int FACILITY_CONTROL = 10;

		private const int FACILITY_DISPATCH = 2;

		private const int FACILITY_DPLAY = 21;

		private const int FACILITY_HTTP = 25;

		private const int FACILITY_INTERNET = 12;

		private const int FACILITY_ITF = 4;

		private const int FACILITY_MEDIASERVER = 13;

		private const int FACILITY_MSMQ = 14;

		private const int FACILITY_NULL = 0;

		private const int FACILITY_RPC = 1;

		private const int FACILITY_SCARD = 16;

		private const int FACILITY_SECURITY = 9;

		private const int FACILITY_SETUPAPI = 15;

		private const int FACILITY_SSPI = 9;

		private const int FACILITY_STORAGE = 3;

		private const int FACILITY_SXS = 23;

		private const int FACILITY_UMI = 22;

		private const int FACILITY_URT = 19;

		private const int FACILITY_WIN32 = 7;

		private const int FACILITY_WINDOWS = 8;

		private const int FACILITY_WINDOWS_CE = 24;

		public static int MAKE_HRESULT(int sev, int fac, int code)
		{
			return (sev << 31) | (fac << 16) | code;
		}

		public static int GetHResult(this COMException exception)
		{
			return exception.ErrorCode;
		}
	}
	public static class IEEE
	{
		private static double UnsignedToFloat(ulong u)
		{
			return (double)(long)(u - int.MaxValue - 1) + 2147483648.0;
		}

		private static double ldexp(double x, int exp)
		{
			return x * Math.Pow(2.0, exp);
		}

		private static double frexp(double x, out int exp)
		{
			exp = (int)Math.Floor(Math.Log(x) / Math.Log(2.0)) + 1;
			return 1.0 - (Math.Pow(2.0, exp) - x) / Math.Pow(2.0, exp);
		}

		private static ulong FloatToUnsigned(double f)
		{
			return (ulong)((long)(f - 2147483648.0) + int.MaxValue + 1);
		}

		public static byte[] ConvertToIeeeExtended(double num)
		{
			int num2;
			if (num < 0.0)
			{
				num2 = 32768;
				num *= -1.0;
			}
			else
			{
				num2 = 0;
			}
			ulong num4;
			ulong num5;
			int num3;
			if (num == 0.0)
			{
				num3 = 0;
				num4 = 0uL;
				num5 = 0uL;
			}
			else
			{
				double num6 = frexp(num, out num3);
				if (num3 > 16384 || !(num6 < 1.0))
				{
					num3 = num2 | 0x7FFF;
					num4 = 0uL;
					num5 = 0uL;
				}
				else
				{
					num3 += 16382;
					if (num3 < 0)
					{
						num6 = ldexp(num6, num3);
						num3 = 0;
					}
					num3 |= num2;
					num6 = ldexp(num6, 32);
					double num7 = Math.Floor(num6);
					num4 = FloatToUnsigned(num7);
					num6 = ldexp(num6 - num7, 32);
					num7 = Math.Floor(num6);
					num5 = FloatToUnsigned(num7);
				}
			}
			return new byte[10]
			{
				(byte)(num3 >> 8),
				(byte)num3,
				(byte)(num4 >> 24),
				(byte)(num4 >> 16),
				(byte)(num4 >> 8),
				(byte)num4,
				(byte)(num5 >> 24),
				(byte)(num5 >> 16),
				(byte)(num5 >> 8),
				(byte)num5
			};
		}

		public static double ConvertFromIeeeExtended(byte[] bytes)
		{
			if (bytes.Length != 10)
			{
				throw new Exception("Incorrect length for IEEE extended.");
			}
			int num = ((bytes[0] & 0x7F) << 8) | bytes[1];
			uint num2 = (uint)((bytes[2] << 24) | (bytes[3] << 16) | (bytes[4] << 8) | bytes[5]);
			uint num3 = (uint)((bytes[6] << 24) | (bytes[7] << 16) | (bytes[8] << 8) | bytes[9]);
			double num4;
			if (num == 0 && num2 == 0 && num3 == 0)
			{
				num4 = 0.0;
			}
			else if (num == 32767)
			{
				num4 = double.NaN;
			}
			else
			{
				num -= 16383;
				num4 = ldexp(UnsignedToFloat(num2), num -= 31);
				num4 += ldexp(UnsignedToFloat(num3), num -= 32);
			}
			if ((bytes[0] & 0x80) == 128)
			{
				return 0.0 - num4;
			}
			return num4;
		}
	}
	public class IgnoreDisposeStream : Stream
	{
		public Stream SourceStream { get; private set; }

		public bool IgnoreDispose { get; set; }

		public override bool CanRead => SourceStream.CanRead;

		public override bool CanSeek => SourceStream.CanSeek;

		public override bool CanWrite => SourceStream.CanWrite;

		public override long Length => SourceStream.Length;

		public override long Position
		{
			get
			{
				return SourceStream.Position;
			}
			set
			{
				SourceStream.Position = value;
			}
		}

		public IgnoreDisposeStream(Stream sourceStream)
		{
			SourceStream = sourceStream;
			IgnoreDispose = true;
		}

		public override void Flush()
		{
			SourceStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return SourceStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return SourceStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			SourceStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			SourceStream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (!IgnoreDispose)
			{
				SourceStream.Dispose();
				SourceStream = null;
			}
		}
	}
	public static class MarshalHelpers
	{
		public static int SizeOf<T>()
		{
			return Marshal.SizeOf(typeof(T));
		}

		public static IntPtr OffsetOf<T>(string fieldName)
		{
			return Marshal.OffsetOf(typeof(T), fieldName);
		}

		public static T PtrToStructure<T>(IntPtr pointer)
		{
			return (T)Marshal.PtrToStructure(pointer, typeof(T));
		}
	}
	internal class MergeSort
	{
		private static void Sort<T>(IList<T> list, int lowIndex, int highIndex, IComparer<T> comparer)
		{
			if (lowIndex >= highIndex)
			{
				return;
			}
			int num = (lowIndex + highIndex) / 2;
			Sort(list, lowIndex, num, comparer);
			Sort(list, num + 1, highIndex, comparer);
			int num2 = num;
			int num3 = num + 1;
			while (lowIndex <= num2 && num3 <= highIndex)
			{
				if (comparer.Compare(list[lowIndex], list[num3]) <= 0)
				{
					lowIndex++;
					continue;
				}
				T value = list[num3];
				for (int num4 = num3 - 1; num4 >= lowIndex; num4--)
				{
					list[num4 + 1] = list[num4];
				}
				list[lowIndex] = value;
				lowIndex++;
				num2++;
				num3++;
			}
		}

		public static void Sort<T>(IList<T> list) where T : IComparable<T>
		{
			Sort(list, 0, list.Count - 1, Comparer<T>.Default);
		}

		public static void Sort<T>(IList<T> list, IComparer<T> comparer)
		{
			Sort(list, 0, list.Count - 1, comparer);
		}
	}
	internal class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);
	}
	public class ProgressLog : UserControl
	{
		private delegate void LogMessageDelegate(Color color, string message);

		private delegate void ClearLogDelegate();

		private IContainer components;

		private RichTextBox richTextBoxLog;

		public new string Text => richTextBoxLog.Text;

		public ProgressLog()
		{
			InitializeComponent();
		}

		public void LogMessage(Color color, string message)
		{
			if (richTextBoxLog.InvokeRequired)
			{
				Invoke(new LogMessageDelegate(LogMessage), color, message);
			}
			else
			{
				richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
				richTextBoxLog.SelectionColor = color;
				richTextBoxLog.AppendText(message);
				richTextBoxLog.AppendText(Environment.NewLine);
			}
		}

		public void ClearLog()
		{
			if (richTextBoxLog.InvokeRequired)
			{
				Invoke(new ClearLogDelegate(ClearLog), new object[0]);
			}
			else
			{
				richTextBoxLog.Clear();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
			base.SuspendLayout();
			this.richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxLog.Location = new System.Drawing.Point(1, 1);
			this.richTextBoxLog.Name = "richTextBoxLog";
			this.richTextBoxLog.ReadOnly = true;
			this.richTextBoxLog.Size = new System.Drawing.Size(311, 129);
			this.richTextBoxLog.TabIndex = 0;
			this.richTextBoxLog.Text = "";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			base.Controls.Add(this.richTextBoxLog);
			base.Name = "ProgressLog";
			base.Padding = new System.Windows.Forms.Padding(1);
			base.Size = new System.Drawing.Size(313, 131);
			base.ResumeLayout(false);
		}
	}
	public static class WavePositionExtensions
	{
		public static TimeSpan GetPositionTimeSpan(this IWavePosition @this)
		{
			return TimeSpan.FromMilliseconds((double)(@this.GetPosition() / (@this.OutputWaveFormat.Channels * @this.OutputWaveFormat.BitsPerSample / 8)) * 1000.0 / (double)@this.OutputWaveFormat.SampleRate);
		}
	}
}
namespace NAudio.Mixer
{
	public class BooleanMixerControl : MixerControl
	{
		private MixerInterop.MIXERCONTROLDETAILS_BOOLEAN boolDetails;

		public bool Value
		{
			get
			{
				GetControlDetails();
				return boolDetails.fValue == 1;
			}
			set
			{
				boolDetails.fValue = (value ? 1 : 0);
				mixerControlDetails.paDetails = Marshal.AllocHGlobal(Marshal.SizeOf(boolDetails));
				Marshal.StructureToPtr(boolDetails, mixerControlDetails.paDetails, fDeleteOld: false);
				MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Mixer | mixerHandleType), "mixerSetControlDetails");
				Marshal.FreeHGlobal(mixerControlDetails.paDetails);
			}
		}

		internal BooleanMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
		{
			base.mixerControl = mixerControl;
			base.mixerHandle = mixerHandle;
			base.mixerHandleType = mixerHandleType;
			base.nChannels = nChannels;
			mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
			GetControlDetails();
		}

		protected override void GetDetails(IntPtr pDetails)
		{
			boolDetails = MarshalHelpers.PtrToStructure<MixerInterop.MIXERCONTROLDETAILS_BOOLEAN>(pDetails);
		}
	}
	public class CustomMixerControl : MixerControl
	{
		internal CustomMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
		{
			base.mixerControl = mixerControl;
			base.mixerHandle = mixerHandle;
			base.mixerHandleType = mixerHandleType;
			base.nChannels = nChannels;
			mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
			GetControlDetails();
		}

		protected override void GetDetails(IntPtr pDetails)
		{
		}
	}
	public class ListTextMixerControl : MixerControl
	{
		internal ListTextMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
		{
			base.mixerControl = mixerControl;
			base.mixerHandle = mixerHandle;
			base.mixerHandleType = mixerHandleType;
			base.nChannels = nChannels;
			mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
			GetControlDetails();
		}

		protected override void GetDetails(IntPtr pDetails)
		{
		}
	}
	public class Mixer
	{
		private MixerInterop.MIXERCAPS caps;

		private IntPtr mixerHandle;

		private MixerFlags mixerHandleType;

		public static int NumberOfDevices => MixerInterop.mixerGetNumDevs();

		public int DestinationCount => (int)caps.cDestinations;

		public string Name => caps.szPname;

		public Manufacturers Manufacturer => (Manufacturers)caps.wMid;

		public int ProductID => caps.wPid;

		public IEnumerable<MixerLine> Destinations
		{
			get
			{
				for (int destination = 0; destination < DestinationCount; destination++)
				{
					yield return GetDestination(destination);
				}
			}
		}

		public static IEnumerable<Mixer> Mixers
		{
			get
			{
				for (int device = 0; device < NumberOfDevices; device++)
				{
					yield return new Mixer(device);
				}
			}
		}

		public Mixer(int mixerIndex)
		{
			if (mixerIndex < 0 || mixerIndex >= NumberOfDevices)
			{
				throw new ArgumentOutOfRangeException("mixerID");
			}
			caps = default(MixerInterop.MIXERCAPS);
			MmException.Try(MixerInterop.mixerGetDevCaps((IntPtr)mixerIndex, ref caps, Marshal.SizeOf(caps)), "mixerGetDevCaps");
			mixerHandle = (IntPtr)mixerIndex;
			mixerHandleType = MixerFlags.Mixer;
		}

		public MixerLine GetDestination(int destinationIndex)
		{
			if (destinationIndex < 0 || destinationIndex >= DestinationCount)
			{
				throw new ArgumentOutOfRangeException("destinationIndex");
			}
			return new MixerLine(mixerHandle, destinationIndex, mixerHandleType);
		}
	}
	public abstract class MixerControl
	{
		internal MixerInterop.MIXERCONTROL mixerControl;

		internal MixerInterop.MIXERCONTROLDETAILS mixerControlDetails;

		protected IntPtr mixerHandle;

		protected int nChannels;

		protected MixerFlags mixerHandleType;

		public string Name => mixerControl.szName;

		public MixerControlType ControlType => mixerControl.dwControlType;

		public bool IsBoolean => IsControlBoolean(mixerControl.dwControlType);

		public bool IsListText => IsControlListText(mixerControl.dwControlType);

		public bool IsSigned => IsControlSigned(mixerControl.dwControlType);

		public bool IsUnsigned => IsControlUnsigned(mixerControl.dwControlType);

		public bool IsCustom => IsControlCustom(mixerControl.dwControlType);

		public static IList<MixerControl> GetMixerControls(IntPtr mixerHandle, MixerLine mixerLine, MixerFlags mixerHandleType)
		{
			List<MixerControl> list = new List<MixerControl>();
			if (mixerLine.ControlsCount > 0)
			{
				int num = MarshalHelpers.SizeOf<MixerInterop.MIXERCONTROL>();
				MixerInterop.MIXERLINECONTROLS mixerLineControls = default(MixerInterop.MIXERLINECONTROLS);
				IntPtr intPtr = Marshal.AllocHGlobal(num * mixerLine.ControlsCount);
				mixerLineControls.cbStruct = Marshal.SizeOf(mixerLineControls);
				mixerLineControls.dwLineID = mixerLine.LineId;
				mixerLineControls.cControls = mixerLine.ControlsCount;
				mixerLineControls.pamxctrl = intPtr;
				mixerLineControls.cbmxctrl = MarshalHelpers.SizeOf<MixerInterop.MIXERCONTROL>();
				try
				{
					MmResult mmResult = MixerInterop.mixerGetLineControls(mixerHandle, ref mixerLineControls, MixerFlags.Mixer | mixerHandleType);
					if (mmResult != 0)
					{
						throw new MmException(mmResult, "mixerGetLineControls");
					}
					for (int i = 0; i < mixerLineControls.cControls; i++)
					{
						MixerControl item = GetMixerControl(controlId: MarshalHelpers.PtrToStructure<MixerInterop.MIXERCONTROL>((IntPtr)(intPtr.ToInt64() + num * i)).dwControlID, mixerHandle: mixerHandle, nLineId: mixerLine.LineId, nChannels: mixerLine.Channels, mixerFlags: mixerHandleType);
						list.Add(item);
					}
					return list;
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return list;
		}

		public static MixerControl GetMixerControl(IntPtr mixerHandle, int nLineId, int controlId, int nChannels, MixerFlags mixerFlags)
		{
			MixerInterop.MIXERLINECONTROLS mixerLineControls = default(MixerInterop.MIXERLINECONTROLS);
			MixerInterop.MIXERCONTROL mIXERCONTROL = default(MixerInterop.MIXERCONTROL);
			IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(mIXERCONTROL));
			mixerLineControls.cbStruct = Marshal.SizeOf(mixerLineControls);
			mixerLineControls.cControls = 1;
			mixerLineControls.dwControlID = controlId;
			mixerLineControls.cbmxctrl = Marshal.SizeOf(mIXERCONTROL);
			mixerLineControls.pamxctrl = intPtr;
			mixerLineControls.dwLineID = nLineId;
			MmResult mmResult = MixerInterop.mixerGetLineControls(mixerHandle, ref mixerLineControls, MixerFlags.ListText | mixerFlags);
			if (mmResult != 0)
			{
				Marshal.FreeCoTaskMem(intPtr);
				throw new MmException(mmResult, "mixerGetLineControls");
			}
			mIXERCONTROL = MarshalHelpers.PtrToStructure<MixerInterop.MIXERCONTROL>(mixerLineControls.pamxctrl);
			Marshal.FreeCoTaskMem(intPtr);
			if (IsControlBoolean(mIXERCONTROL.dwControlType))
			{
				return new BooleanMixerControl(mIXERCONTROL, mixerHandle, mixerFlags, nChannels);
			}
			if (IsControlSigned(mIXERCONTROL.dwControlType))
			{
				return new SignedMixerControl(mIXERCONTROL, mixerHandle, mixerFlags, nChannels);
			}
			if (IsControlUnsigned(mIXERCONTROL.dwControlType))
			{
				return new UnsignedMixerControl(mIXERCONTROL, mixerHandle, mixerFlags, nChannels);
			}
			if (IsControlListText(mIXERCONTROL.dwControlType))
			{
				return new ListTextMixerControl(mIXERCONTROL, mixerHandle, mixerFlags, nChannels);
			}
			if (IsControlCustom(mIXERCONTROL.dwControlType))
			{
				return new CustomMixerControl(mIXERCONTROL, mixerHandle, mixerFlags, nChannels);
			}
			throw new InvalidOperationException($"Unknown mixer control type {mIXERCONTROL.dwControlType}");
		}

		protected void GetControlDetails()
		{
			mixerControlDetails.cbStruct = Marshal.SizeOf(mixerControlDetails);
			mixerControlDetails.dwControlID = mixerControl.dwControlID;
			if (IsCustom)
			{
				mixerControlDetails.cChannels = 0;
			}
			else if ((mixerControl.fdwControl & (true ? 1u : 0u)) != 0)
			{
				mixerControlDetails.cChannels = 1;
			}
			else
			{
				mixerControlDetails.cChannels = nChannels;
			}
			if ((mixerControl.fdwControl & 2u) != 0)
			{
				mixerControlDetails.hwndOwner = (IntPtr)mixerControl.cMultipleItems;
			}
			else if (IsCustom)
			{
				mixerControlDetails.hwndOwner = IntPtr.Zero;
			}
			else
			{
				mixerControlDetails.hwndOwner = IntPtr.Zero;
			}
			if (IsBoolean)
			{
				mixerControlDetails.cbDetails = MarshalHelpers.SizeOf<MixerInterop.MIXERCONTROLDETAILS_BOOLEAN>();
			}
			else if (IsListText)
			{
				mixerControlDetails.cbDetails = MarshalHelpers.SizeOf<MixerInterop.MIXERCONTROLDETAILS_LISTTEXT>();
			}
			else if (IsSigned)
			{
				mixerControlDetails.cbDetails = MarshalHelpers.SizeOf<MixerInterop.MIXERCONTROLDETAILS_SIGNED>();
			}
			else if (IsUnsigned)
			{
				mixerControlDetails.cbDetails = MarshalHelpers.SizeOf<MixerInterop.MIXERCONTROLDETAILS_UNSIGNED>();
			}
			else
			{
				mixerControlDetails.cbDetails = mixerControl.Metrics.customData;
			}
			int num = mixerControlDetails.cbDetails * mixerControlDetails.cChannels;
			if ((mixerControl.fdwControl & 2u) != 0)
			{
				num *= (int)mixerControl.cMultipleItems;
			}
			IntPtr intPtr = Marshal.AllocCoTaskMem(num);
			mixerControlDetails.paDetails = intPtr;
			MmResult mmResult = MixerInterop.mixerGetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Mixer | mixerHandleType);
			if (mmResult == MmResult.NoError)
			{
				GetDetails(mixerControlDetails.paDetails);
			}
			Marshal.FreeCoTaskMem(intPtr);
			if (mmResult != 0)
			{
				throw new MmException(mmResult, "mixerGetControlDetails");
			}
		}

		protected abstract void GetDetails(IntPtr pDetails);

		private static bool IsControlBoolean(MixerControlType controlType)
		{
			switch (controlType)
			{
			case MixerControlType.BooleanMeter:
			case MixerControlType.Boolean:
			case MixerControlType.OnOff:
			case MixerControlType.Mute:
			case MixerControlType.Mono:
			case MixerControlType.Loudness:
			case MixerControlType.StereoEnhance:
			case MixerControlType.Button:
			case MixerControlType.SingleSelect:
			case MixerControlType.Mux:
			case MixerControlType.MultipleSelect:
			case MixerControlType.Mixer:
				return true;
			default:
				return false;
			}
		}

		private static bool IsControlListText(MixerControlType controlType)
		{
			if (controlType == MixerControlType.Equalizer || (uint)(controlType - 1879113728) <= 1u || (uint)(controlType - 1895890944) <= 1u)
			{
				return true;
			}
			return false;
		}

		private static bool IsControlSigned(MixerControlType controlType)
		{
			switch (controlType)
			{
			case MixerControlType.SignedMeter:
			case MixerControlType.PeakMeter:
			case MixerControlType.Signed:
			case MixerControlType.Decibels:
			case MixerControlType.Slider:
			case MixerControlType.Pan:
			case MixerControlType.QSoundPan:
				return true;
			default:
				return false;
			}
		}

		private static bool IsControlUnsigned(MixerControlType controlType)
		{
			switch (controlType)
			{
			case MixerControlType.UnsignedMeter:
			case MixerControlType.Unsigned:
			case MixerControlType.Percent:
			case MixerControlType.Fader:
			case MixerControlType.Volume:
			case MixerControlType.Bass:
			case MixerControlType.Treble:
			case MixerControlType.Equalizer:
			case MixerControlType.MicroTime:
			case MixerControlType.MilliTime:
				return true;
			default:
				return false;
			}
		}

		private static bool IsControlCustom(MixerControlType controlType)
		{
			return controlType == MixerControlType.Custom;
		}

		public override string ToString()
		{
			return $"{Name} {ControlType}";
		}
	}
	[Flags]
	internal enum MixerControlClass
	{
		Custom = 0,
		Meter = 0x10000000,
		Switch = 0x20000000,
		Number = 0x30000000,
		Slider = 0x40000000,
		Fader = 0x50000000,
		Time = 0x60000000,
		List = 0x70000000,
		Mask = 0x70000000
	}
	[Flags]
	internal enum MixerControlSubclass
	{
		SwitchBoolean = 0,
		SwitchButton = 0x1000000,
		MeterPolled = 0,
		TimeMicrosecs = 0,
		TimeMillisecs = 0x1000000,
		ListSingle = 0,
		ListMultiple = 0x1000000,
		Mask = 0xF000000
	}
	[Flags]
	internal enum MixerControlUnits
	{
		Custom = 0,
		Boolean = 0x10000,
		Signed = 0x20000,
		Unsigned = 0x30000,
		Decibels = 0x40000,
		Percent = 0x50000,
		Mask = 0xFF0000
	}
	public enum MixerControlType
	{
		Custom = 0,
		BooleanMeter = 268500992,
		SignedMeter = 268566528,
		PeakMeter = 268566529,
		UnsignedMeter = 268632064,
		Boolean = 536936448,
		OnOff = 536936449,
		Mute = 536936450,
		Mono = 536936451,
		Loudness = 536936452,
		StereoEnhance = 536936453,
		Button = 553713664,
		Decibels = 805568512,
		Signed = 805437440,
		Unsigned = 805502976,
		Percent = 805634048,
		Slider = 1073872896,
		Pan = 1073872897,
		QSoundPan = 1073872898,
		Fader = 1342373888,
		Volume = 1342373889,
		Bass = 1342373890,
		Treble = 1342373891,
		Equalizer = 1342373892,
		SingleSelect = 1879113728,
		Mux = 1879113729,
		MultipleSelect = 1895890944,
		Mixer = 1895890945,
		MicroTime = 1610809344,
		MilliTime = 1627586560
	}
	[Flags]
	public enum MixerFlags
	{
		Handle = int.MinValue,
		Mixer = 0,
		MixerHandle = int.MinValue,
		WaveOut = 0x10000000,
		WaveOutHandle = -1879048192,
		WaveIn = 0x20000000,
		WaveInHandle = -1610612736,
		MidiOut = 0x30000000,
		MidiOutHandle = -1342177280,
		MidiIn = 0x40000000,
		MidiInHandle = -1073741824,
		Aux = 0x50000000,
		Value = 0,
		ListText = 1,
		QueryMask = 0xF,
		All = 0,
		OneById = 1,
		OneByType = 2,
		GetLineInfoOfDestination = 0,
		GetLineInfoOfSource = 1,
		GetLineInfoOfLineId = 2,
		GetLineInfoOfComponentType = 3,
		GetLineInfoOfTargetType = 4,
		GetLineInfoOfQueryMask = 0xF
	}
	internal class MixerInterop
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		public struct MIXERCONTROLDETAILS
		{
			public int cbStruct;

			public int dwControlID;

			public int cChannels;

			public IntPtr hwndOwner;

			public int cbDetails;

			public IntPtr paDetails;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct MIXERCAPS
		{
			public ushort wMid;

			public ushort wPid;

			public uint vDriverVersion;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;

			public uint fdwSupport;

			public uint cDestinations;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct MIXERLINECONTROLS
		{
			public int cbStruct;

			public int dwLineID;

			public int dwControlID;

			public int cControls;

			public int cbmxctrl;

			public IntPtr pamxctrl;
		}

		[Flags]
		public enum MIXERLINE_LINEF
		{
			MIXERLINE_LINEF_ACTIVE = 1,
			MIXERLINE_LINEF_DISCONNECTED = 0x8000,
			MIXERLINE_LINEF_SOURCE = int.MinValue
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct MIXERLINE
		{
			public int cbStruct;

			public int dwDestination;

			public int dwSource;

			public int dwLineID;

			public MIXERLINE_LINEF fdwLine;

			public IntPtr dwUser;

			public MixerLineComponentType dwComponentType;

			public int cChannels;

			public int cConnections;

			public int cControls;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string szShortName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szName;

			public uint dwType;

			public uint dwDeviceID;

			public ushort wMid;

			public ushort wPid;

			public uint vDriverVersion;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Bounds
		{
			public int minimum;

			public int maximum;

			public int reserved2;

			public int reserved3;

			public int reserved4;

			public int reserved5;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Metrics
		{
			public int step;

			public int customData;

			public int reserved2;

			public int reserved3;

			public int reserved4;

			public int reserved5;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct MIXERCONTROL
		{
			public uint cbStruct;

			public int dwControlID;

			public MixerControlType dwControlType;

			public uint fdwControl;

			public uint cMultipleItems;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string szShortName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szName;

			public Bounds Bounds;

			public Metrics Metrics;
		}

		public struct MIXERCONTROLDETAILS_BOOLEAN
		{
			public int fValue;
		}

		public struct MIXERCONTROLDETAILS_SIGNED
		{
			public int lValue;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct MIXERCONTROLDETAILS_LISTTEXT
		{
			public uint dwParam1;

			public uint dwParam2;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szName;
		}

		public struct MIXERCONTROLDETAILS_UNSIGNED
		{
			public uint dwValue;
		}

		public const uint MIXERCONTROL_CONTROLF_UNIFORM = 1u;

		public const uint MIXERCONTROL_CONTROLF_MULTIPLE = 2u;

		public const uint MIXERCONTROL_CONTROLF_DISABLED = 2147483648u;

		public const int MAXPNAMELEN = 32;

		public const int MIXER_SHORT_NAME_CHARS = 16;

		public const int MIXER_LONG_NAME_CHARS = 64;

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern int mixerGetNumDevs();

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerOpen(out IntPtr hMixer, int uMxId, IntPtr dwCallback, IntPtr dwInstance, MixerFlags dwOpenFlags);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerClose(IntPtr hMixer);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerGetControlDetails(IntPtr hMixer, ref MIXERCONTROLDETAILS mixerControlDetails, MixerFlags dwDetailsFlags);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerGetDevCaps(IntPtr nMixerID, ref MIXERCAPS mixerCaps, int mixerCapsSize);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerGetID(IntPtr hMixer, out int mixerID, MixerFlags dwMixerIDFlags);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerGetLineControls(IntPtr hMixer, ref MIXERLINECONTROLS mixerLineControls, MixerFlags dwControlFlags);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerGetLineInfo(IntPtr hMixer, ref MIXERLINE mixerLine, MixerFlags dwInfoFlags);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerMessage(IntPtr hMixer, uint nMessage, IntPtr dwParam1, IntPtr dwParam2);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		public static extern MmResult mixerSetControlDetails(IntPtr hMixer, ref MIXERCONTROLDETAILS mixerControlDetails, MixerFlags dwDetailsFlags);
	}
	public class MixerLine
	{
		private MixerInterop.MIXERLINE mixerLine;

		private IntPtr mixerHandle;

		private MixerFlags mixerHandleType;

		public string Name => mixerLine.szName;

		public string ShortName => mixerLine.szShortName;

		public int LineId => mixerLine.dwLineID;

		public MixerLineComponentType ComponentType => mixerLine.dwComponentType;

		public string TypeDescription => mixerLine.dwComponentType switch
		{
			MixerLineComponentType.DestinationUndefined => "Undefined Destination", 
			MixerLineComponentType.DestinationDigital => "Digital Destination", 
			MixerLineComponentType.DestinationLine => "Line Level Destination", 
			MixerLineComponentType.DestinationMonitor => "Monitor Destination", 
			MixerLineComponentType.DestinationSpeakers => "Speakers Destination", 
			MixerLineComponentType.DestinationHeadphones => "Headphones Destination", 
			MixerLineComponentType.DestinationTelephone => "Telephone Destination", 
			MixerLineComponentType.DestinationWaveIn => "Wave Input Destination", 
			MixerLineComponentType.DestinationVoiceIn => "Voice Recognition Destination", 
			MixerLineComponentType.SourceUndefined => "Undefined Source", 
			MixerLineComponentType.SourceDigital => "Digital Source", 
			MixerLineComponentType.SourceLine => "Line Level Source", 
			MixerLineComponentType.SourceMicrophone => "Microphone Source", 
			MixerLineComponentType.SourceSynthesizer => "Synthesizer Source", 
			MixerLineComponentType.SourceCompactDisc => "Compact Disk Source", 
			MixerLineComponentType.SourceTelephone => "Telephone Source", 
			MixerLineComponentType.SourcePcSpeaker => "PC Speaker Source", 
			MixerLineComponentType.SourceWaveOut => "Wave Out Source", 
			MixerLineComponentType.SourceAuxiliary => "Auxiliary Source", 
			MixerLineComponentType.SourceAnalog => "Analog Source", 
			_ => "Invalid Component Type", 
		};

		public int Channels => mixerLine.cChannels;

		public int SourceCount => mixerLine.cConnections;

		public int ControlsCount => mixerLine.cControls;

		public bool IsActive => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_ACTIVE) != 0;

		public bool IsDisconnected => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_DISCONNECTED) != 0;

		public bool IsSource => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_SOURCE) != 0;

		public IEnumerable<MixerControl> Controls => MixerControl.GetMixerControls(mixerHandle, this, mixerHandleType);

		public IEnumerable<MixerLine> Sources
		{
			get
			{
				for (int source = 0; source < SourceCount; source++)
				{
					yield return GetSource(source);
				}
			}
		}

		public string TargetName => mixerLine.szPname;

		public MixerLine(IntPtr mixerHandle, int destinationIndex, MixerFlags mixerHandleType)
		{
			this.mixerHandle = mixerHandle;
			this.mixerHandleType = mixerHandleType;
			mixerLine = default(MixerInterop.MIXERLINE);
			mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
			mixerLine.dwDestination = destinationIndex;
			MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.Mixer), "mixerGetLineInfo");
		}

		public MixerLine(IntPtr mixerHandle, int destinationIndex, int sourceIndex, MixerFlags mixerHandleType)
		{
			this.mixerHandle = mixerHandle;
			this.mixerHandleType = mixerHandleType;
			mixerLine = default(MixerInterop.MIXERLINE);
			mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
			mixerLine.dwDestination = destinationIndex;
			mixerLine.dwSource = sourceIndex;
			MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.ListText), "mixerGetLineInfo");
		}

		public static int GetMixerIdForWaveIn(int waveInDevice)
		{
			int mixerID = -1;
			MmException.Try(MixerInterop.mixerGetID((IntPtr)waveInDevice, out mixerID, MixerFlags.WaveIn), "mixerGetID");
			return mixerID;
		}

		public MixerLine GetSource(int sourceIndex)
		{
			if (sourceIndex < 0 || sourceIndex >= SourceCount)
			{
				throw new ArgumentOutOfRangeException("sourceIndex");
			}
			return new MixerLine(mixerHandle, mixerLine.dwDestination, sourceIndex, mixerHandleType);
		}

		public override string ToString()
		{
			return $"{Name} {TypeDescription} ({ControlsCount} controls, ID={mixerLine.dwLineID})";
		}
	}
	public enum MixerLineComponentType
	{
		DestinationUndefined = 0,
		DestinationDigital = 1,
		DestinationLine = 2,
		DestinationMonitor = 3,
		DestinationSpeakers = 4,
		DestinationHeadphones = 5,
		DestinationTelephone = 6,
		DestinationWaveIn = 7,
		DestinationVoiceIn = 8,
		SourceUndefined = 4096,
		SourceDigital = 4097,
		SourceLine = 4098,
		SourceMicrophone = 4099,
		SourceSynthesizer = 4100,
		SourceCompactDisc = 4101,
		SourceTelephone = 4102,
		SourcePcSpeaker = 4103,
		SourceWaveOut = 4104,
		SourceAuxiliary = 4105,
		SourceAnalog = 4106
	}
	public class SignedMixerControl : MixerControl
	{
		private MixerInterop.MIXERCONTROLDETAILS_SIGNED signedDetails;

		public int Value
		{
			get
			{
				GetControlDetails();
				return signedDetails.lValue;
			}
			set
			{
				signedDetails.lValue = value;
				mixerControlDetails.paDetails = Marshal.AllocHGlobal(Marshal.SizeOf(signedDetails));
				Marshal.StructureToPtr(signedDetails, mixerControlDetails.paDetails, fDeleteOld: false);
				MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Mixer | mixerHandleType), "mixerSetControlDetails");
				Marshal.FreeHGlobal(mixerControlDetails.paDetails);
			}
		}

		public int MinValue => mixerControl.Bounds.minimum;

		public int MaxValue => mixerControl.Bounds.maximum;

		public double Percent
		{
			get
			{
				return 100.0 * (double)(Value - MinValue) / (double)(MaxValue - MinValue);
			}
			set
			{
				Value = (int)((double)MinValue + value / 100.0 * (double)(MaxValue - MinValue));
			}
		}

		internal SignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
		{
			base.mixerControl = mixerControl;
			base.mixerHandle = mixerHandle;
			base.mixerHandleType = mixerHandleType;
			base.nChannels = nChannels;
			mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
			GetControlDetails();
		}

		protected override void GetDetails(IntPtr pDetails)
		{
			signedDetails = MarshalHelpers.PtrToStructure<MixerInterop.MIXERCONTROLDETAILS_SIGNED>(mixerControlDetails.paDetails);
		}

		public override string ToString()
		{
			return $"{base.ToString()} {Percent}%";
		}
	}
	public class UnsignedMixerControl : MixerControl
	{
		private MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[] unsignedDetails;

		public uint Value
		{
			get
			{
				GetControlDetails();
				return unsignedDetails[0].dwValue;
			}
			set
			{
				int num = Marshal.SizeOf(unsignedDetails[0]);
				mixerControlDetails.paDetails = Marshal.AllocHGlobal(num * nChannels);
				for (int i = 0; i < nChannels; i++)
				{
					unsignedDetails[i].dwValue = value;
					long num2 = mixerControlDetails.paDetails.ToInt64() + num * i;
					Marshal.StructureToPtr(unsignedDetails[i], (IntPtr)num2, fDeleteOld: false);
				}
				MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Mixer | mixerHandleType), "mixerSetControlDetails");
				Marshal.FreeHGlobal(mixerControlDetails.paDetails);
			}
		}

		public uint MinValue => (uint)mixerControl.Bounds.minimum;

		public uint MaxValue => (uint)mixerControl.Bounds.maximum;

		public double Percent
		{
			get
			{
				return 100.0 * (double)(Value - MinValue) / (double)(MaxValue - MinValue);
			}
			set
			{
				Value = (uint)((double)MinValue + value / 100.0 * (double)(MaxValue - MinValue));
			}
		}

		internal UnsignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
		{
			base.mixerControl = mixerControl;
			base.mixerHandle = mixerHandle;
			base.mixerHandleType = mixerHandleType;
			base.nChannels = nChannels;
			mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
			GetControlDetails();
		}

		protected override void GetDetails(IntPtr pDetails)
		{
			unsignedDetails = new MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[nChannels];
			for (int i = 0; i < nChannels; i++)
			{
				unsignedDetails[i] = MarshalHelpers.PtrToStructure<MixerInterop.MIXERCONTROLDETAILS_UNSIGNED>(mixerControlDetails.paDetails);
			}
		}

		public override string ToString()
		{
			return $"{base.ToString()} {Percent}%";
		}
	}
}
namespace NAudio.Midi
{
	public class ChannelAfterTouchEvent : MidiEvent
	{
		private byte afterTouchPressure;

		public int AfterTouchPressure
		{
			get
			{
				return afterTouchPressure;
			}
			set
			{
				if (value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException("value", "After touch pressure must be in the range 0-127");
				}
				afterTouchPressure = (byte)value;
			}
		}

		public ChannelAfterTouchEvent(BinaryReader br)
		{
			afterTouchPressure = br.ReadByte();
			if ((afterTouchPressure & 0x80u) != 0)
			{
				throw new FormatException("Invalid afterTouchPressure");
			}
		}

		public ChannelAfterTouchEvent(long absoluteTime, int channel, int afterTouchPressure)
			: base(absoluteTime, channel, MidiCommandCode.ChannelAfterTouch)
		{
			AfterTouchPressure = afterTouchPressure;
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(afterTouchPressure);
		}
	}
	public class ControlChangeEvent : MidiEvent
	{
		private MidiController controller;

		private byte controllerValue;

		public MidiController Controller
		{
			get
			{
				return controller;
			}
			set
			{
				if ((int)value < 0 || (int)value > 127)
				{
					throw new ArgumentOutOfRangeException("value", "Controller number must be in the range 0-127");
				}
				controller = value;
			}
		}

		public int ControllerValue
		{
			get
			{
				return controllerValue;
			}
			set
			{
				if (value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException("value", "Controller Value must be in the range 0-127");
				}
				controllerValue = (byte)value;
			}
		}

		public ControlChangeEvent(BinaryReader br)
		{
			byte b = br.ReadByte();
			controllerValue = br.ReadByte();
			if ((b & 0x80u) != 0)
			{
				throw new InvalidDataException("Invalid controller");
			}
			controller = (MidiController)b;
			if ((controllerValue & 0x80u) != 0)
			{
				throw new InvalidDataException($"Invalid controllerValue {controllerValue} for controller {controller}, Pos 0x{br.BaseStream.Position:X}");
			}
		}

		public ControlChangeEvent(long absoluteTime, int channel, MidiController controller, int controllerValue)
			: base(absoluteTime, channel, MidiCommandCode.ControlChange)
		{
			Controller = controller;
			ControllerValue = controllerValue;
		}

		public override string ToString()
		{
			return $"{base.ToString()} Controller {controller} Value {controllerValue}";
		}

		public override int GetAsShortMessage()
		{
			byte b = (byte)controller;
			return base.GetAsShortMessage() + (b << 8) + (controllerValue << 16);
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)controller);
			writer.Write(controllerValue);
		}
	}
	public class KeySignatureEvent : MetaEvent
	{
		private readonly byte sharpsFlats;

		private readonly byte majorMinor;

		public int SharpsFlats => (sbyte)sharpsFlats;

		public int MajorMinor => majorMinor;

		public KeySignatureEvent(BinaryReader br, int length)
		{
			if (length != 2)
			{
				throw new FormatException("Invalid key signature length");
			}
			sharpsFlats = br.ReadByte();
			majorMinor = br.ReadByte();
		}

		public KeySignatureEvent(int sharpsFlats, int majorMinor, long absoluteTime)
			: base(MetaEventType.KeySignature, 2, absoluteTime)
		{
			this.sharpsFlats = (byte)sharpsFlats;
			this.majorMinor = (byte)majorMinor;
		}

		public override MidiEvent Clone()
		{
			return (KeySignatureEvent)MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{base.ToString()} {SharpsFlats} {majorMinor}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(sharpsFlats);
			writer.Write(majorMinor);
		}
	}
	public class MetaEvent : MidiEvent
	{
		private MetaEventType metaEvent;

		internal int metaDataLength;

		public MetaEventType MetaEventType => metaEvent;

		protected MetaEvent()
		{
		}

		public MetaEvent(MetaEventType metaEventType, int metaDataLength, long absoluteTime)
			: base(absoluteTime, 1, MidiCommandCode.MetaEvent)
		{
			metaEvent = metaEventType;
			this.metaDataLength = metaDataLength;
		}

		public override MidiEvent Clone()
		{
			return new MetaEvent(metaEvent, metaDataLength, base.AbsoluteTime);
		}

		public static MetaEvent ReadMetaEvent(BinaryReader br)
		{
			MetaEventType metaEventType = (MetaEventType)br.ReadByte();
			int num = MidiEvent.ReadVarInt(br);
			MetaEvent metaEvent = new MetaEvent();
			if (metaEventType <= MetaEventType.SetTempo)
			{
				if (metaEventType <= MetaEventType.DeviceName)
				{
					if (metaEventType != 0)
					{
						if (metaEventType - 1 > MetaEventType.ProgramName)
						{
							goto IL_00a6;
						}
						metaEvent = new TextEvent(br, num);
					}
					else
					{
						metaEvent = new TrackSequenceNumberEvent(br, num);
					}
				}
				else if (metaEventType != MetaEventType.EndTrack)
				{
					if (metaEventType != MetaEventType.SetTempo)
					{
						goto IL_00a6;
					}
					metaEvent = new TempoEvent(br, num);
				}
				else if (num != 0)
				{
					throw new FormatException("End track length");
				}
			}
			else if (metaEventType <= MetaEventType.TimeSignature)
			{
				if (metaEventType != MetaEventType.SmpteOffset)
				{
					if (metaEventType != MetaEventType.TimeSignature)
					{
						goto IL_00a6;
					}
					metaEvent = new TimeSignatureEvent(br, num);
				}
				else
				{
					metaEvent = new SmpteOffsetEvent(br, num);
				}
			}
			else if (metaEventType != MetaEventType.KeySignature)
			{
				if (metaEventType != MetaEventType.SequencerSpecific)
				{
					goto IL_00a6;
				}
				metaEvent = new SequencerSpecificEvent(br, num);
			}
			else
			{
				metaEvent = new KeySignatureEvent(br, num);
			}
			metaEvent.metaEvent = metaEventType;
			metaEvent.metaDataLength = num;
			return metaEvent;
			IL_00a6:
			byte[] array = br.ReadBytes(num);
			if (array.Length != num)
			{
				throw new FormatException("Failed to read metaevent's data fully");
			}
			return new RawMetaEvent(metaEventType, 0L, array);
		}

		public override string ToString()
		{
			return $"{base.AbsoluteTime} {metaEvent}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)metaEvent);
			MidiEvent.WriteVarInt(writer, metaDataLength);
		}
	}
	public enum MetaEventType : byte
	{
		TrackSequenceNumber = 0,
		TextEvent = 1,
		Copyright = 2,
		SequenceTrackName = 3,
		TrackInstrumentName = 4,
		Lyric = 5,
		Marker = 6,
		CuePoint = 7,
		ProgramName = 8,
		DeviceName = 9,
		MidiChannel = 32,
		MidiPort = 33,
		EndTrack = 47,
		SetTempo = 81,
		SmpteOffset = 84,
		TimeSignature = 88,
		KeySignature = 89,
		SequencerSpecific = 127
	}
	public enum MidiCommandCode : byte
	{
		NoteOff = 128,
		NoteOn = 144,
		KeyAfterTouch = 160,
		ControlChange = 176,
		PatchChange = 192,
		ChannelAfterTouch = 208,
		PitchWheelChange = 224,
		Sysex = 240,
		Eox = 247,
		TimingClock = 248,
		StartSequence = 250,
		ContinueSequence = 251,
		StopSequence = 252,
		AutoSensing = 254,
		MetaEvent = byte.MaxValue
	}
	public enum MidiController : byte
	{
		BankSelect = 0,
		Modulation = 1,
		BreathController = 2,
		FootController = 4,
		MainVolume = 7,
		Pan = 10,
		Expression = 11,
		BankSelectLsb = 32,
		Sustain = 64,
		Portamento = 65,
		Sostenuto = 66,
		SoftPedal = 67,
		LegatoFootswitch = 68,
		ResetAllControllers = 121,
		AllNotesOff = 123
	}
	public class MidiEvent : ICloneable
	{
		private MidiCommandCode commandCode;

		private int channel;

		private int deltaTime;

		private long absoluteTime;

		public virtual int Channel
		{
			get
			{
				return channel;
			}
			set
			{
				if (value < 1 || value > 16)
				{
					throw new ArgumentOutOfRangeException("value", value, $"Channel must be 1-16 (Got {value})");
				}
				channel = value;
			}
		}

		public int DeltaTime => deltaTime;

		public long AbsoluteTime
		{
			get
			{
				return absoluteTime;
			}
			set
			{
				absoluteTime = value;
			}
		}

		public MidiCommandCode CommandCode => commandCode;

		public static MidiEvent FromRawMessage(int rawMessage)
		{
			long num = 0L;
			int num2 = rawMessage & 0xFF;
			int num3 = (rawMessage >> 8) & 0xFF;
			int num4 = (rawMessage >> 16) & 0xFF;
			int num5 = 1;
			MidiCommandCode midiCommandCode;
			if ((num2 & 0xF0) == 240)
			{
				midiCommandCode = (MidiCommandCode)num2;
			}
			else
			{
				midiCommandCode = (MidiCommandCode)((uint)num2 & 0xF0u);
				num5 = (num2 & 0xF) + 1;
			}
			switch (midiCommandCode)
			{
			case MidiCommandCode.NoteOff:
			case MidiCommandCode.NoteOn:
			case MidiCommandCode.KeyAfterTouch:
				if (num4 > 0 && midiCommandCode == MidiCommandCode.NoteOn)
				{
					return new NoteOnEvent(num, num5, num3, num4, 0);
				}
				return new NoteEvent(num, num5, midiCommandCode, num3, num4);
			case MidiCommandCode.ControlChange:
				return new ControlChangeEvent(num, num5, (MidiController)num3, num4);
			case MidiCommandCode.PatchChange:
				return new PatchChangeEvent(num, num5, num3);
			case MidiCommandCode.ChannelAfterTouch:
				return new ChannelAfterTouchEvent(num, num5, num3);
			case MidiCommandCode.PitchWheelChange:
				return new PitchWheelChangeEvent(num, num5, num3 + (num4 << 7));
			case MidiCommandCode.TimingClock:
			case MidiCommandCode.StartSequence:
			case MidiCommandCode.ContinueSequence:
			case MidiCommandCode.StopSequence:
			case MidiCommandCode.AutoSensing:
				return new MidiEvent(num, num5, midiCommandCode);
			default:
				throw new FormatException($"Unsupported MIDI Command Code for Raw Message {midiCommandCode}");
			}
		}

		public static MidiEvent ReadNextEvent(BinaryReader br, MidiEvent previous)
		{
			int num = ReadVarInt(br);
			int num2 = 1;
			byte b = br.ReadByte();
			MidiCommandCode midiCommandCode;
			if ((b & 0x80) == 0)
			{
				midiCommandCode = previous.CommandCode;
				num2 = previous.Channel;
				br.BaseStream.Position--;
			}
			else if ((b & 0xF0) == 240)
			{
				midiCommandCode = (MidiCommandCode)b;
			}
			else
			{
				midiCommandCode = (MidiCommandCode)(b & 0xF0u);
				num2 = (b & 0xF) + 1;
			}
			MidiEvent midiEvent;
			switch (midiCommandCode)
			{
			case MidiCommandCode.NoteOn:
				midiEvent = new NoteOnEvent(br);
				break;
			case MidiCommandCode.NoteOff:
			case MidiCommandCode.KeyAfterTouch:
				midiEvent = new NoteEvent(br);
				break;
			case MidiCommandCode.ControlChange:
				midiEvent = new ControlChangeEvent(br);
				break;
			case MidiCommandCode.PatchChange:
				midiEvent = new PatchChangeEvent(br);
				break;
			case MidiCommandCode.ChannelAfterTouch:
				midiEvent = new ChannelAfterTouchEvent(br);
				break;
			case MidiCommandCode.PitchWheelChange:
				midiEvent = new PitchWheelChangeEvent(br);
				break;
			case MidiCommandCode.TimingClock:
			case MidiCommandCode.StartSequence:
			case MidiCommandCode.ContinueSequence:
			case MidiCommandCode.StopSequence:
				midiEvent = new MidiEvent();
				break;
			case MidiCommandCode.Sysex:
				midiEvent = SysexEvent.ReadSysexEvent(br);
				break;
			case MidiCommandCode.MetaEvent:
				midiEvent = MetaEvent.ReadMetaEvent(br);
				break;
			default:
				throw new FormatException($"Unsupported MIDI Command Code {(byte)midiCommandCode:X2}");
			}
			midiEvent.channel = num2;
			midiEvent.deltaTime = num;
			midiEvent.commandCode = midiCommandCode;
			return midiEvent;
		}

		public virtual int GetAsShortMessage()
		{
			return channel - 1 + (int)commandCode;
		}

		protected MidiEvent()
		{
		}

		public MidiEvent(long absoluteTime, int channel, MidiCommandCode commandCode)
		{
			this.absoluteTime = absoluteTime;
			Channel = channel;
			this.commandCode = commandCode;
		}

		public virtual MidiEvent Clone()
		{
			return (MidiEvent)MemberwiseClone();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public static bool IsNoteOff(MidiEvent midiEvent)
		{
			if (midiEvent != null)
			{
				if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
				{
					return ((NoteEvent)midiEvent).Velocity == 0;
				}
				return midiEvent.CommandCode == MidiCommandCode.NoteOff;
			}
			return false;
		}

		public static bool IsNoteOn(MidiEvent midiEvent)
		{
			if (midiEvent != null && midiEvent.CommandCode == MidiCommandCode.NoteOn)
			{
				return ((NoteEvent)midiEvent).Velocity > 0;
			}
			return false;
		}

		public static bool IsEndTrack(MidiEvent midiEvent)
		{
			if (midiEvent != null && midiEvent is MetaEvent metaEvent)
			{
				return metaEvent.MetaEventType == MetaEventType.EndTrack;
			}
			return false;
		}

		public override string ToString()
		{
			if ((int)commandCode >= 240)
			{
				return $"{absoluteTime} {commandCode}";
			}
			return $"{absoluteTime} {commandCode} Ch: {channel}";
		}

		public static int ReadVarInt(BinaryReader br)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				byte b = br.ReadByte();
				num <<= 7;
				num += b & 0x7F;
				if ((b & 0x80) == 0)
				{
					return num;
				}
			}
			throw new FormatException("Invalid Var Int");
		}

		public static void WriteVarInt(BinaryWriter writer, int value)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", value, "Cannot write a negative Var Int");
			}
			if (value > 268435455)
			{
				throw new ArgumentOutOfRangeException("value", value, "Maximum allowed Var Int is 0x0FFFFFFF");
			}
			int num = 0;
			byte[] array = new byte[4];
			do
			{
				array[num++] = (byte)((uint)value & 0x7Fu);
				value >>= 7;
			}
			while (value > 0);
			while (num > 0)
			{
				num--;
				if (num > 0)
				{
					writer.Write((byte)(array[num] | 0x80u));
				}
				else
				{
					writer.Write(array[num]);
				}
			}
		}

		public virtual void Export(ref long absoluteTime, BinaryWriter writer)
		{
			if (this.absoluteTime < absoluteTime)
			{
				throw new FormatException("Can't export unsorted MIDI events");
			}
			WriteVarInt(writer, (int)(this.absoluteTime - absoluteTime));
			absoluteTime = this.absoluteTime;
			int num = (int)commandCode;
			if (commandCode != MidiCommandCode.MetaEvent)
			{
				num += channel - 1;
			}
			writer.Write((byte)num);
		}
	}
	public class MidiEventCollection : IEnumerable<IList<MidiEvent>>, IEnumerable
	{
		private int midiFileType;

		private readonly List<IList<MidiEvent>> trackEvents;

		public int Tracks => trackEvents.Count;

		public long StartAbsoluteTime { get; set; }

		public int DeltaTicksPerQuarterNote { get; }

		public IList<MidiEvent> this[int trackNumber] => trackEvents[trackNumber];

		public int MidiFileType
		{
			get
			{
				return midiFileType;
			}
			set
			{
				if (midiFileType != value)
				{
					midiFileType = value;
					if (value == 0)
					{
						FlattenToOneTrack();
					}
					else
					{
						ExplodeToManyTracks();
					}
				}
			}
		}

		public MidiEventCollection(int midiFileType, int deltaTicksPerQuarterNote)
		{
			this.midiFileType = midiFileType;
			DeltaTicksPerQuarterNote = deltaTicksPerQuarterNote;
			StartAbsoluteTime = 0L;
			trackEvents = new List<IList<MidiEvent>>();
		}

		public IList<MidiEvent> GetTrackEvents(int trackNumber)
		{
			return trackEvents[trackNumber];
		}

		public IList<MidiEvent> AddTrack()
		{
			return AddTrack(null);
		}

		public IList<MidiEvent> AddTrack(IList<MidiEvent> initialEvents)
		{
			List<MidiEvent> list = new List<MidiEvent>();
			if (initialEvents != null)
			{
				list.AddRange(initialEvents);
			}
			trackEvents.Add(list);
			return list;
		}

		public void RemoveTrack(int track)
		{
			trackEvents.RemoveAt(track);
		}

		public void Clear()
		{
			trackEvents.Clear();
		}

		public void AddEvent(MidiEvent midiEvent, int originalTrack)
		{
			if (midiFileType == 0)
			{
				EnsureTracks(1);
				trackEvents[0].Add(midiEvent);
			}
			else if (originalTrack == 0)
			{
				switch (midiEvent.CommandCode)
				{
				case MidiCommandCode.NoteOff:
				case MidiCommandCode.NoteOn:
				case MidiCommandCode.KeyAfterTouch:
				case MidiCommandCode.ControlChange:
				case MidiCommandCode.PatchChange:
				case MidiCommandCode.ChannelAfterTouch:
				case MidiCommandCode.PitchWheelChange:
					EnsureTracks(midiEvent.Channel + 1);
					trackEvents[midiEvent.Channel].Add(midiEvent);
					break;
				default:
					EnsureTracks(1);
					trackEvents[0].Add(midiEvent);
					break;
				}
			}
			else
			{
				EnsureTracks(originalTrack + 1);
				trackEvents[originalTrack].Add(midiEvent);
			}
		}

		private void EnsureTracks(int count)
		{
			for (int i = trackEvents.Count; i < count; i++)
			{
				trackEvents.Add(new List<MidiEvent>());
			}
		}

		private void ExplodeToManyTracks()
		{
			IList<MidiEvent> list = trackEvents[0];
			Clear();
			foreach (MidiEvent item in list)
			{
				AddEvent(item, 0);
			}
			PrepareForExport();
		}

		private void FlattenToOneTrack()
		{
			bool flag = false;
			for (int i = 1; i < trackEvents.Count; i++)
			{
				foreach (MidiEvent item in trackEvents[i])
				{
					if (!MidiEvent.IsEndTrack(item))
					{
						trackEvents[0].Add(item);
						flag = true;
					}
				}
			}
			for (int num = trackEvents.Count - 1; num > 0; num--)
			{
				RemoveTrack(num);
			}
			if (flag)
			{
				PrepareForExport();
			}
		}

		public void PrepareForExport()
		{
			MidiEventComparer comparer = new MidiEventComparer();
			foreach (IList<MidiEvent> trackEvent in trackEvents)
			{
				MergeSort.Sort(trackEvent, comparer);
				int num = 0;
				while (num < trackEvent.Count - 1)
				{
					if (MidiEvent.IsEndTrack(trackEvent[num]))
					{
						trackEvent.RemoveAt(num);
					}
					else
					{
						num++;
					}
				}
			}
			int num2 = 0;
			while (num2 < trackEvents.Count)
			{
				IList<MidiEvent> list = trackEvents[num2];
				if (list.Count == 0)
				{
					RemoveTrack(num2);
					continue;
				}
				if (list.Count == 1 && MidiEvent.IsEndTrack(list[0]))
				{
					RemoveTrack(num2);
					continue;
				}
				if (!MidiEvent.IsEndTrack(list[list.Count - 1]))
				{
					list.Add(new MetaEvent(MetaEventType.EndTrack, 0, list[list.Count - 1].AbsoluteTime));
				}
				num2++;
			}
		}

		public IEnumerator<IList<MidiEvent>> GetEnumerator()
		{
			return trackEvents.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return trackEvents.GetEnumerator();
		}
	}
	public class MidiEventComparer : IComparer<MidiEvent>
	{
		public int Compare(MidiEvent x, MidiEvent y)
		{
			long num = x.AbsoluteTime;
			long num2 = y.AbsoluteTime;
			if (num == num2)
			{
				MetaEvent metaEvent = x as MetaEvent;
				MetaEvent metaEvent2 = y as MetaEvent;
				if (metaEvent != null)
				{
					num = ((metaEvent.MetaEventType != MetaEventType.EndTrack) ? long.MinValue : long.MaxValue);
				}
				if (metaEvent2 != null)
				{
					num2 = ((metaEvent2.MetaEventType != MetaEventType.EndTrack) ? long.MinValue : long.MaxValue);
				}
			}
			return num.CompareTo(num2);
		}
	}
	public class MidiFile
	{
		private readonly MidiEventCollection events;

		private readonly ushort fileFormat;

		private readonly ushort deltaTicksPerQuarterNote;

		private readonly bool strictChecking;

		public int FileFormat => fileFormat;

		public MidiEventCollection Events => events;

		public int Tracks => events.Tracks;

		public int DeltaTicksPerQuarterNote => deltaTicksPerQuarterNote;

		public MidiFile(string filename)
			: this(filename, strictChecking: true)
		{
		}

		public MidiFile(string filename, bool strictChecking)
			: this(File.OpenRead(filename), strictChecking, ownInputStream: true)
		{
		}

		public MidiFile(Stream inputStream, bool strictChecking)
			: this(inputStream, strictChecking, ownInputStream: false)
		{
		}

		private MidiFile(Stream inputStream, bool strictChecking, bool ownInputStream)
		{
			this.strictChecking = strictChecking;
			BinaryReader binaryReader = new BinaryReader(inputStream);
			try
			{
				if (Encoding.UTF8.GetString(binaryReader.ReadBytes(4)) != "MThd")
				{
					throw new FormatException("Not a MIDI file - header chunk missing");
				}
				uint num = SwapUInt32(binaryReader.ReadUInt32());
				if (num != 6)
				{
					throw new FormatException("Unexpected header chunk length");
				}
				fileFormat = SwapUInt16(binaryReader.ReadUInt16());
				int num2 = SwapUInt16(binaryReader.ReadUInt16());
				deltaTicksPerQuarterNote = SwapUInt16(binaryReader.ReadUInt16());
				events = new MidiEventCollection((fileFormat != 0) ? 1 : 0, deltaTicksPerQuarterNote);
				for (int i = 0; i < num2; i++)
				{
					events.AddTrack();
				}
				long num3 = 0L;
				for (int j = 0; j < num2; j++)
				{
					if (fileFormat == 1)
					{
						num3 = 0L;
					}
					if (Encoding.UTF8.GetString(binaryReader.ReadBytes(4)) != "MTrk")
					{
						throw new FormatException("Invalid chunk header");
					}
					num = SwapUInt32(binaryReader.ReadUInt32());
					long position = binaryReader.BaseStream.Position;
					MidiEvent midiEvent = null;
					List<NoteOnEvent> list = new List<NoteOnEvent>();
					while (binaryReader.BaseStream.Position < position + num)
					{
						try
						{
							midiEvent = MidiEvent.ReadNextEvent(binaryReader, midiEvent);
						}
						catch (InvalidDataException)
						{
							if (strictChecking)
							{
								throw;
							}
							continue;
						}
						catch (FormatException)
						{
							if (strictChecking)
							{
								throw;
							}
							continue;
						}
						num3 = (midiEvent.AbsoluteTime = num3 + midiEvent.DeltaTime);
						events[j].Add(midiEvent);
						if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
						{
							NoteEvent noteEvent = (NoteEvent)midiEvent;
							if (noteEvent.Velocity > 0)
							{
								list.Add((NoteOnEvent)noteEvent);
							}
							else
							{
								FindNoteOn(noteEvent, list);
							}
						}
						else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
						{
							FindNoteOn((NoteEvent)midiEvent, list);
						}
						else if (midiEvent.CommandCode == MidiCommandCode.MetaEvent && ((MetaEvent)midiEvent).MetaEventType == MetaEventType.EndTrack && strictChecking && binaryReader.BaseStream.Position < position + num)
						{
							throw new FormatException($"End Track event was not the last MIDI event on track {j}");
						}
					}
					if (list.Count > 0 && strictChecking)
					{
						throw new FormatException($"Note ons without note offs {list.Count} (file format {fileFormat})");
					}
					if (binaryReader.BaseStream.Position != position + num)
					{
						throw new FormatException($"Read too far {num}+{position}!={binaryReader.BaseStream.Position}");
					}
				}
			}
			finally
			{
				if (ownInputStream)
				{
					binaryReader.Close();
				}
			}
		}

		private void FindNoteOn(NoteEvent offEvent, List<NoteOnEvent> outstandingNoteOns)
		{
			bool flag = false;
			foreach (NoteOnEvent outstandingNoteOn in outstandingNoteOns)
			{
				if (outstandingNoteOn.Channel == offEvent.Channel && outstandingNoteOn.NoteNumber == offEvent.NoteNumber)
				{
					outstandingNoteOn.OffEvent = offEvent;
					outstandingNoteOns.Remove(outstandingNoteOn);
					flag = true;
					break;
				}
			}
			if (!flag && strictChecking)
			{
				throw new FormatException($"Got an off without an on {offEvent}");
			}
		}

		private static uint SwapUInt32(uint i)
		{
			return ((i & 0xFF000000u) >> 24) | ((i & 0xFF0000) >> 8) | ((i & 0xFF00) << 8) | ((i & 0xFF) << 24);
		}

		private static ushort SwapUInt16(ushort i)
		{
			return (ushort)(((i & 0xFF00) >> 8) | ((i & 0xFF) << 8));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Format {0}, Tracks {1}, Delta Ticks Per Quarter Note {2}\r\n", fileFormat, Tracks, deltaTicksPerQuarterNote);
			for (int i = 0; i < Tracks; i++)
			{
				foreach (MidiEvent item in events[i])
				{
					stringBuilder.AppendFormat("{0}\r\n", item);
				}
			}
			return stringBuilder.ToString();
		}

		public static void Export(string filename, MidiEventCollection events)
		{
			if (events.MidiFileType == 0 && events.Tracks > 1)
			{
				throw new ArgumentException("Can't export more than one track to a type 0 file");
			}
			using BinaryWriter binaryWriter = new BinaryWriter(File.Create(filename));
			binaryWriter.Write(Encoding.UTF8.GetBytes("MThd"));
			binaryWriter.Write(SwapUInt32(6u));
			binaryWriter.Write(SwapUInt16((ushort)events.MidiFileType));
			binaryWriter.Write(SwapUInt16((ushort)events.Tracks));
			binaryWriter.Write(SwapUInt16((ushort)events.DeltaTicksPerQuarterNote));
			for (int i = 0; i < events.Tracks; i++)
			{
				IList<MidiEvent> list = events[i];
				binaryWriter.Write(Encoding.UTF8.GetBytes("MTrk"));
				long position = binaryWriter.BaseStream.Position;
				binaryWriter.Write(SwapUInt32(0u));
				long absoluteTime = events.StartAbsoluteTime;
				MergeSort.Sort(list, new MidiEventComparer());
				_ = list.Count;
				_ = 0;
				foreach (MidiEvent item in list)
				{
					item.Export(ref absoluteTime, binaryWriter);
				}
				uint num = (uint)((int)(binaryWriter.BaseStream.Position - position) - 4);
				binaryWriter.BaseStream.Position = position;
				binaryWriter.Write(SwapUInt32(num));
				binaryWriter.BaseStream.Position += num;
			}
		}
	}
	public class MidiIn : IDisposable
	{
		private IntPtr hMidiIn = IntPtr.Zero;

		private bool disposed;

		private MidiInterop.MidiInCallback callback;

		public static int NumberOfDevices => MidiInterop.midiInGetNumDevs();

		public event EventHandler<MidiInMessageEventArgs> MessageReceived;

		public event EventHandler<MidiInMessageEventArgs> ErrorReceived;

		public MidiIn(int deviceNo)
		{
			callback = Callback;
			MmException.Try(MidiInterop.midiInOpen(out hMidiIn, (IntPtr)deviceNo, callback, IntPtr.Zero, 196608), "midiInOpen");
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			GC.KeepAlive(callback);
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			MmException.Try(MidiInterop.midiInStart(hMidiIn), "midiInStart");
		}

		public void Stop()
		{
			MmException.Try(MidiInterop.midiInStop(hMidiIn), "midiInStop");
		}

		public void Reset()
		{
			MmException.Try(MidiInterop.midiInReset(hMidiIn), "midiInReset");
		}

		private void Callback(IntPtr midiInHandle, MidiInterop.MidiInMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
		{
			switch (message)
			{
			case MidiInterop.MidiInMessage.Data:
				if (this.MessageReceived != null)
				{
					this.MessageReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
				}
				break;
			case MidiInterop.MidiInMessage.Error:
				if (this.ErrorReceived != null)
				{
					this.ErrorReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
				}
				break;
			case MidiInterop.MidiInMessage.Open:
			case MidiInterop.MidiInMessage.Close:
			case MidiInterop.MidiInMessage.LongData:
			case MidiInterop.MidiInMessage.LongError:
			case (MidiInterop.MidiInMessage)967:
			case (MidiInterop.MidiInMessage)968:
			case (MidiInterop.MidiInMessage)969:
			case (MidiInterop.MidiInMessage)970:
			case (MidiInterop.MidiInMessage)971:
			case MidiInterop.MidiInMessage.MoreData:
				break;
			}
		}

		public static MidiInCapabilities DeviceInfo(int midiInDeviceNumber)
		{
			MidiInCapabilities capabilities = default(MidiInCapabilities);
			int size = Marshal.SizeOf(capabilities);
			MmException.Try(MidiInterop.midiInGetDevCaps((IntPtr)midiInDeviceNumber, out capabilities, size), "midiInGetDevCaps");
			return capabilities;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				MidiInterop.midiInClose(hMidiIn);
			}
			disposed = true;
		}

		~MidiIn()
		{
			Dispose(disposing: false);
		}
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MidiInCapabilities
	{
		private ushort manufacturerId;

		private ushort productId;

		private uint driverVersion;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		private string productName;

		private int support;

		private const int MaxProductNameLength = 32;

		public Manufacturers Manufacturer => (Manufacturers)manufacturerId;

		public int ProductId => productId;

		public string ProductName => productName;
	}
	public class MidiInMessageEventArgs : EventArgs
	{
		public int RawMessage { get; private set; }

		public MidiEvent MidiEvent { get; private set; }

		public int Timestamp { get; private set; }

		public MidiInMessageEventArgs(int message, int timestamp)
		{
			RawMessage = message;
			Timestamp = timestamp;
			try
			{
				MidiEvent = MidiEvent.FromRawMessage(message);
			}
			catch (Exception)
			{
			}
		}
	}
	internal class MidiInterop
	{
		public enum MidiInMessage
		{
			Open = 961,
			Close = 962,
			Data = 963,
			LongData = 964,
			Error = 965,
			LongError = 966,
			MoreData = 972
		}

		public enum MidiOutMessage
		{
			Open = 967,
			Close,
			Done
		}

		public delegate void MidiInCallback(IntPtr midiInHandle, MidiInMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2);

		public delegate void MidiOutCallback(IntPtr midiInHandle, MidiOutMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2);

		public struct MMTIME
		{
			public int wType;

			public int u;
		}

		public struct MIDIEVENT
		{
			public int dwDeltaTime;

			public int dwStreamID;

			public int dwEvent;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public int dwParms;
		}

		public struct MIDIHDR
		{
			public IntPtr lpData;

			public int dwBufferLength;

			public int dwBytesRecorded;

			public IntPtr dwUser;

			public int dwFlags;

			public IntPtr lpNext;

			public IntPtr reserved;

			public int dwOffset;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public IntPtr[] dwReserved;
		}

		public struct MIDIPROPTEMPO
		{
			public int cbStruct;

			public int dwTempo;
		}

		public const int CALLBACK_FUNCTION = 196608;

		public const int CALLBACK_NULL = 0;

		[DllImport("winmm.dll")]
		public static extern MmResult midiConnect(IntPtr hMidiIn, IntPtr hMidiOut, IntPtr pReserved);

		[DllImport("winmm.dll")]
		public static extern MmResult midiDisconnect(IntPtr hMidiIn, IntPtr hMidiOut, IntPtr pReserved);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInAddBuffer(IntPtr hMidiIn, ref MIDIHDR lpMidiInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInClose(IntPtr hMidiIn);

		[DllImport("winmm.dll", CharSet = CharSet.Auto)]
		public static extern MmResult midiInGetDevCaps(IntPtr deviceId, out MidiInCapabilities capabilities, int size);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInGetErrorText(int err, string lpText, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInGetID(IntPtr hMidiIn, out int lpuDeviceId);

		[DllImport("winmm.dll")]
		public static extern int midiInGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern MmResult midiInMessage(IntPtr hMidiIn, int msg, IntPtr dw1, IntPtr dw2);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInOpen(out IntPtr hMidiIn, IntPtr uDeviceID, MidiInCallback callback, IntPtr dwInstance, int dwFlags);

		[DllImport("winmm.dll", EntryPoint = "midiInOpen")]
		public static extern MmResult midiInOpenWindow(out IntPtr hMidiIn, IntPtr uDeviceID, IntPtr callbackWindowHandle, IntPtr dwInstance, int dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInPrepareHeader(IntPtr hMidiIn, ref MIDIHDR lpMidiInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInReset(IntPtr hMidiIn);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInStart(IntPtr hMidiIn);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInStop(IntPtr hMidiIn);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInUnprepareHeader(IntPtr hMidiIn, ref MIDIHDR lpMidiInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutCacheDrumPatches(IntPtr hMidiOut, int uPatch, IntPtr lpKeyArray, int uFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutCachePatches(IntPtr hMidiOut, int uBank, IntPtr lpPatchArray, int uFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutClose(IntPtr hMidiOut);

		[DllImport("winmm.dll", CharSet = CharSet.Auto)]
		public static extern MmResult midiOutGetDevCaps(IntPtr deviceNumber, out MidiOutCapabilities caps, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutGetErrorText(IntPtr err, string lpText, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutGetID(IntPtr hMidiOut, out int lpuDeviceID);

		[DllImport("winmm.dll")]
		public static extern int midiOutGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutGetVolume(IntPtr uDeviceID, ref int lpdwVolume);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutLongMsg(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutMessage(IntPtr hMidiOut, int msg, IntPtr dw1, IntPtr dw2);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutOpen(out IntPtr lphMidiOut, IntPtr uDeviceID, MidiOutCallback dwCallback, IntPtr dwInstance, int dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutPrepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutReset(IntPtr hMidiOut);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutSetVolume(IntPtr hMidiOut, int dwVolume);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutShortMsg(IntPtr hMidiOut, int dwMsg);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutUnprepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamClose(IntPtr hMidiStream);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamOpen(out IntPtr hMidiStream, IntPtr puDeviceID, int cMidi, IntPtr dwCallback, IntPtr dwInstance, int fdwOpen);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamOut(IntPtr hMidiStream, ref MIDIHDR pmh, int cbmh);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamPause(IntPtr hMidiStream);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamPosition(IntPtr hMidiStream, ref MMTIME lpmmt, int cbmmt);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamProperty(IntPtr hMidiStream, IntPtr lppropdata, int dwProperty);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamRestart(IntPtr hMidiStream);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamStop(IntPtr hMidiStream);
	}
	public class MidiMessage
	{
		private int rawData;

		public int RawData => rawData;

		public MidiMessage(int status, int data1, int data2)
		{
			rawData = status + (data1 << 8) + (data2 << 16);
		}

		public MidiMessage(int rawData)
		{
			this.rawData = rawData;
		}

		public static MidiMessage StartNote(int note, int volume, int channel)
		{
			ValidateNoteParameters(note, volume, channel);
			return new MidiMessage(144 + channel - 1, note, volume);
		}

		private static void ValidateNoteParameters(int note, int volume, int channel)
		{
			ValidateChannel(channel);
			if (note < 0 || note > 127)
			{
				throw new ArgumentOutOfRangeException("note", "Note number must be in the range 0-127");
			}
			if (volume < 0 || volume > 127)
			{
				throw new ArgumentOutOfRangeException("volume", "Velocity must be in the range 0-127");
			}
		}

		private static void ValidateChannel(int channel)
		{
			if (channel < 1 || channel > 16)
			{
				throw new ArgumentOutOfRangeException("channel", channel, $"Channel must be 1-16 (Got {channel})");
			}
		}

		public static MidiMessage StopNote(int note, int volume, int channel)
		{
			ValidateNoteParameters(note, volume, channel);
			return new MidiMessage(128 + channel - 1, note, volume);
		}

		public static MidiMessage ChangePatch(int patch, int channel)
		{
			ValidateChannel(channel);
			return new MidiMessage(192 + channel - 1, patch, 0);
		}

		public static MidiMessage ChangeControl(int controller, int value, int channel)
		{
			ValidateChannel(channel);
			return new MidiMessage(176 + channel - 1, controller, value);
		}
	}
	public class MidiOut : IDisposable
	{
		private IntPtr hMidiOut = IntPtr.Zero;

		private bool disposed;

		private MidiInterop.MidiOutCallback callback;

		public static int NumberOfDevices => MidiInterop.midiOutGetNumDevs();

		public int Volume
		{
			get
			{
				int lpdwVolume = 0;
				MmException.Try(MidiInterop.midiOutGetVolume(hMidiOut, ref lpdwVolume), "midiOutGetVolume");
				return lpdwVolume;
			}
			set
			{
				MmException.Try(MidiInterop.midiOutSetVolume(hMidiOut, value), "midiOutSetVolume");
			}
		}

		public static MidiOutCapabilities DeviceInfo(int midiOutDeviceNumber)
		{
			MidiOutCapabilities caps = default(MidiOutCapabilities);
			int uSize = Marshal.SizeOf(caps);
			MmException.Try(MidiInterop.midiOutGetDevCaps((IntPtr)midiOutDeviceNumber, out caps, uSize), "midiOutGetDevCaps");
			return caps;
		}

		public MidiOut(int deviceNo)
		{
			callback = Callback;
			MmException.Try(MidiInterop.midiOutOpen(out hMidiOut, (IntPtr)deviceNo, callback, IntPtr.Zero, 196608), "midiOutOpen");
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			GC.KeepAlive(callback);
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Reset()
		{
			MmException.Try(MidiInterop.midiOutReset(hMidiOut), "midiOutReset");
		}

		public void SendDriverMessage(int message, int param1, int param2)
		{
			MmException.Try(MidiInterop.midiOutMessage(hMidiOut, message, (IntPtr)param1, (IntPtr)param2), "midiOutMessage");
		}

		public void Send(int message)
		{
			MmException.Try(MidiInterop.midiOutShortMsg(hMidiOut, message), "midiOutShortMsg");
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				MidiInterop.midiOutClose(hMidiOut);
			}
			disposed = true;
		}

		private void Callback(IntPtr midiInHandle, MidiInterop.MidiOutMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
		{
		}

		public void SendBuffer(byte[] byteBuffer)
		{
			MidiInterop.MIDIHDR lpMidiOutHdr = default(MidiInterop.MIDIHDR);
			lpMidiOutHdr.lpData = Marshal.AllocHGlobal(byteBuffer.Length);
			Marshal.Copy(byteBuffer, 0, lpMidiOutHdr.lpData, byteBuffer.Length);
			lpMidiOutHdr.dwBufferLength = byteBuffer.Length;
			lpMidiOutHdr.dwBytesRecorded = byteBuffer.Length;
			int uSize = Marshal.SizeOf(lpMidiOutHdr);
			MidiInterop.midiOutPrepareHeader(hMidiOut, ref lpMidiOutHdr, uSize);
			if (MidiInterop.midiOutLongMsg(hMidiOut, ref lpMidiOutHdr, uSize) != 0)
			{
				MidiInterop.midiOutUnprepareHeader(hMidiOut, ref lpMidiOutHdr, uSize);
			}
			Marshal.FreeHGlobal(lpMidiOutHdr.lpData);
		}

		~MidiOut()
		{
			Dispose(disposing: false);
		}
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MidiOutCapabilities
	{
		[Flags]
		private enum MidiOutCapabilityFlags
		{
			Volume = 1,
			LeftRightVolume = 2,
			PatchCaching = 4,
			Stream = 8
		}

		private short manufacturerId;

		private short productId;

		private int driverVersion;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		private string productName;

		private short wTechnology;

		private short wVoices;

		private short wNotes;

		private ushort wChannelMask;

		private MidiOutCapabilityFlags dwSupport;

		private const int MaxProductNameLength = 32;

		public Manufacturers Manufacturer => (Manufacturers)manufacturerId;

		public short ProductId => productId;

		public string ProductName => productName;

		public int Voices => wVoices;

		public int Notes => wNotes;

		public bool SupportsAllChannels => wChannelMask == ushort.MaxValue;

		public bool SupportsPatchCaching => (dwSupport & MidiOutCapabilityFlags.PatchCaching) != 0;

		public bool SupportsSeparateLeftAndRightVolume => (dwSupport & MidiOutCapabilityFlags.LeftRightVolume) != 0;

		public bool SupportsMidiStreamOut => (dwSupport & MidiOutCapabilityFlags.Stream) != 0;

		public bool SupportsVolumeControl => (dwSupport & MidiOutCapabilityFlags.Volume) != 0;

		public MidiOutTechnology Technology => (MidiOutTechnology)wTechnology;

		public bool SupportsChannel(int channel)
		{
			return (wChannelMask & (1 << channel - 1)) > 0;
		}
	}
	public enum MidiOutTechnology
	{
		MidiPort = 1,
		Synth,
		SquareWaveSynth,
		FMSynth,
		MidiMapper,
		WaveTableSynth,
		SoftwareSynth
	}
	public class NoteEvent : MidiEvent
	{
		private int noteNumber;

		private int velocity;

		private static readonly string[] NoteNames = new string[12]
		{
			"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A",
			"A#", "B"
		};

		public virtual int NoteNumber
		{
			get
			{
				return noteNumber;
			}
			set
			{
				if (value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException("value", "Note number must be in the range 0-127");
				}
				noteNumber = value;
			}
		}

		public int Velocity
		{
			get
			{
				return velocity;
			}
			set
			{
				if (value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException("value", "Velocity must be in the range 0-127");
				}
				velocity = value;
			}
		}

		public string NoteName
		{
			get
			{
				if (Channel == 16 || Channel == 10)
				{
					return noteNumber switch
					{
						35 => "Acoustic Bass Drum", 
						36 => "Bass Drum 1", 
						37 => "Side Stick", 
						38 => "Acoustic Snare", 
						39 => "Hand Clap", 
						40 => "Electric Snare", 
						41 => "Low Floor Tom", 
						42 => "Closed Hi-Hat", 
						43 => "High Floor Tom", 
						44 => "Pedal Hi-Hat", 
						45 => "Low Tom", 
						46 => "Open Hi-Hat", 
						47 => "Low-Mid Tom", 
						48 => "Hi-Mid Tom", 
						49 => "Crash Cymbal 1", 
						50 => "High Tom", 
						51 => "Ride Cymbal 1", 
						52 => "Chinese Cymbal", 
						53 => "Ride Bell", 
						54 => "Tambourine", 
						55 => "Splash Cymbal", 
						56 => "Cowbell", 
						57 => "Crash Cymbal 2", 
						58 => "Vibraslap", 
						59 => "Ride Cymbal 2", 
						60 => "Hi Bongo", 
						61 => "Low Bongo", 
						62 => "Mute Hi Conga", 
						63 => "Open Hi Conga", 
						64 => "Low Conga", 
						65 => "High Timbale", 
						66 => "Low Timbale", 
						67 => "High Agogo", 
						68 => "Low Agogo", 
						69 => "Cabasa", 
						70 => "Maracas", 
						71 => "Short Whistle", 
						72 => "Long Whistle", 
						73 => "Short Guiro", 
						74 => "Long Guiro", 
						75 => "Claves", 
						76 => "Hi Wood Block", 
						77 => "Low Wood Block", 
						78 => "Mute Cuica", 
						79 => "Open Cuica", 
						80 => "Mute Triangle", 
						81 => "Open Triangle", 
						_ => $"Drum {noteNumber}", 
					};
				}
				int num = noteNumber / 12;
				return $"{NoteNames[noteNumber % 12]}{num}";
			}
		}

		public NoteEvent(BinaryReader br)
		{
			NoteNumber = br.ReadByte();
			velocity = br.ReadByte();
			if (velocity > 127)
			{
				velocity = 127;
			}
		}

		public NoteEvent(long absoluteTime, int channel, MidiCommandCode commandCode, int noteNumber, int velocity)
			: base(absoluteTime, channel, commandCode)
		{
			NoteNumber = noteNumber;
			Velocity = velocity;
		}

		public override int GetAsShortMessage()
		{
			return base.GetAsShortMessage() + (noteNumber << 8) + (velocity << 16);
		}

		public override string ToString()
		{
			return $"{base.ToString()} {NoteName} Vel:{Velocity}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)noteNumber);
			writer.Write((byte)velocity);
		}
	}
	public class NoteOnEvent : NoteEvent
	{
		private NoteEvent offEvent;

		public NoteEvent OffEvent
		{
			get
			{
				return offEvent;
			}
			set
			{
				if (!MidiEvent.IsNoteOff(value))
				{
					throw new ArgumentException("OffEvent must be a valid MIDI note off event");
				}
				if (value.NoteNumber != NoteNumber)
				{
					throw new ArgumentException("Note Off Event must be for the same note number");
				}
				if (value.Channel != Channel)
				{
					throw new ArgumentException("Note Off Event must be for the same channel");
				}
				offEvent = value;
			}
		}

		public override int NoteNumber
		{
			get
			{
				return base.NoteNumber;
			}
			set
			{
				base.NoteNumber = value;
				if (OffEvent != null)
				{
					OffEvent.NoteNumber = NoteNumber;
				}
			}
		}

		public override int Channel
		{
			get
			{
				return base.Channel;
			}
			set
			{
				base.Channel = value;
				if (OffEvent != null)
				{
					OffEvent.Channel = Channel;
				}
			}
		}

		public int NoteLength
		{
			get
			{
				return (int)(offEvent.AbsoluteTime - base.AbsoluteTime);
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("NoteLength must be 0 or greater");
				}
				offEvent.AbsoluteTime = base.AbsoluteTime + value;
			}
		}

		public NoteOnEvent(BinaryReader br)
			: base(br)
		{
		}

		public NoteOnEvent(long absoluteTime, int channel, int noteNumber, int velocity, int duration)
			: base(absoluteTime, channel, MidiCommandCode.NoteOn, noteNumber, velocity)
		{
			OffEvent = new NoteEvent(absoluteTime, channel, MidiCommandCode.NoteOff, noteNumber, 0);
			NoteLength = duration;
		}

		public override MidiEvent Clone()
		{
			return new NoteOnEvent(base.AbsoluteTime, Channel, NoteNumber, base.Velocity, NoteLength);
		}

		public override string ToString()
		{
			if (base.Velocity == 0 && OffEvent == null)
			{
				return $"{base.ToString()} (Note Off)";
			}
			return string.Format("{0} Len: {1}", base.ToString(), (OffEvent == null) ? "?" : NoteLength.ToString());
		}
	}
	public class PatchChangeEvent : MidiEvent
	{
		private byte patch;

		private static readonly string[] patchNames = new string[128]
		{
			"Acoustic Grand", "Bright Acoustic", "Electric Grand", "Honky-Tonk", "Electric Piano 1", "Electric Piano 2", "Harpsichord", "Clav", "Celesta", "Glockenspiel",
			"Music Box", "Vibraphone", "Marimba", "Xylophone", "Tubular Bells", "Dulcimer", "Drawbar Organ", "Percussive Organ", "Rock Organ", "Church Organ",
			"Reed Organ", "Accoridan", "Harmonica", "Tango Accordian", "Acoustic Guitar(nylon)", "Acoustic Guitar(steel)", "Electric Guitar(jazz)", "Electric Guitar(clean)", "Electric Guitar(muted)", "Overdriven Guitar",
			"Distortion Guitar", "Guitar Harmonics", "Acoustic Bass", "Electric Bass(finger)", "Electric Bass(pick)", "Fretless Bass", "Slap Bass 1", "Slap Bass 2", "Synth Bass 1", "Synth Bass 2",
			"Violin", "Viola", "Cello", "Contrabass", "Tremolo Strings", "Pizzicato Strings", "Orchestral Strings", "Timpani", "String Ensemble 1", "String Ensemble 2",
			"SynthStrings 1", "SynthStrings 2", "Choir Aahs", "Voice Oohs", "Synth Voice", "Orchestra Hit", "Trumpet", "Trombone", "Tuba", "Muted Trumpet",
			"French Horn", "Brass Section", "SynthBrass 1", "SynthBrass 2", "Soprano Sax", "Alto Sax", "Tenor Sax", "Baritone Sax", "Oboe", "English Horn",
			"Bassoon", "Clarinet", "Piccolo", "Flute", "Recorder", "Pan Flute", "Blown Bottle", "Skakuhachi", "Whistle", "Ocarina",
			"Lead 1 (square)", "Lead 2 (sawtooth)", "Lead 3 (calliope)", "Lead 4 (chiff)", "Lead 5 (charang)", "Lead 6 (voice)", "Lead 7 (fifths)", "Lead 8 (bass+lead)", "Pad 1 (new age)", "Pad 2 (warm)",
			"Pad 3 (polysynth)", "Pad 4 (choir)", "Pad 5 (bowed)", "Pad 6 (metallic)", "Pad 7 (halo)", "Pad 8 (sweep)", "FX 1 (rain)", "FX 2 (soundtrack)", "FX 3 (crystal)", "FX 4 (atmosphere)",
			"FX 5 (brightness)", "FX 6 (goblins)", "FX 7 (echoes)", "FX 8 (sci-fi)", "Sitar", "Banjo", "Shamisen", "Koto", "Kalimba", "Bagpipe",
			"Fiddle", "Shanai", "Tinkle Bell", "Agogo", "Steel Drums", "Woodblock", "Taiko Drum", "Melodic Tom", "Synth Drum", "Reverse Cymbal",
			"Guitar Fret Noise", "Breath Noise", "Seashore", "Bird Tweet", "Telephone Ring", "Helicopter", "Applause", "Gunshot"
		};

		public int Patch
		{
			get
			{
				return patch;
			}
			set
			{
				if (value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException("value", "Patch number must be in the range 0-127");
				}
				patch = (byte)value;
			}
		}

		public static string GetPatchName(int patchNumber)
		{
			return patchNames[patchNumber];
		}

		public PatchChangeEvent(BinaryReader br)
		{
			patch = br.ReadByte();
			if ((patch & 0x80u) != 0)
			{
				throw new FormatException("Invalid patch");
			}
		}

		public PatchChangeEvent(long absoluteTime, int channel, int patchNumber)
			: base(absoluteTime, channel, MidiCommandCode.PatchChange)
		{
			Patch = patchNumber;
		}

		public override string ToString()
		{
			return $"{base.ToString()} {GetPatchName(patch)}";
		}

		public override int GetAsShortMessage()
		{
			return base.GetAsShortMessage() + (patch << 8);
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(patch);
		}
	}
	public class PitchWheelChangeEvent : MidiEvent
	{
		private int pitch;

		public int Pitch
		{
			get
			{
				return pitch;
			}
			set
			{
				if (value < 0 || value >= 16384)
				{
					throw new ArgumentOutOfRangeException("value", "Pitch value must be in the range 0 - 0x3FFF");
				}
				pitch = value;
			}
		}

		public PitchWheelChangeEvent(BinaryReader br)
		{
			byte b = br.ReadByte();
			byte b2 = br.ReadByte();
			if ((b & 0x80u) != 0)
			{
				throw new FormatException("Invalid pitchwheelchange byte 1");
			}
			if ((b2 & 0x80u) != 0)
			{
				throw new FormatException("Invalid pitchwheelchange byte 2");
			}
			pitch = b + (b2 << 7);
		}

		public PitchWheelChangeEvent(long absoluteTime, int channel, int pitchWheel)
			: base(absoluteTime, channel, MidiCommandCode.PitchWheelChange)
		{
			Pitch = pitchWheel;
		}

		public override string ToString()
		{
			return $"{base.ToString()} Pitch {pitch} ({pitch - 8192})";
		}

		public override int GetAsShortMessage()
		{
			return base.GetAsShortMessage() + ((pitch & 0x7F) << 8) + (((pitch >> 7) & 0x7F) << 16);
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)((uint)pitch & 0x7Fu));
			writer.Write((byte)((uint)(pitch >> 7) & 0x7Fu));
		}
	}
	public class RawMetaEvent : MetaEvent
	{
		public byte[] Data { get; set; }

		public RawMetaEvent(MetaEventType metaEventType, long absoluteTime, byte[] data)
			: base(metaEventType, (data != null) ? data.Length : 0, absoluteTime)
		{
			Data = data;
		}

		public override MidiEvent Clone()
		{
			return new RawMetaEvent(base.MetaEventType, base.AbsoluteTime, (byte[])Data?.Clone());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder().Append(base.ToString());
			byte[] data = Data;
			foreach (byte b in data)
			{
				stringBuilder.AppendFormat(" {0:X2}", b);
			}
			return stringBuilder.ToString();
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			if (Data != null)
			{
				writer.Write(Data, 0, Data.Length);
			}
		}
	}
	public class SequencerSpecificEvent : MetaEvent
	{
		private byte[] data;

		public byte[] Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
				metaDataLength = data.Length;
			}
		}

		public SequencerSpecificEvent(BinaryReader br, int length)
		{
			data = br.ReadBytes(length);
		}

		public SequencerSpecificEvent(byte[] data, long absoluteTime)
			: base(MetaEventType.SequencerSpecific, data.Length, absoluteTime)
		{
			this.data = data;
		}

		public override MidiEvent Clone()
		{
			return new SequencerSpecificEvent((byte[])data.Clone(), base.AbsoluteTime);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.ToString());
			stringBuilder.Append(" ");
			byte[] array = data;
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:X2} ", b);
			}
			stringBuilder.Length--;
			return stringBuilder.ToString();
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(data);
		}
	}
	public class SmpteOffsetEvent : MetaEvent
	{
		private readonly byte hours;

		private readonly byte minutes;

		private readonly byte seconds;

		private readonly byte frames;

		private readonly byte subFrames;

		public int Hours => hours;

		public int Minutes => minutes;

		public int Seconds => seconds;

		public int Frames => frames;

		public int SubFrames => subFrames;

		public SmpteOffsetEvent(byte hours, byte minutes, byte seconds, byte frames, byte subFrames)
		{
			this.hours = hours;
			this.minutes = minutes;
			this.seconds = seconds;
			this.frames = frames;
			this.subFrames = subFrames;
		}

		public SmpteOffsetEvent(BinaryReader br, int length)
		{
			if (length != 5)
			{
				throw new FormatException($"Invalid SMPTE Offset length: Got {length}, expected 5");
			}
			hours = br.ReadByte();
			minutes = br.ReadByte();
			seconds = br.ReadByte();
			frames = br.ReadByte();
			subFrames = br.ReadByte();
		}

		public override MidiEvent Clone()
		{
			return (SmpteOffsetEvent)MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{base.ToString()} {hours}:{minutes}:{seconds}:{frames}:{subFrames}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(hours);
			writer.Write(minutes);
			writer.Write(seconds);
			writer.Write(frames);
			writer.Write(subFrames);
		}
	}
	public class SysexEvent : MidiEvent
	{
		private byte[] data;

		public static SysexEvent ReadSysexEvent(BinaryReader br)
		{
			SysexEvent sysexEvent = new SysexEvent();
			List<byte> list = new List<byte>();
			bool flag = true;
			while (flag)
			{
				byte b = br.ReadByte();
				if (b == 247)
				{
					flag = false;
				}
				else
				{
					list.Add(b);
				}
			}
			sysexEvent.data = list.ToArray();
			return sysexEvent;
		}

		public override MidiEvent Clone()
		{
			return new SysexEvent
			{
				data = (byte[])data?.Clone()
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = data;
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:X2} ", b);
			}
			return $"{base.AbsoluteTime} Sysex: {data.Length} bytes\r\n{stringBuilder.ToString()}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(data, 0, data.Length);
			writer.Write((byte)247);
		}
	}
	public class TempoEvent : MetaEvent
	{
		private int microsecondsPerQuarterNote;

		public int MicrosecondsPerQuarterNote
		{
			get
			{
				return microsecondsPerQuarterNote;
			}
			set
			{
				microsecondsPerQuarterNote = value;
			}
		}

		public double Tempo
		{
			get
			{
				return 60000000.0 / (double)microsecondsPerQuarterNote;
			}
			set
			{
				microsecondsPerQuarterNote = (int)(60000000.0 / value);
			}
		}

		public TempoEvent(BinaryReader br, int length)
		{
			if (length != 3)
			{
				throw new FormatException("Invalid tempo length");
			}
			microsecondsPerQuarterNote = (br.ReadByte() << 16) + (br.ReadByte() << 8) + br.ReadByte();
		}

		public TempoEvent(int microsecondsPerQuarterNote, long absoluteTime)
			: base(MetaEventType.SetTempo, 3, absoluteTime)
		{
			this.microsecondsPerQuarterNote = microsecondsPerQuarterNote;
		}

		public override MidiEvent Clone()
		{
			return (TempoEvent)MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format("{0} {2}bpm ({1})", base.ToString(), microsecondsPerQuarterNote, 60000000 / microsecondsPerQuarterNote);
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)((uint)(microsecondsPerQuarterNote >> 16) & 0xFFu));
			writer.Write((byte)((uint)(microsecondsPerQuarterNote >> 8) & 0xFFu));
			writer.Write((byte)((uint)microsecondsPerQuarterNote & 0xFFu));
		}
	}
	public class TextEvent : MetaEvent
	{
		private byte[] data;

		public string Text
		{
			get
			{
				return ByteEncoding.Instance.GetString(data);
			}
			set
			{
				Encoding instance = ByteEncoding.Instance;
				data = instance.GetBytes(value);
				metaDataLength = data.Length;
			}
		}

		public byte[] Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
				metaDataLength = data.Length;
			}
		}

		public TextEvent(BinaryReader br, int length)
		{
			data = br.ReadBytes(length);
		}

		public TextEvent(string text, MetaEventType metaEventType, long absoluteTime)
			: base(metaEventType, text.Length, absoluteTime)
		{
			Text = text;
		}

		public override MidiEvent Clone()
		{
			return (TextEvent)MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{base.ToString()} {Text}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(data);
		}
	}
	public class TimeSignatureEvent : MetaEvent
	{
		private byte numerator;

		private byte denominator;

		private byte ticksInMetronomeClick;

		private byte no32ndNotesInQuarterNote;

		public int Numerator => numerator;

		public int Denominator => denominator;

		public int TicksInMetronomeClick => ticksInMetronomeClick;

		public int No32ndNotesInQuarterNote => no32ndNotesInQuarterNote;

		public string TimeSignature
		{
			get
			{
				string arg = $"Unknown ({denominator})";
				switch (denominator)
				{
				case 1:
					arg = "2";
					break;
				case 2:
					arg = "4";
					break;
				case 3:
					arg = "8";
					break;
				case 4:
					arg = "16";
					break;
				case 5:
					arg = "32";
					break;
				}
				return $"{numerator}/{arg}";
			}
		}

		public TimeSignatureEvent(BinaryReader br, int length)
		{
			if (length != 4)
			{
				throw new FormatException($"Invalid time signature length: Got {length}, expected 4");
			}
			numerator = br.ReadByte();
			denominator = br.ReadByte();
			ticksInMetronomeClick = br.ReadByte();
			no32ndNotesInQuarterNote = br.ReadByte();
		}

		public TimeSignatureEvent(long absoluteTime, int numerator, int denominator, int ticksInMetronomeClick, int no32ndNotesInQuarterNote)
			: base(MetaEventType.TimeSignature, 4, absoluteTime)
		{
			this.numerator = (byte)numerator;
			this.denominator = (byte)denominator;
			this.ticksInMetronomeClick = (byte)ticksInMetronomeClick;
			this.no32ndNotesInQuarterNote = (byte)no32ndNotesInQuarterNote;
		}

		public override MidiEvent Clone()
		{
			return (TimeSignatureEvent)MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{base.ToString()} {TimeSignature} TicksInClick:{ticksInMetronomeClick} 32ndsInQuarterNote:{no32ndNotesInQuarterNote}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(numerator);
			writer.Write(denominator);
			writer.Write(ticksInMetronomeClick);
			writer.Write(no32ndNotesInQuarterNote);
		}
	}
	public class TrackSequenceNumberEvent : MetaEvent
	{
		private ushort sequenceNumber;

		public TrackSequenceNumberEvent(ushort sequenceNumber)
		{
			this.sequenceNumber = sequenceNumber;
		}

		public TrackSequenceNumberEvent(BinaryReader br, int length)
		{
			if (length != 2)
			{
				throw new FormatException("Invalid sequence number length");
			}
			sequenceNumber = (ushort)((br.ReadByte() << 8) + br.ReadByte());
		}

		public override MidiEvent Clone()
		{
			return (TrackSequenceNumberEvent)MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{base.ToString()} {sequenceNumber}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)((uint)(sequenceNumber >> 8) & 0xFFu));
			writer.Write((byte)(sequenceNumber & 0xFFu));
		}
	}
}
namespace NAudio.MediaFoundation
{
	public static class AudioSubtypes
	{
		[FieldDescription("AAC")]
		public static readonly Guid MFAudioFormat_AAC = new Guid("00001610-0000-0010-8000-00aa00389b71");

		[FieldDescription("ADTS")]
		public static readonly Guid MFAudioFormat_ADTS = new Guid("00001600-0000-0010-8000-00aa00389b71");

		[FieldDescription("Dolby AC3 SPDIF")]
		public static readonly Guid MFAudioFormat_Dolby_AC3_SPDIF = new Guid("00000092-0000-0010-8000-00aa00389b71");

		[FieldDescription("DRM")]
		public static readonly Guid MFAudioFormat_DRM = new Guid("00000009-0000-0010-8000-00aa00389b71");

		[FieldDescription("DTS")]
		public static readonly Guid MFAudioFormat_DTS = new Guid("00000008-0000-0010-8000-00aa00389b71");

		[FieldDescription("IEEE floating-point")]
		public static readonly Guid MFAudioFormat_Float = new Guid("00000003-0000-0010-8000-00aa00389b71");

		[FieldDescription("MP3")]
		public static readonly Guid MFAudioFormat_MP3 = new Guid("00000055-0000-0010-8000-00aa00389b71");

		[FieldDescription("MPEG")]
		public static readonly Guid MFAudioFormat_MPEG = new Guid("00000050-0000-0010-8000-00aa00389b71");

		[FieldDescription("WMA 9 Voice codec")]
		public static readonly Guid MFAudioFormat_MSP1 = new Guid("0000000a-0000-0010-8000-00aa00389b71");

		[FieldDescription("PCM")]
		public static readonly Guid MFAudioFormat_PCM = new Guid("00000001-0000-0010-8000-00aa00389b71");

		[FieldDescription("WMA SPDIF")]
		public static readonly Guid MFAudioFormat_WMASPDIF = new Guid("00000164-0000-0010-8000-00aa00389b71");

		[FieldDescription("WMAudio Lossless")]
		public static readonly Guid MFAudioFormat_WMAudio_Lossless = new Guid("00000163-0000-0010-8000-00aa00389b71");

		[FieldDescription("Windows Media Audio")]
		public static readonly Guid MFAudioFormat_WMAudioV8 = new Guid("00000161-0000-0010-8000-00aa00389b71");

		[FieldDescription("Windows Media Audio Professional")]
		public static readonly Guid MFAudioFormat_WMAudioV9 = new Guid("00000162-0000-0010-8000-00aa00389b71");

		[FieldDescription("Dolby AC3")]
		public static readonly Guid MFAudioFormat_Dolby_AC3 = new Guid("e06d802c-db46-11cf-b4d1-00805f6cbbea");

		[FieldDescription("MPEG-4 and AAC Audio Types")]
		public static readonly Guid MEDIASUBTYPE_RAW_AAC1 = new Guid("000000ff-0000-0010-8000-00aa00389b71");

		[FieldDescription("Dolby Audio Types")]
		public static readonly Guid MEDIASUBTYPE_DVM = new Guid("00002000-0000-0010-8000-00aa00389b71");

		[FieldDescription("Dolby Audio Types")]
		public static readonly Guid MEDIASUBTYPE_DOLBY_DDPLUS = new Guid("a7fb87af-2d02-42fb-a4d4-05cd93843bdd");

		[FieldDescription("μ-law")]
		public static readonly Guid KSDATAFORMAT_SUBTYPE_MULAW = new Guid("00000007-0000-0010-8000-00aa00389b71");

		[FieldDescription("ADPCM")]
		public static readonly Guid KSDATAFORMAT_SUBTYPE_ADPCM = new Guid("00000002-0000-0010-8000-00aa00389b71");

		[FieldDescription("Dolby Digital Plus for HDMI")]
		public static readonly Guid KSDATAFORMAT_SUBTYPE_IEC61937_DOLBY_DIGITAL_PLUS = new Guid("0000000a-0cea-0010-8000-00aa00389b71");

		[FieldDescription("MSAudio1")]
		public static readonly Guid MEDIASUBTYPE_MSAUDIO1 = new Guid("00000160-0000-0010-8000-00aa00389b71");

		[FieldDescription("IMA ADPCM")]
		public static readonly Guid ImaAdpcm = new Guid("00000011-0000-0010-8000-00aa00389b71");

		[FieldDescription("WMSP2")]
		public static readonly Guid WMMEDIASUBTYPE_WMSP2 = new Guid("0000000b-0000-0010-8000-00aa00389b71");
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("7FEE9E9A-4A89-47a6-899C-B6A53A70FB67")]
	public interface IMFActivate : IMFAttributes
	{
		new void GetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] IntPtr pValue);

		new void GetItemType([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pType);

		new void CompareItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void Compare([MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs, int matchType, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void GetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int punValue);

		new void GetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out long punValue);

		new void GetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

		new void GetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

		new void GetStringLength([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcchLength);

		new void GetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		new void GetAllocatedString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue, out int pcchLength);

		new void GetBlobSize([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcbBlobSize);

		new void GetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		new void GetAllocatedBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ip, out int pcbSize);

		new void GetUnknown([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

		new void SetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value);

		new void DeleteItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

		new void DeleteAllItems();

		new void SetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, int unValue);

		new void SetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, long unValue);

		new void SetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

		new void SetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

		new void SetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPWStr)] string wszValue);

		new void SetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		new void SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

		new void LockStore();

		new void UnlockStore();

		new void GetCount(out int pcItems);

		new void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] IntPtr pValue);

		new void CopyAllItems([In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest);

		void ActivateObject([In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

		void ShutdownObject();

		void DetachObject();
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("2CD2D921-C447-44A7-A13C-4ADABFC247E3")]
	public interface IMFAttributes
	{
		void GetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] IntPtr pValue);

		void GetItemType([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pType);

		void CompareItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		void Compare([MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs, int matchType, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		void GetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int punValue);

		void GetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out long punValue);

		void GetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

		void GetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

		void GetStringLength([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcchLength);

		void GetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		void GetAllocatedString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue, out int pcchLength);

		void GetBlobSize([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcbBlobSize);

		void GetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		void GetAllocatedBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ip, out int pcbSize);

		void GetUnknown([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

		void SetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr Value);

		void DeleteItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

		void DeleteAllItems();

		void SetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, int unValue);

		void SetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, long unValue);

		void SetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

		void SetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

		void SetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPWStr)] string wszValue);

		void SetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		void SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

		void LockStore();

		void UnlockStore();

		void GetCount(out int pcItems);

		void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] IntPtr pValue);

		void CopyAllItems([In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest);
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("ad4c1b00-4bf7-422f-9175-756693d9130d")]
	public interface IMFByteStream
	{
		void GetCapabilities(ref int pdwCapabiities);

		void GetLength(ref long pqwLength);

		void SetLength(long qwLength);

		void GetCurrentPosition(ref long pqwPosition);

		void SetCurrentPosition(long qwPosition);

		void IsEndOfStream([MarshalAs(UnmanagedType.Bool)] ref bool pfEndOfStream);

		void Read(IntPtr pb, int cb, ref int pcbRead);

		void BeginRead(IntPtr pb, int cb, IntPtr pCallback, IntPtr punkState);

		void EndRead(IntPtr pResult, ref int pcbRead);

		void Write(IntPtr pb, int cb, ref int pcbWritten);

		void BeginWrite(IntPtr pb, int cb, IntPtr pCallback, IntPtr punkState);

		void EndWrite(IntPtr pResult, ref int pcbWritten);

		void Seek(int SeekOrigin, long llSeekOffset, int dwSeekFlags, ref long pqwCurrentPosition);

		void Flush();

		void Close();
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("5BC8A76B-869A-46A3-9B03-FA218A66AEBE")]
	public interface IMFCollection
	{
		void GetElementCount(out int pcElements);

		void GetElement([In] int dwElementIndex, [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement);

		void AddElement([In][MarshalAs(UnmanagedType.IUnknown)] object pUnkElement);

		void RemoveElement([In] int dwElementIndex, [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement);

		void InsertElementAt([In] int dwIndex, [In][MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

		void RemoveAllElements();
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("045FA593-8799-42b8-BC8D-8968C6453507")]
	public interface IMFMediaBuffer
	{
		void Lock(out IntPtr ppbBuffer, out int pcbMaxLength, out int pcbCurrentLength);

		void Unlock();

		void GetCurrentLength(out int pcbCurrentLength);

		void SetCurrentLength(int cbCurrentLength);

		void GetMaxLength(out int pcbMaxLength);
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3")]
	public interface IMFMediaEvent : IMFAttributes
	{
		new void GetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] IntPtr pValue);

		new void GetItemType([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pType);

		new void CompareItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void Compare([MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs, int matchType, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void GetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int punValue);

		new void GetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out long punValue);

		new void GetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

		new void GetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

		new void GetStringLength([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcchLength);

		new void GetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		new void GetAllocatedString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue, out int pcchLength);

		new void GetBlobSize([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcbBlobSize);

		new void GetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		new void GetAllocatedBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ip, out int pcbSize);

		new void GetUnknown([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

		new void SetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value);

		new void DeleteItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

		new void DeleteAllItems();

		new void SetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, int unValue);

		new void SetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, long unValue);

		new void SetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

		new void SetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

		new void SetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPWStr)] string wszValue);

		new void SetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		new void SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

		new void LockStore();

		new void UnlockStore();

		new void GetCount(out int pcItems);

		new void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] IntPtr pValue);

		new void CopyAllItems([In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest);

		void GetType(out MediaEventType pmet);

		void GetExtendedType(out Guid pguidExtendedType);

		void GetStatus([MarshalAs(UnmanagedType.Error)] out int phrStatus);

		void GetValue([Out] IntPtr pvValue);
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("44AE0FA8-EA31-4109-8D2E-4CAE4997C555")]
	public interface IMFMediaType : IMFAttributes
	{
		new void GetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] IntPtr pValue);

		new void GetItemType([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pType);

		new void CompareItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void Compare([MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs, int matchType, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void GetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int punValue);

		new void GetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out long punValue);

		new void GetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

		new void GetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

		new void GetStringLength([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcchLength);

		new void GetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		new void GetAllocatedString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue, out int pcchLength);

		new void GetBlobSize([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcbBlobSize);

		new void GetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		new void GetAllocatedBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ip, out int pcbSize);

		new void GetUnknown([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

		new void SetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value);

		new void DeleteItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

		new void DeleteAllItems();

		new void SetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, int unValue);

		new void SetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, long unValue);

		new void SetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

		new void SetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

		new void SetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPWStr)] string wszValue);

		new void SetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		new void SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

		new void LockStore();

		new void UnlockStore();

		new void GetCount(out int pcItems);

		new void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] IntPtr pValue);

		new void CopyAllItems([In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest);

		void GetMajorType(out Guid pguidMajorType);

		void IsCompressedFormat([MarshalAs(UnmanagedType.Bool)] out bool pfCompressed);

		[PreserveSig]
		int IsEqual([In][MarshalAs(UnmanagedType.Interface)] IMFMediaType pIMediaType, ref int pdwFlags);

		void GetRepresentation([In] Guid guidRepresentation, ref IntPtr ppvRepresentation);

		void FreeRepresentation([In] Guid guidRepresentation, [In] IntPtr pvRepresentation);
	}
	[ComImport]
	[Guid("E7FE2E12-661C-40DA-92F9-4F002AB67627")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMFReadWriteClassFactory
	{
		void CreateInstanceFromURL([In][MarshalAs(UnmanagedType.LPStruct)] Guid clsid, [In][MarshalAs(UnmanagedType.LPWStr)] string pwszURL, [In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pAttributes, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);

		void CreateInstanceFromObject([In][MarshalAs(UnmanagedType.LPStruct)] Guid clsid, [In][MarshalAs(UnmanagedType.IUnknown)] object punkObject, [In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pAttributes, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
	}
	[ComImport]
	[Guid("48e2ed0f-98c2-4a37-bed5-166312ddd83f")]
	public class MFReadWriteClassFactory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MFReadWriteClassFactory();
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("c40a00f2-b93a-4d80-ae8c-5a1c634f58e4")]
	public interface IMFSample : IMFAttributes
	{
		new void GetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] IntPtr pValue);

		new void GetItemType([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pType);

		new void CompareItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void Compare([MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs, int matchType, [MarshalAs(UnmanagedType.Bool)] out bool pbResult);

		new void GetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int punValue);

		new void GetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out long punValue);

		new void GetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

		new void GetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

		new void GetStringLength([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcchLength);

		new void GetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue, int cchBufSize, out int pcchLength);

		new void GetAllocatedString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue, out int pcchLength);

		new void GetBlobSize([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out int pcbBlobSize);

		new void GetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] pBuf, int cbBufSize, out int pcbBlobSize);

		new void GetAllocatedBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ip, out int pcbSize);

		new void GetUnknown([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

		new void SetItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr value);

		new void DeleteItem([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

		new void DeleteAllItems();

		new void SetUINT32([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, int unValue);

		new void SetUINT64([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, long unValue);

		new void SetDouble([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

		new void SetGUID([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

		new void SetString([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPWStr)] string wszValue);

		new void SetBlob([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

		new void SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

		new void LockStore();

		new void UnlockStore();

		new void GetCount(out int pcItems);

		new void GetItemByIndex(int unIndex, out Guid pGuidKey, [In][Out] IntPtr pValue);

		new void CopyAllItems([In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest);

		void GetSampleFlags(out int pdwSampleFlags);

		void SetSampleFlags(int dwSampleFlags);

		void GetSampleTime(out long phnsSampletime);

		void SetSampleTime(long hnsSampleTime);

		void GetSampleDuration(out long phnsSampleDuration);

		void SetSampleDuration(long hnsSampleDuration);

		void GetBufferCount(out int pdwBufferCount);

		void GetBufferByIndex(int dwIndex, out IMFMediaBuffer ppBuffer);

		void ConvertToContiguousBuffer(out IMFMediaBuffer ppBuffer);

		void AddBuffer(IMFMediaBuffer pBuffer);

		void RemoveBufferByIndex(int dwIndex);

		void RemoveAllBuffers();

		void GetTotalLength(out int pcbTotalLength);

		void CopyToBuffer(IMFMediaBuffer pBuffer);
	}
	[ComImport]
	[Guid("3137f1cd-fe5e-4805-a5d8-fb477448cb3d")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMFSinkWriter
	{
		void AddStream([In][MarshalAs(UnmanagedType.Interface)] IMFMediaType pTargetMediaType, out int pdwStreamIndex);

		void SetInputMediaType([In] int dwStreamIndex, [In][MarshalAs(UnmanagedType.Interface)] IMFMediaType pInputMediaType, [In][MarshalAs(UnmanagedType.Interface)] IMFAttributes pEncodingParameters);

		void BeginWriting();

		void WriteSample([In] int dwStreamIndex, [In][MarshalAs(UnmanagedType.Interface)] IMFSample pSample);

		void SendStreamTick([In] int dwStreamIndex, [In] long llTimestamp);

		void PlaceMarker([In] int dwStreamIndex, [In] IntPtr pvContext);

		void NotifyEndOfSegment([In] int dwStreamIndex);

		void Flush([In] int dwStreamIndex);

		void DoFinalize();

		void GetServiceForStream([In] int dwStreamIndex, [In] ref Guid guidService, [In] ref Guid riid, out IntPtr ppvObject);

		void GetStatistics([In] int dwStreamIndex, [In][Out] MF_SINK_WRITER_STATISTICS pStats);
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")]
	public interface IMFSourceReader
	{
		void GetStreamSelection([In] int dwStreamIndex, [MarshalAs(UnmanagedType.Bool)] out bool pSelected);

		void SetStreamSelection([In] int dwStreamIndex, [In][MarshalAs(UnmanagedType.Bool)] bool pSelected);

		void GetNativeMediaType([In] int dwStreamIndex, [In] int dwMediaTypeIndex, out IMFMediaType ppMediaType);

		void GetCurrentMediaType([In] int dwStreamIndex, out IMFMediaType ppMediaType);

		void SetCurrentMediaType([In] int dwStreamIndex, IntPtr pdwReserved, [In] IMFMediaType pMediaType);

		void SetCurrentPosition([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidTimeFormat, [In] IntPtr varPosition);

		void ReadSample([In] int dwStreamIndex, [In] int dwControlFlags, out int pdwActualStreamIndex, out MF_SOURCE_READER_FLAG pdwStreamFlags, out ulong pllTimestamp, out IMFSample ppSample);

		void Flush([In] int dwStreamIndex);

		void GetServiceForStream([In] int dwStreamIndex, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidService, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppvObject);

		[PreserveSig]
		int GetPresentationAttribute([In] int dwStreamIndex, [In][MarshalAs(UnmanagedType.LPStruct)] Guid guidAttribute, [Out] IntPtr pvarAttribute);
	}
	[Flags]
	public enum MF_SOURCE_READER_FLAG
	{
		None = 0,
		MF_SOURCE_READERF_ERROR = 1,
		MF_SOURCE_READERF_ENDOFSTREAM = 2,
		MF_SOURCE_READERF_NEWSTREAM = 4,
		MF_SOURCE_READERF_NATIVEMEDIATYPECHANGED = 0x10,
		MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED = 0x20,
		MF_SOURCE_READERF_STREAMTICK = 0x100,
		MF_SOURCE_READERF_ALLEFFECTSREMOVED = 0x200
	}
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("bf94c121-5b05-4e6f-8000-ba598961414d")]
	public interface IMFTransform
	{
		void GetStreamLimits(out int pdwInputMinimum, out int pdwInputMaximum, out int pdwOutputMinimum, out int pdwOutputMaximum);

		void GetStreamCount(out int pcInputStreams, out int pcOutputStreams);

		void GetStreamIds([In] int dwInputIdArraySize, [In][Out] IntPtr pdwInputIDs, [In] int dwOutputIdArraySize, [In][Out] IntPtr pdwOutputIDs);

		void GetInputStreamInfo([In] int dwInputStreamId, out MFT_INPUT_STREAM_INFO pStreamInfo);

		void GetOutputStreamInfo([In] int dwOutputStreamId, out MFT_OUTPUT_STREAM_INFO pStreamInfo);

		void GetAttributes(out IMFAttributes pAttributes);

		void GetInputStreamAttributes([In] int dwInputStreamId, out IMFAttributes pAttributes);

		void GetOutputStreamAttributes([In] int dwOutputStreamId, out IMFAttributes pAttributes);

		void DeleteInputStream([In] int dwOutputStreamId);

		void AddInputStreams([In] int cStreams, [In] IntPtr adwStreamIDs);

		void GetInputAvailableType([In] int dwInputStreamId, [In] int dwTypeIndex, out IMFMediaType ppType);

		void GetOutputAvailableType([In] int dwOutputStreamId, [In] int dwTypeIndex, out IMFMediaType ppType);

		void SetInputType([In] int dwInputStreamId, [In] IMFMediaType pType, [In] _MFT_SET_TYPE_FLAGS dwFlags);

		void SetOutputType([In] int dwOutputStreamId, [In] IMFMediaType pType, [In] _MFT_SET_TYPE_FLAGS dwFlags);

		void GetInputCurrentType([In] int dwInputStreamId, out IMFMediaType ppType);

		void GetOutputCurrentType([In] int dwOutputStreamId, out IMFMediaType ppType);

		void GetInputStatus([In] int dwInputStreamId, out _MFT_INPUT_STATUS_FLAGS pdwFlags);

		void GetOutputStatus([In] int dwInputStreamId, out _MFT_OUTPUT_STATUS_FLAGS pdwFlags);

		void SetOutputBounds([In] long hnsLowerBound, [In] long hnsUpperBound);

		void ProcessEvent([In] int dwInputStreamId, [In] IMFMediaEvent pEvent);

		void ProcessMessage([In] MFT_MESSAGE_TYPE eMessage, [In] IntPtr ulParam);

		void ProcessInput([In] int dwInputStreamId, [In] IMFSample pSample, int dwFlags);

		[PreserveSig]
		int ProcessOutput([In] _MFT_PROCESS_OUTPUT_FLAGS dwFlags, [In] int cOutputBufferCount, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MFT_OUTPUT_DATA_BUFFER[] pOutputSamples, out _MFT_PROCESS_OUTPUT_STATUS pdwStatus);
	}
	public enum MediaEventType
	{
		MEUnknown = 0,
		MEError = 1,
		MEExtendedType = 2,
		MENonFatalError = 3,
		MESessionUnknown = 100,
		MESessionTopologySet = 101,
		MESessionTopologiesCleared = 102,
		MESessionStarted = 103,
		MESessionPaused = 104,
		MESessionStopped = 105,
		MESessionClosed = 106,
		MESessionEnded = 107,
		MESessionRateChanged = 108,
		MESessionScrubSampleComplete = 109,
		MESessionCapabilitiesChanged = 110,
		MESessionTopologyStatus = 111,
		MESessionNotifyPresentationTime = 112,
		MENewPresentation = 113,
		MELicenseAcquisitionStart = 114,
		MELicenseAcquisitionCompleted = 115,
		MEIndividualizationStart = 116,
		MEIndividualizationCompleted = 117,
		MEEnablerProgress = 118,
		MEEnablerCompleted = 119,
		MEPolicyError = 120,
		MEPolicyReport = 121,
		MEBufferingStarted = 122,
		MEBufferingStopped = 123,
		MEConnectStart = 124,
		MEConnectEnd = 125,
		MEReconnectStart = 126,
		MEReconnectEnd = 127,
		MERendererEvent = 128,
		MESessionStreamSinkFormatChanged = 129,
		MESourceUnknown = 200,
		MESourceStarted = 201,
		MEStreamStarted = 202,
		MESourceSeeked = 203,
		MEStreamSeeked = 204,
		MENewStream = 205,
		MEUpdatedStream = 206,
		MESourceStopped = 207,
		MEStreamStopped = 208,
		MESourcePaused = 209,
		MEStreamPaused = 210,
		MEEndOfPresentation = 211,
		MEEndOfStream = 212,
		MEMediaSample = 213,
		MEStreamTick = 214,
		MEStreamThinMode = 215,
		MEStreamFormatChanged = 216,
		MESourceRateChanged = 217,
		MEEndOfPresentationSegment = 218,
		MESourceCharacteristicsChanged = 219,
		MESourceRateChangeRequested = 220,
		MESourceMetadataChanged = 221,
		MESequencerSourceTopologyUpdated = 222,
		MESinkUnknown = 300,
		MEStreamSinkStarted = 301,
		MEStreamSinkStopped = 302,
		MEStreamSinkPaused = 303,
		MEStreamSinkRateChanged = 304,
		MEStreamSinkRequestSample = 305,
		MEStreamSinkMarker = 306,
		MEStreamSinkPrerolled = 307,
		MEStreamSinkScrubSampleComplete = 308,
		MEStreamSinkFormatChanged = 309,
		MEStreamSinkDeviceChanged = 310,
		MEQualityNotify = 311,
		MESinkInvalidated = 312,
		MEAudioSessionNameChanged = 313,
		MEAudioSessionVolumeChanged = 314,
		MEAudioSessionDeviceRemoved = 315,
		MEAudioSessionServerShutdown = 316,
		MEAudioSessionGroupingParamChanged = 317,
		MEAudioSessionIconChanged = 318,
		MEAudioSessionFormatChanged = 319,
		MEAudioSessionDisconnected = 320,
		MEAudioSessionExclusiveModeOverride = 321,
		METrustUnknown = 400,
		MEPolicyChanged = 401,
		MEContentProtectionMessage = 402,
		MEPolicySet = 403,
		MEWMDRMLicenseBackupCompleted = 500,
		MEWMDRMLicenseBackupProgress = 501,
		MEWMDRMLicenseRestoreCompleted = 502,
		MEWMDRMLicenseRestoreProgress = 503,
		MEWMDRMLicenseAcquisitionCompleted = 506,
		MEWMDRMIndividualizationCompleted = 508,
		MEWMDRMIndividualizationProgress = 513,
		MEWMDRMProximityCompleted = 514,
		MEWMDRMLicenseStoreCleaned = 515,
		MEWMDRMRevocationDownloadCompleted = 516,
		METransformUnknown = 600,
		METransformNeedInput = 601,
		METransformHaveOutput = 602,
		METransformDrainComplete = 603,
		METransformMarker = 604
	}
	public static class MediaFoundationAttributes
	{
		public static readonly Guid MF_TRANSFORM_ASYNC = new Guid("f81a699a-649a-497d-8c73-29f8fed6ad7a");

		public static readonly Guid MF_TRANSFORM_ASYNC_UNLOCK = new Guid("e5666d6b-3422-4eb6-a421-da7db1f8e207");

		[FieldDescription("Transform Flags")]
		public static readonly Guid MF_TRANSFORM_FLAGS_Attribute = new Guid("9359bb7e-6275-46c4-a025-1c01e45f1a86");

		[FieldDescription("Transform Category")]
		public static readonly Guid MF_TRANSFORM_CATEGORY_Attribute = new Guid("ceabba49-506d-4757-a6ff-66c184987e4e");

		[FieldDescription("Class identifier")]
		public static readonly Guid MFT_TRANSFORM_CLSID_Attribute = new Guid("6821c42b-65a4-4e82-99bc-9a88205ecd0c");

		[FieldDescription("Input Types")]
		public static readonly Guid MFT_INPUT_TYPES_Attributes = new Guid("4276c9b1-759d-4bf3-9cd0-0d723d138f96");

		[FieldDescription("Output Types")]
		public static readonly Guid MFT_OUTPUT_TYPES_Attributes = new Guid("8eae8cf3-a44f-4306-ba5c-bf5dda242818");

		public static readonly Guid MFT_ENUM_HARDWARE_URL_Attribute = new Guid("2fb866ac-b078-4942-ab6c-003d05cda674");

		[FieldDescription("Name")]
		public static readonly Guid MFT_FRIENDLY_NAME_Attribute = new Guid("314ffbae-5b41-4c95-9c19-4e7d586face3");

		public static readonly Guid MFT_CONNECTED_STREAM_ATTRIBUTE = new Guid("71eeb820-a59f-4de2-bcec-38db1dd611a4");

		public static readonly Guid MFT_CONNECTED_TO_HW_STREAM = new Guid("34e6e728-06d6-4491-a553-4795650db912");

		[FieldDescription("Preferred Output Format")]
		public static readonly Guid MFT_PREFERRED_OUTPUTTYPE_Attribute = new Guid("7e700499-396a-49ee-b1b4-f628021e8c9d");

		public static readonly Guid MFT_PROCESS_LOCAL_Attribute = new Guid("543186e4-4649-4e65-b588-4aa352aff379");

		public static readonly Guid MFT_PREFERRED_ENCODER_PROFILE = new Guid("53004909-1ef5-46d7-a18e-5a75f8b5905f");

		public static readonly Guid MFT_HW_TIMESTAMP_WITH_QPC_Attribute = new Guid("8d030fb8-cc43-4258-a22e-9210bef89be4");

		public static readonly Guid MFT_FIELDOFUSE_UNLOCK_Attribute = new Guid("8ec2e9fd-9148-410d-831e-702439461a8e");

		public static readonly Guid MFT_CODEC_MERIT_Attribute = new Guid("88a7cb15-7b07-4a34-9128-e64c6703c4d3");

		public static readonly Guid MFT_ENUM_TRANSCODE_ONLY_ATTRIBUTE = new Guid("111ea8cd-b62a-4bdb-89f6-67ffcdc2458b");

		[FieldDescription("PMP Host Context")]
		public static readonly Guid MF_PD_PMPHOST_CONTEXT = new Guid("6c990d31-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("App Context")]
		public static readonly Guid MF_PD_APP_CONTEXT = new Guid("6c990d32-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Duration")]
		public static readonly Guid MF_PD_DURATION = new Guid("6c990d33-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Total File Size")]
		public static readonly Guid MF_PD_TOTAL_FILE_SIZE = new Guid("6c990d34-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Audio encoding bitrate")]
		public static readonly Guid MF_PD_AUDIO_ENCODING_BITRATE = new Guid("6c990d35-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Video Encoding Bitrate")]
		public static readonly Guid MF_PD_VIDEO_ENCODING_BITRATE = new Guid("6c990d36-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("MIME Type")]
		public static readonly Guid MF_PD_MIME_TYPE = new Guid("6c990d37-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Last Modified Time")]
		public static readonly Guid MF_PD_LAST_MODIFIED_TIME = new Guid("6c990d38-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Element ID")]
		public static readonly Guid MF_PD_PLAYBACK_ELEMENT_ID = new Guid("6c990d39-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Preferred Language")]
		public static readonly Guid MF_PD_PREFERRED_LANGUAGE = new Guid("6c990d3a-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Playback boundary time")]
		public static readonly Guid MF_PD_PLAYBACK_BOUNDARY_TIME = new Guid("6c990d3b-bb8e-477a-8598-0d5d96fcd88a");

		[FieldDescription("Audio is variable bitrate")]
		public static readonly Guid MF_PD_AUDIO_ISVARIABLEBITRATE = new Guid("33026ee0-e387-4582-ae0a-34a2ad3baa18");

		[FieldDescription("Major Media Type")]
		public static readonly Guid MF_MT_MAJOR_TYPE = new Guid("48eba18e-f8c9-4687-bf11-0a74c9f96a8f");

		[FieldDescription("Media Subtype")]
		public static readonly Guid MF_MT_SUBTYPE = new Guid("f7e34c9a-42e8-4714-b74b-cb29d72c35e5");

		[FieldDescription("Audio block alignment")]
		public static readonly Guid MF_MT_AUDIO_BLOCK_ALIGNMENT = new Guid("322de230-9eeb-43bd-ab7a-ff412251541d");

		[FieldDescription("Audio average bytes per second")]
		public static readonly Guid MF_MT_AUDIO_AVG_BYTES_PER_SECOND = new Guid("1aab75c8-cfef-451c-ab95-ac034b8e1731");

		[FieldDescription("Audio number of channels")]
		public static readonly Guid MF_MT_AUDIO_NUM_CHANNELS = new Guid("37e48bf5-645e-4c5b-89de-ada9e29b696a");

		[FieldDescription("Audio samples per second")]
		public static readonly Guid MF_MT_AUDIO_SAMPLES_PER_SECOND = new Guid("5faeeae7-0290-4c31-9e8a-c534f68d9dba");

		[FieldDescription("Audio bits per sample")]
		public static readonly Guid MF_MT_AUDIO_BITS_PER_SAMPLE = new Guid("f2deb57f-40fa-4764-aa33-ed4f2d1ff669");

		[FieldDescription("Enable Hardware Transforms")]
		public static readonly Guid MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS = new Guid("a634a91c-822b-41b9-a494-4de4643612b0");

		[FieldDescription("User data")]
		public static readonly Guid MF_MT_USER_DATA = new Guid("b6bc765f-4c3b-40a4-bd51-2535b66fe09d");

		[FieldDescription("All samples independent")]
		public static readonly Guid MF_MT_ALL_SAMPLES_INDEPENDENT = new Guid("c9173739-5e56-461c-b713-46fb995cb95f");

		[FieldDescription("Fixed size samples")]
		public static readonly Guid MF_MT_FIXED_SIZE_SAMPLES = new Guid("b8ebefaf-b718-4e04-b0a9-116775e3321b");

		[FieldDescription("DirectShow Format Guid")]
		public static readonly Guid MF_MT_AM_FORMAT_TYPE = new Guid("73d1072d-1870-4174-a063-29ff4ff6c11e");

		[FieldDescription("Preferred legacy format structure")]
		public static readonly Guid MF_MT_AUDIO_PREFER_WAVEFORMATEX = new Guid("a901aaba-e037-458a-bdf6-545be2074042");

		[FieldDescription("Is Compressed")]
		public static readonly Guid MF_MT_COMPRESSED = new Guid("3afd0cee-18f2-4ba5-a110-8bea502e1f92");

		[FieldDescription("Average bitrate")]
		public static readonly Guid MF_MT_AVG_BITRATE = new Guid("20332624-fb0d-4d9e-bd0d-cbf6786c102e");

		[FieldDescription("AAC payload type")]
		public static readonly Guid MF_MT_AAC_PAYLOAD_TYPE = new Guid("bfbabe79-7434-4d1c-94f0-72a3b9e17188");

		[FieldDescription("AAC Audio Profile Level Indication")]
		public static readonly Guid MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION = new Guid("7632f0e6-9538-4d61-acda-ea29c8c14456");
	}
	public static class MediaFoundationErrors
	{
		public const int MF_E_PLATFORM_NOT_INITIALIZED = -1072875856;

		public const int MF_E_BUFFERTOOSMALL = -1072875855;

		public const int MF_E_INVALIDREQUEST = -1072875854;

		public const int MF_E_INVALIDSTREAMNUMBER = -1072875853;

		public const int MF_E_INVALIDMEDIATYPE = -1072875852;

		public const int MF_E_NOTACCEPTING = -1072875851;

		public const int MF_E_NOT_INITIALIZED = -1072875850;

		public const int MF_E_UNSUPPORTED_REPRESENTATION = -1072875849;

		public const int MF_E_NO_MORE_TYPES = -1072875847;

		public const int MF_E_UNSUPPORTED_SERVICE = -1072875846;

		public const int MF_E_UNEXPECTED = -1072875845;

		public const int MF_E_INVALIDNAME = -1072875844;

		public const int MF_E_INVALIDTYPE = -1072875843;

		public const int MF_E_INVALID_FILE_FORMAT = -1072875842;

		public const int MF_E_INVALIDINDEX = -1072875841;

		public const int MF_E_INVALID_TIMESTAMP = -1072875840;

		public const int MF_E_UNSUPPORTED_SCHEME = -1072875837;

		public const int MF_E_UNSUPPORTED_BYTESTREAM_TYPE = -1072875836;

		public const int MF_E_UNSUPPORTED_TIME_FORMAT = -1072875835;

		public const int MF_E_NO_SAMPLE_TIMESTAMP = -1072875832;

		public const int MF_E_NO_SAMPLE_DURATION = -1072875831;

		public const int MF_E_INVALID_STREAM_DATA = -1072875829;

		public const int MF_E_RT_UNAVAILABLE = -1072875825;

		public const int MF_E_UNSUPPORTED_RATE = -1072875824;

		public const int MF_E_THINNING_UNSUPPORTED = -1072875823;

		public const int MF_E_REVERSE_UNSUPPORTED = -1072875822;

		public const int MF_E_UNSUPPORTED_RATE_TRANSITION = -1072875821;

		public const int MF_E_RATE_CHANGE_PREEMPTED = -1072875820;

		public const int MF_E_NOT_FOUND = -1072875819;

		public const int MF_E_NOT_AVAILABLE = -1072875818;

		public const int MF_E_NO_CLOCK = -1072875817;

		public const int MF_S_MULTIPLE_BEGIN = 866008;

		public const int MF_E_MULTIPLE_BEGIN = -1072875815;

		public const int MF_E_MULTIPLE_SUBSCRIBERS = -1072875814;

		public const int MF_E_TIMER_ORPHANED = -1072875813;

		public const int MF_E_STATE_TRANSITION_PENDING = -1072875812;

		public const int MF_E_UNSUPPORTED_STATE_TRANSITION = -1072875811;

		public const int MF_E_UNRECOVERABLE_ERROR_OCCURRED = -1072875810;

		public const int MF_E_SAMPLE_HAS_TOO_MANY_BUFFERS = -1072875809;

		public const int MF_E_SAMPLE_NOT_WRITABLE = -1072875808;

		public const int MF_E_INVALID_KEY = -1072875806;

		public const int MF_E_BAD_STARTUP_VERSION = -1072875805;

		public const int MF_E_UNSUPPORTED_CAPTION = -1072875804;

		public const int MF_E_INVALID_POSITION = -1072875803;

		public const int MF_E_ATTRIBUTENOTFOUND = -1072875802;

		public const int MF_E_PROPERTY_TYPE_NOT_ALLOWED = -1072875801;

		public const int MF_E_PROPERTY_TYPE_NOT_SUPPORTED = -1072875800;

		public const int MF_E_PROPERTY_EMPTY = -1072875799;

		public const int MF_E_PROPERTY_NOT_EMPTY = -1072875798;

		public const int MF_E_PROPERTY_VECTOR_NOT_ALLOWED = -1072875797;

		public const int MF_E_PROPERTY_VECTOR_REQUIRED = -1072875796;

		public const int MF_E_OPERATION_CANCELLED = -1072875795;

		public const int MF_E_BYTESTREAM_NOT_SEEKABLE = -1072875794;

		public const int MF_E_DISABLED_IN_SAFEMODE = -1072875793;

		public const int MF_E_CANNOT_PARSE_BYTESTREAM = -1072875792;

		public const int MF_E_SOURCERESOLVER_MUTUALLY_EXCLUSIVE_FLAGS = -1072875791;

		public const int MF_E_MEDIAPROC_WRONGSTATE = -1072875790;

		public const int MF_E_RT_THROUGHPUT_NOT_AVAILABLE = -1072875789;

		public const int MF_E_RT_TOO_MANY_CLASSES = -1072875788;

		public const int MF_E_RT_WOULDBLOCK = -1072875787;

		public const int MF_E_NO_BITPUMP = -1072875786;

		public const int MF_E_RT_OUTOFMEMORY = -1072875785;

		public const int MF_E_RT_WORKQUEUE_CLASS_NOT_SPECIFIED = -1072875784;

		public const int MF_E_INSUFFICIENT_BUFFER = -1072860816;

		public const int MF_E_CANNOT_CREATE_SINK = -1072875782;

		public const int MF_E_BYTESTREAM_UNKNOWN_LENGTH = -1072875781;

		public const int MF_E_SESSION_PAUSEWHILESTOPPED = -1072875780;

		public const int MF_S_ACTIVATE_REPLACED = 866045;

		public const int MF_E_FORMAT_CHANGE_NOT_SUPPORTED = -1072875778;

		public const int MF_E_INVALID_WORKQUEUE = -1072875777;

		public const int MF_E_DRM_UNSUPPORTED = -1072875776;

		public const int MF_E_UNAUTHORIZED = -1072875775;

		public const int MF_E_OUT_OF_RANGE = -1072875774;

		public const int MF_E_INVALID_CODEC_MERIT = -1072875773;

		public const int MF_E_HW_MFT_FAILED_START_STREAMING = -1072875772;

		public const int MF_S_ASF_PARSEINPROGRESS = 1074608792;

		public const int MF_E_ASF_PARSINGINCOMPLETE = -1072874856;

		public const int MF_E_ASF_MISSINGDATA = -1072874855;

		public const int MF_E_ASF_INVALIDDATA = -1072874854;

		public const int MF_E_ASF_OPAQUEPACKET = -1072874853;

		public const int MF_E_ASF_NOINDEX = -1072874852;

		public const int MF_E_ASF_OUTOFRANGE = -1072874851;

		public const int MF_E_ASF_INDEXNOTLOADED = -1072874850;

		public const int MF_E_ASF_TOO_MANY_PAYLOADS = -1072874849;

		public const int MF_E_ASF_UNSUPPORTED_STREAM_TYPE = -1072874848;

		public const int MF_E_ASF_DROPPED_PACKET = -1072874847;

		public const int MF_E_NO_EVENTS_AVAILABLE = -1072873856;

		public const int MF_E_INVALID_STATE_TRANSITION = -1072873854;

		public const int MF_E_END_OF_STREAM = -1072873852;

		public const int MF_E_SHUTDOWN = -1072873851;

		public const int MF_E_MP3_NOTFOUND = -1072873850;

		public const int MF_E_MP3_OUTOFDATA = -1072873849;

		public const int MF_E_MP3_NOTMP3 = -1072873848;

		public const int MF_E_MP3_NOTSUPPORTED = -1072873847;

		public const int MF_E_NO_DURATION = -1072873846;

		public const int MF_E_INVALID_FORMAT = -1072873844;

		public const int MF_E_PROPERTY_NOT_FOUND = -1072873843;

		public const int MF_E_PROPERTY_READ_ONLY = -1072873842;

		public const int MF_E_PROPERTY_NOT_ALLOWED = -1072873841;

		public const int MF_E_MEDIA_SOURCE_NOT_STARTED = -1072873839;

		public const int MF_E_UNSUPPORTED_FORMAT = -1072873832;

		public const int MF_E_MP3_BAD_CRC = -1072873831;

		public const int MF_E_NOT_PROTECTED = -1072873830;

		public const int MF_E_MEDIA_SOURCE_WRONGSTATE = -1072873829;

		public const int MF_E_MEDIA_SOURCE_NO_STREAMS_SELECTED = -1072873828;

		public const int MF_E_CANNOT_FIND_KEYFRAME_SAMPLE = -1072873827;

		public const int MF_E_NETWORK_RESOURCE_FAILURE = -1072872856;

		public const int MF_E_NET_WRITE = -1072872855;

		public const int MF_E_NET_READ = -1072872854;

		public const int MF_E_NET_REQUIRE_NETWORK = -1072872853;

		public const int MF_E_NET_REQUIRE_ASYNC = -1072872852;

		public const int MF_E_NET_BWLEVEL_NOT_SUPPORTED = -1072872851;

		public const int MF_E_NET_STREAMGROUPS_NOT_SUPPORTED = -1072872850;

		public const int MF_E_NET_MANUALSS_NOT_SUPPORTED = -1072872849;

		public const int MF_E_NET_INVALID_PRESENTATION_DESCRIPTOR = -1072872848;

		public const int MF_E_NET_CACHESTREAM_NOT_FOUND = -1072872847;

		public const int MF_I_MANUAL_PROXY = 1074610802;

		public const int MF_E_NET_REQUIRE_INPUT = -1072872844;

		public const int MF_E_NET_REDIRECT = -1072872843;

		public const int MF_E_NET_REDIRECT_TO_PROXY = -1072872842;

		public const int MF_E_NET_TOO_MANY_REDIRECTS = -1072872841;

		public const int MF_E_NET_TIMEOUT = -1072872840;

		public const int MF_E_NET_CLIENT_CLOSE = -1072872839;

		public const int MF_E_NET_BAD_CONTROL_DATA = -1072872838;

		public const int MF_E_NET_INCOMPATIBLE_SERVER = -1072872837;

		public const int MF_E_NET_UNSAFE_URL = -1072872836;

		public const int MF_E_NET_CACHE_NO_DATA = -1072872835;

		public const int MF_E_NET_EOL = -1072872834;

		public const int MF_E_NET_BAD_REQUEST = -1072872833;

		public const int MF_E_NET_INTERNAL_SERVER_ERROR = -1072872832;

		public const int MF_E_NET_SESSION_NOT_FOUND = -1072872831;

		public const int MF_E_NET_NOCONNECTION = -1072872830;

		public const int MF_E_NET_CONNECTION_FAILURE = -1072872829;

		public const int MF_E_NET_INCOMPATIBLE_PUSHSERVER = -1072872828;

		public const int MF_E_NET_SERVER_ACCESSDENIED = -1072872827;

		public const int MF_E_NET_PROXY_ACCESSDENIED = -1072872826;

		public const int MF_E_NET_CANNOTCONNECT = -1072872825;

		public const int MF_E_NET_INVALID_PUSH_TEMPLATE = -1072872824;

		public const int MF_E_NET_INVALID_PUSH_PUBLISHING_POINT = -1072872823;

		public const int MF_E_NET_BUSY = -1072872822;

		public const int MF_E_NET_RESOURCE_GONE = -1072872821;

		public const int MF_E_NET_ERROR_FROM_PROXY = -1072872820;

		public const int MF_E_NET_PROXY_TIMEOUT = -1072872819;

		public const int MF_E_NET_SERVER_UNAVAILABLE = -1072872818;

		public const int MF_E_NET_TOO_MUCH_DATA = -1072872817;

		public const int MF_E_NET_SESSION_INVALID = -1072872816;

		public const int MF_E_OFFLINE_MODE = -1072872815;

		public const int MF_E_NET_UDP_BLOCKED = -1072872814;

		public const int MF_E_NET_UNSUPPORTED_CONFIGURATION = -1072872813;

		public const int MF_E_NET_PROTOCOL_DISABLED = -1072872812;

		public const int MF_E_ALREADY_INITIALIZED = -1072871856;

		public const int MF_E_BANDWIDTH_OVERRUN = -1072871855;

		public const int MF_E_LATE_SAMPLE = -1072871854;

		public const int MF_E_FLUSH_NEEDED = -1072871853;

		public const int MF_E_INVALID_PROFILE = -1072871852;

		public const int MF_E_INDEX_NOT_COMMITTED = -1072871851;

		public const int MF_E_NO_INDEX = -1072871850;

		public const int MF_E_CANNOT_INDEX_IN_PLACE = -1072871849;

		public const int MF_E_MISSING_ASF_LEAKYBUCKET = -1072871848;

		public const int MF_E_INVALID_ASF_STREAMID = -1072871847;

		public const int MF_E_STREAMSINK_REMOVED = -1072870856;

		public const int MF_E_STREAMSINKS_OUT_OF_SYNC = -1072870854;

		public const int MF_E_STREAMSINKS_FIXED = -1072870853;

		public const int MF_E_STREAMSINK_EXISTS = -1072870852;

		public const int MF_E_SAMPLEALLOCATOR_CANCELED = -1072870851;

		public const int MF_E_SAMPLEALLOCATOR_EMPTY = -1072870850;

		public const int MF_E_SINK_ALREADYSTOPPED = -1072870849;

		public const int MF_E_ASF_FILESINK_BITRATE_UNKNOWN = -1072870848;

		public const int MF_E_SINK_NO_STREAMS = -1072870847;

		public const int MF_S_SINK_NOT_FINALIZED = 870978;

		public const int MF_E_METADATA_TOO_LONG = -1072870845;

		public const int MF_E_SINK_NO_SAMPLES_PROCESSED = -1072870844;

		public const int MF_E_VIDEO_REN_NO_PROCAMP_HW = -1072869856;

		public const int MF_E_VIDEO_REN_NO_DEINTERLACE_HW = -1072869855;

		public const int MF_E_VIDEO_REN_COPYPROT_FAILED = -1072869854;

		public const int MF_E_VIDEO_REN_SURFACE_NOT_SHARED = -1072869853;

		public const int MF_E_VIDEO_DEVICE_LOCKED = -1072869852;

		public const int MF_E_NEW_VIDEO_DEVICE = -1072869851;

		public const int MF_E_NO_VIDEO_SAMPLE_AVAILABLE = -1072869850;

		public const int MF_E_NO_AUDIO_PLAYBACK_DEVICE = -1072869756;

		public const int MF_E_AUDIO_PLAYBACK_DEVICE_IN_USE = -1072869755;

		public const int MF_E_AUDIO_PLAYBACK_DEVICE_INVALIDATED = -1072869754;

		public const int MF_E_AUDIO_SERVICE_NOT_RUNNING = -1072869753;

		public const int MF_E_TOPO_INVALID_OPTIONAL_NODE = -1072868850;

		public const int MF_E_TOPO_CANNOT_FIND_DECRYPTOR = -1072868847;

		public const int MF_E_TOPO_CODEC_NOT_FOUND = -1072868846;

		public const int MF_E_TOPO_CANNOT_CONNECT = -1072868845;

		public const int MF_E_TOPO_UNSUPPORTED = -1072868844;

		public const int MF_E_TOPO_INVALID_TIME_ATTRIBUTES = -1072868843;

		public const int MF_E_TOPO_LOOPS_IN_TOPOLOGY = -1072868842;

		public const int MF_E_TOPO_MISSING_PRESENTATION_DESCRIPTOR = -1072868841;

		public const int MF_E_TOPO_MISSING_STREAM_DESCRIPTOR = -1072868840;

		public const int MF_E_TOPO_STREAM_DESCRIPTOR_NOT_SELECTED = -1072868839;

		public const int MF_E_TOPO_MISSING_SOURCE = -1072868838;

		public const int MF_E_TOPO_SINK_ACTIVATES_UNSUPPORTED = -1072868837;

		public const int MF_E_SEQUENCER_UNKNOWN_SEGMENT_ID = -1072864852;

		public const int MF_S_SEQUENCER_CONTEXT_CANCELED = 876973;

		public const int MF_E_NO_SOURCE_IN_CACHE = -1072864850;

		public const int MF_S_SEQUENCER_SEGMENT_AT_END_OF_STREAM = 876975;

		public const int MF_E_TRANSFORM_TYPE_NOT_SET = -1072861856;

		public const int MF_E_TRANSFORM_STREAM_CHANGE = -1072861855;

		public const int MF_E_TRANSFORM_INPUT_REMAINING = -1072861854;

		public const int MF_E_TRANSFORM_PROFILE_MISSING = -1072861853;

		public const int MF_E_TRANSFORM_PROFILE_INVALID_OR_CORRUPT = -1072861852;

		public const int MF_E_TRANSFORM_PROFILE_TRUNCATED = -1072861851;

		public const int MF_E_TRANSFORM_PROPERTY_PID_NOT_RECOGNIZED = -1072861850;

		public const int MF_E_TRANSFORM_PROPERTY_VARIANT_TYPE_WRONG = -1072861849;

		public const int MF_E_TRANSFORM_PROPERTY_NOT_WRITEABLE = -1072861848;

		public const int MF_E_TRANSFORM_PROPERTY_ARRAY_VALUE_WRONG_NUM_DIM = -1072861847;

		public const int MF_E_TRANSFORM_PROPERTY_VALUE_SIZE_WRONG = -1072861846;

		public const int MF_E_TRANSFORM_PROPERTY_VALUE_OUT_OF_RANGE = -1072861845;

		public const int MF_E_TRANSFORM_PROPERTY_VALUE_INCOMPATIBLE = -1072861844;

		public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_OUTPUT_MEDIATYPE = -1072861843;

		public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_INPUT_MEDIATYPE = -1072861842;

		public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_MEDIATYPE_COMBINATION = -1072861841;

		public const int MF_E_TRANSFORM_CONFLICTS_WITH_OTHER_CURRENTLY_ENABLED_FEATURES = -1072861840;

		public const int MF_E_TRANSFORM_NEED_MORE_INPUT = -1072861838;

		public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_SPKR_CONFIG = -1072861837;

		public const int MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING = -1072861836;

		public const int MF_S_TRANSFORM_DO_NOT_PROPAGATE_EVENT = 879989;

		public const int MF_E_UNSUPPORTED_D3D_TYPE = -1072861834;

		public const int MF_E_TRANSFORM_ASYNC_LOCKED = -1072861833;

		public const int MF_E_TRANSFORM_CANNOT_INITIALIZE_ACM_DRIVER = -1072861832;

		public const int MF_E_LICENSE_INCORRECT_RIGHTS = -1072860856;

		public const int MF_E_LICENSE_OUTOFDATE = -1072860855;

		public const int MF_E_LICENSE_REQUIRED = -1072860854;

		public const int MF_E_DRM_HARDWARE_INCONSISTENT = -1072860853;

		public const int MF_E_NO_CONTENT_PROTECTION_MANAGER = -1072860852;

		public const int MF_E_LICENSE_RESTORE_NO_RIGHTS = -1072860851;

		public const int MF_E_BACKUP_RESTRICTED_LICENSE = -1072860850;

		public const int MF_E_LICENSE_RESTORE_NEEDS_INDIVIDUALIZATION = -1072860849;

		public const int MF_S_PROTECTION_NOT_REQUIRED = 880976;

		public const int MF_E_COMPONENT_REVOKED = -1072860847;

		public const int MF_E_TRUST_DISABLED = -1072860846;

		public const int MF_E_WMDRMOTA_NO_ACTION = -1072860845;

		public const int MF_E_WMDRMOTA_ACTION_ALREADY_SET = -1072860844;

		public const int MF_E_WMDRMOTA_DRM_HEADER_NOT_AVAILABLE = -1072860843;

		public const int MF_E_WMDRMOTA_DRM_ENCRYPTION_SCHEME_NOT_SUPPORTED = -1072860842;

		public const int MF_E_WMDRMOTA_ACTION_MISMATCH = -1072860841;

		public const int MF_E_WMDRMOTA_INVALID_POLICY = -1072860840;

		public const int MF_E_POLICY_UNSUPPORTED = -1072860839;

		public const int MF_E_OPL_NOT_SUPPORTED = -1072860838;

		public const int MF_E_TOPOLOGY_VERIFICATION_FAILED = -1072860837;

		public const int MF_E_SIGNATURE_VERIFICATION_FAILED = -1072860836;

		public const int MF_E_DEBUGGING_NOT_ALLOWED = -1072860835;

		public const int MF_E_CODE_EXPIRED = -1072860834;

		public const int MF_E_GRL_VERSION_TOO_LOW = -1072860833;

		public const int MF_E_GRL_RENEWAL_NOT_FOUND = -1072860832;

		public const int MF_E_GRL_EXTENSIBLE_ENTRY_NOT_FOUND = -1072860831;

		public const int MF_E_KERNEL_UNTRUSTED = -1072860830;

		public const int MF_E_PEAUTH_UNTRUSTED = -1072860829;

		public const int MF_E_NON_PE_PROCESS = -1072860827;

		public const int MF_E_REBOOT_REQUIRED = -1072860825;

		public const int MF_S_WAIT_FOR_POLICY_SET = 881000;

		public const int MF_S_VIDEO_DISABLED_WITH_UNKNOWN_SOFTWARE_OUTPUT = 881001;

		public const int MF_E_GRL_INVALID_FORMAT = -1072860822;

		public const int MF_E_GRL_UNRECOGNIZED_FORMAT = -1072860821;

		public const int MF_E_ALL_PROCESS_RESTART_REQUIRED = -1072860820;

		public const int MF_E_PROCESS_RESTART_REQUIRED = -1072860819;

		public const int MF_E_USERMODE_UNTRUSTED = -1072860818;

		public const int MF_E_PEAUTH_SESSION_NOT_STARTED = -1072860817;

		public const int MF_E_PEAUTH_PUBLICKEY_REVOKED = -1072860815;

		public const int MF_E_GRL_ABSENT = -1072860814;

		public const int MF_S_PE_TRUSTED = 881011;

		public const int MF_E_PE_UNTRUSTED = -1072860812;

		public const int MF_E_PEAUTH_NOT_STARTED = -1072860811;

		public const int MF_E_INCOMPATIBLE_SAMPLE_PROTECTION = -1072860810;

		public const int MF_E_PE_SESSIONS_MAXED = -1072860809;

		public const int MF_E_HIGH_SECURITY_LEVEL_CONTENT_NOT_ALLOWED = -1072860808;

		public const int MF_E_TEST_SIGNED_COMPONENTS_NOT_ALLOWED = -1072860807;

		public const int MF_E_ITA_UNSUPPORTED_ACTION = -1072860806;

		public const int MF_E_ITA_ERROR_PARSING_SAP_PARAMETERS = -1072860805;

		public const int MF_E_POLICY_MGR_ACTION_OUTOFBOUNDS = -1072860804;

		public const int MF_E_BAD_OPL_STRUCTURE_FORMAT = -1072860803;

		public const int MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_PROTECTION_GUID = -1072860802;

		public const int MF_E_NO_PMP_HOST = -1072860801;

		public const int MF_E_ITA_OPL_DATA_NOT_INITIALIZED = -1072860800;

		public const int MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_OUTPUT = -1072860799;

		public const int MF_E_ITA_UNRECOGNIZED_DIGITAL_VIDEO_OUTPUT = -1072860798;

		public const int MF_E_CLOCK_INVALID_CONTINUITY_KEY = -1072849856;

		public const int MF_E_CLOCK_NO_TIME_SOURCE = -1072849855;

		public const int MF_E_CLOCK_STATE_ALREADY_SET = -1072849854;

		public const int MF_E_CLOCK_NOT_SIMPLE = -1072849853;

		public const int MF_S_CLOCK_STOPPED = 891972;

		public const int MF_E_NO_MORE_DROP_MODES = -1072848856;

		public const int MF_E_NO_MORE_QUALITY_LEVELS = -1072848855;

		public const int MF_E_DROPTIME_NOT_SUPPORTED = -1072848854;

		public const int MF_E_QUALITYKNOB_WAIT_LONGER = -1072848853;

		public const int MF_E_QM_INVALIDSTATE = -1072848852;

		public const int MF_E_TRANSCODE_NO_CONTAINERTYPE = -1072847856;

		public const int MF_E_TRANSCODE_PROFILE_NO_MATCHING_STREAMS = -1072847855;

		public const int MF_E_TRANSCODE_NO_MATCHING_ENCODER = -1072847854;

		public const int MF_E_ALLOCATOR_NOT_INITIALIZED = -1072846856;

		public const int MF_E_ALLOCATOR_NOT_COMMITED = -1072846855;

		public const int MF_E_ALLOCATOR_ALREADY_COMMITED = -1072846854;

		public const int MF_E_STREAM_ERROR = -1072846853;

		public const int MF_E_INVALID_STREAM_STATE = -1072846852;

		public const int MF_E_HW_STREAM_NOT_CONNECTED = -1072846851;
	}
	public static class MediaFoundationApi
	{
		private static bool initialized;

		public static void Startup()
		{
			if (!initialized)
			{
				int num = 2;
				OperatingSystem oSVersion = Environment.OSVersion;
				if (oSVersion.Version.Major == 6 && oSVersion.Version.Minor == 0)
				{
					num = 1;
				}
				MediaFoundationInterop.MFStartup((num << 16) | 0x70);
				initialized = true;
			}
		}

		public static IEnumerable<IMFActivate> EnumerateTransforms(Guid category)
		{
			MediaFoundationInterop.MFTEnumEx(category, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL, null, null, out var interfacesPointer, out var pcMFTActivate);
			IMFActivate[] array = new IMFActivate[pcMFTActivate];
			for (int i = 0; i < pcMFTActivate; i++)
			{
				IntPtr pUnk = Marshal.ReadIntPtr(new IntPtr(interfacesPointer.ToInt64() + i * Marshal.SizeOf(interfacesPointer)));
				array[i] = (IMFActivate)Marshal.GetObjectForIUnknown(pUnk);
			}
			IMFActivate[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				yield return array2[j];
			}
			Marshal.FreeCoTaskMem(interfacesPointer);
		}

		public static void Shutdown()
		{
			if (initialized)
			{
				MediaFoundationInterop.MFShutdown();
				initialized = false;
			}
		}

		public static IMFMediaType CreateMediaType()
		{
			MediaFoundationInterop.MFCreateMediaType(out var ppMFType);
			return ppMFType;
		}

		public static IMFMediaType CreateMediaTypeFromWaveFormat(WaveFormat waveFormat)
		{
			IMFMediaType iMFMediaType = CreateMediaType();
			try
			{
				MediaFoundationInterop.MFInitMediaTypeFromWaveFormatEx(iMFMediaType, waveFormat, Marshal.SizeOf(waveFormat));
				return iMFMediaType;
			}
			catch (Exception)
			{
				Marshal.ReleaseComObject(iMFMediaType);
				throw;
			}
		}

		public static IMFMediaBuffer CreateMemoryBuffer(int bufferSize)
		{
			MediaFoundationInterop.MFCreateMemoryBuffer(bufferSize, out var ppBuffer);
			return ppBuffer;
		}

		public static IMFSample CreateSample()
		{
			MediaFoundationInterop.MFCreateSample(out var ppIMFSample);
			return ppIMFSample;
		}

		public static IMFAttributes CreateAttributes(int initialSize)
		{
			MediaFoundationInterop.MFCreateAttributes(out var ppMFAttributes, initialSize);
			return ppMFAttributes;
		}

		public static IMFByteStream CreateByteStream(object stream)
		{
			if (stream is IStream)
			{
				MediaFoundationInterop.MFCreateMFByteStreamOnStream(stream as IStream, out var ppByteStream);
				return ppByteStream;
			}
			throw new ArgumentException("Stream must be IStream in desktop apps");
		}

		public static IMFSourceReader CreateSourceReaderFromByteStream(IMFByteStream byteStream)
		{
			MediaFoundationInterop.MFCreateSourceReaderFromByteStream(byteStream, null, out var ppSourceReader);
			return ppSourceReader;
		}
	}
	public static class MediaFoundationInterop
	{
		public const int MF_SOURCE_READER_ALL_STREAMS = -2;

		public const int MF_SOURCE_READER_FIRST_AUDIO_STREAM = -3;

		public const int MF_SOURCE_READER_FIRST_VIDEO_STREAM = -4;

		public const int MF_SOURCE_READER_MEDIASOURCE = -1;

		public const int MF_SDK_VERSION = 2;

		public const int MF_API_VERSION = 112;

		public const int MF_VERSION = 131184;

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFStartup(int version, int dwFlags = 0);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFShutdown();

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void MFCreateMediaType(out IMFMediaType ppMFType);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void MFInitMediaTypeFromWaveFormatEx([In] IMFMediaType pMFType, [In] WaveFormat pWaveFormat, [In] int cbBufSize);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void MFCreateWaveFormatExFromMFMediaType(IMFMediaType pMFType, ref IntPtr ppWF, ref int pcbSize, int flags = 0);

		[DllImport("mfreadwrite.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFCreateSourceReaderFromURL([In][MarshalAs(UnmanagedType.LPWStr)] string pwszURL, [In] IMFAttributes pAttributes, [MarshalAs(UnmanagedType.Interface)] out IMFSourceReader ppSourceReader);

		[DllImport("mfreadwrite.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFCreateSourceReaderFromByteStream([In] IMFByteStream pByteStream, [In] IMFAttributes pAttributes, [MarshalAs(UnmanagedType.Interface)] out IMFSourceReader ppSourceReader);

		[DllImport("mfreadwrite.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFCreateSinkWriterFromURL([In][MarshalAs(UnmanagedType.LPWStr)] string pwszOutputURL, [In] IMFByteStream pByteStream, [In] IMFAttributes pAttributes, out IMFSinkWriter ppSinkWriter);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFCreateMFByteStreamOnStream([In] IStream punkStream, out IMFByteStream ppByteStream);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFTEnumEx([In] Guid guidCategory, [In] _MFT_ENUM_FLAG flags, [In] MFT_REGISTER_TYPE_INFO pInputType, [In] MFT_REGISTER_TYPE_INFO pOutputType, out IntPtr pppMFTActivate, out int pcMFTActivate);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void MFCreateSample(out IMFSample ppIMFSample);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void MFCreateMemoryBuffer(int cbMaxLength, out IMFMediaBuffer ppBuffer);

		[DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void MFCreateAttributes([MarshalAs(UnmanagedType.Interface)] out IMFAttributes ppMFAttributes, [In] int cInitialSize);

		[DllImport("mf.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void MFTranscodeGetAudioOutputAvailableTypes([In][MarshalAs(UnmanagedType.LPStruct)] Guid guidSubType, [In] _MFT_ENUM_FLAG dwMFTFlags, [In] IMFAttributes pCodecConfig, [MarshalAs(UnmanagedType.Interface)] out IMFCollection ppAvailableTypes);
	}
	public abstract class MediaFoundationTransform : IWaveProvider, IDisposable
	{
		protected readonly IWaveProvider sourceProvider;

		protected readonly WaveFormat outputWaveFormat;

		private readonly byte[] sourceBuffer;

		private byte[] outputBuffer;

		private int outputBufferOffset;

		private int outputBufferCount;

		private IMFTransform transform;

		private bool disposed;

		private long inputPosition;

		private long outputPosition;

		private bool initializedForStreaming;

		public WaveFormat WaveFormat => outputWaveFormat;

		public MediaFoundationTransform(IWaveProvider sourceProvider, WaveFormat outputFormat)
		{
			outputWaveFormat = outputFormat;
			this.sourceProvider = sourceProvider;
			sourceBuffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond];
			outputBuffer = new byte[outputWaveFormat.AverageBytesPerSecond + outputWaveFormat.BlockAlign];
		}

		private void InitializeTransformForStreaming()
		{
			transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_FLUSH, IntPtr.Zero);
			transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_BEGIN_STREAMING, IntPtr.Zero);
			transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_START_OF_STREAM, IntPtr.Zero);
			initializedForStreaming = true;
		}

		protected abstract IMFTransform CreateTransform();

		protected virtual void Dispose(bool disposing)
		{
			if (transform != null)
			{
				Marshal.ReleaseComObject(transform);
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
			}
		}

		~MediaFoundationTransform()
		{
			Dispose(disposing: false);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			if (transform == null)
			{
				transform = CreateTransform();
				InitializeTransformForStreaming();
			}
			int i = 0;
			if (outputBufferCount > 0)
			{
				i += ReadFromOutputBuffer(buffer, offset, count - i);
			}
			for (; i < count; i += ReadFromOutputBuffer(buffer, offset + i, count - i))
			{
				IMFSample iMFSample = ReadFromSource();
				if (iMFSample == null)
				{
					EndStreamAndDrain();
					i += ReadFromOutputBuffer(buffer, offset + i, count - i);
					break;
				}
				if (!initializedForStreaming)
				{
					InitializeTransformForStreaming();
				}
				transform.ProcessInput(0, iMFSample, 0);
				Marshal.ReleaseComObject(iMFSample);
				ReadFromTransform();
			}
			return i;
		}

		private void EndStreamAndDrain()
		{
			transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_END_OF_STREAM, IntPtr.Zero);
			transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_DRAIN, IntPtr.Zero);
			int num;
			do
			{
				num = ReadFromTransform();
			}
			while (num > 0);
			outputBufferCount = 0;
			outputBufferOffset = 0;
			inputPosition = 0L;
			outputPosition = 0L;
			transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_END_STREAMING, IntPtr.Zero);
			initializedForStreaming = false;
		}

		private int ReadFromTransform()
		{
			MFT_OUTPUT_DATA_BUFFER[] array = new MFT_OUTPUT_DATA_BUFFER[1];
			IMFSample iMFSample = MediaFoundationApi.CreateSample();
			IMFMediaBuffer iMFMediaBuffer = MediaFoundationApi.CreateMemoryBuffer(outputBuffer.Length);
			iMFSample.AddBuffer(iMFMediaBuffer);
			iMFSample.SetSampleTime(outputPosition);
			array[0].pSample = iMFSample;
			_MFT_PROCESS_OUTPUT_STATUS pdwStatus;
			int num = transform.ProcessOutput(_MFT_PROCESS_OUTPUT_FLAGS.None, 1, array, out pdwStatus);
			switch (num)
			{
			case -1072861838:
				Marshal.ReleaseComObject(iMFMediaBuffer);
				Marshal.ReleaseComObject(iMFSample);
				return 0;
			default:
				Marshal.ThrowExceptionForHR(num);
				break;
			case 0:
				break;
			}
			array[0].pSample.ConvertToContiguousBuffer(out var ppBuffer);
			ppBuffer.Lock(out var ppbBuffer, out var _, out var pcbCurrentLength);
			outputBuffer = BufferHelpers.Ensure(outputBuffer, pcbCurrentLength);
			Marshal.Copy(ppbBuffer, outputBuffer, 0, pcbCurrentLength);
			outputBufferOffset = 0;
			outputBufferCount = pcbCurrentLength;
			ppBuffer.Unlock();
			outputPosition += BytesToNsPosition(outputBufferCount, WaveFormat);
			Marshal.ReleaseComObject(iMFMediaBuffer);
			iMFSample.RemoveAllBuffers();
			Marshal.ReleaseComObject(iMFSample);
			Marshal.ReleaseComObject(ppBuffer);
			return pcbCurrentLength;
		}

		private static long BytesToNsPosition(int bytes, WaveFormat waveFormat)
		{
			return 10000000L * (long)bytes / waveFormat.AverageBytesPerSecond;
		}

		private IMFSample ReadFromSource()
		{
			int num = sourceProvider.Read(sourceBuffer, 0, sourceBuffer.Length);
			if (num == 0)
			{
				return null;
			}
			IMFMediaBuffer iMFMediaBuffer = MediaFoundationApi.CreateMemoryBuffer(num);
			iMFMediaBuffer.Lock(out var ppbBuffer, out var _, out var _);
			Marshal.Copy(sourceBuffer, 0, ppbBuffer, num);
			iMFMediaBuffer.Unlock();
			iMFMediaBuffer.SetCurrentLength(num);
			IMFSample iMFSample = MediaFoundationApi.CreateSample();
			iMFSample.AddBuffer(iMFMediaBuffer);
			iMFSample.SetSampleTime(inputPosition);
			long num2 = BytesToNsPosition(num, sourceProvider.WaveFormat);
			iMFSample.SetSampleDuration(num2);
			inputPosition += num2;
			Marshal.ReleaseComObject(iMFMediaBuffer);
			return iMFSample;
		}

		private int ReadFromOutputBuffer(byte[] buffer, int offset, int needed)
		{
			int num = Math.Min(needed, outputBufferCount);
			Array.Copy(outputBuffer, outputBufferOffset, buffer, offset, num);
			outputBufferOffset += num;
			outputBufferCount -= num;
			if (outputBufferCount == 0)
			{
				outputBufferOffset = 0;
			}
			return num;
		}

		public void Reposition()
		{
			if (initializedForStreaming)
			{
				EndStreamAndDrain();
				InitializeTransformForStreaming();
			}
		}
	}
	public static class MediaFoundationTransformCategories
	{
		[FieldDescription("Video Decoder")]
		public static readonly Guid VideoDecoder = new Guid("{d6c02d4b-6833-45b4-971a-05a4b04bab91}");

		[FieldDescription("Video Encoder")]
		public static readonly Guid VideoEncoder = new Guid("{f79eac7d-e545-4387-bdee-d647d7bde42a}");

		[FieldDescription("Video Effect")]
		public static readonly Guid VideoEffect = new Guid("{12e17c21-532c-4a6e-8a1c-40825a736397}");

		[FieldDescription("Multiplexer")]
		public static readonly Guid Multiplexer = new Guid("{059c561e-05ae-4b61-b69d-55b61ee54a7b}");

		[FieldDescription("Demultiplexer")]
		public static readonly Guid Demultiplexer = new Guid("{a8700a7a-939b-44c5-99d7-76226b23b3f1}");

		[FieldDescription("Audio Decoder")]
		public static readonly Guid AudioDecoder = new Guid("{9ea73fb4-ef7a-4559-8d5d-719d8f0426c7}");

		[FieldDescription("Audio Encoder")]
		public static readonly Guid AudioEncoder = new Guid("{91c64bd0-f91e-4d8c-9276-db248279d975}");

		[FieldDescription("Audio Effect")]
		public static readonly Guid AudioEffect = new Guid("{11064c48-3648-4ed0-932e-05ce8ac811b7}");

		[FieldDescription("Video Processor")]
		public static readonly Guid VideoProcessor = new Guid("{302EA3FC-AA5F-47f9-9F7A-C2188BB16302}");

		[FieldDescription("Other")]
		public static readonly Guid Other = new Guid("{90175d57-b7ea-4901-aeb3-933a8747756f}");
	}
	public class MediaType
	{
		private readonly IMFMediaType mediaType;

		public int SampleRate
		{
			get
			{
				return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND);
			}
			set
			{
				mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND, value);
			}
		}

		public int ChannelCount
		{
			get
			{
				return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS);
			}
			set
			{
				mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS, value);
			}
		}

		public int BitsPerSample
		{
			get
			{
				return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE);
			}
			set
			{
				mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE, value);
			}
		}

		public int AverageBytesPerSecond => GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_AVG_BYTES_PER_SECOND);

		public Guid SubType
		{
			get
			{
				return GetGuid(MediaFoundationAttributes.MF_MT_SUBTYPE);
			}
			set
			{
				mediaType.SetGUID(MediaFoundationAttributes.MF_MT_SUBTYPE, value);
			}
		}

		public Guid MajorType
		{
			get
			{
				return GetGuid(MediaFoundationAttributes.MF_MT_MAJOR_TYPE);
			}
			set
			{
				mediaType.SetGUID(MediaFoundationAttributes.MF_MT_MAJOR_TYPE, value);
			}
		}

		public IMFMediaType MediaFoundationObject => mediaType;

		public MediaType(IMFMediaType mediaType)
		{
			this.mediaType = mediaType;
		}

		public MediaType()
		{
			mediaType = MediaFoundationApi.CreateMediaType();
		}

		public MediaType(WaveFormat waveFormat)
		{
			mediaType = MediaFoundationApi.CreateMediaTypeFromWaveFormat(waveFormat);
		}

		private int GetUInt32(Guid key)
		{
			mediaType.GetUINT32(key, out var punValue);
			return punValue;
		}

		private Guid GetGuid(Guid key)
		{
			mediaType.GetGUID(key, out var pguidValue);
			return pguidValue;
		}

		public int TryGetUInt32(Guid key, int defaultValue = -1)
		{
			int punValue = defaultValue;
			try
			{
				mediaType.GetUINT32(key, out punValue);
				return punValue;
			}
			catch (COMException exception)
			{
				if (exception.GetHResult() != -1072875802)
				{
					if (exception.GetHResult() == -1072875843)
					{
						throw new ArgumentException("Not a UINT32 parameter");
					}
					throw;
				}
				return punValue;
			}
		}
	}
	public static class MediaTypes
	{
		public static readonly Guid MFMediaType_Default = new Guid("81A412E6-8103-4B06-857F-1862781024AC");

		[FieldDescription("Audio")]
		public static readonly Guid MFMediaType_Audio = new Guid("73647561-0000-0010-8000-00aa00389b71");

		[FieldDescription("Video")]
		public static readonly Guid MFMediaType_Video = new Guid("73646976-0000-0010-8000-00aa00389b71");

		[FieldDescription("Protected Media")]
		public static readonly Guid MFMediaType_Protected = new Guid("7b4b6fe6-9d04-4494-be14-7e0bd076c8e4");

		[FieldDescription("SAMI captions")]
		public static readonly Guid MFMediaType_SAMI = new Guid("e69669a0-3dcd-40cb-9e2e-3708387c0616");

		[FieldDescription("Script stream")]
		public static readonly Guid MFMediaType_Script = new Guid("72178c22-e45b-11d5-bc2a-00b0d0f3f4ab");

		[FieldDescription("Still image stream")]
		public static readonly Guid MFMediaType_Image = new Guid("72178c23-e45b-11d5-bc2a-00b0d0f3f4ab");

		[FieldDescription("HTML stream")]
		public static readonly Guid MFMediaType_HTML = new Guid("72178c24-e45b-11d5-bc2a-00b0d0f3f4ab");

		[FieldDescription("Binary stream")]
		public static readonly Guid MFMediaType_Binary = new Guid("72178c25-e45b-11d5-bc2a-00b0d0f3f4ab");

		[FieldDescription("File transfer")]
		public static readonly Guid MFMediaType_FileTransfer = new Guid("72178c26-e45b-11d5-bc2a-00b0d0f3f4ab");
	}
	public struct MFT_INPUT_STREAM_INFO
	{
		public long hnsMaxLatency;

		public _MFT_INPUT_STREAM_INFO_FLAGS dwFlags;

		public int cbSize;

		public int cbMaxLookahead;

		public int cbAlignment;
	}
	public enum MFT_MESSAGE_TYPE
	{
		MFT_MESSAGE_COMMAND_FLUSH = 0,
		MFT_MESSAGE_COMMAND_DRAIN = 1,
		MFT_MESSAGE_SET_D3D_MANAGER = 2,
		MFT_MESSAGE_DROP_SAMPLES = 3,
		MFT_MESSAGE_COMMAND_TICK = 4,
		MFT_MESSAGE_NOTIFY_BEGIN_STREAMING = 268435456,
		MFT_MESSAGE_NOTIFY_END_STREAMING = 268435457,
		MFT_MESSAGE_NOTIFY_END_OF_STREAM = 268435458,
		MFT_MESSAGE_NOTIFY_START_OF_STREAM = 268435459,
		MFT_MESSAGE_COMMAND_MARKER = 536870912
	}
	public struct MFT_OUTPUT_DATA_BUFFER
	{
		public int dwStreamID;

		public IMFSample pSample;

		public _MFT_OUTPUT_DATA_BUFFER_FLAGS dwStatus;

		public IMFCollection pEvents;
	}
	public struct MFT_OUTPUT_STREAM_INFO
	{
		public _MFT_OUTPUT_STREAM_INFO_FLAGS dwFlags;

		public int cbSize;

		public int cbAlignment;
	}
	[StructLayout(LayoutKind.Sequential)]
	public class MFT_REGISTER_TYPE_INFO
	{
		public Guid guidMajorType;

		public Guid guidSubtype;
	}
	[StructLayout(LayoutKind.Sequential)]
	public class MF_SINK_WRITER_STATISTICS
	{
		public int cb;

		public long llLastTimestampReceived;

		public long llLastTimestampEncoded;

		public long llLastTimestampProcessed;

		public long llLastStreamTickReceived;

		public long llLastSinkSampleRequest;

		public long qwNumSamplesReceived;

		public long qwNumSamplesEncoded;

		public long qwNumSamplesProcessed;

		public long qwNumStreamTicksReceived;

		public int dwByteCountQueued;

		public long qwByteCountProcessed;

		public int dwNumOutstandingSinkSampleRequests;

		public int dwAverageSampleRateReceived;

		public int dwAverageSampleRateEncoded;

		public int dwAverageSampleRateProcessed;
	}
	[Flags]
	public enum _MFT_ENUM_FLAG
	{
		None = 0,
		MFT_ENUM_FLAG_SYNCMFT = 1,
		MFT_ENUM_FLAG_ASYNCMFT = 2,
		MFT_ENUM_FLAG_HARDWARE = 4,
		MFT_ENUM_FLAG_FIELDOFUSE = 8,
		MFT_ENUM_FLAG_LOCALMFT = 0x10,
		MFT_ENUM_FLAG_TRANSCODE_ONLY = 0x20,
		MFT_ENUM_FLAG_SORTANDFILTER = 0x40,
		MFT_ENUM_FLAG_ALL = 0x3F
	}
	[Flags]
	public enum _MFT_INPUT_STATUS_FLAGS
	{
		None = 0,
		MFT_INPUT_STATUS_ACCEPT_DATA = 1
	}
	[Flags]
	public enum _MFT_INPUT_STREAM_INFO_FLAGS
	{
		None = 0,
		MFT_INPUT_STREAM_WHOLE_SAMPLES = 1,
		MFT_INPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER = 2,
		MFT_INPUT_STREAM_FIXED_SAMPLE_SIZE = 4,
		MFT_INPUT_STREAM_HOLDS_BUFFERS = 8,
		MFT_INPUT_STREAM_DOES_NOT_ADDREF = 0x100,
		MFT_INPUT_STREAM_REMOVABLE = 0x200,
		MFT_INPUT_STREAM_OPTIONAL = 0x400,
		MFT_INPUT_STREAM_PROCESSES_IN_PLACE = 0x800
	}
	[Flags]
	public enum _MFT_OUTPUT_DATA_BUFFER_FLAGS
	{
		None = 0,
		MFT_OUTPUT_DATA_BUFFER_INCOMPLETE = 0x1000000,
		MFT_OUTPUT_DATA_BUFFER_FORMAT_CHANGE = 0x100,
		MFT_OUTPUT_DATA_BUFFER_STREAM_END = 0x200,
		MFT_OUTPUT_DATA_BUFFER_NO_SAMPLE = 0x300
	}
	[Flags]
	public enum _MFT_OUTPUT_STATUS_FLAGS
	{
		None = 0,
		MFT_OUTPUT_STATUS_SAMPLE_READY = 1
	}
	[Flags]
	public enum _MFT_OUTPUT_STREAM_INFO_FLAGS
	{
		None = 0,
		MFT_OUTPUT_STREAM_WHOLE_SAMPLES = 1,
		MFT_OUTPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER = 2,
		MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE = 4,
		MFT_OUTPUT_STREAM_DISCARDABLE = 8,
		MFT_OUTPUT_STREAM_OPTIONAL = 0x10,
		MFT_OUTPUT_STREAM_PROVIDES_SAMPLES = 0x100,
		MFT_OUTPUT_STREAM_CAN_PROVIDE_SAMPLES = 0x200,
		MFT_OUTPUT_STREAM_LAZY_READ = 0x400,
		MFT_OUTPUT_STREAM_REMOVABLE = 0x800
	}
	[Flags]
	public enum _MFT_PROCESS_OUTPUT_FLAGS
	{
		None = 0,
		MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER = 1,
		MFT_PROCESS_OUTPUT_REGENERATE_LAST_OUTPUT = 2
	}
	[Flags]
	public enum _MFT_PROCESS_OUTPUT_STATUS
	{
		None = 0,
		MFT_PROCESS_OUTPUT_STATUS_NEW_STREAMS = 0x100
	}
	[Flags]
	public enum _MFT_SET_TYPE_FLAGS
	{
		None = 0,
		MFT_SET_TYPE_TEST_ONLY = 1
	}
}
namespace NAudio.Gui
{
	public class Fader : Control
	{
		private int minimum;

		private int maximum;

		private float percent;

		private Orientation orientation;

		private Container components;

		private readonly int SliderHeight = 30;

		private readonly int SliderWidth = 15;

		private Rectangle sliderRectangle;

		private bool dragging;

		private int dragY;

		public int Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				minimum = value;
			}
		}

		public int Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				maximum = value;
			}
		}

		public int Value
		{
			get
			{
				return (int)(percent * (float)(maximum - minimum)) + minimum;
			}
			set
			{
				percent = (float)(value - minimum) / (float)(maximum - minimum);
			}
		}

		public Orientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
			}
		}

		public Fader()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void DrawSlider(Graphics g)
		{
			Brush brush = new SolidBrush(Color.White);
			Pen pen = new Pen(Color.Black);
			sliderRectangle.X = (base.Width - SliderWidth) / 2;
			sliderRectangle.Width = SliderWidth;
			sliderRectangle.Y = (int)((float)(base.Height - SliderHeight) * percent);
			sliderRectangle.Height = SliderHeight;
			g.FillRectangle(brush, sliderRectangle);
			g.DrawLine(pen, sliderRectangle.Left, sliderRectangle.Top + sliderRectangle.Height / 2, sliderRectangle.Right, sliderRectangle.Top + sliderRectangle.Height / 2);
			brush.Dispose();
			pen.Dispose();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			if (Orientation == Orientation.Vertical)
			{
				Brush brush = new SolidBrush(Color.Black);
				graphics.FillRectangle(brush, base.Width / 2, SliderHeight / 2, 2, base.Height - SliderHeight);
				brush.Dispose();
				DrawSlider(graphics);
			}
			base.OnPaint(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (sliderRectangle.Contains(e.X, e.Y))
			{
				dragging = true;
				dragY = e.Y - sliderRectangle.Y;
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (dragging)
			{
				int num = e.Y - dragY;
				if (num < 0)
				{
					percent = 0f;
				}
				else if (num > base.Height - SliderHeight)
				{
					percent = 1f;
				}
				else
				{
					percent = (float)num / (float)(base.Height - SliderHeight);
				}
				Invalidate();
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			dragging = false;
			base.OnMouseUp(e);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
	}
	public class PanSlider : UserControl
	{
		private Container components;

		private float pan;

		public float Pan
		{
			get
			{
				return pan;
			}
			set
			{
				if (value < -1f)
				{
					value = -1f;
				}
				if (value > 1f)
				{
					value = 1f;
				}
				if (value != pan)
				{
					pan = value;
					if (this.PanChanged != null)
					{
						this.PanChanged(this, EventArgs.Empty);
					}
					Invalidate();
				}
			}
		}

		public event EventHandler PanChanged;

		public PanSlider()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.Name = "PanSlider";
			base.Size = new System.Drawing.Size(104, 16);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Alignment = StringAlignment.Center;
			string s;
			if ((double)pan == 0.0)
			{
				pe.Graphics.FillRectangle(Brushes.Orange, base.Width / 2 - 1, 1, 3, base.Height - 2);
				s = "C";
			}
			else if (pan > 0f)
			{
				pe.Graphics.FillRectangle(Brushes.Orange, base.Width / 2, 1, (int)((float)(base.Width / 2) * pan), base.Height - 2);
				s = $"{pan * 100f:F0}%R";
			}
			else
			{
				pe.Graphics.FillRectangle(Brushes.Orange, (int)((float)(base.Width / 2) * (pan + 1f)), 1, (int)((float)(base.Width / 2) * (0f - pan)), base.Height - 2);
				s = $"{pan * -100f:F0}%L";
			}
			pe.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
			pe.Graphics.DrawString(s, Font, Brushes.Black, base.ClientRectangle, stringFormat);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				SetPanFromMouse(e.X);
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			SetPanFromMouse(e.X);
			base.OnMouseDown(e);
		}

		private void SetPanFromMouse(int x)
		{
			Pan = (float)x / (float)base.Width * 2f - 1f;
		}
	}
	public class Pot : UserControl
	{
		private double minimum;

		private double maximum = 1.0;

		private double value = 0.5;

		private int beginDragY;

		private double beginDragValue;

		private bool dragging;

		private IContainer components;

		public double Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				if (value >= maximum)
				{
					throw new ArgumentOutOfRangeException("Minimum must be less than maximum");
				}
				minimum = value;
				if (Value < minimum)
				{
					Value = minimum;
				}
			}
		}

		public double Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				if (value <= minimum)
				{
					throw new ArgumentOutOfRangeException("Maximum must be greater than minimum");
				}
				maximum = value;
				if (Value > maximum)
				{
					Value = maximum;
				}
			}
		}

		public double Value
		{
			get
			{
				return value;
			}
			set
			{
				SetValue(value, raiseEvents: false);
			}
		}

		public event EventHandler ValueChanged;

		public Pot()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
			InitializeComponent();
		}

		private void SetValue(double newValue, bool raiseEvents)
		{
			if (value != newValue)
			{
				value = newValue;
				if (raiseEvents && this.ValueChanged != null)
				{
					this.ValueChanged(this, EventArgs.Empty);
				}
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			int num = Math.Min(base.Width - 4, base.Height - 4);
			Pen pen = new Pen(ForeColor, 3f);
			pen.LineJoin = LineJoin.Round;
			GraphicsState gstate = e.Graphics.Save();
			e.Graphics.TranslateTransform(base.Width / 2, base.Height / 2);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.DrawArc(pen, new Rectangle(num / -2, num / -2, num, num), 135f, 270f);
			double num2 = (value - minimum) / (maximum - minimum);
			double num3 = 135.0 + num2 * 270.0;
			double num4 = (double)num / 2.0 * Math.Cos(Math.PI * num3 / 180.0);
			double num5 = (double)num / 2.0 * Math.Sin(Math.PI * num3 / 180.0);
			e.Graphics.DrawLine(pen, 0f, 0f, (float)num4, (float)num5);
			e.Graphics.Restore(gstate);
			base.OnPaint(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			dragging = true;
			beginDragY = e.Y;
			beginDragValue = value;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			dragging = false;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (dragging)
			{
				int num = beginDragY - e.Y;
				double num2 = (maximum - minimum) * ((double)num / 150.0);
				double num3 = beginDragValue + num2;
				if (num3 < minimum)
				{
					num3 = minimum;
				}
				if (num3 > maximum)
				{
					num3 = maximum;
				}
				SetValue(num3, raiseEvents: true);
			}
			base.OnMouseMove(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "Pot";
			base.Size = new System.Drawing.Size(32, 32);
			base.ResumeLayout(false);
		}
	}
	public class VolumeMeter : Control
	{
		private Brush foregroundBrush;

		private float amplitude;

		private IContainer components;

		[DefaultValue(-3.0)]
		public float Amplitude
		{
			get
			{
				return amplitude;
			}
			set
			{
				amplitude = value;
				Invalidate();
			}
		}

		[DefaultValue(-60.0)]
		public float MinDb { get; set; }

		[DefaultValue(18.0)]
		public float MaxDb { get; set; }

		[DefaultValue(Orientation.Vertical)]
		public Orientation Orientation { get; set; }

		public VolumeMeter()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
			MinDb = -60f;
			MaxDb = 18f;
			Amplitude = 0f;
			Orientation = Orientation.Vertical;
			InitializeComponent();
			OnForeColorChanged(EventArgs.Empty);
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			foregroundBrush = new SolidBrush(ForeColor);
			base.OnForeColorChanged(e);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			pe.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
			double num = 20.0 * Math.Log10(Amplitude);
			if (num < (double)MinDb)
			{
				num = MinDb;
			}
			if (num > (double)MaxDb)
			{
				num = MaxDb;
			}
			double num2 = (num - (double)MinDb) / (double)(MaxDb - MinDb);
			int num3 = base.Width - 2;
			int num4 = base.Height - 2;
			if (Orientation == Orientation.Horizontal)
			{
				num3 = (int)((double)num3 * num2);
				pe.Graphics.FillRectangle(foregroundBrush, 1, 1, num3, num4);
			}
			else
			{
				num4 = (int)((double)num4 * num2);
				pe.Graphics.FillRectangle(foregroundBrush, 1, base.Height - 1 - num4, num3, num4);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
	}
	public class VolumeSlider : UserControl
	{
		private Container components;

		private float volume = 1f;

		private float MinDb = -48f;

		[DefaultValue(1f)]
		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				if (value < 0f)
				{
					value = 0f;
				}
				if (value > 1f)
				{
					value = 1f;
				}
				if (volume != value)
				{
					volume = value;
					if (this.VolumeChanged != null)
					{
						this.VolumeChanged(this, EventArgs.Empty);
					}
					Invalidate();
				}
			}
		}

		public event EventHandler VolumeChanged;

		public VolumeSlider()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.Name = "VolumeSlider";
			base.Size = new System.Drawing.Size(96, 16);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Alignment = StringAlignment.Center;
			pe.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
			float num = 20f * (float)Math.Log10(Volume);
			float num2 = 1f - num / MinDb;
			pe.Graphics.FillRectangle(Brushes.LightGreen, 1, 1, (int)((float)(base.Width - 2) * num2), base.Height - 2);
			string s = $"{num:F2} dB";
			pe.Graphics.DrawString(s, Font, Brushes.Black, base.ClientRectangle, stringFormat);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				SetVolumeFromMouse(e.X);
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			SetVolumeFromMouse(e.X);
			base.OnMouseDown(e);
		}

		private void SetVolumeFromMouse(int x)
		{
			float num = (1f - (float)x / (float)base.Width) * MinDb;
			if (x <= 0)
			{
				Volume = 0f;
			}
			else
			{
				Volume = (float)Math.Pow(10.0, num / 20f);
			}
		}
	}
	public class WaveformPainter : Control
	{
		private Pen foregroundPen;

		private List<float> samples = new List<float>(1000);

		private int maxSamples;

		private int insertPos;

		private IContainer components;

		public WaveformPainter()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
			InitializeComponent();
			OnForeColorChanged(EventArgs.Empty);
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs e)
		{
			maxSamples = base.Width;
			base.OnResize(e);
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			foregroundPen = new Pen(ForeColor);
			base.OnForeColorChanged(e);
		}

		public void AddMax(float maxSample)
		{
			if (maxSamples != 0)
			{
				if (samples.Count <= maxSamples)
				{
					samples.Add(maxSample);
				}
				else if (insertPos < maxSamples)
				{
					samples[insertPos] = maxSample;
				}
				insertPos++;
				insertPos %= maxSamples;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			for (int i = 0; i < base.Width; i++)
			{
				float num = (float)base.Height * GetSample(i - base.Width + insertPos);
				float num2 = ((float)base.Height - num) / 2f;
				pe.Graphics.DrawLine(foregroundPen, i, num2, i, num2 + num);
			}
		}

		private float GetSample(int index)
		{
			if (index < 0)
			{
				index += maxSamples;
			}
			if ((index >= 0) & (index < samples.Count))
			{
				return samples[index];
			}
			return 0f;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
	}
	public class WaveViewer : UserControl
	{
		private Container components;

		private WaveStream waveStream;

		private int samplesPerPixel = 128;

		private long startPosition;

		private int bytesPerSample;

		public WaveStream WaveStream
		{
			get
			{
				return waveStream;
			}
			set
			{
				waveStream = value;
				if (waveStream != null)
				{
					bytesPerSample = waveStream.WaveFormat.BitsPerSample / 8 * waveStream.WaveFormat.Channels;
				}
				Invalidate();
			}
		}

		public int SamplesPerPixel
		{
			get
			{
				return samplesPerPixel;
			}
			set
			{
				samplesPerPixel = value;
				Invalidate();
			}
		}

		public long StartPosition
		{
			get
			{
				return startPosition;
			}
			set
			{
				startPosition = value;
			}
		}

		public WaveViewer()
		{
			InitializeComponent();
			DoubleBuffered = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (waveStream != null)
			{
				waveStream.Position = 0L;
				byte[] array = new byte[samplesPerPixel * bytesPerSample];
				waveStream.Position = startPosition + e.ClipRectangle.Left * bytesPerSample * samplesPerPixel;
				for (float num = e.ClipRectangle.X; num < (float)e.ClipRectangle.Right; num += 1f)
				{
					short num2 = 0;
					short num3 = 0;
					int num4 = waveStream.Read(array, 0, samplesPerPixel * bytesPerSample);
					if (num4 == 0)
					{
						break;
					}
					for (int i = 0; i < num4; i += 2)
					{
						short num5 = BitConverter.ToInt16(array, i);
						if (num5 < num2)
						{
							num2 = num5;
						}
						if (num5 > num3)
						{
							num3 = num5;
						}
					}
					float num6 = ((float)num2 - -32768f) / 65535f;
					float num7 = ((float)num3 - -32768f) / 65535f;
					e.Graphics.DrawLine(Pens.Black, num, (float)base.Height * num6, num, (float)base.Height * num7);
				}
			}
			base.OnPaint(e);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
	}
}
namespace NAudio.SoundFont
{
	public class Generator
	{
		public GeneratorEnum GeneratorType { get; set; }

		public ushort UInt16Amount { get; set; }

		public short Int16Amount
		{
			get
			{
				return (short)UInt16Amount;
			}
			set
			{
				UInt16Amount = (ushort)value;
			}
		}

		public byte LowByteAmount
		{
			get
			{
				return (byte)(UInt16Amount & 0xFFu);
			}
			set
			{
				UInt16Amount &= 65280;
				UInt16Amount += value;
			}
		}

		public byte HighByteAmount
		{
			get
			{
				return (byte)((UInt16Amount & 0xFF00) >> 8);
			}
			set
			{
				UInt16Amount &= 255;
				UInt16Amount += (ushort)(value << 8);
			}
		}

		public Instrument Instrument { get; set; }

		public SampleHeader SampleHeader { get; set; }

		public override string ToString()
		{
			if (GeneratorType == GeneratorEnum.Instrument)
			{
				return "Generator Instrument " + Instrument.Name;
			}
			if (GeneratorType == GeneratorEnum.SampleID)
			{
				return $"Generator SampleID {SampleHeader}";
			}
			return $"Generator {GeneratorType} {UInt16Amount}";
		}
	}
	internal class GeneratorBuilder : StructureBuilder<Generator>
	{
		public override int Length => 4;

		public Generator[] Generators => data.ToArray();

		public override Generator Read(BinaryReader br)
		{
			Generator generator = new Generator();
			generator.GeneratorType = (GeneratorEnum)br.ReadUInt16();
			generator.UInt16Amount = br.ReadUInt16();
			data.Add(generator);
			return generator;
		}

		public override void Write(BinaryWriter bw, Generator o)
		{
		}

		public void Load(Instrument[] instruments)
		{
			Generator[] generators = Generators;
			foreach (Generator generator in generators)
			{
				if (generator.GeneratorType == GeneratorEnum.Instrument)
				{
					generator.Instrument = instruments[generator.UInt16Amount];
				}
			}
		}

		public void Load(SampleHeader[] sampleHeaders)
		{
			Generator[] generators = Generators;
			foreach (Generator generator in generators)
			{
				if (generator.GeneratorType == GeneratorEnum.SampleID)
				{
					generator.SampleHeader = sampleHeaders[generator.UInt16Amount];
				}
			}
		}
	}
	public enum GeneratorEnum
	{
		StartAddressOffset,
		EndAddressOffset,
		StartLoopAddressOffset,
		EndLoopAddressOffset,
		StartAddressCoarseOffset,
		ModulationLFOToPitch,
		VibratoLFOToPitch,
		ModulationEnvelopeToPitch,
		InitialFilterCutoffFrequency,
		InitialFilterQ,
		ModulationLFOToFilterCutoffFrequency,
		ModulationEnvelopeToFilterCutoffFrequency,
		EndAddressCoarseOffset,
		ModulationLFOToVolume,
		Unused1,
		ChorusEffectsSend,
		ReverbEffectsSend,
		Pan,
		Unused2,
		Unused3,
		Unused4,
		DelayModulationLFO,
		FrequencyModulationLFO,
		DelayVibratoLFO,
		FrequencyVibratoLFO,
		DelayModulationEnvelope,
		AttackModulationEnvelope,
		HoldModulationEnvelope,
		DecayModulationEnvelope,
		SustainModulationEnvelope,
		ReleaseModulationEnvelope,
		KeyNumberToModulationEnvelopeHold,
		KeyNumberToModulationEnvelopeDecay,
		DelayVolumeEnvelope,
		AttackVolumeEnvelope,
		HoldVolumeEnvelope,
		DecayVolumeEnvelope,
		SustainVolumeEnvelope,
		ReleaseVolumeEnvelope,
		KeyNumberToVolumeEnvelopeHold,
		KeyNumberToVolumeEnvelopeDecay,
		Instrument,
		Reserved1,
		KeyRange,
		VelocityRange,
		StartLoopAddressCoarseOffset,
		KeyNumber,
		Velocity,
		InitialAttenuation,
		Reserved2,
		EndLoopAddressCoarseOffset,
		CoarseTune,
		FineTune,
		SampleID,
		SampleModes,
		Reserved3,
		ScaleTuning,
		ExclusiveClass,
		OverridingRootKey,
		Unused5,
		UnusedEnd
	}
	public class InfoChunk
	{
		public SFVersion SoundFontVersion { get; }

		public string WaveTableSoundEngine { get; set; }

		public string BankName { get; set; }

		public string DataROM { get; set; }

		public string CreationDate { get; set; }

		public string Author { get; set; }

		public string TargetProduct { get; set; }

		public string Copyright { get; set; }

		public string Comments { get; set; }

		public string Tools { get; set; }

		public SFVersion ROMVersion { get; set; }

		internal InfoChunk(RiffChunk chunk)
		{
			bool flag = false;
			bool flag2 = false;
			if (chunk.ReadChunkID() != "INFO")
			{
				throw new InvalidDataException("Not an INFO chunk");
			}
			RiffChunk nextSubChunk;
			while ((nextSubChunk = chunk.GetNextSubChunk()) != null)
			{
				switch (nextSubChunk.ChunkID)
				{
				case "ifil":
					flag = true;
					SoundFontVersion = nextSubChunk.GetDataAsStructure(new SFVersionBuilder());
					break;
				case "isng":
					WaveTableSoundEngine = nextSubChunk.GetDataAsString();
					break;
				case "INAM":
					flag2 = true;
					BankName = nextSubChunk.GetDataAsString();
					break;
				case "irom":
					DataROM = nextSubChunk.GetDataAsString();
					break;
				case "iver":
					ROMVersion = nextSubChunk.GetDataAsStructure(new SFVersionBuilder());
					break;
				case "ICRD":
					CreationDate = nextSubChunk.GetDataAsString();
					break;
				case "IENG":
					Author = nextSubChunk.GetDataAsString();
					break;
				case "IPRD":
					TargetProduct = nextSubChunk.GetDataAsString();
					break;
				case "ICOP":
					Copyright = nextSubChunk.GetDataAsString();
					break;
				case "ICMT":
					Comments = nextSubChunk.GetDataAsString();
					break;
				case "ISFT":
					Tools = nextSubChunk.GetDataAsString();
					break;
				default:
					throw new InvalidDataException("Unknown chunk type " + nextSubChunk.ChunkID);
				}
			}
			if (!flag)
			{
				throw new InvalidDataException("Missing SoundFont version information");
			}
			if (!flag2)
			{
				throw new InvalidDataException("Missing SoundFont name information");
			}
		}

		public override string ToString()
		{
			return string.Format("Bank Name: {0}\r\nAuthor: {1}\r\nCopyright: {2}\r\nCreation Date: {3}\r\nTools: {4}\r\nComments: {5}\r\nSound Engine: {6}\r\nSoundFont Version: {7}\r\nTarget Product: {8}\r\nData ROM: {9}\r\nROM Version: {10}", BankName, Author, Copyright, CreationDate, Tools, "TODO-fix comments", WaveTableSoundEngine, SoundFontVersion, TargetProduct, DataROM, ROMVersion);
		}
	}
	public class Instrument
	{
		internal ushort startInstrumentZoneIndex;

		internal ushort endInstrumentZoneIndex;

		public string Name { get; set; }

		public Zone[] Zones { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
	internal class InstrumentBuilder : StructureBuilder<Instrument>
	{
		private Instrument lastInstrument;

		public override int Length => 22;

		public Instrument[] Instruments => data.ToArray();

		public override Instrument Read(BinaryReader br)
		{
			Instrument instrument = new Instrument();
			string text = Encoding.UTF8.GetString(br.ReadBytes(20), 0, 20);
			if (text.IndexOf('\0') >= 0)
			{
				text = text.Substring(0, text.IndexOf('\0'));
			}
			instrument.Name = text;
			instrument.startInstrumentZoneIndex = br.ReadUInt16();
			if (lastInstrument != null)
			{
				lastInstrument.endInstrumentZoneIndex = (ushort)(instrument.startInstrumentZoneIndex - 1);
			}
			data.Add(instrument);
			lastInstrument = instrument;
			return instrument;
		}

		public override void Write(BinaryWriter bw, Instrument instrument)
		{
		}

		public void LoadZones(Zone[] zones)
		{
			for (int i = 0; i < data.Count - 1; i++)
			{
				Instrument instrument = data[i];
				instrument.Zones = new Zone[instrument.endInstrumentZoneIndex - instrument.startInstrumentZoneIndex + 1];
				Array.Copy(zones, instrument.startInstrumentZoneIndex, instrument.Zones, 0, instrument.Zones.Length);
			}
			data.RemoveAt(data.Count - 1);
		}
	}
	public enum TransformEnum
	{
		Linear
	}
	public class Modulator
	{
		public ModulatorType SourceModulationData { get; set; }

		public GeneratorEnum DestinationGenerator { get; set; }

		public short Amount { get; set; }

		public ModulatorType SourceModulationAmount { get; set; }

		public TransformEnum SourceTransform { get; set; }

		public override string ToString()
		{
			return $"Modulator {SourceModulationData} {DestinationGenerator} {Amount} {SourceModulationAmount} {SourceTransform}";
		}
	}
	internal class ModulatorBuilder : StructureBuilder<Modulator>
	{
		public override int Length => 10;

		public Modulator[] Modulators => data.ToArray();

		public override Modulator Read(BinaryReader br)
		{
			Modulator modulator = new Modulator();
			modulator.SourceModulationData = new ModulatorType(br.ReadUInt16());
			modulator.DestinationGenerator = (GeneratorEnum)br.ReadUInt16();
			modulator.Amount = br.ReadInt16();
			modulator.SourceModulationAmount = new ModulatorType(br.ReadUInt16());
			modulator.SourceTransform = (TransformEnum)br.ReadUInt16();
			data.Add(modulator);
			return modulator;
		}

		public override void Write(BinaryWriter bw, Modulator o)
		{
		}
	}
	public enum ControllerSourceEnum
	{
		NoController = 0,
		NoteOnVelocity = 2,
		NoteOnKeyNumber = 3,
		PolyPressure = 10,
		ChannelPressure = 13,
		PitchWheel = 14,
		PitchWheelSensitivity = 16
	}
	public enum SourceTypeEnum
	{
		Linear,
		Concave,
		Convex,
		Switch
	}
	public class ModulatorType
	{
		private bool polarity;

		private bool direction;

		private bool midiContinuousController;

		private ControllerSourceEnum controllerSource;

		private SourceTypeEnum sourceType;

		private ushort midiContinuousControllerNumber;

		internal ModulatorType(ushort raw)
		{
			polarity = (raw & 0x200) == 512;
			direction = (raw & 0x100) == 256;
			midiContinuousController = (raw & 0x80) == 128;
			sourceType = (SourceTypeEnum)((raw & 0xFC00) >> 10);
			controllerSource = (ControllerSourceEnum)(raw & 0x7F);
			midiContinuousControllerNumber = (ushort)(raw & 0x7Fu);
		}

		public override string ToString()
		{
			if (midiContinuousController)
			{
				return $"{sourceType} CC{midiContinuousControllerNumber}";
			}
			return $"{sourceType} {controllerSource}";
		}
	}
	public class Preset
	{
		internal ushort startPresetZoneIndex;

		internal ushort endPresetZoneIndex;

		internal uint library;

		internal uint genre;

		internal uint morphology;

		public string Name { get; set; }

		public ushort PatchNumber { get; set; }

		public ushort Bank { get; set; }

		public Zone[] Zones { get; set; }

		public override string ToString()
		{
			return $"{Bank}-{PatchNumber} {Name}";
		}
	}
	internal class PresetBuilder : StructureBuilder<Preset>
	{
		private Preset lastPreset;

		public override int Length => 38;

		public Preset[] Presets => data.ToArray();

		public override Preset Read(BinaryReader br)
		{
			Preset preset = new Preset();
			string text = Encoding.UTF8.GetString(br.ReadBytes(20), 0, 20);
			if (text.IndexOf('\0') >= 0)
			{
				text = text.Substring(0, text.IndexOf('\0'));
			}
			preset.Name = text;
			preset.PatchNumber = br.ReadUInt16();
			preset.Bank = br.ReadUInt16();
			preset.startPresetZoneIndex = br.ReadUInt16();
			preset.library = br.ReadUInt32();
			preset.genre = br.ReadUInt32();
			preset.morphology = br.ReadUInt32();
			if (lastPreset != null)
			{
				lastPreset.endPresetZoneIndex = (ushort)(preset.startPresetZoneIndex - 1);
			}
			data.Add(preset);
			lastPreset = preset;
			return preset;
		}

		public override void Write(BinaryWriter bw, Preset preset)
		{
		}

		public void LoadZones(Zone[] presetZones)
		{
			for (int i = 0; i < data.Count - 1; i++)
			{
				Preset preset = data[i];
				preset.Zones = new Zone[preset.endPresetZoneIndex - preset.startPresetZoneIndex + 1];
				Array.Copy(presetZones, preset.startPresetZoneIndex, preset.Zones, 0, preset.Zones.Length);
			}
			data.RemoveAt(data.Count - 1);
		}
	}
	public class PresetsChunk
	{
		private PresetBuilder presetHeaders = new PresetBuilder();

		private ZoneBuilder presetZones = new ZoneBuilder();

		private ModulatorBuilder presetZoneModulators = new ModulatorBuilder();

		private GeneratorBuilder presetZoneGenerators = new GeneratorBuilder();

		private InstrumentBuilder instruments = new InstrumentBuilder();

		private ZoneBuilder instrumentZones = new ZoneBuilder();

		private ModulatorBuilder instrumentZoneModulators = new ModulatorBuilder();

		private GeneratorBuilder instrumentZoneGenerators = new GeneratorBuilder();

		private SampleHeaderBuilder sampleHeaders = new SampleHeaderBuilder();

		public Preset[] Presets => presetHeaders.Presets;

		public Instrument[] Instruments => instruments.Instruments;

		public SampleHeader[] SampleHeaders => sampleHeaders.SampleHeaders;

		internal PresetsChunk(RiffChunk chunk)
		{
			string text = chunk.ReadChunkID();
			if (text != "pdta")
			{
				throw new InvalidDataException($"Not a presets data chunk ({text})");
			}
			RiffChunk nextSubChunk;
			while ((nextSubChunk = chunk.GetNextSubChunk()) != null)
			{
				switch (nextSubChunk.ChunkID)
				{
				case "PHDR":
				case "phdr":
					nextSubChunk.GetDataAsStructureArray(presetHeaders);
					break;
				case "PBAG":
				case "pbag":
					nextSubChunk.GetDataAsStructureArray(presetZones);
					break;
				case "PMOD":
				case "pmod":
					nextSubChunk.GetDataAsStructureArray(presetZoneModulators);
					break;
				case "PGEN":
				case "pgen":
					nextSubChunk.GetDataAsStructureArray(presetZoneGenerators);
					break;
				case "INST":
				case "inst":
					nextSubChunk.GetDataAsStructureArray(instruments);
					break;
				case "IBAG":
				case "ibag":
					nextSubChunk.GetDataAsStructureArray(instrumentZones);
					break;
				case "IMOD":
				case "imod":
					nextSubChunk.GetDataAsStructureArray(instrumentZoneModulators);
					break;
				case "IGEN":
				case "igen":
					nextSubChunk.GetDataAsStructureArray(instrumentZoneGenerators);
					break;
				case "SHDR":
				case "shdr":
					nextSubChunk.GetDataAsStructureArray(sampleHeaders);
					break;
				default:
					throw new InvalidDataException($"Unknown chunk type {nextSubChunk.ChunkID}");
				}
			}
			instrumentZoneGenerators.Load(sampleHeaders.SampleHeaders);
			instrumentZones.Load(instrumentZoneModulators.Modulators, instrumentZoneGenerators.Generators);
			instruments.LoadZones(instrumentZones.Zones);
			presetZoneGenerators.Load(instruments.Instruments);
			presetZones.Load(presetZoneModulators.Modulators, presetZoneGenerators.Generators);
			presetHeaders.LoadZones(presetZones.Zones);
			sampleHeaders.RemoveEOS();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Preset Headers:\r\n");
			Preset[] presets = presetHeaders.Presets;
			foreach (Preset arg in presets)
			{
				stringBuilder.AppendFormat("{0}\r\n", arg);
			}
			stringBuilder.Append("Instruments:\r\n");
			Instrument[] array = instruments.Instruments;
			foreach (Instrument arg2 in array)
			{
				stringBuilder.AppendFormat("{0}\r\n", arg2);
			}
			return stringBuilder.ToString();
		}
	}
	internal class RiffChunk
	{
		private string chunkID;

		private BinaryReader riffFile;

		public string ChunkID
		{
			get
			{
				return chunkID;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ChunkID may not be null");
				}
				if (value.Length != 4)
				{
					throw new ArgumentException("ChunkID must be four characters");
				}
				chunkID = value;
			}
		}

		public uint ChunkSize { get; private set; }

		public long DataOffset { get; private set; }

		public static RiffChunk GetTopLevelChunk(BinaryReader file)
		{
			RiffChunk riffChunk = new RiffChunk(file);
			riffChunk.ReadChunk();
			return riffChunk;
		}

		private RiffChunk(BinaryReader file)
		{
			riffFile = file;
			chunkID = "????";
			ChunkSize = 0u;
			DataOffset = 0L;
		}

		public string ReadChunkID()
		{
			byte[] array = riffFile.ReadBytes(4);
			if (array.Length != 4)
			{
				throw new InvalidDataException("Couldn't read Chunk ID");
			}
			return ByteEncoding.Instance.GetString(array, 0, array.Length);
		}

		private void ReadChunk()
		{
			chunkID = ReadChunkID();
			ChunkSize = riffFile.ReadUInt32();
			DataOffset = riffFile.BaseStream.Position;
		}

		public RiffChunk GetNextSubChunk()
		{
			if (riffFile.BaseStream.Position + 8 < DataOffset + ChunkSize)
			{
				RiffChunk riffChunk = new RiffChunk(riffFile);
				riffChunk.ReadChunk();
				return riffChunk;
			}
			return null;
		}

		public byte[] GetData()
		{
			riffFile.BaseStream.Position = DataOffset;
			byte[] array = riffFile.ReadBytes((int)ChunkSize);
			if (array.Length != ChunkSize)
			{
				throw new InvalidDataException($"Couldn't read chunk's data Chunk: {this}, read {array.Length} bytes");
			}
			return array;
		}

		public string GetDataAsString()
		{
			byte[] data = GetData();
			if (data == null)
			{
				return null;
			}
			return ByteEncoding.Instance.GetString(data, 0, data.Length);
		}

		public T GetDataAsStructure<T>(StructureBuilder<T> s)
		{
			riffFile.BaseStream.Position = DataOffset;
			if (s.Length != ChunkSize)
			{
				throw new InvalidDataException($"Chunk size is: {ChunkSize} so can't read structure of: {s.Length}");
			}
			return s.Read(riffFile);
		}

		public T[] GetDataAsStructureArray<T>(StructureBuilder<T> s)
		{
			riffFile.BaseStream.Position = DataOffset;
			if ((long)ChunkSize % (long)s.Length != 0L)
			{
				throw new InvalidDataException($"Chunk size is: {ChunkSize} not a multiple of structure size: {s.Length}");
			}
			int num = (int)((long)ChunkSize / (long)s.Length);
			T[] array = new T[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = s.Read(riffFile);
			}
			return array;
		}

		public override string ToString()
		{
			return $"RiffChunk ID: {ChunkID} Size: {ChunkSize} Data Offset: {DataOffset}";
		}
	}
	internal class SampleDataChunk
	{
		public byte[] SampleData { get; private set; }

		public SampleDataChunk(RiffChunk chunk)
		{
			string text = chunk.ReadChunkID();
			if (text != "sdta")
			{
				throw new InvalidDataException("Not a sample data chunk (" + text + ")");
			}
			SampleData = chunk.GetData();
		}
	}
	public class SampleHeader
	{
		public string SampleName;

		public uint Start;

		public uint End;

		public uint StartLoop;

		public uint EndLoop;

		public uint SampleRate;

		public byte OriginalPitch;

		public sbyte PitchCorrection;

		public ushort SampleLink;

		public SFSampleLink SFSampleLink;

		public override string ToString()
		{
			return SampleName;
		}
	}
	internal class SampleHeaderBuilder : StructureBuilder<SampleHeader>
	{
		public override int Length => 46;

		public SampleHeader[] SampleHeaders => data.ToArray();

		public override SampleHeader Read(BinaryReader br)
		{
			SampleHeader sampleHeader = new SampleHeader();
			byte[] array = br.ReadBytes(20);
			sampleHeader.SampleName = ByteEncoding.Instance.GetString(array, 0, array.Length);
			sampleHeader.Start = br.ReadUInt32();
			sampleHeader.End = br.ReadUInt32();
			sampleHeader.StartLoop = br.ReadUInt32();
			sampleHeader.EndLoop = br.ReadUInt32();
			sampleHeader.SampleRate = br.ReadUInt32();
			sampleHeader.OriginalPitch = br.ReadByte();
			sampleHeader.PitchCorrection = br.ReadSByte();
			sampleHeader.SampleLink = br.ReadUInt16();
			sampleHeader.SFSampleLink = (SFSampleLink)br.ReadUInt16();
			data.Add(sampleHeader);
			return sampleHeader;
		}

		public override void Write(BinaryWriter bw, SampleHeader sampleHeader)
		{
		}

		internal void RemoveEOS()
		{
			data.RemoveAt(data.Count - 1);
		}
	}
	public enum SampleMode
	{
		NoLoop,
		LoopContinuously,
		ReservedNoLoop,
		LoopAndContinue
	}
	public enum SFSampleLink : ushort
	{
		MonoSample = 1,
		RightSample = 2,
		LeftSample = 4,
		LinkedSample = 8,
		RomMonoSample = 32769,
		RomRightSample = 32770,
		RomLeftSample = 32772,
		RomLinkedSample = 32776
	}
	public class SFVersion
	{
		public short Major { get; set; }

		public short Minor { get; set; }
	}
	internal class SFVersionBuilder : StructureBuilder<SFVersion>
	{
		public override int Length => 4;

		public override SFVersion Read(BinaryReader br)
		{
			SFVersion sFVersion = new SFVersion();
			sFVersion.Major = br.ReadInt16();
			sFVersion.Minor = br.ReadInt16();
			data.Add(sFVersion);
			return sFVersion;
		}

		public override void Write(BinaryWriter bw, SFVersion v)
		{
			bw.Write(v.Major);
			bw.Write(v.Minor);
		}
	}
	public class SoundFont
	{
		private InfoChunk info;

		private PresetsChunk presetsChunk;

		private SampleDataChunk sampleData;

		public InfoChunk FileInfo => info;

		public Preset[] Presets => presetsChunk.Presets;

		public Instrument[] Instruments => presetsChunk.Instruments;

		public SampleHeader[] SampleHeaders => presetsChunk.SampleHeaders;

		public byte[] SampleData => sampleData.SampleData;

		public SoundFont(string fileName)
			: this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
		{
		}

		public SoundFont(Stream sfFile)
		{
			using (sfFile)
			{
				RiffChunk topLevelChunk = RiffChunk.GetTopLevelChunk(new BinaryReader(sfFile));
				if (topLevelChunk.ChunkID == "RIFF")
				{
					string text = topLevelChunk.ReadChunkID();
					if (text != "sfbk")
					{
						throw new InvalidDataException($"Not a SoundFont ({text})");
					}
					RiffChunk nextSubChunk = topLevelChunk.GetNextSubChunk();
					if (nextSubChunk.ChunkID == "LIST")
					{
						info = new InfoChunk(nextSubChunk);
						RiffChunk nextSubChunk2 = topLevelChunk.GetNextSubChunk();
						sampleData = new SampleDataChunk(nextSubChunk2);
						nextSubChunk2 = topLevelChunk.GetNextSubChunk();
						presetsChunk = new PresetsChunk(nextSubChunk2);
						return;
					}
					throw new InvalidDataException($"Not info list found ({nextSubChunk.ChunkID})");
				}
				throw new InvalidDataException("Not a RIFF file");
			}
		}

		public override string ToString()
		{
			return $"Info Chunk:\r\n{info}\r\nPresets Chunk:\r\n{presetsChunk}";
		}
	}
	internal abstract class StructureBuilder<T>
	{
		protected List<T> data;

		public abstract int Length { get; }

		public T[] Data => data.ToArray();

		public StructureBuilder()
		{
			Reset();
		}

		public abstract T Read(BinaryReader br);

		public abstract void Write(BinaryWriter bw, T o);

		public void Reset()
		{
			data = new List<T>();
		}
	}
	public class Zone
	{
		internal ushort generatorIndex;

		internal ushort modulatorIndex;

		internal ushort generatorCount;

		internal ushort modulatorCount;

		public Modulator[] Modulators { get; set; }

		public Generator[] Generators { get; set; }

		public override string ToString()
		{
			return $"Zone {generatorCount} Gens:{generatorIndex} {modulatorCount} Mods:{modulatorIndex}";
		}
	}
	internal class ZoneBuilder : StructureBuilder<Zone>
	{
		private Zone lastZone;

		public Zone[] Zones => data.ToArray();

		public override int Length => 4;

		public override Zone Read(BinaryReader br)
		{
			Zone zone = new Zone();
			zone.generatorIndex = br.ReadUInt16();
			zone.modulatorIndex = br.ReadUInt16();
			if (lastZone != null)
			{
				lastZone.generatorCount = (ushort)(zone.generatorIndex - lastZone.generatorIndex);
				lastZone.modulatorCount = (ushort)(zone.modulatorIndex - lastZone.modulatorIndex);
			}
			data.Add(zone);
			lastZone = zone;
			return zone;
		}

		public override void Write(BinaryWriter bw, Zone zone)
		{
		}

		public void Load(Modulator[] modulators, Generator[] generators)
		{
			for (int i = 0; i < data.Count - 1; i++)
			{
				Zone zone = data[i];
				zone.Generators = new Generator[zone.generatorCount];
				Array.Copy(generators, zone.generatorIndex, zone.Generators, 0, zone.generatorCount);
				zone.Modulators = new Modulator[zone.modulatorCount];
				Array.Copy(modulators, zone.modulatorIndex, zone.Modulators, 0, zone.modulatorCount);
			}
			data.RemoveAt(data.Count - 1);
		}
	}
}
namespace NAudio.FileFormats.Wav
{
	internal class WaveFileChunkReader
	{
		private WaveFormat waveFormat;

		private long dataChunkPosition;

		private long dataChunkLength;

		private List<RiffChunk> riffChunks;

		private readonly bool strictMode;

		private bool isRf64;

		private readonly bool storeAllChunks;

		private long riffSize;

		public WaveFormat WaveFormat => waveFormat;

		public long DataChunkPosition => dataChunkPosition;

		public long DataChunkLength => dataChunkLength;

		public List<RiffChunk> RiffChunks => riffChunks;

		public WaveFileChunkReader()
		{
			storeAllChunks = true;
			strictMode = false;
		}

		public void ReadWaveHeader(Stream stream)
		{
			dataChunkPosition = -1L;
			waveFormat = null;
			riffChunks = new List<RiffChunk>();
			dataChunkLength = 0L;
			BinaryReader binaryReader = new BinaryReader(stream);
			ReadRiffHeader(binaryReader);
			riffSize = binaryReader.ReadUInt32();
			if (binaryReader.ReadInt32() != ChunkIdentifier.ChunkIdentifierToInt32("WAVE"))
			{
				throw new FormatException("Not a WAVE file - no WAVE header");
			}
			if (isRf64)
			{
				ReadDs64Chunk(binaryReader);
			}
			int num = ChunkIdentifier.ChunkIdentifierToInt32("data");
			int num2 = ChunkIdentifier.ChunkIdentifierToInt32("fmt ");
			long num3 = Math.Min(riffSize + 8, stream.Length);
			while (stream.Position <= num3 - 8)
			{
				int num4 = binaryReader.ReadInt32();
				uint num5 = binaryReader.ReadUInt32();
				if (num4 == num)
				{
					dataChunkPosition = stream.Position;
					if (!isRf64)
					{
						dataChunkLength = num5;
					}
					stream.Position += num5;
				}
				else if (num4 == num2)
				{
					if (num5 > int.MaxValue)
					{
						throw new InvalidDataException($"Format chunk length must be between 0 and {int.MaxValue}.");
					}
					waveFormat = WaveFormat.FromFormatChunk(binaryReader, (int)num5);
				}
				else
				{
					if (num5 > stream.Length - stream.Position)
					{
						if (!strictMode)
						{
						}
						break;
					}
					if (storeAllChunks)
					{
						if (num5 > int.MaxValue)
						{
							throw new InvalidDataException($"RiffChunk chunk length must be between 0 and {int.MaxValue}.");
						}
						riffChunks.Add(GetRiffChunk(stream, num4, (int)num5));
					}
					stream.Position += num5;
				}
				if (num5 % 2u != 0 && binaryReader.PeekChar() == 0)
				{
					stream.Position++;
				}
			}
			if (waveFormat == null)
			{
				throw new FormatException("Invalid WAV file - No fmt chunk found");
			}
			if (dataChunkPosition == -1)
			{
				throw new FormatException("Invalid WAV file - No data chunk found");
			}
		}

		private void ReadDs64Chunk(BinaryReader reader)
		{
			int num = ChunkIdentifier.ChunkIdentifierToInt32("ds64");
			if (reader.ReadInt32() != num)
			{
				throw new FormatException("Invalid RF64 WAV file - No ds64 chunk found");
			}
			int num2 = reader.ReadInt32();
			riffSize = reader.ReadInt64();
			dataChunkLength = reader.ReadInt64();
			reader.ReadInt64();
			reader.ReadBytes(num2 - 24);
		}

		private static RiffChunk GetRiffChunk(Stream stream, int chunkIdentifier, int chunkLength)
		{
			return new RiffChunk(chunkIdentifier, chunkLength, stream.Position);
		}

		private void ReadRiffHeader(BinaryReader br)
		{
			int num = br.ReadInt32();
			if (num == ChunkIdentifier.ChunkIdentifierToInt32("RF64"))
			{
				isRf64 = true;
			}
			else if (num != ChunkIdentifier.ChunkIdentifierToInt32("RIFF"))
			{
				throw new FormatException("Not a WAVE file - no RIFF header");
			}
		}
	}
}
namespace NAudio.FileFormats.Mp3
{
	public class DmoMp3FrameDecompressor : IMp3FrameDecompressor, IDisposable
	{
		private WindowsMediaMp3Decoder mp3Decoder;

		private WaveFormat pcmFormat;

		private MediaBuffer inputMediaBuffer;

		private DmoOutputDataBuffer outputBuffer;

		private bool reposition;

		public WaveFormat OutputFormat => pcmFormat;

		public DmoMp3FrameDecompressor(WaveFormat sourceFormat)
		{
			mp3Decoder = new WindowsMediaMp3Decoder();
			if (!mp3Decoder.MediaObject.SupportsInputWaveFormat(0, sourceFormat))
			{
				throw new ArgumentException("Unsupported input format");
			}
			mp3Decoder.MediaObject.SetInputWaveFormat(0, sourceFormat);
			pcmFormat = new WaveFormat(sourceFormat.SampleRate, sourceFormat.Channels);
			if (!mp3Decoder.MediaObject.SupportsOutputWaveFormat(0, pcmFormat))
			{
				throw new ArgumentException($"Unsupported output format {pcmFormat}");
			}
			mp3Decoder.MediaObject.SetOutputWaveFormat(0, pcmFormat);
			inputMediaBuffer = new MediaBuffer(sourceFormat.AverageBytesPerSecond);
			outputBuffer = new DmoOutputDataBuffer(pcmFormat.AverageBytesPerSecond);
		}

		public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
		{
			inputMediaBuffer.LoadData(frame.RawData, frame.FrameLength);
			if (reposition)
			{
				mp3Decoder.MediaObject.Flush();
				reposition = false;
			}
			mp3Decoder.MediaObject.ProcessInput(0, inputMediaBuffer, DmoInputDataBufferFlags.None, 0L, 0L);
			outputBuffer.MediaBuffer.SetLength(0);
			outputBuffer.StatusFlags = DmoOutputDataBufferFlags.None;
			mp3Decoder.MediaObject.ProcessOutput(DmoProcessOutputFlags.None, 1, new DmoOutputDataBuffer[1] { outputBuffer });
			if (outputBuffer.Length == 0)
			{
				return 0;
			}
			outputBuffer.RetrieveData(dest, destOffset);
			return outputBuffer.Length;
		}

		public void Reset()
		{
			reposition = true;
		}

		public void Dispose()
		{
			if (inputMediaBuffer != null)
			{
				inputMediaBuffer.Dispose();
				inputMediaBuffer = null;
			}
			outputBuffer.Dispose();
			if (mp3Decoder != null)
			{
				mp3Decoder.Dispose();
				mp3Decoder = null;
			}
		}
	}
}
namespace NAudio.Wave
{
	public enum ChannelMode
	{
		Stereo,
		JointStereo,
		DualChannel,
		Mono
	}
	public class Id3v2Tag
	{
		private long tagStartPosition;

		private long tagEndPosition;

		private byte[] rawData;

		public byte[] RawData => rawData;

		public static Id3v2Tag ReadTag(Stream input)
		{
			try
			{
				return new Id3v2Tag(input);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		public static Id3v2Tag Create(IEnumerable<KeyValuePair<string, string>> tags)
		{
			return ReadTag(CreateId3v2TagStream(tags));
		}

		private static byte[] FrameSizeToBytes(int n)
		{
			byte[] bytes = BitConverter.GetBytes(n);
			Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] CreateId3v2Frame(string key, string value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			if (key.Length != 4)
			{
				throw new ArgumentOutOfRangeException("key", "key " + key + " must be 4 characters long");
			}
			byte[] array = new byte[2] { 255, 254 };
			byte[] array2 = new byte[3];
			byte[] array3 = new byte[2];
			byte[] array4 = ((!(key == "COMM")) ? ByteArrayExtensions.Concat(new byte[1] { 1 }, array, Encoding.Unicode.GetBytes(value)) : ByteArrayExtensions.Concat(new byte[1] { 1 }, array2, array3, array, Encoding.Unicode.GetBytes(value)));
			return ByteArrayExtensions.Concat(Encoding.UTF8.GetBytes(key), FrameSizeToBytes(array4.Length), new byte[2], array4);
		}

		private static byte[] GetId3TagHeaderSize(int size)
		{
			byte[] array = new byte[4];
			for (int num = array.Length - 1; num >= 0; num--)
			{
				array[num] = (byte)(size % 128);
				size /= 128;
			}
			return array;
		}

		private static byte[] CreateId3v2TagHeader(IEnumerable<byte[]> frames)
		{
			int num = 0;
			foreach (byte[] frame in frames)
			{
				num += frame.Length;
			}
			return ByteArrayExtensions.Concat(Encoding.UTF8.GetBytes("ID3"), new byte[2] { 3, 0 }, new byte[1], GetId3TagHeaderSize(num));
		}

		private static Stream CreateId3v2TagStream(IEnumerable<KeyValuePair<string, string>> tags)
		{
			List<byte[]> list = new List<byte[]>();
			foreach (KeyValuePair<string, string> tag in tags)
			{
				list.Add(CreateId3v2Frame(tag.Key, tag.Value));
			}
			byte[] array = CreateId3v2TagHeader(list);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(array, 0, array.Length);
			foreach (byte[] item in list)
			{
				memoryStream.Write(item, 0, item.Length);
			}
			memoryStream.Position = 0L;
			return memoryStream;
		}

		private Id3v2Tag(Stream input)
		{
			tagStartPosition = input.Position;
			BinaryReader binaryReader = new BinaryReader(input);
			byte[] array = binaryReader.ReadBytes(10);
			if (array.Length >= 3 && array[0] == 73 && array[1] == 68 && array[2] == 51)
			{
				if ((array[5] & 0x40) == 64)
				{
					byte[] array2 = binaryReader.ReadBytes(4);
					_ = array2[0] * 2097152 + array2[1] * 16384 + array2[2] * 128;
					_ = array2[3];
				}
				int num = array[6] * 2097152;
				num += array[7] * 16384;
				num += array[8] * 128;
				num += array[9];
				binaryReader.ReadBytes(num);
				if ((array[5] & 0x10) == 16)
				{
					binaryReader.ReadBytes(10);
				}
				tagEndPosition = input.Position;
				input.Position = tagStartPosition;
				rawData = binaryReader.ReadBytes((int)(tagEndPosition - tagStartPosition));
				return;
			}
			input.Position = tagStartPosition;
			throw new FormatException("Not an ID3v2 tag");
		}
	}
	public interface IMp3FrameDecompressor : IDisposable
	{
		WaveFormat OutputFormat { get; }

		int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset);

		void Reset();
	}
	public class Mp3Frame
	{
		private static readonly int[,,] bitRates = new int[2, 3, 15]
		{
			{
				{
					0, 32, 64, 96, 128, 160, 192, 224, 256, 288,
					320, 352, 384, 416, 448
				},
				{
					0, 32, 48, 56, 64, 80, 96, 112, 128, 160,
					192, 224, 256, 320, 384
				},
				{
					0, 32, 40, 48, 56, 64, 80, 96, 112, 128,
					160, 192, 224, 256, 320
				}
			},
			{
				{
					0, 32, 48, 56, 64, 80, 96, 112, 128, 144,
					160, 176, 192, 224, 256
				},
				{
					0, 8, 16, 24, 32, 40, 48, 56, 64, 80,
					96, 112, 128, 144, 160
				},
				{
					0, 8, 16, 24, 32, 40, 48, 56, 64, 80,
					96, 112, 128, 144, 160
				}
			}
		};

		private static readonly int[,] samplesPerFrame = new int[2, 3]
		{
			{ 384, 1152, 1152 },
			{ 384, 1152, 576 }
		};

		private static readonly int[] sampleRatesVersion1 = new int[3] { 44100, 48000, 32000 };

		private static readonly int[] sampleRatesVersion2 = new int[3] { 22050, 24000, 16000 };

		private static readonly int[] sampleRatesVersion25 = new int[3] { 11025, 12000, 8000 };

		private const int MaxFrameLength = 16384;

		public int SampleRate { get; private set; }

		public int FrameLength { get; private set; }

		public int BitRate { get; private set; }

		public byte[] RawData { get; private set; }

		public MpegVersion MpegVersion { get; private set; }

		public MpegLayer MpegLayer { get; private set; }

		public ChannelMode ChannelMode { get; private set; }

		public int SampleCount { get; private set; }

		public int ChannelExtension { get; private set; }

		public int BitRateIndex { get; private set; }

		public bool Copyright { get; private set; }

		public bool CrcPresent { get; private set; }

		public long FileOffset { get; private set; }

		public static Mp3Frame LoadFromStream(Stream input)
		{
			return LoadFromStream(input, readData: true);
		}

		public static Mp3Frame LoadFromStream(Stream input, bool readData)
		{
			Mp3Frame mp3Frame = new Mp3Frame();
			mp3Frame.FileOffset = input.Position;
			byte[] array = new byte[4];
			if (input.Read(array, 0, array.Length) < array.Length)
			{
				return null;
			}
			while (!IsValidHeader(array, mp3Frame))
			{
				array[0] = array[1];
				array[1] = array[2];
				array[2] = array[3];
				if (input.Read(array, 3, 1) < 1)
				{
					return null;
				}
				mp3Frame.FileOffset++;
			}
			int num = mp3Frame.FrameLength - 4;
			if (readData)
			{
				mp3Frame.RawData = new byte[mp3Frame.FrameLength];
				Array.Copy(array, mp3Frame.RawData, 4);
				if (input.Read(mp3Frame.RawData, 4, num) < num)
				{
					throw new EndOfStreamException("Unexpected end of stream before frame complete");
				}
			}
			else
			{
				input.Position += num;
			}
			return mp3Frame;
		}

		private Mp3Frame()
		{
		}

		private static bool IsValidHeader(byte[] headerBytes, Mp3Frame frame)
		{
			if (headerBytes[0] == byte.MaxValue && (headerBytes[1] & 0xE0) == 224)
			{
				frame.MpegVersion = (MpegVersion)((headerBytes[1] & 0x18) >> 3);
				if (frame.MpegVersion == MpegVersion.Reserved)
				{
					return false;
				}
				frame.MpegLayer = (MpegLayer)((headerBytes[1] & 6) >> 1);
				if (frame.MpegLayer == MpegLayer.Reserved)
				{
					return false;
				}
				int num = ((frame.MpegLayer != MpegLayer.Layer1) ? ((frame.MpegLayer == MpegLayer.Layer2) ? 1 : 2) : 0);
				frame.CrcPresent = (headerBytes[1] & 1) == 0;
				frame.BitRateIndex = (headerBytes[2] & 0xF0) >> 4;
				if (frame.BitRateIndex == 15)
				{
					return false;
				}
				int num2 = ((frame.MpegVersion != MpegVersion.Version1) ? 1 : 0);
				frame.BitRate = bitRates[num2, num, frame.BitRateIndex] * 1000;
				if (frame.BitRate == 0)
				{
					return false;
				}
				int num3 = (headerBytes[2] & 0xC) >> 2;
				if (num3 == 3)
				{
					return false;
				}
				if (frame.MpegVersion == MpegVersion.Version1)
				{
					frame.SampleRate = sampleRatesVersion1[num3];
				}
				else if (frame.MpegVersion == MpegVersion.Version2)
				{
					frame.SampleRate = sampleRatesVersion2[num3];
				}
				else
				{
					frame.SampleRate = sampleRatesVersion25[num3];
				}
				bool flag = (headerBytes[2] & 2) == 2;
				_ = headerBytes[2];
				frame.ChannelMode = (ChannelMode)((headerBytes[3] & 0xC0) >> 6);
				frame.ChannelExtension = (headerBytes[3] & 0x30) >> 4;
				if (frame.ChannelExtension != 0 && frame.ChannelMode != ChannelMode.JointStereo)
				{
					return false;
				}
				frame.Copyright = (headerBytes[3] & 8) == 8;
				_ = headerBytes[3];
				_ = headerBytes[3];
				int num4 = (flag ? 1 : 0);
				frame.SampleCount = samplesPerFrame[num2, num];
				int num5 = frame.SampleCount / 8;
				if (frame.MpegLayer == MpegLayer.Layer1)
				{
					frame.FrameLength = (num5 * frame.BitRate / frame.SampleRate + num4) * 4;
				}
				else
				{
					frame.FrameLength = num5 * frame.BitRate / frame.SampleRate + num4;
				}
				if (frame.FrameLength > 16384)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}
	public class AcmMp3FrameDecompressor : IMp3FrameDecompressor, IDisposable
	{
		private readonly AcmStream conversionStream;

		private readonly WaveFormat pcmFormat;

		private bool disposed;

		public WaveFormat OutputFormat => pcmFormat;

		public AcmMp3FrameDecompressor(WaveFormat sourceFormat)
		{
			pcmFormat = AcmStream.SuggestPcmFormat(sourceFormat);
			try
			{
				conversionStream = new AcmStream(sourceFormat, pcmFormat);
			}
			catch (Exception)
			{
				disposed = true;
				GC.SuppressFinalize(this);
				throw;
			}
		}

		public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame", "You must provide a non-null Mp3Frame to decompress");
			}
			Array.Copy(frame.RawData, conversionStream.SourceBuffer, frame.FrameLength);
			int sourceBytesConverted;
			int num = conversionStream.Convert(frame.FrameLength, out sourceBytesConverted);
			if (sourceBytesConverted != frame.FrameLength)
			{
				throw new InvalidOperationException($"Couldn't convert the whole MP3 frame (converted {sourceBytesConverted}/{frame.FrameLength})");
			}
			Array.Copy(conversionStream.DestBuffer, 0, dest, destOffset, num);
			return num;
		}

		public void Reset()
		{
			conversionStream.Reposition();
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				if (conversionStream != null)
				{
					conversionStream.Dispose();
				}
				GC.SuppressFinalize(this);
			}
		}

		~AcmMp3FrameDecompressor()
		{
			Dispose();
		}
	}
	public enum MpegLayer
	{
		Reserved,
		Layer3,
		Layer2,
		Layer1
	}
	public enum MpegVersion
	{
		Version25,
		Reserved,
		Version2,
		Version1
	}
	public class XingHeader
	{
		[Flags]
		private enum XingHeaderOptions
		{
			Frames = 1,
			Bytes = 2,
			Toc = 4,
			VbrScale = 8
		}

		private static int[] sr_table = new int[4] { 44100, 48000, 32000, 99999 };

		private int vbrScale = -1;

		private int startOffset;

		private int endOffset;

		private int tocOffset = -1;

		private int framesOffset = -1;

		private int bytesOffset = -1;

		private Mp3Frame frame;

		public int Frames
		{
			get
			{
				if (framesOffset == -1)
				{
					return -1;
				}
				return ReadBigEndian(frame.RawData, framesOffset);
			}
			set
			{
				if (framesOffset == -1)
				{
					throw new InvalidOperationException("Frames flag is not set");
				}
				WriteBigEndian(frame.RawData, framesOffset, value);
			}
		}

		public int Bytes
		{
			get
			{
				if (bytesOffset == -1)
				{
					return -1;
				}
				return ReadBigEndian(frame.RawData, bytesOffset);
			}
			set
			{
				if (framesOffset == -1)
				{
					throw new InvalidOperationException("Bytes flag is not set");
				}
				WriteBigEndian(frame.RawData, bytesOffset, value);
			}
		}

		public int VbrScale => vbrScale;

		public Mp3Frame Mp3Frame => frame;

		private static int ReadBigEndian(byte[] buffer, int offset)
		{
			return (((((buffer[offset] << 8) | buffer[offset + 1]) << 8) | buffer[offset + 2]) << 8) | buffer[offset + 3];
		}

		private void WriteBigEndian(byte[] buffer, int offset, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			for (int i = 0; i < 4; i++)
			{
				buffer[offset + 3 - i] = bytes[i];
			}
		}

		public static XingHeader LoadXingHeader(Mp3Frame frame)
		{
			XingHeader xingHeader = new XingHeader();
			xingHeader.frame = frame;
			int num = 0;
			if (frame.MpegVersion == MpegVersion.Version1)
			{
				num = ((frame.ChannelMode == ChannelMode.Mono) ? 21 : 36);
			}
			else
			{
				if (frame.MpegVersion != MpegVersion.Version2)
				{
					return null;
				}
				num = ((frame.ChannelMode == ChannelMode.Mono) ? 13 : 21);
			}
			if (frame.RawData[num] == 88 && frame.RawData[num + 1] == 105 && frame.RawData[num + 2] == 110 && frame.RawData[num + 3] == 103)
			{
				xingHeader.startOffset = num;
				num += 4;
			}
			else
			{
				if (frame.RawData[num] != 73 || frame.RawData[num + 1] != 110 || frame.RawData[num + 2] != 102 || frame.RawData[num + 3] != 111)
				{
					return null;
				}
				xingHeader.startOffset = num;
				num += 4;
			}
			int num2 = ReadBigEndian(frame.RawData, num);
			num += 4;
			if (((uint)num2 & (true ? 1u : 0u)) != 0)
			{
				xingHeader.framesOffset = num;
				num += 4;
			}
			if (((uint)num2 & 2u) != 0)
			{
				xingHeader.bytesOffset = num;
				num += 4;
			}
			if (((uint)num2 & 4u) != 0)
			{
				xingHeader.tocOffset = num;
				num += 100;
			}
			if (((uint)num2 & 8u) != 0)
			{
				xingHeader.vbrScale = ReadBigEndian(frame.RawData, num);
				num += 4;
			}
			xingHeader.endOffset = num;
			return xingHeader;
		}

		private XingHeader()
		{
		}
	}
	internal enum AcmMetrics
	{
		CountDrivers = 1,
		CountCodecs = 2,
		CountConverters = 3,
		CountFilters = 4,
		CountDisabled = 5,
		CountHardware = 6,
		CountLocalDrivers = 20,
		CountLocalCodecs = 21,
		CountLocalConverters = 22,
		CountLocalFilters = 23,
		CountLocalDisabled = 24,
		HardwareWaveInput = 30,
		HardwareWaveOutput = 31,
		MaxSizeFormat = 50,
		MaxSizeFilter = 51,
		DriverSupport = 100,
		DriverPriority = 101
	}
	[Flags]
	internal enum AcmStreamConvertFlags
	{
		BlockAlign = 4,
		Start = 0x10,
		End = 0x20
	}
	[StructLayout(LayoutKind.Explicit)]
	internal struct MmTime
	{
		public const int TIME_MS = 1;

		public const int TIME_SAMPLES = 2;

		public const int TIME_BYTES = 4;

		[FieldOffset(0)]
		public uint wType;

		[FieldOffset(4)]
		public uint ms;

		[FieldOffset(4)]
		public uint sample;

		[FieldOffset(4)]
		public uint cb;

		[FieldOffset(4)]
		public uint ticks;

		[FieldOffset(4)]
		public byte smpteHour;

		[FieldOffset(5)]
		public byte smpteMin;

		[FieldOffset(6)]
		public byte smpteSec;

		[FieldOffset(7)]
		public byte smpteFrame;

		[FieldOffset(8)]
		public byte smpteFps;

		[FieldOffset(9)]
		public byte smpteDummy;

		[FieldOffset(10)]
		public byte smptePad0;

		[FieldOffset(11)]
		public byte smptePad1;

		[FieldOffset(4)]
		public uint midiSongPtrPos;
	}
	public class WaveCallbackInfo
	{
		private WaveWindow waveOutWindow;

		private WaveWindowNative waveOutWindowNative;

		public WaveCallbackStrategy Strategy { get; private set; }

		public IntPtr Handle { get; private set; }

		public static WaveCallbackInfo FunctionCallback()
		{
			return new WaveCallbackInfo(WaveCallbackStrategy.FunctionCallback, IntPtr.Zero);
		}

		public static WaveCallbackInfo NewWindow()
		{
			return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
		}

		public static WaveCallbackInfo ExistingWindow(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("Handle cannot be zero");
			}
			return new WaveCallbackInfo(WaveCallbackStrategy.ExistingWindow, handle);
		}

		private WaveCallbackInfo(WaveCallbackStrategy strategy, IntPtr handle)
		{
			Strategy = strategy;
			Handle = handle;
		}

		internal void Connect(WaveInterop.WaveCallback callback)
		{
			if (Strategy == WaveCallbackStrategy.NewWindow)
			{
				waveOutWindow = new WaveWindow(callback);
				waveOutWindow.CreateControl();
				Handle = waveOutWindow.Handle;
			}
			else if (Strategy == WaveCallbackStrategy.ExistingWindow)
			{
				waveOutWindowNative = new WaveWindowNative(callback);
				waveOutWindowNative.AssignHandle(Handle);
			}
		}

		internal MmResult WaveOutOpen(out IntPtr waveOutHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
		{
			if (Strategy == WaveCallbackStrategy.FunctionCallback)
			{
				return WaveInterop.waveOutOpen(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
			}
			return WaveInterop.waveOutOpenWindow(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
		}

		internal MmResult WaveInOpen(out IntPtr waveInHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
		{
			if (Strategy == WaveCallbackStrategy.FunctionCallback)
			{
				return WaveInterop.waveInOpen(out waveInHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
			}
			return WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
		}

		internal void Disconnect()
		{
			if (waveOutWindow != null)
			{
				waveOutWindow.Close();
				waveOutWindow = null;
			}
			if (waveOutWindowNative != null)
			{
				waveOutWindowNative.ReleaseHandle();
				waveOutWindowNative = null;
			}
		}
	}
	public enum WaveCallbackStrategy
	{
		FunctionCallback,
		NewWindow,
		ExistingWindow,
		Event
	}
	[StructLayout(LayoutKind.Sequential)]
	internal class WaveHeader
	{
		public IntPtr dataBuffer;

		public int bufferLength;

		public int bytesRecorded;

		public IntPtr userData;

		public WaveHeaderFlags flags;

		public int loops;

		public IntPtr next;

		public IntPtr reserved;
	}
	[Flags]
	public enum WaveHeaderFlags
	{
		BeginLoop = 4,
		Done = 1,
		EndLoop = 8,
		InQueue = 0x10,
		Prepared = 2
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WaveInCapabilities
	{
		private short manufacturerId;

		private short productId;

		private int driverVersion;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		private string productName;

		private SupportedWaveFormat supportedFormats;

		private short channels;

		private short reserved;

		private Guid manufacturerGuid;

		private Guid productGuid;

		private Guid nameGuid;

		private const int MaxProductNameLength = 32;

		public int Channels => channels;

		public string ProductName => productName;

		public Guid NameGuid => nameGuid;

		public Guid ProductGuid => productGuid;

		public Guid ManufacturerGuid => manufacturerGuid;

		public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
		{
			return (supportedFormats & waveFormat) == waveFormat;
		}
	}
	internal static class WaveCapabilitiesHelpers
	{
		public static readonly Guid MicrosoftDefaultManufacturerId = new Guid("d5a47fa8-6d98-11d1-a21a-00a0c9223196");

		public static readonly Guid DefaultWaveOutGuid = new Guid("E36DC310-6D9A-11D1-A21A-00A0C9223196");

		public static readonly Guid DefaultWaveInGuid = new Guid("E36DC311-6D9A-11D1-A21A-00A0C9223196");

		public static string GetNameFromGuid(Guid guid)
		{
			string result = null;
			using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\MediaCategories");
			using RegistryKey registryKey2 = registryKey.OpenSubKey(guid.ToString("B"));
			if (registryKey2 != null)
			{
				return registryKey2.GetValue("Name") as string;
			}
			return result;
		}
	}
	public class WaveInEventArgs : EventArgs
	{
		private byte[] buffer;

		private int bytes;

		public byte[] Buffer => buffer;

		public int BytesRecorded => bytes;

		public WaveInEventArgs(byte[] buffer, int bytes)
		{
			this.buffer = buffer;
			this.bytes = bytes;
		}
	}
	internal class WaveInterop
	{
		[Flags]
		public enum WaveInOutOpenFlags
		{
			CallbackNull = 0,
			CallbackFunction = 0x30000,
			CallbackEvent = 0x50000,
			CallbackWindow = 0x10000,
			CallbackThread = 0x20000
		}

		public enum WaveMessage
		{
			WaveInOpen = 958,
			WaveInClose = 959,
			WaveInData = 960,
			WaveOutClose = 956,
			WaveOutDone = 957,
			WaveOutOpen = 955
		}

		public delegate void WaveCallback(IntPtr hWaveOut, WaveMessage message, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved);

		[DllImport("winmm.dll")]
		public static extern int mmioStringToFOURCC([MarshalAs(UnmanagedType.LPStr)] string s, int flags);

		[DllImport("winmm.dll")]
		public static extern int waveOutGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutPrepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutUnprepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutWrite(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutOpen(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll", EntryPoint = "waveOutOpen")]
		public static extern MmResult waveOutOpenWindow(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutReset(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutClose(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutPause(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutRestart(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutGetPosition(IntPtr hWaveOut, ref MmTime mmTime, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

		[DllImport("winmm.dll", CharSet = CharSet.Auto)]
		public static extern MmResult waveOutGetDevCaps(IntPtr deviceID, out WaveOutCapabilities waveOutCaps, int waveOutCapsSize);

		[DllImport("winmm.dll")]
		public static extern int waveInGetNumDevs();

		[DllImport("winmm.dll", CharSet = CharSet.Auto)]
		public static extern MmResult waveInGetDevCaps(IntPtr deviceID, out WaveInCapabilities waveInCaps, int waveInCapsSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInAddBuffer(IntPtr hWaveIn, WaveHeader pwh, int cbwh);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInClose(IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInOpen(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll", EntryPoint = "waveInOpen")]
		public static extern MmResult waveInOpenWindow(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInPrepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInUnprepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInReset(IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInStart(IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInStop(IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInGetPosition(IntPtr hWaveIn, out MmTime mmTime, int uSize);
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WaveOutCapabilities
	{
		private short manufacturerId;

		private short productId;

		private int driverVersion;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		private string productName;

		private SupportedWaveFormat supportedFormats;

		private short channels;

		private short reserved;

		private WaveOutSupport support;

		private Guid manufacturerGuid;

		private Guid productGuid;

		private Guid nameGuid;

		private const int MaxProductNameLength = 32;

		public int Channels => channels;

		public bool SupportsPlaybackRateControl => (support & WaveOutSupport.PlaybackRate) == WaveOutSupport.PlaybackRate;

		public string ProductName => productName;

		public Guid NameGuid => nameGuid;

		public Guid ProductGuid => productGuid;

		public Guid ManufacturerGuid => manufacturerGuid;

		public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
		{
			return (supportedFormats & waveFormat) == waveFormat;
		}
	}
	[Flags]
	public enum SupportedWaveFormat
	{
		WAVE_FORMAT_1M08 = 1,
		WAVE_FORMAT_1S08 = 2,
		WAVE_FORMAT_1M16 = 4,
		WAVE_FORMAT_1S16 = 8,
		WAVE_FORMAT_2M08 = 0x10,
		WAVE_FORMAT_2S08 = 0x20,
		WAVE_FORMAT_2M16 = 0x40,
		WAVE_FORMAT_2S16 = 0x80,
		WAVE_FORMAT_4M08 = 0x100,
		WAVE_FORMAT_4S08 = 0x200,
		WAVE_FORMAT_4M16 = 0x400,
		WAVE_FORMAT_4S16 = 0x800,
		WAVE_FORMAT_44M08 = 0x100,
		WAVE_FORMAT_44S08 = 0x200,
		WAVE_FORMAT_44M16 = 0x400,
		WAVE_FORMAT_44S16 = 0x800,
		WAVE_FORMAT_48M08 = 0x1000,
		WAVE_FORMAT_48S08 = 0x2000,
		WAVE_FORMAT_48M16 = 0x4000,
		WAVE_FORMAT_48S16 = 0x8000,
		WAVE_FORMAT_96M08 = 0x10000,
		WAVE_FORMAT_96S08 = 0x20000,
		WAVE_FORMAT_96M16 = 0x40000,
		WAVE_FORMAT_96S16 = 0x80000
	}
	[Flags]
	internal enum WaveOutSupport
	{
		Pitch = 1,
		PlaybackRate = 2,
		Volume = 4,
		LRVolume = 8,
		Sync = 0x10,
		SampleAccurate = 0x20
	}
	internal class WaveWindowNative : NativeWindow
	{
		private WaveInterop.WaveCallback waveCallback;

		public WaveWindowNative(WaveInterop.WaveCallback waveCallback)
		{
			this.waveCallback = waveCallback;
		}

		protected override void WndProc(ref Message m)
		{
			WaveInterop.WaveMessage msg = (WaveInterop.WaveMessage)m.Msg;
			switch (msg)
			{
			case WaveInterop.WaveMessage.WaveOutDone:
			case WaveInterop.WaveMessage.WaveInData:
			{
				IntPtr wParam = m.WParam;
				WaveHeader waveHeader = new WaveHeader();
				Marshal.PtrToStructure(m.LParam, waveHeader);
				waveCallback(wParam, msg, IntPtr.Zero, waveHeader, IntPtr.Zero);
				break;
			}
			case WaveInterop.WaveMessage.WaveOutOpen:
			case WaveInterop.WaveMessage.WaveOutClose:
			case WaveInterop.WaveMessage.WaveInOpen:
			case WaveInterop.WaveMessage.WaveInClose:
				waveCallback(m.WParam, msg, IntPtr.Zero, null, IntPtr.Zero);
				break;
			default:
				base.WndProc(ref m);
				break;
			}
		}
	}
	internal class WaveWindow : Form
	{
		private WaveInterop.WaveCallback waveCallback;

		public WaveWindow(WaveInterop.WaveCallback waveCallback)
		{
			this.waveCallback = waveCallback;
		}

		protected override void WndProc(ref Message m)
		{
			WaveInterop.WaveMessage msg = (WaveInterop.WaveMessage)m.Msg;
			switch (msg)
			{
			case WaveInterop.WaveMessage.WaveOutDone:
			case WaveInterop.WaveMessage.WaveInData:
			{
				IntPtr wParam = m.WParam;
				WaveHeader waveHeader = new WaveHeader();
				Marshal.PtrToStructure(m.LParam, waveHeader);
				waveCallback(wParam, msg, IntPtr.Zero, waveHeader, IntPtr.Zero);
				break;
			}
			case WaveInterop.WaveMessage.WaveOutOpen:
			case WaveInterop.WaveMessage.WaveOutClose:
			case WaveInterop.WaveMessage.WaveInOpen:
			case WaveInterop.WaveMessage.WaveInClose:
				waveCallback(m.WParam, msg, IntPtr.Zero, null, IntPtr.Zero);
				break;
			default:
				base.WndProc(ref m);
				break;
			}
		}
	}
	public static class WaveExtensionMethods
	{
		public static ISampleProvider ToSampleProvider(this IWaveProvider waveProvider)
		{
			return SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
		}

		public static void Init(this IWavePlayer wavePlayer, ISampleProvider sampleProvider, bool convertTo16Bit = false)
		{
			IWaveProvider waveProvider2;
			if (!convertTo16Bit)
			{
				IWaveProvider waveProvider = new SampleToWaveProvider(sampleProvider);
				waveProvider2 = waveProvider;
			}
			else
			{
				IWaveProvider waveProvider = new SampleToWaveProvider16(sampleProvider);
				waveProvider2 = waveProvider;
			}
			IWaveProvider waveProvider3 = waveProvider2;
			wavePlayer.Init(waveProvider3);
		}

		public static WaveFormat AsStandardWaveFormat(this WaveFormat waveFormat)
		{
			if (!(waveFormat is WaveFormatExtensible waveFormatExtensible))
			{
				return waveFormat;
			}
			return waveFormatExtensible.ToStandardWaveFormat();
		}

		public static IWaveProvider ToWaveProvider(this ISampleProvider sampleProvider)
		{
			return new SampleToWaveProvider(sampleProvider);
		}

		public static IWaveProvider ToWaveProvider16(this ISampleProvider sampleProvider)
		{
			return new SampleToWaveProvider16(sampleProvider);
		}

		public static ISampleProvider FollowedBy(this ISampleProvider sampleProvider, ISampleProvider next)
		{
			return new ConcatenatingSampleProvider(new ISampleProvider[2] { sampleProvider, next });
		}

		public static ISampleProvider FollowedBy(this ISampleProvider sampleProvider, TimeSpan silenceDuration, ISampleProvider next)
		{
			OffsetSampleProvider offsetSampleProvider = new OffsetSampleProvider(sampleProvider)
			{
				LeadOut = silenceDuration
			};
			return new ConcatenatingSampleProvider(new ISampleProvider[2] { offsetSampleProvider, next });
		}

		public static ISampleProvider Skip(this ISampleProvider sampleProvider, TimeSpan skipDuration)
		{
			return new OffsetSampleProvider(sampleProvider)
			{
				SkipOver = skipDuration
			};
		}

		public static ISampleProvider Take(this ISampleProvider sampleProvider, TimeSpan takeDuration)
		{
			return new OffsetSampleProvider(sampleProvider)
			{
				Take = takeDuration
			};
		}

		public static ISampleProvider ToMono(this ISampleProvider sourceProvider, float leftVol = 0.5f, float rightVol = 0.5f)
		{
			if (sourceProvider.WaveFormat.Channels == 1)
			{
				return sourceProvider;
			}
			return new StereoToMonoSampleProvider(sourceProvider)
			{
				LeftVolume = leftVol,
				RightVolume = rightVol
			};
		}

		public static ISampleProvider ToStereo(this ISampleProvider sourceProvider, float leftVol = 1f, float rightVol = 1f)
		{
			if (sourceProvider.WaveFormat.Channels == 2)
			{
				return sourceProvider;
			}
			return new MonoToStereoSampleProvider(sourceProvider)
			{
				LeftVolume = leftVol,
				RightVolume = rightVol
			};
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class AdpcmWaveFormat : WaveFormat
	{
		private short samplesPerBlock;

		private short numCoeff;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
		private short[] coefficients;

		public int SamplesPerBlock => samplesPerBlock;

		public int NumCoefficients => numCoeff;

		public short[] Coefficients => coefficients;

		private AdpcmWaveFormat()
			: this(8000, 1)
		{
		}

		public AdpcmWaveFormat(int sampleRate, int channels)
			: base(sampleRate, 0, channels)
		{
			waveFormatTag = WaveFormatEncoding.Adpcm;
			extraSize = 32;
			switch (base.sampleRate)
			{
			case 8000:
			case 11025:
				blockAlign = 256;
				break;
			case 22050:
				blockAlign = 512;
				break;
			default:
				blockAlign = 1024;
				break;
			}
			bitsPerSample = 4;
			samplesPerBlock = (short)((blockAlign - 7 * channels) * 8 / (bitsPerSample * channels) + 2);
			averageBytesPerSecond = base.SampleRate * blockAlign / samplesPerBlock;
			numCoeff = 7;
			coefficients = new short[14]
			{
				256, 0, 512, -256, 0, 0, 192, 64, 240, 0,
				460, -208, 392, -232
			};
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			writer.Write(samplesPerBlock);
			writer.Write(numCoeff);
			short[] array = coefficients;
			foreach (short value in array)
			{
				writer.Write(value);
			}
		}

		public override string ToString()
		{
			return $"Microsoft ADPCM {base.SampleRate} Hz {channels} channels {bitsPerSample} bits per sample {samplesPerBlock} samples per block";
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class Gsm610WaveFormat : WaveFormat
	{
		private readonly short samplesPerBlock;

		public short SamplesPerBlock => samplesPerBlock;

		public Gsm610WaveFormat()
		{
			waveFormatTag = WaveFormatEncoding.Gsm610;
			channels = 1;
			averageBytesPerSecond = 1625;
			bitsPerSample = 0;
			blockAlign = 65;
			sampleRate = 8000;
			extraSize = 2;
			samplesPerBlock = 320;
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			writer.Write(samplesPerBlock);
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class ImaAdpcmWaveFormat : WaveFormat
	{
		private short samplesPerBlock;

		private ImaAdpcmWaveFormat()
		{
		}

		public ImaAdpcmWaveFormat(int sampleRate, int channels, int bitsPerSample)
		{
			waveFormatTag = WaveFormatEncoding.DviAdpcm;
			base.sampleRate = sampleRate;
			base.channels = (short)channels;
			base.bitsPerSample = (short)bitsPerSample;
			extraSize = 2;
			blockAlign = 0;
			averageBytesPerSecond = 0;
			samplesPerBlock = 0;
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class Mp3WaveFormat : WaveFormat
	{
		public Mp3WaveFormatId id;

		public Mp3WaveFormatFlags flags;

		public ushort blockSize;

		public ushort framesPerBlock;

		public ushort codecDelay;

		private const short Mp3WaveFormatExtraBytes = 12;

		public Mp3WaveFormat(int sampleRate, int channels, int blockSize, int bitRate)
		{
			waveFormatTag = WaveFormatEncoding.MpegLayer3;
			base.channels = (short)channels;
			averageBytesPerSecond = bitRate / 8;
			bitsPerSample = 0;
			blockAlign = 1;
			base.sampleRate = sampleRate;
			extraSize = 12;
			id = Mp3WaveFormatId.Mpeg;
			flags = Mp3WaveFormatFlags.PaddingIso;
			this.blockSize = (ushort)blockSize;
			framesPerBlock = 1;
			codecDelay = 0;
		}
	}
	[Flags]
	public enum Mp3WaveFormatFlags
	{
		PaddingIso = 0,
		PaddingOn = 1,
		PaddingOff = 2
	}
	public enum Mp3WaveFormatId : ushort
	{
		Unknown,
		Mpeg,
		ConstantFrameSize
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class OggWaveFormat : WaveFormat
	{
		public uint dwVorbisACMVersion;

		public uint dwLibVorbisVersion;
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class TrueSpeechWaveFormat : WaveFormat
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private short[] unknown;

		public TrueSpeechWaveFormat()
		{
			waveFormatTag = WaveFormatEncoding.DspGroupTrueSpeech;
			channels = 1;
			averageBytesPerSecond = 1067;
			bitsPerSample = 1;
			blockAlign = 32;
			sampleRate = 8000;
			extraSize = 32;
			unknown = new short[16];
			unknown[0] = 1;
			unknown[1] = 240;
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			short[] array = unknown;
			foreach (short value in array)
			{
				writer.Write(value);
			}
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class WaveFormat
	{
		protected WaveFormatEncoding waveFormatTag;

		protected short channels;

		protected int sampleRate;

		protected int averageBytesPerSecond;

		protected short blockAlign;

		protected short bitsPerSample;

		protected short extraSize;

		public WaveFormatEncoding Encoding => waveFormatTag;

		public int Channels => channels;

		public int SampleRate => sampleRate;

		public int AverageBytesPerSecond => averageBytesPerSecond;

		public virtual int BlockAlign => blockAlign;

		public int BitsPerSample => bitsPerSample;

		public int ExtraSize => extraSize;

		public WaveFormat()
			: this(44100, 16, 2)
		{
		}

		public WaveFormat(int sampleRate, int channels)
			: this(sampleRate, 16, channels)
		{
		}

		public int ConvertLatencyToByteSize(int milliseconds)
		{
			int num = (int)((double)AverageBytesPerSecond / 1000.0 * (double)milliseconds);
			if (num % BlockAlign != 0)
			{
				num = num + BlockAlign - num % BlockAlign;
			}
			return num;
		}

		public static WaveFormat CreateCustomFormat(WaveFormatEncoding tag, int sampleRate, int channels, int averageBytesPerSecond, int blockAlign, int bitsPerSample)
		{
			return new WaveFormat
			{
				waveFormatTag = tag,
				channels = (short)channels,
				sampleRate = sampleRate,
				averageBytesPerSecond = averageBytesPerSecond,
				blockAlign = (short)blockAlign,
				bitsPerSample = (short)bitsPerSample,
				extraSize = 0
			};
		}

		public static WaveFormat CreateALawFormat(int sampleRate, int channels)
		{
			return CreateCustomFormat(WaveFormatEncoding.ALaw, sampleRate, channels, sampleRate * channels, channels, 8);
		}

		public static WaveFormat CreateMuLawFormat(int sampleRate, int channels)
		{
			return CreateCustomFormat(WaveFormatEncoding.MuLaw, sampleRate, channels, sampleRate * channels, channels, 8);
		}

		public WaveFormat(int rate, int bits, int channels)
		{
			if (channels < 1)
			{
				throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
			}
			waveFormatTag = WaveFormatEncoding.Pcm;
			this.channels = (short)channels;
			sampleRate = rate;
			bitsPerSample = (short)bits;
			extraSize = 0;
			blockAlign = (short)(channels * (bits / 8));
			averageBytesPerSecond = sampleRate * blockAlign;
		}

		public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
		{
			WaveFormat waveFormat = new WaveFormat();
			waveFormat.waveFormatTag = WaveFormatEncoding.IeeeFloat;
			waveFormat.channels = (short)channels;
			waveFormat.bitsPerSample = 32;
			waveFormat.sampleRate = sampleRate;
			waveFormat.blockAlign = (short)(4 * channels);
			waveFormat.averageBytesPerSecond = sampleRate * waveFormat.blockAlign;
			waveFormat.extraSize = 0;
			return waveFormat;
		}

		public static WaveFormat MarshalFromPtr(IntPtr pointer)
		{
			WaveFormat waveFormat = MarshalHelpers.PtrToStructure<WaveFormat>(pointer);
			switch (waveFormat.Encoding)
			{
			case WaveFormatEncoding.Pcm:
				waveFormat.extraSize = 0;
				break;
			case WaveFormatEncoding.Extensible:
				waveFormat = MarshalHelpers.PtrToStructure<WaveFormatExtensible>(pointer);
				break;
			case WaveFormatEncoding.Adpcm:
				waveFormat = MarshalHelpers.PtrToStructure<AdpcmWaveFormat>(pointer);
				break;
			case WaveFormatEncoding.Gsm610:
				waveFormat = MarshalHelpers.PtrToStructure<Gsm610WaveFormat>(pointer);
				break;
			default:
				if (waveFormat.ExtraSize > 0)
				{
					waveFormat = MarshalHelpers.PtrToStructure<WaveFormatExtraData>(pointer);
				}
				break;
			}
			return waveFormat;
		}

		public static IntPtr MarshalToPtr(WaveFormat format)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(format));
			Marshal.StructureToPtr(format, intPtr, fDeleteOld: false);
			return intPtr;
		}

		public static WaveFormat FromFormatChunk(BinaryReader br, int formatChunkLength)
		{
			WaveFormatExtraData waveFormatExtraData = new WaveFormatExtraData();
			waveFormatExtraData.ReadWaveFormat(br, formatChunkLength);
			waveFormatExtraData.ReadExtraData(br);
			return waveFormatExtraData;
		}

		private void ReadWaveFormat(BinaryReader br, int formatChunkLength)
		{
			if (formatChunkLength < 16)
			{
				throw new InvalidDataException("Invalid WaveFormat Structure");
			}
			waveFormatTag = (WaveFormatEncoding)br.ReadUInt16();
			channels = br.ReadInt16();
			sampleRate = br.ReadInt32();
			averageBytesPerSecond = br.ReadInt32();
			blockAlign = br.ReadInt16();
			bitsPerSample = br.ReadInt16();
			if (formatChunkLength > 16)
			{
				extraSize = br.ReadInt16();
				if (extraSize != formatChunkLength - 18)
				{
					extraSize = (short)(formatChunkLength - 18);
				}
			}
		}

		public WaveFormat(BinaryReader br)
		{
			int formatChunkLength = br.ReadInt32();
			ReadWaveFormat(br, formatChunkLength);
		}

		public override string ToString()
		{
			WaveFormatEncoding waveFormatEncoding = waveFormatTag;
			if (waveFormatEncoding == WaveFormatEncoding.Pcm || waveFormatEncoding == WaveFormatEncoding.Extensible)
			{
				return $"{bitsPerSample} bit PCM: {sampleRate / 1000}kHz {channels} channels";
			}
			return waveFormatTag.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is WaveFormat waveFormat)
			{
				if (waveFormatTag == waveFormat.waveFormatTag && channels == waveFormat.channels && sampleRate == waveFormat.sampleRate && averageBytesPerSecond == waveFormat.averageBytesPerSecond && blockAlign == waveFormat.blockAlign)
				{
					return bitsPerSample == waveFormat.bitsPerSample;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)waveFormatTag ^ (int)channels ^ sampleRate ^ averageBytesPerSecond ^ blockAlign ^ bitsPerSample;
		}

		public virtual void Serialize(BinaryWriter writer)
		{
			writer.Write(18 + extraSize);
			writer.Write((short)Encoding);
			writer.Write((short)Channels);
			writer.Write(SampleRate);
			writer.Write(AverageBytesPerSecond);
			writer.Write((short)BlockAlign);
			writer.Write((short)BitsPerSample);
			writer.Write(extraSize);
		}
	}
	public sealed class WaveFormatCustomMarshaler : ICustomMarshaler
	{
		private static WaveFormatCustomMarshaler marshaler;

		public static ICustomMarshaler GetInstance(string cookie)
		{
			if (marshaler == null)
			{
				marshaler = new WaveFormatCustomMarshaler();
			}
			return marshaler;
		}

		public void CleanUpManagedData(object ManagedObj)
		{
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
			Marshal.FreeHGlobal(pNativeData);
		}

		public int GetNativeDataSize()
		{
			throw new NotImplementedException();
		}

		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			return WaveFormat.MarshalToPtr((WaveFormat)ManagedObj);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return WaveFormat.MarshalFromPtr(pNativeData);
		}
	}
	public enum WaveFormatEncoding : ushort
	{
		Unknown = 0,
		Pcm = 1,
		Adpcm = 2,
		IeeeFloat = 3,
		Vselp = 4,
		IbmCvsd = 5,
		ALaw = 6,
		MuLaw = 7,
		Dts = 8,
		Drm = 9,
		WmaVoice9 = 10,
		OkiAdpcm = 16,
		DviAdpcm = 17,
		ImaAdpcm = 17,
		MediaspaceAdpcm = 18,
		SierraAdpcm = 19,
		G723Adpcm = 20,
		DigiStd = 21,
		DigiFix = 22,
		DialogicOkiAdpcm = 23,
		MediaVisionAdpcm = 24,
		CUCodec = 25,
		YamahaAdpcm = 32,
		SonarC = 33,
		DspGroupTrueSpeech = 34,
		EchoSpeechCorporation1 = 35,
		AudioFileAf36 = 36,
		Aptx = 37,
		AudioFileAf10 = 38,
		Prosody1612 = 39,
		Lrc = 40,
		DolbyAc2 = 48,
		Gsm610 = 49,
		MsnAudio = 50,
		AntexAdpcme = 51,
		ControlResVqlpc = 52,
		DigiReal = 53,
		DigiAdpcm = 54,
		ControlResCr10 = 55,
		WAVE_FORMAT_NMS_VBXADPCM = 56,
		WAVE_FORMAT_CS_IMAADPCM = 57,
		WAVE_FORMAT_ECHOSC3 = 58,
		WAVE_FORMAT_ROCKWELL_ADPCM = 59,
		WAVE_FORMAT_ROCKWELL_DIGITALK = 60,
		WAVE_FORMAT_XEBEC = 61,
		WAVE_FORMAT_G721_ADPCM = 64,
		WAVE_FORMAT_G728_CELP = 65,
		WAVE_FORMAT_MSG723 = 66,
		Mpeg = 80,
		WAVE_FORMAT_RT24 = 82,
		WAVE_FORMAT_PAC = 83,
		MpegLayer3 = 85,
		WAVE_FORMAT_LUCENT_G723 = 89,
		WAVE_FORMAT_CIRRUS = 96,
		WAVE_FORMAT_ESPCM = 97,
		WAVE_FORMAT_VOXWARE = 98,
		WAVE_FORMAT_CANOPUS_ATRAC = 99,
		WAVE_FORMAT_G726_ADPCM = 100,
		WAVE_FORMAT_G722_ADPCM = 101,
		WAVE_FORMAT_DSAT_DISPLAY = 103,
		WAVE_FORMAT_VOXWARE_BYTE_ALIGNED = 105,
		WAVE_FORMAT_VOXWARE_AC8 = 112,
		WAVE_FORMAT_VOXWARE_AC10 = 113,
		WAVE_FORMAT_VOXWARE_AC16 = 114,
		WAVE_FORMAT_VOXWARE_AC20 = 115,
		WAVE_FORMAT_VOXWARE_RT24 = 116,
		WAVE_FORMAT_VOXWARE_RT29 = 117,
		WAVE_FORMAT_VOXWARE_RT29HW = 118,
		WAVE_FORMAT_VOXWARE_VR12 = 119,
		WAVE_FORMAT_VOXWARE_VR18 = 120,
		WAVE_FORMAT_VOXWARE_TQ40 = 121,
		WAVE_FORMAT_SOFTSOUND = 128,
		WAVE_FORMAT_VOXWARE_TQ60 = 129,
		WAVE_FORMAT_MSRT24 = 130,
		WAVE_FORMAT_G729A = 131,
		WAVE_FORMAT_MVI_MVI2 = 132,
		WAVE_FORMAT_DF_G726 = 133,
		WAVE_FORMAT_DF_GSM610 = 134,
		WAVE_FORMAT_ISIAUDIO = 136,
		WAVE_FORMAT_ONLIVE = 137,
		WAVE_FORMAT_SBC24 = 145,
		WAVE_FORMAT_DOLBY_AC3_SPDIF = 146,
		WAVE_FORMAT_MEDIASONIC_G723 = 147,
		WAVE_FORMAT_PROSODY_8KBPS = 148,
		WAVE_FORMAT_ZYXEL_ADPCM = 151,
		WAVE_FORMAT_PHILIPS_LPCBB = 152,
		WAVE_FORMAT_PACKED = 153,
		WAVE_FORMAT_MALDEN_PHONYTALK = 160,
		Gsm = 161,
		G729 = 162,
		G723 = 163,
		Acelp = 164,
		RawAac = 255,
		WAVE_FORMAT_RHETOREX_ADPCM = 256,
		WAVE_FORMAT_IRAT = 257,
		WAVE_FORMAT_VIVO_G723 = 273,
		WAVE_FORMAT_VIVO_SIREN = 274,
		WAVE_FORMAT_DIGITAL_G723 = 291,
		WAVE_FORMAT_SANYO_LD_ADPCM = 293,
		WAVE_FORMAT_SIPROLAB_ACEPLNET = 304,
		WAVE_FORMAT_SIPROLAB_ACELP4800 = 305,
		WAVE_FORMAT_SIPROLAB_ACELP8V3 = 306,
		WAVE_FORMAT_SIPROLAB_G729 = 307,
		WAVE_FORMAT_SIPROLAB_G729A = 308,
		WAVE_FORMAT_SIPROLAB_KELVIN = 309,
		WAVE_FORMAT_G726ADPCM = 320,
		WAVE_FORMAT_QUALCOMM_PUREVOICE = 336,
		WAVE_FORMAT_QUALCOMM_HALFRATE = 337,
		WAVE_FORMAT_TUBGSM = 341,
		WAVE_FORMAT_MSAUDIO1 = 352,
		WindowsMediaAudio = 353,
		WindowsMediaAudioProfessional = 354,
		WindowsMediaAudioLosseless = 355,
		WindowsMediaAudioSpdif = 356,
		WAVE_FORMAT_UNISYS_NAP_ADPCM = 368,
		WAVE_FORMAT_UNISYS_NAP_ULAW = 369,
		WAVE_FORMAT_UNISYS_NAP_ALAW = 370,
		WAVE_FORMAT_UNISYS_NAP_16K = 371,
		WAVE_FORMAT_CREATIVE_ADPCM = 512,
		WAVE_FORMAT_CREATIVE_FASTSPEECH8 = 514,
		WAVE_FORMAT_CREATIVE_FASTSPEECH10 = 515,
		WAVE_FORMAT_UHER_ADPCM = 528,
		WAVE_FORMAT_QUARTERDECK = 544,
		WAVE_FORMAT_ILINK_VC = 560,
		WAVE_FORMAT_RAW_SPORT = 576,
		WAVE_FORMAT_ESST_AC3 = 577,
		WAVE_FORMAT_IPI_HSX = 592,
		WAVE_FORMAT_IPI_RPELP = 593,
		WAVE_FORMAT_CS2 = 608,
		WAVE_FORMAT_SONY_SCX = 624,
		WAVE_FORMAT_FM_TOWNS_SND = 768,
		WAVE_FORMAT_BTV_DIGITAL = 1024,
		WAVE_FORMAT_QDESIGN_MUSIC = 1104,
		WAVE_FORMAT_VME_VMPCM = 1664,
		WAVE_FORMAT_TPC = 1665,
		WAVE_FORMAT_OLIGSM = 4096,
		WAVE_FORMAT_OLIADPCM = 4097,
		WAVE_FORMAT_OLICELP = 4098,
		WAVE_FORMAT_OLISBC = 4099,
		WAVE_FORMAT_OLIOPR = 4100,
		WAVE_FORMAT_LH_CODEC = 4352,
		WAVE_FORMAT_NORRIS = 5120,
		WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS = 5376,
		MPEG_ADTS_AAC = 5632,
		MPEG_RAW_AAC = 5633,
		MPEG_LOAS = 5634,
		NOKIA_MPEG_ADTS_AAC = 5640,
		NOKIA_MPEG_RAW_AAC = 5641,
		VODAFONE_MPEG_ADTS_AAC = 5642,
		VODAFONE_MPEG_RAW_AAC = 5643,
		MPEG_HEAAC = 5648,
		WAVE_FORMAT_DVM = 8192,
		Vorbis1 = 26447,
		Vorbis2 = 26448,
		Vorbis3 = 26449,
		Vorbis1P = 26479,
		Vorbis2P = 26480,
		Vorbis3P = 26481,
		Extensible = 65534,
		WAVE_FORMAT_DEVELOPMENT = ushort.MaxValue
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class WaveFormatExtensible : WaveFormat
	{
		private short wValidBitsPerSample;

		private int dwChannelMask;

		private Guid subFormat;

		public Guid SubFormat => subFormat;

		private WaveFormatExtensible()
		{
		}

		public WaveFormatExtensible(int rate, int bits, int channels)
			: base(rate, bits, channels)
		{
			waveFormatTag = WaveFormatEncoding.Extensible;
			extraSize = 22;
			wValidBitsPerSample = (short)bits;
			for (int i = 0; i < channels; i++)
			{
				dwChannelMask |= 1 << i;
			}
			if (bits == 32)
			{
				subFormat = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
			}
			else
			{
				subFormat = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
			}
		}

		public WaveFormat ToStandardWaveFormat()
		{
			if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT && bitsPerSample == 32)
			{
				return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
			}
			if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_PCM)
			{
				return new WaveFormat(sampleRate, bitsPerSample, channels);
			}
			return this;
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			writer.Write(wValidBitsPerSample);
			writer.Write(dwChannelMask);
			byte[] array = subFormat.ToByteArray();
			writer.Write(array, 0, array.Length);
		}

		public override string ToString()
		{
			return $"{base.ToString()} wBitsPerSample:{wValidBitsPerSample} dwChannelMask:{dwChannelMask} subFormat:{subFormat} extraSize:{extraSize}";
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class WaveFormatExtraData : WaveFormat
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
		private byte[] extraData = new byte[100];

		public byte[] ExtraData => extraData;

		internal WaveFormatExtraData()
		{
		}

		public WaveFormatExtraData(BinaryReader reader)
			: base(reader)
		{
			ReadExtraData(reader);
		}

		internal void ReadExtraData(BinaryReader reader)
		{
			if (extraSize > 0)
			{
				reader.Read(extraData, 0, extraSize);
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			if (extraSize > 0)
			{
				writer.Write(extraData, 0, extraSize);
			}
		}
	}
	public interface IWaveIn : IDisposable
	{
		WaveFormat WaveFormat { get; set; }

		event EventHandler<WaveInEventArgs> DataAvailable;

		event EventHandler<StoppedEventArgs> RecordingStopped;

		void StartRecording();

		void StopRecording();
	}
	public class WasapiLoopbackCapture : WasapiCapture
	{
		public override WaveFormat WaveFormat
		{
			get
			{
				return base.WaveFormat;
			}
			set
			{
				throw new InvalidOperationException("WaveFormat cannot be set for WASAPI Loopback Capture");
			}
		}

		public WasapiLoopbackCapture()
			: this(GetDefaultLoopbackCaptureDevice())
		{
		}

		public WasapiLoopbackCapture(MMDevice captureDevice)
			: base(captureDevice)
		{
		}

		public static MMDevice GetDefaultLoopbackCaptureDevice()
		{
			return new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
		}

		protected override AudioClientStreamFlags GetAudioClientStreamFlags()
		{
			return AudioClientStreamFlags.Loopback;
		}
	}
	public class WaveIn : IWaveIn, IDisposable
	{
		private IntPtr waveInHandle;

		private volatile bool recording;

		private WaveInBuffer[] buffers;

		private readonly WaveInterop.WaveCallback callback;

		private WaveCallbackInfo callbackInfo;

		private readonly SynchronizationContext syncContext;

		private int lastReturnedBufferIndex;

		public static int DeviceCount => WaveInterop.waveInGetNumDevs();

		public int BufferMilliseconds { get; set; }

		public int NumberOfBuffers { get; set; }

		public int DeviceNumber { get; set; }

		public WaveFormat WaveFormat { get; set; }

		public event EventHandler<WaveInEventArgs> DataAvailable;

		public event EventHandler<StoppedEventArgs> RecordingStopped;

		public WaveIn()
			: this(WaveCallbackInfo.NewWindow())
		{
		}

		public WaveIn(IntPtr windowHandle)
			: this(WaveCallbackInfo.ExistingWindow(windowHandle))
		{
		}

		public WaveIn(WaveCallbackInfo callbackInfo)
		{
			syncContext = SynchronizationContext.Current;
			if ((callbackInfo.Strategy == WaveCallbackStrategy.NewWindow || callbackInfo.Strategy == WaveCallbackStrategy.ExistingWindow) && syncContext == null)
			{
				throw new InvalidOperationException("Use WaveInEvent to record on a background thread");
			}
			DeviceNumber = 0;
			WaveFormat = new WaveFormat(8000, 16, 1);
			BufferMilliseconds = 100;
			NumberOfBuffers = 3;
			callback = Callback;
			this.callbackInfo = callbackInfo;
			callbackInfo.Connect(callback);
		}

		public static WaveInCapabilities GetCapabilities(int devNumber)
		{
			WaveInCapabilities waveInCaps = default(WaveInCapabilities);
			int waveInCapsSize = Marshal.SizeOf(waveInCaps);
			MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out waveInCaps, waveInCapsSize), "waveInGetDevCaps");
			return waveInCaps;
		}

		private void CreateBuffers()
		{
			int num = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
			if (num % WaveFormat.BlockAlign != 0)
			{
				num -= num % WaveFormat.BlockAlign;
			}
			buffers = new WaveInBuffer[NumberOfBuffers];
			for (int i = 0; i < buffers.Length; i++)
			{
				buffers[i] = new WaveInBuffer(waveInHandle, num);
			}
		}

		private void Callback(IntPtr waveInHandle, WaveInterop.WaveMessage message, IntPtr userData, WaveHeader waveHeader, IntPtr reserved)
		{
			if (message != WaveInterop.WaveMessage.WaveInData || !recording)
			{
				return;
			}
			WaveInBuffer waveInBuffer = (WaveInBuffer)((GCHandle)waveHeader.userData).Target;
			if (waveInBuffer != null)
			{
				lastReturnedBufferIndex = Array.IndexOf(buffers, waveInBuffer);
				RaiseDataAvailable(waveInBuffer);
				try
				{
					waveInBuffer.Reuse();
				}
				catch (Exception e)
				{
					recording = false;
					RaiseRecordingStopped(e);
				}
			}
		}

		private void RaiseDataAvailable(WaveInBuffer buffer)
		{
			this.DataAvailable?.Invoke(this, new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
		}

		private void RaiseRecordingStopped(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.RecordingStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}

		private void OpenWaveInDevice()
		{
			CloseWaveInDevice();
			MmException.Try(callbackInfo.WaveInOpen(out waveInHandle, DeviceNumber, WaveFormat, callback), "waveInOpen");
			CreateBuffers();
		}

		public void StartRecording()
		{
			if (recording)
			{
				throw new InvalidOperationException("Already recording");
			}
			OpenWaveInDevice();
			EnqueueBuffers();
			MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
			recording = true;
		}

		private void EnqueueBuffers()
		{
			WaveInBuffer[] array = buffers;
			foreach (WaveInBuffer waveInBuffer in array)
			{
				if (!waveInBuffer.InQueue)
				{
					waveInBuffer.Reuse();
				}
			}
		}

		public void StopRecording()
		{
			if (!recording)
			{
				return;
			}
			recording = false;
			MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");
			for (int i = 0; i < buffers.Length; i++)
			{
				int num = (i + lastReturnedBufferIndex + 1) % buffers.Length;
				WaveInBuffer waveInBuffer = buffers[num];
				if (waveInBuffer.Done)
				{
					RaiseDataAvailable(waveInBuffer);
				}
			}
			RaiseRecordingStopped(null);
		}

		public long GetPosition()
		{
			MmTime mmTime = default(MmTime);
			mmTime.wType = 4u;
			MmException.Try(WaveInterop.waveInGetPosition(waveInHandle, out mmTime, Marshal.SizeOf(mmTime)), "waveInGetPosition");
			if (mmTime.wType != 4)
			{
				throw new Exception($"waveInGetPosition: wType -> Expected {4}, Received {mmTime.wType}");
			}
			return mmTime.cb;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (recording)
				{
					StopRecording();
				}
				CloseWaveInDevice();
				if (callbackInfo != null)
				{
					callbackInfo.Disconnect();
					callbackInfo = null;
				}
			}
		}

		private void CloseWaveInDevice()
		{
			if (waveInHandle == IntPtr.Zero)
			{
				return;
			}
			WaveInterop.waveInReset(waveInHandle);
			if (buffers != null)
			{
				for (int i = 0; i < buffers.Length; i++)
				{
					buffers[i].Dispose();
				}
				buffers = null;
			}
			WaveInterop.waveInClose(waveInHandle);
			waveInHandle = IntPtr.Zero;
		}

		public MixerLine GetMixerLine()
		{
			if (waveInHandle != IntPtr.Zero)
			{
				return new MixerLine(waveInHandle, 0, MixerFlags.WaveInHandle);
			}
			return new MixerLine((IntPtr)DeviceNumber, 0, MixerFlags.WaveIn);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
	public class WaveInEvent : IWaveIn, IDisposable
	{
		private readonly AutoResetEvent callbackEvent;

		private readonly SynchronizationContext syncContext;

		private IntPtr waveInHandle;

		private volatile CaptureState captureState;

		private WaveInBuffer[] buffers;

		public static int DeviceCount => WaveInterop.waveInGetNumDevs();

		public int BufferMilliseconds { get; set; }

		public int NumberOfBuffers { get; set; }

		public int DeviceNumber { get; set; }

		public WaveFormat WaveFormat { get; set; }

		public event EventHandler<WaveInEventArgs> DataAvailable;

		public event EventHandler<StoppedEventArgs> RecordingStopped;

		public WaveInEvent()
		{
			callbackEvent = new AutoResetEvent(initialState: false);
			syncContext = SynchronizationContext.Current;
			DeviceNumber = 0;
			WaveFormat = new WaveFormat(8000, 16, 1);
			BufferMilliseconds = 100;
			NumberOfBuffers = 3;
			captureState = CaptureState.Stopped;
		}

		public static WaveInCapabilities GetCapabilities(int devNumber)
		{
			WaveInCapabilities waveInCaps = default(WaveInCapabilities);
			int waveInCapsSize = Marshal.SizeOf(waveInCaps);
			MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out waveInCaps, waveInCapsSize), "waveInGetDevCaps");
			return waveInCaps;
		}

		private void CreateBuffers()
		{
			int num = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
			if (num % WaveFormat.BlockAlign != 0)
			{
				num -= num % WaveFormat.BlockAlign;
			}
			buffers = new WaveInBuffer[NumberOfBuffers];
			for (int i = 0; i < buffers.Length; i++)
			{
				buffers[i] = new WaveInBuffer(waveInHandle, num);
			}
		}

		private void OpenWaveInDevice()
		{
			CloseWaveInDevice();
			MmException.Try(WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)DeviceNumber, WaveFormat, callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent), "waveInOpen");
			CreateBuffers();
		}

		public void StartRecording()
		{
			if (captureState != 0)
			{
				throw new InvalidOperationException("Already recording");
			}
			OpenWaveInDevice();
			MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
			captureState = CaptureState.Starting;
			ThreadPool.QueueUserWorkItem(delegate
			{
				RecordThread();
			}, null);
		}

		private void RecordThread()
		{
			Exception e = null;
			try
			{
				DoRecording();
			}
			catch (Exception ex)
			{
				e = ex;
			}
			finally
			{
				captureState = CaptureState.Stopped;
				RaiseRecordingStoppedEvent(e);
			}
		}

		private void DoRecording()
		{
			captureState = CaptureState.Capturing;
			WaveInBuffer[] array = buffers;
			foreach (WaveInBuffer waveInBuffer in array)
			{
				if (!waveInBuffer.InQueue)
				{
					waveInBuffer.Reuse();
				}
			}
			while (captureState == CaptureState.Capturing)
			{
				if (!callbackEvent.WaitOne())
				{
					continue;
				}
				array = buffers;
				foreach (WaveInBuffer waveInBuffer2 in array)
				{
					if (waveInBuffer2.Done)
					{
						if (waveInBuffer2.BytesRecorded > 0)
						{
							this.DataAvailable?.Invoke(this, new WaveInEventArgs(waveInBuffer2.Data, waveInBuffer2.BytesRecorded));
						}
						if (captureState == CaptureState.Capturing)
						{
							waveInBuffer2.Reuse();
						}
					}
				}
			}
		}

		private void RaiseRecordingStoppedEvent(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.RecordingStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}

		public void StopRecording()
		{
			if (captureState != 0)
			{
				captureState = CaptureState.Stopping;
				MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");
				MmException.Try(WaveInterop.waveInReset(waveInHandle), "waveInReset");
				callbackEvent.Set();
			}
		}

		public long GetPosition()
		{
			MmTime mmTime = default(MmTime);
			mmTime.wType = 4u;
			MmException.Try(WaveInterop.waveInGetPosition(waveInHandle, out mmTime, Marshal.SizeOf(mmTime)), "waveInGetPosition");
			if (mmTime.wType != 4)
			{
				throw new Exception($"waveInGetPosition: wType -> Expected {4}, Received {mmTime.wType}");
			}
			return mmTime.cb;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (captureState != 0)
				{
					StopRecording();
				}
				CloseWaveInDevice();
			}
		}

		private void CloseWaveInDevice()
		{
			WaveInterop.waveInReset(waveInHandle);
			if (buffers != null)
			{
				for (int i = 0; i < buffers.Length; i++)
				{
					buffers[i].Dispose();
				}
				buffers = null;
			}
			WaveInterop.waveInClose(waveInHandle);
			waveInHandle = IntPtr.Zero;
		}

		public MixerLine GetMixerLine()
		{
			if (waveInHandle != IntPtr.Zero)
			{
				return new MixerLine(waveInHandle, 0, MixerFlags.WaveInHandle);
			}
			return new MixerLine((IntPtr)DeviceNumber, 0, MixerFlags.WaveIn);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
	public class AiffFileWriter : Stream
	{
		private Stream outStream;

		private BinaryWriter writer;

		private long dataSizePos;

		private long commSampleCountPos;

		private int dataChunkSize = 8;

		private WaveFormat format;

		private string filename;

		private byte[] value24 = new byte[3];

		public string Filename => filename;

		public override long Length => dataChunkSize;

		public WaveFormat WaveFormat => format;

		public override bool CanRead => false;

		public override bool CanWrite => true;

		public override bool CanSeek => false;

		public override long Position
		{
			get
			{
				return dataChunkSize;
			}
			set
			{
				throw new InvalidOperationException("Repositioning an AiffFileWriter is not supported");
			}
		}

		public static void CreateAiffFile(string filename, WaveStream sourceProvider)
		{
			using AiffFileWriter aiffFileWriter = new AiffFileWriter(filename, sourceProvider.WaveFormat);
			byte[] array = new byte[16384];
			while (sourceProvider.Position < sourceProvider.Length)
			{
				int count = Math.Min((int)(sourceProvider.Length - sourceProvider.Position), array.Length);
				int num = sourceProvider.Read(array, 0, count);
				if (num == 0)
				{
					break;
				}
				aiffFileWriter.Write(array, 0, num);
			}
		}

		public AiffFileWriter(Stream outStream, WaveFormat format)
		{
			this.outStream = outStream;
			this.format = format;
			writer = new BinaryWriter(outStream, Encoding.UTF8);
			writer.Write(Encoding.UTF8.GetBytes("FORM"));
			writer.Write(0);
			writer.Write(Encoding.UTF8.GetBytes("AIFF"));
			CreateCommChunk();
			WriteSsndChunkHeader();
		}

		public AiffFileWriter(string filename, WaveFormat format)
			: this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format)
		{
			this.filename = filename;
		}

		private void WriteSsndChunkHeader()
		{
			writer.Write(Encoding.UTF8.GetBytes("SSND"));
			dataSizePos = outStream.Position;
			writer.Write(0);
			writer.Write(0);
			writer.Write(SwapEndian(format.BlockAlign));
		}

		private byte[] SwapEndian(short n)
		{
			return new byte[2]
			{
				(byte)(n >> 8),
				(byte)((uint)n & 0xFFu)
			};
		}

		private byte[] SwapEndian(int n)
		{
			return new byte[4]
			{
				(byte)((uint)(n >> 24) & 0xFFu),
				(byte)((uint)(n >> 16) & 0xFFu),
				(byte)((uint)(n >> 8) & 0xFFu),
				(byte)((uint)n & 0xFFu)
			};
		}

		private void CreateCommChunk()
		{
			writer.Write(Encoding.UTF8.GetBytes("COMM"));
			writer.Write(SwapEndian(18));
			writer.Write(SwapEndian((short)format.Channels));
			commSampleCountPos = outStream.Position;
			writer.Write(0);
			writer.Write(SwapEndian((short)format.BitsPerSample));
			writer.Write(IEEE.ConvertToIeeeExtended(format.SampleRate));
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("Cannot read from an AiffFileWriter");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException("Cannot seek within an AiffFileWriter");
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException("Cannot set length of an AiffFileWriter");
		}

		public override void Write(byte[] data, int offset, int count)
		{
			byte[] array = new byte[data.Length];
			int num = format.BitsPerSample / 8;
			for (int i = 0; i < data.Length; i++)
			{
				int num2 = (int)Math.Floor((double)i / (double)num) * num + (num - i % num - 1);
				array[i] = data[num2];
			}
			outStream.Write(array, offset, count);
			dataChunkSize += count;
		}

		public void WriteSample(float sample)
		{
			if (WaveFormat.BitsPerSample == 16)
			{
				writer.Write(SwapEndian((short)(32767f * sample)));
				dataChunkSize += 2;
			}
			else if (WaveFormat.BitsPerSample == 24)
			{
				byte[] bytes = BitConverter.GetBytes((int)(2.1474836E+09f * sample));
				value24[2] = bytes[1];
				value24[1] = bytes[2];
				value24[0] = bytes[3];
				writer.Write(value24);
				dataChunkSize += 3;
			}
			else
			{
				if (WaveFormat.BitsPerSample != 32 || WaveFormat.Encoding != WaveFormatEncoding.Extensible)
				{
					throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
				}
				writer.Write(SwapEndian(65535 * (int)sample));
				dataChunkSize += 4;
			}
		}

		public void WriteSamples(float[] samples, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				WriteSample(samples[offset + i]);
			}
		}

		public void WriteSamples(short[] samples, int offset, int count)
		{
			if (WaveFormat.BitsPerSample == 16)
			{
				for (int i = 0; i < count; i++)
				{
					writer.Write(SwapEndian(samples[i + offset]));
				}
				dataChunkSize += count * 2;
			}
			else if (WaveFormat.BitsPerSample == 24)
			{
				for (int j = 0; j < count; j++)
				{
					byte[] bytes = BitConverter.GetBytes(65535 * samples[j + offset]);
					value24[2] = bytes[1];
					value24[1] = bytes[2];
					value24[0] = bytes[3];
					writer.Write(value24);
				}
				dataChunkSize += count * 3;
			}
			else
			{
				if (WaveFormat.BitsPerSample != 32 || WaveFormat.Encoding != WaveFormatEncoding.Extensible)
				{
					throw new InvalidOperationException("Only 16, 24 or 32 bit PCM audio data supported");
				}
				for (int k = 0; k < count; k++)
				{
					writer.Write(SwapEndian(65535 * samples[k + offset]));
				}
				dataChunkSize += count * 4;
			}
		}

		public override void Flush()
		{
			writer.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && outStream != null)
			{
				try
				{
					UpdateHeader(writer);
				}
				finally
				{
					outStream.Dispose();
					outStream = null;
				}
			}
		}

		protected virtual void UpdateHeader(BinaryWriter writer)
		{
			Flush();
			writer.Seek(4, SeekOrigin.Begin);
			writer.Write(SwapEndian((int)(outStream.Length - 8)));
			UpdateCommChunk(writer);
			UpdateSsndChunk(writer);
		}

		private void UpdateCommChunk(BinaryWriter writer)
		{
			writer.Seek((int)commSampleCountPos, SeekOrigin.Begin);
			writer.Write(SwapEndian(dataChunkSize * 8 / format.BitsPerSample / format.Channels));
		}

		private void UpdateSsndChunk(BinaryWriter writer)
		{
			writer.Seek((int)dataSizePos, SeekOrigin.Begin);
			writer.Write(SwapEndian(dataChunkSize));
		}

		~AiffFileWriter()
		{
			Dispose(disposing: false);
		}
	}
	public class AsioAudioAvailableEventArgs : EventArgs
	{
		public IntPtr[] InputBuffers { get; private set; }

		public IntPtr[] OutputBuffers { get; private set; }

		public bool WrittenToOutputBuffers { get; set; }

		public int SamplesPerBuffer { get; private set; }

		public AsioSampleType AsioSampleType { get; private set; }

		public AsioAudioAvailableEventArgs(IntPtr[] inputBuffers, IntPtr[] outputBuffers, int samplesPerBuffer, AsioSampleType asioSampleType)
		{
			InputBuffers = inputBuffers;
			OutputBuffers = outputBuffers;
			SamplesPerBuffer = samplesPerBuffer;
			AsioSampleType = asioSampleType;
		}

		public unsafe int GetAsInterleavedSamples(float[] samples)
		{
			int num = InputBuffers.Length;
			if (samples.Length < SamplesPerBuffer * num)
			{
				throw new ArgumentException("Buffer not big enough");
			}
			int num2 = 0;
			if (AsioSampleType == AsioSampleType.Int32LSB)
			{
				for (int i = 0; i < SamplesPerBuffer; i++)
				{
					for (int j = 0; j < num; j++)
					{
						samples[num2++] = (float)(*(int*)((byte*)(void*)InputBuffers[j] + (nint)i * (nint)4)) / 2.1474836E+09f;
					}
				}
			}
			else if (AsioSampleType == AsioSampleType.Int16LSB)
			{
				for (int k = 0; k < SamplesPerBuffer; k++)
				{
					for (int l = 0; l < num; l++)
					{
						samples[num2++] = (float)(*(short*)((byte*)(void*)InputBuffers[l] + (nint)k * (nint)2)) / 32767f;
					}
				}
			}
			else if (AsioSampleType == AsioSampleType.Int24LSB)
			{
				for (int m = 0; m < SamplesPerBuffer; m++)
				{
					for (int n = 0; n < num; n++)
					{
						byte* ptr = (byte*)(void*)InputBuffers[n] + m * 3;
						int num3 = *ptr | (ptr[1] << 8) | ((sbyte)ptr[2] << 16);
						samples[num2++] = (float)num3 / 8388608f;
					}
				}
			}
			else
			{
				if (AsioSampleType != AsioSampleType.Float32LSB)
				{
					throw new NotImplementedException($"ASIO Sample Type {AsioSampleType} not supported");
				}
				for (int num4 = 0; num4 < SamplesPerBuffer; num4++)
				{
					for (int num5 = 0; num5 < num; num5++)
					{
						samples[num2++] = *(float*)((byte*)(void*)InputBuffers[num5] + (nint)num4 * (nint)4);
					}
				}
			}
			return SamplesPerBuffer * num;
		}

		[Obsolete("Better performance if you use the overload that takes an array, and reuse the same one")]
		public float[] GetAsInterleavedSamples()
		{
			int num = InputBuffers.Length;
			float[] array = new float[SamplesPerBuffer * num];
			GetAsInterleavedSamples(array);
			return array;
		}
	}
	public class AsioOut : IWavePlayer, IDisposable
	{
		private AsioDriverExt driver;

		private IWaveProvider sourceStream;

		private PlaybackState playbackState;

		private int nbSamples;

		private byte[] waveBuffer;

		private AsioSampleConvertor.SampleConvertor convertor;

		private string driverName;

		private readonly SynchronizationContext syncContext;

		private bool isInitialized;

		public int PlaybackLatency
		{
			get
			{
				driver.Driver.GetLatencies(out var _, out var outputLatency);
				return outputLatency;
			}
		}

		public bool AutoStop { get; set; }

		public bool HasReachedEnd { get; private set; }

		public PlaybackState PlaybackState => playbackState;

		public string DriverName => driverName;

		public int NumberOfOutputChannels { get; private set; }

		public int NumberOfInputChannels { get; private set; }

		public int DriverInputChannelCount => driver.Capabilities.NbInputChannels;

		public int DriverOutputChannelCount => driver.Capabilities.NbOutputChannels;

		public int FramesPerBuffer
		{
			get
			{
				if (!isInitialized)
				{
					throw new Exception("Not initialized yet. Call this after calling Init");
				}
				return nbSamples;
			}
		}

		public int ChannelOffset { get; set; }

		public int InputChannelOffset { get; set; }

		[Obsolete("this function will be removed in a future NAudio as ASIO does not support setting the volume on the device")]
		public float Volume
		{
			get
			{
				return 1f;
			}
			set
			{
				if (value != 1f)
				{
					throw new InvalidOperationException("AsioOut does not support setting the device volume");
				}
			}
		}

		public event EventHandler<StoppedEventArgs> PlaybackStopped;

		public event EventHandler<AsioAudioAvailableEventArgs> AudioAvailable;

		public AsioOut()
			: this(0)
		{
		}

		public AsioOut(string driverName)
		{
			syncContext = SynchronizationContext.Current;
			InitFromName(driverName);
		}

		public AsioOut(int driverIndex)
		{
			syncContext = SynchronizationContext.Current;
			string[] driverNames = GetDriverNames();
			if (driverNames.Length == 0)
			{
				throw new ArgumentException("There is no ASIO Driver installed on your system");
			}
			if (driverIndex < 0 || driverIndex > driverNames.Length)
			{
				throw new ArgumentException($"Invalid device number. Must be in the range [0,{driverNames.Length}]");
			}
			InitFromName(driverNames[driverIndex]);
		}

		~AsioOut()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (driver != null)
			{
				if (playbackState != 0)
				{
					driver.Stop();
				}
				driver.ReleaseDriver();
				driver = null;
			}
		}

		public static string[] GetDriverNames()
		{
			return AsioDriver.GetAsioDriverNames();
		}

		public static bool isSupported()
		{
			return GetDriverNames().Length != 0;
		}

		private void InitFromName(string driverName)
		{
			this.driverName = driverName;
			AsioDriver asioDriverByName = AsioDriver.GetAsioDriverByName(driverName);
			driver = new AsioDriverExt(asioDriverByName);
			ChannelOffset = 0;
		}

		public void ShowControlPanel()
		{
			driver.ShowControlPanel();
		}

		public void Play()
		{
			if (playbackState != PlaybackState.Playing)
			{
				playbackState = PlaybackState.Playing;
				HasReachedEnd = false;
				driver.Start();
			}
		}

		public void Stop()
		{
			playbackState = PlaybackState.Stopped;
			driver.Stop();
			HasReachedEnd = false;
			RaisePlaybackStopped(null);
		}

		public void Pause()
		{
			playbackState = PlaybackState.Paused;
			driver.Stop();
		}

		public void Init(IWaveProvider waveProvider)
		{
			InitRecordAndPlayback(waveProvider, 0, -1);
		}

		public void InitRecordAndPlayback(IWaveProvider waveProvider, int recordChannels, int recordOnlySampleRate)
		{
			if (isInitialized)
			{
				throw new InvalidOperationException("Already initialised this instance of AsioOut - dispose and create a new one");
			}
			isInitialized = true;
			int num = waveProvider?.WaveFormat.SampleRate ?? recordOnlySampleRate;
			if (waveProvider != null)
			{
				sourceStream = waveProvider;
				NumberOfOutputChannels = waveProvider.WaveFormat.Channels;
				convertor = AsioSampleConvertor.SelectSampleConvertor(waveProvider.WaveFormat, driver.Capabilities.OutputChannelInfos[0].type);
			}
			else
			{
				NumberOfOutputChannels = 0;
			}
			if (!driver.IsSampleRateSupported(num))
			{
				throw new ArgumentException("SampleRate is not supported");
			}
			if (driver.Capabilities.SampleRate != (double)num)
			{
				driver.SetSampleRate(num);
			}
			driver.FillBufferCallback = driver_BufferUpdate;
			NumberOfInputChannels = recordChannels;
			nbSamples = driver.CreateBuffers(NumberOfOutputChannels, NumberOfInputChannels, useMaxBufferSize: false);
			driver.SetChannelOffset(ChannelOffset, InputChannelOffset);
			if (waveProvider != null)
			{
				waveBuffer = new byte[nbSamples * NumberOfOutputChannels * waveProvider.WaveFormat.BitsPerSample / 8];
			}
		}

		private unsafe void driver_BufferUpdate(IntPtr[] inputChannels, IntPtr[] outputChannels)
		{
			if (NumberOfInputChannels > 0)
			{
				EventHandler<AsioAudioAvailableEventArgs> audioAvailable = this.AudioAvailable;
				if (audioAvailable != null)
				{
					AsioAudioAvailableEventArgs asioAudioAvailableEventArgs = new AsioAudioAvailableEventArgs(inputChannels, outputChannels, nbSamples, driver.Capabilities.InputChannelInfos[0].type);
					audioAvailable(this, asioAudioAvailableEventArgs);
					if (asioAudioAvailableEventArgs.WrittenToOutputBuffers)
					{
						return;
					}
				}
			}
			if (NumberOfOutputChannels <= 0)
			{
				return;
			}
			int num = sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
			if (num < waveBuffer.Length)
			{
				Array.Clear(waveBuffer, num, waveBuffer.Length - num);
			}
			fixed (byte* ptr = &waveBuffer[0])
			{
				void* value = ptr;
				convertor(new IntPtr(value), outputChannels, NumberOfOutputChannels, nbSamples);
			}
			if (num == 0)
			{
				if (AutoStop)
				{
					Stop();
				}
				HasReachedEnd = true;
			}
		}

		private void RaisePlaybackStopped(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.PlaybackStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}

		public string AsioInputChannelName(int channel)
		{
			if (channel <= DriverInputChannelCount)
			{
				return driver.Capabilities.InputChannelInfos[channel].name;
			}
			return "";
		}

		public string AsioOutputChannelName(int channel)
		{
			if (channel <= DriverOutputChannelCount)
			{
				return driver.Capabilities.OutputChannelInfos[channel].name;
			}
			return "";
		}
	}
	public class BextChunkInfo
	{
		public string Description { get; set; }

		public string Originator { get; set; }

		public string OriginatorReference { get; set; }

		public DateTime OriginationDateTime { get; set; }

		public string OriginationDate => OriginationDateTime.ToString("yyyy-MM-dd");

		public string OriginationTime => OriginationDateTime.ToString("HH:mm:ss");

		public long TimeReference { get; set; }

		public ushort Version => 1;

		public string UniqueMaterialIdentifier { get; set; }

		public byte[] Reserved { get; }

		public string CodingHistory { get; set; }

		public BextChunkInfo()
		{
			Reserved = new byte[190];
		}
	}
	public class BwfWriter : IDisposable
	{
		private readonly WaveFormat format;

		private readonly BinaryWriter writer;

		private readonly long dataChunkSizePosition;

		private long dataLength;

		private bool isDisposed;

		public BwfWriter(string filename, WaveFormat format, BextChunkInfo bextChunkInfo)
		{
			this.format = format;
			writer = new BinaryWriter(File.OpenWrite(filename));
			writer.Write(Encoding.UTF8.GetBytes("RIFF"));
			writer.Write(0);
			writer.Write(Encoding.UTF8.GetBytes("WAVE"));
			writer.Write(Encoding.UTF8.GetBytes("JUNK"));
			writer.Write(28);
			writer.Write(0L);
			writer.Write(0L);
			writer.Write(0L);
			writer.Write(0);
			writer.Write(Encoding.UTF8.GetBytes("bext"));
			byte[] bytes = Encoding.ASCII.GetBytes(bextChunkInfo.CodingHistory ?? "");
			int num = 602 + bytes.Length;
			if (num % 2 != 0)
			{
				num++;
			}
			writer.Write(num);
			_ = writer.BaseStream.Position;
			writer.Write(GetAsBytes(bextChunkInfo.Description, 256));
			writer.Write(GetAsBytes(bextChunkInfo.Originator, 32));
			writer.Write(GetAsBytes(bextChunkInfo.OriginatorReference, 32));
			writer.Write(GetAsBytes(bextChunkInfo.OriginationDate, 10));
			writer.Write(GetAsBytes(bextChunkInfo.OriginationTime, 8));
			writer.Write(bextChunkInfo.TimeReference);
			writer.Write(bextChunkInfo.Version);
			writer.Write(GetAsBytes(bextChunkInfo.UniqueMaterialIdentifier, 64));
			writer.Write(bextChunkInfo.Reserved);
			writer.Write(bytes);
			if (bytes.Length % 2 != 0)
			{
				writer.Write((byte)0);
			}
			writer.Write(Encoding.UTF8.GetBytes("fmt "));
			format.Serialize(writer);
			writer.Write(Encoding.UTF8.GetBytes("data"));
			dataChunkSizePosition = writer.BaseStream.Position;
			writer.Write(-1);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("This BWF Writer already disposed");
			}
			writer.Write(buffer, offset, count);
			dataLength += count;
		}

		public void Flush()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("This BWF Writer already disposed");
			}
			writer.Flush();
			FixUpChunkSizes(restorePosition: true);
		}

		private void FixUpChunkSizes(bool restorePosition)
		{
			long position = writer.BaseStream.Position;
			bool num = dataLength > int.MaxValue;
			long num2 = writer.BaseStream.Length - 8;
			if (num)
			{
				int num3 = format.BitsPerSample / 8 * format.Channels;
				writer.BaseStream.Position = 0L;
				writer.Write(Encoding.UTF8.GetBytes("RF64"));
				writer.Write(-1);
				writer.BaseStream.Position += 4L;
				writer.Write(Encoding.UTF8.GetBytes("ds64"));
				writer.BaseStream.Position += 4L;
				writer.Write(num2);
				writer.Write(dataLength);
				writer.Write(dataLength / num3);
			}
			else
			{
				writer.BaseStream.Position = 4L;
				writer.Write((uint)num2);
				writer.BaseStream.Position = dataChunkSizePosition;
				writer.Write((uint)dataLength);
			}
			if (restorePosition)
			{
				writer.BaseStream.Position = position;
			}
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				FixUpChunkSizes(restorePosition: false);
				writer.Close();
				isDisposed = true;
			}
		}

		private static byte[] GetAsBytes(string message, int byteSize)
		{
			byte[] array = new byte[byteSize];
			byte[] bytes = Encoding.ASCII.GetBytes(message ?? "");
			Array.Copy(bytes, array, Math.Min(bytes.Length, byteSize));
			return array;
		}
	}
	public class CueWaveFileWriter : WaveFileWriter
	{
		private CueList cues;

		public CueWaveFileWriter(string fileName, WaveFormat waveFormat)
			: base(fileName, waveFormat)
		{
		}

		public void AddCue(int position, string label)
		{
			if (cues == null)
			{
				cues = new CueList();
			}
			cues.Add(new Cue(position, label));
		}

		private void WriteCues(BinaryWriter w)
		{
			if (cues != null)
			{
				int count = cues.GetRiffChunks().Length;
				w.Seek(0, SeekOrigin.End);
				if (w.BaseStream.Length % 2 == 1)
				{
					w.Write((byte)0);
				}
				w.Write(cues.GetRiffChunks(), 0, count);
				w.Seek(4, SeekOrigin.Begin);
				w.Write((int)(w.BaseStream.Length - 8));
			}
		}

		protected override void UpdateHeader(BinaryWriter writer)
		{
			base.UpdateHeader(writer);
			WriteCues(writer);
		}
	}
	public class DirectSoundOut : IWavePlayer, IDisposable
	{
		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		internal class BufferDescription
		{
			public int dwSize;

			[MarshalAs(UnmanagedType.U4)]
			public DirectSoundBufferCaps dwFlags;

			public uint dwBufferBytes;

			public int dwReserved;

			public IntPtr lpwfxFormat;

			public Guid guidAlgo;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		internal class BufferCaps
		{
			public int dwSize;

			public int dwFlags;

			public int dwBufferBytes;

			public int dwUnlockTransferRate;

			public int dwPlayCpuOverhead;
		}

		internal enum DirectSoundCooperativeLevel : uint
		{
			DSSCL_NORMAL = 1u,
			DSSCL_PRIORITY,
			DSSCL_EXCLUSIVE,
			DSSCL_WRITEPRIMARY
		}

		[Flags]
		internal enum DirectSoundPlayFlags : uint
		{
			DSBPLAY_LOOPING = 1u,
			DSBPLAY_LOCHARDWARE = 2u,
			DSBPLAY_LOCSOFTWARE = 4u,
			DSBPLAY_TERMINATEBY_TIME = 8u,
			DSBPLAY_TERMINATEBY_DISTANCE = 0x10u,
			DSBPLAY_TERMINATEBY_PRIORITY = 0x20u
		}

		internal enum DirectSoundBufferLockFlag : uint
		{
			None,
			FromWriteCursor,
			EntireBuffer
		}

		[Flags]
		internal enum DirectSoundBufferStatus : uint
		{
			DSBSTATUS_PLAYING = 1u,
			DSBSTATUS_BUFFERLOST = 2u,
			DSBSTATUS_LOOPING = 4u,
			DSBSTATUS_LOCHARDWARE = 8u,
			DSBSTATUS_LOCSOFTWARE = 0x10u,
			DSBSTATUS_TERMINATED = 0x20u
		}

		[Flags]
		internal enum DirectSoundBufferCaps : uint
		{
			DSBCAPS_PRIMARYBUFFER = 1u,
			DSBCAPS_STATIC = 2u,
			DSBCAPS_LOCHARDWARE = 4u,
			DSBCAPS_LOCSOFTWARE = 8u,
			DSBCAPS_CTRL3D = 0x10u,
			DSBCAPS_CTRLFREQUENCY = 0x20u,
			DSBCAPS_CTRLPAN = 0x40u,
			DSBCAPS_CTRLVOLUME = 0x80u,
			DSBCAPS_CTRLPOSITIONNOTIFY = 0x100u,
			DSBCAPS_CTRLFX = 0x200u,
			DSBCAPS_STICKYFOCUS = 0x4000u,
			DSBCAPS_GLOBALFOCUS = 0x8000u,
			DSBCAPS_GETCURRENTPOSITION2 = 0x10000u,
			DSBCAPS_MUTE3DATMAXDISTANCE = 0x20000u,
			DSBCAPS_LOCDEFER = 0x40000u
		}

		internal struct DirectSoundBufferPositionNotify
		{
			public uint dwOffset;

			public IntPtr hEventNotify;
		}

		[ComImport]
		[Guid("279AFA83-4981-11CE-A521-0020AF0BE560")]
		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IDirectSound
		{
			void CreateSoundBuffer([In] BufferDescription desc, [MarshalAs(UnmanagedType.Interface)] out object dsDSoundBuffer, IntPtr pUnkOuter);

			void GetCaps(IntPtr caps);

			void DuplicateSoundBuffer([In][MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer bufferOriginal, [In][MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer bufferDuplicate);

			void SetCooperativeLevel(IntPtr HWND, [In][MarshalAs(UnmanagedType.U4)] DirectSoundCooperativeLevel dwLevel);

			void Compact();

			void GetSpeakerConfig(IntPtr pdwSpeakerConfig);

			void SetSpeakerConfig(uint pdwSpeakerConfig);

			void Initialize([In][MarshalAs(UnmanagedType.LPStruct)] Guid guid);
		}

		[ComImport]
		[Guid("279AFA85-4981-11CE-A521-0020AF0BE560")]
		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IDirectSoundBuffer
		{
			void GetCaps([MarshalAs(UnmanagedType.LPStruct)] BufferCaps pBufferCaps);

			void GetCurrentPosition(out uint currentPlayCursor, out uint currentWriteCursor);

			void GetFormat();

			[return: MarshalAs(UnmanagedType.I4)]
			int GetVolume();

			void GetPan(out uint pan);

			[return: MarshalAs(UnmanagedType.I4)]
			int GetFrequency();

			[return: MarshalAs(UnmanagedType.U4)]
			DirectSoundBufferStatus GetStatus();

			void Initialize([In][MarshalAs(UnmanagedType.Interface)] IDirectSound directSound, [In] BufferDescription desc);

			void Lock(int dwOffset, uint dwBytes, out IntPtr audioPtr1, out int audioBytes1, out IntPtr audioPtr2, out int audioBytes2, [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlag dwFlags);

			void Play(uint dwReserved1, uint dwPriority, [In][MarshalAs(UnmanagedType.U4)] DirectSoundPlayFlags dwFlags);

			void SetCurrentPosition(uint dwNewPosition);

			void SetFormat([In] WaveFormat pcfxFormat);

			void SetVolume(int volume);

			void SetPan(uint pan);

			void SetFrequency(uint frequency);

			void Stop();

			void Unlock(IntPtr pvAudioPtr1, int dwAudioBytes1, IntPtr pvAudioPtr2, int dwAudioBytes2);

			void Restore();
		}

		[ComImport]
		[Guid("b0210783-89cd-11d0-af08-00a0c925cd16")]
		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IDirectSoundNotify
		{
			void SetNotificationPositions(uint dwPositionNotifies, [In][MarshalAs(UnmanagedType.LPArray)] DirectSoundBufferPositionNotify[] pcPositionNotifies);
		}

		private delegate bool DSEnumCallback(IntPtr lpGuid, IntPtr lpcstrDescription, IntPtr lpcstrModule, IntPtr lpContext);

		private PlaybackState playbackState;

		private WaveFormat waveFormat;

		private int samplesTotalSize;

		private int samplesFrameSize;

		private int nextSamplesWriteIndex;

		private int desiredLatency;

		private Guid device;

		private byte[] samples;

		private IWaveProvider waveStream;

		private IDirectSound directSound;

		private IDirectSoundBuffer primarySoundBuffer;

		private IDirectSoundBuffer secondaryBuffer;

		private EventWaitHandle frameEventWaitHandle1;

		private EventWaitHandle frameEventWaitHandle2;

		private EventWaitHandle endEventWaitHandle;

		private Thread notifyThread;

		private SynchronizationContext syncContext;

		private long bytesPlayed;

		private object m_LockObject = new object();

		private static List<DirectSoundDeviceInfo> devices;

		public static readonly Guid DSDEVID_DefaultPlayback = new Guid("DEF00000-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static readonly Guid DSDEVID_DefaultCapture = new Guid("DEF00001-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static readonly Guid DSDEVID_DefaultVoicePlayback = new Guid("DEF00002-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static readonly Guid DSDEVID_DefaultVoiceCapture = new Guid("DEF00003-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static IEnumerable<DirectSoundDeviceInfo> Devices
		{
			get
			{
				devices = new List<DirectSoundDeviceInfo>();
				DirectSoundEnumerate(EnumCallback, IntPtr.Zero);
				return devices;
			}
		}

		public TimeSpan PlaybackPosition => TimeSpan.FromMilliseconds((double)(GetPosition() / (waveFormat.Channels * waveFormat.BitsPerSample / 8)) * 1000.0 / (double)waveFormat.SampleRate);

		public PlaybackState PlaybackState => playbackState;

		public float Volume
		{
			get
			{
				return 1f;
			}
			set
			{
				if (value != 1f)
				{
					throw new InvalidOperationException("Setting volume not supported on DirectSoundOut, adjust the volume on your WaveProvider instead");
				}
			}
		}

		public event EventHandler<StoppedEventArgs> PlaybackStopped;

		private static bool EnumCallback(IntPtr lpGuid, IntPtr lpcstrDescription, IntPtr lpcstrModule, IntPtr lpContext)
		{
			DirectSoundDeviceInfo directSoundDeviceInfo = new DirectSoundDeviceInfo();
			if (lpGuid == IntPtr.Zero)
			{
				directSoundDeviceInfo.Guid = Guid.Empty;
			}
			else
			{
				byte[] array = new byte[16];
				Marshal.Copy(lpGuid, array, 0, 16);
				directSoundDeviceInfo.Guid = new Guid(array);
			}
			directSoundDeviceInfo.Description = Marshal.PtrToStringAnsi(lpcstrDescription);
			directSoundDeviceInfo.ModuleName = Marshal.PtrToStringAnsi(lpcstrModule);
			devices.Add(directSoundDeviceInfo);
			return true;
		}

		public DirectSoundOut()
			: this(DSDEVID_DefaultPlayback)
		{
		}

		public DirectSoundOut(Guid device)
			: this(device, 40)
		{
		}

		public DirectSoundOut(int latency)
			: this(DSDEVID_DefaultPlayback, latency)
		{
		}

		public DirectSoundOut(Guid device, int latency)
		{
			if (device == Guid.Empty)
			{
				device = DSDEVID_DefaultPlayback;
			}
			this.device = device;
			desiredLatency = latency;
			syncContext = SynchronizationContext.Current;
		}

		~DirectSoundOut()
		{
			Dispose();
		}

		public void Play()
		{
			if (playbackState == PlaybackState.Stopped)
			{
				notifyThread = new Thread(PlaybackThreadFunc);
				notifyThread.Priority = ThreadPriority.Normal;
				notifyThread.IsBackground = true;
				notifyThread.Start();
			}
			lock (m_LockObject)
			{
				playbackState = PlaybackState.Playing;
			}
		}

		public void Stop()
		{
			if (Monitor.TryEnter(m_LockObject, 50))
			{
				playbackState = PlaybackState.Stopped;
				Monitor.Exit(m_LockObject);
			}
			else if (notifyThread != null)
			{
				notifyThread.Abort();
				notifyThread = null;
			}
		}

		public void Pause()
		{
			lock (m_LockObject)
			{
				playbackState = PlaybackState.Paused;
			}
		}

		public long GetPosition()
		{
			if (playbackState != 0)
			{
				IDirectSoundBuffer directSoundBuffer = secondaryBuffer;
				if (directSoundBuffer != null)
				{
					directSoundBuffer.GetCurrentPosition(out var currentPlayCursor, out var _);
					return currentPlayCursor + bytesPlayed;
				}
			}
			return 0L;
		}

		public void Init(IWaveProvider waveProvider)
		{
			waveStream = waveProvider;
			waveFormat = waveProvider.WaveFormat;
		}

		private void InitializeDirectSound()
		{
			lock (m_LockObject)
			{
				directSound = null;
				DirectSoundCreate(ref device, out directSound, IntPtr.Zero);
				if (directSound != null)
				{
					directSound.SetCooperativeLevel(GetDesktopWindow(), DirectSoundCooperativeLevel.DSSCL_PRIORITY);
					BufferDescription bufferDescription = new BufferDescription();
					bufferDescription.dwSize = Marshal.SizeOf(bufferDescription);
					bufferDescription.dwBufferBytes = 0u;
					bufferDescription.dwFlags = DirectSoundBufferCaps.DSBCAPS_PRIMARYBUFFER;
					bufferDescription.dwReserved = 0;
					bufferDescription.lpwfxFormat = IntPtr.Zero;
					bufferDescription.guidAlgo = Guid.Empty;
					directSound.CreateSoundBuffer(bufferDescription, out var dsDSoundBuffer, IntPtr.Zero);
					primarySoundBuffer = (IDirectSoundBuffer)dsDSoundBuffer;
					primarySoundBuffer.Play(0u, 0u, DirectSoundPlayFlags.DSBPLAY_LOOPING);
					samplesFrameSize = MsToBytes(desiredLatency);
					BufferDescription bufferDescription2 = new BufferDescription();
					bufferDescription2.dwSize = Marshal.SizeOf(bufferDescription2);
					bufferDescription2.dwBufferBytes = (uint)(samplesFrameSize * 2);
					bufferDescription2.dwFlags = DirectSoundBufferCaps.DSBCAPS_CTRLVOLUME | DirectSoundBufferCaps.DSBCAPS_CTRLPOSITIONNOTIFY | DirectSoundBufferCaps.DSBCAPS_STICKYFOCUS | DirectSoundBufferCaps.DSBCAPS_GLOBALFOCUS | DirectSoundBufferCaps.DSBCAPS_GETCURRENTPOSITION2;
					bufferDescription2.dwReserved = 0;
					GCHandle gCHandle = GCHandle.Alloc(waveFormat, GCHandleType.Pinned);
					bufferDescription2.lpwfxFormat = gCHandle.AddrOfPinnedObject();
					bufferDescription2.guidAlgo = Guid.Empty;
					directSound.CreateSoundBuffer(bufferDescription2, out dsDSoundBuffer, IntPtr.Zero);
					secondaryBuffer = (IDirectSoundBuffer)dsDSoundBuffer;
					gCHandle.Free();
					BufferCaps bufferCaps = new BufferCaps();
					bufferCaps.dwSize = Marshal.SizeOf(bufferCaps);
					secondaryBuffer.GetCaps(bufferCaps);
					nextSamplesWriteIndex = 0;
					samplesTotalSize = bufferCaps.dwBufferBytes;
					samples = new byte[samplesTotalSize];
					IDirectSoundNotify obj = (IDirectSoundNotify)dsDSoundBuffer;
					frameEventWaitHandle1 = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
					frameEventWaitHandle2 = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
					endEventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
					DirectSoundBufferPositionNotify[] array = new DirectSoundBufferPositionNotify[3]
					{
						default(DirectSoundBufferPositionNotify),
						default(DirectSoundBufferPositionNotify),
						default(DirectSoundBufferPositionNotify)
					};
					array[0].dwOffset = 0u;
					array[0].hEventNotify = frameEventWaitHandle1.SafeWaitHandle.DangerousGetHandle();
					array[1] = default(DirectSoundBufferPositionNotify);
					array[1].dwOffset = (uint)samplesFrameSize;
					array[1].hEventNotify = frameEventWaitHandle2.SafeWaitHandle.DangerousGetHandle();
					array[2] = default(DirectSoundBufferPositionNotify);
					array[2].dwOffset = uint.MaxValue;
					array[2].hEventNotify = endEventWaitHandle.SafeWaitHandle.DangerousGetHandle();
					obj.SetNotificationPositions(3u, array);
				}
			}
		}

		public void Dispose()
		{
			Stop();
			GC.SuppressFinalize(this);
		}

		private bool IsBufferLost()
		{
			if ((secondaryBuffer.GetStatus() & DirectSoundBufferStatus.DSBSTATUS_BUFFERLOST) == 0)
			{
				return false;
			}
			return true;
		}

		private int MsToBytes(int ms)
		{
			int num = ms * (waveFormat.AverageBytesPerSecond / 1000);
			return num - num % waveFormat.BlockAlign;
		}

		private void PlaybackThreadFunc()
		{
			bool flag = false;
			bool flag2 = false;
			bytesPlayed = 0L;
			Exception ex = null;
			try
			{
				InitializeDirectSound();
				int num = 1;
				if (PlaybackState == PlaybackState.Stopped)
				{
					secondaryBuffer.SetCurrentPosition(0u);
					nextSamplesWriteIndex = 0;
					num = Feed(samplesTotalSize);
				}
				if (num <= 0)
				{
					return;
				}
				lock (m_LockObject)
				{
					playbackState = PlaybackState.Playing;
				}
				secondaryBuffer.Play(0u, 0u, DirectSoundPlayFlags.DSBPLAY_LOOPING);
				WaitHandle[] waitHandles = new WaitHandle[3] { frameEventWaitHandle1, frameEventWaitHandle2, endEventWaitHandle };
				bool flag3 = true;
				while (PlaybackState != PlaybackState.Stopped && flag3)
				{
					int num2 = WaitHandle.WaitAny(waitHandles, 3 * desiredLatency, exitContext: false);
					if (num2 != 258)
					{
						switch (num2)
						{
						case 2:
							StopPlayback();
							flag = true;
							flag3 = false;
							continue;
						case 0:
							if (flag2)
							{
								bytesPlayed += samplesFrameSize * 2;
							}
							break;
						default:
							flag2 = true;
							break;
						}
						num2 = ((num2 == 0) ? 1 : 0);
						nextSamplesWriteIndex = num2 * samplesFrameSize;
						if (Feed(samplesFrameSize) == 0)
						{
							StopPlayback();
							flag = true;
							flag3 = false;
						}
						continue;
					}
					StopPlayback();
					flag = true;
					flag3 = false;
					throw new Exception("DirectSound buffer timeout");
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				if (!flag)
				{
					try
					{
						StopPlayback();
					}
					catch (Exception ex3)
					{
						if (ex == null)
						{
							ex = ex3;
						}
					}
				}
				lock (m_LockObject)
				{
					playbackState = PlaybackState.Stopped;
				}
				bytesPlayed = 0L;
				RaisePlaybackStopped(ex);
			}
		}

		private void RaisePlaybackStopped(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.PlaybackStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}

		private void StopPlayback()
		{
			lock (m_LockObject)
			{
				if (secondaryBuffer != null)
				{
					CleanUpSecondaryBuffer();
					secondaryBuffer.Stop();
					secondaryBuffer = null;
				}
				if (primarySoundBuffer != null)
				{
					primarySoundBuffer.Stop();
					primarySoundBuffer = null;
				}
			}
		}

		private void CleanUpSecondaryBuffer()
		{
			if (secondaryBuffer == null)
			{
				return;
			}
			byte[] source = new byte[samplesTotalSize];
			secondaryBuffer.Lock(0, (uint)samplesTotalSize, out var audioPtr, out var audioBytes, out var audioPtr2, out var audioBytes2, DirectSoundBufferLockFlag.None);
			if (audioPtr != IntPtr.Zero)
			{
				Marshal.Copy(source, 0, audioPtr, audioBytes);
				if (audioPtr2 != IntPtr.Zero)
				{
					Marshal.Copy(source, 0, audioPtr, audioBytes);
				}
			}
			secondaryBuffer.Unlock(audioPtr, audioBytes, audioPtr2, audioBytes2);
		}

		private int Feed(int bytesToCopy)
		{
			int num = bytesToCopy;
			if (IsBufferLost())
			{
				secondaryBuffer.Restore();
			}
			if (playbackState == PlaybackState.Paused)
			{
				Array.Clear(samples, 0, samples.Length);
			}
			else
			{
				num = waveStream.Read(samples, 0, bytesToCopy);
				if (num == 0)
				{
					Array.Clear(samples, 0, samples.Length);
					return 0;
				}
			}
			secondaryBuffer.Lock(nextSamplesWriteIndex, (uint)num, out var audioPtr, out var audioBytes, out var audioPtr2, out var audioBytes2, DirectSoundBufferLockFlag.None);
			if (audioPtr != IntPtr.Zero)
			{
				Marshal.Copy(samples, 0, audioPtr, audioBytes);
				if (audioPtr2 != IntPtr.Zero)
				{
					Marshal.Copy(samples, 0, audioPtr, audioBytes);
				}
			}
			secondaryBuffer.Unlock(audioPtr, audioBytes, audioPtr2, audioBytes2);
			return num;
		}

		[DllImport("dsound.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern void DirectSoundCreate(ref Guid GUID, [MarshalAs(UnmanagedType.Interface)] out IDirectSound directSound, IntPtr pUnkOuter);

		[DllImport("dsound.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DirectSoundEnumerateA", ExactSpelling = true, SetLastError = true)]
		private static extern void DirectSoundEnumerate(DSEnumCallback lpDSEnumCallback, IntPtr lpContext);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();
	}
	public class DirectSoundDeviceInfo
	{
		public Guid Guid { get; set; }

		public string Description { get; set; }

		public string ModuleName { get; set; }
	}
	public interface IWaveBuffer
	{
		byte[] ByteBuffer { get; }

		float[] FloatBuffer { get; }

		short[] ShortBuffer { get; }

		int[] IntBuffer { get; }

		int MaxSize { get; }

		int ByteBufferCount { get; }

		int FloatBufferCount { get; }

		int ShortBufferCount { get; }

		int IntBufferCount { get; }
	}
	public interface IWavePlayer : IDisposable
	{
		float Volume { get; set; }

		PlaybackState PlaybackState { get; }

		event EventHandler<StoppedEventArgs> PlaybackStopped;

		void Play();

		void Stop();

		void Pause();

		void Init(IWaveProvider waveProvider);
	}
	public interface IWavePosition
	{
		WaveFormat OutputWaveFormat { get; }

		long GetPosition();
	}
	public interface IWaveProvider
	{
		WaveFormat WaveFormat { get; }

		int Read(byte[] buffer, int offset, int count);
	}
	public interface ISampleProvider
	{
		WaveFormat WaveFormat { get; }

		int Read(float[] buffer, int offset, int count);
	}
	public class MediaFoundationEncoder : IDisposable
	{
		private readonly MediaType outputMediaType;

		private bool disposed;

		public static int[] GetEncodeBitrates(Guid audioSubtype, int sampleRate, int channels)
		{
			return (from br in (from mt in GetOutputMediaTypes(audioSubtype)
					where mt.SampleRate == sampleRate && mt.ChannelCount == channels
					select mt.AverageBytesPerSecond * 8).Distinct()
				orderby br
				select br).ToArray();
		}

		public static MediaType[] GetOutputMediaTypes(Guid audioSubtype)
		{
			IMFCollection ppAvailableTypes;
			try
			{
				MediaFoundationInterop.MFTranscodeGetAudioOutputAvailableTypes(audioSubtype, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL, null, out ppAvailableTypes);
			}
			catch (COMException exception)
			{
				if (exception.GetHResult() == -1072875819)
				{
					return new MediaType[0];
				}
				throw;
			}
			ppAvailableTypes.GetElementCount(out var pcElements);
			List<MediaType> list = new List<MediaType>(pcElements);
			for (int i = 0; i < pcElements; i++)
			{
				ppAvailableTypes.GetElement(i, out var ppUnkElement);
				IMFMediaType mediaType = (IMFMediaType)ppUnkElement;
				list.Add(new MediaType(mediaType));
			}
			Marshal.ReleaseComObject(ppAvailableTypes);
			return list.ToArray();
		}

		public static void EncodeToWma(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
		{
			using MediaFoundationEncoder mediaFoundationEncoder = new MediaFoundationEncoder(SelectMediaType(AudioSubtypes.MFAudioFormat_WMAudioV8, inputProvider.WaveFormat, desiredBitRate) ?? throw new InvalidOperationException("No suitable WMA encoders available"));
			mediaFoundationEncoder.Encode(outputFile, inputProvider);
		}

		public static void EncodeToMp3(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
		{
			using MediaFoundationEncoder mediaFoundationEncoder = new MediaFoundationEncoder(SelectMediaType(AudioSubtypes.MFAudioFormat_MP3, inputProvider.WaveFormat, desiredBitRate) ?? throw new InvalidOperationException("No suitable MP3 encoders available"));
			mediaFoundationEncoder.Encode(outputFile, inputProvider);
		}

		public static void EncodeToAac(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
		{
			using MediaFoundationEncoder mediaFoundationEncoder = new MediaFoundationEncoder(SelectMediaType(AudioSubtypes.MFAudioFormat_AAC, inputProvider.WaveFormat, desiredBitRate) ?? throw new InvalidOperationException("No suitable AAC encoders available"));
			mediaFoundationEncoder.Encode(outputFile, inputProvider);
		}

		public static MediaType SelectMediaType(Guid audioSubtype, WaveFormat inputFormat, int desiredBitRate)
		{
			return (from mt in GetOutputMediaTypes(audioSubtype)
				where mt.SampleRate == inputFormat.SampleRate && mt.ChannelCount == inputFormat.Channels
				select new
				{
					MediaType = mt,
					Delta = Math.Abs(desiredBitRate - mt.AverageBytesPerSecond * 8)
				} into mt
				orderby mt.Delta
				select mt.MediaType).FirstOrDefault();
		}

		public MediaFoundationEncoder(MediaType outputMediaType)
		{
			if (outputMediaType == null)
			{
				throw new ArgumentNullException("outputMediaType");
			}
			this.outputMediaType = outputMediaType;
		}

		public void Encode(string outputFile, IWaveProvider inputProvider)
		{
			if (inputProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm && inputProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Encode input format must be PCM or IEEE float");
			}
			MediaType mediaType = new MediaType(inputProvider.WaveFormat);
			IMFSinkWriter iMFSinkWriter = CreateSinkWriter(outputFile);
			try
			{
				iMFSinkWriter.AddStream(outputMediaType.MediaFoundationObject, out var pdwStreamIndex);
				iMFSinkWriter.SetInputMediaType(pdwStreamIndex, mediaType.MediaFoundationObject, null);
				PerformEncode(iMFSinkWriter, pdwStreamIndex, inputProvider);
			}
			finally
			{
				Marshal.ReleaseComObject(iMFSinkWriter);
				Marshal.ReleaseComObject(mediaType.MediaFoundationObject);
			}
		}

		private static IMFSinkWriter CreateSinkWriter(string outputFile)
		{
			IMFAttributes iMFAttributes = MediaFoundationApi.CreateAttributes(1);
			iMFAttributes.SetUINT32(MediaFoundationAttributes.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
			try
			{
				MediaFoundationInterop.MFCreateSinkWriterFromURL(outputFile, null, iMFAttributes, out var ppSinkWriter);
				return ppSinkWriter;
			}
			catch (COMException exception)
			{
				if (exception.GetHResult() == -1072875819)
				{
					throw new ArgumentException("Was not able to create a sink writer for this file extension");
				}
				throw;
			}
			finally
			{
				Marshal.ReleaseComObject(iMFAttributes);
			}
		}

		private void PerformEncode(IMFSinkWriter writer, int streamIndex, IWaveProvider inputProvider)
		{
			byte[] managedBuffer = new byte[inputProvider.WaveFormat.AverageBytesPerSecond * 4];
			writer.BeginWriting();
			long num = 0L;
			long num2 = 0L;
			do
			{
				num2 = ConvertOneBuffer(writer, streamIndex, inputProvider, num, managedBuffer);
				num += num2;
			}
			while (num2 > 0);
			writer.DoFinalize();
		}

		private static long BytesToNsPosition(int bytes, WaveFormat waveFormat)
		{
			return 10000000L * (long)bytes / waveFormat.AverageBytesPerSecond;
		}

		private long ConvertOneBuffer(IMFSinkWriter writer, int streamIndex, IWaveProvider inputProvider, long position, byte[] managedBuffer)
		{
			long num = 0L;
			IMFMediaBuffer iMFMediaBuffer = MediaFoundationApi.CreateMemoryBuffer(managedBuffer.Length);
			iMFMediaBuffer.GetMaxLength(out var pcbMaxLength);
			IMFSample iMFSample = MediaFoundationApi.CreateSample();
			iMFSample.AddBuffer(iMFMediaBuffer);
			iMFMediaBuffer.Lock(out var ppbBuffer, out pcbMaxLength, out var _);
			int num2 = inputProvider.Read(managedBuffer, 0, pcbMaxLength);
			if (num2 > 0)
			{
				num = BytesToNsPosition(num2, inputProvider.WaveFormat);
				Marshal.Copy(managedBuffer, 0, ppbBuffer, num2);
				iMFMediaBuffer.SetCurrentLength(num2);
				iMFMediaBuffer.Unlock();
				iMFSample.SetSampleTime(position);
				iMFSample.SetSampleDuration(num);
				writer.WriteSample(streamIndex, iMFSample);
			}
			else
			{
				iMFMediaBuffer.Unlock();
			}
			Marshal.ReleaseComObject(iMFSample);
			Marshal.ReleaseComObject(iMFMediaBuffer);
			return num;
		}

		protected void Dispose(bool disposing)
		{
			Marshal.ReleaseComObject(outputMediaType.MediaFoundationObject);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				Dispose(disposing: true);
			}
			GC.SuppressFinalize(this);
		}

		~MediaFoundationEncoder()
		{
			Dispose(disposing: false);
		}
	}
	public enum PlaybackState
	{
		Stopped,
		Playing,
		Paused
	}
	public class StoppedEventArgs : EventArgs
	{
		private readonly Exception exception;

		public Exception Exception => exception;

		public StoppedEventArgs(Exception exception = null)
		{
			this.exception = exception;
		}
	}
	public class WasapiOut : IWavePlayer, IDisposable, IWavePosition
	{
		private AudioClient audioClient;

		private readonly MMDevice mmDevice;

		private readonly AudioClientShareMode shareMode;

		private AudioRenderClient renderClient;

		private IWaveProvider sourceProvider;

		private int latencyMilliseconds;

		private int bufferFrameCount;

		private int bytesPerFrame;

		private readonly bool isUsingEventSync;

		private EventWaitHandle frameEventWaitHandle;

		private byte[] readBuffer;

		private volatile PlaybackState playbackState;

		private Thread playThread;

		private WaveFormat outputFormat;

		private bool dmoResamplerNeeded;

		private readonly SynchronizationContext syncContext;

		public WaveFormat OutputWaveFormat => outputFormat;

		public PlaybackState PlaybackState => playbackState;

		public float Volume
		{
			get
			{
				return mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
				}
				if (value > 1f)
				{
					throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
				}
				mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
			}
		}

		public AudioStreamVolume AudioStreamVolume
		{
			get
			{
				if (shareMode == AudioClientShareMode.Exclusive)
				{
					throw new InvalidOperationException("AudioStreamVolume is ONLY supported for shared audio streams.");
				}
				return audioClient.AudioStreamVolume;
			}
		}

		public event EventHandler<StoppedEventArgs> PlaybackStopped;

		public WasapiOut()
			: this(GetDefaultAudioEndpoint(), AudioClientShareMode.Shared, useEventSync: true, 200)
		{
		}

		public WasapiOut(AudioClientShareMode shareMode, int latency)
			: this(GetDefaultAudioEndpoint(), shareMode, useEventSync: true, latency)
		{
		}

		public WasapiOut(AudioClientShareMode shareMode, bool useEventSync, int latency)
			: this(GetDefaultAudioEndpoint(), shareMode, useEventSync, latency)
		{
		}

		public WasapiOut(MMDevice device, AudioClientShareMode shareMode, bool useEventSync, int latency)
		{
			audioClient = device.AudioClient;
			mmDevice = device;
			this.shareMode = shareMode;
			isUsingEventSync = useEventSync;
			latencyMilliseconds = latency;
			syncContext = SynchronizationContext.Current;
			outputFormat = audioClient.MixFormat;
		}

		private static MMDevice GetDefaultAudioEndpoint()
		{
			if (Environment.OSVersion.Version.Major < 6)
			{
				throw new NotSupportedException("WASAPI supported only on Windows Vista and above");
			}
			return new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
		}

		private void PlayThread()
		{
			ResamplerDmoStream resamplerDmoStream = null;
			IWaveProvider playbackProvider = sourceProvider;
			Exception e = null;
			try
			{
				if (dmoResamplerNeeded)
				{
					resamplerDmoStream = new ResamplerDmoStream(sourceProvider, outputFormat);
					playbackProvider = resamplerDmoStream;
				}
				bufferFrameCount = audioClient.BufferSize;
				bytesPerFrame = outputFormat.Channels * outputFormat.BitsPerSample / 8;
				readBuffer = new byte[bufferFrameCount * bytesPerFrame];
				FillBuffer(playbackProvider, bufferFrameCount);
				WaitHandle[] waitHandles = new WaitHandle[1] { frameEventWaitHandle };
				audioClient.Start();
				while (playbackState != 0)
				{
					int num = 0;
					if (isUsingEventSync)
					{
						num = WaitHandle.WaitAny(waitHandles, 3 * latencyMilliseconds, exitContext: false);
					}
					else
					{
						Thread.Sleep(latencyMilliseconds / 2);
					}
					if (playbackState == PlaybackState.Playing && num != 258)
					{
						int num2 = ((!isUsingEventSync) ? audioClient.CurrentPadding : ((shareMode == AudioClientShareMode.Shared) ? audioClient.CurrentPadding : 0));
						int num3 = bufferFrameCount - num2;
						if (num3 > 10)
						{
							FillBuffer(playbackProvider, num3);
						}
					}
				}
				Thread.Sleep(latencyMilliseconds / 2);
				audioClient.Stop();
				if (playbackState == PlaybackState.Stopped)
				{
					audioClient.Reset();
				}
			}
			catch (Exception ex)
			{
				e = ex;
			}
			finally
			{
				resamplerDmoStream?.Dispose();
				RaisePlaybackStopped(e);
			}
		}

		private void RaisePlaybackStopped(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.PlaybackStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}

		private void FillBuffer(IWaveProvider playbackProvider, int frameCount)
		{
			IntPtr buffer = renderClient.GetBuffer(frameCount);
			int count = frameCount * bytesPerFrame;
			int num = playbackProvider.Read(readBuffer, 0, count);
			if (num == 0)
			{
				playbackState = PlaybackState.Stopped;
			}
			Marshal.Copy(readBuffer, 0, buffer, num);
			if (isUsingEventSync && shareMode == AudioClientShareMode.Exclusive)
			{
				renderClient.ReleaseBuffer(frameCount, AudioClientBufferFlags.None);
				return;
			}
			int numFramesWritten = num / bytesPerFrame;
			renderClient.ReleaseBuffer(numFramesWritten, AudioClientBufferFlags.None);
		}

		private WaveFormat GetFallbackFormat()
		{
			WaveFormat waveFormat = audioClient.MixFormat;
			if (!audioClient.IsFormatSupported(shareMode, waveFormat))
			{
				WaveFormatExtensible[] array = new WaveFormatExtensible[3]
				{
					new WaveFormatExtensible(outputFormat.SampleRate, 32, outputFormat.Channels),
					new WaveFormatExtensible(outputFormat.SampleRate, 24, outputFormat.Channels),
					new WaveFormatExtensible(outputFormat.SampleRate, 16, outputFormat.Channels)
				};
				for (int i = 0; i < array.Length; i++)
				{
					waveFormat = array[i];
					if (audioClient.IsFormatSupported(shareMode, waveFormat))
					{
						break;
					}
					waveFormat = null;
				}
				if (waveFormat == null)
				{
					waveFormat = new WaveFormatExtensible(outputFormat.SampleRate, 16, 2);
					if (!audioClient.IsFormatSupported(shareMode, waveFormat))
					{
						throw new NotSupportedException("Can't find a supported format to use");
					}
				}
			}
			return waveFormat;
		}

		public long GetPosition()
		{
			ulong position;
			switch (playbackState)
			{
			case PlaybackState.Stopped:
				return 0L;
			case PlaybackState.Playing:
				position = audioClient.AudioClockClient.AdjustedPosition;
				break;
			default:
			{
				audioClient.AudioClockClient.GetPosition(out position, out var _);
				break;
			}
			}
			return (long)position * (long)outputFormat.AverageBytesPerSecond / (long)audioClient.AudioClockClient.Frequency;
		}

		public void Play()
		{
			if (playbackState != PlaybackState.Playing)
			{
				if (playbackState == PlaybackState.Stopped)
				{
					playThread = new Thread(PlayThread);
					playbackState = PlaybackState.Playing;
					playThread.Start();
				}
				else
				{
					playbackState = PlaybackState.Playing;
				}
			}
		}

		public void Stop()
		{
			if (playbackState != 0)
			{
				playbackState = PlaybackState.Stopped;
				playThread.Join();
				playThread = null;
			}
		}

		public void Pause()
		{
			if (playbackState == PlaybackState.Playing)
			{
				playbackState = PlaybackState.Paused;
			}
		}

		public void Init(IWaveProvider waveProvider)
		{
			long num = latencyMilliseconds * 10000;
			outputFormat = waveProvider.WaveFormat;
			if (!audioClient.IsFormatSupported(shareMode, outputFormat, out var closestMatchFormat))
			{
				if (closestMatchFormat == null)
				{
					outputFormat = GetFallbackFormat();
				}
				else
				{
					outputFormat = closestMatchFormat;
				}
				try
				{
					using (new ResamplerDmoStream(waveProvider, outputFormat))
					{
					}
				}
				catch (Exception)
				{
					outputFormat = GetFallbackFormat();
					using (new ResamplerDmoStream(waveProvider, outputFormat))
					{
					}
				}
				dmoResamplerNeeded = true;
			}
			else
			{
				dmoResamplerNeeded = false;
			}
			sourceProvider = waveProvider;
			if (isUsingEventSync)
			{
				if (shareMode == AudioClientShareMode.Shared)
				{
					audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, num, 0L, outputFormat, Guid.Empty);
					long streamLatency = audioClient.StreamLatency;
					if (streamLatency != 0L)
					{
						latencyMilliseconds = (int)(streamLatency / 10000);
					}
				}
				else
				{
					try
					{
						audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, num, num, outputFormat, Guid.Empty);
					}
					catch (COMException ex2)
					{
						if (ex2.ErrorCode != ErrorCodes.AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED)
						{
							throw ex2;
						}
						long num2 = (long)(10000000.0 / (double)outputFormat.SampleRate * (double)audioClient.BufferSize + 0.5);
						audioClient.Dispose();
						audioClient = mmDevice.AudioClient;
						audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, num2, num2, outputFormat, Guid.Empty);
					}
				}
				frameEventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
				audioClient.SetEventHandle(frameEventWaitHandle.SafeWaitHandle.DangerousGetHandle());
			}
			else
			{
				audioClient.Initialize(shareMode, AudioClientStreamFlags.None, num, 0L, outputFormat, Guid.Empty);
			}
			renderClient = audioClient.AudioRenderClient;
		}

		public void Dispose()
		{
			if (audioClient != null)
			{
				Stop();
				audioClient.Dispose();
				audioClient = null;
				renderClient = null;
			}
		}
	}
	[StructLayout(LayoutKind.Explicit, Pack = 2)]
	public class WaveBuffer : IWaveBuffer
	{
		[FieldOffset(0)]
		public int numberOfBytes;

		[FieldOffset(8)]
		private byte[] byteBuffer;

		[FieldOffset(8)]
		private float[] floatBuffer;

		[FieldOffset(8)]
		private short[] shortBuffer;

		[FieldOffset(8)]
		private int[] intBuffer;

		public byte[] ByteBuffer => byteBuffer;

		public float[] FloatBuffer => floatBuffer;

		public short[] ShortBuffer => shortBuffer;

		public int[] IntBuffer => intBuffer;

		public int MaxSize => byteBuffer.Length;

		public int ByteBufferCount
		{
			get
			{
				return numberOfBytes;
			}
			set
			{
				numberOfBytes = CheckValidityCount("ByteBufferCount", value, 1);
			}
		}

		public int FloatBufferCount
		{
			get
			{
				return numberOfBytes / 4;
			}
			set
			{
				numberOfBytes = CheckValidityCount("FloatBufferCount", value, 4);
			}
		}

		public int ShortBufferCount
		{
			get
			{
				return numberOfBytes / 2;
			}
			set
			{
				numberOfBytes = CheckValidityCount("ShortBufferCount", value, 2);
			}
		}

		public int IntBufferCount
		{
			get
			{
				return numberOfBytes / 4;
			}
			set
			{
				numberOfBytes = CheckValidityCount("IntBufferCount", value, 4);
			}
		}

		public WaveBuffer(int sizeToAllocateInBytes)
		{
			int num = sizeToAllocateInBytes % 4;
			sizeToAllocateInBytes = ((num == 0) ? sizeToAllocateInBytes : (sizeToAllocateInBytes + 4 - num));
			byteBuffer = new byte[sizeToAllocateInBytes];
			numberOfBytes = 0;
		}

		public WaveBuffer(byte[] bufferToBoundTo)
		{
			BindTo(bufferToBoundTo);
		}

		public void BindTo(byte[] bufferToBoundTo)
		{
			byteBuffer = bufferToBoundTo;
			numberOfBytes = 0;
		}

		public static implicit operator byte[](WaveBuffer waveBuffer)
		{
			return waveBuffer.byteBuffer;
		}

		public static implicit operator float[](WaveBuffer waveBuffer)
		{
			return waveBuffer.floatBuffer;
		}

		public static implicit operator int[](WaveBuffer waveBuffer)
		{
			return waveBuffer.intBuffer;
		}

		public static implicit operator short[](WaveBuffer waveBuffer)
		{
			return waveBuffer.shortBuffer;
		}

		public void Clear()
		{
			Array.Clear(byteBuffer, 0, byteBuffer.Length);
		}

		public void Copy(Array destinationArray)
		{
			Array.Copy(byteBuffer, destinationArray, numberOfBytes);
		}

		private int CheckValidityCount(string argName, int value, int sizeOfValue)
		{
			int num = value * sizeOfValue;
			if (num % 4 != 0)
			{
				throw new ArgumentOutOfRangeException(argName, $"{argName} cannot set a count ({num}) that is not 4 bytes aligned ");
			}
			if (value < 0 || value > byteBuffer.Length / sizeOfValue)
			{
				throw new ArgumentOutOfRangeException(argName, $"{argName} cannot set a count that exceed max count {byteBuffer.Length / sizeOfValue}");
			}
			return num;
		}
	}
	public class WaveFileWriter : Stream
	{
		private Stream outStream;

		private readonly BinaryWriter writer;

		private long dataSizePos;

		private long factSampleCountPos;

		private long dataChunkSize;

		private readonly WaveFormat format;

		private readonly string filename;

		private readonly byte[] value24 = new byte[3];

		public string Filename => filename;

		public override long Length => dataChunkSize;

		public TimeSpan TotalTime => TimeSpan.FromSeconds((double)Length / (double)WaveFormat.AverageBytesPerSecond);

		public WaveFormat WaveFormat => format;

		public override bool CanRead => false;

		public override bool CanWrite => true;

		public override bool CanSeek => false;

		public override long Position
		{
			get
			{
				return dataChunkSize;
			}
			set
			{
				throw new InvalidOperationException("Repositioning a WaveFileWriter is not supported");
			}
		}

		public static void CreateWaveFile16(string filename, ISampleProvider sourceProvider)
		{
			CreateWaveFile(filename, new SampleToWaveProvider16(sourceProvider));
		}

		public static void CreateWaveFile(string filename, IWaveProvider sourceProvider)
		{
			using WaveFileWriter waveFileWriter = new WaveFileWriter(filename, sourceProvider.WaveFormat);
			byte[] array = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
			while (true)
			{
				int num = sourceProvider.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				waveFileWriter.Write(array, 0, num);
			}
		}

		public static void WriteWavFileToStream(Stream outStream, IWaveProvider sourceProvider)
		{
			using WaveFileWriter waveFileWriter = new WaveFileWriter(new IgnoreDisposeStream(outStream), sourceProvider.WaveFormat);
			byte[] array = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
			while (true)
			{
				int num = sourceProvider.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				waveFileWriter.Write(array, 0, num);
			}
			outStream.Flush();
		}

		public WaveFileWriter(Stream outStream, WaveFormat format)
		{
			this.outStream = outStream;
			this.format = format;
			writer = new BinaryWriter(outStream, Encoding.UTF8);
			writer.Write(Encoding.UTF8.GetBytes("RIFF"));
			writer.Write(0);
			writer.Write(Encoding.UTF8.GetBytes("WAVE"));
			writer.Write(Encoding.UTF8.GetBytes("fmt "));
			format.Serialize(writer);
			CreateFactChunk();
			WriteDataChunkHeader();
		}

		public WaveFileWriter(string filename, WaveFormat format)
			: this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format)
		{
			this.filename = filename;
		}

		private void WriteDataChunkHeader()
		{
			writer.Write(Encoding.UTF8.GetBytes("data"));
			dataSizePos = outStream.Position;
			writer.Write(0);
		}

		private void CreateFactChunk()
		{
			if (HasFactChunk())
			{
				writer.Write(Encoding.UTF8.GetBytes("fact"));
				writer.Write(4);
				factSampleCountPos = outStream.Position;
				writer.Write(0);
			}
		}

		private bool HasFactChunk()
		{
			if (format.Encoding != WaveFormatEncoding.Pcm)
			{
				return format.BitsPerSample != 0;
			}
			return false;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("Cannot read from a WaveFileWriter");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException("Cannot seek within a WaveFileWriter");
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException("Cannot set length of a WaveFileWriter");
		}

		[Obsolete("Use Write instead")]
		public void WriteData(byte[] data, int offset, int count)
		{
			Write(data, offset, count);
		}

		public override void Write(byte[] data, int offset, int count)
		{
			if (outStream.Length + count > uint.MaxValue)
			{
				throw new ArgumentException("WAV file too large", "count");
			}
			outStream.Write(data, offset, count);
			dataChunkSize += count;
		}

		public void WriteSample(float sample)
		{
			if (WaveFormat.BitsPerSample == 16)
			{
				writer.Write((short)(32767f * sample));
				dataChunkSize += 2L;
			}
			else if (WaveFormat.BitsPerSample == 24)
			{
				byte[] bytes = BitConverter.GetBytes((int)(2.1474836E+09f * sample));
				value24[0] = bytes[1];
				value24[1] = bytes[2];
				value24[2] = bytes[3];
				writer.Write(value24);
				dataChunkSize += 3L;
			}
			else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
			{
				writer.Write(65535 * (int)sample);
				dataChunkSize += 4L;
			}
			else
			{
				if (WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
				{
					throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
				}
				writer.Write(sample);
				dataChunkSize += 4L;
			}
		}

		public void WriteSamples(float[] samples, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				WriteSample(samples[offset + i]);
			}
		}

		[Obsolete("Use WriteSamples instead")]
		public void WriteData(short[] samples, int offset, int count)
		{
			WriteSamples(samples, offset, count);
		}

		public void WriteSamples(short[] samples, int offset, int count)
		{
			if (WaveFormat.BitsPerSample == 16)
			{
				for (int i = 0; i < count; i++)
				{
					writer.Write(samples[i + offset]);
				}
				dataChunkSize += count * 2;
			}
			else if (WaveFormat.BitsPerSample == 24)
			{
				for (int j = 0; j < count; j++)
				{
					byte[] bytes = BitConverter.GetBytes(65535 * samples[j + offset]);
					value24[0] = bytes[1];
					value24[1] = bytes[2];
					value24[2] = bytes[3];
					writer.Write(value24);
				}
				dataChunkSize += count * 3;
			}
			else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
			{
				for (int k = 0; k < count; k++)
				{
					writer.Write(65535 * samples[k + offset]);
				}
				dataChunkSize += count * 4;
			}
			else
			{
				if (WaveFormat.BitsPerSample != 32 || WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
				{
					throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
				}
				for (int l = 0; l < count; l++)
				{
					writer.Write((float)samples[l + offset] / 32768f);
				}
				dataChunkSize += count * 4;
			}
		}

		public override void Flush()
		{
			long position = writer.BaseStream.Position;
			UpdateHeader(writer);
			writer.BaseStream.Position = position;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && outStream != null)
			{
				try
				{
					UpdateHeader(writer);
				}
				finally
				{
					outStream.Dispose();
					outStream = null;
				}
			}
		}

		protected virtual void UpdateHeader(BinaryWriter writer)
		{
			writer.Flush();
			UpdateRiffChunk(writer);
			UpdateFactChunk(writer);
			UpdateDataChunk(writer);
		}

		private void UpdateDataChunk(BinaryWriter writer)
		{
			writer.Seek((int)dataSizePos, SeekOrigin.Begin);
			writer.Write((uint)dataChunkSize);
		}

		private void UpdateRiffChunk(BinaryWriter writer)
		{
			writer.Seek(4, SeekOrigin.Begin);
			writer.Write((uint)(outStream.Length - 8));
		}

		private void UpdateFactChunk(BinaryWriter writer)
		{
			if (HasFactChunk())
			{
				int num = format.BitsPerSample * format.Channels;
				if (num != 0)
				{
					writer.Seek((int)factSampleCountPos, SeekOrigin.Begin);
					writer.Write((int)(dataChunkSize * 8 / num));
				}
			}
		}

		~WaveFileWriter()
		{
			Dispose(disposing: false);
		}
	}
	public class WaveOut : IWavePlayer, IDisposable, IWavePosition
	{
		private IntPtr hWaveOut;

		private WaveOutBuffer[] buffers;

		private IWaveProvider waveStream;

		private volatile PlaybackState playbackState;

		private readonly WaveInterop.WaveCallback callback;

		private readonly WaveCallbackInfo callbackInfo;

		private readonly object waveOutLock;

		private int queuedBuffers;

		private readonly SynchronizationContext syncContext;

		public static int DeviceCount => WaveInterop.waveOutGetNumDevs();

		public int DesiredLatency { get; set; }

		public int NumberOfBuffers { get; set; }

		public int DeviceNumber { get; set; } = -1;


		public WaveFormat OutputWaveFormat => waveStream.WaveFormat;

		public PlaybackState PlaybackState => playbackState;

		public float Volume
		{
			get
			{
				return WaveOutUtils.GetWaveOutVolume(hWaveOut, waveOutLock);
			}
			set
			{
				WaveOutUtils.SetWaveOutVolume(value, hWaveOut, waveOutLock);
			}
		}

		public event EventHandler<StoppedEventArgs> PlaybackStopped;

		public static WaveOutCapabilities GetCapabilities(int devNumber)
		{
			WaveOutCapabilities waveOutCaps = default(WaveOutCapabilities);
			int waveOutCapsSize = Marshal.SizeOf(waveOutCaps);
			MmException.Try(WaveInterop.waveOutGetDevCaps((IntPtr)devNumber, out waveOutCaps, waveOutCapsSize), "waveOutGetDevCaps");
			return waveOutCaps;
		}

		public WaveOut()
			: this((SynchronizationContext.Current == null) ? WaveCallbackInfo.FunctionCallback() : WaveCallbackInfo.NewWindow())
		{
		}

		public WaveOut(IntPtr windowHandle)
			: this(WaveCallbackInfo.ExistingWindow(windowHandle))
		{
		}

		public WaveOut(WaveCallbackInfo callbackInfo)
		{
			syncContext = SynchronizationContext.Current;
			DesiredLatency = 300;
			NumberOfBuffers = 2;
			callback = Callback;
			waveOutLock = new object();
			this.callbackInfo = callbackInfo;
			callbackInfo.Connect(callback);
		}

		public void Init(IWaveProvider waveProvider)
		{
			waveStream = waveProvider;
			int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);
			MmResult result;
			lock (waveOutLock)
			{
				result = callbackInfo.WaveOutOpen(out hWaveOut, DeviceNumber, waveStream.WaveFormat, callback);
			}
			MmException.Try(result, "waveOutOpen");
			buffers = new WaveOutBuffer[NumberOfBuffers];
			playbackState = PlaybackState.Stopped;
			for (int i = 0; i < NumberOfBuffers; i++)
			{
				buffers[i] = new WaveOutBuffer(hWaveOut, bufferSize, waveStream, waveOutLock);
			}
		}

		public void Play()
		{
			if (playbackState == PlaybackState.Stopped)
			{
				playbackState = PlaybackState.Playing;
				EnqueueBuffers();
			}
			else if (playbackState == PlaybackState.Paused)
			{
				EnqueueBuffers();
				Resume();
				playbackState = PlaybackState.Playing;
			}
		}

		private void EnqueueBuffers()
		{
			for (int i = 0; i < NumberOfBuffers; i++)
			{
				if (!buffers[i].InQueue)
				{
					if (!buffers[i].OnDone())
					{
						playbackState = PlaybackState.Stopped;
						break;
					}
					Interlocked.Increment(ref queuedBuffers);
				}
			}
		}

		public void Pause()
		{
			if (playbackState == PlaybackState.Playing)
			{
				playbackState = PlaybackState.Paused;
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutPause(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutPause");
				}
			}
		}

		public void Resume()
		{
			if (playbackState == PlaybackState.Paused)
			{
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutRestart(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutRestart");
				}
				playbackState = PlaybackState.Playing;
			}
		}

		public void Stop()
		{
			if (playbackState != 0)
			{
				playbackState = PlaybackState.Stopped;
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutReset(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutReset");
				}
				if (callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback)
				{
					RaisePlaybackStoppedEvent(null);
				}
			}
		}

		public long GetPosition()
		{
			return WaveOutUtils.GetPositionBytes(hWaveOut, waveOutLock);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		protected void Dispose(bool disposing)
		{
			Stop();
			if (disposing && buffers != null)
			{
				for (int i = 0; i < buffers.Length; i++)
				{
					if (buffers[i] != null)
					{
						buffers[i].Dispose();
					}
				}
				buffers = null;
			}
			lock (waveOutLock)
			{
				WaveInterop.waveOutClose(hWaveOut);
			}
			if (disposing)
			{
				callbackInfo.Disconnect();
			}
		}

		~WaveOut()
		{
			Dispose(disposing: false);
		}

		private void Callback(IntPtr hWaveOut, WaveInterop.WaveMessage uMsg, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved)
		{
			if (uMsg != WaveInterop.WaveMessage.WaveOutDone)
			{
				return;
			}
			WaveOutBuffer waveOutBuffer = (WaveOutBuffer)((GCHandle)wavhdr.userData).Target;
			Interlocked.Decrement(ref queuedBuffers);
			Exception e = null;
			if (PlaybackState == PlaybackState.Playing)
			{
				lock (waveOutLock)
				{
					try
					{
						if (waveOutBuffer.OnDone())
						{
							Interlocked.Increment(ref queuedBuffers);
						}
					}
					catch (Exception ex)
					{
						e = ex;
					}
				}
			}
			if (queuedBuffers == 0 && (callbackInfo.Strategy != 0 || playbackState != 0))
			{
				playbackState = PlaybackState.Stopped;
				RaisePlaybackStoppedEvent(e);
			}
		}

		private void RaisePlaybackStoppedEvent(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.PlaybackStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}
	}
	public class WaveOutEvent : IWavePlayer, IDisposable, IWavePosition
	{
		private readonly object waveOutLock;

		private readonly SynchronizationContext syncContext;

		private IntPtr hWaveOut;

		private WaveOutBuffer[] buffers;

		private IWaveProvider waveStream;

		private volatile PlaybackState playbackState;

		private AutoResetEvent callbackEvent;

		public int DesiredLatency { get; set; }

		public int NumberOfBuffers { get; set; }

		public int DeviceNumber { get; set; } = -1;


		public WaveFormat OutputWaveFormat => waveStream.WaveFormat;

		public PlaybackState PlaybackState => playbackState;

		public float Volume
		{
			get
			{
				return WaveOutUtils.GetWaveOutVolume(hWaveOut, waveOutLock);
			}
			set
			{
				WaveOutUtils.SetWaveOutVolume(value, hWaveOut, waveOutLock);
			}
		}

		public event EventHandler<StoppedEventArgs> PlaybackStopped;

		public WaveOutEvent()
		{
			syncContext = SynchronizationContext.Current;
			if (syncContext != null && (syncContext.GetType().Name == "LegacyAspNetSynchronizationContext" || syncContext.GetType().Name == "AspNetSynchronizationContext"))
			{
				syncContext = null;
			}
			DesiredLatency = 300;
			NumberOfBuffers = 2;
			waveOutLock = new object();
		}

		public void Init(IWaveProvider waveProvider)
		{
			if (playbackState != 0)
			{
				throw new InvalidOperationException("Can't re-initialize during playback");
			}
			if (hWaveOut != IntPtr.Zero)
			{
				DisposeBuffers();
				CloseWaveOut();
			}
			callbackEvent = new AutoResetEvent(initialState: false);
			waveStream = waveProvider;
			int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);
			MmResult result;
			lock (waveOutLock)
			{
				result = WaveInterop.waveOutOpenWindow(out hWaveOut, (IntPtr)DeviceNumber, waveStream.WaveFormat, callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent);
			}
			MmException.Try(result, "waveOutOpen");
			buffers = new WaveOutBuffer[NumberOfBuffers];
			playbackState = PlaybackState.Stopped;
			for (int i = 0; i < NumberOfBuffers; i++)
			{
				buffers[i] = new WaveOutBuffer(hWaveOut, bufferSize, waveStream, waveOutLock);
			}
		}

		public void Play()
		{
			if (buffers == null || waveStream == null)
			{
				throw new InvalidOperationException("Must call Init first");
			}
			if (playbackState == PlaybackState.Stopped)
			{
				playbackState = PlaybackState.Playing;
				callbackEvent.Set();
				ThreadPool.QueueUserWorkItem(delegate
				{
					PlaybackThread();
				}, null);
			}
			else if (playbackState == PlaybackState.Paused)
			{
				Resume();
				callbackEvent.Set();
			}
		}

		private void PlaybackThread()
		{
			Exception e = null;
			try
			{
				DoPlayback();
			}
			catch (Exception ex)
			{
				e = ex;
			}
			finally
			{
				playbackState = PlaybackState.Stopped;
				RaisePlaybackStoppedEvent(e);
			}
		}

		private void DoPlayback()
		{
			while (playbackState != 0)
			{
				if (!callbackEvent.WaitOne(DesiredLatency))
				{
					_ = playbackState;
					_ = 1;
				}
				if (playbackState != PlaybackState.Playing)
				{
					continue;
				}
				int num = 0;
				WaveOutBuffer[] array = buffers;
				foreach (WaveOutBuffer waveOutBuffer in array)
				{
					if (waveOutBuffer.InQueue || waveOutBuffer.OnDone())
					{
						num++;
					}
				}
				if (num == 0)
				{
					playbackState = PlaybackState.Stopped;
					callbackEvent.Set();
				}
			}
		}

		public void Pause()
		{
			if (playbackState == PlaybackState.Playing)
			{
				playbackState = PlaybackState.Paused;
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutPause(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutPause");
				}
			}
		}

		private void Resume()
		{
			if (playbackState == PlaybackState.Paused)
			{
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutRestart(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutRestart");
				}
				playbackState = PlaybackState.Playing;
			}
		}

		public void Stop()
		{
			if (playbackState != 0)
			{
				playbackState = PlaybackState.Stopped;
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutReset(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutReset");
				}
				callbackEvent.Set();
			}
		}

		public long GetPosition()
		{
			return WaveOutUtils.GetPositionBytes(hWaveOut, waveOutLock);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		protected void Dispose(bool disposing)
		{
			Stop();
			if (disposing)
			{
				DisposeBuffers();
			}
			CloseWaveOut();
		}

		private void CloseWaveOut()
		{
			if (callbackEvent != null)
			{
				callbackEvent.Close();
				callbackEvent = null;
			}
			lock (waveOutLock)
			{
				if (hWaveOut != IntPtr.Zero)
				{
					WaveInterop.waveOutClose(hWaveOut);
					hWaveOut = IntPtr.Zero;
				}
			}
		}

		private void DisposeBuffers()
		{
			if (buffers != null)
			{
				WaveOutBuffer[] array = buffers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Dispose();
				}
				buffers = null;
			}
		}

		~WaveOutEvent()
		{
			Dispose(disposing: false);
		}

		private void RaisePlaybackStoppedEvent(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.PlaybackStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}
	}
	internal static class WaveOutUtils
	{
		public static float GetWaveOutVolume(IntPtr hWaveOut, object lockObject)
		{
			MmResult result;
			int dwVolume;
			lock (lockObject)
			{
				result = WaveInterop.waveOutGetVolume(hWaveOut, out dwVolume);
			}
			MmException.Try(result, "waveOutGetVolume");
			return (float)(dwVolume & 0xFFFF) / 65535f;
		}

		public static void SetWaveOutVolume(float value, IntPtr hWaveOut, object lockObject)
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
			}
			if (value > 1f)
			{
				throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
			}
			int dwVolume = (int)(value * 65535f) + ((int)(value * 65535f) << 16);
			MmResult result;
			lock (lockObject)
			{
				result = WaveInterop.waveOutSetVolume(hWaveOut, dwVolume);
			}
			MmException.Try(result, "waveOutSetVolume");
		}

		public static long GetPositionBytes(IntPtr hWaveOut, object lockObject)
		{
			lock (lockObject)
			{
				MmTime mmTime = default(MmTime);
				mmTime.wType = 4u;
				MmException.Try(WaveInterop.waveOutGetPosition(hWaveOut, ref mmTime, Marshal.SizeOf(mmTime)), "waveOutGetPosition");
				if (mmTime.wType != 4)
				{
					throw new Exception($"waveOutGetPosition: wType -> Expected {4}, Received {mmTime.wType}");
				}
				return mmTime.cb;
			}
		}
	}
	public class BufferedWaveProvider : IWaveProvider
	{
		private CircularBuffer circularBuffer;

		private readonly WaveFormat waveFormat;

		public bool ReadFully { get; set; }

		public int BufferLength { get; set; }

		public TimeSpan BufferDuration
		{
			get
			{
				return TimeSpan.FromSeconds((double)BufferLength / (double)WaveFormat.AverageBytesPerSecond);
			}
			set
			{
				BufferLength = (int)(value.TotalSeconds * (double)WaveFormat.AverageBytesPerSecond);
			}
		}

		public bool DiscardOnBufferOverflow { get; set; }

		public int BufferedBytes
		{
			get
			{
				if (circularBuffer != null)
				{
					return circularBuffer.Count;
				}
				return 0;
			}
		}

		public TimeSpan BufferedDuration => TimeSpan.FromSeconds((double)BufferedBytes / (double)WaveFormat.AverageBytesPerSecond);

		public WaveFormat WaveFormat => waveFormat;

		public BufferedWaveProvider(WaveFormat waveFormat)
		{
			this.waveFormat = waveFormat;
			BufferLength = waveFormat.AverageBytesPerSecond * 5;
			ReadFully = true;
		}

		public void AddSamples(byte[] buffer, int offset, int count)
		{
			if (circularBuffer == null)
			{
				circularBuffer = new CircularBuffer(BufferLength);
			}
			if (circularBuffer.Write(buffer, offset, count) < count && !DiscardOnBufferOverflow)
			{
				throw new InvalidOperationException("Buffer full");
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			if (circularBuffer != null)
			{
				num = circularBuffer.Read(buffer, offset, count);
			}
			if (ReadFully && num < count)
			{
				Array.Clear(buffer, offset + num, count - num);
				num = count;
			}
			return num;
		}

		public void ClearBuffer()
		{
			if (circularBuffer != null)
			{
				circularBuffer.Reset();
			}
		}
	}
	public class DmoEffectWaveProvider<TDmoEffector, TEffectorParam> : IWaveProvider, IDisposable where TDmoEffector : IDmoEffector<TEffectorParam>, new()
	{
		private readonly IWaveProvider inputProvider;

		private readonly IDmoEffector<TEffectorParam> effector;

		public WaveFormat WaveFormat => inputProvider.WaveFormat;

		public TEffectorParam EffectParams => effector.EffectParams;

		public DmoEffectWaveProvider(IWaveProvider inputProvider)
		{
			this.inputProvider = inputProvider;
			effector = new TDmoEffector();
			MediaObject obj = effector.MediaObject ?? throw new NotSupportedException("Dmo Effector Not Supported: TDmoEffector");
			if (!obj.SupportsInputWaveFormat(0, inputProvider.WaveFormat))
			{
				throw new ArgumentException("Unsupported Input Stream format", "inputProvider");
			}
			obj.AllocateStreamingResources();
			obj.SetInputWaveFormat(0, this.inputProvider.WaveFormat);
			obj.SetOutputWaveFormat(0, this.inputProvider.WaveFormat);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = inputProvider.Read(buffer, offset, count);
			if (effector == null)
			{
				return num;
			}
			if (effector.MediaObjectInPlace.Process(num, offset, buffer, 0L, DmoInPlaceProcessFlags.Normal) == DmoInPlaceProcessReturn.HasEffectTail)
			{
				byte[] data = new byte[num];
				while (effector.MediaObjectInPlace.Process(num, 0, data, 0L, DmoInPlaceProcessFlags.Zero) == DmoInPlaceProcessReturn.HasEffectTail)
				{
				}
			}
			return num;
		}

		public void Dispose()
		{
			if (effector != null)
			{
				effector.MediaObject.FreeStreamingResources();
				effector.Dispose();
			}
		}
	}
	public class MediaFoundationResampler : MediaFoundationTransform
	{
		private int resamplerQuality;

		private static readonly Guid ResamplerClsid = new Guid("f447b69e-1884-4a7e-8055-346f74d6edb3");

		private static readonly Guid IMFTransformIid = new Guid("bf94c121-5b05-4e6f-8000-ba598961414d");

		private IMFActivate activate;

		public int ResamplerQuality
		{
			get
			{
				return resamplerQuality;
			}
			set
			{
				if (value < 1 || value > 60)
				{
					throw new ArgumentOutOfRangeException("Resampler Quality must be between 1 and 60");
				}
				resamplerQuality = value;
			}
		}

		private static bool IsPcmOrIeeeFloat(WaveFormat waveFormat)
		{
			WaveFormatExtensible waveFormatExtensible = waveFormat as WaveFormatExtensible;
			if (waveFormat.Encoding != WaveFormatEncoding.Pcm && waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				if (waveFormatExtensible != null)
				{
					if (!(waveFormatExtensible.SubFormat == AudioSubtypes.MFAudioFormat_PCM))
					{
						return waveFormatExtensible.SubFormat == AudioSubtypes.MFAudioFormat_Float;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		public MediaFoundationResampler(IWaveProvider sourceProvider, WaveFormat outputFormat)
			: base(sourceProvider, outputFormat)
		{
			if (!IsPcmOrIeeeFloat(sourceProvider.WaveFormat))
			{
				throw new ArgumentException("Input must be PCM or IEEE float", "sourceProvider");
			}
			if (!IsPcmOrIeeeFloat(outputFormat))
			{
				throw new ArgumentException("Output must be PCM or IEEE float", "outputFormat");
			}
			MediaFoundationApi.Startup();
			ResamplerQuality = 60;
			object comObject = CreateResamplerComObject();
			FreeComObject(comObject);
		}

		private void FreeComObject(object comObject)
		{
			if (activate != null)
			{
				activate.ShutdownObject();
			}
			Marshal.ReleaseComObject(comObject);
		}

		private object CreateResamplerComObject()
		{
			return new ResamplerMediaComObject();
		}

		private object CreateResamplerComObjectUsingActivator()
		{
			foreach (IMFActivate item in MediaFoundationApi.EnumerateTransforms(MediaFoundationTransformCategories.AudioEffect))
			{
				item.GetGUID(MediaFoundationAttributes.MFT_TRANSFORM_CLSID_Attribute, out var pguidValue);
				if (pguidValue.Equals(ResamplerClsid))
				{
					item.ActivateObject(IMFTransformIid, out var ppv);
					activate = item;
					return ppv;
				}
			}
			return null;
		}

		public MediaFoundationResampler(IWaveProvider sourceProvider, int outputSampleRate)
			: this(sourceProvider, CreateOutputFormat(sourceProvider.WaveFormat, outputSampleRate))
		{
		}

		protected override IMFTransform CreateTransform()
		{
			object obj = CreateResamplerComObject();
			IMFTransform obj2 = (IMFTransform)obj;
			IMFMediaType iMFMediaType = MediaFoundationApi.CreateMediaTypeFromWaveFormat(sourceProvider.WaveFormat);
			obj2.SetInputType(0, iMFMediaType, _MFT_SET_TYPE_FLAGS.None);
			Marshal.ReleaseComObject(iMFMediaType);
			IMFMediaType iMFMediaType2 = MediaFoundationApi.CreateMediaTypeFromWaveFormat(outputWaveFormat);
			obj2.SetOutputType(0, iMFMediaType2, _MFT_SET_TYPE_FLAGS.None);
			Marshal.ReleaseComObject(iMFMediaType2);
			((IWMResamplerProps)obj).SetHalfFilterLength(ResamplerQuality);
			return obj2;
		}

		private static WaveFormat CreateOutputFormat(WaveFormat inputFormat, int outputSampleRate)
		{
			if (inputFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				return new WaveFormat(outputSampleRate, inputFormat.BitsPerSample, inputFormat.Channels);
			}
			if (inputFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				return WaveFormat.CreateIeeeFloatWaveFormat(outputSampleRate, inputFormat.Channels);
			}
			throw new ArgumentException("Can only resample PCM or IEEE float");
		}

		protected override void Dispose(bool disposing)
		{
			if (activate != null)
			{
				activate.ShutdownObject();
				activate = null;
			}
			base.Dispose(disposing);
		}
	}
	public class MixingWaveProvider32 : IWaveProvider
	{
		private List<IWaveProvider> inputs;

		private WaveFormat waveFormat;

		private int bytesPerSample;

		public int InputCount => inputs.Count;

		public WaveFormat WaveFormat => waveFormat;

		public MixingWaveProvider32()
		{
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
			bytesPerSample = 4;
			inputs = new List<IWaveProvider>();
		}

		public MixingWaveProvider32(IEnumerable<IWaveProvider> inputs)
			: this()
		{
			foreach (IWaveProvider input in inputs)
			{
				AddInputStream(input);
			}
		}

		public void AddInputStream(IWaveProvider waveProvider)
		{
			if (waveProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be IEEE floating point", "waveProvider.WaveFormat");
			}
			if (waveProvider.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Only 32 bit audio currently supported", "waveProvider.WaveFormat");
			}
			if (inputs.Count == 0)
			{
				int sampleRate = waveProvider.WaveFormat.SampleRate;
				int channels = waveProvider.WaveFormat.Channels;
				waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
			}
			else if (!waveProvider.WaveFormat.Equals(waveFormat))
			{
				throw new ArgumentException("All incoming channels must have the same format", "waveProvider.WaveFormat");
			}
			lock (inputs)
			{
				inputs.Add(waveProvider);
			}
		}

		public void RemoveInputStream(IWaveProvider waveProvider)
		{
			lock (inputs)
			{
				inputs.Remove(waveProvider);
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			if (count % bytesPerSample != 0)
			{
				throw new ArgumentException("Must read an whole number of samples", "count");
			}
			Array.Clear(buffer, offset, count);
			int num = 0;
			byte[] array = new byte[count];
			lock (inputs)
			{
				foreach (IWaveProvider input in inputs)
				{
					int num2 = input.Read(array, 0, count);
					num = Math.Max(num, num2);
					if (num2 > 0)
					{
						Sum32BitAudio(buffer, offset, array, num2);
					}
				}
				return num;
			}
		}

		private unsafe static void Sum32BitAudio(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
		{
			fixed (byte* ptr = &destBuffer[offset])
			{
				fixed (byte* ptr3 = &sourceBuffer[0])
				{
					float* ptr2 = (float*)ptr;
					float* ptr4 = (float*)ptr3;
					int num = bytesRead / 4;
					for (int i = 0; i < num; i++)
					{
						ptr2[i] += ptr4[i];
					}
				}
			}
		}
	}
	public class MonoToStereoProvider16 : IWaveProvider
	{
		private readonly IWaveProvider sourceProvider;

		private byte[] sourceBuffer;

		public float LeftVolume { get; set; }

		public float RightVolume { get; set; }

		public WaveFormat WaveFormat { get; }

		public MonoToStereoProvider16(IWaveProvider sourceProvider)
		{
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Source must be PCM");
			}
			if (sourceProvider.WaveFormat.Channels != 1)
			{
				throw new ArgumentException("Source must be Mono");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 16)
			{
				throw new ArgumentException("Source must be 16 bit");
			}
			this.sourceProvider = sourceProvider;
			WaveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 2);
			RightVolume = 1f;
			LeftVolume = 1f;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = count / 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
			WaveBuffer waveBuffer2 = new WaveBuffer(buffer);
			int num2 = sourceProvider.Read(sourceBuffer, 0, num) / 2;
			int num3 = offset / 2;
			for (int i = 0; i < num2; i++)
			{
				short num4 = waveBuffer.ShortBuffer[i];
				waveBuffer2.ShortBuffer[num3++] = (short)(LeftVolume * (float)num4);
				waveBuffer2.ShortBuffer[num3++] = (short)(RightVolume * (float)num4);
			}
			return num2 * 4;
		}
	}
	public class MultiplexingWaveProvider : IWaveProvider
	{
		private readonly IList<IWaveProvider> inputs;

		private readonly int outputChannelCount;

		private readonly int inputChannelCount;

		private readonly List<int> mappings;

		private readonly int bytesPerSample;

		private byte[] inputBuffer;

		public WaveFormat WaveFormat { get; }

		public int InputChannelCount => inputChannelCount;

		public int OutputChannelCount => outputChannelCount;

		public MultiplexingWaveProvider(IEnumerable<IWaveProvider> inputs)
			: this(inputs, -1)
		{
		}

		public MultiplexingWaveProvider(IEnumerable<IWaveProvider> inputs, int numberOfOutputChannels)
		{
			this.inputs = new List<IWaveProvider>(inputs);
			outputChannelCount = ((numberOfOutputChannels == -1) ? this.inputs.Sum((IWaveProvider i) => i.WaveFormat.Channels) : numberOfOutputChannels);
			if (this.inputs.Count == 0)
			{
				throw new ArgumentException("You must provide at least one input");
			}
			if (outputChannelCount < 1)
			{
				throw new ArgumentException("You must provide at least one output");
			}
			foreach (IWaveProvider input in this.inputs)
			{
				if (WaveFormat == null)
				{
					if (input.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
					{
						WaveFormat = new WaveFormat(input.WaveFormat.SampleRate, input.WaveFormat.BitsPerSample, outputChannelCount);
					}
					else
					{
						if (input.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
						{
							throw new ArgumentException("Only PCM and 32 bit float are supported");
						}
						WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(input.WaveFormat.SampleRate, outputChannelCount);
					}
				}
				else
				{
					if (input.WaveFormat.BitsPerSample != WaveFormat.BitsPerSample)
					{
						throw new ArgumentException("All inputs must have the same bit depth");
					}
					if (input.WaveFormat.SampleRate != WaveFormat.SampleRate)
					{
						throw new ArgumentException("All inputs must have the same sample rate");
					}
				}
				inputChannelCount += input.WaveFormat.Channels;
			}
			bytesPerSample = WaveFormat.BitsPerSample / 8;
			mappings = new List<int>();
			for (int j = 0; j < outputChannelCount; j++)
			{
				mappings.Add(j % inputChannelCount);
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = bytesPerSample * outputChannelCount;
			int num2 = count / num;
			int num3 = 0;
			int num4 = 0;
			foreach (IWaveProvider input in inputs)
			{
				int num5 = bytesPerSample * input.WaveFormat.Channels;
				int num6 = num2 * num5;
				inputBuffer = BufferHelpers.Ensure(inputBuffer, num6);
				int num7 = input.Read(inputBuffer, 0, num6);
				num4 = Math.Max(num4, num7 / num5);
				for (int i = 0; i < input.WaveFormat.Channels; i++)
				{
					int num8 = num3 + i;
					for (int j = 0; j < outputChannelCount; j++)
					{
						if (mappings[j] != num8)
						{
							continue;
						}
						int num9 = i * bytesPerSample;
						int num10 = offset + j * bytesPerSample;
						int k;
						for (k = 0; k < num2; k++)
						{
							if (num9 >= num7)
							{
								break;
							}
							Array.Copy(inputBuffer, num9, buffer, num10, bytesPerSample);
							num10 += num;
							num9 += num5;
						}
						for (; k < num2; k++)
						{
							Array.Clear(buffer, num10, bytesPerSample);
							num10 += num;
						}
					}
				}
				num3 += input.WaveFormat.Channels;
			}
			return num4 * num;
		}

		public void ConnectInputToOutput(int inputChannel, int outputChannel)
		{
			if (inputChannel < 0 || inputChannel >= InputChannelCount)
			{
				throw new ArgumentException("Invalid input channel");
			}
			if (outputChannel < 0 || outputChannel >= OutputChannelCount)
			{
				throw new ArgumentException("Invalid output channel");
			}
			mappings[outputChannel] = inputChannel;
		}
	}
	public class SilenceProvider : IWaveProvider
	{
		public WaveFormat WaveFormat { get; private set; }

		public SilenceProvider(WaveFormat wf)
		{
			WaveFormat = wf;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			Array.Clear(buffer, offset, count);
			return count;
		}
	}
	public class StereoToMonoProvider16 : IWaveProvider
	{
		private readonly IWaveProvider sourceProvider;

		private byte[] sourceBuffer;

		public float LeftVolume { get; set; }

		public float RightVolume { get; set; }

		public WaveFormat WaveFormat { get; private set; }

		public StereoToMonoProvider16(IWaveProvider sourceProvider)
		{
			LeftVolume = 0.5f;
			RightVolume = 0.5f;
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Source must be PCM");
			}
			if (sourceProvider.WaveFormat.Channels != 2)
			{
				throw new ArgumentException("Source must be stereo");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 16)
			{
				throw new ArgumentException("Source must be 16 bit");
			}
			this.sourceProvider = sourceProvider;
			WaveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 1);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = count * 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
			WaveBuffer waveBuffer2 = new WaveBuffer(buffer);
			int num2 = sourceProvider.Read(sourceBuffer, 0, num);
			int num3 = num2 / 2;
			int num4 = offset / 2;
			for (int i = 0; i < num3; i += 2)
			{
				short num5 = waveBuffer.ShortBuffer[i];
				short num6 = waveBuffer.ShortBuffer[i + 1];
				float num7 = (float)num5 * LeftVolume + (float)num6 * RightVolume;
				if (num7 > 32767f)
				{
					num7 = 32767f;
				}
				if (num7 < -32768f)
				{
					num7 = -32768f;
				}
				waveBuffer2.ShortBuffer[num4++] = (short)num7;
			}
			return num2 / 2;
		}
	}
	public class VolumeWaveProvider16 : IWaveProvider
	{
		private readonly IWaveProvider sourceProvider;

		private float volume;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public WaveFormat WaveFormat => sourceProvider.WaveFormat;

		public VolumeWaveProvider16(IWaveProvider sourceProvider)
		{
			Volume = 1f;
			this.sourceProvider = sourceProvider;
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Expecting PCM input");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 16)
			{
				throw new ArgumentException("Expecting 16 bit");
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = sourceProvider.Read(buffer, offset, count);
			if (volume == 0f)
			{
				for (int i = 0; i < num; i++)
				{
					buffer[offset++] = 0;
				}
			}
			else if (volume != 1f)
			{
				for (int j = 0; j < num; j += 2)
				{
					short num2 = (short)((buffer[offset + 1] << 8) | buffer[offset]);
					float num3 = (float)num2 * volume;
					num2 = (short)num3;
					if (Volume > 1f)
					{
						if (num3 > 32767f)
						{
							num2 = short.MaxValue;
						}
						else if (num3 < -32768f)
						{
							num2 = short.MinValue;
						}
					}
					buffer[offset++] = (byte)((uint)num2 & 0xFFu);
					buffer[offset++] = (byte)(num2 >> 8);
				}
			}
			return num;
		}
	}
	public class Wave16ToFloatProvider : IWaveProvider
	{
		private IWaveProvider sourceProvider;

		private readonly WaveFormat waveFormat;

		private volatile float volume;

		private byte[] sourceBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public Wave16ToFloatProvider(IWaveProvider sourceProvider)
		{
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Only PCM supported");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 16)
			{
				throw new ArgumentException("Only 16 bit audio supported");
			}
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, sourceProvider.WaveFormat.Channels);
			this.sourceProvider = sourceProvider;
			volume = 1f;
		}

		public int Read(byte[] destBuffer, int offset, int numBytes)
		{
			int num = numBytes / 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			int num2 = sourceProvider.Read(sourceBuffer, offset, num);
			WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
			WaveBuffer waveBuffer2 = new WaveBuffer(destBuffer);
			int num3 = num2 / 2;
			int num4 = offset / 4;
			for (int i = 0; i < num3; i++)
			{
				waveBuffer2.FloatBuffer[num4++] = (float)waveBuffer.ShortBuffer[i] / 32768f * volume;
			}
			return num3 * 4;
		}
	}
	public class WaveFloatTo16Provider : IWaveProvider
	{
		private readonly IWaveProvider sourceProvider;

		private readonly WaveFormat waveFormat;

		private volatile float volume;

		private byte[] sourceBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public WaveFloatTo16Provider(IWaveProvider sourceProvider)
		{
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Input wave provider must be IEEE float", "sourceProvider");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Input wave provider must be 32 bit", "sourceProvider");
			}
			waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);
			this.sourceProvider = sourceProvider;
			volume = 1f;
		}

		public int Read(byte[] destBuffer, int offset, int numBytes)
		{
			int num = numBytes * 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			int num2 = sourceProvider.Read(sourceBuffer, 0, num);
			WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
			WaveBuffer waveBuffer2 = new WaveBuffer(destBuffer);
			int num3 = num2 / 4;
			int num4 = offset / 2;
			for (int i = 0; i < num3; i++)
			{
				float num5 = waveBuffer.FloatBuffer[i] * volume;
				if (num5 > 1f)
				{
					num5 = 1f;
				}
				if (num5 < -1f)
				{
					num5 = -1f;
				}
				waveBuffer2.ShortBuffer[num4++] = (short)(num5 * 32767f);
			}
			return num3 * 2;
		}
	}
	public class WaveInProvider : IWaveProvider
	{
		private readonly IWaveIn waveIn;

		private readonly BufferedWaveProvider bufferedWaveProvider;

		public WaveFormat WaveFormat => waveIn.WaveFormat;

		public WaveInProvider(IWaveIn waveIn)
		{
			this.waveIn = waveIn;
			waveIn.DataAvailable += OnDataAvailable;
			bufferedWaveProvider = new BufferedWaveProvider(WaveFormat);
		}

		private void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			return bufferedWaveProvider.Read(buffer, offset, count);
		}
	}
	public abstract class WaveProvider16 : IWaveProvider
	{
		private WaveFormat waveFormat;

		public WaveFormat WaveFormat => waveFormat;

		public WaveProvider16()
			: this(44100, 1)
		{
		}

		public WaveProvider16(int sampleRate, int channels)
		{
			SetWaveFormat(sampleRate, channels);
		}

		public void SetWaveFormat(int sampleRate, int channels)
		{
			waveFormat = new WaveFormat(sampleRate, 16, channels);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			WaveBuffer waveBuffer = new WaveBuffer(buffer);
			int sampleCount = count / 2;
			return Read(waveBuffer.ShortBuffer, offset / 2, sampleCount) * 2;
		}

		public abstract int Read(short[] buffer, int offset, int sampleCount);
	}
	public abstract class WaveProvider32 : IWaveProvider, ISampleProvider
	{
		private WaveFormat waveFormat;

		public WaveFormat WaveFormat => waveFormat;

		public WaveProvider32()
			: this(44100, 1)
		{
		}

		public WaveProvider32(int sampleRate, int channels)
		{
			SetWaveFormat(sampleRate, channels);
		}

		public void SetWaveFormat(int sampleRate, int channels)
		{
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			WaveBuffer waveBuffer = new WaveBuffer(buffer);
			int sampleCount = count / 4;
			return Read(waveBuffer.FloatBuffer, offset / 4, sampleCount) * 4;
		}

		public abstract int Read(float[] buffer, int offset, int sampleCount);
	}
	public class WaveRecorder : IWaveProvider, IDisposable
	{
		private WaveFileWriter writer;

		private IWaveProvider source;

		public WaveFormat WaveFormat => source.WaveFormat;

		public WaveRecorder(IWaveProvider source, string destination)
		{
			this.source = source;
			writer = new WaveFileWriter(destination, source.WaveFormat);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = source.Read(buffer, offset, count);
			writer.Write(buffer, offset, num);
			return num;
		}

		public void Dispose()
		{
			if (writer != null)
			{
				writer.Dispose();
				writer = null;
			}
		}
	}
	public class AiffFileReader : WaveStream
	{
		public struct AiffChunk
		{
			public string ChunkName;

			public uint ChunkLength;

			public uint ChunkStart;

			public AiffChunk(uint start, string name, uint length)
			{
				ChunkStart = start;
				ChunkName = name;
				ChunkLength = length + ((length % 2u == 1) ? 1u : 0u);
			}
		}

		private readonly WaveFormat waveFormat;

		private readonly bool ownInput;

		private readonly long dataPosition;

		private readonly int dataChunkLength;

		private readonly List<AiffChunk> chunks = new List<AiffChunk>();

		private Stream waveStream;

		private readonly object lockObject = new object();

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => dataChunkLength;

		public long SampleCount
		{
			get
			{
				if (waveFormat.Encoding == WaveFormatEncoding.Pcm || waveFormat.Encoding == WaveFormatEncoding.Extensible || waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
				{
					return dataChunkLength / BlockAlign;
				}
				throw new FormatException("Sample count is calculated only for the standard encodings");
			}
		}

		public override long Position
		{
			get
			{
				return waveStream.Position - dataPosition;
			}
			set
			{
				lock (lockObject)
				{
					value = Math.Min(value, Length);
					value -= value % waveFormat.BlockAlign;
					waveStream.Position = value + dataPosition;
				}
			}
		}

		public AiffFileReader(string aiffFile)
			: this(File.OpenRead(aiffFile))
		{
			ownInput = true;
		}

		public AiffFileReader(Stream inputStream)
		{
			waveStream = inputStream;
			ReadAiffHeader(waveStream, out waveFormat, out dataPosition, out dataChunkLength, chunks);
			Position = 0L;
		}

		public static void ReadAiffHeader(Stream stream, out WaveFormat format, out long dataChunkPosition, out int dataChunkLength, List<AiffChunk> chunks)
		{
			dataChunkPosition = -1L;
			format = null;
			BinaryReader binaryReader = new BinaryReader(stream);
			if (ReadChunkName(binaryReader) != "FORM")
			{
				throw new FormatException("Not an AIFF file - no FORM header.");
			}
			ConvertInt(binaryReader.ReadBytes(4));
			string text = ReadChunkName(binaryReader);
			if (text != "AIFC" && text != "AIFF")
			{
				throw new FormatException("Not an AIFF file - no AIFF/AIFC header.");
			}
			dataChunkLength = 0;
			while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
			{
				AiffChunk item = ReadChunkHeader(binaryReader);
				if (item.ChunkName == "\0\0\0\0" || binaryReader.BaseStream.Position + item.ChunkLength > binaryReader.BaseStream.Length)
				{
					break;
				}
				if (item.ChunkName == "COMM")
				{
					short channels = ConvertShort(binaryReader.ReadBytes(2));
					ConvertInt(binaryReader.ReadBytes(4));
					short bits = ConvertShort(binaryReader.ReadBytes(2));
					double num = IEEE.ConvertFromIeeeExtended(binaryReader.ReadBytes(10));
					format = new WaveFormat((int)num, bits, channels);
					if (item.ChunkLength > 18 && text == "AIFC")
					{
						if (new string(binaryReader.ReadChars(4)).ToLower() != "none")
						{
							throw new FormatException("Compressed AIFC is not supported.");
						}
						binaryReader.ReadBytes((int)(item.ChunkLength - 22));
					}
					else
					{
						binaryReader.ReadBytes((int)(item.ChunkLength - 18));
					}
				}
				else if (item.ChunkName == "SSND")
				{
					uint num2 = ConvertInt(binaryReader.ReadBytes(4));
					ConvertInt(binaryReader.ReadBytes(4));
					dataChunkPosition = item.ChunkStart + 16 + num2;
					dataChunkLength = (int)(item.ChunkLength - 8);
					binaryReader.BaseStream.Position += item.ChunkLength - 8;
				}
				else
				{
					chunks?.Add(item);
					binaryReader.BaseStream.Position += item.ChunkLength;
				}
			}
			if (format == null)
			{
				throw new FormatException("Invalid AIFF file - No COMM chunk found.");
			}
			if (dataChunkPosition == -1)
			{
				throw new FormatException("Invalid AIFF file - No SSND chunk found.");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && waveStream != null)
			{
				if (ownInput)
				{
					waveStream.Close();
				}
				waveStream = null;
			}
			base.Dispose(disposing);
		}

		public override int Read(byte[] array, int offset, int count)
		{
			if (count % waveFormat.BlockAlign != 0)
			{
				throw new ArgumentException($"Must read complete blocks: requested {count}, block align is {WaveFormat.BlockAlign}");
			}
			lock (lockObject)
			{
				if (Position + count > dataChunkLength)
				{
					count = dataChunkLength - (int)Position;
				}
				byte[] array2 = new byte[count];
				int num = waveStream.Read(array2, offset, count);
				int num2 = WaveFormat.BitsPerSample / 8;
				for (int i = 0; i < num; i += num2)
				{
					if (WaveFormat.BitsPerSample == 8)
					{
						array[i] = array2[i];
						continue;
					}
					if (WaveFormat.BitsPerSample == 16)
					{
						array[i] = array2[i + 1];
						array[i + 1] = array2[i];
						continue;
					}
					if (WaveFormat.BitsPerSample == 24)
					{
						array[i] = array2[i + 2];
						array[i + 1] = array2[i + 1];
						array[i + 2] = array2[i];
						continue;
					}
					if (WaveFormat.BitsPerSample == 32)
					{
						array[i] = array2[i + 3];
						array[i + 1] = array2[i + 2];
						array[i + 2] = array2[i + 1];
						array[i + 3] = array2[i];
						continue;
					}
					throw new FormatException("Unsupported PCM format.");
				}
				return num;
			}
		}

		private static uint ConvertInt(byte[] buffer)
		{
			if (buffer.Length != 4)
			{
				throw new Exception("Incorrect length for long.");
			}
			return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
		}

		private static short ConvertShort(byte[] buffer)
		{
			if (buffer.Length != 2)
			{
				throw new Exception("Incorrect length for int.");
			}
			return (short)((buffer[0] << 8) | buffer[1]);
		}

		private static AiffChunk ReadChunkHeader(BinaryReader br)
		{
			return new AiffChunk((uint)br.BaseStream.Position, ReadChunkName(br), ConvertInt(br.ReadBytes(4)));
		}

		private static string ReadChunkName(BinaryReader br)
		{
			return new string(br.ReadChars(4));
		}
	}
	public class AudioFileReader : WaveStream, ISampleProvider
	{
		private WaveStream readerStream;

		private readonly SampleChannel sampleChannel;

		private readonly int destBytesPerSample;

		private readonly int sourceBytesPerSample;

		private readonly long length;

		private readonly object lockObject;

		public string FileName { get; }

		public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return SourceToDest(readerStream.Position);
			}
			set
			{
				lock (lockObject)
				{
					readerStream.Position = DestToSource(value);
				}
			}
		}

		public float Volume
		{
			get
			{
				return sampleChannel.Volume;
			}
			set
			{
				sampleChannel.Volume = value;
			}
		}

		public AudioFileReader(string fileName)
		{
			lockObject = new object();
			FileName = fileName;
			CreateReaderStream(fileName);
			sourceBytesPerSample = readerStream.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
			sampleChannel = new SampleChannel(readerStream, forceStereo: false);
			destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
			length = SourceToDest(readerStream.Length);
		}

		private void CreateReaderStream(string fileName)
		{
			if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
			{
				readerStream = new WaveFileReader(fileName);
				if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
				{
					readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
					readerStream = new BlockAlignReductionStream(readerStream);
				}
			}
			else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
			{
				readerStream = new Mp3FileReader(fileName);
			}
			else if (fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase))
			{
				readerStream = new AiffFileReader(fileName);
			}
			else
			{
				readerStream = new MediaFoundationReader(fileName);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			WaveBuffer waveBuffer = new WaveBuffer(buffer);
			int count2 = count / 4;
			return Read(waveBuffer.FloatBuffer, offset / 4, count2) * 4;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			lock (lockObject)
			{
				return sampleChannel.Read(buffer, offset, count);
			}
		}

		private long SourceToDest(long sourceBytes)
		{
			return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
		}

		private long DestToSource(long destBytes)
		{
			return sourceBytesPerSample * (destBytes / destBytesPerSample);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && readerStream != null)
			{
				readerStream.Dispose();
				readerStream = null;
			}
			base.Dispose(disposing);
		}
	}
	public class BlockAlignReductionStream : WaveStream
	{
		private WaveStream sourceStream;

		private long position;

		private readonly CircularBuffer circularBuffer;

		private long bufferStartPosition;

		private byte[] sourceBuffer;

		private readonly object lockObject = new object();

		public override int BlockAlign => WaveFormat.BitsPerSample / 8 * WaveFormat.Channels;

		public override WaveFormat WaveFormat => sourceStream.WaveFormat;

		public override long Length => sourceStream.Length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (lockObject)
				{
					if (position != value)
					{
						if (position % BlockAlign != 0L)
						{
							throw new ArgumentException("Position must be block aligned");
						}
						long num = value - value % sourceStream.BlockAlign;
						if (sourceStream.Position != num)
						{
							sourceStream.Position = num;
							circularBuffer.Reset();
							bufferStartPosition = sourceStream.Position;
						}
						position = value;
					}
				}
			}
		}

		private long BufferEndPosition => bufferStartPosition + circularBuffer.Count;

		public BlockAlignReductionStream(WaveStream sourceStream)
		{
			this.sourceStream = sourceStream;
			circularBuffer = new CircularBuffer(sourceStream.WaveFormat.AverageBytesPerSecond * 4);
		}

		private byte[] GetSourceBuffer(int size)
		{
			if (sourceBuffer == null || sourceBuffer.Length < size)
			{
				sourceBuffer = new byte[size * 2];
			}
			return sourceBuffer;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && sourceStream != null)
			{
				sourceStream.Dispose();
				sourceStream = null;
			}
			base.Dispose(disposing);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			lock (lockObject)
			{
				while (BufferEndPosition < position + count)
				{
					int num = count;
					if (num % sourceStream.BlockAlign != 0)
					{
						num = count + sourceStream.BlockAlign - count % sourceStream.BlockAlign;
					}
					int num2 = sourceStream.Read(GetSourceBuffer(num), 0, num);
					circularBuffer.Write(GetSourceBuffer(num), 0, num2);
					if (num2 == 0)
					{
						break;
					}
				}
				if (bufferStartPosition < position)
				{
					circularBuffer.Advance((int)(position - bufferStartPosition));
					bufferStartPosition = position;
				}
				int num3 = circularBuffer.Read(buffer, offset, count);
				position += num3;
				bufferStartPosition = position;
				return num3;
			}
		}
	}
	internal class ComStream : Stream, IStream
	{
		private Stream stream;

		public override bool CanRead => stream.CanRead;

		public override bool CanSeek => stream.CanSeek;

		public override bool CanWrite => stream.CanWrite;

		public override long Length => stream.Length;

		public override long Position
		{
			get
			{
				return stream.Position;
			}
			set
			{
				stream.Position = value;
			}
		}

		public ComStream(Stream stream)
			: this(stream, synchronizeStream: true)
		{
		}

		internal ComStream(Stream stream, bool synchronizeStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (synchronizeStream)
			{
				stream = Stream.Synchronized(stream);
			}
			this.stream = stream;
		}

		void IStream.Clone(out IStream ppstm)
		{
			ppstm = null;
		}

		void IStream.Commit(int grfCommitFlags)
		{
			stream.Flush();
		}

		void IStream.CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
		{
		}

		void IStream.LockRegion(long libOffset, long cb, int dwLockType)
		{
		}

		void IStream.Read(byte[] pv, int cb, IntPtr pcbRead)
		{
			if (!CanRead)
			{
				throw new InvalidOperationException("Stream is not readable.");
			}
			int val = Read(pv, 0, cb);
			if (pcbRead != IntPtr.Zero)
			{
				Marshal.WriteInt32(pcbRead, val);
			}
		}

		void IStream.Revert()
		{
		}

		void IStream.Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
			long val = Seek(dlibMove, (SeekOrigin)dwOrigin);
			if (plibNewPosition != IntPtr.Zero)
			{
				Marshal.WriteInt64(plibNewPosition, val);
			}
		}

		void IStream.SetSize(long libNewSize)
		{
			SetLength(libNewSize);
		}

		void IStream.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
		{
			System.Runtime.InteropServices.ComTypes.STATSTG sTATSTG = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			sTATSTG.type = 2;
			sTATSTG.cbSize = Length;
			sTATSTG.grfMode = 0;
			System.Runtime.InteropServices.ComTypes.STATSTG sTATSTG2 = sTATSTG;
			if (CanWrite && CanRead)
			{
				sTATSTG2.grfMode |= 2;
			}
			else if (CanRead)
			{
				sTATSTG2.grfMode |= 0;
			}
			else
			{
				if (!CanWrite)
				{
					throw new ObjectDisposedException("Stream");
				}
				sTATSTG2.grfMode |= 1;
			}
			pstatstg = sTATSTG2;
		}

		void IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
		{
		}

		void IStream.Write(byte[] pv, int cb, IntPtr pcbWritten)
		{
			if (!CanWrite)
			{
				throw new InvalidOperationException("Stream is not writeable.");
			}
			Write(pv, 0, cb);
			if (pcbWritten != IntPtr.Zero)
			{
				Marshal.WriteInt32(pcbWritten, cb);
			}
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return stream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}
		}

		public override void Close()
		{
			base.Close();
			if (stream != null)
			{
				stream.Close();
				stream = null;
			}
		}
	}
	public class Cue
	{
		public int Position { get; }

		public string Label { get; }

		public Cue(int position, string label)
		{
			Position = position;
			if (label == null)
			{
				label = "";
			}
			Label = Regex.Replace(label, "[^\\u0000-\\u00FF]", "");
		}
	}
	public class CueList
	{
		private readonly List<Cue> cues = new List<Cue>();

		public int[] CuePositions
		{
			get
			{
				int[] array = new int[cues.Count];
				for (int i = 0; i < cues.Count; i++)
				{
					array[i] = cues[i].Position;
				}
				return array;
			}
		}

		public string[] CueLabels
		{
			get
			{
				string[] array = new string[cues.Count];
				for (int i = 0; i < cues.Count; i++)
				{
					array[i] = cues[i].Label;
				}
				return array;
			}
		}

		public int Count => cues.Count;

		public Cue this[int index] => cues[index];

		public CueList()
		{
		}

		public void Add(Cue cue)
		{
			cues.Add(cue);
		}

		internal CueList(byte[] cueChunkData, byte[] listChunkData)
		{
			int num = BitConverter.ToInt32(cueChunkData, 0);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int[] array = new int[num];
			int num2 = 0;
			int num3 = 4;
			while (cueChunkData.Length - num3 >= 24)
			{
				dictionary[BitConverter.ToInt32(cueChunkData, num3)] = num2;
				array[num2] = BitConverter.ToInt32(cueChunkData, num3 + 20);
				num3 += 24;
				num2++;
			}
			string[] array2 = new string[num];
			int num4 = 0;
			int num5 = ChunkIdentifier.ChunkIdentifierToInt32("labl");
			for (int i = 4; listChunkData.Length - i >= 16; i += num4 + num4 % 2 + 12)
			{
				if (BitConverter.ToInt32(listChunkData, i) == num5)
				{
					num4 = BitConverter.ToInt32(listChunkData, i + 4) - 4;
					int key = BitConverter.ToInt32(listChunkData, i + 8);
					num2 = dictionary[key];
					array2[num2] = Encoding.UTF8.GetString(listChunkData, i + 12, num4 - 1);
				}
			}
			for (int j = 0; j < num; j++)
			{
				cues.Add(new Cue(array[j], array2[j]));
			}
		}

		internal byte[] GetRiffChunks()
		{
			if (Count == 0)
			{
				return null;
			}
			int num = 12 + 24 * Count;
			int num2 = 12;
			for (int i = 0; i < Count; i++)
			{
				int num3 = this[i].Label.Length + 1;
				num2 += num3 + num3 % 2 + 12;
			}
			byte[] array = new byte[num + num2];
			int value = ChunkIdentifier.ChunkIdentifierToInt32("cue ");
			int value2 = ChunkIdentifier.ChunkIdentifierToInt32("data");
			int value3 = ChunkIdentifier.ChunkIdentifierToInt32("LIST");
			int value4 = ChunkIdentifier.ChunkIdentifierToInt32("adtl");
			int value5 = ChunkIdentifier.ChunkIdentifierToInt32("labl");
			using MemoryStream output = new MemoryStream(array);
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(value);
			binaryWriter.Write(num - 8);
			binaryWriter.Write(Count);
			for (int j = 0; j < Count; j++)
			{
				int position = this[j].Position;
				binaryWriter.Write(j);
				binaryWriter.Write(position);
				binaryWriter.Write(value2);
				binaryWriter.Seek(8, SeekOrigin.Current);
				binaryWriter.Write(position);
			}
			binaryWriter.Write(value3);
			binaryWriter.Write(num2 - 8);
			binaryWriter.Write(value4);
			for (int k = 0; k < Count; k++)
			{
				binaryWriter.Write(value5);
				binaryWriter.Write(this[k].Label.Length + 1 + 4);
				binaryWriter.Write(k);
				binaryWriter.Write(Encoding.UTF8.GetBytes(this[k].Label.ToCharArray()));
				if (this[k].Label.Length % 2 == 0)
				{
					binaryWriter.Seek(2, SeekOrigin.Current);
				}
				else
				{
					binaryWriter.Seek(1, SeekOrigin.Current);
				}
			}
			return array;
		}

		internal static CueList FromChunks(WaveFileReader reader)
		{
			CueList result = null;
			byte[] array = null;
			byte[] array2 = null;
			foreach (RiffChunk extraChunk in reader.ExtraChunks)
			{
				if (extraChunk.IdentifierAsString.ToLower() == "cue ")
				{
					array = reader.GetChunkData(extraChunk);
				}
				else if (extraChunk.IdentifierAsString.ToLower() == "list")
				{
					array2 = reader.GetChunkData(extraChunk);
				}
			}
			if (array != null && array2 != null)
			{
				result = new CueList(array, array2);
			}
			return result;
		}
	}
	public class CueWaveFileReader : WaveFileReader
	{
		private CueList cues;

		public CueList Cues
		{
			get
			{
				if (cues == null)
				{
					cues = CueList.FromChunks(this);
				}
				return cues;
			}
		}

		public CueWaveFileReader(string fileName)
			: base(fileName)
		{
		}

		public CueWaveFileReader(Stream inputStream)
			: base(inputStream)
		{
		}
	}
	public interface ISampleNotifier
	{
		event EventHandler<SampleEventArgs> Sample;
	}
	public class SampleEventArgs : EventArgs
	{
		public float Left { get; set; }

		public float Right { get; set; }

		public SampleEventArgs(float left, float right)
		{
			Left = left;
			Right = right;
		}
	}
	public class MediaFoundationReader : WaveStream
	{
		public class MediaFoundationReaderSettings
		{
			public bool RequestFloatOutput { get; set; }

			public bool SingleReaderObject { get; set; }

			public bool RepositionInRead { get; set; }

			public MediaFoundationReaderSettings()
			{
				RepositionInRead = true;
			}
		}

		private WaveFormat waveFormat;

		private long length;

		private MediaFoundationReaderSettings settings;

		private readonly string file;

		private IMFSourceReader pReader;

		private long position;

		private byte[] decoderOutputBuffer;

		private int decoderOutputOffset;

		private int decoderOutputCount;

		private long repositionTo = -1L;

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Position cannot be less than 0");
				}
				if (settings.RepositionInRead)
				{
					repositionTo = value;
					position = value;
				}
				else
				{
					Reposition(value);
				}
			}
		}

		public event EventHandler WaveFormatChanged;

		protected MediaFoundationReader()
		{
		}

		public MediaFoundationReader(string file)
			: this(file, null)
		{
		}

		public MediaFoundationReader(string file, MediaFoundationReaderSettings settings)
		{
			this.file = file;
			Init(settings);
		}

		protected void Init(MediaFoundationReaderSettings initialSettings)
		{
			MediaFoundationApi.Startup();
			settings = initialSettings ?? new MediaFoundationReaderSettings();
			IMFSourceReader iMFSourceReader = CreateReader(settings);
			waveFormat = GetCurrentWaveFormat(iMFSourceReader);
			iMFSourceReader.SetStreamSelection(-3, pSelected: true);
			length = GetLength(iMFSourceReader);
			if (settings.SingleReaderObject)
			{
				pReader = iMFSourceReader;
			}
			else
			{
				Marshal.ReleaseComObject(iMFSourceReader);
			}
		}

		private WaveFormat GetCurrentWaveFormat(IMFSourceReader reader)
		{
			reader.GetCurrentMediaType(-3, out var ppMediaType);
			MediaType mediaType = new MediaType(ppMediaType);
			_ = mediaType.MajorType;
			Guid subType = mediaType.SubType;
			int channelCount = mediaType.ChannelCount;
			int bitsPerSample = mediaType.BitsPerSample;
			int sampleRate = mediaType.SampleRate;
			if (subType == AudioSubtypes.MFAudioFormat_PCM)
			{
				return new WaveFormat(sampleRate, bitsPerSample, channelCount);
			}
			if (subType == AudioSubtypes.MFAudioFormat_Float)
			{
				return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
			}
			string arg = FieldDescriptionHelper.Describe(typeof(AudioSubtypes), subType);
			throw new InvalidDataException($"Unsupported audio sub Type {arg}");
		}

		private static MediaType GetCurrentMediaType(IMFSourceReader reader)
		{
			reader.GetCurrentMediaType(-3, out var ppMediaType);
			return new MediaType(ppMediaType);
		}

		protected virtual IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
		{
			MediaFoundationInterop.MFCreateSourceReaderFromURL(file, null, out var ppSourceReader);
			ppSourceReader.SetStreamSelection(-2, pSelected: false);
			ppSourceReader.SetStreamSelection(-3, pSelected: true);
			MediaType mediaType = new MediaType();
			mediaType.MajorType = NAudio.MediaFoundation.MediaTypes.MFMediaType_Audio;
			mediaType.SubType = (settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM);
			MediaType currentMediaType = GetCurrentMediaType(ppSourceReader);
			mediaType.ChannelCount = currentMediaType.ChannelCount;
			mediaType.SampleRate = currentMediaType.SampleRate;
			try
			{
				ppSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, mediaType.MediaFoundationObject);
			}
			catch (COMException exception) when (exception.GetHResult() == -1072875852)
			{
				if (!(currentMediaType.SubType == AudioSubtypes.MFAudioFormat_AAC) || currentMediaType.ChannelCount != 1)
				{
					throw;
				}
				mediaType.SampleRate = (currentMediaType.SampleRate *= 2);
				mediaType.ChannelCount = (currentMediaType.ChannelCount *= 2);
				ppSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, mediaType.MediaFoundationObject);
			}
			Marshal.ReleaseComObject(currentMediaType.MediaFoundationObject);
			return ppSourceReader;
		}

		private long GetLength(IMFSourceReader reader)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(MarshalHelpers.SizeOf<PropVariant>());
			try
			{
				int presentationAttribute = reader.GetPresentationAttribute(-1, MediaFoundationAttributes.MF_PD_DURATION, intPtr);
				switch (presentationAttribute)
				{
				case -1072875802:
					return 0L;
				default:
					Marshal.ThrowExceptionForHR(presentationAttribute);
					break;
				case 0:
					break;
				}
				return (long)MarshalHelpers.PtrToStructure<PropVariant>(intPtr).Value * waveFormat.AverageBytesPerSecond / 10000000;
			}
			finally
			{
				PropVariant.Clear(intPtr);
				Marshal.FreeHGlobal(intPtr);
			}
		}

		private void EnsureBuffer(int bytesRequired)
		{
			if (decoderOutputBuffer == null || decoderOutputBuffer.Length < bytesRequired)
			{
				decoderOutputBuffer = new byte[bytesRequired];
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (pReader == null)
			{
				pReader = CreateReader(settings);
			}
			if (repositionTo != -1)
			{
				Reposition(repositionTo);
			}
			int num = 0;
			if (decoderOutputCount > 0)
			{
				num += ReadFromDecoderBuffer(buffer, offset, count - num);
			}
			while (num < count)
			{
				pReader.ReadSample(-3, 0, out var _, out var pdwStreamFlags, out var _, out var ppSample);
				if ((pdwStreamFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_ENDOFSTREAM) != 0)
				{
					break;
				}
				if ((pdwStreamFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED) != 0)
				{
					waveFormat = GetCurrentWaveFormat(pReader);
					OnWaveFormatChanged();
				}
				else if (pdwStreamFlags != 0)
				{
					throw new InvalidOperationException($"MediaFoundationReadError {pdwStreamFlags}");
				}
				ppSample.ConvertToContiguousBuffer(out var ppBuffer);
				ppBuffer.Lock(out var ppbBuffer, out var _, out var pcbCurrentLength);
				EnsureBuffer(pcbCurrentLength);
				Marshal.Copy(ppbBuffer, decoderOutputBuffer, 0, pcbCurrentLength);
				decoderOutputOffset = 0;
				decoderOutputCount = pcbCurrentLength;
				num += ReadFromDecoderBuffer(buffer, offset + num, count - num);
				ppBuffer.Unlock();
				Marshal.ReleaseComObject(ppBuffer);
				Marshal.ReleaseComObject(ppSample);
			}
			position += num;
			return num;
		}

		private int ReadFromDecoderBuffer(byte[] buffer, int offset, int needed)
		{
			int num = Math.Min(needed, decoderOutputCount);
			Array.Copy(decoderOutputBuffer, decoderOutputOffset, buffer, offset, num);
			decoderOutputOffset += num;
			decoderOutputCount -= num;
			if (decoderOutputCount == 0)
			{
				decoderOutputOffset = 0;
			}
			return num;
		}

		private void Reposition(long desiredPosition)
		{
			PropVariant propVariant = PropVariant.FromLong(10000000 * repositionTo / waveFormat.AverageBytesPerSecond);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(propVariant));
			Marshal.StructureToPtr(propVariant, intPtr, fDeleteOld: false);
			pReader.SetCurrentPosition(Guid.Empty, intPtr);
			Marshal.FreeHGlobal(intPtr);
			decoderOutputCount = 0;
			decoderOutputOffset = 0;
			position = desiredPosition;
			repositionTo = -1L;
		}

		protected override void Dispose(bool disposing)
		{
			if (pReader != null)
			{
				Marshal.ReleaseComObject(pReader);
				pReader = null;
			}
			base.Dispose(disposing);
		}

		private void OnWaveFormatChanged()
		{
			this.WaveFormatChanged?.Invoke(this, EventArgs.Empty);
		}
	}
	internal class Mp3Index
	{
		public long FilePosition { get; set; }

		public long SamplePosition { get; set; }

		public int SampleCount { get; set; }

		public int ByteCount { get; set; }
	}
	public class Mp3FileReader : WaveStream
	{
		public delegate IMp3FrameDecompressor FrameDecompressorBuilder(WaveFormat mp3Format);

		private readonly WaveFormat waveFormat;

		private Stream mp3Stream;

		private readonly long mp3DataLength;

		private readonly long dataStartPosition;

		private readonly XingHeader xingHeader;

		private readonly bool ownInputStream;

		private List<Mp3Index> tableOfContents;

		private int tocIndex;

		private long totalSamples;

		private readonly int bytesPerSample;

		private readonly int bytesPerDecodedFrame;

		private IMp3FrameDecompressor decompressor;

		private readonly byte[] decompressBuffer;

		private int decompressBufferOffset;

		private int decompressLeftovers;

		private bool repositionedFlag;

		private long position;

		private readonly object repositionLock = new object();

		public Mp3WaveFormat Mp3WaveFormat { get; private set; }

		public Id3v2Tag Id3v2Tag { get; }

		public byte[] Id3v1Tag { get; }

		public override long Length => totalSamples * bytesPerSample;

		public override WaveFormat WaveFormat => waveFormat;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (repositionLock)
				{
					value = Math.Max(Math.Min(value, Length), 0L);
					long num = value / bytesPerSample;
					Mp3Index mp3Index = null;
					for (int i = 0; i < tableOfContents.Count; i++)
					{
						if (tableOfContents[i].SamplePosition + tableOfContents[i].SampleCount > num)
						{
							mp3Index = tableOfContents[i];
							tocIndex = i;
							break;
						}
					}
					decompressBufferOffset = 0;
					decompressLeftovers = 0;
					repositionedFlag = true;
					if (mp3Index != null)
					{
						mp3Stream.Position = mp3Index.FilePosition;
						long num2 = num - mp3Index.SamplePosition;
						if (num2 > 0)
						{
							decompressBufferOffset = (int)num2 * bytesPerSample;
						}
					}
					else
					{
						mp3Stream.Position = mp3DataLength + dataStartPosition;
					}
					position = value;
				}
			}
		}

		public XingHeader XingHeader => xingHeader;

		public Mp3FileReader(string mp3FileName)
			: this(File.OpenRead(mp3FileName), CreateAcmFrameDecompressor, ownInputStream: true)
		{
		}

		public Mp3FileReader(string mp3FileName, FrameDecompressorBuilder frameDecompressorBuilder)
			: this(File.OpenRead(mp3FileName), frameDecompressorBuilder, ownInputStream: true)
		{
		}

		public Mp3FileReader(Stream inputStream)
			: this(inputStream, CreateAcmFrameDecompressor, ownInputStream: false)
		{
		}

		public Mp3FileReader(Stream inputStream, FrameDecompressorBuilder frameDecompressorBuilder)
			: this(inputStream, frameDecompressorBuilder, ownInputStream: false)
		{
		}

		private Mp3FileReader(Stream inputStream, FrameDecompressorBuilder frameDecompressorBuilder, bool ownInputStream)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (frameDecompressorBuilder == null)
			{
				throw new ArgumentNullException("frameDecompressorBuilder");
			}
			this.ownInputStream = ownInputStream;
			try
			{
				mp3Stream = inputStream;
				Id3v2Tag = Id3v2Tag.ReadTag(mp3Stream);
				dataStartPosition = mp3Stream.Position;
				Mp3Frame mp3Frame = Mp3Frame.LoadFromStream(mp3Stream);
				if (mp3Frame == null)
				{
					throw new InvalidDataException("Invalid MP3 file - no MP3 Frames Detected");
				}
				double num = mp3Frame.BitRate;
				xingHeader = XingHeader.LoadXingHeader(mp3Frame);
				if (xingHeader != null)
				{
					dataStartPosition = mp3Stream.Position;
				}
				Mp3Frame mp3Frame2 = Mp3Frame.LoadFromStream(mp3Stream);
				if (mp3Frame2 != null && (mp3Frame2.SampleRate != mp3Frame.SampleRate || mp3Frame2.ChannelMode != mp3Frame.ChannelMode))
				{
					dataStartPosition = mp3Frame2.FileOffset;
					mp3Frame = mp3Frame2;
				}
				mp3DataLength = mp3Stream.Length - dataStartPosition;
				mp3Stream.Position = mp3Stream.Length - 128;
				byte[] array = new byte[128];
				mp3Stream.Read(array, 0, 128);
				if (array[0] == 84 && array[1] == 65 && array[2] == 71)
				{
					Id3v1Tag = array;
					mp3DataLength -= 128L;
				}
				mp3Stream.Position = dataStartPosition;
				Mp3WaveFormat = new Mp3WaveFormat(mp3Frame.SampleRate, (mp3Frame.ChannelMode == ChannelMode.Mono) ? 1 : 2, mp3Frame.FrameLength, (int)num);
				CreateTableOfContents();
				tocIndex = 0;
				num = (double)mp3DataLength * 8.0 / TotalSeconds();
				mp3Stream.Position = dataStartPosition;
				Mp3WaveFormat = new Mp3WaveFormat(mp3Frame.SampleRate, (mp3Frame.ChannelMode == ChannelMode.Mono) ? 1 : 2, mp3Frame.FrameLength, (int)num);
				decompressor = frameDecompressorBuilder(Mp3WaveFormat);
				waveFormat = decompressor.OutputFormat;
				bytesPerSample = decompressor.OutputFormat.BitsPerSample / 8 * decompressor.OutputFormat.Channels;
				bytesPerDecodedFrame = 1152 * bytesPerSample;
				decompressBuffer = new byte[bytesPerDecodedFrame * 2];
			}
			catch (Exception)
			{
				if (ownInputStream)
				{
					inputStream.Dispose();
				}
				throw;
			}
		}

		public static IMp3FrameDecompressor CreateAcmFrameDecompressor(WaveFormat mp3Format)
		{
			return new AcmMp3FrameDecompressor(mp3Format);
		}

		private void CreateTableOfContents()
		{
			try
			{
				tableOfContents = new List<Mp3Index>((int)(mp3DataLength / 400));
				Mp3Frame mp3Frame;
				do
				{
					Mp3Index mp3Index = new Mp3Index();
					mp3Index.FilePosition = mp3Stream.Position;
					mp3Index.SamplePosition = totalSamples;
					mp3Frame = ReadNextFrame(readData: false);
					if (mp3Frame != null)
					{
						ValidateFrameFormat(mp3Frame);
						totalSamples += mp3Frame.SampleCount;
						mp3Index.SampleCount = mp3Frame.SampleCount;
						mp3Index.ByteCount = (int)(mp3Stream.Position - mp3Index.FilePosition);
						tableOfContents.Add(mp3Index);
					}
				}
				while (mp3Frame != null);
			}
			catch (EndOfStreamException)
			{
			}
		}

		private void ValidateFrameFormat(Mp3Frame frame)
		{
			if (frame.SampleRate != Mp3WaveFormat.SampleRate)
			{
				throw new InvalidOperationException($"Got a frame at sample rate {frame.SampleRate}, in an MP3 with sample rate {Mp3WaveFormat.SampleRate}. Mp3FileReader does not support sample rate changes.");
			}
			if (((frame.ChannelMode == ChannelMode.Mono) ? 1 : 2) != Mp3WaveFormat.Channels)
			{
				throw new InvalidOperationException($"Got a frame with channel mode {frame.ChannelMode}, in an MP3 with {Mp3WaveFormat.Channels} channels. Mp3FileReader does not support changes to channel count.");
			}
		}

		private double TotalSeconds()
		{
			return (double)totalSamples / (double)Mp3WaveFormat.SampleRate;
		}

		public Mp3Frame ReadNextFrame()
		{
			Mp3Frame mp3Frame = ReadNextFrame(readData: true);
			if (mp3Frame != null)
			{
				position += mp3Frame.SampleCount * bytesPerSample;
			}
			return mp3Frame;
		}

		private Mp3Frame ReadNextFrame(bool readData)
		{
			Mp3Frame mp3Frame = null;
			try
			{
				mp3Frame = Mp3Frame.LoadFromStream(mp3Stream, readData);
				if (mp3Frame != null)
				{
					tocIndex++;
					return mp3Frame;
				}
				return mp3Frame;
			}
			catch (EndOfStreamException)
			{
				return mp3Frame;
			}
		}

		public override int Read(byte[] sampleBuffer, int offset, int numBytes)
		{
			int num = 0;
			lock (repositionLock)
			{
				if (decompressLeftovers != 0)
				{
					int num2 = Math.Min(decompressLeftovers, numBytes);
					Array.Copy(decompressBuffer, decompressBufferOffset, sampleBuffer, offset, num2);
					decompressLeftovers -= num2;
					if (decompressLeftovers == 0)
					{
						decompressBufferOffset = 0;
					}
					else
					{
						decompressBufferOffset += num2;
					}
					num += num2;
					offset += num2;
				}
				int num3 = tocIndex;
				if (repositionedFlag)
				{
					decompressor.Reset();
					tocIndex = Math.Max(0, tocIndex - 3);
					mp3Stream.Position = tableOfContents[tocIndex].FilePosition;
					repositionedFlag = false;
				}
				while (num < numBytes)
				{
					Mp3Frame mp3Frame = ReadNextFrame(readData: true);
					if (mp3Frame == null)
					{
						break;
					}
					int num4 = decompressor.DecompressFrame(mp3Frame, decompressBuffer, 0);
					if (tocIndex > num3 && num4 != 0)
					{
						if (tocIndex == num3 + 1 && num4 == bytesPerDecodedFrame * 2)
						{
							Array.Copy(decompressBuffer, bytesPerDecodedFrame, decompressBuffer, 0, bytesPerDecodedFrame);
							num4 = bytesPerDecodedFrame;
						}
						int num5 = Math.Min(num4 - decompressBufferOffset, numBytes - num);
						Array.Copy(decompressBuffer, decompressBufferOffset, sampleBuffer, offset, num5);
						if (num5 + decompressBufferOffset < num4)
						{
							decompressBufferOffset = num5 + decompressBufferOffset;
							decompressLeftovers = num4 - decompressBufferOffset;
						}
						else
						{
							decompressBufferOffset = 0;
							decompressLeftovers = 0;
						}
						offset += num5;
						num += num5;
					}
				}
			}
			position += num;
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (mp3Stream != null)
				{
					if (ownInputStream)
					{
						mp3Stream.Dispose();
					}
					mp3Stream = null;
				}
				if (decompressor != null)
				{
					decompressor.Dispose();
					decompressor = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	public class RawSourceWaveStream : WaveStream
	{
		private readonly Stream sourceStream;

		private readonly WaveFormat waveFormat;

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => sourceStream.Length;

		public override long Position
		{
			get
			{
				return sourceStream.Position;
			}
			set
			{
				sourceStream.Position = value - value % waveFormat.BlockAlign;
			}
		}

		public RawSourceWaveStream(Stream sourceStream, WaveFormat waveFormat)
		{
			this.sourceStream = sourceStream;
			this.waveFormat = waveFormat;
		}

		public RawSourceWaveStream(byte[] byteStream, int offset, int count, WaveFormat waveFormat)
		{
			sourceStream = new MemoryStream(byteStream, offset, count);
			this.waveFormat = waveFormat;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				return sourceStream.Read(buffer, offset, count);
			}
			catch (EndOfStreamException)
			{
				return 0;
			}
		}
	}
	public class ResamplerDmoStream : WaveStream
	{
		private readonly IWaveProvider inputProvider;

		private readonly WaveStream inputStream;

		private readonly WaveFormat outputFormat;

		private DmoOutputDataBuffer outputBuffer;

		private DmoResampler dmoResampler;

		private MediaBuffer inputMediaBuffer;

		private long position;

		public override WaveFormat WaveFormat => outputFormat;

		public override long Length
		{
			get
			{
				if (inputStream == null)
				{
					throw new InvalidOperationException("Cannot report length if the input was an IWaveProvider");
				}
				return InputToOutputPosition(inputStream.Length);
			}
		}

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				if (inputStream == null)
				{
					throw new InvalidOperationException("Cannot set position if the input was not a WaveStream");
				}
				inputStream.Position = OutputToInputPosition(value);
				position = InputToOutputPosition(inputStream.Position);
				dmoResampler.MediaObject.Discontinuity(0);
			}
		}

		public ResamplerDmoStream(IWaveProvider inputProvider, WaveFormat outputFormat)
		{
			this.inputProvider = inputProvider;
			inputStream = inputProvider as WaveStream;
			this.outputFormat = outputFormat;
			dmoResampler = new DmoResampler();
			if (!dmoResampler.MediaObject.SupportsInputWaveFormat(0, inputProvider.WaveFormat))
			{
				throw new ArgumentException("Unsupported Input Stream format", "inputProvider");
			}
			dmoResampler.MediaObject.SetInputWaveFormat(0, inputProvider.WaveFormat);
			if (!dmoResampler.MediaObject.SupportsOutputWaveFormat(0, outputFormat))
			{
				throw new ArgumentException("Unsupported Output Stream format", "outputFormat");
			}
			dmoResampler.MediaObject.SetOutputWaveFormat(0, outputFormat);
			if (inputStream != null)
			{
				position = InputToOutputPosition(inputStream.Position);
			}
			inputMediaBuffer = new MediaBuffer(inputProvider.WaveFormat.AverageBytesPerSecond);
			outputBuffer = new DmoOutputDataBuffer(outputFormat.AverageBytesPerSecond);
		}

		private long InputToOutputPosition(long inputPosition)
		{
			double num = (double)outputFormat.AverageBytesPerSecond / (double)inputProvider.WaveFormat.AverageBytesPerSecond;
			long num2 = (long)((double)inputPosition * num);
			if (num2 % outputFormat.BlockAlign != 0L)
			{
				num2 -= num2 % outputFormat.BlockAlign;
			}
			return num2;
		}

		private long OutputToInputPosition(long outputPosition)
		{
			double num = (double)outputFormat.AverageBytesPerSecond / (double)inputProvider.WaveFormat.AverageBytesPerSecond;
			long num2 = (long)((double)outputPosition / num);
			if (num2 % inputProvider.WaveFormat.BlockAlign != 0L)
			{
				num2 -= num2 % inputProvider.WaveFormat.BlockAlign;
			}
			return num2;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			while (num < count)
			{
				if (dmoResampler.MediaObject.IsAcceptingData(0))
				{
					int num2 = (int)OutputToInputPosition(count - num);
					byte[] array = new byte[num2];
					int num3 = inputProvider.Read(array, 0, num2);
					if (num3 == 0)
					{
						break;
					}
					inputMediaBuffer.LoadData(array, num3);
					dmoResampler.MediaObject.ProcessInput(0, inputMediaBuffer, DmoInputDataBufferFlags.None, 0L, 0L);
					outputBuffer.MediaBuffer.SetLength(0);
					outputBuffer.StatusFlags = DmoOutputDataBufferFlags.None;
					dmoResampler.MediaObject.ProcessOutput(DmoProcessOutputFlags.None, 1, new DmoOutputDataBuffer[1] { outputBuffer });
					if (outputBuffer.Length == 0)
					{
						break;
					}
					outputBuffer.RetrieveData(buffer, offset + num);
					num += outputBuffer.Length;
				}
			}
			position += num;
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (inputMediaBuffer != null)
			{
				inputMediaBuffer.Dispose();
				inputMediaBuffer = null;
			}
			outputBuffer.Dispose();
			if (dmoResampler != null)
			{
				dmoResampler = null;
			}
			base.Dispose(disposing);
		}
	}
	public class RiffChunk
	{
		public int Identifier { get; }

		public string IdentifierAsString => Encoding.UTF8.GetString(BitConverter.GetBytes(Identifier));

		public int Length { get; private set; }

		public long StreamPosition { get; private set; }

		public RiffChunk(int identifier, int length, long streamPosition)
		{
			Identifier = identifier;
			Length = length;
			StreamPosition = streamPosition;
		}
	}
	public class SimpleCompressorEffect : ISampleProvider
	{
		private readonly ISampleProvider sourceStream;

		private readonly SimpleCompressor simpleCompressor;

		private readonly int channels;

		private readonly object lockObject = new object();

		public double MakeUpGain
		{
			get
			{
				return simpleCompressor.MakeUpGain;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.MakeUpGain = value;
				}
			}
		}

		public double Threshold
		{
			get
			{
				return simpleCompressor.Threshold;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Threshold = value;
				}
			}
		}

		public double Ratio
		{
			get
			{
				return simpleCompressor.Ratio;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Ratio = value;
				}
			}
		}

		public double Attack
		{
			get
			{
				return simpleCompressor.Attack;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Attack = value;
				}
			}
		}

		public double Release
		{
			get
			{
				return simpleCompressor.Release;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Release = value;
				}
			}
		}

		public bool Enabled { get; set; }

		public WaveFormat WaveFormat => sourceStream.WaveFormat;

		public SimpleCompressorEffect(ISampleProvider sourceStream)
		{
			this.sourceStream = sourceStream;
			channels = sourceStream.WaveFormat.Channels;
			simpleCompressor = new SimpleCompressor(5.0, 10.0, sourceStream.WaveFormat.SampleRate);
			simpleCompressor.Threshold = 16.0;
			simpleCompressor.Ratio = 6.0;
			simpleCompressor.MakeUpGain = 16.0;
		}

		public int Read(float[] array, int offset, int count)
		{
			lock (lockObject)
			{
				int num = sourceStream.Read(array, offset, count);
				if (Enabled)
				{
					for (int i = 0; i < num; i += channels)
					{
						double @in = array[offset + i];
						double in2 = ((channels == 1) ? 0f : array[offset + i + 1]);
						simpleCompressor.Process(ref @in, ref in2);
						array[offset + i] = (float)@in;
						if (channels > 1)
						{
							array[offset + i + 1] = (float)in2;
						}
					}
				}
				return num;
			}
		}
	}
	public class StreamMediaFoundationReader : MediaFoundationReader
	{
		private readonly Stream stream;

		public StreamMediaFoundationReader(Stream stream, MediaFoundationReaderSettings settings = null)
		{
			this.stream = stream;
			Init(settings);
		}

		protected override IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
		{
			IMFSourceReader iMFSourceReader = MediaFoundationApi.CreateSourceReaderFromByteStream(MediaFoundationApi.CreateByteStream(new ComStream(stream)));
			iMFSourceReader.SetStreamSelection(-2, pSelected: false);
			iMFSourceReader.SetStreamSelection(-3, pSelected: true);
			iMFSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, new MediaType
			{
				MajorType = NAudio.MediaFoundation.MediaTypes.MFMediaType_Audio,
				SubType = (settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM)
			}.MediaFoundationObject);
			return iMFSourceReader;
		}
	}
	public class Wave32To16Stream : WaveStream
	{
		private WaveStream sourceStream;

		private readonly WaveFormat waveFormat;

		private readonly long length;

		private long position;

		private bool clip;

		private float volume;

		private readonly object lockObject = new object();

		private byte[] sourceBuffer;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public override int BlockAlign => sourceStream.BlockAlign / 2;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (lockObject)
				{
					value -= value % BlockAlign;
					sourceStream.Position = value * 2;
					position = value;
				}
			}
		}

		public override WaveFormat WaveFormat => waveFormat;

		public bool Clip
		{
			get
			{
				return clip;
			}
			set
			{
				clip = value;
			}
		}

		public Wave32To16Stream(WaveStream sourceStream)
		{
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Only 32 bit Floating point supported");
			}
			if (sourceStream.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Only 32 bit Floating point supported");
			}
			waveFormat = new WaveFormat(sourceStream.WaveFormat.SampleRate, 16, sourceStream.WaveFormat.Channels);
			volume = 1f;
			this.sourceStream = sourceStream;
			length = sourceStream.Length / 2;
			position = sourceStream.Position / 2;
		}

		public override int Read(byte[] destBuffer, int offset, int numBytes)
		{
			lock (lockObject)
			{
				int num = numBytes * 2;
				sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
				int num2 = sourceStream.Read(sourceBuffer, 0, num);
				Convert32To16(destBuffer, offset, sourceBuffer, num2);
				position += num2 / 2;
				return num2 / 2;
			}
		}

		private unsafe void Convert32To16(byte[] destBuffer, int offset, byte[] source, int bytesRead)
		{
			fixed (byte* ptr = &destBuffer[offset])
			{
				fixed (byte* ptr3 = &source[0])
				{
					short* ptr2 = (short*)ptr;
					float* ptr4 = (float*)ptr3;
					int num = bytesRead / 4;
					for (int i = 0; i < num; i++)
					{
						float num2 = ptr4[i] * volume;
						if (num2 > 1f)
						{
							ptr2[i] = short.MaxValue;
							clip = true;
						}
						else if (num2 < -1f)
						{
							ptr2[i] = short.MinValue;
							clip = true;
						}
						else
						{
							ptr2[i] = (short)(num2 * 32767f);
						}
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && sourceStream != null)
			{
				sourceStream.Dispose();
				sourceStream = null;
			}
			base.Dispose(disposing);
		}
	}
	public class WaveChannel32 : WaveStream, ISampleNotifier
	{
		private WaveStream sourceStream;

		private readonly WaveFormat waveFormat;

		private readonly long length;

		private readonly int destBytesPerSample;

		private readonly int sourceBytesPerSample;

		private volatile float volume;

		private volatile float pan;

		private long position;

		private readonly ISampleChunkConverter sampleProvider;

		private readonly object lockObject = new object();

		private SampleEventArgs sampleEventArgs = new SampleEventArgs(0f, 0f);

		public override int BlockAlign => (int)SourceToDest(sourceStream.BlockAlign);

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (lockObject)
				{
					value -= value % BlockAlign;
					if (value < 0)
					{
						sourceStream.Position = 0L;
					}
					else
					{
						sourceStream.Position = DestToSource(value);
					}
					position = SourceToDest(sourceStream.Position);
				}
			}
		}

		public bool PadWithZeroes { get; set; }

		public override WaveFormat WaveFormat => waveFormat;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public float Pan
		{
			get
			{
				return pan;
			}
			set
			{
				pan = value;
			}
		}

		public event EventHandler<SampleEventArgs> Sample;

		public WaveChannel32(WaveStream sourceStream, float volume, float pan)
		{
			PadWithZeroes = true;
			ISampleChunkConverter[] array = new ISampleChunkConverter[8]
			{
				new Mono8SampleChunkConverter(),
				new Stereo8SampleChunkConverter(),
				new Mono16SampleChunkConverter(),
				new Stereo16SampleChunkConverter(),
				new Mono24SampleChunkConverter(),
				new Stereo24SampleChunkConverter(),
				new MonoFloatSampleChunkConverter(),
				new StereoFloatSampleChunkConverter()
			};
			foreach (ISampleChunkConverter sampleChunkConverter in array)
			{
				if (sampleChunkConverter.Supports(sourceStream.WaveFormat))
				{
					sampleProvider = sampleChunkConverter;
					break;
				}
			}
			if (sampleProvider == null)
			{
				throw new ArgumentException("Unsupported sourceStream format");
			}
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceStream.WaveFormat.SampleRate, 2);
			destBytesPerSample = 8;
			this.sourceStream = sourceStream;
			this.volume = volume;
			this.pan = pan;
			sourceBytesPerSample = sourceStream.WaveFormat.Channels * sourceStream.WaveFormat.BitsPerSample / 8;
			length = SourceToDest(sourceStream.Length);
			position = 0L;
		}

		private long SourceToDest(long sourceBytes)
		{
			return sourceBytes / sourceBytesPerSample * destBytesPerSample;
		}

		private long DestToSource(long destBytes)
		{
			return destBytes / destBytesPerSample * sourceBytesPerSample;
		}

		public WaveChannel32(WaveStream sourceStream)
			: this(sourceStream, 1f, 0f)
		{
		}

		public override int Read(byte[] destBuffer, int offset, int numBytes)
		{
			lock (lockObject)
			{
				int num = 0;
				WaveBuffer waveBuffer = new WaveBuffer(destBuffer);
				if (position < 0)
				{
					num = (int)Math.Min(numBytes, -position);
					for (int i = 0; i < num; i++)
					{
						destBuffer[i + offset] = 0;
					}
				}
				if (num < numBytes)
				{
					sampleProvider.LoadNextChunk(sourceStream, (numBytes - num) / 8);
					int num2 = offset / 4 + num / 4;
					float sampleLeft;
					float sampleRight;
					while (sampleProvider.GetNextSample(out sampleLeft, out sampleRight) && num < numBytes)
					{
						sampleLeft = ((pan <= 0f) ? sampleLeft : (sampleLeft * (1f - pan) / 2f));
						sampleRight = ((pan >= 0f) ? sampleRight : (sampleRight * (pan + 1f) / 2f));
						sampleLeft *= volume;
						sampleRight *= volume;
						waveBuffer.FloatBuffer[num2++] = sampleLeft;
						waveBuffer.FloatBuffer[num2++] = sampleRight;
						num += 8;
						if (this.Sample != null)
						{
							RaiseSample(sampleLeft, sampleRight);
						}
					}
				}
				if (PadWithZeroes && num < numBytes)
				{
					Array.Clear(destBuffer, offset + num, numBytes - num);
					num = numBytes;
				}
				position += num;
				return num;
			}
		}

		public override bool HasData(int count)
		{
			if (sourceStream.HasData(count))
			{
				if (position + count < 0)
				{
					return false;
				}
				if (position < length)
				{
					return volume != 0f;
				}
				return false;
			}
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && sourceStream != null)
			{
				sourceStream.Dispose();
				sourceStream = null;
			}
			base.Dispose(disposing);
		}

		private void RaiseSample(float left, float right)
		{
			sampleEventArgs.Left = left;
			sampleEventArgs.Right = right;
			this.Sample(this, sampleEventArgs);
		}
	}
	public class WaveFileReader : WaveStream
	{
		private readonly WaveFormat waveFormat;

		private readonly bool ownInput;

		private readonly long dataPosition;

		private readonly long dataChunkLength;

		private readonly object lockObject = new object();

		private Stream waveStream;

		public List<RiffChunk> ExtraChunks { get; }

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => dataChunkLength;

		public long SampleCount
		{
			get
			{
				if (waveFormat.Encoding == WaveFormatEncoding.Pcm || waveFormat.Encoding == WaveFormatEncoding.Extensible || waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
				{
					return dataChunkLength / BlockAlign;
				}
				throw new InvalidOperationException("Sample count is calculated only for the standard encodings");
			}
		}

		public override long Position
		{
			get
			{
				return waveStream.Position - dataPosition;
			}
			set
			{
				lock (lockObject)
				{
					value = Math.Min(value, Length);
					value -= value % waveFormat.BlockAlign;
					waveStream.Position = value + dataPosition;
				}
			}
		}

		public WaveFileReader(string waveFile)
			: this(File.OpenRead(waveFile), ownInput: true)
		{
		}

		public WaveFileReader(Stream inputStream)
			: this(inputStream, ownInput: false)
		{
		}

		private WaveFileReader(Stream inputStream, bool ownInput)
		{
			waveStream = inputStream;
			WaveFileChunkReader waveFileChunkReader = new WaveFileChunkReader();
			try
			{
				waveFileChunkReader.ReadWaveHeader(inputStream);
				waveFormat = waveFileChunkReader.WaveFormat;
				dataPosition = waveFileChunkReader.DataChunkPosition;
				dataChunkLength = waveFileChunkReader.DataChunkLength;
				ExtraChunks = waveFileChunkReader.RiffChunks;
			}
			catch
			{
				if (ownInput)
				{
					inputStream.Dispose();
				}
				throw;
			}
			Position = 0L;
			this.ownInput = ownInput;
		}

		public byte[] GetChunkData(RiffChunk chunk)
		{
			long position = waveStream.Position;
			waveStream.Position = chunk.StreamPosition;
			byte[] array = new byte[chunk.Length];
			waveStream.Read(array, 0, array.Length);
			waveStream.Position = position;
			return array;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && waveStream != null)
			{
				if (ownInput)
				{
					waveStream.Dispose();
				}
				waveStream = null;
			}
			base.Dispose(disposing);
		}

		public override int Read(byte[] array, int offset, int count)
		{
			if (count % waveFormat.BlockAlign != 0)
			{
				throw new ArgumentException($"Must read complete blocks: requested {count}, block align is {WaveFormat.BlockAlign}");
			}
			lock (lockObject)
			{
				if (Position + count > dataChunkLength)
				{
					count = (int)(dataChunkLength - Position);
				}
				return waveStream.Read(array, offset, count);
			}
		}

		public float[] ReadNextSampleFrame()
		{
			WaveFormatEncoding encoding = waveFormat.Encoding;
			if (encoding != WaveFormatEncoding.Pcm && encoding != WaveFormatEncoding.IeeeFloat && encoding != WaveFormatEncoding.Extensible)
			{
				throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
			}
			float[] array = new float[waveFormat.Channels];
			int num = waveFormat.Channels * (waveFormat.BitsPerSample / 8);
			byte[] array2 = new byte[num];
			int num2 = Read(array2, 0, num);
			if (num2 == 0)
			{
				return null;
			}
			if (num2 < num)
			{
				throw new InvalidDataException("Unexpected end of file");
			}
			int num3 = 0;
			for (int i = 0; i < waveFormat.Channels; i++)
			{
				if (waveFormat.BitsPerSample == 16)
				{
					array[i] = (float)BitConverter.ToInt16(array2, num3) / 32768f;
					num3 += 2;
					continue;
				}
				if (waveFormat.BitsPerSample == 24)
				{
					array[i] = (float)(((sbyte)array2[num3 + 2] << 16) | (array2[num3 + 1] << 8) | array2[num3]) / 8388608f;
					num3 += 3;
					continue;
				}
				if (waveFormat.BitsPerSample == 32 && waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
				{
					array[i] = BitConverter.ToSingle(array2, num3);
					num3 += 4;
					continue;
				}
				if (waveFormat.BitsPerSample == 32)
				{
					array[i] = (float)BitConverter.ToInt32(array2, num3) / 2.1474836E+09f;
					num3 += 4;
					continue;
				}
				throw new InvalidOperationException("Unsupported bit depth");
			}
			return array;
		}

		[Obsolete("Use ReadNextSampleFrame instead (this version does not support stereo properly)")]
		public bool TryReadFloat(out float sampleValue)
		{
			float[] array = ReadNextSampleFrame();
			sampleValue = ((array != null) ? array[0] : 0f);
			return array != null;
		}
	}
	public class WaveFormatConversionProvider : IWaveProvider, IDisposable
	{
		private readonly AcmStream conversionStream;

		private readonly IWaveProvider sourceProvider;

		private readonly int preferredSourceReadSize;

		private int leftoverDestBytes;

		private int leftoverDestOffset;

		private int leftoverSourceBytes;

		private bool isDisposed;

		public WaveFormat WaveFormat { get; }

		public WaveFormatConversionProvider(WaveFormat targetFormat, IWaveProvider sourceProvider)
		{
			this.sourceProvider = sourceProvider;
			WaveFormat = targetFormat;
			conversionStream = new AcmStream(sourceProvider.WaveFormat, targetFormat);
			preferredSourceReadSize = Math.Min(sourceProvider.WaveFormat.AverageBytesPerSecond, conversionStream.SourceBuffer.Length);
			preferredSourceReadSize -= preferredSourceReadSize % sourceProvider.WaveFormat.BlockAlign;
		}

		public void Reposition()
		{
			leftoverDestBytes = 0;
			leftoverDestOffset = 0;
			leftoverSourceBytes = 0;
			conversionStream.Reposition();
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int i = 0;
			if (count % WaveFormat.BlockAlign != 0)
			{
				count -= count % WaveFormat.BlockAlign;
			}
			int num4;
			for (; i < count; i += num4)
			{
				int num = Math.Min(count - i, leftoverDestBytes);
				if (num > 0)
				{
					Array.Copy(conversionStream.DestBuffer, leftoverDestOffset, buffer, offset + i, num);
					leftoverDestOffset += num;
					leftoverDestBytes -= num;
					i += num;
				}
				if (i >= count)
				{
					break;
				}
				int count2 = Math.Min(preferredSourceReadSize, conversionStream.SourceBuffer.Length - leftoverSourceBytes);
				int num2 = sourceProvider.Read(conversionStream.SourceBuffer, leftoverSourceBytes, count2) + leftoverSourceBytes;
				if (num2 == 0)
				{
					break;
				}
				int sourceBytesConverted;
				int num3 = conversionStream.Convert(num2, out sourceBytesConverted);
				if (sourceBytesConverted == 0)
				{
					break;
				}
				leftoverSourceBytes = num2 - sourceBytesConverted;
				if (leftoverSourceBytes > 0)
				{
					Buffer.BlockCopy(conversionStream.SourceBuffer, sourceBytesConverted, conversionStream.SourceBuffer, 0, leftoverSourceBytes);
				}
				if (num3 <= 0)
				{
					break;
				}
				int val = count - i;
				num4 = Math.Min(num3, val);
				if (num4 < num3)
				{
					leftoverDestBytes = num3 - num4;
					leftoverDestOffset = num4;
				}
				Array.Copy(conversionStream.DestBuffer, 0, buffer, i + offset, num4);
			}
			return i;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
				conversionStream?.Dispose();
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		~WaveFormatConversionProvider()
		{
			Dispose(disposing: false);
		}
	}
	public class WaveFormatConversionStream : WaveStream
	{
		private readonly WaveFormatConversionProvider conversionProvider;

		private readonly WaveFormat targetFormat;

		private readonly long length;

		private long position;

		private readonly WaveStream sourceStream;

		private bool isDisposed;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				value -= value % BlockAlign;
				long num = EstimateDestToSource(value);
				sourceStream.Position = num;
				position = EstimateSourceToDest(sourceStream.Position);
				conversionProvider.Reposition();
			}
		}

		public override long Length => length;

		public override WaveFormat WaveFormat => targetFormat;

		public WaveFormatConversionStream(WaveFormat targetFormat, WaveStream sourceStream)
		{
			this.sourceStream = sourceStream;
			this.targetFormat = targetFormat;
			conversionProvider = new WaveFormatConversionProvider(targetFormat, sourceStream);
			length = EstimateSourceToDest((int)sourceStream.Length);
			position = 0L;
		}

		public static WaveStream CreatePcmStream(WaveStream sourceStream)
		{
			if (sourceStream.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				return sourceStream;
			}
			WaveFormat waveFormat = AcmStream.SuggestPcmFormat(sourceStream.WaveFormat);
			if (waveFormat.SampleRate < 8000)
			{
				if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.G723)
				{
					throw new InvalidOperationException("Invalid suggested output format, please explicitly provide a target format");
				}
				waveFormat = new WaveFormat(8000, 16, 1);
			}
			return new WaveFormatConversionStream(waveFormat, sourceStream);
		}

		[Obsolete("can be unreliable, use of this method not encouraged")]
		public int SourceToDest(int source)
		{
			return (int)EstimateSourceToDest(source);
		}

		private long EstimateSourceToDest(long source)
		{
			long num = source * targetFormat.AverageBytesPerSecond / sourceStream.WaveFormat.AverageBytesPerSecond;
			return num - num % targetFormat.BlockAlign;
		}

		private long EstimateDestToSource(long dest)
		{
			long num = dest * sourceStream.WaveFormat.AverageBytesPerSecond / targetFormat.AverageBytesPerSecond;
			return (int)(num - num % sourceStream.WaveFormat.BlockAlign);
		}

		[Obsolete("can be unreliable, use of this method not encouraged")]
		public int DestToSource(int dest)
		{
			return (int)EstimateDestToSource(dest);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = conversionProvider.Read(buffer, offset, count);
			position += num;
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
				if (disposing)
				{
					sourceStream.Dispose();
					conversionProvider.Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
	internal class WaveInBuffer : IDisposable
	{
		private readonly WaveHeader header;

		private readonly int bufferSize;

		private readonly byte[] buffer;

		private GCHandle hBuffer;

		private IntPtr waveInHandle;

		private GCHandle hHeader;

		private GCHandle hThis;

		public byte[] Data => buffer;

		public bool Done => (header.flags & WaveHeaderFlags.Done) == WaveHeaderFlags.Done;

		public bool InQueue => (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;

		public int BytesRecorded => header.bytesRecorded;

		public int BufferSize => bufferSize;

		public WaveInBuffer(IntPtr waveInHandle, int bufferSize)
		{
			this.bufferSize = bufferSize;
			buffer = new byte[bufferSize];
			hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			this.waveInHandle = waveInHandle;
			header = new WaveHeader();
			hHeader = GCHandle.Alloc(header, GCHandleType.Pinned);
			header.dataBuffer = hBuffer.AddrOfPinnedObject();
			header.bufferLength = bufferSize;
			header.loops = 1;
			hThis = GCHandle.Alloc(this);
			header.userData = (IntPtr)hThis;
			MmException.Try(WaveInterop.waveInPrepareHeader(waveInHandle, header, Marshal.SizeOf(header)), "waveInPrepareHeader");
		}

		public void Reuse()
		{
			MmException.Try(WaveInterop.waveInUnprepareHeader(waveInHandle, header, Marshal.SizeOf(header)), "waveUnprepareHeader");
			MmException.Try(WaveInterop.waveInPrepareHeader(waveInHandle, header, Marshal.SizeOf(header)), "waveInPrepareHeader");
			MmException.Try(WaveInterop.waveInAddBuffer(waveInHandle, header, Marshal.SizeOf(header)), "waveInAddBuffer");
		}

		~WaveInBuffer()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		protected void Dispose(bool disposing)
		{
			if (waveInHandle != IntPtr.Zero)
			{
				WaveInterop.waveInUnprepareHeader(waveInHandle, header, Marshal.SizeOf(header));
				waveInHandle = IntPtr.Zero;
			}
			if (hHeader.IsAllocated)
			{
				hHeader.Free();
			}
			if (hBuffer.IsAllocated)
			{
				hBuffer.Free();
			}
			if (hThis.IsAllocated)
			{
				hThis.Free();
			}
		}
	}
	public class WaveMixerStream32 : WaveStream
	{
		private readonly List<WaveStream> inputStreams;

		private readonly object inputsLock;

		private WaveFormat waveFormat;

		private long length;

		private long position;

		private readonly int bytesPerSample;

		public int InputCount => inputStreams.Count;

		public bool AutoStop { get; set; }

		public override int BlockAlign => waveFormat.BlockAlign;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (inputsLock)
				{
					value = Math.Min(value, Length);
					foreach (WaveStream inputStream in inputStreams)
					{
						inputStream.Position = Math.Min(value, inputStream.Length);
					}
					position = value;
				}
			}
		}

		public override WaveFormat WaveFormat => waveFormat;

		public WaveMixerStream32()
		{
			AutoStop = true;
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
			bytesPerSample = 4;
			inputStreams = new List<WaveStream>();
			inputsLock = new object();
		}

		public WaveMixerStream32(IEnumerable<WaveStream> inputStreams, bool autoStop)
			: this()
		{
			AutoStop = autoStop;
			foreach (WaveStream inputStream in inputStreams)
			{
				AddInputStream(inputStream);
			}
		}

		public void AddInputStream(WaveStream waveStream)
		{
			if (waveStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be IEEE floating point", "waveStream");
			}
			if (waveStream.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Only 32 bit audio currently supported", "waveStream");
			}
			if (inputStreams.Count == 0)
			{
				int sampleRate = waveStream.WaveFormat.SampleRate;
				int channels = waveStream.WaveFormat.Channels;
				waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
			}
			else if (!waveStream.WaveFormat.Equals(waveFormat))
			{
				throw new ArgumentException("All incoming channels must have the same format", "waveStream");
			}
			lock (inputsLock)
			{
				inputStreams.Add(waveStream);
				length = Math.Max(length, waveStream.Length);
				waveStream.Position = Position;
			}
		}

		public void RemoveInputStream(WaveStream waveStream)
		{
			lock (inputsLock)
			{
				if (!inputStreams.Remove(waveStream))
				{
					return;
				}
				long val = 0L;
				foreach (WaveStream inputStream in inputStreams)
				{
					val = Math.Max(val, inputStream.Length);
				}
				length = val;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (AutoStop && position + count > length)
			{
				count = (int)(length - position);
			}
			if (count % bytesPerSample != 0)
			{
				throw new ArgumentException("Must read an whole number of samples", "count");
			}
			Array.Clear(buffer, offset, count);
			int val = 0;
			byte[] array = new byte[count];
			lock (inputsLock)
			{
				foreach (WaveStream inputStream in inputStreams)
				{
					if (inputStream.HasData(count))
					{
						int num = inputStream.Read(array, 0, count);
						val = Math.Max(val, num);
						if (num > 0)
						{
							Sum32BitAudio(buffer, offset, array, num);
						}
					}
					else
					{
						val = Math.Max(val, count);
						inputStream.Position += count;
					}
				}
			}
			position += count;
			return count;
		}

		private unsafe static void Sum32BitAudio(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
		{
			fixed (byte* ptr = &destBuffer[offset])
			{
				fixed (byte* ptr3 = &sourceBuffer[0])
				{
					float* ptr2 = (float*)ptr;
					float* ptr4 = (float*)ptr3;
					int num = bytesRead / 4;
					for (int i = 0; i < num; i++)
					{
						ptr2[i] += ptr4[i];
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (inputsLock)
				{
					foreach (WaveStream inputStream in inputStreams)
					{
						inputStream.Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}
	}
	public class WaveOffsetStream : WaveStream
	{
		private WaveStream sourceStream;

		private long audioStartPosition;

		private long sourceOffsetBytes;

		private long sourceLengthBytes;

		private long length;

		private readonly int bytesPerSample;

		private long position;

		private TimeSpan startTime;

		private TimeSpan sourceOffset;

		private TimeSpan sourceLength;

		private readonly object lockObject = new object();

		public TimeSpan StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				lock (lockObject)
				{
					startTime = value;
					audioStartPosition = (long)(startTime.TotalSeconds * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
					length = audioStartPosition + sourceLengthBytes;
					Position = Position;
				}
			}
		}

		public TimeSpan SourceOffset
		{
			get
			{
				return sourceOffset;
			}
			set
			{
				lock (lockObject)
				{
					sourceOffset = value;
					sourceOffsetBytes = (long)(sourceOffset.TotalSeconds * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
					Position = Position;
				}
			}
		}

		public TimeSpan SourceLength
		{
			get
			{
				return sourceLength;
			}
			set
			{
				lock (lockObject)
				{
					sourceLength = value;
					sourceLengthBytes = (long)(sourceLength.TotalSeconds * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
					length = audioStartPosition + sourceLengthBytes;
					Position = Position;
				}
			}
		}

		public override int BlockAlign => sourceStream.BlockAlign;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (lockObject)
				{
					value -= value % BlockAlign;
					if (value < audioStartPosition)
					{
						sourceStream.Position = sourceOffsetBytes;
					}
					else
					{
						sourceStream.Position = sourceOffsetBytes + (value - audioStartPosition);
					}
					position = value;
				}
			}
		}

		public override WaveFormat WaveFormat => sourceStream.WaveFormat;

		public WaveOffsetStream(WaveStream sourceStream, TimeSpan startTime, TimeSpan sourceOffset, TimeSpan sourceLength)
		{
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Only PCM supported");
			}
			this.sourceStream = sourceStream;
			position = 0L;
			bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8 * sourceStream.WaveFormat.Channels;
			StartTime = startTime;
			SourceOffset = sourceOffset;
			SourceLength = sourceLength;
		}

		public WaveOffsetStream(WaveStream sourceStream)
			: this(sourceStream, TimeSpan.Zero, TimeSpan.Zero, sourceStream.TotalTime)
		{
		}

		public override int Read(byte[] destBuffer, int offset, int numBytes)
		{
			lock (lockObject)
			{
				int num = 0;
				if (position < audioStartPosition)
				{
					num = (int)Math.Min(numBytes, audioStartPosition - position);
					for (int i = 0; i < num; i++)
					{
						destBuffer[i + offset] = 0;
					}
				}
				if (num < numBytes)
				{
					int count = (int)Math.Min(numBytes - num, sourceLengthBytes + sourceOffsetBytes - sourceStream.Position);
					int num2 = sourceStream.Read(destBuffer, num + offset, count);
					num += num2;
				}
				for (int j = num; j < numBytes; j++)
				{
					destBuffer[offset + j] = 0;
				}
				position += numBytes;
				return numBytes;
			}
		}

		public override bool HasData(int count)
		{
			if (position + count < audioStartPosition)
			{
				return false;
			}
			if (position >= length)
			{
				return false;
			}
			return sourceStream.HasData(count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && sourceStream != null)
			{
				sourceStream.Dispose();
				sourceStream = null;
			}
			base.Dispose(disposing);
		}
	}
	internal class WaveOutBuffer : IDisposable
	{
		private readonly WaveHeader header;

		private readonly int bufferSize;

		private readonly byte[] buffer;

		private readonly IWaveProvider waveStream;

		private readonly object waveOutLock;

		private GCHandle hBuffer;

		private IntPtr hWaveOut;

		private GCHandle hHeader;

		private GCHandle hThis;

		public bool InQueue => (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;

		public int BufferSize => bufferSize;

		public WaveOutBuffer(IntPtr hWaveOut, int bufferSize, IWaveProvider bufferFillStream, object waveOutLock)
		{
			this.bufferSize = bufferSize;
			buffer = new byte[bufferSize];
			hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			this.hWaveOut = hWaveOut;
			waveStream = bufferFillStream;
			this.waveOutLock = waveOutLock;
			header = new WaveHeader();
			hHeader = GCHandle.Alloc(header, GCHandleType.Pinned);
			header.dataBuffer = hBuffer.AddrOfPinnedObject();
			header.bufferLength = bufferSize;
			header.loops = 1;
			hThis = GCHandle.Alloc(this);
			header.userData = (IntPtr)hThis;
			lock (waveOutLock)
			{
				MmException.Try(WaveInterop.waveOutPrepareHeader(hWaveOut, header, Marshal.SizeOf(header)), "waveOutPrepareHeader");
			}
		}

		~WaveOutBuffer()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		protected void Dispose(bool disposing)
		{
			if (hHeader.IsAllocated)
			{
				hHeader.Free();
			}
			if (hBuffer.IsAllocated)
			{
				hBuffer.Free();
			}
			if (hThis.IsAllocated)
			{
				hThis.Free();
			}
			if (hWaveOut != IntPtr.Zero)
			{
				lock (waveOutLock)
				{
					WaveInterop.waveOutUnprepareHeader(hWaveOut, header, Marshal.SizeOf(header));
				}
				hWaveOut = IntPtr.Zero;
			}
		}

		internal bool OnDone()
		{
			int num;
			lock (waveStream)
			{
				num = waveStream.Read(buffer, 0, buffer.Length);
			}
			if (num == 0)
			{
				return false;
			}
			for (int i = num; i < buffer.Length; i++)
			{
				buffer[i] = 0;
			}
			WriteToWaveOut();
			return true;
		}

		private void WriteToWaveOut()
		{
			MmResult mmResult;
			lock (waveOutLock)
			{
				mmResult = WaveInterop.waveOutWrite(hWaveOut, header, Marshal.SizeOf(header));
			}
			if (mmResult != 0)
			{
				throw new MmException(mmResult, "waveOutWrite");
			}
			GC.KeepAlive(this);
		}
	}
	public abstract class WaveStream : Stream, IWaveProvider
	{
		public abstract WaveFormat WaveFormat { get; }

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public virtual int BlockAlign => WaveFormat.BlockAlign;

		public virtual TimeSpan CurrentTime
		{
			get
			{
				return TimeSpan.FromSeconds((double)Position / (double)WaveFormat.AverageBytesPerSecond);
			}
			set
			{
				Position = (long)(value.TotalSeconds * (double)WaveFormat.AverageBytesPerSecond);
			}
		}

		public virtual TimeSpan TotalTime => TimeSpan.FromSeconds((double)Length / (double)WaveFormat.AverageBytesPerSecond);

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				Position = offset;
				break;
			case SeekOrigin.Current:
				Position += offset;
				break;
			default:
				Position = Length + offset;
				break;
			}
			return Position;
		}

		public override void SetLength(long length)
		{
			throw new NotSupportedException("Can't set length of a WaveFormatString");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Can't write to a WaveFormatString");
		}

		public void Skip(int seconds)
		{
			long num = Position + WaveFormat.AverageBytesPerSecond * seconds;
			if (num > Length)
			{
				Position = Length;
			}
			else if (num < 0)
			{
				Position = 0L;
			}
			else
			{
				Position = num;
			}
		}

		public virtual bool HasData(int count)
		{
			return Position < Length;
		}
	}
}
namespace NAudio.Wave.WaveFormats
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class WmaWaveFormat : WaveFormat
	{
		private short wValidBitsPerSample;

		private int dwChannelMask;

		private int dwReserved1;

		private int dwReserved2;

		private short wEncodeOptions;

		private short wReserved3;

		public WmaWaveFormat(int sampleRate, int bitsPerSample, int channels)
			: base(sampleRate, bitsPerSample, channels)
		{
			wValidBitsPerSample = (short)bitsPerSample;
			switch (channels)
			{
			case 1:
				dwChannelMask = 1;
				break;
			case 2:
				dwChannelMask = 3;
				break;
			}
			waveFormatTag = WaveFormatEncoding.WindowsMediaAudio;
		}
	}
}
namespace NAudio.Wave.SampleProviders
{
	internal interface ISampleChunkConverter
	{
		bool Supports(WaveFormat format);

		void LoadNextChunk(IWaveProvider sourceProvider, int samplePairsRequired);

		bool GetNextSample(out float sampleLeft, out float sampleRight);
	}
	internal class Mono16SampleChunkConverter : ISampleChunkConverter
	{
		private int sourceSample;

		private byte[] sourceBuffer;

		private WaveBuffer sourceWaveBuffer;

		private int sourceSamples;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 16)
			{
				return waveFormat.Channels == 1;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 2;
			sourceSample = 0;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceWaveBuffer = new WaveBuffer(sourceBuffer);
			sourceSamples = source.Read(sourceBuffer, 0, num) / 2;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (sourceSample < sourceSamples)
			{
				sampleLeft = (float)sourceWaveBuffer.ShortBuffer[sourceSample++] / 32768f;
				sampleRight = sampleLeft;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class Mono24SampleChunkConverter : ISampleChunkConverter
	{
		private int offset;

		private byte[] sourceBuffer;

		private int sourceBytes;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 24)
			{
				return waveFormat.Channels == 1;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 3;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceBytes = source.Read(sourceBuffer, 0, num);
			offset = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (offset < sourceBytes)
			{
				sampleLeft = (float)(((sbyte)sourceBuffer[offset + 2] << 16) | (sourceBuffer[offset + 1] << 8) | sourceBuffer[offset]) / 8388608f;
				offset += 3;
				sampleRight = sampleLeft;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class Mono8SampleChunkConverter : ISampleChunkConverter
	{
		private int offset;

		private byte[] sourceBuffer;

		private int sourceBytes;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 8)
			{
				return waveFormat.Channels == 1;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, samplePairsRequired);
			sourceBytes = source.Read(sourceBuffer, 0, samplePairsRequired);
			offset = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (offset < sourceBytes)
			{
				sampleLeft = (float)(int)sourceBuffer[offset] / 256f;
				offset++;
				sampleRight = sampleLeft;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class MonoFloatSampleChunkConverter : ISampleChunkConverter
	{
		private int sourceSample;

		private byte[] sourceBuffer;

		private WaveBuffer sourceWaveBuffer;

		private int sourceSamples;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				return waveFormat.Channels == 1;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 4;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceWaveBuffer = new WaveBuffer(sourceBuffer);
			sourceSamples = source.Read(sourceBuffer, 0, num) / 4;
			sourceSample = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (sourceSample < sourceSamples)
			{
				sampleLeft = sourceWaveBuffer.FloatBuffer[sourceSample++];
				sampleRight = sampleLeft;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class Stereo16SampleChunkConverter : ISampleChunkConverter
	{
		private int sourceSample;

		private byte[] sourceBuffer;

		private WaveBuffer sourceWaveBuffer;

		private int sourceSamples;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 16)
			{
				return waveFormat.Channels == 2;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 4;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceWaveBuffer = new WaveBuffer(sourceBuffer);
			sourceSamples = source.Read(sourceBuffer, 0, num) / 2;
			sourceSample = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (sourceSample < sourceSamples)
			{
				sampleLeft = (float)sourceWaveBuffer.ShortBuffer[sourceSample++] / 32768f;
				sampleRight = (float)sourceWaveBuffer.ShortBuffer[sourceSample++] / 32768f;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class Stereo24SampleChunkConverter : ISampleChunkConverter
	{
		private int offset;

		private byte[] sourceBuffer;

		private int sourceBytes;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 24)
			{
				return waveFormat.Channels == 2;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 6;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceBytes = source.Read(sourceBuffer, 0, num);
			offset = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (offset < sourceBytes)
			{
				sampleLeft = (float)(((sbyte)sourceBuffer[offset + 2] << 16) | (sourceBuffer[offset + 1] << 8) | sourceBuffer[offset]) / 8388608f;
				offset += 3;
				sampleRight = (float)(((sbyte)sourceBuffer[offset + 2] << 16) | (sourceBuffer[offset + 1] << 8) | sourceBuffer[offset]) / 8388608f;
				offset += 3;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class Stereo8SampleChunkConverter : ISampleChunkConverter
	{
		private int offset;

		private byte[] sourceBuffer;

		private int sourceBytes;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 8)
			{
				return waveFormat.Channels == 2;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceBytes = source.Read(sourceBuffer, 0, num);
			offset = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (offset < sourceBytes)
			{
				sampleLeft = (float)(int)sourceBuffer[offset++] / 256f;
				sampleRight = (float)(int)sourceBuffer[offset++] / 256f;
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	internal class StereoFloatSampleChunkConverter : ISampleChunkConverter
	{
		private int sourceSample;

		private byte[] sourceBuffer;

		private WaveBuffer sourceWaveBuffer;

		private int sourceSamples;

		public bool Supports(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				return waveFormat.Channels == 2;
			}
			return false;
		}

		public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
		{
			int num = samplePairsRequired * 8;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			sourceWaveBuffer = new WaveBuffer(sourceBuffer);
			sourceSamples = source.Read(sourceBuffer, 0, num) / 4;
			sourceSample = 0;
		}

		public bool GetNextSample(out float sampleLeft, out float sampleRight)
		{
			if (sourceSample < sourceSamples)
			{
				sampleLeft = sourceWaveBuffer.FloatBuffer[sourceSample++];
				sampleRight = sourceWaveBuffer.FloatBuffer[sourceSample++];
				return true;
			}
			sampleLeft = 0f;
			sampleRight = 0f;
			return false;
		}
	}
	public class AdsrSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		private readonly EnvelopeGenerator adsr;

		private float attackSeconds;

		private float releaseSeconds;

		public float AttackSeconds
		{
			get
			{
				return attackSeconds;
			}
			set
			{
				attackSeconds = value;
				adsr.AttackRate = attackSeconds * (float)WaveFormat.SampleRate;
			}
		}

		public float ReleaseSeconds
		{
			get
			{
				return releaseSeconds;
			}
			set
			{
				releaseSeconds = value;
				adsr.ReleaseRate = releaseSeconds * (float)WaveFormat.SampleRate;
			}
		}

		public WaveFormat WaveFormat => source.WaveFormat;

		public AdsrSampleProvider(ISampleProvider source)
		{
			if (source.WaveFormat.Channels > 1)
			{
				throw new ArgumentException("Currently only supports mono inputs");
			}
			this.source = source;
			adsr = new EnvelopeGenerator();
			AttackSeconds = 0.01f;
			adsr.SustainLevel = 1f;
			adsr.DecayRate = 0f * (float)WaveFormat.SampleRate;
			ReleaseSeconds = 0.3f;
			adsr.Gate(gate: true);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			if (adsr.State == EnvelopeGenerator.EnvelopeState.Idle)
			{
				return 0;
			}
			int num = source.Read(buffer, offset, count);
			for (int i = 0; i < num; i++)
			{
				buffer[offset++] *= adsr.Process();
			}
			return num;
		}

		public void Stop()
		{
			adsr.Gate(gate: false);
		}
	}
	public class ConcatenatingSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider[] providers;

		private int currentProviderIndex;

		public WaveFormat WaveFormat => providers[0].WaveFormat;

		public ConcatenatingSampleProvider(IEnumerable<ISampleProvider> providers)
		{
			if (providers == null)
			{
				throw new ArgumentNullException("providers");
			}
			this.providers = providers.ToArray();
			if (this.providers.Length == 0)
			{
				throw new ArgumentException("Must provide at least one input", "providers");
			}
			if (this.providers.Any((ISampleProvider p) => p.WaveFormat.Channels != WaveFormat.Channels))
			{
				throw new ArgumentException("All inputs must have the same channel count", "providers");
			}
			if (this.providers.Any((ISampleProvider p) => p.WaveFormat.SampleRate != WaveFormat.SampleRate))
			{
				throw new ArgumentException("All inputs must have the same sample rate", "providers");
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = 0;
			while (num < count && currentProviderIndex < providers.Length)
			{
				int count2 = count - num;
				int num2 = providers[currentProviderIndex].Read(buffer, num, count2);
				num += num2;
				if (num2 == 0)
				{
					currentProviderIndex++;
				}
			}
			return num;
		}
	}
	public class FadeInOutSampleProvider : ISampleProvider
	{
		private enum FadeState
		{
			Silence,
			FadingIn,
			FullVolume,
			FadingOut
		}

		private readonly object lockObject = new object();

		private readonly ISampleProvider source;

		private int fadeSamplePosition;

		private int fadeSampleCount;

		private FadeState fadeState;

		public WaveFormat WaveFormat => source.WaveFormat;

		public FadeInOutSampleProvider(ISampleProvider source, bool initiallySilent = false)
		{
			this.source = source;
			fadeState = ((!initiallySilent) ? FadeState.FullVolume : FadeState.Silence);
		}

		public void BeginFadeIn(double fadeDurationInMilliseconds)
		{
			lock (lockObject)
			{
				fadeSamplePosition = 0;
				fadeSampleCount = (int)(fadeDurationInMilliseconds * (double)source.WaveFormat.SampleRate / 1000.0);
				fadeState = FadeState.FadingIn;
			}
		}

		public void BeginFadeOut(double fadeDurationInMilliseconds)
		{
			lock (lockObject)
			{
				fadeSamplePosition = 0;
				fadeSampleCount = (int)(fadeDurationInMilliseconds * (double)source.WaveFormat.SampleRate / 1000.0);
				fadeState = FadeState.FadingOut;
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = source.Read(buffer, offset, count);
			lock (lockObject)
			{
				if (fadeState == FadeState.FadingIn)
				{
					FadeIn(buffer, offset, num);
					return num;
				}
				if (fadeState == FadeState.FadingOut)
				{
					FadeOut(buffer, offset, num);
					return num;
				}
				if (fadeState == FadeState.Silence)
				{
					ClearBuffer(buffer, offset, count);
					return num;
				}
				return num;
			}
		}

		private static void ClearBuffer(float[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				buffer[i + offset] = 0f;
			}
		}

		private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
		{
			int num = 0;
			while (num < sourceSamplesRead)
			{
				float num2 = 1f - (float)fadeSamplePosition / (float)fadeSampleCount;
				for (int i = 0; i < source.WaveFormat.Channels; i++)
				{
					buffer[offset + num++] *= num2;
				}
				fadeSamplePosition++;
				if (fadeSamplePosition > fadeSampleCount)
				{
					fadeState = FadeState.Silence;
					ClearBuffer(buffer, num + offset, sourceSamplesRead - num);
					break;
				}
			}
		}

		private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
		{
			int num = 0;
			while (num < sourceSamplesRead)
			{
				float num2 = (float)fadeSamplePosition / (float)fadeSampleCount;
				for (int i = 0; i < source.WaveFormat.Channels; i++)
				{
					buffer[offset + num++] *= num2;
				}
				fadeSamplePosition++;
				if (fadeSamplePosition > fadeSampleCount)
				{
					fadeState = FadeState.FullVolume;
					break;
				}
			}
		}
	}
	public class MeteringSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		private readonly float[] maxSamples;

		private int sampleCount;

		private readonly int channels;

		private readonly StreamVolumeEventArgs args;

		public int SamplesPerNotification { get; set; }

		public WaveFormat WaveFormat => source.WaveFormat;

		public event EventHandler<StreamVolumeEventArgs> StreamVolume;

		public MeteringSampleProvider(ISampleProvider source)
			: this(source, source.WaveFormat.SampleRate / 10)
		{
		}

		public MeteringSampleProvider(ISampleProvider source, int samplesPerNotification)
		{
			this.source = source;
			channels = source.WaveFormat.Channels;
			maxSamples = new float[channels];
			SamplesPerNotification = samplesPerNotification;
			args = new StreamVolumeEventArgs
			{
				MaxSampleValues = maxSamples
			};
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = source.Read(buffer, offset, count);
			if (this.StreamVolume != null)
			{
				for (int i = 0; i < num; i += channels)
				{
					for (int j = 0; j < channels; j++)
					{
						float val = Math.Abs(buffer[offset + i + j]);
						maxSamples[j] = Math.Max(maxSamples[j], val);
					}
					sampleCount++;
					if (sampleCount >= SamplesPerNotification)
					{
						this.StreamVolume(this, args);
						sampleCount = 0;
						Array.Clear(maxSamples, 0, channels);
					}
				}
			}
			return num;
		}
	}
	public class StreamVolumeEventArgs : EventArgs
	{
		public float[] MaxSampleValues { get; set; }
	}
	public class MixingSampleProvider : ISampleProvider
	{
		private readonly List<ISampleProvider> sources;

		private float[] sourceBuffer;

		private const int MaxInputs = 1024;

		public IEnumerable<ISampleProvider> MixerInputs => sources;

		public bool ReadFully { get; set; }

		public WaveFormat WaveFormat { get; private set; }

		public event EventHandler<SampleProviderEventArgs> MixerInputEnded;

		public MixingSampleProvider(WaveFormat waveFormat)
		{
			if (waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Mixer wave format must be IEEE float");
			}
			sources = new List<ISampleProvider>();
			WaveFormat = waveFormat;
		}

		public MixingSampleProvider(IEnumerable<ISampleProvider> sources)
		{
			this.sources = new List<ISampleProvider>();
			foreach (ISampleProvider source in sources)
			{
				AddMixerInput(source);
			}
			if (this.sources.Count == 0)
			{
				throw new ArgumentException("Must provide at least one input in this constructor");
			}
		}

		public void AddMixerInput(IWaveProvider mixerInput)
		{
			AddMixerInput(SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(mixerInput));
		}

		public void AddMixerInput(ISampleProvider mixerInput)
		{
			lock (sources)
			{
				if (sources.Count >= 1024)
				{
					throw new InvalidOperationException("Too many mixer inputs");
				}
				sources.Add(mixerInput);
			}
			if (WaveFormat == null)
			{
				WaveFormat = mixerInput.WaveFormat;
			}
			else if (WaveFormat.SampleRate != mixerInput.WaveFormat.SampleRate || WaveFormat.Channels != mixerInput.WaveFormat.Channels)
			{
				throw new ArgumentException("All mixer inputs must have the same WaveFormat");
			}
		}

		public void RemoveMixerInput(ISampleProvider mixerInput)
		{
			lock (sources)
			{
				sources.Remove(mixerInput);
			}
		}

		public void RemoveAllMixerInputs()
		{
			lock (sources)
			{
				sources.Clear();
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = 0;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, count);
			lock (sources)
			{
				for (int num2 = sources.Count - 1; num2 >= 0; num2--)
				{
					ISampleProvider sampleProvider = sources[num2];
					int num3 = sampleProvider.Read(sourceBuffer, 0, count);
					int num4 = offset;
					for (int i = 0; i < num3; i++)
					{
						if (i >= num)
						{
							buffer[num4++] = sourceBuffer[i];
						}
						else
						{
							buffer[num4++] += sourceBuffer[i];
						}
					}
					num = Math.Max(num3, num);
					if (num3 < count)
					{
						this.MixerInputEnded?.Invoke(this, new SampleProviderEventArgs(sampleProvider));
						sources.RemoveAt(num2);
					}
				}
			}
			if (ReadFully && num < count)
			{
				int num5 = offset + num;
				while (num5 < offset + count)
				{
					buffer[num5++] = 0f;
				}
				num = count;
			}
			return num;
		}
	}
	public class SampleProviderEventArgs : EventArgs
	{
		public ISampleProvider SampleProvider { get; private set; }

		public SampleProviderEventArgs(ISampleProvider sampleProvider)
		{
			SampleProvider = sampleProvider;
		}
	}
	public class MonoToStereoSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		private float[] sourceBuffer;

		public WaveFormat WaveFormat { get; }

		public float LeftVolume { get; set; }

		public float RightVolume { get; set; }

		public MonoToStereoSampleProvider(ISampleProvider source)
		{
			LeftVolume = 1f;
			RightVolume = 1f;
			if (source.WaveFormat.Channels != 1)
			{
				throw new ArgumentException("Source must be mono");
			}
			this.source = source;
			WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int count2 = count / 2;
			int num = offset;
			EnsureSourceBuffer(count2);
			int num2 = source.Read(sourceBuffer, 0, count2);
			for (int i = 0; i < num2; i++)
			{
				buffer[num++] = sourceBuffer[i] * LeftVolume;
				buffer[num++] = sourceBuffer[i] * RightVolume;
			}
			return num2 * 2;
		}

		private void EnsureSourceBuffer(int count)
		{
			if (sourceBuffer == null || sourceBuffer.Length < count)
			{
				sourceBuffer = new float[count];
			}
		}
	}
	public class MultiplexingSampleProvider : ISampleProvider
	{
		private readonly IList<ISampleProvider> inputs;

		private readonly WaveFormat waveFormat;

		private readonly int outputChannelCount;

		private readonly int inputChannelCount;

		private readonly List<int> mappings;

		private float[] inputBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public int InputChannelCount => inputChannelCount;

		public int OutputChannelCount => outputChannelCount;

		public MultiplexingSampleProvider(IEnumerable<ISampleProvider> inputs, int numberOfOutputChannels)
		{
			this.inputs = new List<ISampleProvider>(inputs);
			outputChannelCount = numberOfOutputChannels;
			if (this.inputs.Count == 0)
			{
				throw new ArgumentException("You must provide at least one input");
			}
			if (numberOfOutputChannels < 1)
			{
				throw new ArgumentException("You must provide at least one output");
			}
			foreach (ISampleProvider input in this.inputs)
			{
				if (waveFormat == null)
				{
					if (input.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
					{
						throw new ArgumentException("Only 32 bit float is supported");
					}
					waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(input.WaveFormat.SampleRate, numberOfOutputChannels);
				}
				else
				{
					if (input.WaveFormat.BitsPerSample != waveFormat.BitsPerSample)
					{
						throw new ArgumentException("All inputs must have the same bit depth");
					}
					if (input.WaveFormat.SampleRate != waveFormat.SampleRate)
					{
						throw new ArgumentException("All inputs must have the same sample rate");
					}
				}
				inputChannelCount += input.WaveFormat.Channels;
			}
			mappings = new List<int>();
			for (int i = 0; i < outputChannelCount; i++)
			{
				mappings.Add(i % inputChannelCount);
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = count / outputChannelCount;
			int num2 = 0;
			int num3 = 0;
			foreach (ISampleProvider input in inputs)
			{
				int num4 = num * input.WaveFormat.Channels;
				inputBuffer = BufferHelpers.Ensure(inputBuffer, num4);
				int num5 = input.Read(inputBuffer, 0, num4);
				num3 = Math.Max(num3, num5 / input.WaveFormat.Channels);
				for (int i = 0; i < input.WaveFormat.Channels; i++)
				{
					int num6 = num2 + i;
					for (int j = 0; j < outputChannelCount; j++)
					{
						if (mappings[j] != num6)
						{
							continue;
						}
						int num7 = i;
						int num8 = offset + j;
						int k;
						for (k = 0; k < num; k++)
						{
							if (num7 >= num5)
							{
								break;
							}
							buffer[num8] = inputBuffer[num7];
							num8 += outputChannelCount;
							num7 += input.WaveFormat.Channels;
						}
						for (; k < num; k++)
						{
							buffer[num8] = 0f;
							num8 += outputChannelCount;
						}
					}
				}
				num2 += input.WaveFormat.Channels;
			}
			return num3 * outputChannelCount;
		}

		public void ConnectInputToOutput(int inputChannel, int outputChannel)
		{
			if (inputChannel < 0 || inputChannel >= InputChannelCount)
			{
				throw new ArgumentException("Invalid input channel");
			}
			if (outputChannel < 0 || outputChannel >= OutputChannelCount)
			{
				throw new ArgumentException("Invalid output channel");
			}
			mappings[outputChannel] = inputChannel;
		}
	}
	public class NotifyingSampleProvider : ISampleProvider, ISampleNotifier
	{
		private readonly ISampleProvider source;

		private readonly SampleEventArgs sampleArgs = new SampleEventArgs(0f, 0f);

		private readonly int channels;

		public WaveFormat WaveFormat => source.WaveFormat;

		public event EventHandler<SampleEventArgs> Sample;

		public NotifyingSampleProvider(ISampleProvider source)
		{
			this.source = source;
			channels = WaveFormat.Channels;
		}

		public int Read(float[] buffer, int offset, int sampleCount)
		{
			int num = source.Read(buffer, offset, sampleCount);
			if (this.Sample != null)
			{
				for (int i = 0; i < num; i += channels)
				{
					sampleArgs.Left = buffer[offset + i];
					sampleArgs.Right = ((channels > 1) ? buffer[offset + i + 1] : sampleArgs.Left);
					this.Sample(this, sampleArgs);
				}
			}
			return num;
		}
	}
	public class OffsetSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider sourceProvider;

		private int phase;

		private int phasePos;

		private int delayBySamples;

		private int skipOverSamples;

		private int takeSamples;

		private int leadOutSamples;

		public int DelayBySamples
		{
			get
			{
				return delayBySamples;
			}
			set
			{
				if (phase != 0)
				{
					throw new InvalidOperationException("Can't set DelayBySamples after calling Read");
				}
				if (value % WaveFormat.Channels != 0)
				{
					throw new ArgumentException("DelayBySamples must be a multiple of WaveFormat.Channels");
				}
				delayBySamples = value;
			}
		}

		public TimeSpan DelayBy
		{
			get
			{
				return SamplesToTimeSpan(delayBySamples);
			}
			set
			{
				delayBySamples = TimeSpanToSamples(value);
			}
		}

		public int SkipOverSamples
		{
			get
			{
				return skipOverSamples;
			}
			set
			{
				if (phase != 0)
				{
					throw new InvalidOperationException("Can't set SkipOverSamples after calling Read");
				}
				if (value % WaveFormat.Channels != 0)
				{
					throw new ArgumentException("SkipOverSamples must be a multiple of WaveFormat.Channels");
				}
				skipOverSamples = value;
			}
		}

		public TimeSpan SkipOver
		{
			get
			{
				return SamplesToTimeSpan(skipOverSamples);
			}
			set
			{
				skipOverSamples = TimeSpanToSamples(value);
			}
		}

		public int TakeSamples
		{
			get
			{
				return takeSamples;
			}
			set
			{
				if (phase != 0)
				{
					throw new InvalidOperationException("Can't set TakeSamples after calling Read");
				}
				if (value % WaveFormat.Channels != 0)
				{
					throw new ArgumentException("TakeSamples must be a multiple of WaveFormat.Channels");
				}
				takeSamples = value;
			}
		}

		public TimeSpan Take
		{
			get
			{
				return SamplesToTimeSpan(takeSamples);
			}
			set
			{
				takeSamples = TimeSpanToSamples(value);
			}
		}

		public int LeadOutSamples
		{
			get
			{
				return leadOutSamples;
			}
			set
			{
				if (phase != 0)
				{
					throw new InvalidOperationException("Can't set LeadOutSamples after calling Read");
				}
				if (value % WaveFormat.Channels != 0)
				{
					throw new ArgumentException("LeadOutSamples must be a multiple of WaveFormat.Channels");
				}
				leadOutSamples = value;
			}
		}

		public TimeSpan LeadOut
		{
			get
			{
				return SamplesToTimeSpan(leadOutSamples);
			}
			set
			{
				leadOutSamples = TimeSpanToSamples(value);
			}
		}

		public WaveFormat WaveFormat => sourceProvider.WaveFormat;

		private int TimeSpanToSamples(TimeSpan time)
		{
			return (int)(time.TotalSeconds * (double)WaveFormat.SampleRate) * WaveFormat.Channels;
		}

		private TimeSpan SamplesToTimeSpan(int samples)
		{
			return TimeSpan.FromSeconds((double)(samples / WaveFormat.Channels) / (double)WaveFormat.SampleRate);
		}

		public OffsetSampleProvider(ISampleProvider sourceProvider)
		{
			this.sourceProvider = sourceProvider;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = 0;
			if (phase == 0)
			{
				phase++;
			}
			if (phase == 1)
			{
				int num2 = Math.Min(count, DelayBySamples - phasePos);
				for (int i = 0; i < num2; i++)
				{
					buffer[offset + i] = 0f;
				}
				phasePos += num2;
				num += num2;
				if (phasePos >= DelayBySamples)
				{
					phase++;
					phasePos = 0;
				}
			}
			if (phase == 2)
			{
				if (SkipOverSamples > 0)
				{
					float[] array = new float[WaveFormat.SampleRate * WaveFormat.Channels];
					int num3;
					for (int j = 0; j < SkipOverSamples; j += num3)
					{
						int count2 = Math.Min(SkipOverSamples - j, array.Length);
						num3 = sourceProvider.Read(array, 0, count2);
						if (num3 == 0)
						{
							break;
						}
					}
				}
				phase++;
				phasePos = 0;
			}
			if (phase == 3)
			{
				int num4 = count - num;
				if (takeSamples != 0)
				{
					num4 = Math.Min(num4, takeSamples - phasePos);
				}
				int num5 = sourceProvider.Read(buffer, offset + num, num4);
				phasePos += num5;
				num += num5;
				if (num5 < num4 || (takeSamples > 0 && phasePos >= takeSamples))
				{
					phase++;
					phasePos = 0;
				}
			}
			if (phase == 4)
			{
				int num6 = Math.Min(count - num, LeadOutSamples - phasePos);
				for (int k = 0; k < num6; k++)
				{
					buffer[offset + num + k] = 0f;
				}
				phasePos += num6;
				num += num6;
				if (phasePos >= LeadOutSamples)
				{
					phase++;
					phasePos = 0;
				}
			}
			return num;
		}
	}
	public class PanningSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		private float pan;

		private float leftMultiplier;

		private float rightMultiplier;

		private readonly WaveFormat waveFormat;

		private float[] sourceBuffer;

		private IPanStrategy panStrategy;

		public float Pan
		{
			get
			{
				return pan;
			}
			set
			{
				if (value < -1f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("value", "Pan must be in the range -1 to 1");
				}
				pan = value;
				UpdateMultipliers();
			}
		}

		public IPanStrategy PanStrategy
		{
			get
			{
				return panStrategy;
			}
			set
			{
				panStrategy = value;
				UpdateMultipliers();
			}
		}

		public WaveFormat WaveFormat => waveFormat;

		public PanningSampleProvider(ISampleProvider source)
		{
			if (source.WaveFormat.Channels != 1)
			{
				throw new ArgumentException("Source sample provider must be mono");
			}
			this.source = source;
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
			panStrategy = new SinPanStrategy();
		}

		private void UpdateMultipliers()
		{
			StereoSamplePair multipliers = panStrategy.GetMultipliers(Pan);
			leftMultiplier = multipliers.Left;
			rightMultiplier = multipliers.Right;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = count / 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int num3 = offset;
			for (int i = 0; i < num2; i++)
			{
				buffer[num3++] = leftMultiplier * sourceBuffer[i];
				buffer[num3++] = rightMultiplier * sourceBuffer[i];
			}
			return num2 * 2;
		}
	}
	public struct StereoSamplePair
	{
		public float Left { get; set; }

		public float Right { get; set; }
	}
	public interface IPanStrategy
	{
		StereoSamplePair GetMultipliers(float pan);
	}
	public class StereoBalanceStrategy : IPanStrategy
	{
		public StereoSamplePair GetMultipliers(float pan)
		{
			float left = ((pan <= 0f) ? 1f : ((1f - pan) / 2f));
			float right = ((pan >= 0f) ? 1f : ((pan + 1f) / 2f));
			StereoSamplePair result = default(StereoSamplePair);
			result.Left = left;
			result.Right = right;
			return result;
		}
	}
	public class SquareRootPanStrategy : IPanStrategy
	{
		public StereoSamplePair GetMultipliers(float pan)
		{
			float num = (0f - pan + 1f) / 2f;
			float left = (float)Math.Sqrt(num);
			float right = (float)Math.Sqrt(1f - num);
			StereoSamplePair result = default(StereoSamplePair);
			result.Left = left;
			result.Right = right;
			return result;
		}
	}
	public class SinPanStrategy : IPanStrategy
	{
		private const float HalfPi = (float)Math.PI / 2f;

		public StereoSamplePair GetMultipliers(float pan)
		{
			float num = (0f - pan + 1f) / 2f;
			float left = (float)Math.Sin(num * ((float)Math.PI / 2f));
			float right = (float)Math.Cos(num * ((float)Math.PI / 2f));
			StereoSamplePair result = default(StereoSamplePair);
			result.Left = left;
			result.Right = right;
			return result;
		}
	}
	public class LinearPanStrategy : IPanStrategy
	{
		public StereoSamplePair GetMultipliers(float pan)
		{
			float num = (0f - pan + 1f) / 2f;
			float left = num;
			float right = 1f - num;
			StereoSamplePair result = default(StereoSamplePair);
			result.Left = left;
			result.Right = right;
			return result;
		}
	}
	public class Pcm16BitToSampleProvider : SampleProviderConverterBase
	{
		public Pcm16BitToSampleProvider(IWaveProvider source)
			: base(source)
		{
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int num = count * 2;
			EnsureSourceBuffer(num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int num3 = offset;
			for (int i = 0; i < num2; i += 2)
			{
				buffer[num3++] = (float)BitConverter.ToInt16(sourceBuffer, i) / 32768f;
			}
			return num2 / 2;
		}
	}
	public class Pcm24BitToSampleProvider : SampleProviderConverterBase
	{
		public Pcm24BitToSampleProvider(IWaveProvider source)
			: base(source)
		{
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int num = count * 3;
			EnsureSourceBuffer(num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int num3 = offset;
			for (int i = 0; i < num2; i += 3)
			{
				buffer[num3++] = (float)(((sbyte)sourceBuffer[i + 2] << 16) | (sourceBuffer[i + 1] << 8) | sourceBuffer[i]) / 8388608f;
			}
			return num2 / 3;
		}
	}
	public class Pcm32BitToSampleProvider : SampleProviderConverterBase
	{
		public Pcm32BitToSampleProvider(IWaveProvider source)
			: base(source)
		{
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int num = count * 4;
			EnsureSourceBuffer(num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int num3 = offset;
			for (int i = 0; i < num2; i += 4)
			{
				buffer[num3++] = (float)(((sbyte)sourceBuffer[i + 3] << 24) | (sourceBuffer[i + 2] << 16) | (sourceBuffer[i + 1] << 8) | sourceBuffer[i]) / 2.1474836E+09f;
			}
			return num2 / 4;
		}
	}
	public class Pcm8BitToSampleProvider : SampleProviderConverterBase
	{
		public Pcm8BitToSampleProvider(IWaveProvider source)
			: base(source)
		{
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			EnsureSourceBuffer(count);
			int num = source.Read(sourceBuffer, 0, count);
			int num2 = offset;
			for (int i = 0; i < num; i++)
			{
				buffer[num2++] = (float)(int)sourceBuffer[i] / 128f - 1f;
			}
			return num;
		}
	}
	public class SampleChannel : ISampleProvider
	{
		private readonly VolumeSampleProvider volumeProvider;

		private readonly MeteringSampleProvider preVolumeMeter;

		private readonly WaveFormat waveFormat;

		public WaveFormat WaveFormat => waveFormat;

		public float Volume
		{
			get
			{
				return volumeProvider.Volume;
			}
			set
			{
				volumeProvider.Volume = value;
			}
		}

		public event EventHandler<StreamVolumeEventArgs> PreVolumeMeter
		{
			add
			{
				preVolumeMeter.StreamVolume += value;
			}
			remove
			{
				preVolumeMeter.StreamVolume -= value;
			}
		}

		public SampleChannel(IWaveProvider waveProvider)
			: this(waveProvider, forceStereo: false)
		{
		}

		public SampleChannel(IWaveProvider waveProvider, bool forceStereo)
		{
			ISampleProvider sampleProvider = SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
			if (sampleProvider.WaveFormat.Channels == 1 && forceStereo)
			{
				sampleProvider = new MonoToStereoSampleProvider(sampleProvider);
			}
			waveFormat = sampleProvider.WaveFormat;
			preVolumeMeter = new MeteringSampleProvider(sampleProvider);
			volumeProvider = new VolumeSampleProvider(preVolumeMeter);
		}

		public int Read(float[] buffer, int offset, int sampleCount)
		{
			return volumeProvider.Read(buffer, offset, sampleCount);
		}
	}
	public abstract class SampleProviderConverterBase : ISampleProvider
	{
		protected IWaveProvider source;

		private readonly WaveFormat waveFormat;

		protected byte[] sourceBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public SampleProviderConverterBase(IWaveProvider source)
		{
			this.source = source;
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, source.WaveFormat.Channels);
		}

		public abstract int Read(float[] buffer, int offset, int count);

		protected void EnsureSourceBuffer(int sourceBytesRequired)
		{
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
		}
	}
	internal static class SampleProviderConverters
	{
		public static ISampleProvider ConvertWaveProviderIntoSampleProvider(IWaveProvider waveProvider)
		{
			if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				if (waveProvider.WaveFormat.BitsPerSample == 8)
				{
					return new Pcm8BitToSampleProvider(waveProvider);
				}
				if (waveProvider.WaveFormat.BitsPerSample == 16)
				{
					return new Pcm16BitToSampleProvider(waveProvider);
				}
				if (waveProvider.WaveFormat.BitsPerSample == 24)
				{
					return new Pcm24BitToSampleProvider(waveProvider);
				}
				if (waveProvider.WaveFormat.BitsPerSample == 32)
				{
					return new Pcm32BitToSampleProvider(waveProvider);
				}
				throw new InvalidOperationException("Unsupported bit depth");
			}
			if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				if (waveProvider.WaveFormat.BitsPerSample == 64)
				{
					return new WaveToSampleProvider64(waveProvider);
				}
				return new WaveToSampleProvider(waveProvider);
			}
			throw new ArgumentException("Unsupported source encoding");
		}
	}
	public class SampleToWaveProvider : IWaveProvider
	{
		private readonly ISampleProvider source;

		public WaveFormat WaveFormat => source.WaveFormat;

		public SampleToWaveProvider(ISampleProvider source)
		{
			if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be already floating point");
			}
			this.source = source;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int count2 = count / 4;
			WaveBuffer waveBuffer = new WaveBuffer(buffer);
			return source.Read(waveBuffer.FloatBuffer, offset / 4, count2) * 4;
		}
	}
	public class SampleToWaveProvider16 : IWaveProvider
	{
		private readonly ISampleProvider sourceProvider;

		private readonly WaveFormat waveFormat;

		private volatile float volume;

		private float[] sourceBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public SampleToWaveProvider16(ISampleProvider sourceProvider)
		{
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Input source provider must be IEEE float", "sourceProvider");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Input source provider must be 32 bit", "sourceProvider");
			}
			waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);
			this.sourceProvider = sourceProvider;
			volume = 1f;
		}

		public int Read(byte[] destBuffer, int offset, int numBytes)
		{
			int num = numBytes / 2;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			int num2 = sourceProvider.Read(sourceBuffer, 0, num);
			WaveBuffer waveBuffer = new WaveBuffer(destBuffer);
			int num3 = offset / 2;
			for (int i = 0; i < num2; i++)
			{
				float num4 = sourceBuffer[i] * volume;
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				if (num4 < -1f)
				{
					num4 = -1f;
				}
				waveBuffer.ShortBuffer[num3++] = (short)(num4 * 32767f);
			}
			return num2 * 2;
		}
	}
	public class SampleToWaveProvider24 : IWaveProvider
	{
		private readonly ISampleProvider sourceProvider;

		private readonly WaveFormat waveFormat;

		private volatile float volume;

		private float[] sourceBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public SampleToWaveProvider24(ISampleProvider sourceProvider)
		{
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Input source provider must be IEEE float", "sourceProvider");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Input source provider must be 32 bit", "sourceProvider");
			}
			waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 24, sourceProvider.WaveFormat.Channels);
			this.sourceProvider = sourceProvider;
			volume = 1f;
		}

		public int Read(byte[] destBuffer, int offset, int numBytes)
		{
			int num = numBytes / 3;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
			int num2 = sourceProvider.Read(sourceBuffer, 0, num);
			int num3 = offset;
			for (int i = 0; i < num2; i++)
			{
				float num4 = sourceBuffer[i] * volume;
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				if (num4 < -1f)
				{
					num4 = -1f;
				}
				int num5 = (int)((double)num4 * 8388607.0);
				destBuffer[num3++] = (byte)num5;
				destBuffer[num3++] = (byte)(num5 >> 8);
				destBuffer[num3++] = (byte)(num5 >> 16);
			}
			return num2 * 3;
		}
	}
	public class SignalGenerator : ISampleProvider
	{
		private readonly WaveFormat waveFormat;

		private readonly Random random = new Random();

		private readonly double[] pinkNoiseBuffer = new double[7];

		private const double TwoPi = Math.PI * 2.0;

		private int nSample;

		private double phi;

		public WaveFormat WaveFormat => waveFormat;

		public double Frequency { get; set; }

		public double FrequencyLog => Math.Log(Frequency);

		public double FrequencyEnd { get; set; }

		public double FrequencyEndLog => Math.Log(FrequencyEnd);

		public double Gain { get; set; }

		public bool[] PhaseReverse { get; }

		public SignalGeneratorType Type { get; set; }

		public double SweepLengthSecs { get; set; }

		public SignalGenerator()
			: this(44100, 2)
		{
		}

		public SignalGenerator(int sampleRate, int channel)
		{
			phi = 0.0;
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channel);
			Type = SignalGeneratorType.Sin;
			Frequency = 440.0;
			Gain = 1.0;
			PhaseReverse = new bool[channel];
			SweepLengthSecs = 2.0;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = offset;
			for (int i = 0; i < count / waveFormat.Channels; i++)
			{
				double num2;
				switch (Type)
				{
				case SignalGeneratorType.Sin:
				{
					double num4 = Math.PI * 2.0 * Frequency / (double)waveFormat.SampleRate;
					num2 = Gain * Math.Sin((double)nSample * num4);
					nSample++;
					break;
				}
				case SignalGeneratorType.Square:
				{
					double num4 = 2.0 * Frequency / (double)waveFormat.SampleRate;
					double num7 = (double)nSample * num4 % 2.0 - 1.0;
					num2 = ((num7 > 0.0) ? Gain : (0.0 - Gain));
					nSample++;
					break;
				}
				case SignalGeneratorType.Triangle:
				{
					double num4 = 2.0 * Frequency / (double)waveFormat.SampleRate;
					double num7 = (double)nSample * num4 % 2.0;
					num2 = 2.0 * num7;
					if (num2 > 1.0)
					{
						num2 = 2.0 - num2;
					}
					if (num2 < -1.0)
					{
						num2 = -2.0 - num2;
					}
					num2 *= Gain;
					nSample++;
					break;
				}
				case SignalGeneratorType.SawTooth:
				{
					double num4 = 2.0 * Frequency / (double)waveFormat.SampleRate;
					double num7 = (double)nSample * num4 % 2.0 - 1.0;
					num2 = Gain * num7;
					nSample++;
					break;
				}
				case SignalGeneratorType.White:
					num2 = Gain * NextRandomTwo();
					break;
				case SignalGeneratorType.Pink:
				{
					double num5 = NextRandomTwo();
					pinkNoiseBuffer[0] = 0.99886 * pinkNoiseBuffer[0] + num5 * 0.0555179;
					pinkNoiseBuffer[1] = 0.99332 * pinkNoiseBuffer[1] + num5 * 0.0750759;
					pinkNoiseBuffer[2] = 0.969 * pinkNoiseBuffer[2] + num5 * 0.153852;
					pinkNoiseBuffer[3] = 0.8665 * pinkNoiseBuffer[3] + num5 * 0.3104856;
					pinkNoiseBuffer[4] = 0.55 * pinkNoiseBuffer[4] + num5 * 0.5329522;
					pinkNoiseBuffer[5] = -0.7616 * pinkNoiseBuffer[5] - num5 * 0.016898;
					double num6 = pinkNoiseBuffer[0] + pinkNoiseBuffer[1] + pinkNoiseBuffer[2] + pinkNoiseBuffer[3] + pinkNoiseBuffer[4] + pinkNoiseBuffer[5] + pinkNoiseBuffer[6] + num5 * 0.5362;
					pinkNoiseBuffer[6] = num5 * 0.115926;
					num2 = Gain * (num6 / 5.0);
					break;
				}
				case SignalGeneratorType.Sweep:
				{
					double num3 = Math.Exp(FrequencyLog + (double)nSample * (FrequencyEndLog - FrequencyLog) / (SweepLengthSecs * (double)waveFormat.SampleRate));
					double num4 = Math.PI * 2.0 * num3 / (double)waveFormat.SampleRate;
					phi += num4;
					num2 = Gain * Math.Sin(phi);
					nSample++;
					if ((double)nSample > SweepLengthSecs * (double)waveFormat.SampleRate)
					{
						nSample = 0;
						phi = 0.0;
					}
					break;
				}
				default:
					num2 = 0.0;
					break;
				}
				for (int j = 0; j < waveFormat.Channels; j++)
				{
					if (PhaseReverse[j])
					{
						buffer[num++] = (float)(0.0 - num2);
					}
					else
					{
						buffer[num++] = (float)num2;
					}
				}
			}
			return count;
		}

		private double NextRandomTwo()
		{
			return 2.0 * random.NextDouble() - 1.0;
		}
	}
	public enum SignalGeneratorType
	{
		Pink,
		White,
		Sweep,
		Sin,
		Square,
		Triangle,
		SawTooth
	}
	public class SmbPitchShiftingSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider sourceStream;

		private readonly WaveFormat waveFormat;

		private float pitch = 1f;

		private readonly int fftSize;

		private readonly long osamp;

		private readonly SmbPitchShifter shifterLeft = new SmbPitchShifter();

		private readonly SmbPitchShifter shifterRight = new SmbPitchShifter();

		private const float LIM_THRESH = 0.95f;

		private const float LIM_RANGE = 0.050000012f;

		private const float M_PI_2 = (float)Math.PI / 2f;

		public WaveFormat WaveFormat => waveFormat;

		public float PitchFactor
		{
			get
			{
				return pitch;
			}
			set
			{
				pitch = value;
			}
		}

		public SmbPitchShiftingSampleProvider(ISampleProvider sourceProvider)
			: this(sourceProvider, 4096, 4L, 1f)
		{
		}

		public SmbPitchShiftingSampleProvider(ISampleProvider sourceProvider, int fftSize, long osamp, float initialPitch)
		{
			sourceStream = sourceProvider;
			waveFormat = sourceProvider.WaveFormat;
			this.fftSize = fftSize;
			this.osamp = osamp;
			PitchFactor = initialPitch;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = sourceStream.Read(buffer, offset, count);
			if (pitch == 1f)
			{
				return num;
			}
			if (waveFormat.Channels == 1)
			{
				float[] array = new float[num];
				int num2 = 0;
				for (int i = offset; i <= num + offset - 1; i++)
				{
					array[num2] = buffer[i];
					num2++;
				}
				shifterLeft.PitchShift(pitch, num, fftSize, osamp, waveFormat.SampleRate, array);
				num2 = 0;
				for (int j = offset; j <= num + offset - 1; j++)
				{
					buffer[j] = Limiter(array[num2]);
					num2++;
				}
				return num;
			}
			if (waveFormat.Channels == 2)
			{
				float[] array2 = new float[num >> 1];
				float[] array3 = new float[num >> 1];
				int num3 = 0;
				for (int k = offset; k <= num + offset - 1; k += 2)
				{
					array2[num3] = buffer[k];
					array3[num3] = buffer[k + 1];
					num3++;
				}
				shifterLeft.PitchShift(pitch, num >> 1, fftSize, osamp, waveFormat.SampleRate, array2);
				shifterRight.PitchShift(pitch, num >> 1, fftSize, osamp, waveFormat.SampleRate, array3);
				num3 = 0;
				for (int l = offset; l <= num + offset - 1; l += 2)
				{
					buffer[l] = Limiter(array2[num3]);
					buffer[l + 1] = Limiter(array3[num3]);
					num3++;
				}
				return num;
			}
			throw new Exception("Shifting of more than 2 channels is currently not supported.");
		}

		private float Limiter(float sample)
		{
			if (0.95f < sample)
			{
				float num = (sample - 0.95f) / 0.050000012f;
				return (float)(Math.Atan(num) / 1.5707963705062866 * 0.050000011920928955 + 0.949999988079071);
			}
			if (sample < -0.95f)
			{
				float num = (0f - (sample + 0.95f)) / 0.050000012f;
				return 0f - (float)(Math.Atan(num) / 1.5707963705062866 * 0.050000011920928955 + 0.949999988079071);
			}
			return sample;
		}
	}
	public class StereoToMonoSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider sourceProvider;

		private float[] sourceBuffer;

		public float LeftVolume { get; set; }

		public float RightVolume { get; set; }

		public WaveFormat WaveFormat { get; }

		public StereoToMonoSampleProvider(ISampleProvider sourceProvider)
		{
			LeftVolume = 0.5f;
			RightVolume = 0.5f;
			if (sourceProvider.WaveFormat.Channels != 2)
			{
				throw new ArgumentException("Source must be stereo");
			}
			this.sourceProvider = sourceProvider;
			WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, 1);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = count * 2;
			if (sourceBuffer == null || sourceBuffer.Length < num)
			{
				sourceBuffer = new float[num];
			}
			int num2 = sourceProvider.Read(sourceBuffer, 0, num);
			int num3 = offset;
			for (int i = 0; i < num2; i += 2)
			{
				float num4 = sourceBuffer[i];
				float num5 = sourceBuffer[i + 1];
				float num6 = num4 * LeftVolume + num5 * RightVolume;
				buffer[num3++] = num6;
			}
			return num2 / 2;
		}
	}
	public class VolumeSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		public WaveFormat WaveFormat => source.WaveFormat;

		public float Volume { get; set; }

		public VolumeSampleProvider(ISampleProvider source)
		{
			this.source = source;
			Volume = 1f;
		}

		public int Read(float[] buffer, int offset, int sampleCount)
		{
			int result = source.Read(buffer, offset, sampleCount);
			if (Volume != 1f)
			{
				for (int i = 0; i < sampleCount; i++)
				{
					buffer[offset + i] *= Volume;
				}
			}
			return result;
		}
	}
	public class WaveToSampleProvider : SampleProviderConverterBase
	{
		public WaveToSampleProvider(IWaveProvider source)
			: base(source)
		{
			if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be already floating point");
			}
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int num = count * 4;
			EnsureSourceBuffer(num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int result = num2 / 4;
			int num3 = offset;
			for (int i = 0; i < num2; i += 4)
			{
				buffer[num3++] = BitConverter.ToSingle(sourceBuffer, i);
			}
			return result;
		}
	}
	public class WaveToSampleProvider64 : SampleProviderConverterBase
	{
		public WaveToSampleProvider64(IWaveProvider source)
			: base(source)
		{
			if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be already floating point");
			}
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int num = count * 8;
			EnsureSourceBuffer(num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int result = num2 / 8;
			int num3 = offset;
			for (int i = 0; i < num2; i += 8)
			{
				long value = BitConverter.ToInt64(sourceBuffer, i);
				buffer[num3++] = (float)BitConverter.Int64BitsToDouble(value);
			}
			return result;
		}
	}
	public class WdlResamplingSampleProvider : ISampleProvider
	{
		private readonly WdlResampler resampler;

		private readonly WaveFormat outFormat;

		private readonly ISampleProvider source;

		private readonly int channels;

		public WaveFormat WaveFormat => outFormat;

		public WdlResamplingSampleProvider(ISampleProvider source, int newSampleRate)
		{
			channels = source.WaveFormat.Channels;
			outFormat = WaveFormat.CreateIeeeFloatWaveFormat(newSampleRate, channels);
			this.source = source;
			resampler = new WdlResampler();
			resampler.SetMode(interp: true, 2, sinc: false);
			resampler.SetFilterParms();
			resampler.SetFeedMode(wantInputDriven: false);
			resampler.SetRates(source.WaveFormat.SampleRate, newSampleRate);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = count / channels;
			float[] inbuffer;
			int inbufferOffset;
			int num2 = resampler.ResamplePrepare(num, outFormat.Channels, out inbuffer, out inbufferOffset);
			int nsamples_in = source.Read(inbuffer, inbufferOffset, num2 * channels) / channels;
			return resampler.ResampleOut(buffer, offset, nsamples_in, num, channels) * channels;
		}
	}
}
namespace NAudio.Wave.Compression
{
	public class AcmDriver : IDisposable
	{
		private static List<AcmDriver> drivers;

		private AcmDriverDetails details;

		private IntPtr driverId;

		private IntPtr driverHandle;

		private List<AcmFormatTag> formatTags;

		private List<AcmFormat> tempFormatsList;

		private IntPtr localDllHandle;

		public int MaxFormatSize
		{
			get
			{
				MmException.Try(AcmInterop.acmMetrics(driverHandle, AcmMetrics.MaxSizeFormat, out var output), "acmMetrics");
				return output;
			}
		}

		public string ShortName => details.shortName;

		public string LongName => details.longName;

		public IntPtr DriverId => driverId;

		public IEnumerable<AcmFormatTag> FormatTags
		{
			get
			{
				if (formatTags == null)
				{
					if (driverHandle == IntPtr.Zero)
					{
						throw new InvalidOperationException("Driver must be opened first");
					}
					formatTags = new List<AcmFormatTag>();
					AcmFormatTagDetails formatTagDetails = default(AcmFormatTagDetails);
					formatTagDetails.structureSize = Marshal.SizeOf(formatTagDetails);
					MmException.Try(AcmInterop.acmFormatTagEnum(driverHandle, ref formatTagDetails, AcmFormatTagEnumCallback, IntPtr.Zero, 0), "acmFormatTagEnum");
				}
				return formatTags;
			}
		}

		public static bool IsCodecInstalled(string shortName)
		{
			foreach (AcmDriver item in EnumerateAcmDrivers())
			{
				if (item.ShortName == shortName)
				{
					return true;
				}
			}
			return false;
		}

		public static AcmDriver AddLocalDriver(string driverFile)
		{
			IntPtr intPtr = NativeMethods.LoadLibrary(driverFile);
			if (intPtr == IntPtr.Zero)
			{
				throw new ArgumentException("Failed to load driver file");
			}
			IntPtr procAddress = NativeMethods.GetProcAddress(intPtr, "DriverProc");
			if (procAddress == IntPtr.Zero)
			{
				NativeMethods.FreeLibrary(intPtr);
				throw new ArgumentException("Failed to discover DriverProc");
			}
			IntPtr hAcmDriver;
			MmResult mmResult = AcmInterop.acmDriverAdd(out hAcmDriver, intPtr, procAddress, 0, AcmDriverAddFlags.Function);
			if (mmResult != 0)
			{
				NativeMethods.FreeLibrary(intPtr);
				throw new MmException(mmResult, "acmDriverAdd");
			}
			AcmDriver acmDriver = new AcmDriver(hAcmDriver);
			if (string.IsNullOrEmpty(acmDriver.details.longName))
			{
				acmDriver.details.longName = "Local driver: " + Path.GetFileName(driverFile);
				acmDriver.localDllHandle = intPtr;
			}
			return acmDriver;
		}

		public static void RemoveLocalDriver(AcmDriver localDriver)
		{
			if (localDriver.localDllHandle == IntPtr.Zero)
			{
				throw new ArgumentException("Please pass in the AcmDriver returned by the AddLocalDriver method");
			}
			MmResult result = AcmInterop.acmDriverRemove(localDriver.driverId, 0);
			NativeMethods.FreeLibrary(localDriver.localDllHandle);
			MmException.Try(result, "acmDriverRemove");
		}

		public static bool ShowFormatChooseDialog(IntPtr ownerWindowHandle, string windowTitle, AcmFormatEnumFlags enumFlags, WaveFormat enumFormat, out WaveFormat selectedFormat, out string selectedFormatDescription, out string selectedFormatTagDescription)
		{
			AcmFormatChoose formatChoose = default(AcmFormatChoose);
			formatChoose.structureSize = Marshal.SizeOf(formatChoose);
			formatChoose.styleFlags = AcmFormatChooseStyleFlags.None;
			formatChoose.ownerWindowHandle = ownerWindowHandle;
			int num = 200;
			formatChoose.selectedWaveFormatPointer = Marshal.AllocHGlobal(num);
			formatChoose.selectedWaveFormatByteSize = num;
			formatChoose.title = windowTitle;
			formatChoose.name = null;
			formatChoose.formatEnumFlags = enumFlags;
			formatChoose.waveFormatEnumPointer = IntPtr.Zero;
			if (enumFormat != null)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(enumFormat));
				Marshal.StructureToPtr(enumFormat, intPtr, fDeleteOld: false);
				formatChoose.waveFormatEnumPointer = intPtr;
			}
			formatChoose.instanceHandle = IntPtr.Zero;
			formatChoose.templateName = null;
			MmResult mmResult = AcmInterop.acmFormatChoose(ref formatChoose);
			selectedFormat = null;
			selectedFormatDescription = null;
			selectedFormatTagDescription = null;
			if (mmResult == MmResult.NoError)
			{
				selectedFormat = WaveFormat.MarshalFromPtr(formatChoose.selectedWaveFormatPointer);
				selectedFormatDescription = formatChoose.formatDescription;
				selectedFormatTagDescription = formatChoose.formatTagDescription;
			}
			Marshal.FreeHGlobal(formatChoose.waveFormatEnumPointer);
			Marshal.FreeHGlobal(formatChoose.selectedWaveFormatPointer);
			if (mmResult != MmResult.AcmCancelled && mmResult != 0)
			{
				throw new MmException(mmResult, "acmFormatChoose");
			}
			return mmResult == MmResult.NoError;
		}

		public static AcmDriver FindByShortName(string shortName)
		{
			foreach (AcmDriver item in EnumerateAcmDrivers())
			{
				if (item.ShortName == shortName)
				{
					return item;
				}
			}
			return null;
		}

		public static IEnumerable<AcmDriver> EnumerateAcmDrivers()
		{
			drivers = new List<AcmDriver>();
			MmException.Try(AcmInterop.acmDriverEnum(DriverEnumCallback, IntPtr.Zero, (AcmDriverEnumFlags)0), "acmDriverEnum");
			return drivers;
		}

		private static bool DriverEnumCallback(IntPtr hAcmDriver, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
		{
			drivers.Add(new AcmDriver(hAcmDriver));
			return true;
		}

		private AcmDriver(IntPtr hAcmDriver)
		{
			driverId = hAcmDriver;
			details = default(AcmDriverDetails);
			details.structureSize = Marshal.SizeOf(details);
			MmException.Try(AcmInterop.acmDriverDetails(hAcmDriver, ref details, 0), "acmDriverDetails");
		}

		public override string ToString()
		{
			return LongName;
		}

		public IEnumerable<AcmFormat> GetFormats(AcmFormatTag formatTag)
		{
			if (driverHandle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Driver must be opened first");
			}
			tempFormatsList = new List<AcmFormat>();
			AcmFormatDetails formatDetails = default(AcmFormatDetails);
			formatDetails.structSize = Marshal.SizeOf(formatDetails);
			formatDetails.waveFormatByteSize = 1024;
			formatDetails.waveFormatPointer = Marshal.AllocHGlobal(formatDetails.waveFormatByteSize);
			formatDetails.formatTag = (int)formatTag.FormatTag;
			MmResult result = AcmInterop.acmFormatEnum(driverHandle, ref formatDetails, AcmFormatEnumCallback, IntPtr.Zero, AcmFormatEnumFlags.None);
			Marshal.FreeHGlobal(formatDetails.waveFormatPointer);
			MmException.Try(result, "acmFormatEnum");
			return tempFormatsList;
		}

		public void Open()
		{
			if (driverHandle == IntPtr.Zero)
			{
				MmException.Try(AcmInterop.acmDriverOpen(out driverHandle, DriverId, 0), "acmDriverOpen");
			}
		}

		public void Close()
		{
			if (driverHandle != IntPtr.Zero)
			{
				MmException.Try(AcmInterop.acmDriverClose(driverHandle, 0), "acmDriverClose");
				driverHandle = IntPtr.Zero;
			}
		}

		private bool AcmFormatTagEnumCallback(IntPtr hAcmDriverId, ref AcmFormatTagDetails formatTagDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
		{
			formatTags.Add(new AcmFormatTag(formatTagDetails));
			return true;
		}

		private bool AcmFormatEnumCallback(IntPtr hAcmDriverId, ref AcmFormatDetails formatDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
		{
			tempFormatsList.Add(new AcmFormat(formatDetails));
			return true;
		}

		public void Dispose()
		{
			if (driverHandle != IntPtr.Zero)
			{
				Close();
				GC.SuppressFinalize(this);
			}
		}
	}
	internal enum AcmDriverAddFlags
	{
		Local = 0,
		Global = 8,
		Function = 3,
		NotifyWindowHandle = 4
	}
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct AcmDriverDetails
	{
		public int structureSize;

		public uint fccType;

		public uint fccComp;

		public ushort manufacturerId;

		public ushort productId;

		public uint acmVersion;

		public uint driverVersion;

		public AcmDriverDetailsSupportFlags supportFlags;

		public int formatTagsCount;

		public int filterTagsCount;

		public IntPtr hicon;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string shortName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string longName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string copyright;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string licensing;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string features;

		private const int ShortNameChars = 32;

		private const int LongNameChars = 128;

		private const int CopyrightChars = 80;

		private const int LicensingChars = 128;

		private const int FeaturesChars = 512;
	}
	[Flags]
	public enum AcmDriverDetailsSupportFlags
	{
		Codec = 1,
		Converter = 2,
		Filter = 4,
		Hardware = 8,
		Async = 0x10,
		Local = 0x40000000,
		Disabled = int.MinValue
	}
	[Flags]
	internal enum AcmDriverEnumFlags
	{
		NoLocal = 0x40000000,
		Disabled = int.MinValue
	}
	public class AcmFormat
	{
		private readonly AcmFormatDetails formatDetails;

		public int FormatIndex => formatDetails.formatIndex;

		public WaveFormatEncoding FormatTag => (WaveFormatEncoding)formatDetails.formatTag;

		public AcmDriverDetailsSupportFlags SupportFlags => formatDetails.supportFlags;

		public WaveFormat WaveFormat { get; private set; }

		public int WaveFormatByteSize => formatDetails.waveFormatByteSize;

		public string FormatDescription => formatDetails.formatDescription;

		internal AcmFormat(AcmFormatDetails formatDetails)
		{
			this.formatDetails = formatDetails;
			WaveFormat = WaveFormat.MarshalFromPtr(formatDetails.waveFormatPointer);
		}
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
	internal struct AcmFormatChoose
	{
		public int structureSize;

		public AcmFormatChooseStyleFlags styleFlags;

		public IntPtr ownerWindowHandle;

		public IntPtr selectedWaveFormatPointer;

		public int selectedWaveFormatByteSize;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string title;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
		public string formatTagDescription;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string formatDescription;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string name;

		public int nameByteSize;

		public AcmFormatEnumFlags formatEnumFlags;

		public IntPtr waveFormatEnumPointer;

		public IntPtr instanceHandle;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string templateName;

		public IntPtr customData;

		public AcmInterop.AcmFormatChooseHookProc windowCallbackFunction;
	}
	[Flags]
	internal enum AcmFormatChooseStyleFlags
	{
		None = 0,
		ShowHelp = 4,
		EnableHook = 8,
		EnableTemplate = 0x10,
		EnableTemplateHandle = 0x20,
		InitToWfxStruct = 0x40,
		ContextHelp = 0x80
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct AcmFormatDetails
	{
		public int structSize;

		public int formatIndex;

		public int formatTag;

		public AcmDriverDetailsSupportFlags supportFlags;

		public IntPtr waveFormatPointer;

		public int waveFormatByteSize;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string formatDescription;

		public const int FormatDescriptionChars = 128;
	}
	[Flags]
	public enum AcmFormatEnumFlags
	{
		None = 0,
		Convert = 0x100000,
		Hardware = 0x400000,
		Input = 0x800000,
		Channels = 0x20000,
		SamplesPerSecond = 0x40000,
		Output = 0x1000000,
		Suggest = 0x200000,
		BitsPerSample = 0x80000,
		FormatTag = 0x10000
	}
	[Flags]
	internal enum AcmFormatSuggestFlags
	{
		FormatTag = 0x10000,
		Channels = 0x20000,
		SamplesPerSecond = 0x40000,
		BitsPerSample = 0x80000,
		TypeMask = 0xFF0000
	}
	public class AcmFormatTag
	{
		private AcmFormatTagDetails formatTagDetails;

		public int FormatTagIndex => formatTagDetails.formatTagIndex;

		public WaveFormatEncoding FormatTag => (WaveFormatEncoding)formatTagDetails.formatTag;

		public int FormatSize => formatTagDetails.formatSize;

		public AcmDriverDetailsSupportFlags SupportFlags => formatTagDetails.supportFlags;

		public int StandardFormatsCount => formatTagDetails.standardFormatsCount;

		public string FormatDescription => formatTagDetails.formatDescription;

		internal AcmFormatTag(AcmFormatTagDetails formatTagDetails)
		{
			this.formatTagDetails = formatTagDetails;
		}
	}
	internal struct AcmFormatTagDetails
	{
		public int structureSize;

		public int formatTagIndex;

		public int formatTag;

		public int formatSize;

		public AcmDriverDetailsSupportFlags supportFlags;

		public int standardFormatsCount;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
		public string formatDescription;

		public const int FormatTagDescriptionChars = 48;
	}
	internal class AcmInterop
	{
		public delegate bool AcmDriverEnumCallback(IntPtr hAcmDriverId, IntPtr instance, AcmDriverDetailsSupportFlags flags);

		public delegate bool AcmFormatEnumCallback(IntPtr hAcmDriverId, ref AcmFormatDetails formatDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags);

		public delegate bool AcmFormatTagEnumCallback(IntPtr hAcmDriverId, ref AcmFormatTagDetails formatTagDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags);

		public delegate bool AcmFormatChooseHookProc(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam);

		[DllImport("msacm32.dll")]
		public static extern MmResult acmDriverAdd(out IntPtr driverHandle, IntPtr driverModule, IntPtr driverFunctionAddress, int priority, AcmDriverAddFlags flags);

		[DllImport("msacm32.dll")]
		public static extern MmResult acmDriverRemove(IntPtr driverHandle, int removeFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverClose(IntPtr hAcmDriver, int closeFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverEnum(AcmDriverEnumCallback fnCallback, IntPtr dwInstance, AcmDriverEnumFlags flags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverDetails(IntPtr hAcmDriver, ref AcmDriverDetails driverDetails, int reserved);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverOpen(out IntPtr pAcmDriver, IntPtr hAcmDriverId, int openFlags);

		[DllImport("Msacm32.dll", EntryPoint = "acmFormatChooseW")]
		public static extern MmResult acmFormatChoose(ref AcmFormatChoose formatChoose);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmFormatEnum(IntPtr hAcmDriver, ref AcmFormatDetails formatDetails, AcmFormatEnumCallback callback, IntPtr instance, AcmFormatEnumFlags flags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmFormatSuggest(IntPtr hAcmDriver, [In][MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat sourceFormat, [In][Out][MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat destFormat, int sizeDestFormat, AcmFormatSuggestFlags suggestFlags);

		[DllImport("Msacm32.dll", EntryPoint = "acmFormatSuggest")]
		public static extern MmResult acmFormatSuggest2(IntPtr hAcmDriver, IntPtr sourceFormatPointer, IntPtr destFormatPointer, int sizeDestFormat, AcmFormatSuggestFlags suggestFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmFormatTagEnum(IntPtr hAcmDriver, ref AcmFormatTagDetails formatTagDetails, AcmFormatTagEnumCallback callback, IntPtr instance, int reserved);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmMetrics(IntPtr hAcmObject, AcmMetrics metric, out int output);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamOpen(out IntPtr hAcmStream, IntPtr hAcmDriver, [In][MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat sourceFormat, [In][MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat destFormat, [In] WaveFilter waveFilter, IntPtr callback, IntPtr instance, AcmStreamOpenFlags openFlags);

		[DllImport("Msacm32.dll", EntryPoint = "acmStreamOpen")]
		public static extern MmResult acmStreamOpen2(out IntPtr hAcmStream, IntPtr hAcmDriver, IntPtr sourceFormatPointer, IntPtr destFormatPointer, [In] WaveFilter waveFilter, IntPtr callback, IntPtr instance, AcmStreamOpenFlags openFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamClose(IntPtr hAcmStream, int closeFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamConvert(IntPtr hAcmStream, [In][Out] AcmStreamHeaderStruct streamHeader, AcmStreamConvertFlags streamConvertFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamPrepareHeader(IntPtr hAcmStream, [In][Out] AcmStreamHeaderStruct streamHeader, int prepareFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamReset(IntPtr hAcmStream, int resetFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamSize(IntPtr hAcmStream, int inputBufferSize, out int outputBufferSize, AcmStreamSizeFlags flags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamUnprepareHeader(IntPtr hAcmStream, [In][Out] AcmStreamHeaderStruct streamHeader, int flags);
	}
	public class AcmStream : IDisposable
	{
		private IntPtr streamHandle;

		private IntPtr driverHandle;

		private AcmStreamHeader streamHeader;

		private readonly WaveFormat sourceFormat;

		public byte[] SourceBuffer => streamHeader.SourceBuffer;

		public byte[] DestBuffer => streamHeader.DestBuffer;

		public AcmStream(WaveFormat sourceFormat, WaveFormat destFormat)
		{
			try
			{
				streamHandle = IntPtr.Zero;
				this.sourceFormat = sourceFormat;
				int num = Math.Max(65536, sourceFormat.AverageBytesPerSecond);
				num -= num % sourceFormat.BlockAlign;
				IntPtr intPtr = WaveFormat.MarshalToPtr(sourceFormat);
				IntPtr intPtr2 = WaveFormat.MarshalToPtr(destFormat);
				try
				{
					MmException.Try(AcmInterop.acmStreamOpen2(out streamHandle, IntPtr.Zero, intPtr, intPtr2, null, IntPtr.Zero, IntPtr.Zero, AcmStreamOpenFlags.NonRealTime), "acmStreamOpen");
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
					Marshal.FreeHGlobal(intPtr2);
				}
				int destBufferLength = SourceToDest(num);
				streamHeader = new AcmStreamHeader(streamHandle, num, destBufferLength);
				driverHandle = IntPtr.Zero;
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public AcmStream(IntPtr driverId, WaveFormat sourceFormat, WaveFilter waveFilter)
		{
			int num = Math.Max(16384, sourceFormat.AverageBytesPerSecond);
			this.sourceFormat = sourceFormat;
			num -= num % sourceFormat.BlockAlign;
			MmException.Try(AcmInterop.acmDriverOpen(out driverHandle, driverId, 0), "acmDriverOpen");
			IntPtr intPtr = WaveFormat.MarshalToPtr(sourceFormat);
			try
			{
				MmException.Try(AcmInterop.acmStreamOpen2(out streamHandle, driverHandle, intPtr, intPtr, waveFilter, IntPtr.Zero, IntPtr.Zero, AcmStreamOpenFlags.NonRealTime), "acmStreamOpen");
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			streamHeader = new AcmStreamHeader(streamHandle, num, SourceToDest(num));
		}

		public int SourceToDest(int source)
		{
			if (source == 0)
			{
				return 0;
			}
			MmException.Try(AcmInterop.acmStreamSize(streamHandle, source, out var outputBufferSize, AcmStreamSizeFlags.Source), "acmStreamSize");
			return outputBufferSize;
		}

		public int DestToSource(int dest)
		{
			if (dest == 0)
			{
				return 0;
			}
			MmException.Try(AcmInterop.acmStreamSize(streamHandle, dest, out var outputBufferSize, AcmStreamSizeFlags.Destination), "acmStreamSize");
			return outputBufferSize;
		}

		public static WaveFormat SuggestPcmFormat(WaveFormat compressedFormat)
		{
			WaveFormat waveFormat = new WaveFormat(compressedFormat.SampleRate, 16, compressedFormat.Channels);
			IntPtr intPtr = WaveFormat.MarshalToPtr(waveFormat);
			IntPtr intPtr2 = WaveFormat.MarshalToPtr(compressedFormat);
			try
			{
				MmResult result = AcmInterop.acmFormatSuggest2(IntPtr.Zero, intPtr2, intPtr, Marshal.SizeOf(waveFormat), AcmFormatSuggestFlags.FormatTag);
				waveFormat = WaveFormat.MarshalFromPtr(intPtr);
				MmException.Try(result, "acmFormatSuggest");
				return waveFormat;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
				Marshal.FreeHGlobal(intPtr2);
			}
		}

		public void Reposition()
		{
			streamHeader.Reposition();
		}

		public int Convert(int bytesToConvert, out int sourceBytesConverted)
		{
			if (bytesToConvert % sourceFormat.BlockAlign != 0)
			{
				bytesToConvert -= bytesToConvert % sourceFormat.BlockAlign;
			}
			return streamHeader.Convert(bytesToConvert, out sourceBytesConverted);
		}

		[Obsolete("Call the version returning sourceBytesConverted instead")]
		public int Convert(int bytesToConvert)
		{
			int sourceBytesConverted;
			int result = Convert(bytesToConvert, out sourceBytesConverted);
			if (sourceBytesConverted != bytesToConvert)
			{
				throw new MmException(MmResult.NotSupported, "AcmStreamHeader.Convert didn't convert everything");
			}
			return result;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && streamHeader != null)
			{
				streamHeader.Dispose();
				streamHeader = null;
			}
			if (streamHandle != IntPtr.Zero)
			{
				MmResult mmResult = AcmInterop.acmStreamClose(streamHandle, 0);
				streamHandle = IntPtr.Zero;
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "acmStreamClose");
				}
			}
			if (driverHandle != IntPtr.Zero)
			{
				AcmInterop.acmDriverClose(driverHandle, 0);
				driverHandle = IntPtr.Zero;
			}
		}

		~AcmStream()
		{
			Dispose(disposing: false);
		}
	}
	internal class AcmStreamHeader : IDisposable
	{
		private AcmStreamHeaderStruct streamHeader;

		private GCHandle hSourceBuffer;

		private GCHandle hDestBuffer;

		private IntPtr streamHandle;

		private bool firstTime;

		private bool disposed;

		public byte[] SourceBuffer { get; private set; }

		public byte[] DestBuffer { get; private set; }

		public AcmStreamHeader(IntPtr streamHandle, int sourceBufferLength, int destBufferLength)
		{
			streamHeader = new AcmStreamHeaderStruct();
			SourceBuffer = new byte[sourceBufferLength];
			hSourceBuffer = GCHandle.Alloc(SourceBuffer, GCHandleType.Pinned);
			DestBuffer = new byte[destBufferLength];
			hDestBuffer = GCHandle.Alloc(DestBuffer, GCHandleType.Pinned);
			this.streamHandle = streamHandle;
			firstTime = true;
		}

		private void Prepare()
		{
			streamHeader.cbStruct = Marshal.SizeOf(streamHeader);
			streamHeader.sourceBufferLength = SourceBuffer.Length;
			streamHeader.sourceBufferPointer = hSourceBuffer.AddrOfPinnedObject();
			streamHeader.destBufferLength = DestBuffer.Length;
			streamHeader.destBufferPointer = hDestBuffer.AddrOfPinnedObject();
			MmException.Try(AcmInterop.acmStreamPrepareHeader(streamHandle, streamHeader, 0), "acmStreamPrepareHeader");
		}

		private void Unprepare()
		{
			streamHeader.sourceBufferLength = SourceBuffer.Length;
			streamHeader.sourceBufferPointer = hSourceBuffer.AddrOfPinnedObject();
			streamHeader.destBufferLength = DestBuffer.Length;
			streamHeader.destBufferPointer = hDestBuffer.AddrOfPinnedObject();
			MmResult mmResult = AcmInterop.acmStreamUnprepareHeader(streamHandle, streamHeader, 0);
			if (mmResult != 0)
			{
				throw new MmException(mmResult, "acmStreamUnprepareHeader");
			}
		}

		public void Reposition()
		{
			firstTime = true;
		}

		public int Convert(int bytesToConvert, out int sourceBytesConverted)
		{
			Prepare();
			try
			{
				streamHeader.sourceBufferLength = bytesToConvert;
				streamHeader.sourceBufferLengthUsed = bytesToConvert;
				AcmStreamConvertFlags streamConvertFlags = (firstTime ? (AcmStreamConvertFlags.BlockAlign | AcmStreamConvertFlags.Start) : AcmStreamConvertFlags.BlockAlign);
				MmException.Try(AcmInterop.acmStreamConvert(streamHandle, streamHeader, streamConvertFlags), "acmStreamConvert");
				firstTime = false;
				sourceBytesConverted = streamHeader.sourceBufferLengthUsed;
			}
			finally
			{
				Unprepare();
			}
			return streamHeader.destBufferLengthUsed;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				SourceBuffer = null;
				DestBuffer = null;
				hSourceBuffer.Free();
				hDestBuffer.Free();
			}
			disposed = true;
		}

		~AcmStreamHeader()
		{
			Dispose(disposing: false);
		}
	}
	[Flags]
	internal enum AcmStreamHeaderStatusFlags
	{
		Done = 0x10000,
		Prepared = 0x20000,
		InQueue = 0x100000
	}
	[StructLayout(LayoutKind.Sequential, Size = 128)]
	internal class AcmStreamHeaderStruct
	{
		public int cbStruct;

		public AcmStreamHeaderStatusFlags fdwStatus;

		public IntPtr userData;

		public IntPtr sourceBufferPointer;

		public int sourceBufferLength;

		public int sourceBufferLengthUsed;

		public IntPtr sourceUserData;

		public IntPtr destBufferPointer;

		public int destBufferLength;

		public int destBufferLengthUsed;

		public IntPtr destUserData;
	}
	[Flags]
	internal enum AcmStreamOpenFlags
	{
		Query = 1,
		Async = 2,
		NonRealTime = 4,
		CallbackTypeMask = 0x70000,
		CallbackNull = 0,
		CallbackWindow = 0x10000,
		CallbackTask = 0x20000,
		CallbackFunction = 0x30000,
		CallbackThread = 0x20000,
		CallbackEvent = 0x50000
	}
	internal enum AcmStreamSizeFlags
	{
		Source,
		Destination
	}
	[StructLayout(LayoutKind.Sequential)]
	public class WaveFilter
	{
		public int StructureSize = Marshal.SizeOf(typeof(WaveFilter));

		public int FilterTag;

		public int Filter;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public int[] Reserved;
	}
}
namespace NAudio.Wave.Asio
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Asio64Bit
	{
		public uint hi;

		public uint lo;
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AsioCallbacks
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void AsioBufferSwitchCallBack(int doubleBufferIndex, bool directProcess);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void AsioSampleRateDidChangeCallBack(double sRate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int AsioAsioMessageCallBack(AsioMessageSelector selector, int value, IntPtr message, IntPtr opt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr AsioBufferSwitchTimeInfoCallBack(IntPtr asioTimeParam, int doubleBufferIndex, bool directProcess);

		public AsioBufferSwitchCallBack pbufferSwitch;

		public AsioSampleRateDidChangeCallBack psampleRateDidChange;

		public AsioAsioMessageCallBack pasioMessage;

		public AsioBufferSwitchTimeInfoCallBack pbufferSwitchTimeInfo;
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AsioChannelInfo
	{
		public int channel;

		public bool isInput;

		public bool isActive;

		public int channelGroup;

		[MarshalAs(UnmanagedType.U4)]
		public AsioSampleType type;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string name;
	}
	public class AsioDriver
	{
		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		private class AsioDriverVTable
		{
			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate int ASIOInit(IntPtr _pUnknown, IntPtr sysHandle);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate void ASIOgetDriverName(IntPtr _pUnknown, StringBuilder name);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate int ASIOgetDriverVersion(IntPtr _pUnknown);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate void ASIOgetErrorMessage(IntPtr _pUnknown, StringBuilder errorMessage);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOstart(IntPtr _pUnknown);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOstop(IntPtr _pUnknown);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetChannels(IntPtr _pUnknown, out int numInputChannels, out int numOutputChannels);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetLatencies(IntPtr _pUnknown, out int inputLatency, out int outputLatency);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetBufferSize(IntPtr _pUnknown, out int minSize, out int maxSize, out int preferredSize, out int granularity);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOcanSampleRate(IntPtr _pUnknown, double sampleRate);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetSampleRate(IntPtr _pUnknown, out double sampleRate);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOsetSampleRate(IntPtr _pUnknown, double sampleRate);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetClockSources(IntPtr _pUnknown, out long clocks, int numSources);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOsetClockSource(IntPtr _pUnknown, int reference);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetSamplePosition(IntPtr _pUnknown, out long samplePos, ref Asio64Bit timeStamp);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOgetChannelInfo(IntPtr _pUnknown, ref AsioChannelInfo info);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOcreateBuffers(IntPtr _pUnknown, IntPtr bufferInfos, int numChannels, int bufferSize, IntPtr callbacks);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOdisposeBuffers(IntPtr _pUnknown);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOcontrolPanel(IntPtr _pUnknown);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOfuture(IntPtr _pUnknown, int selector, IntPtr opt);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate AsioError ASIOoutputReady(IntPtr _pUnknown);

			public ASIOInit init;

			public ASIOgetDriverName getDriverName;

			public ASIOgetDriverVersion getDriverVersion;

			public ASIOgetErrorMessage getErrorMessage;

			public ASIOstart start;

			public ASIOstop stop;

			public ASIOgetChannels getChannels;

			public ASIOgetLatencies getLatencies;

			public ASIOgetBufferSize getBufferSize;

			public ASIOcanSampleRate canSampleRate;

			public ASIOgetSampleRate getSampleRate;

			public ASIOsetSampleRate setSampleRate;

			public ASIOgetClockSources getClockSources;

			public ASIOsetClockSource setClockSource;

			public ASIOgetSamplePosition getSamplePosition;

			public ASIOgetChannelInfo getChannelInfo;

			public ASIOcreateBuffers createBuffers;

			public ASIOdisposeBuffers disposeBuffers;

			public ASIOcontrolPanel controlPanel;

			public ASIOfuture future;

			public ASIOoutputReady outputReady;
		}

		private IntPtr pAsioComObject;

		private IntPtr pinnedcallbacks;

		private AsioDriverVTable asioDriverVTable;

		private AsioDriver()
		{
		}

		public static string[] GetAsioDriverNames()
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ASIO");
			string[] result = new string[0];
			if (registryKey != null)
			{
				result = registryKey.GetSubKeyNames();
				registryKey.Close();
			}
			return result;
		}

		public static AsioDriver GetAsioDriverByName(string name)
		{
			return GetAsioDriverByGuid(new Guid((Registry.LocalMachine.OpenSubKey("SOFTWARE\\ASIO\\" + name) ?? throw new ArgumentException("Driver Name " + name + " doesn't exist")).GetValue("CLSID").ToString()));
		}

		public static AsioDriver GetAsioDriverByGuid(Guid guid)
		{
			AsioDriver asioDriver = new AsioDriver();
			asioDriver.InitFromGuid(guid);
			return asioDriver;
		}

		public bool Init(IntPtr sysHandle)
		{
			return asioDriverVTable.init(pAsioComObject, sysHandle) == 1;
		}

		public string GetDriverName()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			asioDriverVTable.getDriverName(pAsioComObject, stringBuilder);
			return stringBuilder.ToString();
		}

		public int GetDriverVersion()
		{
			return asioDriverVTable.getDriverVersion(pAsioComObject);
		}

		public string GetErrorMessage()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			asioDriverVTable.getErrorMessage(pAsioComObject, stringBuilder);
			return stringBuilder.ToString();
		}

		public void Start()
		{
			HandleException(asioDriverVTable.start(pAsioComObject), "start");
		}

		public AsioError Stop()
		{
			return asioDriverVTable.stop(pAsioComObject);
		}

		public void GetChannels(out int numInputChannels, out int numOutputChannels)
		{
			HandleException(asioDriverVTable.getChannels(pAsioComObject, out numInputChannels, out numOutputChannels), "getChannels");
		}

		public AsioError GetLatencies(out int inputLatency, out int outputLatency)
		{
			return asioDriverVTable.getLatencies(pAsioComObject, out inputLatency, out outputLatency);
		}

		public void GetBufferSize(out int minSize, out int maxSize, out int preferredSize, out int granularity)
		{
			HandleException(asioDriverVTable.getBufferSize(pAsioComObject, out minSize, out maxSize, out preferredSize, out granularity), "getBufferSize");
		}

		public bool CanSampleRate(double sampleRate)
		{
			AsioError asioError = asioDriverVTable.canSampleRate(pAsioComObject, sampleRate);
			switch (asioError)
			{
			case AsioError.ASE_NoClock:
				return false;
			case AsioError.ASE_OK:
				return true;
			default:
				HandleException(asioError, "canSampleRate");
				return false;
			}
		}

		public double GetSampleRate()
		{
			HandleException(asioDriverVTable.getSampleRate(pAsioComObject, out var sampleRate), "getSampleRate");
			return sampleRate;
		}

		public void SetSampleRate(double sampleRate)
		{
			HandleException(asioDriverVTable.setSampleRate(pAsioComObject, sampleRate), "setSampleRate");
		}

		public void GetClockSources(out long clocks, int numSources)
		{
			HandleException(asioDriverVTable.getClockSources(pAsioComObject, out clocks, numSources), "getClockSources");
		}

		public void SetClockSource(int reference)
		{
			HandleException(asioDriverVTable.setClockSource(pAsioComObject, reference), "setClockSources");
		}

		public void GetSamplePosition(out long samplePos, ref Asio64Bit timeStamp)
		{
			HandleException(asioDriverVTable.getSamplePosition(pAsioComObject, out samplePos, ref timeStamp), "getSamplePosition");
		}

		public AsioChannelInfo GetChannelInfo(int channelNumber, bool trueForInputInfo)
		{
			AsioChannelInfo asioChannelInfo = default(AsioChannelInfo);
			asioChannelInfo.channel = channelNumber;
			asioChannelInfo.isInput = trueForInputInfo;
			AsioChannelInfo info = asioChannelInfo;
			HandleException(asioDriverVTable.getChannelInfo(pAsioComObject, ref info), "getChannelInfo");
			return info;
		}

		public void CreateBuffers(IntPtr bufferInfos, int numChannels, int bufferSize, ref AsioCallbacks callbacks)
		{
			pinnedcallbacks = Marshal.AllocHGlobal(Marshal.SizeOf(callbacks));
			Marshal.StructureToPtr(callbacks, pinnedcallbacks, fDeleteOld: false);
			HandleException(asioDriverVTable.createBuffers(pAsioComObject, bufferInfos, numChannels, bufferSize, pinnedcallbacks), "createBuffers");
		}

		public AsioError DisposeBuffers()
		{
			AsioError result = asioDriverVTable.disposeBuffers(pAsioComObject);
			Marshal.FreeHGlobal(pinnedcallbacks);
			return result;
		}

		public void ControlPanel()
		{
			HandleException(asioDriverVTable.controlPanel(pAsioComObject), "controlPanel");
		}

		public void Future(int selector, IntPtr opt)
		{
			HandleException(asioDriverVTable.future(pAsioComObject, selector, opt), "future");
		}

		public AsioError OutputReady()
		{
			return asioDriverVTable.outputReady(pAsioComObject);
		}

		public void ReleaseComAsioDriver()
		{
			Marshal.Release(pAsioComObject);
		}

		private void HandleException(AsioError error, string methodName)
		{
			if (error != 0 && error != AsioError.ASE_SUCCESS)
			{
				throw new AsioException("Error code [" + AsioException.getErrorName(error) + "] while calling ASIO method <" + methodName + ">, " + GetErrorMessage())
				{
					Error = error
				};
			}
		}

		private void InitFromGuid(Guid asioGuid)
		{
			int num = CoCreateInstance(ref asioGuid, IntPtr.Zero, 1u, ref asioGuid, out pAsioComObject);
			if (num != 0)
			{
				throw new COMException("Unable to instantiate ASIO. Check if STAThread is set", num);
			}
			IntPtr ptr = Marshal.ReadIntPtr(pAsioComObject);
			asioDriverVTable = new AsioDriverVTable();
			FieldInfo[] fields = typeof(AsioDriverVTable).GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				object delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(ptr, (i + 3) * IntPtr.Size), fieldInfo.FieldType);
				fieldInfo.SetValue(asioDriverVTable, delegateForFunctionPointer);
			}
		}

		[DllImport("ole32.Dll")]
		private static extern int CoCreateInstance(ref Guid clsid, IntPtr inner, uint context, ref Guid uuid, out IntPtr rReturnedComObject);
	}
	public class AsioDriverCapability
	{
		public string DriverName;

		public int NbInputChannels;

		public int NbOutputChannels;

		public int InputLatency;

		public int OutputLatency;

		public int BufferMinSize;

		public int BufferMaxSize;

		public int BufferPreferredSize;

		public int BufferGranularity;

		public double SampleRate;

		public AsioChannelInfo[] InputChannelInfos;

		public AsioChannelInfo[] OutputChannelInfos;
	}
	public delegate void AsioFillBufferCallback(IntPtr[] inputChannels, IntPtr[] outputChannels);
	public class AsioDriverExt
	{
		private readonly AsioDriver driver;

		private AsioCallbacks callbacks;

		private AsioDriverCapability capability;

		private AsioBufferInfo[] bufferInfos;

		private bool isOutputReadySupported;

		private IntPtr[] currentOutputBuffers;

		private IntPtr[] currentInputBuffers;

		private int numberOfOutputChannels;

		private int numberOfInputChannels;

		private AsioFillBufferCallback fillBufferCallback;

		private int bufferSize;

		private int outputChannelOffset;

		private int inputChannelOffset;

		public AsioDriver Driver => driver;

		public AsioFillBufferCallback FillBufferCallback
		{
			get
			{
				return fillBufferCallback;
			}
			set
			{
				fillBufferCallback = value;
			}
		}

		public AsioDriverCapability Capabilities => capability;

		public AsioDriverExt(AsioDriver driver)
		{
			this.driver = driver;
			if (!driver.Init(IntPtr.Zero))
			{
				throw new InvalidOperationException(driver.GetErrorMessage());
			}
			callbacks = default(AsioCallbacks);
			callbacks.pasioMessage = AsioMessageCallBack;
			callbacks.pbufferSwitch = BufferSwitchCallBack;
			callbacks.pbufferSwitchTimeInfo = BufferSwitchTimeInfoCallBack;
			callbacks.psampleRateDidChange = SampleRateDidChangeCallBack;
			BuildCapabilities();
		}

		public void SetChannelOffset(int outputChannelOffset, int inputChannelOffset)
		{
			if (outputChannelOffset + numberOfOutputChannels <= Capabilities.NbOutputChannels)
			{
				this.outputChannelOffset = outputChannelOffset;
				if (inputChannelOffset + numberOfInputChannels <= Capabilities.NbInputChannels)
				{
					this.inputChannelOffset = inputChannelOffset;
					return;
				}
				throw new ArgumentException("Invalid channel offset");
			}
			throw new ArgumentException("Invalid channel offset");
		}

		public void Start()
		{
			driver.Start();
		}

		public void Stop()
		{
			driver.Stop();
		}

		public void ShowControlPanel()
		{
			driver.ControlPanel();
		}

		public void ReleaseDriver()
		{
			try
			{
				driver.DisposeBuffers();
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine(ex.ToString());
			}
			driver.ReleaseComAsioDriver();
		}

		public bool IsSampleRateSupported(double sampleRate)
		{
			return driver.CanSampleRate(sampleRate);
		}

		public void SetSampleRate(double sampleRate)
		{
			driver.SetSampleRate(sampleRate);
			BuildCapabilities();
		}

		public unsafe int CreateBuffers(int numberOfOutputChannels, int numberOfInputChannels, bool useMaxBufferSize)
		{
			if (numberOfOutputChannels < 0 || numberOfOutputChannels > capability.NbOutputChannels)
			{
				throw new ArgumentException($"Invalid number of channels {numberOfOutputChannels}, must be in the range [0,{capability.NbOutputChannels}]");
			}
			if (numberOfInputChannels < 0 || numberOfInputChannels > capability.NbInputChannels)
			{
				throw new ArgumentException("numberOfInputChannels", $"Invalid number of input channels {numberOfInputChannels}, must be in the range [0,{capability.NbInputChannels}]");
			}
			this.numberOfOutputChannels = numberOfOutputChannels;
			this.numberOfInputChannels = numberOfInputChannels;
			int num = capability.NbInputChannels + capability.NbOutputChannels;
			bufferInfos = new AsioBufferInfo[num];
			currentOutputBuffers = new IntPtr[numberOfOutputChannels];
			currentInputBuffers = new IntPtr[numberOfInputChannels];
			int num2 = 0;
			int num3 = 0;
			while (num3 < capability.NbInputChannels)
			{
				bufferInfos[num2].isInput = true;
				bufferInfos[num2].channelNum = num3;
				bufferInfos[num2].pBuffer0 = IntPtr.Zero;
				bufferInfos[num2].pBuffer1 = IntPtr.Zero;
				num3++;
				num2++;
			}
			int num4 = 0;
			while (num4 < capability.NbOutputChannels)
			{
				bufferInfos[num2].isInput = false;
				bufferInfos[num2].channelNum = num4;
				bufferInfos[num2].pBuffer0 = IntPtr.Zero;
				bufferInfos[num2].pBuffer1 = IntPtr.Zero;
				num4++;
				num2++;
			}
			if (useMaxBufferSize)
			{
				bufferSize = capability.BufferMaxSize;
			}
			else
			{
				bufferSize = capability.BufferPreferredSize;
			}
			fixed (AsioBufferInfo* value = &bufferInfos[0])
			{
				IntPtr intPtr = new IntPtr(value);
				driver.CreateBuffers(intPtr, num, bufferSize, ref callbacks);
			}
			isOutputReadySupported = driver.OutputReady() == AsioError.ASE_OK;
			return bufferSize;
		}

		private void BuildCapabilities()
		{
			capability = new AsioDriverCapability();
			capability.DriverName = driver.GetDriverName();
			driver.GetChannels(out capability.NbInputChannels, out capability.NbOutputChannels);
			capability.InputChannelInfos = new AsioChannelInfo[capability.NbInputChannels];
			capability.OutputChannelInfos = new AsioChannelInfo[capability.NbOutputChannels];
			for (int i = 0; i < capability.NbInputChannels; i++)
			{
				capability.InputChannelInfos[i] = driver.GetChannelInfo(i, trueForInputInfo: true);
			}
			for (int j = 0; j < capability.NbOutputChannels; j++)
			{
				capability.OutputChannelInfos[j] = driver.GetChannelInfo(j, trueForInputInfo: false);
			}
			capability.SampleRate = driver.GetSampleRate();
			AsioError latencies = driver.GetLatencies(out capability.InputLatency, out capability.OutputLatency);
			if (latencies != 0 && latencies != AsioError.ASE_NotPresent)
			{
				throw new AsioException("ASIOgetLatencies")
				{
					Error = latencies
				};
			}
			driver.GetBufferSize(out capability.BufferMinSize, out capability.BufferMaxSize, out capability.BufferPreferredSize, out capability.BufferGranularity);
		}

		private void BufferSwitchCallBack(int doubleBufferIndex, bool directProcess)
		{
			for (int i = 0; i < numberOfInputChannels; i++)
			{
				currentInputBuffers[i] = bufferInfos[i + inputChannelOffset].Buffer(doubleBufferIndex);
			}
			for (int j = 0; j < numberOfOutputChannels; j++)
			{
				currentOutputBuffers[j] = bufferInfos[j + outputChannelOffset + capability.NbInputChannels].Buffer(doubleBufferIndex);
			}
			fillBufferCallback?.Invoke(currentInputBuffers, currentOutputBuffers);
			if (isOutputReadySupported)
			{
				driver.OutputReady();
			}
		}

		private void SampleRateDidChangeCallBack(double sRate)
		{
			capability.SampleRate = sRate;
		}

		private int AsioMessageCallBack(AsioMessageSelector selector, int value, IntPtr message, IntPtr opt)
		{
			switch (selector)
			{
			case AsioMessageSelector.kAsioSelectorSupported:
				switch ((AsioMessageSelector)Enum.ToObject(typeof(AsioMessageSelector), value))
				{
				case AsioMessageSelector.kAsioEngineVersion:
					return 1;
				case AsioMessageSelector.kAsioResetRequest:
					return 0;
				case AsioMessageSelector.kAsioBufferSizeChange:
					return 0;
				case AsioMessageSelector.kAsioResyncRequest:
					return 0;
				case AsioMessageSelector.kAsioLatenciesChanged:
					return 0;
				case AsioMessageSelector.kAsioSupportsTimeInfo:
					return 0;
				case AsioMessageSelector.kAsioSupportsTimeCode:
					return 0;
				}
				break;
			case AsioMessageSelector.kAsioEngineVersion:
				return 2;
			case AsioMessageSelector.kAsioResetRequest:
				return 1;
			case AsioMessageSelector.kAsioBufferSizeChange:
				return 0;
			case AsioMessageSelector.kAsioResyncRequest:
				return 0;
			case AsioMessageSelector.kAsioLatenciesChanged:
				return 0;
			case AsioMessageSelector.kAsioSupportsTimeInfo:
				return 0;
			case AsioMessageSelector.kAsioSupportsTimeCode:
				return 0;
			}
			return 0;
		}

		private IntPtr BufferSwitchTimeInfoCallBack(IntPtr asioTimeParam, int doubleBufferIndex, bool directProcess)
		{
			return IntPtr.Zero;
		}
	}
	public enum AsioError
	{
		ASE_OK = 0,
		ASE_SUCCESS = 1061701536,
		ASE_NotPresent = -1000,
		ASE_HWMalfunction = -999,
		ASE_InvalidParameter = -998,
		ASE_InvalidMode = -997,
		ASE_SPNotAdvancing = -996,
		ASE_NoClock = -995,
		ASE_NoMemory = -994
	}
	public enum AsioMessageSelector
	{
		kAsioSelectorSupported = 1,
		kAsioEngineVersion,
		kAsioResetRequest,
		kAsioBufferSizeChange,
		kAsioResyncRequest,
		kAsioLatenciesChanged,
		kAsioSupportsTimeInfo,
		kAsioSupportsTimeCode,
		kAsioMMCCommand,
		kAsioSupportsInputMonitor,
		kAsioSupportsInputGain,
		kAsioSupportsInputMeter,
		kAsioSupportsOutputGain,
		kAsioSupportsOutputMeter,
		kAsioOverload
	}
	internal class AsioSampleConvertor
	{
		public delegate void SampleConvertor(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples);

		public static SampleConvertor SelectSampleConvertor(WaveFormat waveFormat, AsioSampleType asioType)
		{
			SampleConvertor result = null;
			bool flag = waveFormat.Channels == 2;
			switch (asioType)
			{
			case AsioSampleType.Int32LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					result = (flag ? new SampleConvertor(ConvertorShortToInt2Channels) : new SampleConvertor(ConvertorShortToIntGeneric));
					break;
				case 32:
					result = ((waveFormat.Encoding != WaveFormatEncoding.IeeeFloat) ? (flag ? new SampleConvertor(ConvertorIntToInt2Channels) : new SampleConvertor(ConvertorIntToIntGeneric)) : (flag ? new SampleConvertor(ConvertorFloatToInt2Channels) : new SampleConvertor(ConvertorFloatToIntGeneric)));
					break;
				}
				break;
			case AsioSampleType.Int16LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					result = (flag ? new SampleConvertor(ConvertorShortToShort2Channels) : new SampleConvertor(ConvertorShortToShortGeneric));
					break;
				case 32:
					result = ((waveFormat.Encoding != WaveFormatEncoding.IeeeFloat) ? (flag ? new SampleConvertor(ConvertorIntToShort2Channels) : new SampleConvertor(ConvertorIntToShortGeneric)) : (flag ? new SampleConvertor(ConvertorFloatToShort2Channels) : new SampleConvertor(ConvertorFloatToShortGeneric)));
					break;
				}
				break;
			case AsioSampleType.Int24LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					throw new ArgumentException("Not a supported conversion");
				case 32:
					if (waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
					{
						result = ConverterFloatTo24LSBGeneric;
						break;
					}
					throw new ArgumentException("Not a supported conversion");
				}
				break;
			case AsioSampleType.Float32LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					throw new ArgumentException("Not a supported conversion");
				case 32:
					result = ((waveFormat.Encoding != WaveFormatEncoding.IeeeFloat) ? new SampleConvertor(ConvertorIntToFloatGeneric) : new SampleConvertor(ConverterFloatToFloatGeneric));
					break;
				}
				break;
			default:
				throw new ArgumentException($"ASIO Buffer Type {Enum.GetName(typeof(AsioSampleType), asioType)} is not yet supported.");
			}
			return result;
		}

		public unsafe static void ConvertorShortToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			ptr2++;
			ptr3++;
			for (int i = 0; i < nbSamples; i++)
			{
				*ptr2 = *ptr;
				*ptr3 = ptr[1];
				ptr += 2;
				ptr2 += 2;
				ptr3 += 2;
			}
		}

		public unsafe static void ConvertorShortToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)(void*)asioOutputBuffers[i];
				int num = i;
				short* ptr2 = array[num];
				array[num] = ptr2 + 1;
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					short* intPtr = array[k];
					short* num2 = ptr;
					ptr = num2 + 1;
					*intPtr = *num2;
					short*[] array2 = array;
					int num = k;
					array2[num] += 2;
				}
			}
		}

		public unsafe static void ConvertorFloatToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			int* ptr2 = (int*)(void*)asioOutputBuffers[0];
			int* ptr3 = (int*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				int* num = ptr2;
				ptr2 = num + 1;
				*num = clampToInt(*ptr);
				int* num2 = ptr3;
				ptr3 = num2 + 1;
				*num2 = clampToInt(ptr[1]);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorFloatToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			int*[] array = new int*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (int*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					int* ptr2 = array[num];
					array[num] = ptr2 + 1;
					float* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = clampToInt(*num2);
				}
			}
		}

		public unsafe static void ConvertorIntToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			int* ptr = (int*)(void*)inputInterleavedBuffer;
			int* ptr2 = (int*)(void*)asioOutputBuffers[0];
			int* ptr3 = (int*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				int* num = ptr2;
				ptr2 = num + 1;
				*num = *ptr;
				int* num2 = ptr3;
				ptr3 = num2 + 1;
				*num2 = ptr[1];
				ptr += 2;
			}
		}

		public unsafe static void ConvertorIntToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			int* ptr = (int*)(void*)inputInterleavedBuffer;
			int*[] array = new int*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (int*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					int* ptr2 = array[num];
					array[num] = ptr2 + 1;
					int* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = *num2;
				}
			}
		}

		public unsafe static void ConvertorIntToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			int* ptr = (int*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				short* num = ptr2;
				ptr2 = num + 1;
				*num = (short)(*ptr / 65536);
				short* num2 = ptr3;
				ptr3 = num2 + 1;
				*num2 = (short)(ptr[1] / 65536);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorIntToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			int* ptr = (int*)(void*)inputInterleavedBuffer;
			int*[] array = new int*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (int*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					int* ptr2 = array[num];
					array[num] = ptr2 + 1;
					int* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = (short)(*num2 / 65536);
				}
			}
		}

		public unsafe static void ConvertorIntToFloatGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			int* ptr = (int*)(void*)inputInterleavedBuffer;
			float*[] array = new float*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (float*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					float* ptr2 = array[num];
					array[num] = ptr2 + 1;
					int* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = *num2 / int.MinValue;
				}
			}
		}

		public unsafe static void ConvertorShortToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				short* num = ptr2;
				ptr2 = num + 1;
				*num = *ptr;
				short* num2 = ptr3;
				ptr3 = num2 + 1;
				*num2 = ptr[1];
				ptr += 2;
			}
		}

		public unsafe static void ConvertorShortToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					short* ptr2 = array[num];
					array[num] = ptr2 + 1;
					short* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = *num2;
				}
			}
		}

		public unsafe static void ConvertorFloatToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				short* num = ptr2;
				ptr2 = num + 1;
				*num = clampToShort(*ptr);
				short* num2 = ptr3;
				ptr3 = num2 + 1;
				*num2 = clampToShort(ptr[1]);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorFloatToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					short* ptr2 = array[num];
					array[num] = ptr2 + 1;
					float* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = clampToShort(*num2);
				}
			}
		}

		public unsafe static void ConverterFloatTo24LSBGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			byte*[] array = new byte*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (byte*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					float* num = ptr;
					ptr = num + 1;
					int num2 = clampTo24Bit(*num);
					int num3 = k;
					*(array[num3]++) = (byte)num2;
					num3 = k;
					*(array[num3]++) = (byte)(num2 >> 8);
					num3 = k;
					*(array[num3]++) = (byte)(num2 >> 16);
				}
			}
		}

		public unsafe static void ConverterFloatToFloatGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			float*[] array = new float*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (float*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = k;
					float* ptr2 = array[num];
					array[num] = ptr2 + 1;
					float* num2 = ptr;
					ptr = num2 + 1;
					*ptr2 = *num2;
				}
			}
		}

		private static int clampTo24Bit(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (int)(sampleValue * 8388607.0);
		}

		private static int clampToInt(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (int)(sampleValue * 2147483647.0);
		}

		private static short clampToShort(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (short)(sampleValue * 32767.0);
		}
	}
	public enum AsioSampleType
	{
		Int16MSB = 0,
		Int24MSB = 1,
		Int32MSB = 2,
		Float32MSB = 3,
		Float64MSB = 4,
		Int32MSB16 = 8,
		Int32MSB18 = 9,
		Int32MSB20 = 10,
		Int32MSB24 = 11,
		Int16LSB = 16,
		Int24LSB = 17,
		Int32LSB = 18,
		Float32LSB = 19,
		Float64LSB = 20,
		Int32LSB16 = 24,
		Int32LSB18 = 25,
		Int32LSB20 = 26,
		Int32LSB24 = 27,
		DSDInt8LSB1 = 32,
		DSDInt8MSB1 = 33,
		DSDInt8NER8 = 40
	}
	internal class AsioException : Exception
	{
		private AsioError error;

		public AsioError Error
		{
			get
			{
				return error;
			}
			set
			{
				error = value;
				Data["ASIOError"] = error;
			}
		}

		public AsioException()
		{
		}

		public AsioException(string message)
			: base(message)
		{
		}

		public AsioException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public static string getErrorName(AsioError error)
		{
			return Enum.GetName(typeof(AsioError), error);
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct AsioBufferInfo
	{
		public bool isInput;

		public int channelNum;

		public IntPtr pBuffer0;

		public IntPtr pBuffer1;

		public IntPtr Buffer(int bufferIndex)
		{
			if (bufferIndex != 0)
			{
				return pBuffer1;
			}
			return pBuffer0;
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct AsioTimeCode
	{
		public double speed;

		public Asio64Bit timeCodeSamples;

		public AsioTimeCodeFlags flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string future;
	}
	[Flags]
	internal enum AsioTimeCodeFlags
	{
		kTcValid = 1,
		kTcRunning = 2,
		kTcReverse = 4,
		kTcOnspeed = 8,
		kTcStill = 0x10,
		kTcSpeedValid = 0x100
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct AsioTimeInfo
	{
		public double speed;

		public Asio64Bit systemTime;

		public Asio64Bit samplePosition;

		public double sampleRate;

		public AsioTimeInfoFlags flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
		public string reserved;
	}
	[Flags]
	internal enum AsioTimeInfoFlags
	{
		kSystemTimeValid = 1,
		kSamplePositionValid = 2,
		kSampleRateValid = 4,
		kSpeedValid = 8,
		kSampleRateChanged = 0x10,
		kClockSourceChanged = 0x20
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct AsioTime
	{
		public int reserved1;

		public int reserved2;

		public int reserved3;

		public int reserved4;

		public AsioTimeInfo timeInfo;

		public AsioTimeCode timeCode;
	}
}
namespace NAudio.Dsp
{
	public class BiQuadFilter
	{
		private double a0;

		private double a1;

		private double a2;

		private double a3;

		private double a4;

		private float x1;

		private float x2;

		private float y1;

		private float y2;

		public float Transform(float inSample)
		{
			double num = a0 * (double)inSample + a1 * (double)x1 + a2 * (double)x2 - a3 * (double)y1 - a4 * (double)y2;
			x2 = x1;
			x1 = inSample;
			y2 = y1;
			y1 = (float)num;
			return y1;
		}

		private void SetCoefficients(double aa0, double aa1, double aa2, double b0, double b1, double b2)
		{
			a0 = b0 / aa0;
			a1 = b1 / aa0;
			a2 = b2 / aa0;
			a3 = aa1 / aa0;
			a4 = aa2 / aa0;
		}

		public void SetLowPassFilter(float sampleRate, float cutoffFrequency, float q)
		{
			double num = Math.PI * 2.0 * (double)cutoffFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num) / (double)(2f * q);
			double b = (1.0 - num2) / 2.0;
			double b2 = 1.0 - num2;
			double b3 = (1.0 - num2) / 2.0;
			double aa = 1.0 + num3;
			double aa2 = -2.0 * num2;
			double aa3 = 1.0 - num3;
			SetCoefficients(aa, aa2, aa3, b, b2, b3);
		}

		public void SetPeakingEq(float sampleRate, float centreFrequency, float q, float dbGain)
		{
			double num = Math.PI * 2.0 * (double)centreFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num) / (double)(2f * q);
			double num4 = Math.Pow(10.0, dbGain / 40f);
			double b = 1.0 + num3 * num4;
			double b2 = -2.0 * num2;
			double b3 = 1.0 - num3 * num4;
			double aa = 1.0 + num3 / num4;
			double aa2 = -2.0 * num2;
			double aa3 = 1.0 - num3 / num4;
			SetCoefficients(aa, aa2, aa3, b, b2, b3);
		}

		public void SetHighPassFilter(float sampleRate, float cutoffFrequency, float q)
		{
			double num = Math.PI * 2.0 * (double)cutoffFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num) / (double)(2f * q);
			double b = (1.0 + num2) / 2.0;
			double b2 = 0.0 - (1.0 + num2);
			double b3 = (1.0 + num2) / 2.0;
			double aa = 1.0 + num3;
			double aa2 = -2.0 * num2;
			double aa3 = 1.0 - num3;
			SetCoefficients(aa, aa2, aa3, b, b2, b3);
		}

		public static BiQuadFilter LowPassFilter(float sampleRate, float cutoffFrequency, float q)
		{
			BiQuadFilter biQuadFilter = new BiQuadFilter();
			biQuadFilter.SetLowPassFilter(sampleRate, cutoffFrequency, q);
			return biQuadFilter;
		}

		public static BiQuadFilter HighPassFilter(float sampleRate, float cutoffFrequency, float q)
		{
			BiQuadFilter biQuadFilter = new BiQuadFilter();
			biQuadFilter.SetHighPassFilter(sampleRate, cutoffFrequency, q);
			return biQuadFilter;
		}

		public static BiQuadFilter BandPassFilterConstantSkirtGain(float sampleRate, float centreFrequency, float q)
		{
			double num = Math.PI * 2.0 * (double)centreFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			double num4 = num3 / (double)(2f * q);
			double b = num3 / 2.0;
			int num5 = 0;
			double b2 = (0.0 - num3) / 2.0;
			double num6 = 1.0 + num4;
			double num7 = -2.0 * num2;
			double num8 = 1.0 - num4;
			return new BiQuadFilter(num6, num7, num8, b, num5, b2);
		}

		public static BiQuadFilter BandPassFilterConstantPeakGain(float sampleRate, float centreFrequency, float q)
		{
			double num = Math.PI * 2.0 * (double)centreFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num) / (double)(2f * q);
			double b = num3;
			int num4 = 0;
			double b2 = 0.0 - num3;
			double num5 = 1.0 + num3;
			double num6 = -2.0 * num2;
			double num7 = 1.0 - num3;
			return new BiQuadFilter(num5, num6, num7, b, num4, b2);
		}

		public static BiQuadFilter NotchFilter(float sampleRate, float centreFrequency, float q)
		{
			double num = Math.PI * 2.0 * (double)centreFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num) / (double)(2f * q);
			int num4 = 1;
			double b = -2.0 * num2;
			int num5 = 1;
			double num6 = 1.0 + num3;
			double num7 = -2.0 * num2;
			double num8 = 1.0 - num3;
			return new BiQuadFilter(num6, num7, num8, num4, b, num5);
		}

		public static BiQuadFilter AllPassFilter(float sampleRate, float centreFrequency, float q)
		{
			double num = Math.PI * 2.0 * (double)centreFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num) / (double)(2f * q);
			double b = 1.0 - num3;
			double b2 = -2.0 * num2;
			double b3 = 1.0 + num3;
			double num4 = 1.0 + num3;
			double num5 = -2.0 * num2;
			double num6 = 1.0 - num3;
			return new BiQuadFilter(num4, num5, num6, b, b2, b3);
		}

		public static BiQuadFilter PeakingEQ(float sampleRate, float centreFrequency, float q, float dbGain)
		{
			BiQuadFilter biQuadFilter = new BiQuadFilter();
			biQuadFilter.SetPeakingEq(sampleRate, centreFrequency, q, dbGain);
			return biQuadFilter;
		}

		public static BiQuadFilter LowShelf(float sampleRate, float cutoffFrequency, float shelfSlope, float dbGain)
		{
			double num = Math.PI * 2.0 * (double)cutoffFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			double num4 = Math.Pow(10.0, dbGain / 40f);
			double num5 = num3 / 2.0 * Math.Sqrt((num4 + 1.0 / num4) * (double)(1f / shelfSlope - 1f) + 2.0);
			double num6 = 2.0 * Math.Sqrt(num4) * num5;
			double b = num4 * (num4 + 1.0 - (num4 - 1.0) * num2 + num6);
			double b2 = 2.0 * num4 * (num4 - 1.0 - (num4 + 1.0) * num2);
			double b3 = num4 * (num4 + 1.0 - (num4 - 1.0) * num2 - num6);
			double num7 = num4 + 1.0 + (num4 - 1.0) * num2 + num6;
			double num8 = -2.0 * (num4 - 1.0 + (num4 + 1.0) * num2);
			double num9 = num4 + 1.0 + (num4 - 1.0) * num2 - num6;
			return new BiQuadFilter(num7, num8, num9, b, b2, b3);
		}

		public static BiQuadFilter HighShelf(float sampleRate, float cutoffFrequency, float shelfSlope, float dbGain)
		{
			double num = Math.PI * 2.0 * (double)cutoffFrequency / (double)sampleRate;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			double num4 = Math.Pow(10.0, dbGain / 40f);
			double num5 = num3 / 2.0 * Math.Sqrt((num4 + 1.0 / num4) * (double)(1f / shelfSlope - 1f) + 2.0);
			double num6 = 2.0 * Math.Sqrt(num4) * num5;
			double b = num4 * (num4 + 1.0 + (num4 - 1.0) * num2 + num6);
			double b2 = -2.0 * num4 * (num4 - 1.0 + (num4 + 1.0) * num2);
			double b3 = num4 * (num4 + 1.0 + (num4 - 1.0) * num2 - num6);
			double num7 = num4 + 1.0 - (num4 - 1.0) * num2 + num6;
			double num8 = 2.0 * (num4 - 1.0 - (num4 + 1.0) * num2);
			double num9 = num4 + 1.0 - (num4 - 1.0) * num2 - num6;
			return new BiQuadFilter(num7, num8, num9, b, b2, b3);
		}

		private BiQuadFilter()
		{
			x1 = (x2 = 0f);
			y1 = (y2 = 0f);
		}

		private BiQuadFilter(double a0, double a1, double a2, double b0, double b1, double b2)
		{
			SetCoefficients(a0, a1, a2, b0, b1, b2);
			x1 = (x2 = 0f);
			y1 = (y2 = 0f);
		}
	}
	public struct Complex
	{
		public float X;

		public float Y;
	}
	internal class EnvelopeDetector
	{
		private double sampleRate;

		private double ms;

		private double coeff;

		public double TimeConstant
		{
			get
			{
				return ms;
			}
			set
			{
				ms = value;
				SetCoef();
			}
		}

		public double SampleRate
		{
			get
			{
				return sampleRate;
			}
			set
			{
				sampleRate = value;
				SetCoef();
			}
		}

		public EnvelopeDetector()
			: this(1.0, 44100.0)
		{
		}

		public EnvelopeDetector(double ms, double sampleRate)
		{
			this.sampleRate = sampleRate;
			this.ms = ms;
			SetCoef();
		}

		public double Run(double inValue, double state)
		{
			return inValue + coeff * (state - inValue);
		}

		private void SetCoef()
		{
			coeff = Math.Exp(-1.0 / (0.001 * ms * sampleRate));
		}
	}
	internal class AttRelEnvelope
	{
		protected const double DC_OFFSET = 1E-25;

		private readonly EnvelopeDetector attack;

		private readonly EnvelopeDetector release;

		public double Attack
		{
			get
			{
				return attack.TimeConstant;
			}
			set
			{
				attack.TimeConstant = value;
			}
		}

		public double Release
		{
			get
			{
				return release.TimeConstant;
			}
			set
			{
				release.TimeConstant = value;
			}
		}

		public double SampleRate
		{
			get
			{
				return attack.SampleRate;
			}
			set
			{
				double num3 = (attack.SampleRate = (release.SampleRate = value));
			}
		}

		public AttRelEnvelope(double attackMilliseconds, double releaseMilliseconds, double sampleRate)
		{
			attack = new EnvelopeDetector(attackMilliseconds, sampleRate);
			release = new EnvelopeDetector(releaseMilliseconds, sampleRate);
		}

		public double Run(double inValue, double state)
		{
			if (!(inValue > state))
			{
				return release.Run(inValue, state);
			}
			return attack.Run(inValue, state);
		}
	}
	public class EnvelopeGenerator
	{
		public enum EnvelopeState
		{
			Idle,
			Attack,
			Decay,
			Sustain,
			Release
		}

		private EnvelopeState state;

		private float output;

		private float attackRate;

		private float decayRate;

		private float releaseRate;

		private float attackCoef;

		private float decayCoef;

		private float releaseCoef;

		private float sustainLevel;

		private float targetRatioAttack;

		private float targetRatioDecayRelease;

		private float attackBase;

		private float decayBase;

		private float releaseBase;

		public float AttackRate
		{
			get
			{
				return attackRate;
			}
			set
			{
				attackRate = value;
				attackCoef = CalcCoef(value, targetRatioAttack);
				attackBase = (1f + targetRatioAttack) * (1f - attackCoef);
			}
		}

		public float DecayRate
		{
			get
			{
				return decayRate;
			}
			set
			{
				decayRate = value;
				decayCoef = CalcCoef(value, targetRatioDecayRelease);
				decayBase = (sustainLevel - targetRatioDecayRelease) * (1f - decayCoef);
			}
		}

		public float ReleaseRate
		{
			get
			{
				return releaseRate;
			}
			set
			{
				releaseRate = value;
				releaseCoef = CalcCoef(value, targetRatioDecayRelease);
				releaseBase = (0f - targetRatioDecayRelease) * (1f - releaseCoef);
			}
		}

		public float SustainLevel
		{
			get
			{
				return sustainLevel;
			}
			set
			{
				sustainLevel = value;
				decayBase = (sustainLevel - targetRatioDecayRelease) * (1f - decayCoef);
			}
		}

		public EnvelopeState State => state;

		public EnvelopeGenerator()
		{
			Reset();
			AttackRate = 0f;
			DecayRate = 0f;
			ReleaseRate = 0f;
			SustainLevel = 1f;
			SetTargetRatioAttack(0.3f);
			SetTargetRatioDecayRelease(0.0001f);
		}

		private static float CalcCoef(float rate, float targetRatio)
		{
			return (float)Math.Exp((0.0 - Math.Log((1f + targetRatio) / targetRatio)) / (double)rate);
		}

		private void SetTargetRatioAttack(float targetRatio)
		{
			if (targetRatio < 1E-09f)
			{
				targetRatio = 1E-09f;
			}
			targetRatioAttack = targetRatio;
			attackBase = (1f + targetRatioAttack) * (1f - attackCoef);
		}

		private void SetTargetRatioDecayRelease(float targetRatio)
		{
			if (targetRatio < 1E-09f)
			{
				targetRatio = 1E-09f;
			}
			targetRatioDecayRelease = targetRatio;
			decayBase = (sustainLevel - targetRatioDecayRelease) * (1f - decayCoef);
			releaseBase = (0f - targetRatioDecayRelease) * (1f - releaseCoef);
		}

		public float Process()
		{
			switch (state)
			{
			case EnvelopeState.Attack:
				output = attackBase + output * attackCoef;
				if (output >= 1f)
				{
					output = 1f;
					state = EnvelopeState.Decay;
				}
				break;
			case EnvelopeState.Decay:
				output = decayBase + output * decayCoef;
				if (output <= sustainLevel)
				{
					output = sustainLevel;
					state = EnvelopeState.Sustain;
				}
				break;
			case EnvelopeState.Release:
				output = releaseBase + output * releaseCoef;
				if ((double)output <= 0.0)
				{
					output = 0f;
					state = EnvelopeState.Idle;
				}
				break;
			}
			return output;
		}

		public void Gate(bool gate)
		{
			if (gate)
			{
				state = EnvelopeState.Attack;
			}
			else if (state != 0)
			{
				state = EnvelopeState.Release;
			}
		}

		public void Reset()
		{
			state = EnvelopeState.Idle;
			output = 0f;
		}

		public float GetOutput()
		{
			return output;
		}
	}
	public static class FastFourierTransform
	{
		public static void FFT(bool forward, int m, Complex[] data)
		{
			int num = 1;
			for (int i = 0; i < m; i++)
			{
				num *= 2;
			}
			int num2 = num >> 1;
			int num3 = 0;
			for (int i = 0; i < num - 1; i++)
			{
				if (i < num3)
				{
					float x = data[i].X;
					float y = data[i].Y;
					data[i].X = data[num3].X;
					data[i].Y = data[num3].Y;
					data[num3].X = x;
					data[num3].Y = y;
				}
				int num4;
				for (num4 = num2; num4 <= num3; num4 >>= 1)
				{
					num3 -= num4;
				}
				num3 += num4;
			}
			float num5 = -1f;
			float num6 = 0f;
			int num7 = 1;
			for (int j = 0; j < m; j++)
			{
				int num8 = num7;
				num7 <<= 1;
				float num9 = 1f;
				float num10 = 0f;
				for (num3 = 0; num3 < num8; num3++)
				{
					for (int i = num3; i < num; i += num7)
					{
						int num11 = i + num8;
						float num12 = num9 * data[num11].X - num10 * data[num11].Y;
						float num13 = num9 * data[num11].Y + num10 * data[num11].X;
						data[num11].X = data[i].X - num12;
						data[num11].Y = data[i].Y - num13;
						data[i].X += num12;
						data[i].Y += num13;
					}
					float num14 = num9 * num5 - num10 * num6;
					num10 = num9 * num6 + num10 * num5;
					num9 = num14;
				}
				num6 = (float)Math.Sqrt((1f - num5) / 2f);
				if (forward)
				{
					num6 = 0f - num6;
				}
				num5 = (float)Math.Sqrt((1f + num5) / 2f);
			}
			if (forward)
			{
				for (int i = 0; i < num; i++)
				{
					data[i].X /= num;
					data[i].Y /= num;
				}
			}
		}

		public static double HammingWindow(int n, int frameSize)
		{
			return 0.54 - 0.46 * Math.Cos(Math.PI * 2.0 * (double)n / (double)(frameSize - 1));
		}

		public static double HannWindow(int n, int frameSize)
		{
			return 0.5 * (1.0 - Math.Cos(Math.PI * 2.0 * (double)n / (double)(frameSize - 1)));
		}

		public static double BlackmannHarrisWindow(int n, int frameSize)
		{
			return 287.0 / 800.0 - 0.48829 * Math.Cos(Math.PI * 2.0 * (double)n / (double)(frameSize - 1)) + 0.14128 * Math.Cos(Math.PI * 4.0 * (double)n / (double)(frameSize - 1)) - 0.01168 * Math.Cos(Math.PI * 6.0 * (double)n / (double)(frameSize - 1));
		}
	}
	public class ImpulseResponseConvolution
	{
		public float[] Convolve(float[] input, float[] impulseResponse)
		{
			float[] array = new float[input.Length + impulseResponse.Length];
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < impulseResponse.Length; j++)
				{
					if (i >= j && i - j < input.Length)
					{
						array[i] += impulseResponse[j] * input[i - j];
					}
				}
			}
			Normalize(array);
			return array;
		}

		public void Normalize(float[] data)
		{
			float num = 0f;
			for (int i = 0; i < data.Length; i++)
			{
				num = Math.Max(num, Math.Abs(data[i]));
			}
			if ((double)num > 1.0)
			{
				for (int j = 0; j < data.Length; j++)
				{
					data[j] /= num;
				}
			}
		}
	}
	internal class SimpleCompressor : AttRelEnvelope
	{
		private double envdB;

		public double MakeUpGain { get; set; }

		public double Threshold { get; set; }

		public double Ratio { get; set; }

		public SimpleCompressor(double attackTime, double releaseTime, double sampleRate)
			: base(attackTime, releaseTime, sampleRate)
		{
			Threshold = 0.0;
			Ratio = 1.0;
			MakeUpGain = 0.0;
			envdB = 1E-25;
		}

		public SimpleCompressor()
			: this(10.0, 10.0, 44100.0)
		{
		}

		public void InitRuntime()
		{
			envdB = 1E-25;
		}

		public void Process(ref double in1, ref double in2)
		{
			double val = Math.Abs(in1);
			double val2 = Math.Abs(in2);
			double num = Decibels.LinearToDecibels(Math.Max(val, val2) + 1E-25) - Threshold;
			if (num < 0.0)
			{
				num = 0.0;
			}
			num += 1E-25;
			envdB = Run(num, envdB);
			num = envdB - 1E-25;
			double dB = num * (Ratio - 1.0);
			dB = Decibels.DecibelsToLinear(dB) * Decibels.DecibelsToLinear(MakeUpGain);
			in1 *= dB;
			in2 *= dB;
		}
	}
	internal class SimpleGate : AttRelEnvelope
	{
		private double threshdB;

		private double thresh;

		private double env;

		public double Threshold
		{
			get
			{
				return threshdB;
			}
			set
			{
				threshdB = value;
				thresh = Decibels.DecibelsToLinear(value);
			}
		}

		public SimpleGate()
			: base(10.0, 10.0, 44100.0)
		{
			threshdB = 0.0;
			thresh = 1.0;
			env = 1E-25;
		}

		public void Process(ref double in1, ref double in2)
		{
			double val = Math.Abs(in1);
			double val2 = Math.Abs(in2);
			double num = ((Math.Max(val, val2) > thresh) ? 1.0 : 0.0);
			num += 1E-25;
			env = Run(num, env);
			num = env - 1E-25;
			in1 *= num;
			in2 *= num;
		}
	}
	public class SmbPitchShifter
	{
		private static int MAX_FRAME_LENGTH = 16000;

		private float[] gInFIFO = new float[MAX_FRAME_LENGTH];

		private float[] gOutFIFO = new float[MAX_FRAME_LENGTH];

		private float[] gFFTworksp = new float[2 * MAX_FRAME_LENGTH];

		private float[] gLastPhase = new float[MAX_FRAME_LENGTH / 2 + 1];

		private float[] gSumPhase = new float[MAX_FRAME_LENGTH / 2 + 1];

		private float[] gOutputAccum = new float[2 * MAX_FRAME_LENGTH];

		private float[] gAnaFreq = new float[MAX_FRAME_LENGTH];

		private float[] gAnaMagn = new float[MAX_FRAME_LENGTH];

		private float[] gSynFreq = new float[MAX_FRAME_LENGTH];

		private float[] gSynMagn = new float[MAX_FRAME_LENGTH];

		private long gRover;

		public void PitchShift(float pitchShift, long numSampsToProcess, float sampleRate, float[] indata)
		{
			PitchShift(pitchShift, numSampsToProcess, 2048L, 10L, sampleRate, indata);
		}

		public void PitchShift(float pitchShift, long numSampsToProcess, long fftFrameSize, long osamp, float sampleRate, float[] indata)
		{
			long num = fftFrameSize / 2;
			long num2 = fftFrameSize / osamp;
			double num3 = (double)sampleRate / (double)fftFrameSize;
			double num4 = Math.PI * 2.0 * (double)num2 / (double)fftFrameSize;
			long num5 = fftFrameSize - num2;
			if (gRover == 0L)
			{
				gRover = num5;
			}
			for (long num6 = 0L; num6 < numSampsToProcess; num6++)
			{
				gInFIFO[gRover] = indata[num6];
				indata[num6] = gOutFIFO[gRover - num5];
				gRover++;
				if (gRover < fftFrameSize)
				{
					continue;
				}
				gRover = num5;
				for (long num7 = 0L; num7 < fftFrameSize; num7++)
				{
					double num8 = -0.5 * Math.Cos(Math.PI * 2.0 * (double)num7 / (double)fftFrameSize) + 0.5;
					gFFTworksp[2 * num7] = (float)((double)gInFIFO[num7] * num8);
					gFFTworksp[2 * num7 + 1] = 0f;
				}
				ShortTimeFourierTransform(gFFTworksp, fftFrameSize, -1L);
				for (long num7 = 0L; num7 <= num; num7++)
				{
					double num9 = gFFTworksp[2 * num7];
					double num10 = gFFTworksp[2 * num7 + 1];
					double num11 = 2.0 * Math.Sqrt(num9 * num9 + num10 * num10);
					double num12 = Math.Atan2(num10, num9);
					double num13 = num12 - (double)gLastPhase[num7];
					gLastPhase[num7] = (float)num12;
					num13 -= (double)num7 * num4;
					long num14 = (long)(num13 / Math.PI);
					num14 = ((num14 < 0) ? (num14 - (num14 & 1)) : (num14 + (num14 & 1)));
					num13 -= Math.PI * (double)num14;
					num13 = (double)osamp * num13 / (Math.PI * 2.0);
					num13 = (double)num7 * num3 + num13 * num3;
					gAnaMagn[num7] = (float)num11;
					gAnaFreq[num7] = (float)num13;
				}
				for (int i = 0; i < fftFrameSize; i++)
				{
					gSynMagn[i] = 0f;
					gSynFreq[i] = 0f;
				}
				for (long num7 = 0L; num7 <= num; num7++)
				{
					long num15 = (long)((float)num7 * pitchShift);
					if (num15 <= num)
					{
						gSynMagn[num15] += gAnaMagn[num7];
						gSynFreq[num15] = gAnaFreq[num7] * pitchShift;
					}
				}
				for (long num7 = 0L; num7 <= num; num7++)
				{
					double num11 = gSynMagn[num7];
					double num13 = gSynFreq[num7];
					num13 -= (double)num7 * num3;
					num13 /= num3;
					num13 = Math.PI * 2.0 * num13 / (double)osamp;
					num13 += (double)num7 * num4;
					gSumPhase[num7] += (float)num13;
					double num12 = gSumPhase[num7];
					gFFTworksp[2 * num7] = (float)(num11 * Math.Cos(num12));
					gFFTworksp[2 * num7 + 1] = (float)(num11 * Math.Sin(num12));
				}
				for (long num7 = fftFrameSize + 2; num7 < 2 * fftFrameSize; num7++)
				{
					gFFTworksp[num7] = 0f;
				}
				ShortTimeFourierTransform(gFFTworksp, fftFrameSize, 1L);
				for (long num7 = 0L; num7 < fftFrameSize; num7++)
				{
					double num8 = -0.5 * Math.Cos(Math.PI * 2.0 * (double)num7 / (double)fftFrameSize) + 0.5;
					gOutputAccum[num7] += (float)(2.0 * num8 * (double)gFFTworksp[2 * num7] / (double)(num * osamp));
				}
				for (long num7 = 0L; num7 < num2; num7++)
				{
					gOutFIFO[num7] = gOutputAccum[num7];
				}
				for (long num7 = 0L; num7 < fftFrameSize; num7++)
				{
					gOutputAccum[num7] = gOutputAccum[num7 + num2];
				}
				for (long num7 = 0L; num7 < num5; num7++)
				{
					gInFIFO[num7] = gInFIFO[num7 + num2];
				}
			}
		}

		public void ShortTimeFourierTransform(float[] fftBuffer, long fftFrameSize, long sign)
		{
			for (long num = 2L; num < 2 * fftFrameSize - 2; num += 2)
			{
				long num2 = 2L;
				long num3 = 0L;
				while (num2 < 2 * fftFrameSize)
				{
					if ((num & num2) != 0L)
					{
						num3++;
					}
					num3 <<= 1;
					num2 <<= 1;
				}
				if (num < num3)
				{
					float num4 = fftBuffer[num];
					fftBuffer[num] = fftBuffer[num3];
					fftBuffer[num3] = num4;
					num4 = fftBuffer[num + 1];
					fftBuffer[num + 1] = fftBuffer[num3 + 1];
					fftBuffer[num3 + 1] = num4;
				}
			}
			long num5 = (long)(Math.Log(fftFrameSize) / Math.Log(2.0) + 0.5);
			long num6 = 0L;
			long num7 = 2L;
			for (; num6 < num5; num6++)
			{
				num7 <<= 1;
				long num8 = num7 >> 1;
				float num9 = 1f;
				float num10 = 0f;
				float num11 = (float)Math.PI / (float)(num8 >> 1);
				float num12 = (float)Math.Cos(num11);
				float num13 = (float)((double)sign * Math.Sin(num11));
				for (long num3 = 0L; num3 < num8; num3 += 2)
				{
					float num14;
					for (long num = num3; num < 2 * fftFrameSize; num += num7)
					{
						num14 = fftBuffer[num + num8] * num9 - fftBuffer[num + num8 + 1] * num10;
						float num15 = fftBuffer[num + num8] * num10 + fftBuffer[num + num8 + 1] * num9;
						fftBuffer[num + num8] = fftBuffer[num] - num14;
						fftBuffer[num + num8 + 1] = fftBuffer[num + 1] - num15;
						fftBuffer[num] += num14;
						fftBuffer[num + 1] += num15;
					}
					num14 = num9 * num12 - num10 * num13;
					num10 = num9 * num13 + num10 * num12;
					num9 = num14;
				}
			}
		}
	}
	internal class WdlResampler
	{
		private class WDL_Resampler_IIRFilter
		{
			private double m_fpos;

			private double m_a1;

			private double m_a2;

			private double m_b0;

			private double m_b1;

			private double m_b2;

			private double[,] m_hist;

			public WDL_Resampler_IIRFilter()
			{
				m_fpos = -1.0;
				Reset();
			}

			public void Reset()
			{
				m_hist = new double[256, 4];
			}

			public void setParms(double fpos, double Q)
			{
				if (!(Math.Abs(fpos - m_fpos) < 1E-06))
				{
					m_fpos = fpos;
					double num = fpos * Math.PI;
					double num2 = Math.Cos(num);
					double num3 = Math.Sin(num) / (2.0 * Q);
					double num4 = 1.0 / (1.0 + num3);
					m_b1 = (1.0 - num2) * num4;
					m_b2 = (m_b0 = m_b1 * 0.5);
					m_a1 = -2.0 * num2 * num4;
					m_a2 = (1.0 - num3) * num4;
				}
			}

			public void Apply(float[] inBuffer, int inIndex, float[] outBuffer, int outIndex, int ns, int span, int w)
			{
				double b = m_b0;
				double b2 = m_b1;
				double b3 = m_b2;
				double a = m_a1;
				double a2 = m_a2;
				while (ns-- != 0)
				{
					double num = inBuffer[inIndex];
					inIndex += span;
					double x = num * b + m_hist[w, 0] * b2 + m_hist[w, 1] * b3 - m_hist[w, 2] * a - m_hist[w, 3] * a2;
					m_hist[w, 1] = m_hist[w, 0];
					m_hist[w, 0] = num;
					m_hist[w, 3] = m_hist[w, 2];
					m_hist[w, 2] = denormal_filter(x);
					outBuffer[outIndex] = (float)m_hist[w, 2];
					outIndex += span;
				}
			}

			private double denormal_filter(float x)
			{
				return x;
			}

			private double denormal_filter(double x)
			{
				return x;
			}
		}

		private const int WDL_RESAMPLE_MAX_FILTERS = 4;

		private const int WDL_RESAMPLE_MAX_NCH = 64;

		private const double PI = Math.PI;

		private double m_sratein;

		private double m_srateout;

		private double m_fracpos;

		private double m_ratio;

		private double m_filter_ratio;

		private float m_filterq;

		private float m_filterpos;

		private float[] m_rsinbuf;

		private float[] m_filter_coeffs;

		private WDL_Resampler_IIRFilter m_iirfilter;

		private int m_filter_coeffs_size;

		private int m_last_requested;

		private int m_filtlatency;

		private int m_samples_in_rsinbuf;

		private int m_lp_oversize;

		private int m_sincsize;

		private int m_filtercnt;

		private int m_sincoversize;

		private bool m_interp;

		private bool m_feedmode;

		public WdlResampler()
		{
			m_filterq = 0.707f;
			m_filterpos = 0.693f;
			m_sincoversize = 0;
			m_lp_oversize = 1;
			m_sincsize = 0;
			m_filtercnt = 1;
			m_interp = true;
			m_feedmode = false;
			m_filter_coeffs_size = 0;
			m_sratein = 44100.0;
			m_srateout = 44100.0;
			m_ratio = 1.0;
			m_filter_ratio = -1.0;
			Reset();
		}

		public void SetMode(bool interp, int filtercnt, bool sinc, int sinc_size = 64, int sinc_interpsize = 32)
		{
			m_sincsize = ((sinc && sinc_size >= 4) ? ((sinc_size > 8192) ? 8192 : sinc_size) : 0);
			m_sincoversize = ((m_sincsize == 0) ? 1 : ((sinc_interpsize <= 1) ? 1 : ((sinc_interpsize >= 4096) ? 4096 : sinc_interpsize)));
			m_filtercnt = ((m_sincsize == 0) ? ((filtercnt > 0) ? ((filtercnt >= 4) ? 4 : filtercnt) : 0) : 0);
			m_interp = interp && m_sincsize == 0;
			if (m_sincsize == 0)
			{
				m_filter_coeffs = new float[0];
				m_filter_coeffs_size = 0;
			}
			if (m_filtercnt == 0)
			{
				m_iirfilter = null;
			}
		}

		public void SetFilterParms(float filterpos = 0.693f, float filterq = 0.707f)
		{
			m_filterpos = filterpos;
			m_filterq = filterq;
		}

		public void SetFeedMode(bool wantInputDriven)
		{
			m_feedmode = wantInputDriven;
		}

		public void Reset(double fracpos = 0.0)
		{
			m_last_requested = 0;
			m_filtlatency = 0;
			m_fracpos = fracpos;
			m_samples_in_rsinbuf = 0;
			if (m_iirfilter != null)
			{
				m_iirfilter.Reset();
			}
		}

		public void SetRates(double rate_in, double rate_out)
		{
			if (rate_in < 1.0)
			{
				rate_in = 1.0;
			}
			if (rate_out < 1.0)
			{
				rate_out = 1.0;
			}
			if (rate_in != m_sratein || rate_out != m_srateout)
			{
				m_sratein = rate_in;
				m_srateout = rate_out;
				m_ratio = m_sratein / m_srateout;
			}
		}

		public double GetCurrentLatency()
		{
			double num = ((double)m_samples_in_rsinbuf - (double)m_filtlatency) / m_sratein;
			if (num < 0.0)
			{
				num = 0.0;
			}
			return num;
		}

		public int ResamplePrepare(int out_samples, int nch, out float[] inbuffer, out int inbufferOffset)
		{
			if (nch > 64 || nch < 1)
			{
				inbuffer = null;
				inbufferOffset = 0;
				return 0;
			}
			int num = 0;
			if (m_sincsize > 1)
			{
				num = m_sincsize;
			}
			int num2 = num / 2;
			if (num2 > 1 && m_samples_in_rsinbuf < num2 - 1)
			{
				m_filtlatency += num2 - 1 - m_samples_in_rsinbuf;
				m_samples_in_rsinbuf = num2 - 1;
				if (m_samples_in_rsinbuf > 0)
				{
					m_rsinbuf = new float[m_samples_in_rsinbuf * nch];
				}
			}
			int num3 = 0;
			num3 = (m_feedmode ? out_samples : ((int)(m_ratio * (double)out_samples) + 4 + num - m_samples_in_rsinbuf));
			if (num3 < 0)
			{
				num3 = 0;
			}
			while (true)
			{
				Array.Resize(ref m_rsinbuf, (m_samples_in_rsinbuf + num3) * nch);
				int num4 = m_rsinbuf.Length / ((nch == 0) ? 1 : nch) - m_samples_in_rsinbuf;
				if (num4 == num3)
				{
					break;
				}
				if (num3 > 4 && num4 == 0)
				{
					num3 /= 2;
					continue;
				}
				num3 = num4;
				break;
			}
			inbuffer = m_rsinbuf;
			inbufferOffset = m_samples_in_rsinbuf * nch;
			m_last_requested = num3;
			return num3;
		}

		public int ResampleOut(float[] outBuffer, int outBufferIndex, int nsamples_in, int nsamples_out, int nch)
		{
			if (nch > 64 || nch < 1)
			{
				return 0;
			}
			if (m_filtercnt > 0 && m_ratio > 1.0 && nsamples_in > 0)
			{
				if (m_iirfilter == null)
				{
					m_iirfilter = new WDL_Resampler_IIRFilter();
				}
				int filtercnt = m_filtercnt;
				m_iirfilter.setParms(1.0 / m_ratio * (double)m_filterpos, m_filterq);
				int num = m_samples_in_rsinbuf * nch;
				int num2 = 0;
				for (int i = 0; i < nch; i++)
				{
					for (int j = 0; j < filtercnt; j++)
					{
						m_iirfilter.Apply(m_rsinbuf, num + i, m_rsinbuf, num + i, nsamples_in, nch, num2++);
					}
				}
			}
			m_samples_in_rsinbuf += Math.Min(nsamples_in, m_last_requested);
			int num3 = m_samples_in_rsinbuf;
			if (nsamples_in < m_last_requested)
			{
				int num4 = (m_last_requested - nsamples_in) * 2 + m_sincsize * 2;
				int num5 = (m_samples_in_rsinbuf + num4) * nch;
				Array.Resize(ref m_rsinbuf, num5);
				if (m_rsinbuf.Length == num5)
				{
					Array.Clear(m_rsinbuf, m_samples_in_rsinbuf * nch, num4 * nch);
					num3 = m_samples_in_rsinbuf + num4;
				}
			}
			int num6 = 0;
			double num7 = m_fracpos;
			double ratio = m_ratio;
			int num8 = 0;
			int num9 = outBufferIndex;
			int num10 = nsamples_out;
			int num11 = 0;
			if (m_sincsize != 0)
			{
				if (m_ratio > 1.0)
				{
					BuildLowPass(1.0 / (m_ratio * 1.03));
				}
				else
				{
					BuildLowPass(1.0);
				}
				int filter_coeffs_size = m_filter_coeffs_size;
				int num12 = num3 - filter_coeffs_size;
				num11 = filter_coeffs_size / 2 - 1;
				int filterIndex = 0;
				if (nch == 1)
				{
					while (num10-- != 0)
					{
						int num13 = (int)num7;
						if (num13 >= num12 - 1)
						{
							break;
						}
						SincSample1(outBuffer, num9, m_rsinbuf, num8 + num13, num7 - (double)num13, m_filter_coeffs, filterIndex, filter_coeffs_size);
						num9++;
						num7 += ratio;
						num6++;
					}
				}
				else if (nch == 2)
				{
					while (num10-- != 0)
					{
						int num14 = (int)num7;
						if (num14 >= num12 - 1)
						{
							break;
						}
						SincSample2(outBuffer, num9, m_rsinbuf, num8 + num14 * 2, num7 - (double)num14, m_filter_coeffs, filterIndex, filter_coeffs_size);
						num9 += 2;
						num7 += ratio;
						num6++;
					}
				}
				else
				{
					while (num10-- != 0)
					{
						int num15 = (int)num7;
						if (num15 >= num12 - 1)
						{
							break;
						}
						SincSample(outBuffer, num9, m_rsinbuf, num8 + num15 * nch, num7 - (double)num15, nch, m_filter_coeffs, filterIndex, filter_coeffs_size);
						num9 += nch;
						num7 += ratio;
						num6++;
					}
				}
			}
			else if (!m_interp)
			{
				if (nch == 1)
				{
					while (num10-- != 0)
					{
						int num16 = (int)num7;
						if (num16 >= num3)
						{
							break;
						}
						outBuffer[num9++] = m_rsinbuf[num8 + num16];
						num7 += ratio;
						num6++;
					}
				}
				else if (nch == 2)
				{
					while (num10-- != 0)
					{
						int num17 = (int)num7;
						if (num17 >= num3)
						{
							break;
						}
						num17 += num17;
						outBuffer[num9] = m_rsinbuf[num8 + num17];
						outBuffer[num9 + 1] = m_rsinbuf[num8 + num17 + 1];
						num9 += 2;
						num7 += ratio;
						num6++;
					}
				}
				else
				{
					while (num10-- != 0)
					{
						int num18 = (int)num7;
						if (num18 >= num3)
						{
							break;
						}
						Array.Copy(m_rsinbuf, num8 + num18 * nch, outBuffer, num9, nch);
						num9 += nch;
						num7 += ratio;
						num6++;
					}
				}
			}
			else if (nch == 1)
			{
				while (num10-- != 0)
				{
					int num19 = (int)num7;
					double num20 = num7 - (double)num19;
					if (num19 >= num3 - 1)
					{
						break;
					}
					double num21 = 1.0 - num20;
					int num22 = num8 + num19;
					outBuffer[num9++] = (float)((double)m_rsinbuf[num22] * num21 + (double)m_rsinbuf[num22 + 1] * num20);
					num7 += ratio;
					num6++;
				}
			}
			else if (nch == 2)
			{
				while (num10-- != 0)
				{
					int num23 = (int)num7;
					double num24 = num7 - (double)num23;
					if (num23 >= num3 - 1)
					{
						break;
					}
					double num25 = 1.0 - num24;
					int num26 = num8 + num23 * 2;
					outBuffer[num9] = (float)((double)m_rsinbuf[num26] * num25 + (double)m_rsinbuf[num26 + 2] * num24);
					outBuffer[num9 + 1] = (float)((double)m_rsinbuf[num26 + 1] * num25 + (double)m_rsinbuf[num26 + 3] * num24);
					num9 += 2;
					num7 += ratio;
					num6++;
				}
			}
			else
			{
				while (num10-- != 0)
				{
					int num27 = (int)num7;
					double num28 = num7 - (double)num27;
					if (num27 >= num3 - 1)
					{
						break;
					}
					double num29 = 1.0 - num28;
					int num30 = nch;
					int num31 = num8 + num27 * nch;
					while (num30-- != 0)
					{
						outBuffer[num9++] = (float)((double)m_rsinbuf[num31] * num29 + (double)m_rsinbuf[num31 + nch] * num28);
						num31++;
					}
					num7 += ratio;
					num6++;
				}
			}
			if (m_filtercnt > 0 && m_ratio < 1.0 && num6 > 0)
			{
				if (m_iirfilter == null)
				{
					m_iirfilter = new WDL_Resampler_IIRFilter();
				}
				int filtercnt2 = m_filtercnt;
				m_iirfilter.setParms(m_ratio * (double)m_filterpos, m_filterq);
				int num32 = 0;
				for (int k = 0; k < nch; k++)
				{
					for (int l = 0; l < filtercnt2; l++)
					{
						m_iirfilter.Apply(outBuffer, k, outBuffer, k, num6, nch, num32++);
					}
				}
			}
			if (num6 > 0 && num3 > m_samples_in_rsinbuf)
			{
				double num33 = (num7 - (double)m_samples_in_rsinbuf + (double)num11) / ratio;
				if (num33 > 0.0)
				{
					num6 -= (int)(num33 + 0.5);
					if (num6 < 0)
					{
						num6 = 0;
					}
				}
			}
			int num34 = (int)num7;
			m_fracpos = num7 - (double)num34;
			m_samples_in_rsinbuf -= num34;
			if (m_samples_in_rsinbuf <= 0)
			{
				m_samples_in_rsinbuf = 0;
			}
			else
			{
				Array.Copy(m_rsinbuf, num8 + num34 * nch, m_rsinbuf, num8, m_samples_in_rsinbuf * nch);
			}
			return num6;
		}

		private void BuildLowPass(double filtpos)
		{
			int sincsize = m_sincsize;
			int sincoversize = m_sincoversize;
			if (m_filter_ratio == filtpos && m_filter_coeffs_size == sincsize && m_lp_oversize == sincoversize)
			{
				return;
			}
			m_lp_oversize = sincoversize;
			m_filter_ratio = filtpos;
			int num = (sincsize + 1) * m_lp_oversize;
			Array.Resize(ref m_filter_coeffs, num);
			if (m_filter_coeffs.Length == num)
			{
				m_filter_coeffs_size = sincsize;
				int num2 = sincsize * m_lp_oversize;
				int num3 = num2 / 2;
				double num4 = 0.0;
				double num5 = 0.0;
				double num6 = Math.PI * 2.0 / (double)num2;
				double num7 = Math.PI / (double)m_lp_oversize * filtpos;
				double num8 = num7 * (double)(-num3);
				for (int i = -num3; i < num3 + m_lp_oversize; i++)
				{
					double num9 = 287.0 / 800.0 - 0.48829 * Math.Cos(num5) + 0.14128 * Math.Cos(2.0 * num5) - 0.01168 * Math.Cos(6.0 * num5);
					if (i != 0)
					{
						num9 *= Math.Sin(num8) / num8;
					}
					num5 += num6;
					num8 += num7;
					m_filter_coeffs[num3 + i] = (float)num9;
					if (i < num3)
					{
						num4 += num9;
					}
				}
				num4 = (double)m_lp_oversize / num4;
				for (int i = 0; i < num2 + m_lp_oversize; i++)
				{
					m_filter_coeffs[i] = (float)((double)m_filter_coeffs[i] * num4);
				}
			}
			else
			{
				m_filter_coeffs_size = 0;
			}
		}

		private void SincSample(float[] outBuffer, int outBufferIndex, float[] inBuffer, int inBufferIndex, double fracpos, int nch, float[] filter, int filterIndex, int filtsz)
		{
			int lp_oversize = m_lp_oversize;
			fracpos *= (double)lp_oversize;
			int num = (int)fracpos;
			filterIndex += lp_oversize - 1 - num;
			fracpos -= (double)num;
			for (int i = 0; i < nch; i++)
			{
				double num2 = 0.0;
				double num3 = 0.0;
				int num4 = filterIndex;
				int num5 = inBufferIndex + i;
				int num6 = filtsz;
				while (num6-- != 0)
				{
					num2 += (double)(filter[num4] * inBuffer[num5]);
					num3 += (double)(filter[num4 + 1] * inBuffer[num5]);
					num5 += nch;
					num4 += lp_oversize;
				}
				outBuffer[outBufferIndex + i] = (float)(num2 * fracpos + num3 * (1.0 - fracpos));
			}
		}

		private void SincSample1(float[] outBuffer, int outBufferIndex, float[] inBuffer, int inBufferIndex, double fracpos, float[] filter, int filterIndex, int filtsz)
		{
			int lp_oversize = m_lp_oversize;
			fracpos *= (double)lp_oversize;
			int num = (int)fracpos;
			filterIndex += lp_oversize - 1 - num;
			fracpos -= (double)num;
			double num2 = 0.0;
			double num3 = 0.0;
			int num4 = filterIndex;
			int num5 = inBufferIndex;
			int num6 = filtsz;
			while (num6-- != 0)
			{
				num2 += (double)(filter[num4] * inBuffer[num5]);
				num3 += (double)(filter[num4 + 1] * inBuffer[num5]);
				num5++;
				num4 += lp_oversize;
			}
			outBuffer[outBufferIndex] = (float)(num2 * fracpos + num3 * (1.0 - fracpos));
		}

		private void SincSample2(float[] outptr, int outBufferIndex, float[] inBuffer, int inBufferIndex, double fracpos, float[] filter, int filterIndex, int filtsz)
		{
			int lp_oversize = m_lp_oversize;
			fracpos *= (double)lp_oversize;
			int num = (int)fracpos;
			filterIndex += lp_oversize - 1 - num;
			fracpos -= (double)num;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			int num6 = filterIndex;
			int num7 = inBufferIndex;
			int num8 = filtsz / 2;
			while (num8-- != 0)
			{
				num2 += (double)(filter[num6] * inBuffer[num7]);
				num3 += (double)(filter[num6] * inBuffer[num7 + 1]);
				num4 += (double)(filter[num6 + 1] * inBuffer[num7]);
				num5 += (double)(filter[num6 + 1] * inBuffer[num7 + 1]);
				num2 += (double)(filter[num6 + lp_oversize] * inBuffer[num7 + 2]);
				num3 += (double)(filter[num6 + lp_oversize] * inBuffer[num7 + 3]);
				num4 += (double)(filter[num6 + lp_oversize + 1] * inBuffer[num7 + 2]);
				num5 += (double)(filter[num6 + lp_oversize + 1] * inBuffer[num7 + 3]);
				num7 += 4;
				num6 += lp_oversize * 2;
			}
			outptr[outBufferIndex] = (float)(num2 * fracpos + num4 * (1.0 - fracpos));
			outptr[outBufferIndex + 1] = (float)(num3 * fracpos + num5 * (1.0 - fracpos));
		}
	}
}
namespace NAudio.Dmo
{
	internal class AudioMediaSubtypes
	{
		public static readonly Guid MEDIASUBTYPE_PCM = new Guid("00000001-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_PCMAudioObsolete = new Guid("e436eb8a-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_MPEG1Packet = new Guid("e436eb80-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_MPEG1Payload = new Guid("e436eb81-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_MPEG2_AUDIO = new Guid("e06d802b-db46-11cf-b4d1-00805f6cbbea");

		public static readonly Guid MEDIASUBTYPE_DVD_LPCM_AUDIO = new Guid("e06d8032-db46-11cf-b4d1-00805f6cbbea");

		public static readonly Guid MEDIASUBTYPE_DRM_Audio = new Guid("00000009-0000-0010-8000-00aa00389b71");

		public static readonly Guid MEDIASUBTYPE_IEEE_FLOAT = new Guid("00000003-0000-0010-8000-00aa00389b71");

		public static readonly Guid MEDIASUBTYPE_DOLBY_AC3 = new Guid("e06d802c-db46-11cf-b4d1-00805f6cbbea");

		public static readonly Guid MEDIASUBTYPE_DOLBY_AC3_SPDIF = new Guid("00000092-0000-0010-8000-00aa00389b71");

		public static readonly Guid MEDIASUBTYPE_RAW_SPORT = new Guid("00000240-0000-0010-8000-00aa00389b71");

		public static readonly Guid MEDIASUBTYPE_SPDIF_TAG_241h = new Guid("00000241-0000-0010-8000-00aa00389b71");

		public static readonly Guid MEDIASUBTYPE_I420 = new Guid("30323449-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_IYUV = new Guid("56555949-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_RGB1 = new Guid("e436eb78-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_RGB24 = new Guid("e436eb7d-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_RGB32 = new Guid("e436eb7e-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_RGB4 = new Guid("e436eb79-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_RGB555 = new Guid("e436eb7c-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_RGB565 = new Guid("e436eb7b-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_RGB8 = new Guid("e436eb7a-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_UYVY = new Guid("59565955-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_VIDEOIMAGE = new Guid("1d4a45f2-e5f6-4b44-8388-f0ae5c0e0c37");

		public static readonly Guid MEDIASUBTYPE_YUY2 = new Guid("32595559-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_YV12 = new Guid("31313259-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_YVU9 = new Guid("39555659-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIASUBTYPE_YVYU = new Guid("55595659-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMFORMAT_MPEG2Video = new Guid("e06d80e3-db46-11cf-b4d1-00805f6cbbea");

		public static readonly Guid WMFORMAT_Script = new Guid("5C8510F2-DEBE-4ca7-BBA5-F07A104F8DFF");

		public static readonly Guid WMFORMAT_VideoInfo = new Guid("05589f80-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid WMFORMAT_WaveFormatEx = new Guid("05589f81-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid WMFORMAT_WebStream = new Guid("da1e6b13-8359-4050-b398-388e965bf00c");

		public static readonly Guid WMMEDIASUBTYPE_ACELPnet = new Guid("00000130-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_Base = new Guid("00000000-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_DRM = new Guid("00000009-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_MP3 = new Guid("00000055-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_MP43 = new Guid("3334504D-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_MP4S = new Guid("5334504D-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_M4S2 = new Guid("3253344D-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_P422 = new Guid("32323450-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_MPEG2_VIDEO = new Guid("e06d8026-db46-11cf-b4d1-00805f6cbbea");

		public static readonly Guid WMMEDIASUBTYPE_MSS1 = new Guid("3153534D-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_MSS2 = new Guid("3253534D-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_PCM = new Guid("00000001-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WebStream = new Guid("776257d4-c627-41cb-8f81-7ac7ff1c40cc");

		public static readonly Guid WMMEDIASUBTYPE_WMAudio_Lossless = new Guid("00000163-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMAudioV2 = new Guid("00000161-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMAudioV7 = new Guid("00000161-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMAudioV8 = new Guid("00000161-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMAudioV9 = new Guid("00000162-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMSP1 = new Guid("0000000A-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMV1 = new Guid("31564D57-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMV2 = new Guid("32564D57-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMV3 = new Guid("33564D57-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMVA = new Guid("41564D57-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WMVP = new Guid("50564D57-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIASUBTYPE_WVP2 = new Guid("32505657-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIATYPE_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIATYPE_FileTransfer = new Guid("D9E47579-930E-4427-ADFC-AD80F290E470");

		public static readonly Guid WMMEDIATYPE_Image = new Guid("34A50FD8-8AA5-4386-81FE-A0EFE0488E31");

		public static readonly Guid WMMEDIATYPE_Script = new Guid("73636d64-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMMEDIATYPE_Text = new Guid("9BBA1EA7-5AB2-4829-BA57-0940209BCF3E");

		public static readonly Guid WMMEDIATYPE_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");

		public static readonly Guid WMSCRIPTTYPE_TwoStrings = new Guid("82f38a70-c29f-11d1-97ad-00a0c95ea850");

		public static readonly Guid MEDIASUBTYPE_WAVE = new Guid("e436eb8b-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_AU = new Guid("e436eb8c-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIASUBTYPE_AIFF = new Guid("e436eb8d-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid[] AudioSubTypes = new Guid[13]
		{
			MEDIASUBTYPE_PCM, MEDIASUBTYPE_PCMAudioObsolete, MEDIASUBTYPE_MPEG1Packet, MEDIASUBTYPE_MPEG1Payload, MEDIASUBTYPE_MPEG2_AUDIO, MEDIASUBTYPE_DVD_LPCM_AUDIO, MEDIASUBTYPE_DRM_Audio, MEDIASUBTYPE_IEEE_FLOAT, MEDIASUBTYPE_DOLBY_AC3, MEDIASUBTYPE_DOLBY_AC3_SPDIF,
			MEDIASUBTYPE_RAW_SPORT, MEDIASUBTYPE_SPDIF_TAG_241h, WMMEDIASUBTYPE_MP3
		};

		public static readonly string[] AudioSubTypeNames = new string[13]
		{
			"PCM", "PCM Obsolete", "MPEG1Packet", "MPEG1Payload", "MPEG2_AUDIO", "DVD_LPCM_AUDIO", "DRM_Audio", "IEEE_FLOAT", "DOLBY_AC3", "DOLBY_AC3_SPDIF",
			"RAW_SPORT", "SPDIF_TAG_241h", "MP3"
		};

		public static string GetAudioSubtypeName(Guid subType)
		{
			for (int i = 0; i < AudioSubTypes.Length; i++)
			{
				if (subType == AudioSubTypes[i])
				{
					return AudioSubTypeNames[i];
				}
			}
			return subType.ToString();
		}
	}
	public class DmoDescriptor
	{
		public string Name { get; private set; }

		public Guid Clsid { get; private set; }

		public DmoDescriptor(string name, Guid clsid)
		{
			Name = name;
			Clsid = clsid;
		}
	}
	public class DmoEnumerator
	{
		public static IEnumerable<DmoDescriptor> GetAudioEffectNames()
		{
			return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_EFFECT);
		}

		public static IEnumerable<DmoDescriptor> GetAudioEncoderNames()
		{
			return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_ENCODER);
		}

		public static IEnumerable<DmoDescriptor> GetAudioDecoderNames()
		{
			return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_DECODER);
		}

		private static IEnumerable<DmoDescriptor> GetDmos(Guid category)
		{
			Marshal.ThrowExceptionForHR(DmoInterop.DMOEnum(ref category, DmoEnumFlags.None, 0, null, 0, null, out var enumDmo));
			int itemsFetched;
			do
			{
				enumDmo.Next(1, out var clsid, out var name, out itemsFetched);
				if (itemsFetched == 1)
				{
					string name2 = Marshal.PtrToStringUni(name);
					Marshal.FreeCoTaskMem(name);
					yield return new DmoDescriptor(name2, clsid);
				}
			}
			while (itemsFetched > 0);
		}
	}
	[Flags]
	internal enum DmoEnumFlags
	{
		None = 0,
		DMO_ENUMF_INCLUDE_KEYED = 1
	}
	internal static class DmoGuids
	{
		public static readonly Guid DMOCATEGORY_AUDIO_DECODER = new Guid("57f2db8b-e6bb-4513-9d43-dcd2a6593125");

		public static readonly Guid DMOCATEGORY_AUDIO_ENCODER = new Guid("33D9A761-90C8-11d0-BD43-00A0C911CE86");

		public static readonly Guid DMOCATEGORY_VIDEO_DECODER = new Guid("4a69b442-28be-4991-969c-b500adf5d8a8");

		public static readonly Guid DMOCATEGORY_VIDEO_ENCODER = new Guid("33D9A760-90C8-11d0-BD43-00A0C911CE86");

		public static readonly Guid DMOCATEGORY_AUDIO_EFFECT = new Guid("f3602b3f-0592-48df-a4cd-674721e7ebeb");

		public static readonly Guid DMOCATEGORY_VIDEO_EFFECT = new Guid("d990ee14-776c-4723-be46-3da2f56f10b9");

		public static readonly Guid DMOCATEGORY_AUDIO_CAPTURE_EFFECT = new Guid("f665aaba-3e09-4920-aa5f-219811148f09");
	}
	internal static class DmoMediaTypeGuids
	{
		public static readonly Guid FORMAT_None = new Guid("0F6417D6-C318-11D0-A43F-00A0C9223196");

		public static readonly Guid FORMAT_VideoInfo = new Guid("05589f80-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid FORMAT_VideoInfo2 = new Guid("F72A76A0-EB0A-11d0-ACE4-0000C0CC16BA");

		public static readonly Guid FORMAT_WaveFormatEx = new Guid("05589f81-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid FORMAT_MPEGVideo = new Guid("05589f82-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid FORMAT_MPEGStreams = new Guid("05589f83-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid FORMAT_DvInfo = new Guid("05589f84-c356-11ce-bf01-00aa0055595a");

		public static readonly Guid FORMAT_525WSS = new Guid("C7ECF04D-4582-4869-9ABB-BFB523B62EDF");
	}
	internal enum DmoHResults
	{
		DMO_E_INVALIDSTREAMINDEX = -2147220991,
		DMO_E_INVALIDTYPE,
		DMO_E_TYPE_NOT_SET,
		DMO_E_NOTACCEPTING,
		DMO_E_TYPE_NOT_ACCEPTED,
		DMO_E_NO_MORE_ITEMS
	}
	[Flags]
	public enum DmoInPlaceProcessFlags
	{
		Normal = 0,
		Zero = 1
	}
	public enum DmoInPlaceProcessReturn
	{
		Normal,
		HasEffectTail
	}
	[Flags]
	public enum DmoInputDataBufferFlags
	{
		None = 0,
		SyncPoint = 1,
		Time = 2,
		TimeLength = 4
	}
	[Flags]
	internal enum DmoInputStatusFlags
	{
		None = 0,
		DMO_INPUT_STATUSF_ACCEPT_DATA = 1
	}
	internal static class DmoInterop
	{
		[DllImport("msdmo.dll")]
		public static extern int DMOEnum([In] ref Guid guidCategory, DmoEnumFlags flags, int inTypes, [In] DmoPartialMediaType[] inTypesArray, int outTypes, [In] DmoPartialMediaType[] outTypesArray, out IEnumDmo enumDmo);

		[DllImport("msdmo.dll")]
		public static extern int MoFreeMediaType([In] ref DmoMediaType mediaType);

		[DllImport("msdmo.dll")]
		public static extern int MoInitMediaType([In][Out] ref DmoMediaType mediaType, int formatBlockBytes);

		[DllImport("msdmo.dll")]
		public static extern int DMOGetName([In] ref Guid clsidDMO, [Out] StringBuilder name);
	}
	public struct DmoMediaType
	{
		private Guid majortype;

		private Guid subtype;

		private bool bFixedSizeSamples;

		private bool bTemporalCompression;

		private int lSampleSize;

		private Guid formattype;

		private IntPtr pUnk;

		private int cbFormat;

		private IntPtr pbFormat;

		public Guid MajorType => majortype;

		public string MajorTypeName => MediaTypes.GetMediaTypeName(majortype);

		public Guid SubType => subtype;

		public string SubTypeName
		{
			get
			{
				if (majortype == MediaTypes.MEDIATYPE_Audio)
				{
					return AudioMediaSubtypes.GetAudioSubtypeName(subtype);
				}
				return subtype.ToString();
			}
		}

		public bool FixedSizeSamples => bFixedSizeSamples;

		public int SampleSize => lSampleSize;

		public Guid FormatType => formattype;

		public string FormatTypeName
		{
			get
			{
				if (formattype == DmoMediaTypeGuids.FORMAT_None)
				{
					return "None";
				}
				if (formattype == Guid.Empty)
				{
					return "Null";
				}
				if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
				{
					return "WaveFormatEx";
				}
				return FormatType.ToString();
			}
		}

		public WaveFormat GetWaveFormat()
		{
			if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
			{
				return WaveFormat.MarshalFromPtr(pbFormat);
			}
			throw new InvalidOperationException("Not a WaveFormat type");
		}

		public void SetWaveFormat(WaveFormat waveFormat)
		{
			majortype = MediaTypes.MEDIATYPE_Audio;
			if (waveFormat is WaveFormatExtensible waveFormatExtensible)
			{
				subtype = waveFormatExtensible.SubFormat;
			}
			else
			{
				switch (waveFormat.Encoding)
				{
				case WaveFormatEncoding.Pcm:
					subtype = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
					break;
				case WaveFormatEncoding.IeeeFloat:
					subtype = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
					break;
				case WaveFormatEncoding.MpegLayer3:
					subtype = AudioMediaSubtypes.WMMEDIASUBTYPE_MP3;
					break;
				default:
					throw new ArgumentException($"Not a supported encoding {waveFormat.Encoding}");
				}
			}
			bFixedSizeSamples = SubType == AudioMediaSubtypes.MEDIASUBTYPE_PCM || SubType == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
			formattype = DmoMediaTypeGuids.FORMAT_WaveFormatEx;
			if (cbFormat < Marshal.SizeOf(waveFormat))
			{
				throw new InvalidOperationException("Not enough memory assigned for a WaveFormat structure");
			}
			Marshal.StructureToPtr(waveFormat, pbFormat, fDeleteOld: false);
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct DmoOutputDataBuffer : IDisposable
	{
		[MarshalAs(UnmanagedType.Interface)]
		private IMediaBuffer pBuffer;

		private DmoOutputDataBufferFlags dwStatus;

		private long rtTimestamp;

		private long referenceTimeDuration;

		public IMediaBuffer MediaBuffer
		{
			get
			{
				return pBuffer;
			}
			internal set
			{
				pBuffer = value;
			}
		}

		public int Length => ((MediaBuffer)pBuffer).Length;

		public DmoOutputDataBufferFlags StatusFlags
		{
			get
			{
				return dwStatus;
			}
			internal set
			{
				dwStatus = value;
			}
		}

		public long Timestamp
		{
			get
			{
				return rtTimestamp;
			}
			internal set
			{
				rtTimestamp = value;
			}
		}

		public long Duration
		{
			get
			{
				return referenceTimeDuration;
			}
			internal set
			{
				referenceTimeDuration = value;
			}
		}

		public bool MoreDataAvailable => (StatusFlags & DmoOutputDataBufferFlags.Incomplete) == DmoOutputDataBufferFlags.Incomplete;

		public DmoOutputDataBuffer(int maxBufferSize)
		{
			pBuffer = new MediaBuffer(maxBufferSize);
			dwStatus = DmoOutputDataBufferFlags.None;
			rtTimestamp = 0L;
			referenceTimeDuration = 0L;
		}

		public void Dispose()
		{
			if (pBuffer != null)
			{
				((MediaBuffer)pBuffer).Dispose();
				pBuffer = null;
				GC.SuppressFinalize(this);
			}
		}

		public void RetrieveData(byte[] data, int offset)
		{
			((MediaBuffer)pBuffer).RetrieveData(data, offset);
		}
	}
	[Flags]
	public enum DmoOutputDataBufferFlags
	{
		None = 0,
		SyncPoint = 1,
		Time = 2,
		TimeLength = 4,
		Incomplete = 0x1000000
	}
	internal struct DmoPartialMediaType
	{
		private Guid type;

		private Guid subtype;

		public Guid Type
		{
			get
			{
				return type;
			}
			internal set
			{
				type = value;
			}
		}

		public Guid Subtype
		{
			get
			{
				return subtype;
			}
			internal set
			{
				subtype = value;
			}
		}
	}
	[Flags]
	public enum DmoProcessOutputFlags
	{
		None = 0,
		DiscardWhenNoBuffer = 1
	}
	[Flags]
	internal enum DmoSetTypeFlags
	{
		None = 0,
		DMO_SET_TYPEF_TEST_ONLY = 1,
		DMO_SET_TYPEF_CLEAR = 2
	}
	[Guid("2c3cd98a-2bfa-4a53-9c27-5249ba64ba0f")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumDmo
	{
		int Next(int itemsToFetch, out Guid clsid, out IntPtr name, out int itemsFetched);

		int Skip(int itemsToSkip);

		int Reset();

		int Clone(out IEnumDmo enumPointer);
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("59eff8b9-938c-4a26-82f2-95cb84cdc837")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMediaBuffer
	{
		[PreserveSig]
		int SetLength(int length);

		[PreserveSig]
		int GetMaxLength(out int maxLength);

		[PreserveSig]
		int GetBufferAndLength(IntPtr bufferPointerPointer, IntPtr validDataLengthPointer);
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("d8ad0f58-5494-4102-97c5-ec798e59bcf4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMediaObject
	{
		[PreserveSig]
		int GetStreamCount(out int inputStreams, out int outputStreams);

		[PreserveSig]
		int GetInputStreamInfo(int inputStreamIndex, out InputStreamInfoFlags flags);

		[PreserveSig]
		int GetOutputStreamInfo(int outputStreamIndex, out OutputStreamInfoFlags flags);

		[PreserveSig]
		int GetInputType(int inputStreamIndex, int typeIndex, out DmoMediaType mediaType);

		[PreserveSig]
		int GetOutputType(int outputStreamIndex, int typeIndex, out DmoMediaType mediaType);

		[PreserveSig]
		int SetInputType(int inputStreamIndex, [In] ref DmoMediaType mediaType, DmoSetTypeFlags flags);

		[PreserveSig]
		int SetOutputType(int outputStreamIndex, [In] ref DmoMediaType mediaType, DmoSetTypeFlags flags);

		[PreserveSig]
		int GetInputCurrentType(int inputStreamIndex, out DmoMediaType mediaType);

		[PreserveSig]
		int GetOutputCurrentType(int outputStreamIndex, out DmoMediaType mediaType);

		[PreserveSig]
		int GetInputSizeInfo(int inputStreamIndex, out int size, out int maxLookahead, out int alignment);

		[PreserveSig]
		int GetOutputSizeInfo(int outputStreamIndex, out int size, out int alignment);

		[PreserveSig]
		int GetInputMaxLatency(int inputStreamIndex, out long referenceTimeMaxLatency);

		[PreserveSig]
		int SetInputMaxLatency(int inputStreamIndex, long referenceTimeMaxLatency);

		[PreserveSig]
		int Flush();

		[PreserveSig]
		int Discontinuity(int inputStreamIndex);

		[PreserveSig]
		int AllocateStreamingResources();

		[PreserveSig]
		int FreeStreamingResources();

		[PreserveSig]
		int GetInputStatus(int inputStreamIndex, out DmoInputStatusFlags flags);

		[PreserveSig]
		int ProcessInput(int inputStreamIndex, [In] IMediaBuffer mediaBuffer, DmoInputDataBufferFlags flags, long referenceTimeTimestamp, long referenceTimeDuration);

		[PreserveSig]
		int ProcessOutput(DmoProcessOutputFlags flags, int outputBufferCount, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] DmoOutputDataBuffer[] outputBuffers, out int statusReserved);

		[PreserveSig]
		int Lock(bool acquireLock);
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("651B9AD0-0FC7-4AA9-9538-D89931010741")]
	internal interface IMediaObjectInPlace
	{
		[PreserveSig]
		int Process([In] int size, [In] IntPtr data, [In] long refTimeStart, [In] DmoInPlaceProcessFlags dwFlags);

		[PreserveSig]
		int Clone([MarshalAs(UnmanagedType.Interface)] out IMediaObjectInPlace mediaObjectInPlace);

		[PreserveSig]
		int GetLatency(out long latencyTime);
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("6d6cbb60-a223-44aa-842f-a2f06750be6d")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMediaParamInfo
	{
		[PreserveSig]
		int GetParamCount(out int paramCount);

		[PreserveSig]
		int GetParamInfo(int paramIndex, ref MediaParamInfo paramInfo);

		[PreserveSig]
		int GetParamText(int paramIndex, out IntPtr paramText);

		[PreserveSig]
		int GetNumTimeFormats(out int numTimeFormats);

		[PreserveSig]
		int GetSupportedTimeFormat(int formatIndex, out Guid guidTimeFormat);

		[PreserveSig]
		int GetCurrentTimeFormat(out Guid guidTimeFormat, out int mediaTimeData);
	}
	[Flags]
	internal enum InputStreamInfoFlags
	{
		None = 0,
		DMO_INPUT_STREAMF_WHOLE_SAMPLES = 1,
		DMO_INPUT_STREAMF_SINGLE_SAMPLE_PER_BUFFER = 2,
		DMO_INPUT_STREAMF_FIXED_SAMPLE_SIZE = 4,
		DMO_INPUT_STREAMF_HOLDS_BUFFERS = 8
	}
	[Guid("E7E9984F-F09F-4da4-903F-6E2E0EFE56B5")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWMResamplerProps
	{
		int SetHalfFilterLength(int outputQuality);

		int SetUserChannelMtx([In] float[] channelConversionMatrix);
	}
	public class MediaBuffer : IMediaBuffer, IDisposable
	{
		private IntPtr buffer;

		private int length;

		private readonly int maxLength;

		public int Length
		{
			get
			{
				return length;
			}
			set
			{
				if (length > maxLength)
				{
					throw new ArgumentException("Cannot be greater than maximum buffer size");
				}
				length = value;
			}
		}

		public MediaBuffer(int maxLength)
		{
			buffer = Marshal.AllocCoTaskMem(maxLength);
			this.maxLength = maxLength;
		}

		public void Dispose()
		{
			if (buffer != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(buffer);
				buffer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		~MediaBuffer()
		{
			Dispose();
		}

		int IMediaBuffer.SetLength(int length)
		{
			if (length > maxLength)
			{
				return -2147483645;
			}
			this.length = length;
			return 0;
		}

		int IMediaBuffer.GetMaxLength(out int maxLength)
		{
			maxLength = this.maxLength;
			return 0;
		}

		int IMediaBuffer.GetBufferAndLength(IntPtr bufferPointerPointer, IntPtr validDataLengthPointer)
		{
			if (bufferPointerPointer != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(bufferPointerPointer, buffer);
			}
			if (validDataLengthPointer != IntPtr.Zero)
			{
				Marshal.WriteInt32(validDataLengthPointer, length);
			}
			return 0;
		}

		public void LoadData(byte[] data, int bytes)
		{
			Length = bytes;
			Marshal.Copy(data, 0, buffer, bytes);
		}

		public void RetrieveData(byte[] data, int offset)
		{
			Marshal.Copy(buffer, data, offset, Length);
		}
	}
	public class MediaObject : IDisposable
	{
		private IMediaObject mediaObject;

		private readonly int inputStreams;

		private readonly int outputStreams;

		public int InputStreamCount => inputStreams;

		public int OutputStreamCount => outputStreams;

		internal MediaObject(IMediaObject mediaObject)
		{
			this.mediaObject = mediaObject;
			mediaObject.GetStreamCount(out inputStreams, out outputStreams);
		}

		public DmoMediaType? GetInputType(int inputStream, int inputTypeIndex)
		{
			try
			{
				if (mediaObject.GetInputType(inputStream, inputTypeIndex, out var mediaType) == 0)
				{
					DmoInterop.MoFreeMediaType(ref mediaType);
					return mediaType;
				}
			}
			catch (COMException exception)
			{
				if (exception.GetHResult() != -2147220986)
				{
					throw;
				}
			}
			return null;
		}

		public DmoMediaType? GetOutputType(int outputStream, int outputTypeIndex)
		{
			try
			{
				if (mediaObject.GetOutputType(outputStream, outputTypeIndex, out var mediaType) == 0)
				{
					DmoInterop.MoFreeMediaType(ref mediaType);
					return mediaType;
				}
			}
			catch (COMException exception)
			{
				if (exception.GetHResult() != -2147220986)
				{
					throw;
				}
			}
			return null;
		}

		public DmoMediaType GetOutputCurrentType(int outputStreamIndex)
		{
			DmoMediaType mediaType;
			int outputCurrentType = mediaObject.GetOutputCurrentType(outputStreamIndex, out mediaType);
			switch (outputCurrentType)
			{
			case 0:
				DmoInterop.MoFreeMediaType(ref mediaType);
				return mediaType;
			case -2147220989:
				throw new InvalidOperationException("Media type was not set.");
			default:
				throw Marshal.GetExceptionForHR(outputCurrentType);
			}
		}

		public IEnumerable<DmoMediaType> GetInputTypes(int inputStreamIndex)
		{
			int typeIndex = 0;
			while (true)
			{
				DmoMediaType? inputType;
				DmoMediaType? dmoMediaType = (inputType = GetInputType(inputStreamIndex, typeIndex));
				if (dmoMediaType.HasValue)
				{
					yield return inputType.Value;
					typeIndex++;
					continue;
				}
				break;
			}
		}

		public IEnumerable<DmoMediaType> GetOutputTypes(int outputStreamIndex)
		{
			int typeIndex = 0;
			while (true)
			{
				DmoMediaType? outputType;
				DmoMediaType? dmoMediaType = (outputType = GetOutputType(outputStreamIndex, typeIndex));
				if (dmoMediaType.HasValue)
				{
					yield return outputType.Value;
					typeIndex++;
					continue;
				}
				break;
			}
		}

		public bool SupportsInputType(int inputStreamIndex, DmoMediaType mediaType)
		{
			return SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
		}

		private bool SetInputType(int inputStreamIndex, DmoMediaType mediaType, DmoSetTypeFlags flags)
		{
			switch (mediaObject.SetInputType(inputStreamIndex, ref mediaType, flags))
			{
			case -2147220991:
				throw new ArgumentException("Invalid stream index");
			default:
				_ = -2147220987;
				return false;
			case 0:
				return true;
			}
		}

		public void SetInputType(int inputStreamIndex, DmoMediaType mediaType)
		{
			if (!SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.None))
			{
				throw new ArgumentException("Media Type not supported");
			}
		}

		public void SetInputWaveFormat(int inputStreamIndex, WaveFormat waveFormat)
		{
			DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
			bool num = SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.None);
			DmoInterop.MoFreeMediaType(ref mediaType);
			if (!num)
			{
				throw new ArgumentException("Media Type not supported");
			}
		}

		public bool SupportsInputWaveFormat(int inputStreamIndex, WaveFormat waveFormat)
		{
			DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
			bool result = SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
			DmoInterop.MoFreeMediaType(ref mediaType);
			return result;
		}

		private DmoMediaType CreateDmoMediaTypeForWaveFormat(WaveFormat waveFormat)
		{
			DmoMediaType mediaType = default(DmoMediaType);
			int formatBlockBytes = Marshal.SizeOf(waveFormat);
			DmoInterop.MoInitMediaType(ref mediaType, formatBlockBytes);
			mediaType.SetWaveFormat(waveFormat);
			return mediaType;
		}

		public bool SupportsOutputType(int outputStreamIndex, DmoMediaType mediaType)
		{
			return SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
		}

		public bool SupportsOutputWaveFormat(int outputStreamIndex, WaveFormat waveFormat)
		{
			DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
			bool result = SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
			DmoInterop.MoFreeMediaType(ref mediaType);
			return result;
		}

		private bool SetOutputType(int outputStreamIndex, DmoMediaType mediaType, DmoSetTypeFlags flags)
		{
			int num = mediaObject.SetOutputType(outputStreamIndex, ref mediaType, flags);
			return num switch
			{
				-2147220987 => false, 
				0 => true, 
				_ => throw Marshal.GetExceptionForHR(num), 
			};
		}

		public void SetOutputType(int outputStreamIndex, DmoMediaType mediaType)
		{
			if (!SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.None))
			{
				throw new ArgumentException("Media Type not supported");
			}
		}

		public void SetOutputWaveFormat(int outputStreamIndex, WaveFormat waveFormat)
		{
			DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
			bool num = SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.None);
			DmoInterop.MoFreeMediaType(ref mediaType);
			if (!num)
			{
				throw new ArgumentException("Media Type not supported");
			}
		}

		public MediaObjectSizeInfo GetInputSizeInfo(int inputStreamIndex)
		{
			Marshal.ThrowExceptionForHR(mediaObject.GetInputSizeInfo(inputStreamIndex, out var size, out var maxLookahead, out var alignment));
			return new MediaObjectSizeInfo(size, maxLookahead, alignment);
		}

		public MediaObjectSizeInfo GetOutputSizeInfo(int outputStreamIndex)
		{
			Marshal.ThrowExceptionForHR(mediaObject.GetOutputSizeInfo(outputStreamIndex, out var size, out var alignment));
			return new MediaObjectSizeInfo(size, 0, alignment);
		}

		public void ProcessInput(int inputStreamIndex, IMediaBuffer mediaBuffer, DmoInputDataBufferFlags flags, long timestamp, long duration)
		{
			Marshal.ThrowExceptionForHR(mediaObject.ProcessInput(inputStreamIndex, mediaBuffer, flags, timestamp, duration));
		}

		public void ProcessOutput(DmoProcessOutputFlags flags, int outputBufferCount, DmoOutputDataBuffer[] outputBuffers)
		{
			Marshal.ThrowExceptionForHR(mediaObject.ProcessOutput(flags, outputBufferCount, outputBuffers, out var _));
		}

		public void AllocateStreamingResources()
		{
			Marshal.ThrowExceptionForHR(mediaObject.AllocateStreamingResources());
		}

		public void FreeStreamingResources()
		{
			Marshal.ThrowExceptionForHR(mediaObject.FreeStreamingResources());
		}

		public long GetInputMaxLatency(int inputStreamIndex)
		{
			Marshal.ThrowExceptionForHR(mediaObject.GetInputMaxLatency(inputStreamIndex, out var referenceTimeMaxLatency));
			return referenceTimeMaxLatency;
		}

		public void Flush()
		{
			Marshal.ThrowExceptionForHR(mediaObject.Flush());
		}

		public void Discontinuity(int inputStreamIndex)
		{
			Marshal.ThrowExceptionForHR(mediaObject.Discontinuity(inputStreamIndex));
		}

		public bool IsAcceptingData(int inputStreamIndex)
		{
			Marshal.ThrowExceptionForHR(mediaObject.GetInputStatus(inputStreamIndex, out var flags));
			return (flags & DmoInputStatusFlags.DMO_INPUT_STATUSF_ACCEPT_DATA) == DmoInputStatusFlags.DMO_INPUT_STATUSF_ACCEPT_DATA;
		}

		public void Dispose()
		{
			if (mediaObject != null)
			{
				Marshal.ReleaseComObject(mediaObject);
				mediaObject = null;
			}
		}
	}
	public class MediaObjectInPlace : IDisposable
	{
		private IMediaObjectInPlace mediaObjectInPlace;

		internal MediaObjectInPlace(IMediaObjectInPlace mediaObjectInPlace)
		{
			this.mediaObjectInPlace = mediaObjectInPlace;
		}

		public DmoInPlaceProcessReturn Process(int size, int offset, byte[] data, long timeStart, DmoInPlaceProcessFlags inPlaceFlag)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			Marshal.Copy(data, offset, intPtr, size);
			int num = mediaObjectInPlace.Process(size, intPtr, timeStart, inPlaceFlag);
			Marshal.ThrowExceptionForHR(num);
			Marshal.Copy(intPtr, data, offset, size);
			Marshal.FreeHGlobal(intPtr);
			return (DmoInPlaceProcessReturn)num;
		}

		public MediaObjectInPlace Clone()
		{
			Marshal.ThrowExceptionForHR(this.mediaObjectInPlace.Clone(out var mediaObjectInPlace));
			return new MediaObjectInPlace(mediaObjectInPlace);
		}

		public long GetLatency()
		{
			Marshal.ThrowExceptionForHR(mediaObjectInPlace.GetLatency(out var latencyTime));
			return latencyTime;
		}

		public MediaObject GetMediaObject()
		{
			return new MediaObject((IMediaObject)mediaObjectInPlace);
		}

		public void Dispose()
		{
			if (mediaObjectInPlace != null)
			{
				Marshal.ReleaseComObject(mediaObjectInPlace);
				mediaObjectInPlace = null;
			}
		}
	}
	public class MediaObjectSizeInfo
	{
		public int Size { get; private set; }

		public int MaxLookahead { get; }

		public int Alignment { get; }

		public MediaObjectSizeInfo(int size, int maxLookahead, int alignment)
		{
			Size = size;
			MaxLookahead = maxLookahead;
			Alignment = alignment;
		}

		public override string ToString()
		{
			return $"Size: {Size}, Alignment {Alignment}, MaxLookahead {MaxLookahead}";
		}
	}
	internal struct MediaParamInfo
	{
		public MediaParamType mpType;

		public MediaParamCurveType mopCaps;

		public float mpdMinValue;

		public float mpdMaxValue;

		public float mpdNeutralValue;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string szUnitText;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string szLabel;
	}
	internal enum MediaParamType
	{
		Int,
		Float,
		Bool,
		Enum,
		Max
	}
	[Flags]
	internal enum MediaParamCurveType
	{
		MP_CURVE_JUMP = 1,
		MP_CURVE_LINEAR = 2,
		MP_CURVE_SQUARE = 4,
		MP_CURVE_INVSQUARE = 8,
		MP_CURVE_SINE = 0x10
	}
	internal static class MediaTypes
	{
		public static readonly Guid MEDIATYPE_AnalogAudio = new Guid("0482DEE1-7817-11cf-8a03-00aa006ecb65");

		public static readonly Guid MEDIATYPE_AnalogVideo = new Guid("0482DDE1-7817-11cf-8A03-00AA006ECB65");

		public static readonly Guid MEDIATYPE_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIATYPE_AUXLine21Data = new Guid("670AEA80-3A82-11d0-B79B-00AA003767A7");

		public static readonly Guid MEDIATYPE_File = new Guid("656c6966-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIATYPE_Interleaved = new Guid("73766169-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIATYPE_Midi = new Guid("7364696D-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIATYPE_ScriptCommand = new Guid("73636d64-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIATYPE_Stream = new Guid("e436eb83-524f-11ce-9f53-0020af0ba770");

		public static readonly Guid MEDIATYPE_Text = new Guid("73747874-0000-0010-8000-00AA00389B71");

		public static readonly Guid MEDIATYPE_Timecode = new Guid("0482DEE3-7817-11cf-8a03-00aa006ecb65");

		public static readonly Guid MEDIATYPE_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");

		public static readonly Guid[] MajorTypes = new Guid[12]
		{
			MEDIATYPE_AnalogAudio, MEDIATYPE_AnalogVideo, MEDIATYPE_Audio, MEDIATYPE_AUXLine21Data, MEDIATYPE_File, MEDIATYPE_Interleaved, MEDIATYPE_Midi, MEDIATYPE_ScriptCommand, MEDIATYPE_Stream, MEDIATYPE_Text,
			MEDIATYPE_Timecode, MEDIATYPE_Video
		};

		public static readonly string[] MajorTypeNames = new string[12]
		{
			"Analog Audio", "Analog Video", "Audio", "AUXLine21Data", "File", "Interleaved", "Midi", "ScriptCommand", "Stream", "Text",
			"Timecode", "Video"
		};

		public static string GetMediaTypeName(Guid majorType)
		{
			for (int i = 0; i < MajorTypes.Length; i++)
			{
				if (majorType == MajorTypes[i])
				{
					return MajorTypeNames[i];
				}
			}
			throw new ArgumentException("Major Type not found");
		}
	}
	[Flags]
	internal enum OutputStreamInfoFlags
	{
		DMO_OUTPUT_STREAMF_WHOLE_SAMPLES = 1,
		DMO_OUTPUT_STREAMF_SINGLE_SAMPLE_PER_BUFFER = 2,
		DMO_OUTPUT_STREAMF_FIXED_SAMPLE_SIZE = 4,
		DMO_OUTPUT_STREAMF_DISCARDABLE = 8,
		DMO_OUTPUT_STREAMF_OPTIONAL = 0x10
	}
	[ComImport]
	[Guid("f447b69e-1884-4a7e-8055-346f74d6edb3")]
	internal class ResamplerMediaComObject
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ResamplerMediaComObject();
	}
	public class DmoResampler : IDisposable
	{
		private MediaObject mediaObject;

		private IPropertyStore propertyStoreInterface;

		private IWMResamplerProps resamplerPropsInterface;

		private ResamplerMediaComObject mediaComObject;

		public MediaObject MediaObject => mediaObject;

		public DmoResampler()
		{
			mediaComObject = new ResamplerMediaComObject();
			mediaObject = new MediaObject((IMediaObject)mediaComObject);
			propertyStoreInterface = (IPropertyStore)mediaComObject;
			resamplerPropsInterface = (IWMResamplerProps)mediaComObject;
		}

		public void Dispose()
		{
			if (propertyStoreInterface != null)
			{
				Marshal.ReleaseComObject(propertyStoreInterface);
				propertyStoreInterface = null;
			}
			if (resamplerPropsInterface != null)
			{
				Marshal.ReleaseComObject(resamplerPropsInterface);
				resamplerPropsInterface = null;
			}
			if (mediaObject != null)
			{
				mediaObject.Dispose();
				mediaObject = null;
			}
			if (mediaComObject != null)
			{
				Marshal.ReleaseComObject(mediaComObject);
				mediaComObject = null;
			}
		}
	}
	[ComImport]
	[Guid("bbeea841-0a63-4f52-a7ab-a9b3a84ed38a")]
	internal class WindowsMediaMp3DecoderComObject
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern WindowsMediaMp3DecoderComObject();
	}
	public class WindowsMediaMp3Decoder : IDisposable
	{
		private MediaObject mediaObject;

		private IPropertyStore propertyStoreInterface;

		private WindowsMediaMp3DecoderComObject mediaComObject;

		public MediaObject MediaObject => mediaObject;

		public WindowsMediaMp3Decoder()
		{
			mediaComObject = new WindowsMediaMp3DecoderComObject();
			mediaObject = new MediaObject((IMediaObject)mediaComObject);
			propertyStoreInterface = (IPropertyStore)mediaComObject;
		}

		public void Dispose()
		{
			if (propertyStoreInterface != null)
			{
				Marshal.ReleaseComObject(propertyStoreInterface);
				propertyStoreInterface = null;
			}
			if (mediaObject != null)
			{
				mediaObject.Dispose();
				mediaObject = null;
			}
			if (mediaComObject != null)
			{
				Marshal.ReleaseComObject(mediaComObject);
				mediaComObject = null;
			}
		}
	}
}
namespace NAudio.Dmo.Effect
{
	public enum ChorusPhase
	{
		Neg180,
		Neg90,
		Zero,
		Pos90,
		Pos180
	}
	public enum ChorusWaveForm
	{
		Triangle,
		Sin
	}
	internal struct DsFxChorus
	{
		public float WetDryMix;

		public float Depth;

		public float FeedBack;

		public float Frequency;

		public ChorusWaveForm WaveForm;

		public float Delay;

		public ChorusPhase Phase;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("880842e3-145f-43e6-a934-a71806e50547")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXChorus
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxChorus param);

		[PreserveSig]
		int GetAllParameters(out DsFxChorus param);
	}
	public class DmoChorus : IDmoEffector<DmoChorus.Params>, IDisposable
	{
		public struct Params
		{
			public const float WetDryMixMin = 0f;

			public const float WetDryMixMax = 100f;

			public const float WetDrtMixDefault = 50f;

			public const float DepthMin = 0f;

			public const float DepthMax = 100f;

			public const float DepthDefault = 10f;

			public const float FeedBackMin = -99f;

			public const float FeedBackMax = 99f;

			public const float FeedBaclDefault = 25f;

			public const float FrequencyMin = 0f;

			public const float FrequencyMax = 10f;

			public const float FrequencyDefault = 1.1f;

			public const ChorusWaveForm WaveFormDefault = ChorusWaveForm.Sin;

			public const float DelayMin = 0f;

			public const float DelayMax = 20f;

			public const float DelayDefault = 16f;

			public const ChorusPhase PhaseDefault = ChorusPhase.Pos90;

			private readonly IDirectSoundFXChorus fxChorus;

			public float WetDryMix
			{
				get
				{
					return GetAllParameters().WetDryMix;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					allParameters.WetDryMix = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float Depth
			{
				get
				{
					return GetAllParameters().Depth;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					allParameters.Depth = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float FeedBack
			{
				get
				{
					return GetAllParameters().FeedBack;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					allParameters.FeedBack = Math.Max(Math.Min(99f, value), -99f);
					SetAllParameters(allParameters);
				}
			}

			public float Frequency
			{
				get
				{
					return GetAllParameters().Frequency;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					allParameters.Frequency = Math.Max(Math.Min(10f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public ChorusWaveForm WaveForm
			{
				get
				{
					return GetAllParameters().WaveForm;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					if (Enum.IsDefined(typeof(ChorusWaveForm), value))
					{
						allParameters.WaveForm = value;
					}
					SetAllParameters(allParameters);
				}
			}

			public float Delay
			{
				get
				{
					return GetAllParameters().Delay;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					allParameters.Delay = Math.Max(Math.Min(20f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public ChorusPhase Phase
			{
				get
				{
					return GetAllParameters().Phase;
				}
				set
				{
					DsFxChorus allParameters = GetAllParameters();
					if (Enum.IsDefined(typeof(ChorusPhase), value))
					{
						allParameters.Phase = value;
					}
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXChorus dsFxObject)
			{
				fxChorus = dsFxObject;
			}

			private void SetAllParameters(DsFxChorus param)
			{
				Marshal.ThrowExceptionForHR(fxChorus.SetAllParameters(ref param));
			}

			private DsFxChorus GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxChorus.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoChorus()
		{
			Guid guidChorus = new Guid("EFE6629C-81F7-4281-BD91-C9D604A95AF6");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidChorus));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXChorus)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxCompressor
	{
		public float Gain;

		public float Attack;

		public float Release;

		public float Threshold;

		public float Ratio;

		public float PreDelay;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("4bbd1154-62f6-4e2c-a15c-d3b6c417f7a0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXCompressor
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxCompressor param);

		[PreserveSig]
		int GetAllParameters(out DsFxCompressor param);
	}
	public class DmoCompressor : IDmoEffector<DmoCompressor.Params>, IDisposable
	{
		public struct Params
		{
			public const float GainMin = -60f;

			public const float GainMax = 60f;

			public const float GainDefault = 0f;

			public const float AttackMin = 0.01f;

			public const float AttackMax = 500f;

			public const float AttackDefault = 10f;

			public const float ReleaseMin = 50f;

			public const float ReleaseMax = 3000f;

			public const float ReleaseDefault = 200f;

			public const float ThresholdMin = -60f;

			public const float ThresholdMax = 0f;

			public const float TjresholdDefault = -20f;

			public const float RatioMin = 1f;

			public const float RatioMax = 100f;

			public const float RatioDefault = 3f;

			public const float PreDelayMin = 0f;

			public const float PreDelayMax = 4f;

			public const float PreDelayDefault = 4f;

			private readonly IDirectSoundFXCompressor fxCompressor;

			public float Gain
			{
				get
				{
					return GetAllParameters().Gain;
				}
				set
				{
					DsFxCompressor allParameters = GetAllParameters();
					allParameters.Gain = Math.Max(Math.Min(60f, value), -60f);
					SetAllParameters(allParameters);
				}
			}

			public float Attack
			{
				get
				{
					return GetAllParameters().Attack;
				}
				set
				{
					DsFxCompressor allParameters = GetAllParameters();
					allParameters.Attack = Math.Max(Math.Min(500f, value), 0.01f);
					SetAllParameters(allParameters);
				}
			}

			public float Release
			{
				get
				{
					return GetAllParameters().Release;
				}
				set
				{
					DsFxCompressor allParameters = GetAllParameters();
					allParameters.Release = Math.Max(Math.Min(3000f, value), 50f);
					SetAllParameters(allParameters);
				}
			}

			public float Threshold
			{
				get
				{
					return GetAllParameters().Threshold;
				}
				set
				{
					DsFxCompressor allParameters = GetAllParameters();
					allParameters.Threshold = Math.Max(Math.Min(0f, value), -60f);
					SetAllParameters(allParameters);
				}
			}

			public float Ratio
			{
				get
				{
					return GetAllParameters().Ratio;
				}
				set
				{
					DsFxCompressor allParameters = GetAllParameters();
					allParameters.Ratio = Math.Max(Math.Min(100f, value), 1f);
					SetAllParameters(allParameters);
				}
			}

			public float PreDelay
			{
				get
				{
					return GetAllParameters().PreDelay;
				}
				set
				{
					DsFxCompressor allParameters = GetAllParameters();
					allParameters.PreDelay = Math.Max(Math.Min(4f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXCompressor dsFxObject)
			{
				fxCompressor = dsFxObject;
			}

			private void SetAllParameters(DsFxCompressor param)
			{
				Marshal.ThrowExceptionForHR(fxCompressor.SetAllParameters(ref param));
			}

			private DsFxCompressor GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxCompressor.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoCompressor()
		{
			Guid guidChorus = new Guid("EF011F79-4000-406D-87AF-BFFB3FC39D57");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidChorus));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXCompressor)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxDistortion
	{
		public float Gain;

		public float Edge;

		public float PostEqCenterFrequency;

		public float PostEqBandWidth;

		public float PreLowPassCutoff;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("8ecf4326-455f-4d8b-bda9-8d5d3e9e3e0b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXDistortion
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxDistortion param);

		[PreserveSig]
		int GetAllParameters(out DsFxDistortion param);
	}
	public class DmoDistortion : IDmoEffector<DmoDistortion.Params>, IDisposable
	{
		public struct Params
		{
			public const float GainMin = -60f;

			public const float GainMax = 0f;

			public const float GainDefault = -18f;

			public const float EdgeMin = 0f;

			public const float EdgeMax = 100f;

			public const float EdgeDefault = 15f;

			public const float PostEqCenterFrequencyMin = 100f;

			public const float PostEqCenterFrequencyMax = 8000f;

			public const float PostEqCenterFrequencyDefault = 2400f;

			public const float PostEqBandWidthMin = 100f;

			public const float PostEqBandWidthMax = 8000f;

			public const float PostEqBandWidthDefault = 2400f;

			public const float PreLowPassCutoffMin = 100f;

			public const float PreLowPassCutoffMax = 8000f;

			public const float PreLowPassCutoffDefault = 8000f;

			private readonly IDirectSoundFXDistortion fxDistortion;

			public float Gain
			{
				get
				{
					return GetAllParameters().Gain;
				}
				set
				{
					DsFxDistortion allParameters = GetAllParameters();
					allParameters.Gain = Math.Max(Math.Min(0f, value), -60f);
					SetAllParameters(allParameters);
				}
			}

			public float Edge
			{
				get
				{
					return GetAllParameters().Edge;
				}
				set
				{
					DsFxDistortion allParameters = GetAllParameters();
					allParameters.Edge = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float PostEqCenterFrequency
			{
				get
				{
					return GetAllParameters().PostEqCenterFrequency;
				}
				set
				{
					DsFxDistortion allParameters = GetAllParameters();
					allParameters.PostEqCenterFrequency = Math.Max(Math.Min(8000f, value), 100f);
					SetAllParameters(allParameters);
				}
			}

			public float PostEqBandWidth
			{
				get
				{
					return GetAllParameters().PostEqBandWidth;
				}
				set
				{
					DsFxDistortion allParameters = GetAllParameters();
					allParameters.PostEqBandWidth = Math.Max(Math.Min(8000f, value), 100f);
					SetAllParameters(allParameters);
				}
			}

			public float PreLowPassCutoff
			{
				get
				{
					return GetAllParameters().PreLowPassCutoff;
				}
				set
				{
					DsFxDistortion allParameters = GetAllParameters();
					allParameters.PreLowPassCutoff = Math.Max(Math.Min(8000f, value), 100f);
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXDistortion dsFxObject)
			{
				fxDistortion = dsFxObject;
			}

			private void SetAllParameters(DsFxDistortion param)
			{
				Marshal.ThrowExceptionForHR(fxDistortion.SetAllParameters(ref param));
			}

			private DsFxDistortion GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxDistortion.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoDistortion()
		{
			Guid guidDistortion = new Guid("EF114C90-CD1D-484E-96E5-09CFAF912A21");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidDistortion));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXDistortion)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxEcho
	{
		public float WetDryMix;

		public float FeedBack;

		public float LeftDelay;

		public float RightDelay;

		public EchoPanDelay PanDelay;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("8bd28edf-50db-4e92-a2bd-445488d1ed42")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXEcho
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxEcho param);

		[PreserveSig]
		int GetAllParameters(out DsFxEcho param);
	}
	public class DmoEcho : IDmoEffector<DmoEcho.Params>, IDisposable
	{
		public struct Params
		{
			public const float WetDryMixMin = 0f;

			public const float WetDryMixMax = 100f;

			public const float WetDeyMixDefault = 50f;

			public const float FeedBackMin = 0f;

			public const float FeedBackMax = 100f;

			public const float FeedBackDefault = 50f;

			public const float LeftDelayMin = 1f;

			public const float LeftDelayMax = 2000f;

			public const float LeftDelayDefault = 500f;

			public const float RightDelayMin = 1f;

			public const float RightDelayMax = 2000f;

			public const float RightDelayDefault = 500f;

			public const EchoPanDelay PanDelayDefault = EchoPanDelay.Off;

			private readonly IDirectSoundFXEcho fxEcho;

			public float WetDryMix
			{
				get
				{
					return GetAllParameters().WetDryMix;
				}
				set
				{
					DsFxEcho allParameters = GetAllParameters();
					allParameters.WetDryMix = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float FeedBack
			{
				get
				{
					return GetAllParameters().FeedBack;
				}
				set
				{
					DsFxEcho allParameters = GetAllParameters();
					allParameters.FeedBack = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float LeftDelay
			{
				get
				{
					return GetAllParameters().LeftDelay;
				}
				set
				{
					DsFxEcho allParameters = GetAllParameters();
					allParameters.LeftDelay = Math.Max(Math.Min(2000f, value), 1f);
					SetAllParameters(allParameters);
				}
			}

			public float RightDelay
			{
				get
				{
					return GetAllParameters().RightDelay;
				}
				set
				{
					DsFxEcho allParameters = GetAllParameters();
					allParameters.RightDelay = Math.Max(Math.Min(2000f, value), 1f);
					SetAllParameters(allParameters);
				}
			}

			public EchoPanDelay PanDelay
			{
				get
				{
					return GetAllParameters().PanDelay;
				}
				set
				{
					DsFxEcho allParameters = GetAllParameters();
					if (Enum.IsDefined(typeof(EchoPanDelay), value))
					{
						allParameters.PanDelay = value;
					}
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXEcho dsFxObject)
			{
				fxEcho = dsFxObject;
			}

			private void SetAllParameters(DsFxEcho param)
			{
				Marshal.ThrowExceptionForHR(fxEcho.SetAllParameters(ref param));
			}

			private DsFxEcho GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxEcho.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoEcho()
		{
			Guid guidEcho = new Guid("EF3E932C-D40B-4F51-8CCF-3F98F1B29D5D");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidEcho));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXEcho)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxFlanger
	{
		public float WetDryMix;

		public float Depth;

		public float FeedBack;

		public float Frequency;

		public FlangerWaveForm WaveForm;

		public float Delay;

		public FlangerPhase Phase;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("903e9878-2c92-4072-9b2c-ea68f5396783")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXFlanger
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxFlanger param);

		[PreserveSig]
		int GetAllParameters(out DsFxFlanger param);
	}
	public class DmoFlanger : IDmoEffector<DmoFlanger.Params>, IDisposable
	{
		public struct Params
		{
			public const float WetDryMixMin = 0f;

			public const float WetDryMixMax = 100f;

			public const float WetDrtMixDefault = 50f;

			public const float DepthMin = 0f;

			public const float DepthMax = 100f;

			public const float DepthDefault = 100f;

			public const float FeedBackMin = -99f;

			public const float FeedBackMax = 99f;

			public const float FeedBaclDefault = -50f;

			public const float FrequencyMin = 0f;

			public const float FrequencyMax = 10f;

			public const float FrequencyDefault = 0.25f;

			public const FlangerWaveForm WaveFormDefault = FlangerWaveForm.Sin;

			public const float DelayMin = 0f;

			public const float DelayMax = 4f;

			public const float DelayDefault = 2f;

			public const FlangerPhase PhaseDefault = FlangerPhase.Zero;

			private readonly IDirectSoundFXFlanger fxFlanger;

			public float WetDryMix
			{
				get
				{
					return GetAllParameters().WetDryMix;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					allParameters.WetDryMix = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float Depth
			{
				get
				{
					return GetAllParameters().Depth;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					allParameters.Depth = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float FeedBack
			{
				get
				{
					return GetAllParameters().FeedBack;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					allParameters.FeedBack = Math.Max(Math.Min(99f, value), -99f);
					SetAllParameters(allParameters);
				}
			}

			public float Frequency
			{
				get
				{
					return GetAllParameters().Frequency;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					allParameters.Frequency = Math.Max(Math.Min(10f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public FlangerWaveForm WaveForm
			{
				get
				{
					return GetAllParameters().WaveForm;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					if (Enum.IsDefined(typeof(FlangerWaveForm), value))
					{
						allParameters.WaveForm = value;
					}
					SetAllParameters(allParameters);
				}
			}

			public float Delay
			{
				get
				{
					return GetAllParameters().Delay;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					allParameters.Delay = Math.Max(Math.Min(4f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public FlangerPhase Phase
			{
				get
				{
					return GetAllParameters().Phase;
				}
				set
				{
					DsFxFlanger allParameters = GetAllParameters();
					if (Enum.IsDefined(typeof(FlangerPhase), value))
					{
						allParameters.Phase = value;
					}
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXFlanger dsFxObject)
			{
				fxFlanger = dsFxObject;
			}

			private void SetAllParameters(DsFxFlanger param)
			{
				Marshal.ThrowExceptionForHR(fxFlanger.SetAllParameters(ref param));
			}

			private DsFxFlanger GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxFlanger.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoFlanger()
		{
			Guid guidFlanger = new Guid("EFCA3D92-DFD8-4672-A603-7420894BAD98");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidFlanger));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXFlanger)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxGargle
	{
		public uint RateHz;

		public GargleWaveShape WaveShape;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("d616f352-d622-11ce-aac5-0020af0b99a3")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXGargle
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxGargle param);

		[PreserveSig]
		int GetAllParameters(out DsFxGargle param);
	}
	public class DmoGargle : IDmoEffector<DmoGargle.Params>, IDisposable
	{
		public struct Params
		{
			public const uint RateHzMin = 1u;

			public const uint RateHzMax = 1000u;

			public const uint RateHzDefault = 20u;

			public const GargleWaveShape WaveShapeDefault = GargleWaveShape.Triangle;

			private readonly IDirectSoundFXGargle fxGargle;

			public uint RateHz
			{
				get
				{
					return GetAllParameters().RateHz;
				}
				set
				{
					DsFxGargle allParameters = GetAllParameters();
					allParameters.RateHz = Math.Max(Math.Min(1000u, value), 1u);
					SetAllParameters(allParameters);
				}
			}

			public GargleWaveShape WaveShape
			{
				get
				{
					return GetAllParameters().WaveShape;
				}
				set
				{
					DsFxGargle allParameters = GetAllParameters();
					if (Enum.IsDefined(typeof(GargleWaveShape), value))
					{
						allParameters.WaveShape = value;
					}
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXGargle dsFxObject)
			{
				fxGargle = dsFxObject;
			}

			private void SetAllParameters(DsFxGargle param)
			{
				Marshal.ThrowExceptionForHR(fxGargle.SetAllParameters(ref param));
			}

			private DsFxGargle GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxGargle.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoGargle()
		{
			Guid guidGargle = new Guid("DAFD8210-5711-4B91-9FE3-F75B7AE279BF");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidGargle));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXGargle)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxI3Dl2Reverb
	{
		public int Room;

		public int RoomHf;

		public float RoomRollOffFactor;

		public float DecayTime;

		public float DecayHfRatio;

		public int Reflections;

		public float ReflectionsDelay;

		public int Reverb;

		public float ReverbDelay;

		public float Diffusion;

		public float Density;

		public float HfReference;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("4b166a6a-0d66-43f3-80e3-ee6280dee1a4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXI3DL2Reverb
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxI3Dl2Reverb param);

		[PreserveSig]
		int GetAllParameters(out DsFxI3Dl2Reverb param);

		[PreserveSig]
		int SetPreset([In] uint preset);

		[PreserveSig]
		int GetPreset(out uint preset);

		[PreserveSig]
		int SetQuality([In] int quality);

		[PreserveSig]
		int GetQuality(out int quality);
	}
	public class DmoI3DL2Reverb : IDmoEffector<DmoI3DL2Reverb.Params>, IDisposable
	{
		public struct Params
		{
			public const int RoomMin = -10000;

			public const int RoomMax = 0;

			public const int RoomDefault = -1000;

			public const int RoomHfMin = -10000;

			public const int RoomHfMax = 0;

			public const int RoomHfDefault = -100;

			public const float RoomRollOffFactorMin = 0f;

			public const float RoomRollOffFactorMax = 10f;

			public const float RoomRollOffFactorDefault = 0f;

			public const float DecayTimeMin = 0.1f;

			public const float DecayTimeMax = 20f;

			public const float DecayTimeDefault = 1.49f;

			public const float DecayHfRatioMin = 0.1f;

			public const float DecayHfRatioMax = 2f;

			public const float DecayHfRatioDefault = 0.83f;

			public const int ReflectionsMin = -10000;

			public const int ReflectionsMax = 1000;

			public const int ReflectionsDefault = -2602;

			public const float ReflectionsDelayMin = 0f;

			public const float ReflectionsDelayMax = 0.3f;

			public const float ReflectionsDelayDefault = 0.007f;

			public const int ReverbMin = -10000;

			public const int ReverbMax = 2000;

			public const int ReverbDefault = 200;

			public const float ReverbDelayMin = 0f;

			public const float ReverbDelayMax = 0.1f;

			public const float ReverbDelayDefault = 0.011f;

			public const float DiffusionMin = 0f;

			public const float DiffusionMax = 100f;

			public const float DiffusionDefault = 100f;

			public const float DensityMin = 0f;

			public const float DensityMax = 100f;

			public const float DensityDefault = 100f;

			public const float HfReferenceMin = 20f;

			public const float HfReferenceMax = 20000f;

			public const float HfReferenceDefault = 5000f;

			public const int QualityMin = 0;

			public const int QualityMax = 3;

			public const int QualityDefault = 2;

			private readonly IDirectSoundFXI3DL2Reverb fxI3Dl2Reverb;

			public int Room
			{
				get
				{
					return GetAllParameters().Room;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.Room = Math.Max(Math.Min(0, value), -10000);
					SetAllParameters(allParameters);
				}
			}

			public int RoomHf
			{
				get
				{
					return GetAllParameters().RoomHf;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.RoomHf = Math.Max(Math.Min(0, value), -10000);
					SetAllParameters(allParameters);
				}
			}

			public float RoomRollOffFactor
			{
				get
				{
					return GetAllParameters().RoomRollOffFactor;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.RoomRollOffFactor = Math.Max(Math.Min(10f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float DecayTime
			{
				get
				{
					return GetAllParameters().DecayTime;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.DecayTime = Math.Max(Math.Min(20f, value), 0.1f);
					SetAllParameters(allParameters);
				}
			}

			public float DecayHfRatio
			{
				get
				{
					return GetAllParameters().DecayHfRatio;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.DecayHfRatio = Math.Max(Math.Min(2f, value), 0.1f);
					SetAllParameters(allParameters);
				}
			}

			public int Reflections
			{
				get
				{
					return GetAllParameters().Reflections;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.Reflections = Math.Max(Math.Min(1000, value), -10000);
					SetAllParameters(allParameters);
				}
			}

			public float ReflectionsDelay
			{
				get
				{
					return GetAllParameters().ReflectionsDelay;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.ReflectionsDelay = Math.Max(Math.Min(0.3f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public int Reverb
			{
				get
				{
					return GetAllParameters().Reverb;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.Reverb = Math.Max(Math.Min(2000, value), -10000);
					SetAllParameters(allParameters);
				}
			}

			public float ReverbDelay
			{
				get
				{
					return GetAllParameters().ReverbDelay;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.ReverbDelay = Math.Max(Math.Min(0.1f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float Diffusion
			{
				get
				{
					return GetAllParameters().Diffusion;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.Diffusion = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float Density
			{
				get
				{
					return GetAllParameters().Density;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.Density = Math.Max(Math.Min(100f, value), 0f);
					SetAllParameters(allParameters);
				}
			}

			public float HfReference
			{
				get
				{
					return GetAllParameters().HfReference;
				}
				set
				{
					DsFxI3Dl2Reverb allParameters = GetAllParameters();
					allParameters.HfReference = Math.Max(Math.Min(20000f, value), 20f);
					SetAllParameters(allParameters);
				}
			}

			public int Quality
			{
				get
				{
					Marshal.ThrowExceptionForHR(fxI3Dl2Reverb.GetQuality(out var quality));
					return quality;
				}
				set
				{
					Marshal.ThrowExceptionForHR(fxI3Dl2Reverb.SetQuality(value));
				}
			}

			internal Params(IDirectSoundFXI3DL2Reverb dsFxObject)
			{
				fxI3Dl2Reverb = dsFxObject;
			}

			public void SetPreset(I3DL2EnvironmentPreset preset)
			{
				Marshal.ThrowExceptionForHR(fxI3Dl2Reverb.SetPreset((uint)preset));
			}

			public I3DL2EnvironmentPreset GetPreset()
			{
				Marshal.ThrowExceptionForHR(fxI3Dl2Reverb.GetPreset(out var preset));
				return (I3DL2EnvironmentPreset)preset;
			}

			private void SetAllParameters(DsFxI3Dl2Reverb param)
			{
				Marshal.ThrowExceptionForHR(fxI3Dl2Reverb.SetAllParameters(ref param));
			}

			private DsFxI3Dl2Reverb GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxI3Dl2Reverb.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoI3DL2Reverb()
		{
			Guid guidi3Dl2Reverb = new Guid("EF985E71-D5C7-42D4-BA4D-2D073E2E96F4");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidi3Dl2Reverb));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXI3DL2Reverb)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxParamEq
	{
		public float Center;

		public float BandWidth;

		public float Gain;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("c03ca9fe-fe90-4204-8078-82334cd177da")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFxParamEq
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxParamEq param);

		[PreserveSig]
		int GetAllParameters(out DsFxParamEq param);
	}
	public class DmoParamEq : IDmoEffector<DmoParamEq.Params>, IDisposable
	{
		public struct Params
		{
			public const float CenterMin = 80f;

			public const float CenterMax = 16000f;

			public const float CenterDefault = 8000f;

			public const float BandWidthMin = 1f;

			public const float BandWidthMax = 36f;

			public const float BandWidthDefault = 12f;

			public const float GainMin = -15f;

			public const float GainMax = 15f;

			public const float GainDefault = 0f;

			private readonly IDirectSoundFxParamEq fxParamEq;

			public float Center
			{
				get
				{
					return GetAllParameters().Center;
				}
				set
				{
					DsFxParamEq allParameters = GetAllParameters();
					allParameters.Center = Math.Max(Math.Min(16000f, value), 80f);
					SetAllParameters(allParameters);
				}
			}

			public float BandWidth
			{
				get
				{
					return GetAllParameters().BandWidth;
				}
				set
				{
					DsFxParamEq allParameters = GetAllParameters();
					allParameters.BandWidth = Math.Max(Math.Min(36f, value), 1f);
					SetAllParameters(allParameters);
				}
			}

			public float Gain
			{
				get
				{
					return GetAllParameters().Gain;
				}
				set
				{
					DsFxParamEq allParameters = GetAllParameters();
					allParameters.Gain = Math.Max(Math.Min(15f, value), -15f);
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFxParamEq dsFxObject)
			{
				fxParamEq = dsFxObject;
			}

			private void SetAllParameters(DsFxParamEq param)
			{
				Marshal.ThrowExceptionForHR(fxParamEq.SetAllParameters(ref param));
			}

			private DsFxParamEq GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxParamEq.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoParamEq()
		{
			Guid guidParamEq = new Guid("120CED89-3BF4-4173-A132-3CB406CF3231");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidParamEq));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFxParamEq)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	internal struct DsFxWavesReverb
	{
		public float InGain;

		public float ReverbMix;

		public float ReverbTime;

		public float HighFreqRtRatio;
	}
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[Guid("46858c3a-0dc6-45e3-b760-d4eef16cb325")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDirectSoundFXWavesReverb
	{
		[PreserveSig]
		int SetAllParameters([In] ref DsFxWavesReverb param);

		[PreserveSig]
		int GetAllParameters(out DsFxWavesReverb param);
	}
	public class DmoWavesReverb : IDmoEffector<DmoWavesReverb.Params>, IDisposable
	{
		public struct Params
		{
			public const float InGainMin = -96f;

			public const float InGainMax = 0f;

			public const float InGainDefault = 0f;

			public const float ReverbMixMin = -96f;

			public const float ReverbMixMax = 0f;

			public const float ReverbMixDefault = 0f;

			public const float ReverbTimeMin = 0.001f;

			public const float ReverbTimeMax = 3000f;

			public const float ReverbTimeDefault = 1000f;

			public const float HighFreqRtRatioMin = 0.001f;

			public const float HighFreqRtRatioMax = 0.999f;

			public const float HighFreqRtRatioDefault = 0.001f;

			private readonly IDirectSoundFXWavesReverb fxWavesReverb;

			public float InGain
			{
				get
				{
					return GetAllParameters().InGain;
				}
				set
				{
					DsFxWavesReverb allParameters = GetAllParameters();
					allParameters.InGain = Math.Max(Math.Min(0f, value), -96f);
					SetAllParameters(allParameters);
				}
			}

			public float ReverbMix
			{
				get
				{
					return GetAllParameters().ReverbMix;
				}
				set
				{
					DsFxWavesReverb allParameters = GetAllParameters();
					allParameters.ReverbMix = Math.Max(Math.Min(0f, value), -96f);
					SetAllParameters(allParameters);
				}
			}

			public float ReverbTime
			{
				get
				{
					return GetAllParameters().ReverbTime;
				}
				set
				{
					DsFxWavesReverb allParameters = GetAllParameters();
					allParameters.ReverbTime = Math.Max(Math.Min(3000f, value), 0.001f);
					SetAllParameters(allParameters);
				}
			}

			public float HighFreqRtRatio
			{
				get
				{
					return GetAllParameters().HighFreqRtRatio;
				}
				set
				{
					DsFxWavesReverb allParameters = GetAllParameters();
					allParameters.HighFreqRtRatio = Math.Max(Math.Min(0.999f, value), 0.001f);
					SetAllParameters(allParameters);
				}
			}

			internal Params(IDirectSoundFXWavesReverb dsFxObject)
			{
				fxWavesReverb = dsFxObject;
			}

			private void SetAllParameters(DsFxWavesReverb param)
			{
				Marshal.ThrowExceptionForHR(fxWavesReverb.SetAllParameters(ref param));
			}

			private DsFxWavesReverb GetAllParameters()
			{
				Marshal.ThrowExceptionForHR(fxWavesReverb.GetAllParameters(out var param));
				return param;
			}
		}

		private readonly MediaObject mediaObject;

		private readonly MediaObjectInPlace mediaObjectInPlace;

		private readonly Params effectParams;

		public MediaObject MediaObject => mediaObject;

		public MediaObjectInPlace MediaObjectInPlace => mediaObjectInPlace;

		public Params EffectParams => effectParams;

		public DmoWavesReverb()
		{
			Guid guidWavesReverb = new Guid("87FC0268-9A55-4360-95AA-004A1D9DE26C");
			DmoDescriptor dmoDescriptor = DmoEnumerator.GetAudioEffectNames().First((DmoDescriptor descriptor) => object.Equals(descriptor.Clsid, guidWavesReverb));
			if (dmoDescriptor != null)
			{
				object obj = Activator.CreateInstance(Type.GetTypeFromCLSID(dmoDescriptor.Clsid));
				mediaObject = new MediaObject((IMediaObject)obj);
				mediaObjectInPlace = new MediaObjectInPlace((IMediaObjectInPlace)obj);
				effectParams = new Params((IDirectSoundFXWavesReverb)obj);
			}
		}

		public void Dispose()
		{
			mediaObjectInPlace?.Dispose();
			mediaObject?.Dispose();
		}
	}
	public enum EchoPanDelay
	{
		Off,
		On
	}
	public enum FlangerPhase
	{
		Neg180,
		Neg90,
		Zero,
		Pos90,
		Pos180
	}
	public enum FlangerWaveForm
	{
		Triangle,
		Sin
	}
	public enum GargleWaveShape : uint
	{
		Triangle,
		Square
	}
	public enum I3DL2EnvironmentPreset
	{
		Default,
		Generic,
		PaddedCell,
		Room,
		Bathroom,
		LivingRoom,
		StoneRoom,
		Auditorium,
		ConcertHall,
		Cave,
		Arena,
		Hangar,
		CarpetedHallway,
		Hallway,
		StoneCorridor,
		Alley,
		Forest,
		City,
		Mountains,
		Quarry,
		Plain,
		ParkingLot,
		SewerPipe,
		UnderWater,
		SmallRoom,
		MediumRoom,
		LargeRoom,
		MediumHall,
		LargeHall,
		Plate
	}
	public interface IDmoEffector<out TParameters> : IDisposable
	{
		MediaObject MediaObject { get; }

		MediaObjectInPlace MediaObjectInPlace { get; }

		TParameters EffectParams { get; }
	}
}
namespace NAudio.CoreAudioApi
{
	public class AudioCaptureClient : IDisposable
	{
		private IAudioCaptureClient audioCaptureClientInterface;

		internal AudioCaptureClient(IAudioCaptureClient audioCaptureClientInterface)
		{
			this.audioCaptureClientInterface = audioCaptureClientInterface;
		}

		public IntPtr GetBuffer(out int numFramesToRead, out AudioClientBufferFlags bufferFlags, out long devicePosition, out long qpcPosition)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out var dataBuffer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
			return dataBuffer;
		}

		public IntPtr GetBuffer(out int numFramesToRead, out AudioClientBufferFlags bufferFlags)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out var dataBuffer, out numFramesToRead, out bufferFlags, out var _, out var _));
			return dataBuffer;
		}

		public int GetNextPacketSize()
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetNextPacketSize(out var numFramesInNextPacket));
			return numFramesInNextPacket;
		}

		public void ReleaseBuffer(int numFramesWritten)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.ReleaseBuffer(numFramesWritten));
		}

		public void Dispose()
		{
			if (audioCaptureClientInterface != null)
			{
				Marshal.ReleaseComObject(audioCaptureClientInterface);
				audioCaptureClientInterface = null;
				GC.SuppressFinalize(this);
			}
		}
	}
	public class AudioClient : IDisposable
	{
		private IAudioClient audioClientInterface;

		private WaveFormat mixFormat;

		private AudioRenderClient audioRenderClient;

		private AudioCaptureClient audioCaptureClient;

		private AudioClockClient audioClockClient;

		private AudioStreamVolume audioStreamVolume;

		private AudioClientShareMode shareMode;

		public WaveFormat MixFormat
		{
			get
			{
				if (mixFormat == null)
				{
					Marshal.ThrowExceptionForHR(audioClientInterface.GetMixFormat(out var deviceFormatPointer));
					WaveFormat waveFormat = WaveFormat.MarshalFromPtr(deviceFormatPointer);
					Marshal.FreeCoTaskMem(deviceFormatPointer);
					mixFormat = waveFormat;
				}
				return mixFormat;
			}
		}

		public int BufferSize
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetBufferSize(out var bufferSize));
				return (int)bufferSize;
			}
		}

		public long StreamLatency => audioClientInterface.GetStreamLatency();

		public int CurrentPadding
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetCurrentPadding(out var currentPadding));
				return currentPadding;
			}
		}

		public long DefaultDevicePeriod
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out var defaultDevicePeriod, out var _));
				return defaultDevicePeriod;
			}
		}

		public long MinimumDevicePeriod
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out var _, out var minimumDevicePeriod));
				return minimumDevicePeriod;
			}
		}

		public AudioStreamVolume AudioStreamVolume
		{
			get
			{
				if (shareMode == AudioClientShareMode.Exclusive)
				{
					throw new InvalidOperationException("AudioStreamVolume is ONLY supported for shared audio streams.");
				}
				if (audioStreamVolume == null)
				{
					Guid interfaceId = new Guid("93014887-242D-4068-8A15-CF5E93B90FE3");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioStreamVolume = new AudioStreamVolume((IAudioStreamVolume)interfacePointer);
				}
				return audioStreamVolume;
			}
		}

		public AudioClockClient AudioClockClient
		{
			get
			{
				if (audioClockClient == null)
				{
					Guid interfaceId = new Guid("CD63314F-3FBA-4a1b-812C-EF96358728E7");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioClockClient = new AudioClockClient((IAudioClock)interfacePointer);
				}
				return audioClockClient;
			}
		}

		public AudioRenderClient AudioRenderClient
		{
			get
			{
				if (audioRenderClient == null)
				{
					Guid interfaceId = new Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioRenderClient = new AudioRenderClient((IAudioRenderClient)interfacePointer);
				}
				return audioRenderClient;
			}
		}

		public AudioCaptureClient AudioCaptureClient
		{
			get
			{
				if (audioCaptureClient == null)
				{
					Guid interfaceId = new Guid("c8adbd64-e71e-48a0-a4de-185c395cd317");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioCaptureClient = new AudioCaptureClient((IAudioCaptureClient)interfacePointer);
				}
				return audioCaptureClient;
			}
		}

		internal AudioClient(IAudioClient audioClientInterface)
		{
			this.audioClientInterface = audioClientInterface;
		}

		public void Initialize(AudioClientShareMode shareMode, AudioClientStreamFlags streamFlags, long bufferDuration, long periodicity, WaveFormat waveFormat, Guid audioSessionGuid)
		{
			this.shareMode = shareMode;
			Marshal.ThrowExceptionForHR(audioClientInterface.Initialize(shareMode, streamFlags, bufferDuration, periodicity, waveFormat, ref audioSessionGuid));
			mixFormat = null;
		}

		public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat)
		{
			WaveFormatExtensible closestMatchFormat;
			return IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);
		}

		private IntPtr GetPointerToPointer()
		{
			return Marshal.AllocHGlobal(MarshalHelpers.SizeOf<IntPtr>());
		}

		public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat, out WaveFormatExtensible closestMatchFormat)
		{
			IntPtr pointerToPointer = GetPointerToPointer();
			closestMatchFormat = null;
			int num = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, pointerToPointer);
			IntPtr intPtr = MarshalHelpers.PtrToStructure<IntPtr>(pointerToPointer);
			if (intPtr != IntPtr.Zero)
			{
				closestMatchFormat = MarshalHelpers.PtrToStructure<WaveFormatExtensible>(intPtr);
				Marshal.FreeCoTaskMem(intPtr);
			}
			Marshal.FreeHGlobal(pointerToPointer);
			switch (num)
			{
			case 0:
				return true;
			case 1:
				return false;
			case -2004287480:
				return false;
			default:
				Marshal.ThrowExceptionForHR(num);
				throw new NotSupportedException("Unknown hresult " + num);
			}
		}

		public void Start()
		{
			audioClientInterface.Start();
		}

		public void Stop()
		{
			audioClientInterface.Stop();
		}

		public void SetEventHandle(IntPtr eventWaitHandle)
		{
			audioClientInterface.SetEventHandle(eventWaitHandle);
		}

		public void Reset()
		{
			audioClientInterface.Reset();
		}

		public void Dispose()
		{
			if (audioClientInterface != null)
			{
				if (audioClockClient != null)
				{
					audioClockClient.Dispose();
					audioClockClient = null;
				}
				if (audioRenderClient != null)
				{
					audioRenderClient.Dispose();
					audioRenderClient = null;
				}
				if (audioCaptureClient != null)
				{
					audioCaptureClient.Dispose();
					audioCaptureClient = null;
				}
				if (audioStreamVolume != null)
				{
					audioStreamVolume.Dispose();
					audioStreamVolume = null;
				}
				Marshal.ReleaseComObject(audioClientInterface);
				audioClientInterface = null;
				GC.SuppressFinalize(this);
			}
		}
	}
	[Flags]
	public enum AudioClientBufferFlags
	{
		None = 0,
		DataDiscontinuity = 1,
		Silent = 2,
		TimestampError = 4
	}
	public struct AudioClientProperties
	{
		public uint cbSize;

		public int bIsOffload;

		public AudioStreamCategory eCategory;

		public AudioClientStreamOptions Options;
	}
	public enum AudioClientShareMode
	{
		Shared,
		Exclusive
	}
	[Flags]
	public enum AudioClientStreamFlags
	{
		None = 0,
		CrossProcess = 0x10000,
		Loopback = 0x20000,
		EventCallback = 0x40000,
		NoPersist = 0x80000
	}
	public enum AudioClientStreamOptions
	{
		None,
		Raw
	}
	public class AudioClockClient : IDisposable
	{
		private IAudioClock audioClockClientInterface;

		public int Characteristics
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClockClientInterface.GetCharacteristics(out var characteristics));
				return (int)characteristics;
			}
		}

		public ulong Frequency
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClockClientInterface.GetFrequency(out var frequency));
				return frequency;
			}
		}

		public ulong AdjustedPosition
		{
			get
			{
				int num = 0;
				ulong position;
				ulong qpcPosition;
				while (!GetPosition(out position, out qpcPosition) && ++num != 5)
				{
				}
				if (Stopwatch.IsHighResolution)
				{
					ulong num2 = ((ulong)((decimal)Stopwatch.GetTimestamp() * 10000000m / (decimal)Stopwatch.Frequency) - qpcPosition) * Frequency / 10000000uL;
					return position + num2;
				}
				return position;
			}
		}

		public bool CanAdjustPosition => Stopwatch.IsHighResolution;

		internal AudioClockClient(IAudioClock audioClockClientInterface)
		{
			this.audioClockClientInterface = audioClockClientInterface;
		}

		public bool GetPosition(out ulong position, out ulong qpcPosition)
		{
			int position2 = audioClockClientInterface.GetPosition(out position, out qpcPosition);
			if (position2 == -1)
			{
				return false;
			}
			Marshal.ThrowExceptionForHR(position2);
			return true;
		}

		public void Dispose()
		{
			if (audioClockClientInterface != null)
			{
				Marshal.ReleaseComObject(audioClockClientInterface);
				audioClockClientInterface = null;
				GC.SuppressFinalize(this);
			}
		}
	}
	public class AudioEndpointVolume : IDisposable
	{
		private readonly IAudioEndpointVolume audioEndPointVolume;

		private AudioEndpointVolumeCallback callBack;

		private Guid notificationGuid = Guid.Empty;

		public Guid NotificationGuid
		{
			get
			{
				return notificationGuid;
			}
			set
			{
				notificationGuid = value;
			}
		}

		public AudioEndpointVolumeVolumeRange VolumeRange { get; }

		public EEndpointHardwareSupport HardwareSupport { get; }

		public AudioEndpointVolumeStepInformation StepInformation { get; }

		public AudioEndpointVolumeChannels Channels { get; }

		public float MasterVolumeLevel
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevel(out var pfLevelDB));
				return pfLevelDB;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevel(value, ref notificationGuid));
			}
		}

		public float MasterVolumeLevelScalar
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevelScalar(out var pfLevel));
				return pfLevel;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevelScalar(value, ref notificationGuid));
			}
		}

		public bool Mute
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMute(out var pbMute));
				return pbMute;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMute(value, ref notificationGuid));
			}
		}

		public event AudioEndpointVolumeNotificationDelegate OnVolumeNotification;

		public void VolumeStepUp()
		{
			Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepUp(ref notificationGuid));
		}

		public void VolumeStepDown()
		{
			Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepDown(ref notificationGuid));
		}

		internal AudioEndpointVolume(IAudioEndpointVolume realEndpointVolume)
		{
			audioEndPointVolume = realEndpointVolume;
			Channels = new AudioEndpointVolumeChannels(audioEndPointVolume);
			StepInformation = new AudioEndpointVolumeStepInformation(audioEndPointVolume);
			Marshal.ThrowExceptionForHR(audioEndPointVolume.QueryHardwareSupport(out var pdwHardwareSupportMask));
			HardwareSupport = (EEndpointHardwareSupport)pdwHardwareSupportMask;
			VolumeRange = new AudioEndpointVolumeVolumeRange(audioEndPointVolume);
			callBack = new AudioEndpointVolumeCallback(this);
			Marshal.ThrowExceptionForHR(audioEndPointVolume.RegisterControlChangeNotify(callBack));
		}

		internal void FireNotification(AudioVolumeNotificationData notificationData)
		{
			this.OnVolumeNotification?.Invoke(notificationData);
		}

		public void Dispose()
		{
			if (callBack != null)
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.UnregisterControlChangeNotify(callBack));
				callBack = null;
			}
			Marshal.ReleaseComObject(audioEndPointVolume);
			GC.SuppressFinalize(this);
		}

		~AudioEndpointVolume()
		{
			Dispose();
		}
	}
	internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
	{
		private readonly AudioEndpointVolume parent;

		internal AudioEndpointVolumeCallback(AudioEndpointVolume parent)
		{
			this.parent = parent;
		}

		public void OnNotify(IntPtr notifyData)
		{
			AudioVolumeNotificationDataStruct audioVolumeNotificationDataStruct = MarshalHelpers.PtrToStructure<AudioVolumeNotificationDataStruct>(notifyData);
			IntPtr intPtr = MarshalHelpers.OffsetOf<AudioVolumeNotificationDataStruct>("ChannelVolume");
			IntPtr pointer = (IntPtr)((long)notifyData + (long)intPtr);
			float[] array = new float[audioVolumeNotificationDataStruct.nChannels];
			for (int i = 0; i < audioVolumeNotificationDataStruct.nChannels; i++)
			{
				array[i] = MarshalHelpers.PtrToStructure<float>(pointer);
			}
			AudioVolumeNotificationData notificationData = new AudioVolumeNotificationData(audioVolumeNotificationDataStruct.guidEventContext, audioVolumeNotificationDataStruct.bMuted, audioVolumeNotificationDataStruct.fMasterVolume, array, audioVolumeNotificationDataStruct.guidEventContext);
			parent.FireNotification(notificationData);
		}
	}
	public class AudioEndpointVolumeChannel
	{
		private readonly uint channel;

		private readonly IAudioEndpointVolume audioEndpointVolume;

		private Guid notificationGuid = Guid.Empty;

		public Guid NotificationGuid
		{
			get
			{
				return notificationGuid;
			}
			set
			{
				notificationGuid = value;
			}
		}

		public float VolumeLevel
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevel(channel, out var pfLevelDB));
				return pfLevelDB;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevel(channel, value, ref notificationGuid));
			}
		}

		public float VolumeLevelScalar
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevelScalar(channel, out var pfLevel));
				return pfLevel;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevelScalar(channel, value, ref notificationGuid));
			}
		}

		internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
		{
			this.channel = (uint)channel;
			audioEndpointVolume = parent;
		}
	}
	public class AudioEndpointVolumeChannels
	{
		private readonly IAudioEndpointVolume audioEndPointVolume;

		private readonly AudioEndpointVolumeChannel[] channels;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetChannelCount(out var pnChannelCount));
				return pnChannelCount;
			}
		}

		public AudioEndpointVolumeChannel this[int index] => channels[index];

		internal AudioEndpointVolumeChannels(IAudioEndpointVolume parent)
		{
			audioEndPointVolume = parent;
			int count = Count;
			channels = new AudioEndpointVolumeChannel[count];
			for (int i = 0; i < count; i++)
			{
				channels[i] = new AudioEndpointVolumeChannel(audioEndPointVolume, i);
			}
		}
	}
	public delegate void AudioEndpointVolumeNotificationDelegate(AudioVolumeNotificationData data);
	public class AudioEndpointVolumeStepInformation
	{
		private readonly uint step;

		private readonly uint stepCount;

		public uint Step => step;

		public uint StepCount => stepCount;

		internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent)
		{
			Marshal.ThrowExceptionForHR(parent.GetVolumeStepInfo(out step, out stepCount));
		}
	}
	public class AudioEndpointVolumeVolumeRange
	{
		private readonly float volumeMinDecibels;

		private readonly float volumeMaxDecibels;

		private readonly float volumeIncrementDecibels;

		public float MinDecibels => volumeMinDecibels;

		public float MaxDecibels => volumeMaxDecibels;

		public float IncrementDecibels => volumeIncrementDecibels;

		internal AudioEndpointVolumeVolumeRange(IAudioEndpointVolume parent)
		{
			Marshal.ThrowExceptionForHR(parent.GetVolumeRange(out volumeMinDecibels, out volumeMaxDecibels, out volumeIncrementDecibels));
		}
	}
	public class AudioMeterInformation
	{
		private readonly IAudioMeterInformation audioMeterInformation;

		public AudioMeterInformationChannels PeakValues { get; }

		public EEndpointHardwareSupport HardwareSupport { get; }

		public float MasterPeakValue
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioMeterInformation.GetPeakValue(out var pfPeak));
				return pfPeak;
			}
		}

		internal AudioMeterInformation(IAudioMeterInformation realInterface)
		{
			audioMeterInformation = realInterface;
			Marshal.ThrowExceptionForHR(audioMeterInformation.QueryHardwareSupport(out var pdwHardwareSupportMask));
			HardwareSupport = (EEndpointHardwareSupport)pdwHardwareSupportMask;
			PeakValues = new AudioMeterInformationChannels(audioMeterInformation);
		}
	}
	public class AudioMeterInformationChannels
	{
		private readonly IAudioMeterInformation audioMeterInformation;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioMeterInformation.GetMeteringChannelCount(out var pnChannelCount));
				return pnChannelCount;
			}
		}

		public float this[int index]
		{
			get
			{
				int count = Count;
				if (index >= count)
				{
					throw new ArgumentOutOfRangeException("index", $"Peak index cannot be greater than number of channels ({count})");
				}
				float[] array = new float[Count];
				GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				Marshal.ThrowExceptionForHR(audioMeterInformation.GetChannelsPeakValues(array.Length, gCHandle.AddrOfPinnedObject()));
				gCHandle.Free();
				return array[index];
			}
		}

		internal AudioMeterInformationChannels(IAudioMeterInformation parent)
		{
			audioMeterInformation = parent;
		}
	}
	public class AudioRenderClient : IDisposable
	{
		private IAudioRenderClient audioRenderClientInterface;

		internal AudioRenderClient(IAudioRenderClient audioRenderClientInterface)
		{
			this.audioRenderClientInterface = audioRenderClientInterface;
		}

		public IntPtr GetBuffer(int numFramesRequested)
		{
			Marshal.ThrowExceptionForHR(audioRenderClientInterface.GetBuffer(numFramesRequested, out var dataBufferPointer));
			return dataBufferPointer;
		}

		public void ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags)
		{
			Marshal.ThrowExceptionForHR(audioRenderClientInterface.ReleaseBuffer(numFramesWritten, bufferFlags));
		}

		public void Dispose()
		{
			if (audioRenderClientInterface != null)
			{
				Marshal.ReleaseComObject(audioRenderClientInterface);
				audioRenderClientInterface = null;
				GC.SuppressFinalize(this);
			}
		}
	}
	public class AudioSessionControl : IDisposable
	{
		private readonly IAudioSessionControl audioSessionControlInterface;

		private readonly IAudioSessionControl2 audioSessionControlInterface2;

		private AudioSessionEventsCallback audioSessionEventCallback;

		public AudioMeterInformation AudioMeterInformation { get; }

		public SimpleAudioVolume SimpleAudioVolume { get; }

		public AudioSessionState State
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetState(out var state));
				return state;
			}
		}

		public string DisplayName
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetDisplayName(out var displayName));
				return displayName;
			}
			set
			{
				if (value != string.Empty)
				{
					Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetDisplayName(value, Guid.Empty));
				}
			}
		}

		public string IconPath
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetIconPath(out var iconPath));
				return iconPath;
			}
			set
			{
				if (value != string.Empty)
				{
					Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetIconPath(value, Guid.Empty));
				}
			}
		}

		public string GetSessionIdentifier
		{
			get
			{
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				Marshal.ThrowExceptionForHR(audioSessionControlInterface2.GetSessionIdentifier(out var retVal));
				return retVal;
			}
		}

		public string GetSessionInstanceIdentifier
		{
			get
			{
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				Marshal.ThrowExceptionForHR(audioSessionControlInterface2.GetSessionInstanceIdentifier(out var retVal));
				return retVal;
			}
		}

		public uint GetProcessID
		{
			get
			{
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				Marshal.ThrowExceptionForHR(audioSessionControlInterface2.GetProcessId(out var retVal));
				return retVal;
			}
		}

		public bool IsSystemSoundsSession
		{
			get
			{
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				return audioSessionControlInterface2.IsSystemSoundsSession() == 0;
			}
		}

		public AudioSessionControl(IAudioSessionControl audioSessionControl)
		{
			audioSessionControlInterface = audioSessionControl;
			audioSessionControlInterface2 = audioSessionControl as IAudioSessionControl2;
			if (audioSessionControlInterface is IAudioMeterInformation realInterface)
			{
				AudioMeterInformation = new AudioMeterInformation(realInterface);
			}
			if (audioSessionControlInterface is ISimpleAudioVolume realSimpleVolume)
			{
				SimpleAudioVolume = new SimpleAudioVolume(realSimpleVolume);
			}
		}

		public void Dispose()
		{
			if (audioSessionEventCallback != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.UnregisterAudioSessionNotification(audioSessionEventCallback));
				audioSessionEventCallback = null;
			}
			GC.SuppressFinalize(this);
		}

		~AudioSessionControl()
		{
			Dispose();
		}

		public Guid GetGroupingParam()
		{
			Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetGroupingParam(out var groupingId));
			return groupingId;
		}

		public void SetGroupingParam(Guid groupingId, Guid context)
		{
			Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetGroupingParam(groupingId, context));
		}

		public void RegisterEventClient(IAudioSessionEventsHandler eventClient)
		{
			audioSessionEventCallback = new AudioSessionEventsCallback(eventClient);
			Marshal.ThrowExceptionForHR(audioSessionControlInterface.RegisterAudioSessionNotification(audioSessionEventCallback));
		}

		public void UnRegisterEventClient(IAudioSessionEventsHandler eventClient)
		{
			if (audioSessionEventCallback != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.UnregisterAudioSessionNotification(audioSessionEventCallback));
				audioSessionEventCallback = null;
			}
		}
	}
	public class AudioSessionEventsCallback : IAudioSessionEvents
	{
		private readonly IAudioSessionEventsHandler audioSessionEventsHandler;

		public AudioSessionEventsCallback(IAudioSessionEventsHandler handler)
		{
			audioSessionEventsHandler = handler;
		}

		public int OnDisplayNameChanged([In][MarshalAs(UnmanagedType.LPWStr)] string displayName, [In] ref Guid eventContext)
		{
			audioSessionEventsHandler.OnDisplayNameChanged(displayName);
			return 0;
		}

		public int OnIconPathChanged([In][MarshalAs(UnmanagedType.LPWStr)] string iconPath, [In] ref Guid eventContext)
		{
			audioSessionEventsHandler.OnIconPathChanged(iconPath);
			return 0;
		}

		public int OnSimpleVolumeChanged([In][MarshalAs(UnmanagedType.R4)] float volume, [In][MarshalAs(UnmanagedType.Bool)] bool isMuted, [In] ref Guid eventContext)
		{
			audioSessionEventsHandler.OnVolumeChanged(volume, isMuted);
			return 0;
		}

		public int OnChannelVolumeChanged([In][MarshalAs(UnmanagedType.U4)] uint channelCount, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes, [In][MarshalAs(UnmanagedType.U4)] uint channelIndex, [In] ref Guid eventContext)
		{
			audioSessionEventsHandler.OnChannelVolumeChanged(channelCount, newVolumes, channelIndex);
			return 0;
		}

		public int OnGroupingParamChanged([In] ref Guid groupingId, [In] ref Guid eventContext)
		{
			audioSessionEventsHandler.OnGroupingParamChanged(ref groupingId);
			return 0;
		}

		public int OnStateChanged([In] AudioSessionState state)
		{
			audioSessionEventsHandler.OnStateChanged(state);
			return 0;
		}

		public int OnSessionDisconnected([In] AudioSessionDisconnectReason disconnectReason)
		{
			audioSessionEventsHandler.OnSessionDisconnected(disconnectReason);
			return 0;
		}
	}
	public class AudioSessionManager
	{
		public delegate void SessionCreatedDelegate(object sender, IAudioSessionControl newSession);

		private readonly IAudioSessionManager audioSessionInterface;

		private readonly IAudioSessionManager2 audioSessionInterface2;

		private AudioSessionNotification audioSessionNotification;

		private SessionCollection sessions;

		private SimpleAudioVolume simpleAudioVolume;

		private AudioSessionControl audioSessionControl;

		public SimpleAudioVolume SimpleAudioVolume
		{
			get
			{
				if (simpleAudioVolume == null)
				{
					audioSessionInterface.GetSimpleAudioVolume(Guid.Empty, 0u, out var audioVolume);
					simpleAudioVolume = new SimpleAudioVolume(audioVolume);
				}
				return simpleAudioVolume;
			}
		}

		public AudioSessionControl AudioSessionControl
		{
			get
			{
				if (audioSessionControl == null)
				{
					audioSessionInterface.GetAudioSessionControl(Guid.Empty, 0u, out var sessionControl);
					audioSessionControl = new AudioSessionControl(sessionControl);
				}
				return audioSessionControl;
			}
		}

		public SessionCollection Sessions => sessions;

		public event SessionCreatedDelegate OnSessionCreated;

		internal AudioSessionManager(IAudioSessionManager audioSessionManager)
		{
			audioSessionInterface = audioSessionManager;
			audioSessionInterface2 = audioSessionManager as IAudioSessionManager2;
			RefreshSessions();
		}

		internal void FireSessionCreated(IAudioSessionControl newSession)
		{
			this.OnSessionCreated?.Invoke(this, newSession);
		}

		public void RefreshSessions()
		{
			UnregisterNotifications();
			if (audioSessionInterface2 != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionInterface2.GetSessionEnumerator(out var sessionEnum));
				sessions = new SessionCollection(sessionEnum);
				audioSessionNotification = new AudioSessionNotification(this);
				Marshal.ThrowExceptionForHR(audioSessionInterface2.RegisterSessionNotification(audioSessionNotification));
			}
		}

		public void Dispose()
		{
			UnregisterNotifications();
			GC.SuppressFinalize(this);
		}

		private void UnregisterNotifications()
		{
			sessions = null;
			if (audioSessionNotification != null && audioSessionInterface2 != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionInterface2.UnregisterSessionNotification(audioSessionNotification));
				audioSessionNotification = null;
			}
		}

		~AudioSessionManager()
		{
			Dispose();
		}
	}
	internal class AudioSessionNotification : IAudioSessionNotification
	{
		private AudioSessionManager parent;

		internal AudioSessionNotification(AudioSessionManager parent)
		{
			this.parent = parent;
		}

		[PreserveSig]
		public int OnSessionCreated(IAudioSessionControl newSession)
		{
			parent.FireSessionCreated(newSession);
			return 0;
		}
	}
	public enum AudioStreamCategory
	{
		Other,
		ForegroundOnlyMedia,
		BackgroundCapableMedia,
		Communications,
		Alerts,
		SoundEffects,
		GameEffects,
		GameMedia
	}
	public class AudioStreamVolume : IDisposable
	{
		private IAudioStreamVolume audioStreamVolumeInterface;

		public int ChannelCount
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out var dwCount));
				return (int)dwCount;
			}
		}

		internal AudioStreamVolume(IAudioStreamVolume audioStreamVolumeInterface)
		{
			this.audioStreamVolumeInterface = audioStreamVolumeInterface;
		}

		private void CheckChannelIndex(int channelIndex, string parameter)
		{
			int channelCount = ChannelCount;
			if (channelIndex >= channelCount)
			{
				throw new ArgumentOutOfRangeException(parameter, "You must supply a valid channel index < current count of channels: " + channelCount);
			}
		}

		public float[] GetAllVolumes()
		{
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out var dwCount));
			float[] array = new float[dwCount];
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetAllVolumes(dwCount, array));
			return array;
		}

		public float GetChannelVolume(int channelIndex)
		{
			CheckChannelIndex(channelIndex, "channelIndex");
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelVolume((uint)channelIndex, out var fLevel));
			return fLevel;
		}

		public void SetAllVolumes(float[] levels)
		{
			int channelCount = ChannelCount;
			if (levels == null)
			{
				throw new ArgumentNullException("levels");
			}
			if (levels.Length != channelCount)
			{
				throw new ArgumentOutOfRangeException("levels", string.Format(CultureInfo.InvariantCulture, "SetAllVolumes MUST be supplied with a volume level for ALL channels. The AudioStream has {0} channels and you supplied {1} channels.", channelCount, levels.Length));
			}
			for (int i = 0; i < levels.Length; i++)
			{
				float num = levels[i];
				if (num < 0f)
				{
					throw new ArgumentOutOfRangeException("levels", "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i);
				}
				if (num > 1f)
				{
					throw new ArgumentOutOfRangeException("levels", "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i);
				}
			}
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetAllVoumes((uint)channelCount, levels));
		}

		public void SetChannelVolume(int index, float level)
		{
			CheckChannelIndex(index, "index");
			if (level < 0f)
			{
				throw new ArgumentOutOfRangeException("level", "Volume must be between 0.0 and 1.0");
			}
			if (level > 1f)
			{
				throw new ArgumentOutOfRangeException("level", "Volume must be between 0.0 and 1.0");
			}
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetChannelVolume((uint)index, level));
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && audioStreamVolumeInterface != null)
			{
				Marshal.ReleaseComObject(audioStreamVolumeInterface);
				audioStreamVolumeInterface = null;
			}
		}
	}
	public class AudioVolumeNotificationData
	{
		public Guid EventContext { get; }

		public bool Muted { get; }

		public Guid Guid { get; }

		public float MasterVolume { get; }

		public int Channels { get; }

		public float[] ChannelVolume { get; }

		public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume, Guid guid)
		{
			EventContext = eventContext;
			Muted = muted;
			MasterVolume = masterVolume;
			Channels = channelVolume.Length;
			ChannelVolume = channelVolume;
			Guid = guid;
		}
	}
	public class Connector
	{
		private readonly IConnector connectorInterface;

		public ConnectorType Type
		{
			get
			{
				connectorInterface.GetType(out var type);
				return type;
			}
		}

		public DataFlow DataFlow
		{
			get
			{
				connectorInterface.GetDataFlow(out var flow);
				return flow;
			}
		}

		public bool IsConnected
		{
			get
			{
				connectorInterface.IsConnected(out var connected);
				return connected;
			}
		}

		public Connector ConnectedTo
		{
			get
			{
				connectorInterface.GetConnectedTo(out var conTo);
				return new Connector(conTo);
			}
		}

		public string ConnectedToConnectorId
		{
			get
			{
				connectorInterface.GetConnectorIdConnectedTo(out var id);
				return id;
			}
		}

		public string ConnectedToDeviceId
		{
			get
			{
				connectorInterface.GetDeviceIdConnectedTo(out var id);
				return id;
			}
		}

		internal Connector(IConnector connector)
		{
			connectorInterface = connector;
		}

		public void ConnectTo(Connector other)
		{
			connectorInterface.ConnectTo(other.connectorInterface);
		}

		public void Disconnect()
		{
			connectorInterface.Disconnect();
		}
	}
	public enum ConnectorType
	{
		UnknownConnector,
		PhysicalInternal,
		PhysicalExternal,
		SoftwareIo,
		SoftwareFixed,
		Network
	}
	public enum DataFlow
	{
		Render,
		Capture,
		All
	}
	[Flags]
	public enum DeviceState
	{
		Active = 1,
		Disabled = 2,
		NotPresent = 4,
		Unplugged = 8,
		All = 0xF
	}
	public class DeviceTopology
	{
		private readonly IDeviceTopology deviceTopologyInterface;

		public uint ConnectorCount
		{
			get
			{
				deviceTopologyInterface.GetConnectorCount(out var count);
				return count;
			}
		}

		public string DeviceId
		{
			get
			{
				deviceTopologyInterface.GetDeviceId(out var id);
				return id;
			}
		}

		internal DeviceTopology(IDeviceTopology deviceTopology)
		{
			deviceTopologyInterface = deviceTopology;
		}

		public Connector GetConnector(uint index)
		{
			deviceTopologyInterface.GetConnector(index, out var connector);
			return new Connector(connector);
		}
	}
	[Flags]
	public enum EEndpointHardwareSupport
	{
		Volume = 1,
		Mute = 2,
		Meter = 4
	}
	public class MMDevice : IDisposable
	{
		private readonly IMMDevice deviceInterface;

		private PropertyStore propertyStore;

		private AudioMeterInformation audioMeterInformation;

		private AudioEndpointVolume audioEndpointVolume;

		private AudioSessionManager audioSessionManager;

		private DeviceTopology deviceTopology;

		private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");

		private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");

		private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");

		private static Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");

		private static Guid IDD_IDeviceTopology = new Guid("2A07407E-6497-4A18-9787-32F79BD0D98F");

		public AudioClient AudioClient => GetAudioClient();

		public AudioMeterInformation AudioMeterInformation
		{
			get
			{
				if (audioMeterInformation == null)
				{
					GetAudioMeterInformation();
				}
				return audioMeterInformation;
			}
		}

		public AudioEndpointVolume AudioEndpointVolume
		{
			get
			{
				if (audioEndpointVolume == null)
				{
					GetAudioEndpointVolume();
				}
				return audioEndpointVolume;
			}
		}

		public AudioSessionManager AudioSessionManager
		{
			get
			{
				if (audioSessionManager == null)
				{
					GetAudioSessionManager();
				}
				return audioSessionManager;
			}
		}

		public DeviceTopology DeviceTopology
		{
			get
			{
				if (deviceTopology == null)
				{
					GetDeviceTopology();
				}
				return deviceTopology;
			}
		}

		public PropertyStore Properties
		{
			get
			{
				if (propertyStore == null)
				{
					GetPropertyInformation();
				}
				return propertyStore;
			}
		}

		public string FriendlyName
		{
			get
			{
				if (propertyStore == null)
				{
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName))
				{
					return (string)propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
				}
				return "Unknown";
			}
		}

		public string DeviceFriendlyName
		{
			get
			{
				if (propertyStore == null)
				{
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
				{
					return (string)propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
				}
				return "Unknown";
			}
		}

		public string IconPath
		{
			get
			{
				if (propertyStore == null)
				{
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_Device_IconPath))
				{
					return (string)propertyStore[PropertyKeys.PKEY_Device_IconPath].Value;
				}
				return "Unknown";
			}
		}

		public string InstanceId
		{
			get
			{
				if (propertyStore == null)
				{
					GetPropertyInformation();
				}
				if (propertyStore.Contains(PropertyKeys.PKEY_Device_InstanceId))
				{
					return (string)propertyStore[PropertyKeys.PKEY_Device_InstanceId].Value;
				}
				return "Unknown";
			}
		}

		public string ID
		{
			get
			{
				Marshal.ThrowExceptionForHR(deviceInterface.GetId(out var id));
				return id;
			}
		}

		public DataFlow DataFlow
		{
			get
			{
				(deviceInterface as IMMEndpoint).GetDataFlow(out var dataFlow);
				return dataFlow;
			}
		}

		public DeviceState State
		{
			get
			{
				Marshal.ThrowExceptionForHR(deviceInterface.GetState(out var state));
				return state;
			}
		}

		public void GetPropertyInformation(StorageAccessMode stgmAccess = StorageAccessMode.Read)
		{
			Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(stgmAccess, out var properties));
			propertyStore = new PropertyStore(properties);
		}

		private AudioClient GetAudioClient()
		{
			Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioClient, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
			return new AudioClient(interfacePointer as IAudioClient);
		}

		private void GetAudioMeterInformation()
		{
			Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
			audioMeterInformation = new AudioMeterInformation(interfacePointer as IAudioMeterInformation);
		}

		private void GetAudioEndpointVolume()
		{
			Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioEndpointVolume, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
			audioEndpointVolume = new AudioEndpointVolume(interfacePointer as IAudioEndpointVolume);
		}

		private void GetAudioSessionManager()
		{
			Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IAudioSessionManager, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
			audioSessionManager = new AudioSessionManager(interfacePointer as IAudioSessionManager);
		}

		private void GetDeviceTopology()
		{
			Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IDeviceTopology, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
			deviceTopology = new DeviceTopology(interfacePointer as IDeviceTopology);
		}

		internal MMDevice(IMMDevice realDevice)
		{
			deviceInterface = realDevice;
		}

		public override string ToString()
		{
			return FriendlyName;
		}

		public void Dispose()
		{
			audioEndpointVolume?.Dispose();
			audioSessionManager?.Dispose();
			GC.SuppressFinalize(this);
		}

		~MMDevice()
		{
			Dispose();
		}
	}
	public class MMDeviceCollection : IEnumerable<MMDevice>, IEnumerable
	{
		private readonly IMMDeviceCollection mmDeviceCollection;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(mmDeviceCollection.GetCount(out var numDevices));
				return numDevices;
			}
		}

		public MMDevice this[int index]
		{
			get
			{
				mmDeviceCollection.Item(index, out var device);
				return new MMDevice(device);
			}
		}

		internal MMDeviceCollection(IMMDeviceCollection parent)
		{
			mmDeviceCollection = parent;
		}

		public IEnumerator<MMDevice> GetEnumerator()
		{
			for (int index = 0; index < Count; index++)
			{
				yield return this[index];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
	public class MMDeviceEnumerator : IDisposable
	{
		private IMMDeviceEnumerator realEnumerator;

		public MMDeviceEnumerator()
		{
			if (Environment.OSVersion.Version.Major < 6)
			{
				throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
			}
			realEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
		}

		public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, DeviceState dwStateMask)
		{
			Marshal.ThrowExceptionForHR(realEnumerator.EnumAudioEndpoints(dataFlow, dwStateMask, out var devices));
			return new MMDeviceCollection(devices);
		}

		public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role)
		{
			Marshal.ThrowExceptionForHR(realEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out var endpoint));
			return new MMDevice(endpoint);
		}

		public bool HasDefaultAudioEndpoint(DataFlow dataFlow, Role role)
		{
			IMMDevice endpoint;
			int defaultAudioEndpoint = realEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out endpoint);
			switch (defaultAudioEndpoint)
			{
			case 0:
				Marshal.ReleaseComObject(endpoint);
				return true;
			case -2147023728:
				return false;
			default:
				Marshal.ThrowExceptionForHR(defaultAudioEndpoint);
				return false;
			}
		}

		public MMDevice GetDevice(string id)
		{
			Marshal.ThrowExceptionForHR(realEnumerator.GetDevice(id, out var deviceName));
			return new MMDevice(deviceName);
		}

		public int RegisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
		{
			return realEnumerator.RegisterEndpointNotificationCallback(client);
		}

		public int UnregisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
		{
			return realEnumerator.UnregisterEndpointNotificationCallback(client);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && realEnumerator != null)
			{
				Marshal.ReleaseComObject(realEnumerator);
				realEnumerator = null;
			}
		}
	}
	public struct PropertyKey
	{
		public Guid formatId;

		public int propertyId;

		public PropertyKey(Guid formatId, int propertyId)
		{
			this.formatId = formatId;
			this.propertyId = propertyId;
		}
	}
	public static class PropertyKeys
	{
		public static readonly PropertyKey PKEY_DeviceInterface_FriendlyName = new PropertyKey(new Guid(40784238, -18412, 16715, 131, 205, 133, 109, 111, 239, 72, 34), 2);

		public static readonly PropertyKey PKEY_AudioEndpoint_FormFactor = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 0);

		public static readonly PropertyKey PKEY_AudioEndpoint_ControlPanelPageProvider = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 1);

		public static readonly PropertyKey PKEY_AudioEndpoint_Association = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 2);

		public static readonly PropertyKey PKEY_AudioEndpoint_PhysicalSpeakers = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 3);

		public static readonly PropertyKey PKEY_AudioEndpoint_GUID = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 4);

		public static readonly PropertyKey PKEY_AudioEndpoint_Disable_SysFx = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 5);

		public static readonly PropertyKey PKEY_AudioEndpoint_FullRangeSpeakers = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 6);

		public static readonly PropertyKey PKEY_AudioEndpoint_Supports_EventDriven_Mode = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 7);

		public static readonly PropertyKey PKEY_AudioEndpoint_JackSubType = new PropertyKey(new Guid(497408003, -11118, 20189, 140, 35, 224, 192, byte.MaxValue, 238, 127, 14), 8);

		public static readonly PropertyKey PKEY_AudioEngine_DeviceFormat = new PropertyKey(new Guid(-241236403, 2092, 20007, 188, 115, 104, 130, 161, 187, 142, 76), 0);

		public static readonly PropertyKey PKEY_AudioEngine_OEMFormat = new PropertyKey(new Guid(-460911066, 15557, 19666, 186, 70, 202, 10, 154, 112, 237, 4), 3);

		public static readonly PropertyKey PKEY_Device_FriendlyName = new PropertyKey(new Guid(-1537465010, -8420, 20221, 128, 32, 103, 209, 70, 168, 80, 224), 14);

		public static readonly PropertyKey PKEY_Device_IconPath = new PropertyKey(new Guid(630898684, 20647, 18382, 175, 8, 104, 201, 167, 215, 51, 102), 12);

		public static readonly PropertyKey PKEY_Device_DeviceDesc = new PropertyKey(new Guid(-1537465010, -8420, 20221, 128, 32, 103, 209, 70, 168, 80, 224), 2);

		public static readonly PropertyKey PKEY_Device_ControllerDeviceId = new PropertyKey(new Guid(-1275528621, 4, 17294, 144, 3, 81, 164, 110, 19, 155, 252), 2);

		public static readonly PropertyKey PKEY_Device_InterfaceKey = new PropertyKey(new Guid(590439624, 6956, 19581, 188, 104, 182, 113, 104, 122, 37, 103), 1);

		public static readonly PropertyKey PKEY_Device_InstanceId = new PropertyKey(new Guid(2026065864, 4170, 19146, 158, 164, 82, 77, 82, 153, 110, 87), 256);
	}
	public class PropertyStore
	{
		private readonly IPropertyStore storeInterface;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(storeInterface.GetCount(out var propCount));
				return propCount;
			}
		}

		public PropertyStoreProperty this[int index]
		{
			get
			{
				PropertyKey key = Get(index);
				Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out var value));
				return new PropertyStoreProperty(key, value);
			}
		}

		public PropertyStoreProperty this[PropertyKey key]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					PropertyKey key2 = Get(i);
					if (key2.formatId == key.formatId && key2.propertyId == key.propertyId)
					{
						Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key2, out var value));
						return new PropertyStoreProperty(key2, value);
					}
				}
				return null;
			}
		}

		public bool Contains(PropertyKey key)
		{
			for (int i = 0; i < Count; i++)
			{
				PropertyKey propertyKey = Get(i);
				if (propertyKey.formatId == key.formatId && propertyKey.propertyId == key.propertyId)
				{
					return true;
				}
			}
			return false;
		}

		public PropertyKey Get(int index)
		{
			Marshal.ThrowExceptionForHR(storeInterface.GetAt(index, out var key));
			return key;
		}

		public PropVariant GetValue(int index)
		{
			PropertyKey key = Get(index);
			Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out var value));
			return value;
		}

		public void SetValue(PropertyKey key, PropVariant value)
		{
			Marshal.ThrowExceptionForHR(storeInterface.SetValue(ref key, ref value));
		}

		public void Commit()
		{
			Marshal.ThrowExceptionForHR(storeInterface.Commit());
		}

		internal PropertyStore(IPropertyStore store)
		{
			storeInterface = store;
		}
	}
	public class PropertyStoreProperty
	{
		private PropVariant propertyValue;

		public PropertyKey Key { get; }

		public object Value => propertyValue.Value;

		internal PropertyStoreProperty(PropertyKey key, PropVariant value)
		{
			Key = key;
			propertyValue = value;
		}
	}
	public enum Role
	{
		Console,
		Multimedia,
		Communications
	}
	public class SessionCollection
	{
		private readonly IAudioSessionEnumerator audioSessionEnumerator;

		public AudioSessionControl this[int index]
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionEnumerator.GetSession(index, out var session));
				return new AudioSessionControl(session);
			}
		}

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionEnumerator.GetCount(out var sessionCount));
				return sessionCount;
			}
		}

		internal SessionCollection(IAudioSessionEnumerator realEnumerator)
		{
			audioSessionEnumerator = realEnumerator;
		}
	}
	public class SimpleAudioVolume : IDisposable
	{
		private readonly ISimpleAudioVolume simpleAudioVolume;

		public float Volume
		{
			get
			{
				Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMasterVolume(out var levelNorm));
				return levelNorm;
			}
			set
			{
				if ((double)value >= 0.0 && (double)value <= 1.0)
				{
					Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMasterVolume(value, Guid.Empty));
				}
			}
		}

		public bool Mute
		{
			get
			{
				Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMute(out var isMuted));
				return isMuted;
			}
			set
			{
				Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMute(value, Guid.Empty));
			}
		}

		internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume)
		{
			simpleAudioVolume = realSimpleVolume;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		~SimpleAudioVolume()
		{
			Dispose();
		}
	}
	public enum CaptureState
	{
		Stopped,
		Starting,
		Capturing,
		Stopping
	}
	public class WasapiCapture : IWaveIn, IDisposable
	{
		private const long ReftimesPerSec = 10000000L;

		private const long ReftimesPerMillisec = 10000L;

		private volatile CaptureState captureState;

		private byte[] recordBuffer;

		private Thread captureThread;

		private AudioClient audioClient;

		private int bytesPerFrame;

		private WaveFormat waveFormat;

		private bool initialized;

		private readonly SynchronizationContext syncContext;

		private readonly bool isUsingEventSync;

		private EventWaitHandle frameEventWaitHandle;

		private readonly int audioBufferMillisecondsLength;

		public AudioClientShareMode ShareMode { get; set; }

		public CaptureState CaptureState => captureState;

		public virtual WaveFormat WaveFormat
		{
			get
			{
				return waveFormat.AsStandardWaveFormat();
			}
			set
			{
				waveFormat = value;
			}
		}

		public event EventHandler<WaveInEventArgs> DataAvailable;

		public event EventHandler<StoppedEventArgs> RecordingStopped;

		public WasapiCapture()
			: this(GetDefaultCaptureDevice())
		{
		}

		public WasapiCapture(MMDevice captureDevice)
			: this(captureDevice, useEventSync: false)
		{
		}

		public WasapiCapture(MMDevice captureDevice, bool useEventSync)
			: this(captureDevice, useEventSync, 100)
		{
		}

		public WasapiCapture(MMDevice captureDevice, bool useEventSync, int audioBufferMillisecondsLength)
		{
			syncContext = SynchronizationContext.Current;
			audioClient = captureDevice.AudioClient;
			ShareMode = AudioClientShareMode.Shared;
			isUsingEventSync = useEventSync;
			this.audioBufferMillisecondsLength = audioBufferMillisecondsLength;
			waveFormat = audioClient.MixFormat;
		}

		public static MMDevice GetDefaultCaptureDevice()
		{
			return new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
		}

		private void InitializeCaptureDevice()
		{
			if (initialized)
			{
				return;
			}
			long num = 10000L * (long)audioBufferMillisecondsLength;
			if (!audioClient.IsFormatSupported(ShareMode, waveFormat))
			{
				throw new ArgumentException("Unsupported Wave Format");
			}
			AudioClientStreamFlags audioClientStreamFlags = GetAudioClientStreamFlags();
			if (isUsingEventSync)
			{
				if (ShareMode == AudioClientShareMode.Shared)
				{
					audioClient.Initialize(ShareMode, AudioClientStreamFlags.EventCallback | audioClientStreamFlags, num, 0L, waveFormat, Guid.Empty);
				}
				else
				{
					audioClient.Initialize(ShareMode, AudioClientStreamFlags.EventCallback | audioClientStreamFlags, num, num, waveFormat, Guid.Empty);
				}
				frameEventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
				audioClient.SetEventHandle(frameEventWaitHandle.SafeWaitHandle.DangerousGetHandle());
			}
			else
			{
				audioClient.Initialize(ShareMode, audioClientStreamFlags, num, 0L, waveFormat, Guid.Empty);
			}
			int bufferSize = audioClient.BufferSize;
			bytesPerFrame = waveFormat.Channels * waveFormat.BitsPerSample / 8;
			recordBuffer = new byte[bufferSize * bytesPerFrame];
			initialized = true;
		}

		protected virtual AudioClientStreamFlags GetAudioClientStreamFlags()
		{
			return AudioClientStreamFlags.None;
		}

		public void StartRecording()
		{
			if (captureState != 0)
			{
				throw new InvalidOperationException("Previous recording still in progress");
			}
			captureState = CaptureState.Starting;
			InitializeCaptureDevice();
			captureThread = new Thread((ThreadStart)delegate
			{
				CaptureThread(audioClient);
			});
			captureThread.Start();
		}

		public void StopRecording()
		{
			if (captureState != 0)
			{
				captureState = CaptureState.Stopping;
			}
		}

		private void CaptureThread(AudioClient client)
		{
			Exception e = null;
			try
			{
				DoRecording(client);
			}
			catch (Exception ex)
			{
				e = ex;
			}
			finally
			{
				client.Stop();
			}
			captureThread = null;
			captureState = CaptureState.Stopped;
			RaiseRecordingStopped(e);
		}

		private void DoRecording(AudioClient client)
		{
			int bufferSize = client.BufferSize;
			long num = (long)(10000000.0 * (double)bufferSize / (double)waveFormat.SampleRate);
			int millisecondsTimeout = (int)(num / 10000 / 2);
			int millisecondsTimeout2 = (int)(3 * num / 10000);
			AudioCaptureClient audioCaptureClient = client.AudioCaptureClient;
			client.Start();
			if (captureState == CaptureState.Starting)
			{
				captureState = CaptureState.Capturing;
			}
			while (captureState == CaptureState.Capturing)
			{
				bool flag = true;
				if (isUsingEventSync)
				{
					flag = frameEventWaitHandle.WaitOne(millisecondsTimeout2, exitContext: false);
				}
				else
				{
					Thread.Sleep(millisecondsTimeout);
				}
				if (captureState == CaptureState.Capturing)
				{
					if (flag)
					{
						ReadNextPacket(audioCaptureClient);
					}
					continue;
				}
				break;
			}
		}

		private void RaiseRecordingStopped(Exception e)
		{
			EventHandler<StoppedEventArgs> handler = this.RecordingStopped;
			if (handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				handler(this, new StoppedEventArgs(e));
				return;
			}
			syncContext.Post(delegate
			{
				handler(this, new StoppedEventArgs(e));
			}, null);
		}

		private void ReadNextPacket(AudioCaptureClient capture)
		{
			int nextPacketSize = capture.GetNextPacketSize();
			int num = 0;
			while (nextPacketSize != 0)
			{
				int numFramesToRead;
				AudioClientBufferFlags bufferFlags;
				IntPtr buffer = capture.GetBuffer(out numFramesToRead, out bufferFlags);
				int num2 = numFramesToRead * bytesPerFrame;
				if (Math.Max(0, recordBuffer.Length - num) < num2 && num > 0)
				{
					this.DataAvailable?.Invoke(this, new WaveInEventArgs(recordBuffer, num));
					num = 0;
				}
				if ((bufferFlags & AudioClientBufferFlags.Silent) != AudioClientBufferFlags.Silent)
				{
					Marshal.Copy(buffer, recordBuffer, num, num2);
				}
				else
				{
					Array.Clear(recordBuffer, num, num2);
				}
				num += num2;
				capture.ReleaseBuffer(numFramesToRead);
				nextPacketSize = capture.GetNextPacketSize();
			}
			this.DataAvailable?.Invoke(this, new WaveInEventArgs(recordBuffer, num));
		}

		public void Dispose()
		{
			StopRecording();
			if (captureThread != null)
			{
				captureThread.Join();
				captureThread = null;
			}
			if (audioClient != null)
			{
				audioClient.Dispose();
				audioClient = null;
			}
		}
	}
}
namespace NAudio.CoreAudioApi.Interfaces
{
	internal struct AudioVolumeNotificationDataStruct
	{
		public Guid guidEventContext;

		public bool bMuted;

		public float fMasterVolume;

		public uint nChannels;

		public float ChannelVolume;
	}
	public struct Blob
	{
		public int Length;

		public IntPtr Data;
	}
	[Flags]
	internal enum ClsCtx
	{
		INPROC_SERVER = 1,
		INPROC_HANDLER = 2,
		LOCAL_SERVER = 4,
		INPROC_SERVER16 = 8,
		REMOTE_SERVER = 0x10,
		INPROC_HANDLER16 = 0x20,
		NO_CODE_DOWNLOAD = 0x400,
		NO_CUSTOM_MARSHAL = 0x1000,
		ENABLE_CODE_DOWNLOAD = 0x2000,
		NO_FAILURE_LOG = 0x4000,
		DISABLE_AAA = 0x8000,
		ENABLE_AAA = 0x10000,
		FROM_DEFAULT_CONTEXT = 0x20000,
		ACTIVATE_32_BIT_SERVER = 0x40000,
		ACTIVATE_64_BIT_SERVER = 0x80000,
		ENABLE_CLOAKING = 0x100000,
		PS_DLL = int.MinValue,
		INPROC = 3,
		SERVER = 0x15,
		ALL = 0x17
	}
	public enum AudioClientErrors
	{
		NotInitialized = -2004287487,
		UnsupportedFormat = -2004287480,
		DeviceInUse = -2004287478,
		ResourcesInvalidated = -2004287450
	}
	internal static class ErrorCodes
	{
		public const int SEVERITY_ERROR = 1;

		public const int FACILITY_AUDCLNT = 2185;

		public static readonly int AUDCLNT_E_NOT_INITIALIZED = HResult.MAKE_HRESULT(1, 2185, 1);

		public static readonly int AUDCLNT_E_ALREADY_INITIALIZED = HResult.MAKE_HRESULT(1, 2185, 2);

		public static readonly int AUDCLNT_E_WRONG_ENDPOINT_TYPE = HResult.MAKE_HRESULT(1, 2185, 3);

		public static readonly int AUDCLNT_E_DEVICE_INVALIDATED = HResult.MAKE_HRESULT(1, 2185, 4);

		public static readonly int AUDCLNT_E_NOT_STOPPED = HResult.MAKE_HRESULT(1, 2185, 5);

		public static readonly int AUDCLNT_E_BUFFER_TOO_LARGE = HResult.MAKE_HRESULT(1, 2185, 6);

		public static readonly int AUDCLNT_E_OUT_OF_ORDER = HResult.MAKE_HRESULT(1, 2185, 7);

		public static readonly int AUDCLNT_E_UNSUPPORTED_FORMAT = HResult.MAKE_HRESULT(1, 2185, 8);

		public static readonly int AUDCLNT_E_INVALID_SIZE = HResult.MAKE_HRESULT(1, 2185, 9);

		public static readonly int AUDCLNT_E_DEVICE_IN_USE = HResult.MAKE_HRESULT(1, 2185, 10);

		public static readonly int AUDCLNT_E_BUFFER_OPERATION_PENDING = HResult.MAKE_HRESULT(1, 2185, 11);

		public static readonly int AUDCLNT_E_THREAD_NOT_REGISTERED = HResult.MAKE_HRESULT(1, 2185, 12);

		public static readonly int AUDCLNT_E_EXCLUSIVE_MODE_NOT_ALLOWED = HResult.MAKE_HRESULT(1, 2185, 14);

		public static readonly int AUDCLNT_E_ENDPOINT_CREATE_FAILED = HResult.MAKE_HRESULT(1, 2185, 15);

		public static readonly int AUDCLNT_E_SERVICE_NOT_RUNNING = HResult.MAKE_HRESULT(1, 2185, 16);

		public static readonly int AUDCLNT_E_EVENTHANDLE_NOT_EXPECTED = HResult.MAKE_HRESULT(1, 2185, 17);

		public static readonly int AUDCLNT_E_EXCLUSIVE_MODE_ONLY = HResult.MAKE_HRESULT(1, 2185, 18);

		public static readonly int AUDCLNT_E_BUFDURATION_PERIOD_NOT_EQUAL = HResult.MAKE_HRESULT(1, 2185, 19);

		public static readonly int AUDCLNT_E_EVENTHANDLE_NOT_SET = HResult.MAKE_HRESULT(1, 2185, 20);

		public static readonly int AUDCLNT_E_INCORRECT_BUFFER_SIZE = HResult.MAKE_HRESULT(1, 2185, 21);

		public static readonly int AUDCLNT_E_BUFFER_SIZE_ERROR = HResult.MAKE_HRESULT(1, 2185, 22);

		public static readonly int AUDCLNT_E_CPUUSAGE_EXCEEDED = HResult.MAKE_HRESULT(1, 2185, 23);

		public static readonly int AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED = HResult.MAKE_HRESULT(1, 2185, 25);

		public static readonly int AUDCLNT_E_RESOURCES_INVALIDATED = -2004287450;
	}
	[ComImport]
	[Guid("C8ADBD64-E71E-48a0-A4DE-185C395CD317")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioCaptureClient
	{
		int GetBuffer(out IntPtr dataBuffer, out int numFramesToRead, out AudioClientBufferFlags bufferFlags, out long devicePosition, out long qpcPosition);

		int ReleaseBuffer(int numFramesRead);

		int GetNextPacketSize(out int numFramesInNextPacket);
	}
	[ComImport]
	[Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioClient
	{
		[PreserveSig]
		int Initialize(AudioClientShareMode shareMode, AudioClientStreamFlags streamFlags, long hnsBufferDuration, long hnsPeriodicity, [In] WaveFormat pFormat, [In] ref Guid audioSessionGuid);

		int GetBufferSize(out uint bufferSize);

		[return: MarshalAs(UnmanagedType.I8)]
		long GetStreamLatency();

		int GetCurrentPadding(out int currentPadding);

		[PreserveSig]
		int IsFormatSupported(AudioClientShareMode shareMode, [In] WaveFormat pFormat, IntPtr closestMatchFormat);

		int GetMixFormat(out IntPtr deviceFormatPointer);

		int GetDevicePeriod(out long defaultDevicePeriod, out long minimumDevicePeriod);

		int Start();

		int Stop();

		int Reset();

		int SetEventHandle(IntPtr eventHandle);

		[PreserveSig]
		int GetService([In][MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId, [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
	}
	[Guid("CD63314F-3FBA-4a1b-812C-EF96358728E7")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioClock
	{
		[PreserveSig]
		int GetFrequency(out ulong frequency);

		[PreserveSig]
		int GetPosition(out ulong devicePosition, out ulong qpcPosition);

		[PreserveSig]
		int GetCharacteristics(out uint characteristics);
	}
	[Guid("6f49ff73-6727-49AC-A008-D98CF5E70048")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioClock2 : IAudioClock
	{
		[PreserveSig]
		int GetDevicePosition(out ulong devicePosition, out ulong qpcPosition);
	}
	[Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioEndpointVolume
	{
		int RegisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);

		int UnregisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);

		int GetChannelCount(out int pnChannelCount);

		int SetMasterVolumeLevel(float fLevelDB, ref Guid pguidEventContext);

		int SetMasterVolumeLevelScalar(float fLevel, ref Guid pguidEventContext);

		int GetMasterVolumeLevel(out float pfLevelDB);

		int GetMasterVolumeLevelScalar(out float pfLevel);

		int SetChannelVolumeLevel(uint nChannel, float fLevelDB, ref Guid pguidEventContext);

		int SetChannelVolumeLevelScalar(uint nChannel, float fLevel, ref Guid pguidEventContext);

		int GetChannelVolumeLevel(uint nChannel, out float pfLevelDB);

		int GetChannelVolumeLevelScalar(uint nChannel, out float pfLevel);

		int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, ref Guid pguidEventContext);

		int GetMute(out bool pbMute);

		int GetVolumeStepInfo(out uint pnStep, out uint pnStepCount);

		int VolumeStepUp(ref Guid pguidEventContext);

		int VolumeStepDown(ref Guid pguidEventContext);

		int QueryHardwareSupport(out uint pdwHardwareSupportMask);

		int GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
	}
	[Guid("657804FA-D6AD-4496-8A60-352752AF4F89")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioEndpointVolumeCallback
	{
		void OnNotify(IntPtr notifyData);
	}
	[Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioMeterInformation
	{
		int GetPeakValue(out float pfPeak);

		int GetMeteringChannelCount(out int pnChannelCount);

		int GetChannelsPeakValues(int u32ChannelCount, [In] IntPtr afPeakValues);

		int QueryHardwareSupport(out int pdwHardwareSupportMask);
	}
	[ComImport]
	[Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioRenderClient
	{
		int GetBuffer(int numFramesRequested, out IntPtr dataBufferPointer);

		int ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags);
	}
	[Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAudioSessionControl
	{
		[PreserveSig]
		int GetState(out AudioSessionState state);

		[PreserveSig]
		int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string displayName);

		[PreserveSig]
		int SetDisplayName([In][MarshalAs(UnmanagedType.LPWStr)] string displayName, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string iconPath);

		[PreserveSig]
		int SetIconPath([In][MarshalAs(UnmanagedType.LPWStr)] string iconPath, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetGroupingParam(out Guid groupingId);

		[PreserveSig]
		int SetGroupingParam([In][MarshalAs(UnmanagedType.LPStruct)] Guid groupingId, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int RegisterAudioSessionNotification([In] IAudioSessionEvents client);

		[PreserveSig]
		int UnregisterAudioSessionNotification([In] IAudioSessionEvents client);
	}
	[Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAudioSessionControl2 : IAudioSessionControl
	{
		[PreserveSig]
		new int GetState(out AudioSessionState state);

		[PreserveSig]
		new int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string displayName);

		[PreserveSig]
		new int SetDisplayName([In][MarshalAs(UnmanagedType.LPWStr)] string displayName, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		new int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string iconPath);

		[PreserveSig]
		new int SetIconPath([In][MarshalAs(UnmanagedType.LPWStr)] string iconPath, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		new int GetGroupingParam(out Guid groupingId);

		[PreserveSig]
		new int SetGroupingParam([In][MarshalAs(UnmanagedType.LPStruct)] Guid groupingId, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		new int RegisterAudioSessionNotification([In] IAudioSessionEvents client);

		[PreserveSig]
		new int UnregisterAudioSessionNotification([In] IAudioSessionEvents client);

		[PreserveSig]
		int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string retVal);

		[PreserveSig]
		int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string retVal);

		[PreserveSig]
		int GetProcessId(out uint retVal);

		[PreserveSig]
		int IsSystemSoundsSession();

		[PreserveSig]
		int SetDuckingPreference(bool optOut);
	}
	[Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionEnumerator
	{
		int GetCount(out int sessionCount);

		int GetSession(int sessionCount, out IAudioSessionControl session);
	}
	public enum AudioSessionState
	{
		AudioSessionStateInactive,
		AudioSessionStateActive,
		AudioSessionStateExpired
	}
	public enum AudioSessionDisconnectReason
	{
		DisconnectReasonDeviceRemoval,
		DisconnectReasonServerShutdown,
		DisconnectReasonFormatChanged,
		DisconnectReasonSessionLogoff,
		DisconnectReasonSessionDisconnected,
		DisconnectReasonExclusiveModeOverride
	}
	[Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAudioSessionEvents
	{
		[PreserveSig]
		int OnDisplayNameChanged([In][MarshalAs(UnmanagedType.LPWStr)] string displayName, [In] ref Guid eventContext);

		[PreserveSig]
		int OnIconPathChanged([In][MarshalAs(UnmanagedType.LPWStr)] string iconPath, [In] ref Guid eventContext);

		[PreserveSig]
		int OnSimpleVolumeChanged([In][MarshalAs(UnmanagedType.R4)] float volume, [In][MarshalAs(UnmanagedType.Bool)] bool isMuted, [In] ref Guid eventContext);

		[PreserveSig]
		int OnChannelVolumeChanged([In][MarshalAs(UnmanagedType.U4)] uint channelCount, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes, [In][MarshalAs(UnmanagedType.U4)] uint channelIndex, [In] ref Guid eventContext);

		[PreserveSig]
		int OnGroupingParamChanged([In] ref Guid groupingId, [In] ref Guid eventContext);

		[PreserveSig]
		int OnStateChanged([In] AudioSessionState state);

		[PreserveSig]
		int OnSessionDisconnected([In] AudioSessionDisconnectReason disconnectReason);
	}
	public interface IAudioSessionEventsHandler
	{
		void OnVolumeChanged(float volume, bool isMuted);

		void OnDisplayNameChanged(string displayName);

		void OnIconPathChanged(string iconPath);

		void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex);

		void OnGroupingParamChanged(ref Guid groupingId);

		void OnStateChanged(AudioSessionState state);

		void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason);
	}
	[Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionManager
	{
		[PreserveSig]
		int GetAudioSessionControl([Optional][In][MarshalAs(UnmanagedType.LPStruct)] Guid sessionId, [In][MarshalAs(UnmanagedType.U4)] uint streamFlags, [MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl sessionControl);

		[PreserveSig]
		int GetSimpleAudioVolume([Optional][In][MarshalAs(UnmanagedType.LPStruct)] Guid sessionId, [In][MarshalAs(UnmanagedType.U4)] uint streamFlags, [MarshalAs(UnmanagedType.Interface)] out ISimpleAudioVolume audioVolume);
	}
	[Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionManager2 : IAudioSessionManager
	{
		[PreserveSig]
		new int GetAudioSessionControl([Optional][In][MarshalAs(UnmanagedType.LPStruct)] Guid sessionId, [In][MarshalAs(UnmanagedType.U4)] uint streamFlags, [MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl sessionControl);

		[PreserveSig]
		new int GetSimpleAudioVolume([Optional][In][MarshalAs(UnmanagedType.LPStruct)] Guid sessionId, [In][MarshalAs(UnmanagedType.U4)] uint streamFlags, [MarshalAs(UnmanagedType.Interface)] out ISimpleAudioVolume audioVolume);

		[PreserveSig]
		int GetSessionEnumerator(out IAudioSessionEnumerator sessionEnum);

		[PreserveSig]
		int RegisterSessionNotification(IAudioSessionNotification sessionNotification);

		[PreserveSig]
		int UnregisterSessionNotification(IAudioSessionNotification sessionNotification);

		[PreserveSig]
		int RegisterDuckNotification(string sessionId, IAudioSessionNotification audioVolumeDuckNotification);

		[PreserveSig]
		int UnregisterDuckNotification(IntPtr audioVolumeDuckNotification);
	}
	[Guid("641DD20B-4D41-49CC-ABA3-174B9477BB08")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAudioSessionNotification
	{
		[PreserveSig]
		int OnSessionCreated(IAudioSessionControl newSession);
	}
	[Guid("93014887-242D-4068-8A15-CF5E93B90FE3")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioStreamVolume
	{
		[PreserveSig]
		int GetChannelCount(out uint dwCount);

		[PreserveSig]
		int SetChannelVolume([In] uint dwIndex, [In] float fLevel);

		[PreserveSig]
		int GetChannelVolume([In] uint dwIndex, out float fLevel);

		[PreserveSig]
		int SetAllVoumes([In] uint dwCount, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4, SizeParamIndex = 0)] float[] fVolumes);

		[PreserveSig]
		int GetAllVolumes([In] uint dwCount, [MarshalAs(UnmanagedType.LPArray)] float[] pfVolumes);
	}
	[ComImport]
	[Guid("9C2C4058-23F5-41DE-877A-DF3AF236A09E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IConnector
	{
		int GetType(out ConnectorType type);

		int GetDataFlow(out DataFlow flow);

		int ConnectTo([In] IConnector connectTo);

		int Disconnect();

		int IsConnected(out bool connected);

		int GetConnectedTo(out IConnector conTo);

		int GetConnectorIdConnectedTo([MarshalAs(UnmanagedType.LPWStr)] out string id);

		int GetDeviceIdConnectedTo([MarshalAs(UnmanagedType.LPWStr)] out string id);
	}
	[ComImport]
	[Guid("2A07407E-6497-4A18-9787-32F79BD0D98F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDeviceTopology
	{
		int GetConnectorCount(out uint count);

		int GetConnector(uint index, out IConnector connector);

		int GetSubunitCount(out uint count);

		int GetSubunit(uint index, out ISubunit subunit);

		int GetPartById(uint id, out IPart part);

		int GetDeviceId([MarshalAs(UnmanagedType.LPWStr)] out string id);

		int GetSignalPath(IPart from, IPart to, bool rejectMixedPaths, out IPartsList parts);
	}
	[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMMDevice
	{
		int Activate(ref Guid id, ClsCtx clsCtx, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);

		int OpenPropertyStore(StorageAccessMode stgmAccess, out IPropertyStore properties);

		int GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);

		int GetState(out DeviceState state);
	}
	[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMMDeviceCollection
	{
		int GetCount(out int numDevices);

		int Item(int deviceNumber, out IMMDevice device);
	}
	[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMMDeviceEnumerator
	{
		int EnumAudioEndpoints(DataFlow dataFlow, DeviceState stateMask, out IMMDeviceCollection devices);

		[PreserveSig]
		int GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IMMDevice endpoint);

		int GetDevice(string id, out IMMDevice deviceName);

		int RegisterEndpointNotificationCallback(IMMNotificationClient client);

		int UnregisterEndpointNotificationCallback(IMMNotificationClient client);
	}
	[Guid("1BE09788-6894-4089-8586-9A2A6C265AC5")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMMEndpoint
	{
		int GetDataFlow(out DataFlow dataFlow);
	}
	[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMNotificationClient
	{
		void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.I4)] DeviceState newState);

		void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);

		void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

		void OnDefaultDeviceChanged(DataFlow flow, Role role, [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId);

		void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PropertyKey key);
	}
	[ComImport]
	[Guid("AE2DE0E4-5BCA-4F2D-AA46-5D13F8FDB3A9")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPart
	{
	}
	[ComImport]
	[Guid("6DAA848C-5EB0-45CC-AEA5-998A2CDA1FFB")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPartsList
	{
		int GetCount(out uint count);

		int GetPart(uint index, out IPart part);
	}
	[Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPropertyStore
	{
		int GetCount(out int propCount);

		int GetAt(int property, out PropertyKey key);

		int GetValue(ref PropertyKey key, out PropVariant value);

		int SetValue(ref PropertyKey key, ref PropVariant value);

		int Commit();
	}
	[Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ISimpleAudioVolume
	{
		[PreserveSig]
		int SetMasterVolume([In][MarshalAs(UnmanagedType.R4)] float levelNorm, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetMasterVolume([MarshalAs(UnmanagedType.R4)] out float levelNorm);

		[PreserveSig]
		int SetMute([In][MarshalAs(UnmanagedType.Bool)] bool isMuted, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetMute([MarshalAs(UnmanagedType.Bool)] out bool isMuted);
	}
	[ComImport]
	[Guid("82149A85-DBA6-4487-86BB-EA8F7FEFCC71")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ISubunit
	{
	}
	[ComImport]
	[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
	internal class MMDeviceEnumeratorComObject
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MMDeviceEnumeratorComObject();
	}
	public enum StorageAccessMode
	{
		Read,
		Write,
		ReadWrite
	}
	[StructLayout(LayoutKind.Explicit)]
	public struct PropVariant
	{
		[FieldOffset(0)]
		public short vt;

		[FieldOffset(2)]
		public short wReserved1;

		[FieldOffset(4)]
		public short wReserved2;

		[FieldOffset(6)]
		public short wReserved3;

		[FieldOffset(8)]
		public sbyte cVal;

		[FieldOffset(8)]
		public byte bVal;

		[FieldOffset(8)]
		public short iVal;

		[FieldOffset(8)]
		public ushort uiVal;

		[FieldOffset(8)]
		public int lVal;

		[FieldOffset(8)]
		public uint ulVal;

		[FieldOffset(8)]
		public int intVal;

		[FieldOffset(8)]
		public uint uintVal;

		[FieldOffset(8)]
		public long hVal;

		[FieldOffset(8)]
		public long uhVal;

		[FieldOffset(8)]
		public float fltVal;

		[FieldOffset(8)]
		public double dblVal;

		[FieldOffset(8)]
		public short boolVal;

		[FieldOffset(8)]
		public int scode;

		[FieldOffset(8)]
		public System.Runtime.InteropServices.ComTypes.FILETIME filetime;

		[FieldOffset(8)]
		public Blob blobVal;

		[FieldOffset(8)]
		public IntPtr pointerValue;

		public VarEnum DataType => (VarEnum)vt;

		public object Value
		{
			get
			{
				VarEnum dataType = DataType;
				switch (dataType)
				{
				case VarEnum.VT_I1:
					return bVal;
				case VarEnum.VT_I2:
					return iVal;
				case VarEnum.VT_I4:
					return lVal;
				case VarEnum.VT_I8:
					return hVal;
				case VarEnum.VT_INT:
					return iVal;
				case VarEnum.VT_UI4:
					return ulVal;
				case VarEnum.VT_UI8:
					return uhVal;
				case VarEnum.VT_LPWSTR:
					return Marshal.PtrToStringUni(pointerValue);
				case VarEnum.VT_BLOB:
				case (VarEnum)4113:
					return GetBlob();
				case VarEnum.VT_CLSID:
					return MarshalHelpers.PtrToStructure<Guid>(pointerValue);
				case VarEnum.VT_BOOL:
					return boolVal switch
					{
						-1 => true, 
						0 => false, 
						_ => throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0"), 
					};
				case VarEnum.VT_FILETIME:
					return DateTime.FromFileTime(((long)filetime.dwHighDateTime << 32) + filetime.dwLowDateTime);
				default:
					throw new NotImplementedException("PropVariant " + dataType);
				}
			}
		}

		public static PropVariant FromLong(long value)
		{
			PropVariant result = default(PropVariant);
			result.vt = 20;
			result.hVal = value;
			return result;
		}

		private byte[] GetBlob()
		{
			byte[] array = new byte[blobVal.Length];
			Marshal.Copy(blobVal.Data, array, 0, array.Length);
			return array;
		}

		public T[] GetBlobAsArrayOf<T>()
		{
			int length = blobVal.Length;
			int num = Marshal.SizeOf((T)Activator.CreateInstance(typeof(T)));
			if (length % num != 0)
			{
				throw new InvalidDataException($"Blob size {length} not a multiple of struct size {num}");
			}
			int num2 = length / num;
			T[] array = new T[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = (T)Activator.CreateInstance(typeof(T));
				Marshal.PtrToStructure(new IntPtr((long)blobVal.Data + i * num), array[i]);
			}
			return array;
		}

		[Obsolete("Call with pointer instead")]
		public void Clear()
		{
			PropVariantNative.PropVariantClear(ref this);
		}

		public static void Clear(IntPtr ptr)
		{
			PropVariantNative.PropVariantClear(ptr);
		}
	}
	internal class PropVariantNative
	{
		[DllImport("ole32.dll")]
		internal static extern int PropVariantClear(ref PropVariant pvar);

		[DllImport("ole32.dll")]
		internal static extern int PropVariantClear(IntPtr pvar);
	}
}
namespace NAudio.Codecs
{
	public class ALawDecoder
	{
		private static readonly short[] ALawDecompressTable = new short[256]
		{
			-5504, -5248, -6016, -5760, -4480, -4224, -4992, -4736, -7552, -7296,
			-8064, -7808, -6528, -6272, -7040, -6784, -2752, -2624, -3008, -2880,
			-2240, -2112, -2496, -2368, -3776, -3648, -4032, -3904, -3264, -3136,
			-3520, -3392, -22016, -20992, -24064, -23040, -17920, -16896, -19968, -18944,
			-30208, -29184, -32256, -31232, -26112, -25088, -28160, -27136, -11008, -10496,
			-12032, -11520, -8960, -8448, -9984, -9472, -15104, -14592, -16128, -15616,
			-13056, -12544, -14080, -13568, -344, -328, -376, -360, -280, -264,
			-312, -296, -472, -456, -504, -488, -408, -392, -440, -424,
			-88, -72, -120, -104, -24, -8, -56, -40, -216, -200,
			-248, -232, -152, -136, -184, -168, -1376, -1312, -1504, -1440,
			-1120, -1056, -1248, -1184, -1888, -1824, -2016, -1952, -1632, -1568,
			-1760, -1696, -688, -656, -752, -720, -560, -528, -624, -592,
			-944, -912, -1008, -976, -816, -784, -880, -848, 5504, 5248,
			6016, 5760, 4480, 4224, 4992, 4736, 7552, 7296, 8064, 7808,
			6528, 6272, 7040, 6784, 2752, 2624, 3008, 2880, 2240, 2112,
			2496, 2368, 3776, 3648, 4032, 3904, 3264, 3136, 3520, 3392,
			22016, 20992, 24064, 23040, 17920, 16896, 19968, 18944, 30208, 29184,
			32256, 31232, 26112, 25088, 28160, 27136, 11008, 10496, 12032, 11520,
			8960, 8448, 9984, 9472, 15104, 14592, 16128, 15616, 13056, 12544,
			14080, 13568, 344, 328, 376, 360, 280, 264, 312, 296,
			472, 456, 504, 488, 408, 392, 440, 424, 88, 72,
			120, 104, 24, 8, 56, 40, 216, 200, 248, 232,
			152, 136, 184, 168, 1376, 1312, 1504, 1440, 1120, 1056,
			1248, 1184, 1888, 1824, 2016, 1952, 1632, 1568, 1760, 1696,
			688, 656, 752, 720, 560, 528, 624, 592, 944, 912,
			1008, 976, 816, 784, 880, 848
		};

		public static short ALawToLinearSample(byte aLaw)
		{
			return ALawDecompressTable[aLaw];
		}
	}
	public static class ALawEncoder
	{
		private const int cBias = 132;

		private const int cClip = 32635;

		private static readonly byte[] ALawCompressTable = new byte[128]
		{
			1, 1, 2, 2, 3, 3, 3, 3, 4, 4,
			4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7
		};

		public static byte LinearToALawSample(short sample)
		{
			int num = (~sample >> 8) & 0x80;
			if (num == 0)
			{
				sample = (short)(-sample);
			}
			if (sample > 32635)
			{
				sample = 32635;
			}
			byte b;
			if (sample >= 256)
			{
				int num2 = ALawCompressTable[(sample >> 8) & 0x7F];
				int num3 = (sample >> num2 + 3) & 0xF;
				b = (byte)((num2 << 4) | num3);
			}
			else
			{
				b = (byte)(sample >> 4);
			}
			return (byte)(b ^ (byte)((uint)num ^ 0x55u));
		}
	}
	public class G722Codec
	{
		private static readonly int[] wl = new int[8] { -60, -30, 58, 172, 334, 538, 1198, 3042 };

		private static readonly int[] rl42 = new int[16]
		{
			0, 7, 6, 5, 4, 3, 2, 1, 7, 6,
			5, 4, 3, 2, 1, 0
		};

		private static readonly int[] ilb = new int[32]
		{
			2048, 2093, 2139, 2186, 2233, 2282, 2332, 2383, 2435, 2489,
			2543, 2599, 2656, 2714, 2774, 2834, 2896, 2960, 3025, 3091,
			3158, 3228, 3298, 3371, 3444, 3520, 3597, 3676, 3756, 3838,
			3922, 4008
		};

		private static readonly int[] wh = new int[3] { 0, -214, 798 };

		private static readonly int[] rh2 = new int[4] { 2, 1, 2, 1 };

		private static readonly int[] qm2 = new int[4] { -7408, -1616, 7408, 1616 };

		private static readonly int[] qm4 = new int[16]
		{
			0, -20456, -12896, -8968, -6288, -4240, -2584, -1200, 20456, 12896,
			8968, 6288, 4240, 2584, 1200, 0
		};

		private static readonly int[] qm5 = new int[32]
		{
			-280, -280, -23352, -17560, -14120, -11664, -9752, -8184, -6864, -5712,
			-4696, -3784, -2960, -2208, -1520, -880, 23352, 17560, 14120, 11664,
			9752, 8184, 6864, 5712, 4696, 3784, 2960, 2208, 1520, 880,
			280, -280
		};

		private static readonly int[] qm6 = new int[64]
		{
			-136, -136, -136, -136, -24808, -21904, -19008, -16704, -14984, -13512,
			-12280, -11192, -10232, -9360, -8576, -7856, -7192, -6576, -6000, -5456,
			-4944, -4464, -4008, -3576, -3168, -2776, -2400, -2032, -1688, -1360,
			-1040, -728, 24808, 21904, 19008, 16704, 14984, 13512, 12280, 11192,
			10232, 9360, 8576, 7856, 7192, 6576, 6000, 5456, 4944, 4464,
			4008, 3576, 3168, 2776, 2400, 2032, 1688, 1360, 1040, 728,
			432, 136, -432, -136
		};

		private static readonly int[] qmf_coeffs = new int[12]
		{
			3, -11, 12, 32, -210, 951, 3876, -805, 362, -156,
			53, -11
		};

		private static readonly int[] q6 = new int[32]
		{
			0, 35, 72, 110, 150, 190, 233, 276, 323, 370,
			422, 473, 530, 587, 650, 714, 786, 858, 940, 1023,
			1121, 1219, 1339, 1458, 1612, 1765, 1980, 2195, 2557, 2919,
			0, 0
		};

		private static readonly int[] iln = new int[32]
		{
			0, 63, 62, 31, 30, 29, 28, 27, 26, 25,
			24, 23, 22, 21, 20, 19, 18, 17, 16, 15,
			14, 13, 12, 11, 10, 9, 8, 7, 6, 5,
			4, 0
		};

		private static readonly int[] ilp = new int[32]
		{
			0, 61, 60, 59, 58, 57, 56, 55, 54, 53,
			52, 51, 50, 49, 48, 47, 46, 45, 44, 43,
			42, 41, 40, 39, 38, 37, 36, 35, 34, 33,
			32, 0
		};

		private static readonly int[] ihn = new int[3] { 0, 1, 0 };

		private static readonly int[] ihp = new int[3] { 0, 3, 2 };

		private static short Saturate(int amp)
		{
			short num = (short)amp;
			if (amp == num)
			{
				return num;
			}
			if (amp > 32767)
			{
				return short.MaxValue;
			}
			return short.MinValue;
		}

		private static void Block4(G722CodecState s, int band, int d)
		{
			s.Band[band].d[0] = d;
			s.Band[band].r[0] = Saturate(s.Band[band].s + d);
			s.Band[band].p[0] = Saturate(s.Band[band].sz + d);
			for (int i = 0; i < 3; i++)
			{
				s.Band[band].sg[i] = s.Band[band].p[i] >> 15;
			}
			int num = Saturate(s.Band[band].a[1] << 2);
			int num2 = ((s.Band[band].sg[0] == s.Band[band].sg[1]) ? (-num) : num);
			if (num2 > 32767)
			{
				num2 = 32767;
			}
			int num3 = ((s.Band[band].sg[0] == s.Band[band].sg[2]) ? 128 : (-128));
			num3 += num2 >> 7;
			num3 += s.Band[band].a[2] * 32512 >> 15;
			if (num3 > 12288)
			{
				num3 = 12288;
			}
			else if (num3 < -12288)
			{
				num3 = -12288;
			}
			s.Band[band].ap[2] = num3;
			s.Band[band].sg[0] = s.Band[band].p[0] >> 15;
			s.Band[band].sg[1] = s.Band[band].p[1] >> 15;
			num = ((s.Band[band].sg[0] == s.Band[band].sg[1]) ? 192 : (-192));
			num2 = s.Band[band].a[1] * 32640 >> 15;
			s.Band[band].ap[1] = Saturate(num + num2);
			num3 = Saturate(15360 - s.Band[band].ap[2]);
			if (s.Band[band].ap[1] > num3)
			{
				s.Band[band].ap[1] = num3;
			}
			else if (s.Band[band].ap[1] < -num3)
			{
				s.Band[band].ap[1] = -num3;
			}
			num = ((d != 0) ? 128 : 0);
			s.Band[band].sg[0] = d >> 15;
			for (int i = 1; i < 7; i++)
			{
				s.Band[band].sg[i] = s.Band[band].d[i] >> 15;
				num2 = ((s.Band[band].sg[i] == s.Band[band].sg[0]) ? num : (-num));
				num3 = s.Band[band].b[i] * 32640 >> 15;
				s.Band[band].bp[i] = Saturate(num2 + num3);
			}
			for (int i = 6; i > 0; i--)
			{
				s.Band[band].d[i] = s.Band[band].d[i - 1];
				s.Band[band].b[i] = s.Band[band].bp[i];
			}
			for (int i = 2; i > 0; i--)
			{
				s.Band[band].r[i] = s.Band[band].r[i - 1];
				s.Band[band].p[i] = s.Band[band].p[i - 1];
				s.Band[band].a[i] = s.Band[band].ap[i];
			}
			num = Saturate(s.Band[band].r[1] + s.Band[band].r[1]);
			num = s.Band[band].a[1] * num >> 15;
			num2 = Saturate(s.Band[band].r[2] + s.Band[band].r[2]);
			num2 = s.Band[band].a[2] * num2 >> 15;
			s.Band[band].sp = Saturate(num + num2);
			s.Band[band].sz = 0;
			for (int i = 6; i > 0; i--)
			{
				num = Saturate(s.Band[band].d[i] + s.Band[band].d[i]);
				s.Band[band].sz += s.Band[band].b[i] * num >> 15;
			}
			s.Band[band].sz = Saturate(s.Band[band].sz);
			s.Band[band].s = Saturate(s.Band[band].sp + s.Band[band].sz);
		}

		public int Decode(G722CodecState state, short[] outputBuffer, byte[] inputG722Data, int inputLength)
		{
			int result = 0;
			int num = 0;
			int num2 = 0;
			while (num2 < inputLength)
			{
				int num3;
				if (state.Packed)
				{
					if (state.InBits < state.BitsPerSample)
					{
						state.InBuffer |= (uint)(inputG722Data[num2++] << state.InBits);
						state.InBits += 8;
					}
					num3 = (int)state.InBuffer & ((1 << state.BitsPerSample) - 1);
					state.InBuffer >>= state.BitsPerSample;
					state.InBits -= state.BitsPerSample;
				}
				else
				{
					num3 = inputG722Data[num2++];
				}
				int num5;
				int num4;
				int num6;
				switch (state.BitsPerSample)
				{
				default:
					num4 = num3 & 0x3F;
					num5 = (num3 >> 6) & 3;
					num6 = qm6[num4];
					num4 >>= 2;
					break;
				case 7:
					num4 = num3 & 0x1F;
					num5 = (num3 >> 5) & 3;
					num6 = qm5[num4];
					num4 >>= 1;
					break;
				case 6:
					num4 = num3 & 0xF;
					num5 = (num3 >> 4) & 3;
					num6 = qm4[num4];
					break;
				}
				num6 = state.Band[0].det * num6 >> 15;
				int num7 = state.Band[0].s + num6;
				if (num7 > 16383)
				{
					num7 = 16383;
				}
				else if (num7 < -16384)
				{
					num7 = -16384;
				}
				num6 = qm4[num4];
				int d = state.Band[0].det * num6 >> 15;
				num6 = rl42[num4];
				num4 = state.Band[0].nb * 127 >> 7;
				num4 += wl[num6];
				if (num4 < 0)
				{
					num4 = 0;
				}
				else if (num4 > 18432)
				{
					num4 = 18432;
				}
				state.Band[0].nb = num4;
				num4 = (state.Band[0].nb >> 6) & 0x1F;
				num6 = 8 - (state.Band[0].nb >> 11);
				int num8 = ((num6 < 0) ? (ilb[num4] << -num6) : (ilb[num4] >> num6));
				state.Band[0].det = num8 << 2;
				Block4(state, 0, d);
				if (!state.EncodeFrom8000Hz)
				{
					num6 = qm2[num5];
					int num9 = state.Band[1].det * num6 >> 15;
					num = num9 + state.Band[1].s;
					if (num > 16383)
					{
						num = 16383;
					}
					else if (num < -16384)
					{
						num = -16384;
					}
					num6 = rh2[num5];
					num4 = state.Band[1].nb * 127 >> 7;
					num4 += wh[num6];
					if (num4 < 0)
					{
						num4 = 0;
					}
					else if (num4 > 22528)
					{
						num4 = 22528;
					}
					state.Band[1].nb = num4;
					num4 = (state.Band[1].nb >> 6) & 0x1F;
					num6 = 10 - (state.Band[1].nb >> 11);
					num8 = ((num6 < 0) ? (ilb[num4] << -num6) : (ilb[num4] >> num6));
					state.Band[1].det = num8 << 2;
					Block4(state, 1, num9);
				}
				if (state.ItuTestMode)
				{
					outputBuffer[result++] = (short)(num7 << 1);
					outputBuffer[result++] = (short)(num << 1);
					continue;
				}
				if (state.EncodeFrom8000Hz)
				{
					outputBuffer[result++] = (short)(num7 << 1);
					continue;
				}
				for (int i = 0; i < 22; i++)
				{
					state.QmfSignalHistory[i] = state.QmfSignalHistory[i + 2];
				}
				state.QmfSignalHistory[22] = num7 + num;
				state.QmfSignalHistory[23] = num7 - num;
				int num10 = 0;
				int num11 = 0;
				for (int i = 0; i < 12; i++)
				{
					num11 += state.QmfSignalHistory[2 * i] * qmf_coeffs[i];
					num10 += state.QmfSignalHistory[2 * i + 1] * qmf_coeffs[11 - i];
				}
				outputBuffer[result++] = (short)(num10 >> 11);
				outputBuffer[result++] = (short)(num11 >> 11);
			}
			return result;
		}

		public int Encode(G722CodecState state, byte[] outputBuffer, short[] inputBuffer, int inputBufferCount)
		{
			int result = 0;
			int num = 0;
			int num2 = 0;
			while (num2 < inputBufferCount)
			{
				int num3;
				int i;
				if (state.ItuTestMode)
				{
					num3 = (num = inputBuffer[num2++] >> 1);
				}
				else if (state.EncodeFrom8000Hz)
				{
					num3 = inputBuffer[num2++] >> 1;
				}
				else
				{
					for (i = 0; i < 22; i++)
					{
						state.QmfSignalHistory[i] = state.QmfSignalHistory[i + 2];
					}
					state.QmfSignalHistory[22] = inputBuffer[num2++];
					state.QmfSignalHistory[23] = inputBuffer[num2++];
					int num4 = 0;
					int num5 = 0;
					for (i = 0; i < 12; i++)
					{
						num5 += state.QmfSignalHistory[2 * i] * qmf_coeffs[i];
						num4 += state.QmfSignalHistory[2 * i + 1] * qmf_coeffs[11 - i];
					}
					num3 = num4 + num5 >> 14;
					num = num4 - num5 >> 14;
				}
				int num6 = Saturate(num3 - state.Band[0].s);
				int num7 = ((num6 >= 0) ? num6 : (-(num6 + 1)));
				int num8;
				for (i = 1; i < 30; i++)
				{
					num8 = q6[i] * state.Band[0].det >> 12;
					if (num7 < num8)
					{
						break;
					}
				}
				int num9 = ((num6 < 0) ? iln[i] : ilp[i]);
				int num10 = num9 >> 2;
				int num11 = qm4[num10];
				int d = state.Band[0].det * num11 >> 15;
				int num12 = rl42[num10];
				num7 = state.Band[0].nb * 127 >> 7;
				state.Band[0].nb = num7 + wl[num12];
				if (state.Band[0].nb < 0)
				{
					state.Band[0].nb = 0;
				}
				else if (state.Band[0].nb > 18432)
				{
					state.Band[0].nb = 18432;
				}
				num8 = (state.Band[0].nb >> 6) & 0x1F;
				num11 = 8 - (state.Band[0].nb >> 11);
				int num13 = ((num11 < 0) ? (ilb[num8] << -num11) : (ilb[num8] >> num11));
				state.Band[0].det = num13 << 2;
				Block4(state, 0, d);
				int num14;
				if (state.EncodeFrom8000Hz)
				{
					num14 = (0xC0 | num9) >> 8 - state.BitsPerSample;
				}
				else
				{
					int num15 = Saturate(num - state.Band[1].s);
					num7 = ((num15 >= 0) ? num15 : (-(num15 + 1)));
					num8 = 564 * state.Band[1].det >> 12;
					int num16 = ((num7 < num8) ? 1 : 2);
					int num17 = ((num15 < 0) ? ihn[num16] : ihp[num16]);
					num11 = qm2[num17];
					int d2 = state.Band[1].det * num11 >> 15;
					int num18 = rh2[num17];
					num7 = state.Band[1].nb * 127 >> 7;
					state.Band[1].nb = num7 + wh[num18];
					if (state.Band[1].nb < 0)
					{
						state.Band[1].nb = 0;
					}
					else if (state.Band[1].nb > 22528)
					{
						state.Band[1].nb = 22528;
					}
					num8 = (state.Band[1].nb >> 6) & 0x1F;
					num11 = 10 - (state.Band[1].nb >> 11);
					num13 = ((num11 < 0) ? (ilb[num8] << -num11) : (ilb[num8] >> num11));
					state.Band[1].det = num13 << 2;
					Block4(state, 1, d2);
					num14 = ((num17 << 6) | num9) >> 8 - state.BitsPerSample;
				}
				if (state.Packed)
				{
					state.OutBuffer |= (uint)(num14 << state.OutBits);
					state.OutBits += state.BitsPerSample;
					if (state.OutBits >= 8)
					{
						outputBuffer[result++] = (byte)(state.OutBuffer & 0xFFu);
						state.OutBits -= 8;
						state.OutBuffer >>= 8;
					}
				}
				else
				{
					outputBuffer[result++] = (byte)num14;
				}
			}
			return result;
		}
	}
	public class G722CodecState
	{
		public bool ItuTestMode { get; set; }

		public bool Packed { get; private set; }

		public bool EncodeFrom8000Hz { get; private set; }

		public int BitsPerSample { get; private set; }

		public int[] QmfSignalHistory { get; private set; }

		public Band[] Band { get; private set; }

		public uint InBuffer { get; internal set; }

		public int InBits { get; internal set; }

		public uint OutBuffer { get; internal set; }

		public int OutBits { get; internal set; }

		public G722CodecState(int rate, G722Flags options)
		{
			Band = new Band[2]
			{
				new Band(),
				new Band()
			};
			QmfSignalHistory = new int[24];
			ItuTestMode = false;
			switch (rate)
			{
			case 48000:
				BitsPerSample = 6;
				break;
			case 56000:
				BitsPerSample = 7;
				break;
			case 64000:
				BitsPerSample = 8;
				break;
			default:
				throw new ArgumentException("Invalid rate, should be 48000, 56000 or 64000");
			}
			if ((options & G722Flags.SampleRate8000) == G722Flags.SampleRate8000)
			{
				EncodeFrom8000Hz = true;
			}
			if ((options & G722Flags.Packed) == G722Flags.Packed && BitsPerSample != 8)
			{
				Packed = true;
			}
			else
			{
				Packed = false;
			}
			Band[0].det = 32;
			Band[1].det = 8;
		}
	}
	public class Band
	{
		public int s;

		public int sp;

		public int sz;

		public int[] r = new int[3];

		public int[] a = new int[3];

		public int[] ap = new int[3];

		public int[] p = new int[3];

		public int[] d = new int[7];

		public int[] b = new int[7];

		public int[] bp = new int[7];

		public int[] sg = new int[7];

		public int nb;

		public int det;
	}
	[Flags]
	public enum G722Flags
	{
		None = 0,
		SampleRate8000 = 1,
		Packed = 2
	}
	public static class MuLawDecoder
	{
		private static readonly short[] MuLawDecompressTable = new short[256]
		{
			-32124, -31100, -30076, -29052, -28028, -27004, -25980, -24956, -23932, -22908,
			-21884, -20860, -19836, -18812, -17788, -16764, -15996, -15484, -14972, -14460,
			-13948, -13436, -12924, -12412, -11900, -11388, -10876, -10364, -9852, -9340,
			-8828, -8316, -7932, -7676, -7420, -7164, -6908, -6652, -6396, -6140,
			-5884, -5628, -5372, -5116, -4860, -4604, -4348, -4092, -3900, -3772,
			-3644, -3516, -3388, -3260, -3132, -3004, -2876, -2748, -2620, -2492,
			-2364, -2236, -2108, -1980, -1884, -1820, -1756, -1692, -1628, -1564,
			-1500, -1436, -1372, -1308, -1244, -1180, -1116, -1052, -988, -924,
			-876, -844, -812, -780, -748, -716, -684, -652, -620, -588,
			-556, -524, -492, -460, -428, -396, -372, -356, -340, -324,
			-308, -292, -276, -260, -244, -228, -212, -196, -180, -164,
			-148, -132, -120, -112, -104, -96, -88, -80, -72, -64,
			-56, -48, -40, -32, -24, -16, -8, -1, 32124, 31100,
			30076, 29052, 28028, 27004, 25980, 24956, 23932, 22908, 21884, 20860,
			19836, 18812, 17788, 16764, 15996, 15484, 14972, 14460, 13948, 13436,
			12924, 12412, 11900, 11388, 10876, 10364, 9852, 9340, 8828, 8316,
			7932, 7676, 7420, 7164, 6908, 6652, 6396, 6140, 5884, 5628,
			5372, 5116, 4860, 4604, 4348, 4092, 3900, 3772, 3644, 3516,
			3388, 3260, 3132, 3004, 2876, 2748, 2620, 2492, 2364, 2236,
			2108, 1980, 1884, 1820, 1756, 1692, 1628, 1564, 1500, 1436,
			1372, 1308, 1244, 1180, 1116, 1052, 988, 924, 876, 844,
			812, 780, 748, 716, 684, 652, 620, 588, 556, 524,
			492, 460, 428, 396, 372, 356, 340, 324, 308, 292,
			276, 260, 244, 228, 212, 196, 180, 164, 148, 132,
			120, 112, 104, 96, 88, 80, 72, 64, 56, 48,
			40, 32, 24, 16, 8, 0
		};

		public static short MuLawToLinearSample(byte muLaw)
		{
			return MuLawDecompressTable[muLaw];
		}
	}
	public static class MuLawEncoder
	{
		private const int cBias = 132;

		private const int cClip = 32635;

		private static readonly byte[] MuLawCompressTable = new byte[256]
		{
			0, 0, 1, 1, 2, 2, 2, 2, 3, 3,
			3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
			4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
			4, 4, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7
		};

		public static byte LinearToMuLawSample(short sample)
		{
			int num = (sample >> 8) & 0x80;
			if (num != 0)
			{
				sample = (short)(-sample);
			}
			if (sample > 32635)
			{
				sample = 32635;
			}
			sample = (short)(sample + 132);
			int num2 = MuLawCompressTable[(sample >> 7) & 0xFF];
			int num3 = (sample >> num2 + 3) & 0xF;
			return (byte)(~(num | (num2 << 4) | num3));
		}
	}
}
