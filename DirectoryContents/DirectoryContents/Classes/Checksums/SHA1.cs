namespace DirectoryContents.Classes.Checksums
{
    public class SHA1 : IHashAlgorithim
    {
        public byte[] GetHash(byte[] data)
        {
            byte[] hash = null;

            using (System.Security.Cryptography.SHA1 crypto = System.Security.Cryptography.SHA1.Create())
            {
                hash = crypto.ComputeHash(data);
            }

            return hash;
        }
    }
}