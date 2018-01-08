using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatApp;
using ConferenceLobbyUI;
using dotSpace.Objects.Network;


namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {

            var hostentry = Dns.GetHostEntry("").AddressList[0];
            string uri = "tcp://" + hostentry + ":5002";
            
            ChatTest(args, uri);
            //Console.WriteLine("Program has terminated");

            new ChatUI();
            
        }

        private static void ChatTest(string[] args, string uri)
        {
            using (var spaceRepo = new SpaceRepository())
            {
                new Chat(args[0].Equals("host"), args[0], spaceRepo, args[0].Equals("host") ? uri : args[2], args[1]).InitializeChat();
                Console.WriteLine("Chat is done.");
                spaceRepo.CloseGate(uri);
                //spaceRepo.Dispose();
                //Environment.Exit(0);
            }

        }
    }
}
