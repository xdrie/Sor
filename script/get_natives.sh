set -e

MGPKG=$(ls ~/.nuget/packages/monogame.framework.desktopgl.core/ | tail -n1)

echo "Using MonoGame DesktopGL package v$MGPKG"

mkdir -p natives
# get natives for win64, macos, and linux64
cp -rv ~/.nuget/packages/monogame.framework.desktopgl.core/$MGPKG/runtimes/win-x64/native/* natives/
cp -rv ~/.nuget/packages/monogame.framework.desktopgl.core/$MGPKG/runtimes/linux-x64/native/* natives/
cp -rv ~/.nuget/packages/monogame.framework.desktopgl.core/$MGPKG/runtimes/osx/native/* natives/
