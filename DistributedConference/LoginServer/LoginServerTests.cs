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
            string username = "jens";
            string password = "hunter2";

            Console.WriteLine("client: strings inited");
            loginServer testServer = new loginServer();
            Console.WriteLine("loginserver inited");
            var accountSpace = new RemoteSpace("tcp://10.16.169.224:5001/accountCreation");
            var loginSpace = new RemoteSpace("tcp://10.16.169.224:5001/loginAttempts");
         

            //CREATE NEW USER
            Console.WriteLine("client: Attempting to create new user...");
            accountSpace.Put(username, password);
            var result = accountSpace.Get(username, typeof(int));
            Console.WriteLine("client: received reply from server...");
            if ((int)result[1] == 1)
            {
                Console.WriteLine("client: success!");
            }
            else if((int)result[1] == 0)
            {
                Console.WriteLine("client: failure...");
            }
            //

            //CREATE EXISTING USER
            //Console.WriteLine("Attempting to create new user... expects failure");
            //accountSpace.Put(username, password);
            //var result1 = accountSpace.Get(username, typeof(int));
            //Console.WriteLine("received reply from server...");
            //if ((int)result[1] == 1)
            //{
            //    Console.WriteLine("success!");
            //}
            //else if ((int)result[1] == 0)
            //{
            //    Console.WriteLine("failure...");
            //}
            //


            Console.WriteLine("client: attempting login...");
            loginSpace.Put(username, password + "meme");

            var result3 = loginSpace.Get(username, typeof(int));
            if((int)result3[1] == 1)
            {
                Console.WriteLine("client: success!");
            }
            else if ((int)result3[1] == 0)
            {
                Console.WriteLine("client: failure...");
            }





        }
    }


}
