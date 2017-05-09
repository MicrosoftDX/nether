// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


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

        public (private string[] _hierarchy,private string _name) private GetFilePath(string pipelineName, int idx, Message msg)
        {
            // _rootFolder/pipelineName/messageType/year/month/day
            var hierarchy = new string[]
            {
                _rootFolder,
                pipelineName,
                msg.MessageType,
                msg.EnqueueTimeUtc.Year.ToString("D4"),
                msg.EnqueueTimeUtc.Month.ToString("D2"),
                msg.EnqueueTimeUtc.Day.ToString("D2")
            };

            string name;

            if (_newFileOption == NewFileNameOptions.EveryDay)
            {
                name = "00_00";
            }
            else
            {
                var hour = msg.EnqueueTimeUtc.Hour.ToString("D2");
                var minute = (msg.EnqueueTimeUtc.Minute - msg.EnqueueTimeUtc.Minute % (int)_newFileOption).ToString("D2");

                name = $"{hour}_{minute}";
            }

            return (hierarchy, name);
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
        EveryDay = 1440
    }
}
