
# Sor

the mechanical bird game

## build

the game supports running on both .net core and mono. after using build scripts, archives are placed in `builds/`

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

## license

copyright &copy; 2019-2020 xdrie. all rights reserved.
