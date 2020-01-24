set -e

pushd .

PROJECT=Sor
FRAMEWORK=net472
PROJECT_PATH=./src/$PROJECT/${PROJECT}Dk
NATIVES_PATH=$(pwd)/natives

PARSE_VERSION=$(grep 'GAME_VERSION' ./src/$PROJECT/$PROJECT/NGame.cs | cut -d \" -f2)
REVISION=$(git tag -l --points-at HEAD)
GIT_REVISION=$(git rev-parse --short HEAD)
if [ -z "${REVISION}" ]; then
    REVISION="${PARSE_VERSION}_${GIT_REVISION}"
fi
ARCNAME="${PROJECT}_$TARGET-v$REVISION"
ARTIFACT="builds/$ARCNAME.7z"

cd src/$PROJECT
echo "running restore..."
dotnet restore
echo "running build..."
msbuild $PROJECT.sln /p:Configuration=Release

popd

OUTPATH=$PROJECT_PATH/bin/Release/$FRAMEWORK
echo "copying natives..."
cp $NATIVES_PATH/* $OUTPATH/
cp script/${PROJECT}_game $OUTPATH/
chmod +x $OUTPATH/${PROJECT}_game

mkdir -p builds/
echo "compressing to $ARTIFACT..."
7z a $ARTIFACT $OUTPATH/*

echo "release built!"
