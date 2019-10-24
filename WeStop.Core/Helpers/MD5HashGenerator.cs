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
                return Encoding.UTF8.GetString(md5Provider.ComputeHash(Encoding.UTF8.GetBytes(text)));
            }
        }
    }
}
