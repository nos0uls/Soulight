# profile_mirroring.py — Профилирование screen mirroring pipeline
#
# Этот скрипт измеряет время каждого этапа обработки кадра,
# чтобы найти узкие места и понять, где теряется CPU.

import sys
import time
import numpy as np

sys.path.insert(0, ".")

from soulight.led_config import LEDConfig, SIDE_TOP, SIDE_BOTTOM, SIDE_LEFT, SIDE_RIGHT
from soulight.screen_mirroring.layout import build_layout
from soulight.screen_mirroring.screen_capture import ScreenCapturer
from soulight.screen_mirroring.sampler import sample_frame, FrameSmoother


def profile_capture_path(iterations=50):
    """
    Профилирует полный путь capture → sampling → smoothing.
    Измеряет каждый этап отдельно.
    """
    print(f"\n=== Profiling {iterations} iterations ===\n")
    
    # Настраиваем типичную конфигурацию
    cfg = LEDConfig()
    cfg.counts[SIDE_TOP] = 20
    cfg.counts[SIDE_BOTTOM] = 20
    cfg.counts[SIDE_LEFT] = 15
    cfg.counts[SIDE_RIGHT] = 15
    cfg._rebuild_enabled()
    
    capturer = ScreenCapturer(monitor_index=1)
    geometry = capturer.get_monitor_geometry()
    
    print(f"Monitor: {geometry.width}x{geometry.height}")
    print(f"LEDs: {cfg.total}")
    
    # Строим layout для разных edge depths
    for edge_pct in [6, 8, 12]:
        edge_fraction = edge_pct / 100.0
        layout = build_layout(
            config=cfg,
            capture_width=geometry.width,
            capture_height=geometry.height,
            edge_fraction=edge_fraction,
        )
        
        print(f"\n--- Edge depth: {edge_pct}% ({layout.edge_depth}px) ---")
        
        smoother = FrameSmoother(factor=0.35)
        
        times_capture = []
        times_sample = []
        times_smooth = []
        times_total = []
        
        # Прогреваем кеши
        for _ in range(5):
            frame = capturer.capture_edges(edge_depth=layout.edge_depth)
            sample_frame(frame, layout, smoother, saturation_boost=1.3)
        
        # Реальные замеры
        for i in range(iterations):
            t0 = time.perf_counter()
            
            # 1. Capture
            t_cap_start = time.perf_counter()
            frame = capturer.capture_edges(edge_depth=layout.edge_depth)
            t_cap_end = time.perf_counter()
            
            # 2. Sampling (без smoothing)
            t_sample_start = time.perf_counter()
            sampled = sample_frame(frame, layout, smoother=None, saturation_boost=1.3)
            t_sample_end = time.perf_counter()
            
            # 3. Smoothing
            t_smooth_start = time.perf_counter()
            smoothed_colors = smoother.apply(sampled.logical_colors)
            t_smooth_end = time.perf_counter()
            
            t1 = time.perf_counter()
            
            times_capture.append((t_cap_end - t_cap_start) * 1000)
            times_sample.append((t_sample_end - t_sample_start) * 1000)
            times_smooth.append((t_smooth_end - t_smooth_start) * 1000)
            times_total.append((t1 - t0) * 1000)
        
        # Статистика
        def stats(times):
            arr = np.array(times)
            return {
                "min": np.min(arr),
                "max": np.max(arr),
                "mean": np.mean(arr),
                "median": np.median(arr),
                "p95": np.percentile(arr, 95),
            }
        
        cap_stats = stats(times_capture)
        sample_stats = stats(times_sample)
        smooth_stats = stats(times_smooth)
        total_stats = stats(times_total)
        
        print(f"Capture:  mean={cap_stats['mean']:.2f}ms  p95={cap_stats['p95']:.2f}ms  max={cap_stats['max']:.2f}ms")
        print(f"Sample:   mean={sample_stats['mean']:.2f}ms  p95={sample_stats['p95']:.2f}ms  max={sample_stats['max']:.2f}ms")
        print(f"Smooth:   mean={smooth_stats['mean']:.2f}ms  p95={smooth_stats['p95']:.2f}ms  max={smooth_stats['max']:.2f}ms")
        print(f"Total:    mean={total_stats['mean']:.2f}ms  p95={total_stats['p95']:.2f}ms  max={total_stats['max']:.2f}ms")
        print(f"Implied max FPS: {1000.0 / total_stats['mean']:.1f}")
        
        # Процентное распределение
        cap_pct = (cap_stats['mean'] / total_stats['mean']) * 100
        sample_pct = (sample_stats['mean'] / total_stats['mean']) * 100
        smooth_pct = (smooth_stats['mean'] / total_stats['mean']) * 100
        
        print(f"\nTime distribution:")
        print(f"  Capture: {cap_pct:.1f}%")
        print(f"  Sample:  {sample_pct:.1f}%")
        print(f"  Smooth:  {smooth_pct:.1f}%")
    
    capturer.close()


def analyze_mss_overhead():
    """
    Сравнивает overhead создания fresh mss() vs reuse.
    """
    import mss
    
    print("\n=== MSS overhead analysis ===\n")
    
    iterations = 100
    
    # Fresh mss() каждый раз
    times_fresh = []
    for _ in range(iterations):
        t0 = time.perf_counter()
        with mss.mss() as sct:
            monitors = sct.monitors
        t1 = time.perf_counter()
        times_fresh.append((t1 - t0) * 1000)
    
    # Reuse mss instance
    times_reuse = []
    with mss.mss() as sct:
        for _ in range(iterations):
            t0 = time.perf_counter()
            monitors = sct.monitors
            t1 = time.perf_counter()
            times_reuse.append((t1 - t0) * 1000)
    
    fresh_mean = np.mean(times_fresh)
    reuse_mean = np.mean(times_reuse)
    
    print(f"Fresh mss() per call: {fresh_mean:.3f}ms")
    print(f"Reuse mss instance:   {reuse_mean:.3f}ms")
    print(f"Overhead per frame:   {fresh_mean - reuse_mean:.3f}ms")


if __name__ == "__main__":
    try:
        analyze_mss_overhead()
        profile_capture_path(iterations=50)
    except KeyboardInterrupt:
        print("\nInterrupted by user")
    except Exception as e:
        print(f"\nError: {e}")
        import traceback
        traceback.print_exc()
