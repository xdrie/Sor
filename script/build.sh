#!/usr/bin/env bash

# fail on error
set -e

# banner
echo "== MGCORE build script =="
echo "================================"
echo ""
echo ""

GAME=Sor
SRCDIR=src/${GAME}
SLN=${SRCDIR}/${GAME}.sln
PROJ=${SRCDIR}/${GAME}Dk/${GAME}Dk.csproj
FRAMEWORK=netcoreapp3.1

echo "getting submodules..."
git submodule update --init --recursive

# make sure we're in the right start
echo "finding project..."
ls $PROJ

echo "restoring packages..."
dotnet restore $SLN

# push our starting directory
pushd . > /dev/null

# update pipeline references
# echo "building pipeline importers..."
# cd lib/XNez/Nez.PipelineImporter
# dotnet publish

popd # go back

# build project
echo "building project..."
dotnet build -c Release -f $FRAMEWORK $PROJ
