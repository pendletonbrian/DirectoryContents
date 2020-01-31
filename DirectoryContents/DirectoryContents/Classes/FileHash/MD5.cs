using System;

namespace DirectoryContents.Classes.FileHash
{
    public class MD5 : IFileHash
    {
        public UInt64 GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.MD5 crypto = System.Security.Cryptography.MD5.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return BitConverter.ToUInt64(hash, 0);
        }
    }
}