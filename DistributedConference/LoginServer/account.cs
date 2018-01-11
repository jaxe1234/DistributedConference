using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LoginServer
{
    public class Account
    {
        public string Username { get; }
        public string Hash { get; }
        public byte[] Salt { get;}
        

       

        public Account(string username, string password)
        {
            

            Username = username;
            Salt = getSalt();
            Hash = GeneratePassHash(Encoding.UTF8.GetBytes(password), Salt);
            
            


        }

        public static string GeneratePassHash(byte[] password, byte[] salt)
        {
            byte[] data = new byte[password.Length + salt.Length];
            password.CopyTo(data, 0);
            salt.CopyTo(data, password.Length);
            SHA512Managed hasher = new SHA512Managed();
            return Encoding.UTF8.GetString(hasher.ComputeHash(data));

            
            
           
        }

        private byte[] getSalt()
        {

            byte[] saltInitArr = new byte[16];
            RNGCryptoServiceProvider saltGen = new RNGCryptoServiceProvider();
            saltGen.GetBytes(saltInitArr);
            saltGen.Dispose();
            return saltInitArr;
            
            


        }
    }
}
