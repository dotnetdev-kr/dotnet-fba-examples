#!/usr/bin/env python3

import ctypes
import os
from pathlib import Path

# dylib 파일의 절대 경로 설정
script_dir = Path(__file__).parent
dylib_path = script_dir / "artifacts" / "05-native-aot" / "05-native-aot.dylib"

# dylib 로드
lib = ctypes.CDLL(str(dylib_path))

# mymethod 함수 설정
# void mymethod(nint ptrText)
mymethod = lib.mymethod
mymethod.argtypes = [ctypes.c_void_p]
mymethod.restype = None

# 유니코드 문자열을 UTF-16으로 인코딩하여 포인터로 전달
def call_mymethod(text: str):
    # UTF-16 LE (Little Endian) 인코딩 + null terminator
    encoded = text.encode('utf-16-le') + b'\x00\x00'
    buffer = ctypes.create_string_buffer(encoded)
    mymethod(ctypes.cast(buffer, ctypes.c_void_p))

if __name__ == "__main__":
    call_mymethod("Hello from Python!")
    call_mymethod("Native AOT 호출 테스트")
    call_mymethod("안녕하세요!")
