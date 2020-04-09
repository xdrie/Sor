
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
./script/build_native_release.sh <runtime> # ex. one of linux-x64, osx-x64, win-x64
```
this will drop an archive in `builds/` when completed.
run `./SorDk` (Unix) or `SorDk.exe` (Win). native builds are specific to the target system and are completely self-contained.

#### pack
a packer can optionally be used to clean up and minify output bundles. use `./scripts/get_tools.sh` to fetch tools, then pass the `pack` argument to the native build script. set `WARP_COMPRESS=1` before the command to optimize with warp. when using warp build, make sure natives are bundled because it seems to be iffy with native libraries.

## license

copyright &copy; 2019-2020 xdrie. all rights reserved.
