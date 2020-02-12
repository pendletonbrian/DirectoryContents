using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryContents.Classes.Checksums
{
    public class HashAlgorithimFactory
    {
        public IHashAlgorithim Get(Enumerations.ChecksumAlgorithim algorithim)
        {
            IHashAlgorithim hashAlgoithim = new Empty();

            switch (algorithim)
            {
                case Enumerations.ChecksumAlgorithim.None:
                    break;
                case Enumerations.ChecksumAlgorithim.MD5:
                    break;
                case Enumerations.ChecksumAlgorithim.SHA1:
                    break;
                case Enumerations.ChecksumAlgorithim.SHA256:
                    break;
                case Enumerations.ChecksumAlgorithim.SHA384:
                    break;
                case Enumerations.ChecksumAlgorithim.SHA512:
                    break;
                default:
                    throw new ArgumentException($"Unhandled {nameof(Enumerations.ChecksumAlgorithim)}: {algorithim}");
            }

            return hashAlgoithim;
        }
    }
}
