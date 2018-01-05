using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;

namespace ChatApp
{
    public class Chat
    {
        private ISpace ChatSpace;
        string uri = "tcp://127.0.0.1:5001";
        private string ConferenceName = "room";
        private int k = 0;
        private string LockedInUser = "Bob";
        public Chat(bool isHost, string name)
        {
            if (isHost)
            {
                Console.WriteLine("You are host!");
                ChatSpace = new SequentialSpace();
                SpaceRepository chatRepo = new SpaceRepository();
                chatRepo.AddSpace(ConferenceName, ChatSpace);
                chatRepo.AddGate(uri);
            }
            else
            {
                Console.WriteLine("You are a slave!");
                ChatSpace = new RemoteSpace(uri + "/" + ConferenceName);
            }

            Task.Run(() => ChatReader());
            Task.Run(() => ChatSender());
            }


        public void ChatReader()
        {
            Console.WriteLine("Making chat-reader...");
            while (true)
            {
                var recieved = ChatSpace.Query((k + 1), typeof(string), typeof(string));
                string name = (string)recieved[1];
                string message = (string)recieved[2];
                k++;
                Console.WriteLine(name + ": " + message);
            }
        }

        public void ChatSender()
        {
            Console.WriteLine("Making chat-sender...");
            while (true)
            {
                try
                {
                    string message = Console.ReadLine();
                    ChatSpace.Put(k, LockedInUser, message);
                    k++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
