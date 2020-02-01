using System;

namespace DirectoryContents.Classes.FileHash
{
    public class SHA384 : IFileHash
    {
        /// <summary>
        /// Get the hash for the SHA-384 algorithim.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        public ulong GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA384 crypto = System.Security.Cryptography.SHA384.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return BitConverter.ToUInt64(hash, 0);
        }
    }
}