using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Network.ConnectionModes;
using dotSpace.Objects.Space;

namespace ChatApp
{
    public class Chat
    {

        string uri = "tcp://10.16.174.190:5002";
        private string ConferenceName = "chat";
        private int K = 0;
        private string LockedInUser;
        public Chat(bool isHost, string name, SpaceRepository chatRepo)
        {
            this.LockedInUser = name;
            if (isHost)
            {
                Console.WriteLine("You are host!");
                var ChatSpace = new SequentialSpace();
                chatRepo.AddSpace(ConferenceName, ChatSpace);
                chatRepo.AddGate(uri);
                Task.Run(() => ChatReader(ChatSpace));
                Task.Run(() => ChatSender(ChatSpace));
                ChatSpace.Get("done");
            }
            else
            {
                Console.WriteLine("You are a slave!");
                var ChatSpace = new RemoteSpace(uri + "/" + ConferenceName);
                Task.Run(() => ChatReader(ChatSpace));
                Task.Run(() => ChatSender(ChatSpace));
                ChatSpace.Get("done");
            }




        }


        public void ChatReader(ISpace ChatSpace)
        {
            Console.WriteLine("Making chat-reader...");
            while (true)
            {
                Console.WriteLine("Getting messages...");
                var received = ChatSpace.Query((K + 1), typeof(string), typeof(string));
                string receivedName = (string)received[1];
                string message = (string)received[2];
                if (!receivedName.Equals(LockedInUser))
                {
                    K++;
                }
                Console.WriteLine(receivedName + ": " + message);
            }
        }

        public void ChatSender(ISpace ChatSpace)
        {
            Console.WriteLine("Making chat-sender...");
            while (true)
            {
                try
                {
                    string message = Console.ReadLine();
                    K++;
                    Console.WriteLine("Your message was: " + message);
                    ChatSpace.Put(K, LockedInUser, message);

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
