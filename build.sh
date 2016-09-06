#!/bin/bash

dotnet --version

# TODO - investigate the exception when running dotnet restore with parallelisation
echo "*** Restoring packages"
dotnet restore --disable-parallel


echo
echo "*** Building projects"

while IFS= read -r var
do
  dotnet build "$var"
done < "build/build-order.txt"


echo
echo "*** Running tests"

testExitCode=0
while IFS= read -r var
do
  dotnet test "$var"
  lastexit=$?
  if [ $lastexit -ne 0 ]
  then
    testExitCode=$lastexit
  fi
done < "build/test-order.txt"

exit $testExitCode