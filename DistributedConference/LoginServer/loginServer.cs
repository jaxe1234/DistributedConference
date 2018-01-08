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
        private SequentialSpace loginAttempts = new SequentialSpace();
        private SequentialSpace accountCreation = new SequentialSpace();
        private SequentialSpace loggedInUsers = new SequentialSpace(); //might need public so the rest of the server can validate who is online. might not matter at all.




        private void getAccountCreationService()
        {//To create a user, put at (username, password) tuple in accountCreation and check for confirmation
            while (true)
            {
                ITuple attempt = accountCreation.Get(typeof(string), typeof(string));
                if (attempt != null)// <---
                {
                    var existsInDB = userAccounts.QueryP(attempt[0], typeof(string), typeof(byte[]));
                    if(existsInDB == null)
                    {
                        account newUser = new account(attempt[0] as string, attempt[1] as string);
                        userAccounts.Put(newUser);
                        accountCreation.Put(attempt[0],1); //lav 1 til en enum for success
                    }
                    else
                    {
                        accountCreation.Put(attempt[0], 0);
                    }



                }


            }
        }

        private void getLoginAttemptsService()
        {
            while (true)
            {
                ITuple attempt = loginAttempts.Get(typeof(string), typeof(string)); //get er blocking = ingen null return
                if (attempt != null)// <---
                {
                    string user = (string)attempt[0];
                    string pass = (string)attempt[1];
                    ITuple userAcc = userAccounts.Query(user, typeof(string), typeof(byte[]));
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
            loginServerSpaces.AddGate("tcp://10.16.169.224:5001"); //tjek IP hver dag. just in case.
            //loginServerSpaces.AddSpace("loggedInUsers", loggedInUsers);
            loginServerSpaces.AddSpace("loginAttempts", loginAttempts);
            //loginServerSpaces.AddSpace("userAccounts", userAccounts);
            loginServerSpaces.AddSpace("accountCreation", accountCreation);
            //Not good. ikke alle spaces skal være remote. How do we into security?

            Task.Factory.StartNew(() => getAccountCreationService());
            Task.Factory.StartNew(() => getLoginAttemptsService());
           
        }

       
    }

}









