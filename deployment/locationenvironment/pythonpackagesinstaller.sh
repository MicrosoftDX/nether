#!/usr/bin/env bash

# Import the HDInsight helper method module.
wget -O /tmp/HDInsightUtilities-v01.sh -q https://hdiconfigactions.blob.core.windows.net/linuxconfigactionmodulev01/HDInsightUtilities-v01.sh && source /tmp/HDInsightUtilities-v01.sh && rm -f /tmp/HDInsightUtilities-v01.sh

# Install Python packages.
echo "Installing packages needed for Spark... "
pip install pytz
pip install geopy
pip install pandas
pip install -U pip setuptools
pip install matplotlib
sudo apt-get install python-tk -y
pip install -U scikit-learn
pip install numpy scipy matplotlib ipython jupyter pandas sympy nose
pip install shapely
sudo apt-get install libgeos-dev -y
echo "Finished installing packages needed for Spark"

exit 0