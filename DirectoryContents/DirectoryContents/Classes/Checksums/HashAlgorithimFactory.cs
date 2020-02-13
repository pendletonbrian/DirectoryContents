using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryContents.Classes.Checksums
{
    public class HashAlgorithimFactory
    {
        public static IHashAlgorithim Get(Enumerations.ChecksumAlgorithim algorithim)
        {
            IHashAlgorithim hashAlgoithim = new Empty();

            switch (algorithim)
            {
                case Enumerations.ChecksumAlgorithim.None:
                    break;

                case Enumerations.ChecksumAlgorithim.MD5:
                    hashAlgoithim = new MD5();
                    break;

                case Enumerations.ChecksumAlgorithim.SHA1:
                    hashAlgoithim = new SHA1();
                    break;

                case Enumerations.ChecksumAlgorithim.SHA256:
                    hashAlgoithim = new SHA256();
                    break;

                case Enumerations.ChecksumAlgorithim.SHA384:
                    hashAlgoithim = new SHA384();
                    break;

                case Enumerations.ChecksumAlgorithim.SHA512:
                    hashAlgoithim = new SHA512();
                    break;

                default:
                    throw new ArgumentException($"Unhandled {nameof(Enumerations.ChecksumAlgorithim)}: {algorithim}");
            }

            return hashAlgoithim;
        }
    }
}
