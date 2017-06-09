// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Collections.Generic;

namespace Nether.Analytics
{
    public class PipelineDateFilePathAlgorithm : IFilePathAlgorithm
    {
        private string _rootFolder;
        private bool _partitionByPipeline;
        private bool _partitionByMessageType;
        private NewFileNameOptions _newFileOption;

        public PipelineDateFilePathAlgorithm(string rootFolder = "nether", bool partitionByPipeline = false, bool partitionByMessageType = true, NewFileNameOptions newFileOption = NewFileNameOptions.Every15Minutes)
        {
            _rootFolder = rootFolder;
            _newFileOption = newFileOption;
            _partitionByPipeline = partitionByPipeline;
            _partitionByMessageType = partitionByMessageType;
        }

        public FilePathResult GetFilePath(string partitionId, string pipelineName, int index, Message msg)
        {
            var hierarchy = new List<string>();
            hierarchy.Add(_rootFolder);
            if (_partitionByPipeline) hierarchy.Add(pipelineName);
            if (_partitionByMessageType)hierarchy.Add(msg.MessageType);
            hierarchy.Add(msg.EnqueuedTimeUtc.Year.ToString("D4"));
            hierarchy.Add(msg.EnqueuedTimeUtc.Month.ToString("D2"));
            hierarchy.Add(msg.EnqueuedTimeUtc.Day.ToString("D2"));

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
            var name = $"{hour}_{minute}_p{partition}";

            return new FilePathResult { Hierarchy = hierarchy.ToArray(), Name = name };
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
