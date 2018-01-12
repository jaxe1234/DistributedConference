using System;

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

        public static string UniqueString(int length)
        {
            Random random = new Random();
            uint hash = (uint)(DateTime.Now.Ticks + random.Next()).GetHashCode();
            return ConvertHashCodeToAlphabeticString(hash);
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

