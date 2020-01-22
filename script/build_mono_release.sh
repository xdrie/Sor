set -e

pushd .

PROJECT=Sor
FRAMEWORK=net472
PROJECT_PATH=./src/$PROJECT/${PROJECT}Dk

REVISION=$(git log --pretty=format:'%h' -n 1)
ARCNAME="sor_mono-$REVISION"
ARTIFACT="builds/$ARCNAME.7z"

echo "getting dependencies..."
git submodule update --init --recursive # update submodules

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
