import os
import subprocess
from dataclasses import dataclass
from typing import Iterable, List, Sequence, Tuple

NUM_LEDS = 75
COLOR_BYTES = NUM_LEDS * 3
DEFAULT_SOURCE_REVISION = "4f8511e68545bc071b36a32f4a48f0de181947a9^"
DEFAULT_HANDSHAKE_FILE = "surely_full_red.csv"
DEFAULT_GREEN_FILE = "green.csv"


@dataclass(frozen=True)
class RawGitReplayCapture:
    repo_path: str
    revision: str
    handshake_file: str
    green_file: str
    handshake_writes: List[bytes]
    green_writes: List[bytes]


class RawGitReplayProtocol:
    def __init__(
        self,
        repo_path: str,
        revision: str = DEFAULT_SOURCE_REVISION,
        handshake_file: str = DEFAULT_HANDSHAKE_FILE,
        green_file: str = DEFAULT_GREEN_FILE,
    ):
        self._repo_path = os.path.abspath(repo_path)
        self._revision = revision
        self._handshake_file = handshake_file
        self._green_file = green_file
        self._capture = self._load_capture()

    @property
    def capture(self) -> RawGitReplayCapture:
        return self._capture

    def iter_handshake_writes(self) -> Iterable[bytes]:
        return list(self._capture.handshake_writes)

    def iter_green_replay_writes(self, limit: int | None = None) -> Iterable[bytes]:
        writes = self._capture.green_writes
        if limit is not None:
            writes = writes[: max(0, int(limit))]
        for raw in writes:
            if len(raw) == 5 or self.is_color_packet(raw):
                yield raw

    def iter_color_replay_writes(
        self,
        colors_rgb: Sequence[Tuple[int, int, int]],
        limit: int | None = None,
    ) -> Iterable[bytes]:
        writes = self._capture.green_writes
        if limit is not None:
            writes = writes[: max(0, int(limit))]
        target = self._normalize_colors(colors_rgb)
        for raw in writes:
            if len(raw) == 5:
                yield raw
            elif self.is_color_packet(raw):
                yield self._apply_colors_to_green_packet(raw, target)

    @staticmethod
    def is_color_packet(raw: bytes) -> bool:
        return 239 <= len(raw) <= 245

    def _load_capture(self) -> RawGitReplayCapture:
        handshake_csv = self._load_deleted_file_from_git(self._handshake_file)
        green_csv = self._load_deleted_file_from_git(self._green_file)
        handshake_writes = self._parse_writes_from_csv_text(handshake_csv)[:114]
        green_writes = self._parse_writes_from_csv_text(green_csv)
        return RawGitReplayCapture(
            repo_path=self._repo_path,
            revision=self._revision,
            handshake_file=self._handshake_file,
            green_file=self._green_file,
            handshake_writes=handshake_writes,
            green_writes=green_writes,
        )

    def _load_deleted_file_from_git(self, file_path: str) -> str:
        spec = f"{self._revision}:{file_path}"
        result = subprocess.run(
            ["git", "show", spec],
            cwd=self._repo_path,
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
            check=True,
        )
        return result.stdout

    @staticmethod
    def _parse_writes_from_csv_text(text: str) -> List[bytes]:
        writes: List[bytes] = []
        for line in text.splitlines():
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append(raw)
        return writes

    @staticmethod
    def _normalize_colors(colors_rgb: Sequence[Tuple[int, int, int]]) -> List[Tuple[int, int, int]]:
        out: List[Tuple[int, int, int]] = []
        for idx in range(NUM_LEDS):
            if idx < len(colors_rgb):
                r, g, b = colors_rgb[idx]
            else:
                r, g, b = (0, 0, 0)
            out.append((
                max(0, min(255, int(r))),
                max(0, min(255, int(g))),
                max(0, min(255, int(b))),
            ))
        return out

    @staticmethod
    def _apply_colors_to_green_packet(raw: bytes, colors_rgb: Sequence[Tuple[int, int, int]]) -> bytes:
        m = bytearray(raw)
        color_start = len(m) - COLOR_BYTES
        for led, (r, g, b) in enumerate(colors_rgb[:NUM_LEDS]):
            base = color_start + led * 3
            if base + 2 >= len(m):
                break
            m[base + 0] ^= r
            m[base + 1] ^= 255 ^ g
            m[base + 2] ^= b
        return bytes(m)
