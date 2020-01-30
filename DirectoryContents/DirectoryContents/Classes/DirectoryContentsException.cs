using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryContents.Classes
{
    public class DirectoryContentsException : Exception
    {
        public DirectoryContentsException(string msg) : base(msg)
        {
        }
    }
}
