namespace DirectoryContents.Classes.Checksums
{
    public class SHA512 : IHashAlgorithim
    {
        public string AlgorithimName { get { return "SHA-512"; } }

        /// <summary>
        /// Get the hash for the SHA-512 algorithim.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        public byte[] GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA512 crypto = System.Security.Cryptography.SHA512.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return hash;
        }
    }
}