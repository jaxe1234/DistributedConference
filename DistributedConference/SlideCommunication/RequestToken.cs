using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NamingTools;

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
        public string Token { get; }
        private string Secret { get; }
        public string ResponseToken { get {
                if (string.IsNullOrEmpty(Secret))
                {
                    throw new ArgumentNullException();
                }
                return NamingTool.GetSHA256String(Secret + Token);
            }
        } 
    }
}
