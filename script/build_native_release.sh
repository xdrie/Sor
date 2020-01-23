set -e

pushd .

# external tool settings
export CppCompilerAndLinker=clang

# arguments
FRAMEWORK=netcoreapp3.0
TARGET=$1

# platform
PROJECT=sor
PROJECT_DIR=src/Sor/SorDk
BIN_NAME=SorDk
BINARY=$BIN_NAME
if [[ $TARGET == win* ]];
then
    BINARY=$BIN_NAME.exe
    ARCTYPE="7z"
else
    BINARY=$BIN_NAME
    ARCTYPE="tar.xz"
fi

# options
STRIP_BINARY=0
UPX_COMPRESS=0

# outputs
REVISION=$(git log --pretty=format:'%h' -n 1)
ARCNAME="${PROJECT}_$TARGET-$REVISION"
ARTIFACT="builds/$PROJECT-$ARCNAME.$ARCTYPE"

echo "release builder script [target $TARGET/$FRAMEWORK]"
echo "ART: $ARTIFACT"
echo "REV: $REVISION"

echo "running native compile..."
cd $PROJECT_DIR
dotnet publish -f $FRAMEWORK -r $TARGET -c Release

echo "copying to staging directory..."
cp -r bin/Release/$FRAMEWORK/$TARGET/publish bin/Release/$FRAMEWORK/$TARGET/publish_staging

cd bin/Release/$FRAMEWORK/$TARGET/publish_staging

if [[ $STRIP_BINARY -eq 1 ]];
then
    echo "stripping binary '$BINARY'..."
    strip $BINARY
fi

echo "cleaning misc files..."
rm -rf *.pdb

if [[ $UPX_COMPRESS -eq 1 ]];
then
    echo "compressing binary with UPX..."
    upx --lzma $BINARY
fi

# check the binary
ls -la $BINARY

popd

mkdir -p builds/
echo "compressing to $ARTIFACT..."
if [[ $ARCTYPE == "7z" ]];
then
    7z a $ARTIFACT ./$PROJECT_DIR/bin/Release/$FRAMEWORK/$TARGET/publish_staging/*
elif [[ $ARCTYPE == "tar.xz" ]];
then
    tar --transform "s/^publish_staging/$ARCNAME/" -cJvf $ARTIFACT -C $PROJECT_DIR/bin/Release/$FRAMEWORK/$TARGET/ publish_staging
fi

echo "cleaning publish_staging..."
rm -r $PROJECT_DIR/bin/Release/$FRAMEWORK/$TARGET/publish_staging

echo "release built!"
