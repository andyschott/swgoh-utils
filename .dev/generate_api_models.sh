#!/bin/sh

pushd ../api

dotnet build SwgohApi.Models.TypeGen

dotnet dotnet-typegen generate \
  -p SwgohApi.Models.TypeGen \
  -o ../../src/app/apiModels

popd
