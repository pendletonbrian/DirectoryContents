using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryContents.Classes.FileHash
{
    public class Hasher
    {
        #region Private Members

        private readonly IFileHash m_FileHash;

        #endregion

        #region constructor

        public Hasher(IFileHash fileHash)
        {
            m_FileHash = fileHash;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to get the hash for the given file.  If the file does not exist,
        /// null is returned.
        /// </summary>
        /// <param name="filename">The fully qualified name to the file.</param>
        /// <param name="hashValue">Whether or not the file was able to be hashed.</param>
        /// <returns></returns>
        public bool? TryGetFileHash(string filename, out ulong hashValue)
        {
            try
            {
                if (File.Exists(filename) == false)
                {
                    hashValue = UInt64.MinValue;

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
                    hashValue = m_FileHash.GetHash(inputBytes);

                    return false;
                }

                hashValue = UInt64.MaxValue;

                return true;
            }
            catch
            {
                hashValue = UInt64.MinValue;

                return false;
            }
        }

        #endregion
    }
}
