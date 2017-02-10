#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.


##
## Parse arguments
##

noDotNetRestore=0
noWebClientRestore=0
while [[ $# -gt 0 ]]
do
key="$1"
case $key in
    --no-dot-net-restore)
    noDotNetRestore=1
    shift # past argument
    ;;
    --no-web-client-restore)
    noWebClientRestore=1
    shift # past argument
    ;;
    *)
            # unknown option
    ;;
esac
shift # past argument or value
done

dotnet --version

##
## .NET Package restore
##

if [ $noDotNetRestore != 0 ] 
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

##
## Web client install
##

if [ $noWebClientRestore != 0 ] 
then
  echo "*** Skipping npm/bower install"
else
  echo "*** Installing npm/bower packages..."

  # check node.js and NPM are installed
  command -v nodejs >/dev/null 2>%1 || { echo >&2 "Node.js is not installed. Aborting."; exit 1;}
  command -v npm >/dev/null 2>%1 || { echo >&2 "NPM is not installed. Aborting."; exit 1;}

  # fix for Ubuntu (node binary is called nodejs)
  # commenting out for now as we can use nodejs legacy
  # ln -s /usr/bin/nodejs /usr/bin/node

  # install bower
  if command bower 2>/dev/null; then
      echo "bower already installed"
  else
      echo "bower not present, run 'npm install -g bower'"
      exit 123
  fi

  buildExitCode=0

  pushd .
  cd src/Nether.Web
  echo "*** npm install ..."
  npm install
  lastexit=$?
  if [ $lastexit -ne 0 ]
  then
    buildExitCode=$lastexit
  fi

  # install gulp
  if command gulp 2>/dev/null; then
      echo "gulp already installed"
  else
      echo "gulp not present, run 'npm install -g gulp''"
      exit 123
  fi

  echo "*** bower install ..."
  bower install
  lastexit=$?
  if [ $lastexit -ne 0 ]
  then
    buildExitCode=$lastexit
  fi

  echo "*** gulp npmtolib..."
  gulp npmtolib
  lastexit=$?
  if [ $lastexit -ne 0 ]
  then
    buildExitCode=$lastexit
  fi

  echo "*** done."
  popd
fi



##
## Build!
##

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

# Run gulp task for typescript
cd src/Nether.Web
gulp compiletsforadminui
cd ..
cd ..

if [ $buildExitCode -ne 0 ]
then
  echo
  echo "*** Build failed"
  exit $buildExitCode
fi

echo "*** Build completed"

