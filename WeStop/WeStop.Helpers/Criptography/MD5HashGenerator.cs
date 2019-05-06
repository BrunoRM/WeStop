// https://docs.microsoft.com/pt-br/dotnet/api/system.security.cryptography.md5?view=netframework-4.8

using System;
using System.Security.Cryptography;
using System.Text;

namespace WeStop.Helpers.Criptography
{
    public class MD5HashGenerator
    {
        public string GetMD5Hash(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }

        public bool VerifyMd5Hash(string input, string hash)
        {
            string hashOfInput = GetMD5Hash(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
                return true;
            else
                return false;
        }
    }
}
