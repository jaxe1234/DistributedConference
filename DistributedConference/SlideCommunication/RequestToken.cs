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
        public RequestToken(string secret)
        {
            Secret = secret;
            Token = Guid.NewGuid().ToString();
        }
        public string Token { get; }
        private string Secret { get; }
        public string ResponseToken => NamingTool.GetSHA256String(Secret + Token);
    }
}
