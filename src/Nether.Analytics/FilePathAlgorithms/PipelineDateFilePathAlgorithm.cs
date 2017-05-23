// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Nether.Analytics
{
    public class PipelineDateFilePathAlgorithm : IFilePathAlgorithm
    {
        private string _rootFolder;
        private NewFileNameOptions _newFileOption;

        public PipelineDateFilePathAlgorithm(string rootFolder = "nether", NewFileNameOptions newFileOption = NewFileNameOptions.Every15Minutes)
        {
            _rootFolder = rootFolder;
            _newFileOption = newFileOption;
        }

        public FilePathResult GetFilePath(string pipelineName, int idx, Message msg)
        {
            return GetFilePath(pipelineName, msg.MessageType, msg.EnqueueTimeUtc);
        }

        public System.Collections.Generic.IEnumerable<FilePathResult> GetFilePaths(string pipelineName, string messageType, DateTime start, DateTime end)
        {
            do
            {
                yield return GetFilePath(pipelineName, messageType, start);
                start = start.AddMinutes((int)_newFileOption);

            } while (start <= end);
        }

        public FilePathResult GetFilePath(string pipelineName, string messageType, DateTime dateTime)
        {
            // _rootFolder/pipelineName/messageType/year/month/day
            var hierarchy = new string[]
            {
                _rootFolder,
                pipelineName,
                messageType,
                dateTime.Year.ToString("D4"),
                dateTime.Month.ToString("D2"),
                dateTime.Day.ToString("D2")
            };

            string name;


            DateTime t = msg.EnqueuedTimeUtc;
            t = t - TimeSpan.FromSeconds(t.Second);
            t = t - TimeSpan.FromMinutes(t.Minute % (int)_newFileOption);

            if (_newFileOption > NewFileNameOptions.EveryHour)
            {
                var h = ((int)_newFileOption) / 60;
                t = t - TimeSpan.FromHours(t.Hour % h);
            }

            var hour = t.Hour.ToString("D2");
            var minute = t.Minute.ToString("D2");
            var partition = msg.PartitionId.PadLeft(2, '0');
            name = $"{hour}_{minute}_p{partition}";

            return new FilePathResult { Hierarchy = hierarchy, Name = name };
        }
    }

    public enum NewFileNameOptions
    {
        Every1Minute = 1,
        Every5Minutes = 5,
        Every10Minutes = 10,
        Every15Minutes = 15,
        Every30Minutes = 30,
        EveryHour = 60,
        Every3Hours = 180,
        Every6Hours = 360,
        Every12Hours = 720,
        EveryDay = 1440
    }
}
