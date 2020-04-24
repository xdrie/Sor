
![icon](media/icon.png)

# Sor

the mechanical bird game

## build

the game supports running on both .net core and mono. after using build scripts, archives are placed in `builds/`

### dependencies

- [.NET Core 3.1](https://dotnet.microsoft.com/download)
- [Mono](https://www.mono-project.com/download/stable/) (only for Mono build)
- XNG (XNez + Glint) (git)
- Lunch Simulator (git)

pull the source submodules first
```sh
git submodule update --init --recursive # update submodules
```

### native build
```sh
./script/build_arc.sh <runtime> # ex. one of linux-x64, osx-x64, win-x64
```
this will drop an archive in `builds/` when completed.
run `./SorDk` (Unix) or `SorDk.exe` (Win). native builds are specific to the target system and are completely self-contained.

#### experimental: Mono build

targeting mono can allow the built release to be cross-platform and run on any platform with Mono.
```sh
# optional, bundles native libraries
./script/get_natives.sh
# build archive for mono
FRAMEWORK=net48 CHANNEL=mono ./script/build_arc.sh win-x64
```

#### experimental: CoreRT build

optionally, [CoreRT](https://github.com/dotnet/corert) can be used to create a native binary. It produces significantly smaller file sizes and creates a much neater output; however it is highly experimental and unsupported. to tell the native builder to use CoreRT:
```sh
USE_CORERT=1 USE_UPX=1 ./script/build_arc.sh <runtime> # uses CoreRT and UPX to build a native binary
```

## license

copyright &copy; 2019-2020 xdrie. all rights reserved.
