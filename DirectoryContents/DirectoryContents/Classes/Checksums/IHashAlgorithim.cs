using System;

namespace DirectoryContents.Classes.Checksums
{
    public interface IHashAlgorithim
    {
        /// <summary>
        /// Gets the hash of the given data.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        byte[] GetHash(byte[] data);
    }
}