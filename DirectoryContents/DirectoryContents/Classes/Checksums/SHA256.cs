namespace DirectoryContents.Classes.Checksums
{
    public class SHA256 : IHashAlgorithim
    {
        /// <summary>
        /// Get the hash for the SHA-256 algorithim.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        public byte[] GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA256 crypto = System.Security.Cryptography.SHA256.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return hash;
        }
    }
}