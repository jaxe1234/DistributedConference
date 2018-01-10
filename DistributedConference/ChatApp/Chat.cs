using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Network.ConnectionModes;
using dotSpace.Objects.Space;
using NamingTools;


namespace ChatApp
{
    public class Chat
    {
        //string uri = "tcp://10.16.174.190:5002";
        public ISpace chatSpace { get; private set; }
        private int K = 0;
        private string LockedInUser;
        public Chat(bool isHost, string name, SpaceRepository chatRepo, string uri, string conferenceName)
        {
            this.LockedInUser = name;
            if (isHost)
            {
                Console.WriteLine("You are host!");
                chatSpace = new SequentialSpace();
                Console.WriteLine("Conference name: " + conferenceName + " with hash: " + NamingTool.GenerateUniqueSequentialSpaceName(conferenceName));
                chatRepo.AddSpace(NamingTool.GenerateUniqueSequentialSpaceName(conferenceName), chatSpace);
                chatRepo.AddGate(uri);
                
                
            }
            else
            {
                Console.WriteLine("You are a slave!");
                Console.WriteLine(NamingTool.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
                try
                {
                    chatSpace = new RemoteSpace(NamingTool.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception + "\t" + exception.Message);
                    throw;
                }
            }
        }

        private static string formatMessage(string formattedTimeString, string name, string message)
        {
            return $"[{formattedTimeString}]  {name}: {message}";
        }

        public void InitializeChat()
        {


            // my own stackoverflow post on this topic:
            // https://stackoverflow.com/questions/48128273/cancelling-parallel-tasks-with-thread-blocking-operations

            // this is how to cancel a task running a thread-blocking operation:
            // https://stackoverflow.com/questions/22735533/how-do-i-cancel-a-blocked-task-in-c-sharp-using-a-cancellation-token
            // https://msdn.microsoft.com/en-us/library/dd321315(v=vs.110).aspx
            // https://msdn.microsoft.com/en-us/library/hh160373(v=vs.110).aspx
            // http://www.c-sharpcorner.com/UploadFile/80ae1e/canceling-a-running-task/
            // https://binary-studio.com/2015/10/23/task-cancellation-in-c-and-things-you-should-know-about-it/

            var cancellationTokenSource = new CancellationTokenSource();

            var reader = Task.Run(async () =>
            {
                var temp = await Task<bool>.Factory.StartNew(() => ChatReader(chatSpace, cancellationTokenSource));//await ChatReader(chatSpace, cancellationTokenSource);
                Console.WriteLine("Reader was terminated");
                return temp;
            }, cancellationTokenSource.Token);
            
            var sender = new ChatSender(LockedInUser, chatSpace, cancellationTokenSource, this).RunAsConsole();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
            }
        }


        public bool ChatReader(ISpace chatSpace, CancellationTokenSource cancelTokenSource)
        {
            Console.WriteLine("Making chat-reader...");
            while (!cancelTokenSource.Token.IsCancellationRequested)
            {
                //Console.WriteLine("Getting messages...");
                var received = Task.Run(() =>
                {
                    return chatSpace.Query(K + 1, typeof(string), typeof(string), typeof(string));

                }, cancelTokenSource.Token).Result;



                K = (int) received[0];
                string formattedTimeString = (string) received[1];
                string receivedName = (string)received[2];
                string message = (string)received[3];
                
                Console.WriteLine(formatMessage(formattedTimeString, receivedName, message));
            }
            return true;
        }
        
        public class ChatSender
        {
            public string LockedInUser { get; private set; }
            public ISpace ChatSpace { get; private set; }
            public CancellationTokenSource CancelTokenSource { get; private set; }
            public Chat Chat { get; private set; }

            public ChatSender(string user, ISpace space, CancellationTokenSource source, Chat chat)
            {
                LockedInUser = user;
                ChatSpace = space;
                CancelTokenSource = source;
                Chat = chat;
            }

            public string SendMessage(string msg)
            {
                string fullMessage;
                DateTime time = DateTime.Now;
                string formattedTimeString = time.ToString("HH':'mm':'ss");
                try
                {
                    Chat.K++;
                    if (msg == "!quit" || msg == "!exit")
                    {
                        CancelTokenSource.Cancel();
                    }
                    else
                    {
                        ChatSpace.Put(Chat.K, formattedTimeString, LockedInUser, msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return formatMessage(formattedTimeString, LockedInUser, msg);
            }

            public bool RunAsConsole()
            {
                Console.WriteLine("Making chat-sender...");
                while (!CancelTokenSource.Token.IsCancellationRequested)
                {
                    string message = Console.ReadLine();
                    SendMessage(message);
                }
                return true;
            }
        }
    }

    
}
