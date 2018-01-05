using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DistributedConference
{
    class account
    {
        public string username { get; }
        public byte[] salt { get;}
        public string hash { get; }

       

        public account(string username, string password)
        {
            

            this.username = username;
            this.salt = getSalt();
            this.hash = generatePassHash(Encoding.UTF8.GetBytes(password), salt);
            
            


        }

        private string generatePassHash(byte[] password, byte[] salt)
        {
            byte[] data = new byte[password.Length + salt.Length];
            password.CopyTo(data, 0);
            salt.CopyTo(data, password.Length);
            SHA512Managed hasher = new SHA512Managed();
            return hasher.ComputeHash(data).ToString();
            
           
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
