set -e

pushd .

FRAMEWORK=net472

REVISION=$(git log --pretty=format:'%h' -n 1)
ARCNAME="sor_mono-$REVISION"
ARTIFACT="builds/$ARCNAME.7z"

echo "Running MSBuild..."
cd src/Sor
msbuild Sor.sln /p:Configuration=Release

popd

echo "Copying natives..."
cp natives/* src/Sor/SorDk/bin/Release/net471/

mkdir -p builds/
echo "Compressing to $ARTIFACT..."
7z a $ARTIFACT ./src/Sor/SorDk/bin/Release/net471/*

echo "Release built!"
