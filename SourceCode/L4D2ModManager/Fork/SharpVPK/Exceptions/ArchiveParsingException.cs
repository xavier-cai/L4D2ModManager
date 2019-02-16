using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpVPK.Exceptions
{
    public class ArchiveParsingException : Exception
    {
        public ArchiveParsingException()
        {
        }

        public ArchiveParsingException(string message) 
            : base(message)
        {
        }

        public ArchiveParsingException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
