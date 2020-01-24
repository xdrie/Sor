
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

### mono build
```sh
./script/get_natives.sh # get native libraries
./script/build_mono_release.sh
```
run `SorDk.exe` with mono or use the helper script `./Sor_game`. mono builds are cross-platform and will run on any system that has mono and compatible native libraries, which are bundled for windows, macos, and linux.

### native build
```sh
./script/build_native_release.sh <runtime> # ex. linux-x64
```
run `./SorDk` (Unix) or `SorDk.exe` (Win). native builds are specific to the target system and are completely self-contained.

#### pack
a packer can optionally be used to clean up and minify output bundles. use `./scripts/get_tools.sh` to fetch tools, then pass the `pack` argument to the native build script. set `WARP_COMPRESS=1` before the command to optimize with warp.

## license

copyright &copy; 2019-2020 xdrie. all rights reserved.
