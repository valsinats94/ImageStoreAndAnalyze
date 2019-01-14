using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Exceptions
{
    public class NoSuchCategoryException : Exception
    {
        public NoSuchCategoryException()
        {
        }

        public NoSuchCategoryException(string message)
            : base(message)
        {
        }
    }
}
