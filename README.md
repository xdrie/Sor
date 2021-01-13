
![icon](media/icon.png)

# Sor

the mechanical bird game

## build

the game supports running on both .net core and mono. after using build scripts, archives are placed in `builds/`

### dependencies

- [.NET Core 3.1](https://dotnet.microsoft.com/download)
- [Mono](https://www.mono-project.com/download/stable/) (only for Mono build)
- XNG (XNez + Glint) (git)
- DuckMind (git)

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

## license

copyright &copy; 2019-2021 xdrie. all rights reserved.
