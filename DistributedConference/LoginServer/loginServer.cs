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




        private void GetAccountCreationService()
        {//To create a user, put at (username, password) tuple in accountCreation and check for confirmation
            while (true)
            {
                ITuple attempt = accountCreation.Get(typeof(string), typeof(string));
                if (attempt != null)// <---
                {
                    Console.WriteLine("server: saw request for user creation. With input " + attempt[0] + " " + attempt[1]);
                    var existsInDB = userAccounts.QueryAll(typeof(account)); //attempt[0], typeof(string), typeof(byte[])
                    var usernmTaken = existsInDB.Any(t => (t[0] as account).username ==(attempt[0] as string));
                    //var usernmTaken = existsInDB.Select(t => t[0] as account).Any(a => a.username == (attempt[0] as string));
                    if (!usernmTaken)
                    {
                        account newUser = new account(attempt[0] as string, attempt[1] as string);
                        userAccounts.Put(newUser);
                       // Console.WriteLine(newUser.username + " " + newUser.hash);

                        accountCreation.Put(attempt[0],1); //lav 1 til en enum for success
                        Console.WriteLine("server: created user");
                    }
                    else
                    {
                        accountCreation.Put(attempt[0], 0);
                        Console.WriteLine("server: rejected user creation");
                    }



                }


            }
        }

        private void GetLoginAttemptsService()
        {
            while (true)
            {
                ITuple attempt = loginAttempts.Get(typeof(string), typeof(string)); //get er blocking = ingen null return
                if (attempt != null)// <---
                {
                   
                    string user = (string)attempt[0];
                    string pass = (string)attempt[1];
                    var userAccs = userAccounts.QueryAll(typeof(account));
                    var userAccount = userAccs.Select(t => t[0] as account).FirstOrDefault(a => a.username == user);
                    if (userAccount != null) {
                        
                        if (userAccount.hash == (account.generatePassHash(Encoding.UTF8.GetBytes(pass), userAccount.salt)))
                        {
                            loggedInUsers.Put(user);
                            loginAttempts.Put(user, 1);
                            Console.WriteLine("server: deposited results succ");
                        }
                        else
                        {
                            loginAttempts.Put(user, 0);
                            Console.WriteLine("server: deposited results fail");
                        }
                       
                    }
                    else
                    {
                        loginAttempts.Put(user, 0);
                        Console.WriteLine("server: deposited results null");
                    }
                }
              attempt = null;
            }
            
        }

        public loginServer() {
            
            
            loginServerSpaces.AddGate("tcp://10.16.169.224:5001"); //tjek IP hver dag. just in case.
            //loginServerSpaces.AddSpace("loggedInUsers", loggedInUsers);
            loginServerSpaces.AddSpace("loginAttempts", loginAttempts);
            //loginServerSpaces.AddSpace("userAccounts", userAccounts);
            loginServerSpaces.AddSpace("accountCreation", accountCreation);
            //Not good. ikke alle spaces skal være remote. How do we into security?

            Task.Factory.StartNew(() => GetAccountCreationService());
            Task.Factory.StartNew(() => GetLoginAttemptsService());
            

        }

       
    }

}









