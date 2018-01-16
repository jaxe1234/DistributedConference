using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class RSA
    {
        
        public static string RSAEncrypt(string Password)
        {
            var loginSpace = new RemoteSpace("tcp://" + _Resources.Resources.InternetProtocolAddress +":5001/loginAttempts?CONN");
            var PubKey = loginSpace.Query(typeof(string))[0] as string;
            byte[] BytePass = Encoding.UTF8.GetBytes(Password);// Convert.FromBase64String(Password);
            byte[] encryptedData;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(PubKey);
            encryptedData = RSA.Encrypt(BytePass, true);
            RSA.Dispose();
            return Convert.ToBase64String(encryptedData);
        }
    }
}
