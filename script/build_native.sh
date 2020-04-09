#!/usr/bin/env bash

set -e

pushd .

# external tool settings
export CppCompilerAndLinker=clang

# arguments
FRAMEWORK=netcoreapp3.1
TARGET=$1

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
ARCNAME="${PROJECT}_$TARGET-v$REVISION"
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

PUBLISH_ARGS="-c Release -f $FRAMEWORK -r $TARGET ${PROPS}"

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

# check the binary
echo "checking output bin:"
ls -lah $A_BINARY

echo "compressing to $ARTIFACT..."
if [[ $ARCTYPE == "7z" ]]; then
    7z a $ARTIFACT "./$PROJECT_DIR/${STAGING}/*"
elif [[ $ARCTYPE == "tar.xz" ]]; then
    tar --transform "s/^publish_staging/$ARCNAME/" -cJvf $ARTIFACT -C "$PROJECT_DIR/bin/Release/$FRAMEWORK/$TARGET/" publish_staging
fi

echo "cleaning publish_staging..."
rm -r "$PROJECT_DIR/${STAGING}"

echo "release built!"
