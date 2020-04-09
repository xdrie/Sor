#!/usr/bin/env bash

set -e

echo "building all (${ARGS})..."
./script/build.sh
./script/build_native_release.sh linux-x64 ${ARGS}
./script/build_native_release.sh osx-x64 ${ARGS}
./script/build_native_release.sh win-x64 ${ARGS}