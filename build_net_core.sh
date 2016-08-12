#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

dotnet restore
dotnet test ./Cottle.Test -c Release -f netcoreapp1.0
dotnet pack ./Cottle -c Release -o ./artifacts