#!/usr/bin/env bash

set -e

pushd .

PROJECT=Sor
FRAMEWORK=net472
PROJECT_PATH=./src/$PROJECT/${PROJECT}Dk
NATIVES_PATH=$(pwd)/natives
DEBUG=0
if [ -n "$1" ] && [ "$1" = "debug" ]; then
    echo "setting DEBUG=1"
    DEBUG=1
fi
ARCTYPE=7z
if [ -n "$2" ]; then
    echo "setting ARCTYPE=$2"
    ARCTYPE=$2
fi

PARSE_VERSION=$(grep 'GAME_VERSION' ./src/$PROJECT/$PROJECT/NGame.cs | cut -d \" -f2)
REVISION=$(git tag -l --points-at HEAD)
GIT_REVISION=$(git rev-parse --short HEAD)
if [ -z "${REVISION}" ]; then
    REVISION="${PARSE_VERSION}_${GIT_REVISION}"
fi
ARCNAME="${PROJECT}_mono-v$REVISION"

CONFIGURATION=Release
if [[ $DEBUG -eq 1 ]];
then
CONFIGURATION=Debug
ARCNAME="${ARCNAME}_dbg"
fi

ARTIFACT="builds/$ARCNAME.$ARCTYPE"

cd src/$PROJECT
echo "running restore..."
dotnet restore $PROJECT.Mono.sln
echo "running build..."
msbuild $PROJECT.Mono.sln /p:Configuration=$CONFIGURATION

popd

OUTPATH=$PROJECT_PATH/bin/$CONFIGURATION/$FRAMEWORK
echo "copying natives..."
cp $NATIVES_PATH/* $OUTPATH/
cp script/${PROJECT}_game $OUTPATH/
chmod +x $OUTPATH/${PROJECT}_game

mkdir -p builds/
echo "compressing to $ARTIFACT..."
7z a $ARTIFACT $OUTPATH/*

echo "release built!"
