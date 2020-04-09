#!/usr/bin/env bash

set -e

pushd .

# external tool settings
export CppCompilerAndLinker=clang

# arguments
FRAMEWORK=netcoreapp3.1
TARGET=$1
PACK=0
if [ -n "$2" ] && [ "$2" = "pack" ]; then
    echo "setting PACK=1"
    PACK=1
    PROPS="$PROPS /p:PackBinary=1"
fi

# platform
PROJECT=Sor
PROJECT_RUNNER=${PROJECT}Dk
PROJECT_DIR=src/$PROJECT/$PROJECT_RUNNER
BIN_NAME=$PROJECT_RUNNER
BINARY=$BIN_NAME
if [[ $TARGET == win* ]];
then
    BINARY=$BIN_NAME.exe
    ARCTYPE="7z"
else
    BINARY=$BIN_NAME
    ARCTYPE="tar.xz"
fi

# tool options
STRIP_BINARY=0
UPX_COMPRESS=0
WARP_BIN=$(pwd)/builds/warp-packer
WARP_COMPRESS=1
WARP_ARCH=$TARGET
NATIVES_PATH=$(pwd)/natives

# correct architecture names for warp
if [[ $TARGET == win* ]];
then
    WARP_ARCH=windows-x64
fi
if [[ $TARGET == osx* ]];
then
    WARP_ARCH=macos-x64
fi

# outputs
PARSE_VERSION=$(grep 'GAME_VERSION' ./src/$PROJECT/$PROJECT/Game/Config.cs | head -1 | cut -d \" -f2)
REVISION=$(git tag -l --points-at HEAD)
GIT_REVISION=$(git rev-parse --short HEAD)
if [ -z "${REVISION}" ]; then
    REVISION="${PARSE_VERSION}_${GIT_REVISION}"
fi
ARCNAME="${PROJECT}_$TARGET-v$REVISION"
ARTIFACT="builds/$ARCNAME.$ARCTYPE"

echo "release builder script [target $TARGET/$FRAMEWORK]"
echo "ART: $ARTIFACT"
echo "REV: $REVISION"

PUBLISH_ARGS="${PROPS} -f $FRAMEWORK -r $TARGET -c Release"
echo "running native compile (${PUBLISH_ARGS})..."
cd $PROJECT_DIR
dotnet publish ${PROJECT_RUNNER}.csproj ${PUBLISH_ARGS}
PUBLISH=bin/Release/$FRAMEWORK/$TARGET/publish

echo "copying to staging directory..."
STAGING=${PUBLISH}_staging

# set binary to full path of output executable
A_BINARY="./$PROJECT_DIR/$STAGING/$BINARY"
echo "target: [$A_BINARY]"

if [[ $PACK -eq 1 ]];
then
    if [[ $WARP_COMPRESS -eq 1 ]];
    then
        mkdir -p ${STAGING}
        echo "running WARP tool..."
        $WARP_BIN --arch $WARP_ARCH --input_dir $PUBLISH --exec $BINARY --output "$STAGING/$BINARY"

        echo "copying natives..."
        cp $NATIVES_PATH/* $STAGING/
    else
        cp -r ${PUBLISH} ${STAGING}
    fi
else
    cp -r ${PUBLISH} ${STAGING}

    cd ${STAGING}

    if [[ $STRIP_BINARY -eq 1 ]];
    then
        echo "stripping binary '$BIN_NAME'..."
        strip $BIN_NAME
    fi

    echo "cleaning misc files..."
    rm -rf *.pdb

    if [[ $UPX_COMPRESS -eq 1 ]];
    then
        echo "compressing binary with UPX..."
        upx --lzma $BIN_NAME
    fi

fi

popd # return to build root

# check the binary
echo "checking output bin:"
ls -lah $A_BINARY

mkdir -p builds/
echo "compressing to $ARTIFACT..."
if [[ $ARCTYPE == "7z" ]];
then
    7z a $ARTIFACT "./$PROJECT_DIR/${STAGING}/*"
elif [[ $ARCTYPE == "tar.xz" ]];
then
    tar --transform "s/^publish_staging/$ARCNAME/" -cJvf $ARTIFACT -C "$PROJECT_DIR/bin/Release/$FRAMEWORK/$TARGET/" publish_staging
fi

echo "cleaning publish_staging..."
rm -r "$PROJECT_DIR/${STAGING}"

echo "release built!"
