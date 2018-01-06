using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Network.ConnectionModes;
using dotSpace.Objects.Space;
using dotSpaceUtilities;


namespace ChatApp
{
    public class Chat
    {
        private bool ContinueSession = true;
        //string uri = "tcp://10.16.174.190:5002";
        private int K = 0;
        private string LockedInUser;
        public Chat(bool isHost, string name, SpaceRepository chatRepo, string uri, string conferenceName)
        {
            this.LockedInUser = name;
            if (isHost)
            {
                Console.WriteLine("You are host!");
                var ChatSpace = new SequentialSpace();
                Console.WriteLine(RepoUtility.GenerateUniqueSequentialSpaceName(conferenceName));
                chatRepo.AddSpace(RepoUtility.GenerateUniqueSequentialSpaceName(conferenceName), ChatSpace);
                chatRepo.AddGate(uri);
                if (Task.Run(() => ChatReader(ChatSpace)) == Task.FromResult(false) ||
                    Task.Run(() => ChatSender(ChatSpace)) == Task.FromResult(false))
                {
                    // https://stackoverflow.com/questions/31513409/async-method-to-return-true-or-false-in-a-task
                    // the idea of this if-statement is to run the sender and reader in a constant check for a return statement
                    // if either one returns false we can simply terminate the chat client altogether
                    // this can be done by putting "done" in the ChatSpace
                    ChatSpace.Put("done");
                }
                ChatSpace.Get("done");
            }
            else
            {
                Console.WriteLine("You are a slave!");
                Console.WriteLine(RepoUtility.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
                var ChatSpace = new RemoteSpace(RepoUtility.GenerateUniqueRemoteSpaceUri(uri, conferenceName));

                if (Task.Run(() => ChatReader(ChatSpace)) == Task.FromResult(false) ||
                    Task.Run(() => ChatSender(ChatSpace)) == Task.FromResult(false))
                {
                    ChatSpace.Put("done");
                }

                ChatSpace.Get("done");
            }
            
        }


        public Task<bool> ChatReader(ISpace ChatSpace)
        {
            Console.WriteLine("Making chat-reader...");
            while (ContinueSession)
            {
                //Console.WriteLine("Getting messages...");
                var received = ChatSpace.Query((K + 1), typeof(string), typeof(string));
                string receivedName = (string)received[1];
                string message = (string)received[2];
                if (!receivedName.Equals(LockedInUser))
                {
                    K++;
                }
                Console.WriteLine(receivedName + ": " + message);
            }
            return Task.FromResult(true);
        }

        public Task<bool> ChatSender(ISpace ChatSpace)
        {
            Console.WriteLine("Making chat-sender...");
            while (ContinueSession)
            {
                try
                {
                    string message = Console.ReadLine();
                    K++;
                    //Console.WriteLine("Your message was: " + message);
                    if (message == "!quit" || message == "!exit")
                    {
                        throw new ConferenceTransmissionEndedException(
                            "You have ended your transmission with the conference holder.");
                    }
                    ChatSpace.Put(K, LockedInUser, message);

                }
                catch (ConferenceTransmissionEndedException e)
                {
                    Console.WriteLine(e.Message);
                    ContinueSession = false;
                    ChatSpace.Put("done");
                    return Task.FromResult(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return Task.FromResult(true);
        }
    }
}
