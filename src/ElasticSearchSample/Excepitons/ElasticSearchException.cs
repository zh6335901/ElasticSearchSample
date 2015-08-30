using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample
{
    public class ElasticSearchException : Exception
    {
        public ElasticSearchException() { }

        public ElasticSearchException(string message) : base(message) { }
    }
}
