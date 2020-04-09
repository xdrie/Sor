#!/usr/bin/env bash

set -e

echo "building all releases..."

./script/build.sh

# run three main builds
./script/build_native.sh linux-x64
./script/build_native.sh osx-x64
./script/build_native.sh win-x64