#!/usr/bin/env python3

import os
import glob
import subprocess

def get_files(base):
    return glob.glob(os.path.join(base, "Assets/**/*.cs"), recursive=True)

def clang_format(files):
    return subprocess.run(["clang-format", "-style=file", "-i", *files], check=True)

if __name__ == "__main__":
    base = os.path.dirname(os.path.realpath(__file__))
    files = get_files(base)
    clang_format(files)

