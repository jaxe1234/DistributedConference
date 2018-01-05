using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using dotSpace.Objects.Network;
using dotSpace.Interfaces.Space;

namespace DistributedConference
{
    class login
    {
        private string username;
        private string password;
        private RemoteSpace LoginServer;


        public login(RemoteSpace LoginServer)
        {
            // this.username = username;
            // this.password = password;
            this.LoginServer = LoginServer;



        }

        public bool LogIn(string user, string pass)
        {
            // ALL OF THESE CHECKS NEED TO BE IN THE SERVER!!!!
            // ITuple attempt = LoginServer.QueryP(user, typeof(string), typeof(byte[]));
            //space should contain tuples of string username, string pwrd hash, byte[] salt
            RemoteSpace loginServer = new RemoteSpace("tcp://10.16.169.224:5001/" + "loginAttempts");
            loginServer.Put(user, pass);



            //attempt != null && attempt[1].Equals(account.generatePassHash(Encoding.UTF8.GetBytes(pass),attempt[2] as byte[]))) 



            return false;
        }


       



    }
}
