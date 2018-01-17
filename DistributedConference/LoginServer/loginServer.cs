using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Space;
using dotSpace.Objects.Network;
using dotSpace.Interfaces.Space;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Security.Cryptography;
using NamingTools;

namespace LoginServer
{
    class loginServer
    {

        string PrivKey;
        SpaceRepository loginServerSpaces = new SpaceRepository();
        private SequentialSpace userAccounts = new SequentialSpace();
        private SequentialSpace conferences = new SequentialSpace();
        private SequentialSpace getConferences = new SequentialSpace();
        private SequentialSpace loginAttempts = new SequentialSpace();
        private SequentialSpace accountCreation = new SequentialSpace();
        private SequentialSpace loggedInUsers = new SequentialSpace(); //might need public so the rest of the server can validate who is online. might not matter at all.




        private void GetAccountCreationService()
        {//To create a user, put at (username, password) tuple in accountCreation and check for confirmation
            while (true)
            {

                //spoghetti
                ITuple attempt = accountCreation.Get(typeof(string), typeof(string));
                if (attempt != null)// <---
                {
                    var username = (string)attempt[0];
                    var password = (string)attempt[1];

                    Console.WriteLine("server: saw request for user creation. With input " + username + " " + password);
                    try
                    {
                        var exists = selectAccount(username);
                        accountCreation.Put(username, 0);
                        Console.WriteLine("server: rejected user creation");
                    }
                    catch (Exception)
                    {
                        Account newUser = new Account(username, password);
                        userAccounts.Put(newUser);
                        accountCreation.Put(username, 1); //lav 1 til en enum for success
                        Console.WriteLine("server: created user");
                    }

                }

            }
        }


