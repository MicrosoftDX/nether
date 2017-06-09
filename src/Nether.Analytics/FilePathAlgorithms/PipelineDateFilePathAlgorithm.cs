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

        public FilePathResult GetFilePath(string partitionId, string pipelineName, int index, Message msg)
        {
            return GetFilePath(pipelineName, msg.MessageType, msg.EnqueuedTimeUtc, partitionId);
        }

        public System.Collections.Generic.IEnumerable<FilePathResult> GetFilePaths(string pipelineName, string messageType, DateTime from, DateTime to)
        {
            do
            {
                yield return GetFilePath(pipelineName, messageType, from, null);
                from = from.AddMinutes((int)_newFileOption);
            } while (from <= to);
        }

        public string GetRootPath(string pipelineName, string messageType)
        {
            // _rootFolder/pipelineName/messageType/year/month/day
            var hierarchy = new string[]
            {
                _rootFolder,
                pipelineName,
                messageType,
            };

            return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), hierarchy);
        }

        public FilePathResult GetFilePath(string pipelineName, string messageType, DateTime dateTime, string partitionId)
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


            dateTime = dateTime - TimeSpan.FromSeconds(dateTime.Second);
            dateTime = dateTime - TimeSpan.FromMinutes(dateTime.Minute % (int)_newFileOption);

            if (_newFileOption > NewFileNameOptions.EveryHour)
            {
                var h = ((int)_newFileOption) / 60;
                dateTime = dateTime - TimeSpan.FromHours(dateTime.Hour % h);
            }

            var hour = dateTime.Hour.ToString("D2");
            var minute = dateTime.Minute.ToString("D2");

            // In case the paritionId passed to us is null, or empty, we'll add the *
            // and assume the caller can "query" the underlying file system to get the 
            // multiple partitions
            var partition = string.IsNullOrEmpty(partitionId) ? "*" : partitionId.PadLeft(2, '0');
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
