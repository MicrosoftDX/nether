using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace IdentityServerTestClient
{
    public abstract class CommandBase
    {
        public CommandLineApplication CommandConfig { get; private set; }
        public IdentityClientApplication Application { get; private set; }

        public CommandBase(IdentityClientApplication application)
        {
            Application = application;
        }
        public virtual void Register(CommandLineApplication config)
        {
            CommandConfig = config;
            config.OnExecute(() => WrappedExecuteAsync());
        }
        private async Task<int> WrappedExecuteAsync()
        {
            try
            {
                return await ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        protected abstract Task<int> ExecuteAsync();
    }
}