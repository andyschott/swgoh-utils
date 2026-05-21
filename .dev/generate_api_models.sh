#!/bin/sh

pushd ../api

dotnet dotnet-typegen generate \
  -p SwgohAPi.Models.TypeGen \
  -o ../../src/app/apiModels

popd
