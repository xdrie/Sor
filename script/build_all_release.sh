#!/usr/bin/env bash

set -e

ARGS=""
PACK=0
if [ -n "$1" ] && [ "$1" = "pack" ]; then
    echo "setting PACK=1"
    PACK=1
    ARGS="${ARGS} pack"
fi

if [[ $PACK -eq 1 ]];
then
    echo "getting tools..."
    ./script/get_tools.sh
fi

echo "building all (${ARGS})..."
./script/build.sh
./script/get_natives.sh
# ./script/build_mono_release.sh
./script/build_native_release.sh linux-x64 ${ARGS}
./script/build_native_release.sh osx-x64 ${ARGS}
./script/build_native_release.sh win-x64 ${ARGS}