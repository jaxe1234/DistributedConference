using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace;
using dotSpace.Objects.Space;
using dotSpace.Objects.Network;
using dotSpace.Interfaces.Space;

namespace LoginServer
{
    class loginServer 
    {

        SpaceRepository loginServerSpaces = new SpaceRepository();
        private SequentialSpace userAccounts = new SequentialSpace();
        private SequentialSpace loggedInUsers = new SequentialSpace();
        private SequentialSpace loginAttempts = new SequentialSpace();



        private void getAccountCreationsService()
        {
            while (true)
            {

            }
        }

        private void getLoginAttemptsService(loginServer loginService)
        {
            while (true)
            {
                ITuple attempt = loginService.loginAttempts.Get(typeof(string), typeof(string));
                if (attempt != null)
                {
                    string user = (string)attempt[0];
                    string pass = (string)attempt[1];
                    ITuple userAcc = userAccounts.Get(user, typeof(string), typeof(byte[]));
                    account.generatePassHash(Encoding.UTF8.GetBytes(pass), userAcc[2] as byte[]);
                    if (attempt[1] == userAcc[1])
                    {
                        loggedInUsers.Put(user);
                    }
                }
              attempt = null;
            }
            
        }

        public loginServer() {
            
            loginServer loginService = new loginServer();
            Task.Factory.StartNew(() => getAccountCreationsService());
            Task.Factory.StartNew(() => getLoginAttemptsService(loginService));
            loginServerSpaces.AddGate("tcp://10.16.169.224:5001");
            loginServerSpaces.AddSpace("loggedInUsers", loggedInUsers);
            loginServerSpaces.AddSpace("loginAttempts", loginAttempts);
            loginServerSpaces.AddSpace("userAccounts", userAccounts);
        }

        //public loginServer(SequentialSpace userAccounts, SequentialSpace loggedInUsers)
        //{
        //    this.userAccounts = userAccounts;
        //    this.loggedInUsers = loggedInUsers;
        //}
    }

}









