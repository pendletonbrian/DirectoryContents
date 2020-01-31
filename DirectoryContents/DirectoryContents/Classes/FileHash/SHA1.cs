using System;

namespace DirectoryContents.Classes.FileHash
{
    public class SHA1 : IFileHash
    {
        public UInt64 GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA1 crypto = System.Security.Cryptography.SHA1.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return BitConverter.ToUInt64(hash, 0);
        }
    }
}