using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
    class LoginServerTests
    {
        public static void Main(string[] args)
        {
            LoginServer testServer = new LoginServer();
            var accountSpace = new RemoteSpace("tcp://" + _Resources.Resources.InternetProtocolAddress + ":5001/accountCreation");
            var loginSpace = new RemoteSpace("tcp://" + _Resources.Resources.InternetProtocolAddress + ":5001/loginAttempts");
        }
    }
}