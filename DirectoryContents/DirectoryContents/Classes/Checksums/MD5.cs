namespace DirectoryContents.Classes.Checksums
{
    public class MD5 : IHashAlgorithim
    {
        public string AlgorithimName { get { return "MD5"; } }

        public byte[] GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.MD5 crypto = System.Security.Cryptography.MD5.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return hash;
        }
    }
}