#!/bin/sh

dotnet tool run dotnet-ef \
  --project SwgohApi.Infrastructure/SwgohApi.Infrastructure.csproj \
	--startup-project SwgohApi \
	"$@"
