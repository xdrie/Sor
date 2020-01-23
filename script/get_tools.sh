set -e

mkdir -p builds
WARP_BIN=builds/warp-packer
WARP_DL=https://github.com/dgiagio/warp/releases/download/v0.3.0/linux-x64.warp-packer
wget $WARP_DL -O $WARP_BIN
chmod +x WARP_BIN