        private void GetLoginAttemptsService()
        {
            while (true)
            {

                //spoghetti
                ITuple attempt = loginAttempts.Get(typeof(string), typeof(string)); //get er blocking = ingen null return
                if (attempt != null)// <---
                {
                    Console.WriteLine("saw login request");
                    string user = (string)attempt[0];
                    string pass = attempt[1] as string;

                    try
                    {
                        var userAccount = selectAccount(user);
                        if (boolFromRawPassAndUser(pass, user) && (loggedInUsers.QueryP(user) == null))
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
                    catch (Exception)
                    {
                        loginAttempts.Put(user, 0);
                        Console.WriteLine("server: deposited results null");
                    }

                }
                //spohetti





                attempt = null;
            }

        }


        private void GetLogoutAttemptsService()
        {
            while (true)
            {

                //spoghetti
                ITuple attempt = loginAttempts.Get("logout", typeof(string), typeof(string)); //get er blocking = ingen null return
                if (attempt != null)// <---
                {
                    Console.WriteLine("saw logout request");
                    string user = (string)attempt[1];
                    string pass = attempt[2] as string;

                    try
                    {
                        if(boolFromRawPassAndUser(pass, user) && !(loggedInUsers.QueryP(user) == null))
                        {
                            loggedInUsers.Get(user);
                            Console.WriteLine("logged out " + user);
                        }
                       
                     

                    }
                    catch (Exception)
                    {
                        
                        Console.WriteLine("malformed logout request");
                    }

                }
                //spohetti





                attempt = null;
            }

        }





        private bool IsAuthorized(string username, string password)
        {

            //string RawPass = RSADecrypt(password, PrivKey);
            var foundUser = selectAccount(username);
            if (foundUser != null && boolFromRawPassAndUser(password, username) && loggedInUsers.QueryP(username) != null)
            {
                return true;
            }
            return false;


        }
        private bool boolFromRawPassAndUser(string raw, string Username)
        {
            raw = RSADecrypt(raw, PrivKey);
            var foundUser = selectAccount(Username);
            return foundUser.Hash == (Account.GeneratePassHash(Encoding.UTF8.GetBytes(raw), foundUser.Salt));
        }
        private Account selectAccount(string Username)
        {
            var list = userAccounts.QueryAll(typeof(Account));
            var returnVal = list.Select(t => t[0] as Account).FirstOrDefault(a => a.Username == Username);
            if (returnVal != null)
            {
                return returnVal;
            }
            else
            {
                throw new Exception("no such user");
            }
        }


        private void GetConferenceListService()
        {

            while (true)
            {
                var request = getConferences.Get(typeof(string), typeof(string), typeof(string), typeof(int), typeof(string));
                var username = (string)request[0];
                var conferenceName = (string)request[1];
                var ipOfConference = (string)request[2];
                var requestType = (int)request[3];
                var pass = (string)request[4];

                var account = selectAccount(username);

                Console.WriteLine("got request to create or delete conference");
                if (!IsAuthorized(username, pass))
                {
                    Console.WriteLine("User was not authorized");
                    getConferences.Put("Result", 0, username);
                    break;
                }
                if (requestType == 1)
                {
                    //add
                    conferences.Put(username, conferenceName, ipOfConference);
                    List<string> confList = conferences.QueryAll(typeof(string), typeof(string), typeof(string)).Select(t => t.Get<string>(1)).ToList();
                    getConferences.Get(typeof(List<string>));
                    getConferences.Put(confList);
                    Console.WriteLine("added conference " + conferenceName);
                    getConferences.Put("Result", 1, username);
                }
                if ((int)request[3] == 0)
                {
                    //remove
                    conferences.Get(username, conferenceName, ipOfConference);
                    List<string> confList = conferences.QueryAll(typeof(string), typeof(string), typeof(string)).Select(t => t.Get<string>(1)).ToList();
                    getConferences.Get(typeof(List<string>));
                    getConferences.Put(confList);
                    Console.WriteLine("removed conference " + conferenceName);


                }
            }
        }

        private void GetIPService()
        {
            while (true)
            {
                var request = getConferences.Get(typeof(string), typeof(string), 0, typeof(string));
                var username = (string)request[0];
                var conferenceName = (string)request[1];
                var pass = (string)request[3];
                if (!IsAuthorized(username, pass))
                {
                    Console.WriteLine("User was not authorized");
                    getConferences.Put(username, "", 0);

                    break;
                }

                var result = conferences.Query(typeof(string), conferenceName, typeof(string));
                var ipOfConference = (string)result[2];
                getConferences.Put(username, ipOfConference, 1);
            }
        }




        public static string RSADecrypt(string Password, string PrivKey)
        {
            byte[] BytePass = Convert.FromBase64String(Password);// Encoding.ASCII.GetBytes(Password);
            byte[] decryptedData;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PrivKey);
            decryptedData = rsa.Decrypt(BytePass, true);
            rsa.Dispose();
            return Encoding.UTF8.GetString(decryptedData);

        }

        public loginServer()
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string PubKey = rsa.ToXmlString(false);
            PrivKey = rsa.ToXmlString(true);

            //string DecryptedPasword = RSADecrypt(someThing, prikey);
            string ip = IpFetcher.GetLocalIpAdress();
            loginServerSpaces.AddGate("tcp://" + ip + ":5001?CONN"); //tjek IP hver dag. just in case.
            //loginServerSpaces.AddSpace("loggedInUsers", loggedInUsers);
            loginServerSpaces.AddSpace("loginAttempts", loginAttempts);
            //loginServerSpaces.AddSpace("userAccounts", userAccounts);
            loginServerSpaces.AddSpace("accountCreation", accountCreation);
            loginServerSpaces.AddSpace("getConferenceList", getConferences);
            loginAttempts.Put(PubKey);
            //Not good. ikke alle spaces skal være remote. How do we into security?

            getConferences.Put(new List<string>());



            Task.Factory.StartNew(() => GetAccountCreationService());
            Task.Factory.StartNew(() => GetLoginAttemptsService());
            Task.Factory.StartNew(() => GetConferenceListService());
            Task.Factory.StartNew(() => GetIPService());
            Task.Factory.StartNew(() => GetLogoutAttemptsService());


        }


    }

}









