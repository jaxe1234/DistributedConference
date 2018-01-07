using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatApp;
using dotSpace.Objects.Network;


namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "tcp://" + Dns.GetHostByName(Dns.GetHostName()).AddressList[0] + ":5002";
            
            ChatTest(args, uri);
            Console.WriteLine("Program has terminated");
        }

        private static void ChatTest(string[] args, string uri)
        {
            SpaceRepository spaceRepo = new SpaceRepository();
            new Chat(args[0].Equals("host"), args[0], spaceRepo, uri, args[1]).InitializeChat();
            Console.WriteLine("Chat is done.");
            spaceRepo.CloseGate(uri);
            //spaceRepo.Dispose();
            //Environment.Exit(0);
        }
    }
}
