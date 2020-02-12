namespace DirectoryContents.Classes.Checksums
{
    public class MD5 : IHashAlgorithim
    {
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