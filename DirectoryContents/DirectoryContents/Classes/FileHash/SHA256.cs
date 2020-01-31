using System;

namespace DirectoryContents.Classes.FileHash
{
    public class SHA256 : IFileHash
    {
        /// <summary>
        /// Get the hash for the SHA-256 algorithim.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        public UInt64 GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA256 crypto = System.Security.Cryptography.SHA256.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return BitConverter.ToUInt64(hash, 0);
        }
    }
}