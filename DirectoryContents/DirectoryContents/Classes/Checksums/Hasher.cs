using System;
using System.Diagnostics;
using System.IO;

namespace DirectoryContents.Classes.Checksums
{
    public class Hasher
    {
        #region Private Members

        private readonly IHashAlgorithim m_Algorithim;

        #endregion Private Members

        #region constructor

        public Hasher(IHashAlgorithim algorithim)
        {
            m_Algorithim = algorithim;
        }

        #endregion constructor

        #region Public Methods

        /// <summary>
        /// Attempts to get the hash for the given file.
        /// </summary>
        /// <param name="filename">
        /// The fully qualified name to the file.
        /// </param>
        /// <param name="checksum">
        /// The checksum value. If the file doesn't exist, or there is an issue,
        /// the value is set to string.Empty.
        /// </param>
        /// <returns>
        /// Whether or not the file was able to be hashed.   If the file does not
        /// exist, null is returned.
        /// </returns>
        public bool? TryGetFileChecksum(string filename, out string checksum)
        {
            try
            {
                if (File.Exists(filename) == false)
                {
                    checksum = string.Empty;

                    Debug.WriteLine($"File \"{filename}\" does not exist.");

                    return null;
                }

                byte[] inputBytes = null;

                using (var binRdr = new BinaryReader(File.OpenRead(filename)))
                {
                    inputBytes = binRdr.ReadBytes((int)binRdr.BaseStream.Length);
                }

                if (inputBytes is null)
                {
                    checksum = string.Empty;

                    return false;
                }

                checksum = BitConverter.ToString(m_Algorithim.GetHash(inputBytes));

                if (string.IsNullOrWhiteSpace(checksum) == false)
                {
                    checksum = checksum.Replace("-", string.Empty);
                }

                return true;
            }
            catch
            {
                checksum = string.Empty;

                return false;
            }
        }

        #endregion Public Methods
    }
}