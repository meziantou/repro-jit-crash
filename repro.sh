#!/usr/bin/env bash
set -u
cd "$(dirname "$0")"
dotnet build Repro -c Release || exit 1
bin=./Repro/bin/Release/net11.0/Repro
for i in 1 2 3; do
  "$bin" > /dev/null 2>&1
  echo "default run $i: exit $? (139 = crash)"
done
DOTNET_TieredPGO=0 "$bin" > /dev/null 2>&1
echo "DOTNET_TieredPGO=0: exit $? (0 = OK)"
