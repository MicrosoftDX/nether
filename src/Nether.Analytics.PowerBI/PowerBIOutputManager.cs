using System;
using System.Threading.Tasks;

namespace Nether.Analytics.PowerBI
{
    public class PowerBIOutputManager : IOutputManager
    {
        //TODO: Implement a working solution for outputting to PowerBI.
        // Take a look at the implementation for DataLakeStoreOutputManager for inspiration and sync
        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            throw new NotImplementedException();
        }
    }
}
