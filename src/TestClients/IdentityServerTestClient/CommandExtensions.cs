using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerTestClient
{
    static class CommandExtensions
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
    }
}
