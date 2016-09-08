#!/bin/bash

dotnet --version

# TODO - investigate the exception when running dotnet restore with parallelisation
echo "*** Restoring packages"
buildExitCode=0

dotnet restore --disable-parallel

buildExitCode=$?
if [ $buildExitCode -ne 0 ]
then
  exit=$buildExitCode
fi


echo
echo "*** Building projects"
buildExitCode=0

while IFS= read -r var
do
  if [ "x$var" != "x" ]
  then
    echo "*** dotnet build $var"
    dotnet build "$var"
    lastexit=$?
    if [ $lastexit -ne 0 ]
    then
      buildExitCode=$lastexit
    fi
  fi
done < "build/build-order.txt"

if [ $buildExitCode -ne 0 ]
then
  exit=$buildExitCode
fi

#echo
#echo "*** Running tests"
#
#buildExitCode=0
#while IFS= read -r var
#do
#  dotnet test "$var"
#  lastexit=$?
#  if [ $lastexit -ne 0 ]
#  then
#    buildExitCode=$lastexit
#  fi
#done < "build/test-order.txt"
#
#exit $buildExitCode