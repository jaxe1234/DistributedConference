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
    class LoginServer
    {
        readonly string _privKey;
        readonly SpaceRepository _loginServerSpaces = new SpaceRepository();
        private readonly SequentialSpace _userAccounts = new SequentialSpace();
        private readonly SequentialSpace _conferences = new SequentialSpace();
        private readonly SequentialSpace _getConferences = new SequentialSpace();
        private readonly SequentialSpace _loginAttempts = new SequentialSpace();
        private readonly SequentialSpace _accountCreation = new SequentialSpace();
        private readonly SequentialSpace _loggedInUsers = new SequentialSpace(); //might need public so the rest of the server can validate who is online. might not matter at all.


        private void GetAccountCreationService()
        {//To create a user, put at (username, password) tuple in accountCreation and check for confirmation
            while (true)
            {
                ITuple attempt = _accountCreation.Get(typeof(string), typeof(string));

                var username = (string)attempt[0];
                var password = (string)attempt[1];

                Console.WriteLine("server: saw request for user creation. With input " + username + " " + password);
                try
                {
                    var exists = SelectAccount(username);
                    _accountCreation.Put(username, 0);
                    Console.WriteLine("server: rejected user creation");
                }
                catch (Exception)
                {
                    var newUser = new Account(username, password);
                    _userAccounts.Put(newUser);
                    _accountCreation.Put(username, 1); //lav 1 til en enum for success
                    Console.WriteLine("server: created user");
                }
            }
        }


        private void GetLoginAttemptsService()
        {
            while (true)
            {
                ITuple attempt = _loginAttempts.Get(typeof(string), typeof(string));
                Console.WriteLine("saw login request");
                var user = attempt[0] as string;
                var pass = attempt[1] as string;

                try
                {
                    SelectAccount(user); // don't remove, used to check whether account exists. Because accountExists(user) is hard
                    if (BoolFromRawPassAndUser(pass, user) && (_loggedInUsers.QueryP(user) == null))
                    {
                        _loggedInUsers.Put(user);
                        _loginAttempts.Put(user, 1);
                        Console.WriteLine("server: deposited results succ");
                    }
                    else
                    {
                        _loginAttempts.Put(user, 0);
                        Console.WriteLine("server: deposited results fail");
                    }
                }
                catch (Exception)
                {
                    _loginAttempts.Put(user, 0);
                    Console.WriteLine("server: deposited results null");
                }
            }
        }


        private void GetLogoutAttemptsService()
        {
            while (true)
            {
                ITuple attempt = _loginAttempts.Get("logout", typeof(string), typeof(string));
                Console.WriteLine("saw logout request");
                var user = attempt[1] as string;
                var pass = attempt[2] as string;

                try
                {
                    if (BoolFromRawPassAndUser(pass, user) && _loggedInUsers.QueryP(user) != null)
                    {
                        _loggedInUsers.Get(user);
                        Console.WriteLine("logged out " + user);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("malformed logout request");
                }
            }
        }


        private bool IsAuthorized(string username, string password)
        {
            //string RawPass = RSADecrypt(password, PrivKey);
            var foundUser = SelectAccount(username);
            return foundUser != null && BoolFromRawPassAndUser(password, username) && _loggedInUsers.QueryP(username) != null;
        }
        private bool BoolFromRawPassAndUser(string raw, string Username)
        {
            raw = RSADecrypt(raw, _privKey);
            var foundUser = SelectAccount(Username);
            return foundUser.Hash == Account.GeneratePassHash(Encoding.UTF8.GetBytes(raw), foundUser.Salt);
        }
        private Account SelectAccount(string Username)
        {
            var list = _userAccounts.QueryAll(typeof(Account));
            var returnVal = list.Select(t => t[0] as Account).FirstOrDefault(a => a.Username == Username);
            if (returnVal != null)
            {
                return returnVal;
            }
            throw new Exception("no such user");

        }


        private void GetConferenceListService()
        {
            while (true)
            {
                var request = _getConferences.Get(typeof(string), typeof(string), typeof(string), typeof(int), typeof(string));
                var username = (string)request[0];
                var conferenceName = (string)request[1];
                var ipOfConference = (string)request[2];
                var requestType = (int)request[3];
                var pass = (string)request[4];

                SelectAccount(username); // don't remove

                Console.WriteLine("got request to create or delete conference");
                if (!IsAuthorized(username, pass))
                {
                    Console.WriteLine("User was not authorized");
                    _getConferences.Put("Result", 0, username);
                    break;
                }
                if (requestType == 1)
                {
                    //add
                    _conferences.Put(username, conferenceName, ipOfConference);
                    List<string> confList = _conferences.QueryAll(typeof(string), typeof(string), typeof(string)).Select(t => t.Get<string>(1)).ToList();
                    _getConferences.Get(typeof(List<string>));
                    _getConferences.Put(confList);
                    Console.WriteLine("added conference " + conferenceName);
                    _getConferences.Put("Result", 1, username);
                }
                if ((int)request[3] == 0)
                {
                    //remove
                    _conferences.Get(username, conferenceName, ipOfConference);
                    List<string> confList = _conferences.QueryAll(typeof(string), typeof(string), typeof(string)).Select(t => t.Get<string>(1)).ToList();
                    _getConferences.Get(typeof(List<string>));
                    _getConferences.Put(confList);
                    Console.WriteLine("removed conference " + conferenceName);
                }
            }
        }

        private void GetIpService()
        {
            while (true)
            {
                var request = _getConferences.Get(typeof(string), typeof(string), 0, typeof(string));
                var username = (string)request[0];
                var conferenceName = (string)request[1];
                var pass = (string)request[3];
                if (!IsAuthorized(username, pass))
                {
                    Console.WriteLine("User was not authorized");
                    _getConferences.Put(username, "", 0);

                    break;
                }

                var result = _conferences.Query(typeof(string), conferenceName, typeof(string));
                var ipOfConference = (string)result[2];
                _getConferences.Put(username, ipOfConference, 1);
            }
        }

        public static string RSADecrypt(string password, string privKey)
        {
            byte[] bytePass = Convert.FromBase64String(password);
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privKey);
            byte[] decryptedData = rsa.Decrypt(bytePass, true);
            rsa.Dispose();
            return Encoding.UTF8.GetString(decryptedData);
        }

        public LoginServer()
        {
            var rsa = new RSACryptoServiceProvider();
            var pubKey = rsa.ToXmlString(false);
            _privKey = rsa.ToXmlString(true);

            //string DecryptedPasword = RSADecrypt(someThing, prikey);
            var ip = IpFetcher.GetLocalIpAdress();
            _loginServerSpaces.AddGate("tcp://" + ip + ":5001?CONN");
            //loginServerSpaces.AddSpace("loggedInUsers", loggedInUsers);
            _loginServerSpaces.AddSpace("loginAttempts", _loginAttempts);
            //loginServerSpaces.AddSpace("userAccounts", userAccounts);
            _loginServerSpaces.AddSpace("accountCreation", _accountCreation);
            _loginServerSpaces.AddSpace("getConferenceList", _getConferences);
            _loginAttempts.Put(pubKey);

            _getConferences.Put(new List<string>());

            Task.Factory.StartNew(GetAccountCreationService);
            Task.Factory.StartNew(GetLoginAttemptsService);
            Task.Factory.StartNew(GetConferenceListService);
            Task.Factory.StartNew(GetIpService);
            Task.Factory.StartNew(GetLogoutAttemptsService);
        }
    }
}