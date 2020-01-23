
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

### mono build
```sh
./script/get_natives.sh # get native libraries
./script/build_mono_release.sh
```
run `SorDk.exe` with mono or use the helper script `./Sor_game`

### native build
```sh
./script/build_native_release.sh <runtime> # ex. linux-x64
```
run `./SorDk` (Unix) or `SorDk.exe` (Win)

## license

copyright &copy; 2019-2020 xdrie. all rights reserved.
