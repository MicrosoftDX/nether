// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityTestClient
{
    internal static class CommandExtensions
    {
        public static CommandOption StandardHelpOption(this CommandLineApplication app)
        {
            return app.HelpOption("-? | -h | --help");
        }
        public static void ShowHelpOnExecute(this CommandLineApplication app)
        {
            app.OnExecute(() => { app.ShowHelp(); return 0; });
        }

        public static CommandLineApplication Command(this CommandLineApplication app, string name, string description, CommandBase command)
        {
            return app.Command(name, config =>
            {
                config.Description = description;
                command.Register(config);
            });
        }


        public static string GetValue(this CommandOption option, string optionName, bool requireNotNull = false, bool promptIfNull = false, bool sensitive = false, string additionalPromptText = null)
        {
            string value = option.Value();
            if (requireNotNull && string.IsNullOrEmpty(value))
            {
                if (promptIfNull)
                {
                    if (optionName == null)
                    {
                        throw new Exception("promptIfNull specified but promptName not set");
                    }
                    Console.WriteLine($"Please enter {optionName}{additionalPromptText}:");

                    if (sensitive)
                    {
                        value = ReadSensitiveValue();
                    }
                    else
                    {
                        value = Console.ReadLine();
                    }
                }
                if (string.IsNullOrEmpty(value))
                {
                    throw new CommandArgumentException($"{optionName} not specified - exiting");
                }
            }
            return value;
        }


        public static string ReadSensitiveValue()
        {
            var buf = new StringBuilder();

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        return buf.ToString();
                    case ConsoleKey.Backspace:
                        buf.Remove(buf.Length - 1, 1);
                        break;
                    default:
                        if (char.IsControl(keyInfo.KeyChar))
                        {
                            return null;
                        }
                        buf.Append(keyInfo.KeyChar);
                        break;
                }
            }
        }
    }
}
