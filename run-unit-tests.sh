#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

echo
echo "*** Executing tests"

testExitCode=0

while IFS= read -r var
do
  if [ "x$var" != "x" ]
  then
    echo "*** dotnet test $var"
    dotnet test "$var"
    lastexit=$?
    if [ $lastexit -ne 0 ]
    then
      testExitCode=$lastexit
    fi
  fi
done < "build/unit-test-order.txt"

if [ $testExitCode -ne 0 ]
then
  exit=$testExitCode
fi
