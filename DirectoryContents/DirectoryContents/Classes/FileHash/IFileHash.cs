using System;

namespace DirectoryContents.Classes.FileHash
{
    public interface IFileHash
    {
        /// <summary>
        /// Gets the hash of the given data.
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <returns>
        /// </returns>
        UInt64 GetHash(byte[] data);
    }
}