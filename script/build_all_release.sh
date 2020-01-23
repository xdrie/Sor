set -e

./script/build.sh
./script/get_natives.sh
./script/build_mono_release.sh
./script/build_native_release.sh linux-x64
./script/build_native_release.sh osx-x64
./script/build_native_release.sh win-x64