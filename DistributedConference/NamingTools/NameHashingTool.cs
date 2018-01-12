using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NamingTools
{
    public class NameHashingTool
    {
        public static string GenerateUniqueRemoteSpaceUri(string uri, string name)
        {
            return uri + "/" + "Conference" + ConvertHashCodeToAlphabeticString((uint)name.GetHashCode() % UInt32.MaxValue);
        }
        public static string GenerateUniqueSequentialSpaceName(string name)
        {
            return "Conference" + ConvertHashCodeToAlphabeticString((uint)name.GetHashCode() % UInt32.MaxValue);
        }

        public static string UniqueString()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetSHA256String(string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return string.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }

        private static string ConvertHashCodeToAlphabeticString(uint hash)
        {
            char[] chars = hash.ToString().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(chars[i] + 65);
            }

            return new String(chars).ToUpper();
        }
    }
}

