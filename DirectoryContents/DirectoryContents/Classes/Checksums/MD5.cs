namespace DirectoryContents.Classes.Checksums
{
    public class MD5 : IHashAlgorithim
    {
        public string AlgorithimName { get { return "MD5 (Not FIPS compliant)"; } }

        public byte[] GetHash(byte[] data)
        {
            byte[] hash = null;

            // Non-FIPS compliant, Cryptography Next Generation version

            using (var crypto = System.Security.Cryptography.MD5.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return hash;
        }
    }
}
