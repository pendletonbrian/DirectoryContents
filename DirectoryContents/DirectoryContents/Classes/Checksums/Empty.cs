namespace DirectoryContents.Classes.Checksums
{
    public class Empty : IHashAlgorithim
    {
        public Empty()
        {
        }

        public string AlgorithimName { get { return "Empty"; } }

        /// <summary>
        /// Returns zero.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        public byte[] GetHash(byte[] data)
        {
            return new byte[0];
        }
    }
}