#!/bin/bash

# TODO - investigate the exception when running dotnet restore with parallelisation
dotnet restore --disable-parallel

while IFS= read -r var
do
  dotnet build "$var"
done < "build/build-order.txt"