using System;

namespace dotSpaceUtilities
{
    public class RepoUtility
    {
        public static string GenerateUniqueRemoteSpaceUri(string uri, string name)
        {
            return uri + "/" + "Conference" + ConvertHashCodeToAlphabeticString((uint) name.GetHashCode() % UInt32.MaxValue);
        }
        public static string GenerateUniqueSequentialSpaceName(string name)
        {
            return "Conference" + ConvertHashCodeToAlphabeticString((uint) name.GetHashCode() % UInt32.MaxValue);
        }

        private static string ConvertHashCodeToAlphabeticString(uint hash)
        {
            char[] chars = hash.ToString().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char) (chars[i] + 65);
            }
            
            return new String(chars).ToUpper();
        }
    }
}
