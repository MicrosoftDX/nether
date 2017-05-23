using System;
using System.Collections.Generic;
using System.IO;

namespace AnalyticsTestClient
{
    public class PlayerDistributionProvider
    {
        double[,] _distributionTable = new double[7, 24];

        public PlayerDistributionProvider(string playerDistributionFile)
        {
            var s = File.ReadAllText(playerDistributionFile);

            var lines = s.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 8)
            {
                throw new ArgumentException("Player Distribution File should contain 8 rows. 1 header and 7 week days");
            }

            for (int i = 0; i < 7; i++)
            {
                var cols = lines[i + 1].Split('\t');

                if (cols.Length != 25)
                {
                    throw new ArgumentException("Player Distribution File should contain 25 tab separated columns per day. Week day and the 24 hours");
                }

                for (int j = 0; j < 24; j++)
                {
                    _distributionTable[i, j] = double.Parse(cols[j + 1]);
                }
            }
        }

        public double GetDistribution(DateTime t)
        {
            int i = (int)t.DayOfWeek;
            int j = t.Hour;

            return _distributionTable[i, j];
        }
    }
}