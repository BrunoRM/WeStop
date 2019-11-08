using System.Security.Cryptography;
using System.Text;

namespace WeStop.Core.Helpers
{
    public static class MD5HashGenerator
    {
        public static string GenerateHash(string text)
        {
            using (var md5Provider = MD5.Create())
            {
                byte[] data = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
