namespace DirectoryContents.Classes.Checksums
{
    public class Empty : IHashAlgorithim
    {
        public Empty() { }

        /// <summary>
        /// Returns zero.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ulong GetHash(byte[] data)
        {
            return 0UL;
        }
    }
}
