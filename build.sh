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

  while IFS= read -r var
  do
    if [ "x$var" != "x" ]
    then
      echo "*** dotnet restore $var"
      dotnet restore "$var"
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
    echo "*** Restore failed"
    exit $buildExitCode
  fi
fi

#  want to do this, but need to figure it out for xplat
# echo
# echo "*** Building solution"
# buildExitCode=0
# dotnet msbuild Nether.sln
# lastexit=$?
# if [ $lastexit -ne 0 ]
# then
#   buildExitCode=$lastexit
# fi

# if [ $buildExitCode -ne 0 ]
# then
#   echo
#   echo "*** Build failed"
#   exit $buildExitCode
# fi


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
  exit $buildExitCode
fi

echo "*** Build completed"

