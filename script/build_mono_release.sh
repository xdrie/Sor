set -e

pushd .

PROJECT=Sor
FRAMEWORK=net472
PROJECT_PATH=./src/$PROJECT/${PROJECT}Dk

REVISION=$(git log --pretty=format:'%h' -n 1)
ARCNAME="sor_mono-$REVISION"
ARTIFACT="builds/$ARCNAME.7z"

echo "Running MSBuild..."
cd src/$PROJECT
msbuild $PROJECT.sln /p:Configuration=Release

popd

echo "Copying natives..."
cp natives/* $PROJECT_PATH/bin/Release/$FRAMEWORK/

mkdir -p builds/
echo "Compressing to $ARTIFACT..."
7z a $ARTIFACT $PROJECT_PATH/bin/Release/$FRAMEWORK/*

echo "Release built!"
