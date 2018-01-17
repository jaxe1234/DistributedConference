using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ProjectUtilities;

namespace SlideCommunication
{
    public class RequestToken
    {
        public RequestToken()
        {
            Token = Guid.NewGuid().ToString();
        }

        public RequestToken(string secret) : this()
        {
            Secret = secret;
        }

        public RequestToken(string token, string secret)
        {
            Secret = secret;
            Token = token;
        }
        public string Token { get; }
        private string Secret { get; }
        public string ResponseToken { get {
                if (string.IsNullOrEmpty(Secret))
                {
                    throw new ArgumentNullException();
                }
                return NameHashingTool.GetSHA256String(Secret + Token);
            }
        } 
    }
}
