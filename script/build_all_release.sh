#!/usr/bin/env bash

set -e

echo "building all releases..."

./script/build.sh

# run mono build
# ./script/get_natives.sh # optional, bundles native libraries
# build archive for mono
# FRAMEWORK=net4.8 CHANNEL=mono ./script/build_arc.sh win-x64

# run three main builds
./script/build_arc.sh linux-x64
./script/build_arc.sh osx-x64
./script/build_arc.sh win-x64
