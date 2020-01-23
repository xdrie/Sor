set -e

pushd .

PROJECT=Sor
FRAMEWORK=net472
PROJECT_PATH=./src/$PROJECT/${PROJECT}Dk

REVISION=$(git tag -l --points-at HEAD)
if [ -z "${REVISION}" ]; then
    REVISION=$(git rev-parse --short HEAD)
fi
ARCNAME="sor_mono-$REVISION"
ARTIFACT="builds/$ARCNAME.7z"

cd src/$PROJECT
echo "running restore..."
dotnet restore
echo "running build..."
msbuild $PROJECT.sln /p:Configuration=Release

popd

OUTPATH=$PROJECT_PATH/bin/Release/$FRAMEWORK
echo "copying natives..."
cp natives/* $OUTPATH/
cp script/${PROJECT}_game $OUTPATH/
chmod +x $OUTPATH/${PROJECT}_game

mkdir -p builds/
echo "compressing to $ARTIFACT..."
7z a $ARTIFACT $OUTPATH/*

echo "release built!"
