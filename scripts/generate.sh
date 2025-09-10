#!/usr/bin/env bash
set -euo pipefail
pip install -r requirements.txt
python genai/generate_testcases.py
