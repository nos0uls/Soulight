# test_dxcam_performance.py — Быстрый тест dxcam vs mss производительности

import sys
import time
import numpy as np

sys.path.insert(0, ".")

from soulight.screen_mirroring.screen_capture import ScreenCapturer

def test_capture_backend(iterations=30):
    """
    Сравнивает производительность dxcam vs mss для edge capture.
    """
    print("\n=== Testing DXCam vs MSS ===\n")
    
    edge_depth = 86  # 8% для 1080p
    
    # Test DXCam
    print("Testing DXCam backend...")
    capturer_dxcam = ScreenCapturer(monitor_index=1, prefer_dxcam=True)
    
    times_dxcam = []
    try:
        # Прогрев
        for _ in range(5):
            capturer_dxcam.capture_edges(edge_depth=edge_depth)
        
        # Замеры
        for i in range(iterations):
            t0 = time.perf_counter()
            frame = capturer_dxcam.capture_edges(edge_depth=edge_depth)
            t1 = time.perf_counter()
            times_dxcam.append((t1 - t0) * 1000)
        
        dxcam_mean = np.mean(times_dxcam)
        dxcam_p95 = np.percentile(times_dxcam, 95)
        dxcam_max = np.max(times_dxcam)
        
        print(f"DXCam: mean={dxcam_mean:.2f}ms  p95={dxcam_p95:.2f}ms  max={dxcam_max:.2f}ms")
        print(f"Implied max FPS: {1000.0 / dxcam_mean:.1f}")
    except Exception as e:
        print(f"DXCam failed: {e}")
        dxcam_mean = None
    finally:
        capturer_dxcam.close()
    
    # Test MSS
    print("\nTesting MSS backend...")
    capturer_mss = ScreenCapturer(monitor_index=1, prefer_dxcam=False)
    
    times_mss = []
    try:
        # Прогрев
        for _ in range(5):
            capturer_mss.capture_edges(edge_depth=edge_depth)
        
        # Замеры
        for i in range(iterations):
            t0 = time.perf_counter()
            frame = capturer_mss.capture_edges(edge_depth=edge_depth)
            t1 = time.perf_counter()
            times_mss.append((t1 - t0) * 1000)
        
        mss_mean = np.mean(times_mss)
        mss_p95 = np.percentile(times_mss, 95)
        mss_max = np.max(times_mss)
        
        print(f"MSS:   mean={mss_mean:.2f}ms  p95={mss_p95:.2f}ms  max={mss_max:.2f}ms")
        print(f"Implied max FPS: {1000.0 / mss_mean:.1f}")
    finally:
        capturer_mss.close()
    
    # Сравнение
    if dxcam_mean is not None:
        speedup = mss_mean / dxcam_mean
        print(f"\n=== DXCam is {speedup:.1f}x faster than MSS ===")
        print(f"Time saved per frame: {mss_mean - dxcam_mean:.2f}ms")
        print(f"At 14 FPS: {(mss_mean - dxcam_mean) * 14:.1f}ms saved per second")


if __name__ == "__main__":
    try:
        test_capture_backend(iterations=30)
    except KeyboardInterrupt:
        print("\nInterrupted by user")
    except Exception as e:
        print(f"\nError: {e}")
        import traceback
        traceback.print_exc()
