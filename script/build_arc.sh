#!/usr/bin/env bash

set -e

pushd .

# external tool settings
export CppCompilerAndLinker=clang

# arguments
TARGET=$1 # target platform
FRAMEWORK=${FRAMEWORK:-netcoreapp3.1} # target framework
RID=${RID:-1} # pass the RID to build

if [[ -z $TARGET ]]; then
    echo "usage: ./build_native <target>"
    exit 2
fi

# platform
PROJECT=Sor
PROJECT_RUNNER=${PROJECT}Dk
PROJECT_DIR=src/$PROJECT/$PROJECT_RUNNER
BIN_NAME=$PROJECT_RUNNER
BINARY=$BIN_NAME
if [[ $TARGET == win* ]]; then
    BINARY=$BIN_NAME.exe
    ARCTYPE="7z"
else
    BINARY=$BIN_NAME
    ARCTYPE="tar.xz"
fi
NATIVES_PATH=$(pwd)/natives

# builder options
USE_CORERT=${USE_CORERT:-0}
USE_UPX=${USE_UPX:-0}
USE_STRIP=$USE_UPX

# outputs
PARSE_VERSION=$(grep 'GAME_VERSION' ./src/$PROJECT/$PROJECT/Game/Config.cs | head -1 | cut -d \" -f2)
REVISION=$(git tag -l --points-at HEAD)
GIT_REVISION=$(git rev-parse --short HEAD)
if [ -z "${REVISION}" ]; then
    REVISION="${PARSE_VERSION}_${GIT_REVISION}"
fi
CHANNEL=${CHANNEL:-$TARGET}
ARCNAME="${PROJECT}_$CHANNEL-$REVISION"
ARTIFACT_DIR="builds"
ARTIFACT="$ARTIFACT_DIR/$ARCNAME.$ARCTYPE"

mkdir -p $ARTIFACT_DIR

# banner
echo "== MGCORE[native] release builder =="
echo "================================"
echo "target: $TARGET/$FRAMEWORK"
echo "options: (USE_CORERT: $USE_CORERT) (USE_UPX: $USE_UPX)"
echo "ART: $ARTIFACT"
echo "REV: $REVISION"
echo ""

# set up build args
PROPS=""
if [[ $USE_CORERT -eq 1 ]]; then
    PROPS="/p:CoreRTMode=Default"
fi

BUILD_OPTS=""
if [[ $RID -eq 1 ]]; then
    BUILD_OPTS="$BUILD_OPTS -r $TARGET"
fi
PUBLISH_ARGS="-c Release -f $FRAMEWORK ${BUILD_OPTS} ${PROPS}"

# native compile
echo "running native compile (${PUBLISH_ARGS})..."
cd $PROJECT_DIR
dotnet publish ${PROJECT_RUNNER}.csproj ${PUBLISH_ARGS}
PUBLISH=bin/Release/$FRAMEWORK/$TARGET/publish

echo "copying to staging directory..."
STAGING=${PUBLISH}_staging

# set binary to full path of output executable
A_BINARY="./$PROJECT_DIR/$STAGING/$BINARY"
echo "target: [$A_BINARY]"

# create the staging dir
cp -r ${PUBLISH} ${STAGING}

# === Preparation
# make all necessary modifications to published files here.

cd ${STAGING}

if [[ $USE_STRIP -eq 1 ]]; then
    echo "stripping binary '$BIN_NAME'..."
    strip $BIN_NAME
fi

echo "cleaning misc files..."
rm -rf *.pdb

if [[ $USE_UPX -eq 1 ]]; then
    echo "compressing binary with UPX..."
    upx --lzma $BIN_NAME
fi

# --

popd # return to build root

STAGING_PATH="./$PROJECT_DIR/${STAGING}"

# optionally, copy natives
if [ -d "natives" ]; then
    echo "copying natives..."
    cp natives/* $STAGING_PATH/
fi

# check the binary
echo "checking output bin:"
ls -lah $A_BINARY

echo "compressing to $ARTIFACT..."
if [[ $ARCTYPE == "7z" ]]; then
    7z a $ARTIFACT "$STAGING_PATH/*"
elif [[ $ARCTYPE == "tar.xz" ]]; then
    tar --transform "s/^publish_staging/$ARCNAME/" -cJvf $ARTIFACT -C "$PROJECT_DIR/bin/Release/$FRAMEWORK/$TARGET/" publish_staging
fi

echo "cleaning publish_staging..."
rm -r "$STAGING_PATH"

echo "release built!"
