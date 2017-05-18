using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Analytics
{
    public class FileResultsReader : IResultsReader
    {
        public FileResultsReader()
        {
            
        }
        public FileResultsReader(IMessageFormatter serializer, IFilePathAlgorithm filePathAlgorithm, string rootPath = @"C:\")
        {

        }

        public IEnumerable<Message> GetLatest(int max = 100)
        {
            throw new NotImplementedException();
        }
    }
}
