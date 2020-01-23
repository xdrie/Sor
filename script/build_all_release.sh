set -e

ARGS=""
if [ -n "$1" ] && [ "$1" = "pack" ]; then
    echo "setting PACK on"
    ARGS="${ARGS} pack"
fi

echo "building all (${ARGS})..."
./script/build.sh
./script/get_natives.sh
./script/build_mono_release.sh
./script/build_native_release.sh linux-x64 ${ARGS}
./script/build_native_release.sh osx-x64 ${ARGS}
./script/build_native_release.sh win-x64 ${ARGS}