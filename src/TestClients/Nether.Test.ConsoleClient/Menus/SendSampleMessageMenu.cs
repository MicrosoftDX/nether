// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Nether.Ingest;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Nether.Test.ConsoleClient
{
    internal class SendSampleMessageMenu : ConsoleMenu
    {
        private IAnalyticsClient _client;
        private string _sampleFolder = @"c:\tmp\sample-messages\";
        private string _searchPattern = "*.json";

        public SendSampleMessageMenu(IAnalyticsClient client)
        {
            _client = client;

            Title = "Send Sample Message";

            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            MenuItems.Clear();

            if (Directory.Exists(_sampleFolder))
            {
                var files = Directory.GetFiles(_sampleFolder, _searchPattern);

                var menuKey = 'a';
                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    MenuItems.Add(menuKey++,
                        new ConsoleMenuItem(
                            fileName,
                            () => SampleFileSelected(file)));
                }
            }

            MenuItems.Add('1', new ConsoleMenuItem($"Set folder [${_sampleFolder}]", SetSampleFolder));
            MenuItems.Add('2', new ConsoleMenuItem($"Set search pattern [${_searchPattern}]", SetSearchPattern));
        }

        private void SampleFileSelected(string file)
        {
            var fileContent = File.ReadAllText(file);
            var templateObj = JObject.Parse(fileContent);
            var obj = new JObject();


            foreach (var property in templateObj)
            {
                var propertyType = property.Value.Type.ToString();

                switch (propertyType.ToLower())
                {
                    case "string":
                        var s = ConsoleEx.ReadLine(property.Key, (string)property.Value);
                        obj.Add(property.Key, s);
                        break;

                    case "integer":
                        var i = ConsoleEx.ReadLine(property.Key, (int)property.Value);
                        obj.Add(property.Key, i);
                        break;

                    default:
                        break;
                }
            }

            _client.SendMessageAsync(obj.ToString()).Wait();
            _client.FlushAsync().Wait();
        }

        private void SetSampleFolder()
        {
            _sampleFolder = ConsoleEx.ReadLine("Folder for sample messages", _sampleFolder, s => Directory.Exists(s));
            LoadMenuItems();
        }

        private void SetSearchPattern()
        {
            _searchPattern = ConsoleEx.ReadLine("Search Pattern", _searchPattern);
            LoadMenuItems();
        }
    }
}