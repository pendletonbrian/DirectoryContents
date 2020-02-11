using System;

namespace DirectoryContents.Classes.Checksums
{
    public class SHA384 : IHashAlgorithim
    {
        /// <summary>
        /// Get the hash for the SHA-384 algorithim.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        public byte[] GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA384 crypto = System.Security.Cryptography.SHA384.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return hash;
        }
    }
}