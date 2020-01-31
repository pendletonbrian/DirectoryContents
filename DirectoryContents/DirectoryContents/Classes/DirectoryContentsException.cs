using System;

namespace DirectoryContents.Classes
{
    public class DirectoryContentsException : Exception
    {
        public DirectoryContentsException(string msg) : base(msg)
        {
        }
    }
}