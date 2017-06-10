// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Collections.Generic;

namespace Nether.Analytics
{
    public class DateFolderStructure : IFolderStructure
    {
        private string _rootFolder;
        private bool _partitionByPipeline;
        private bool _partitionByMessageTypeAndVersion;
        private NewFileNameOptions _newFileOption;

        public DateFolderStructure(string rootFolder = "nether", bool partitionByPipeline = false, bool partitionByMessageTypeAndVersion = true, NewFileNameOptions newFileOption = NewFileNameOptions.Every15Minutes)
        {
            _rootFolder = rootFolder;
            _newFileOption = newFileOption;
            _partitionByPipeline = partitionByPipeline;
            _partitionByMessageTypeAndVersion = partitionByMessageTypeAndVersion;
        }

        public string[] GetFolders(string partitionId, string pipelineName, int index, Message msg, out string fileName)
        {
            var folders = new List<string>();

            folders.Add(_rootFolder);
            if (_partitionByPipeline)
            {
                folders.Add(pipelineName);
            }
            if (_partitionByMessageTypeAndVersion)
            {
                folders.Add(msg.MessageType);
                folders.Add(msg.Version.Compatible);
            }
            folders.Add(msg.EnqueuedTimeUtc.Year.ToString("D4"));
            folders.Add(msg.EnqueuedTimeUtc.Month.ToString("D2"));
            folders.Add(msg.EnqueuedTimeUtc.Day.ToString("D2"));

            var dateTime = msg.EnqueuedTimeUtc;

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
            fileName = $"{hour}_{minute}_p{partition}";

            return folders.ToArray();
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
