#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

noRestore=0
while [[ $# -gt 0 ]]
do
key="$1"
case $key in
    --no-restore)
    noRestore=1
    shift # past argument
    ;;
    *)
            # unknown option
    ;;
esac
shift # past argument or value
done

dotnet --version

# TODO - investigate the exception when running dotnet restore with parallelisation
if [ $noRestore != 0 ] 
then
  echo "*** Skipping package restore"
else
  echo "*** Restoring packages"
  buildExitCode=0

  dotnet restore --disable-parallel

  buildExitCode=$?
  if [ $buildExitCode -ne 0 ]
  then
    exit=$buildExitCode
  fi
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
  echo
  echo "*** Build failed"
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