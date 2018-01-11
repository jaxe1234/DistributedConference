using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dotSpace.Interfaces.Network;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Network.ConnectionModes;
using dotSpace.Objects.Space;
using NamingTools;


namespace ChatApp
{
    public class Chat
    {
        public ISpace ChatSpace { get; }
        private int K;
        private readonly string LoggedInUser;
        private readonly ObservableCollection<string> dataSource;
        public Chat(string name, string uri, string conferenceName, IRepository chatRepo, ObservableCollection<string> dataSource) //For host
        {
            this.LoggedInUser = name;

            Console.WriteLine("You are host!");
            ChatSpace = new SequentialSpace();
            Console.WriteLine("Conference name: " + conferenceName + " with hash: " + NamingTool.GenerateUniqueSequentialSpaceName(conferenceName));
            chatRepo.AddSpace(NamingTool.GenerateUniqueSequentialSpaceName(conferenceName), ChatSpace);
            chatRepo.AddGate(uri);
            this.dataSource = dataSource;
        }

        public Chat(string name, string uri, string conferenceName, ObservableCollection<string> dataSource) //For client
        {
            this.LoggedInUser = name;
            Console.WriteLine("You are a slave!");
            Console.WriteLine(NamingTool.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
            this.dataSource = dataSource;
            try
            {
                ChatSpace = new RemoteSpace(NamingTool.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + "\t" + exception.Message);
                throw;
            }
        }


        private static string FormatMessage(string formattedTimeString, string name, string message)
        {
            return $"[{formattedTimeString}]  {name}: {message}";
        }

        public void InitializeChat()
        {
            // this is how to cancel a task running a thread-blocking operation:
            // https://stackoverflow.com/questions/22735533/how-do-i-cancel-a-blocked-task-in-c-sharp-using-a-cancellation-token
            // https://msdn.microsoft.com/en-us/library/dd321315(v=vs.110).aspx
            // https://msdn.microsoft.com/en-us/library/hh160373(v=vs.110).aspx
            // http://www.c-sharpcorner.com/UploadFile/80ae1e/canceling-a-running-task/
            // https://binary-studio.com/2015/10/23/task-cancellation-in-c-and-things-you-should-know-about-it/

            var cancellationTokenSource = new CancellationTokenSource();

            var reader = Task.Run(async () =>
            {
                var temp = await ChatReader(ChatSpace, cancellationTokenSource, dataSource);
                Console.WriteLine("Reader was terminated");
                return temp;
            }, cancellationTokenSource.Token);

            //var sender = new ChatSender(LoggedInUser, ChatSpace, cancellationTokenSource, this).RunAsConsole();

            //while (!cancellationTokenSource.IsCancellationRequested)
            {
            }
        }


        public Task<bool> ChatReader(ISpace chatSpace, CancellationTokenSource cancelTokenSource, ObservableCollection<string> dataSource)
        {
            Console.WriteLine("Making chat-reader...");
            while (!cancelTokenSource.Token.IsCancellationRequested)
            {
                //Console.WriteLine("Getting messages...");
                ITuple received = null;

                received = Task.Run(() =>
                {
                    try
                    {
                        return chatSpace.Query(K + 1, typeof(string), typeof(string), typeof(string));
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    return null;
                }, cancelTokenSource.Token).Result;
                Console.WriteLine("received message");

                if (received == null)
                {
                    cancelTokenSource.Cancel();
                    return Task.FromResult(false);
                }

                K = (int)received[0];
                string formattedTimeString = (string)received[1];
                string receivedName = (string)received[2];
                string message = (string)received[3];
                string finalMsg = FormatMessage(formattedTimeString, receivedName, message);
                dataSource.Add(finalMsg);
                Console.WriteLine(finalMsg);
            }
            return Task.FromResult(true);
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
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    CancelTokenSource.Cancel();
                    return String.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return FormatMessage(formattedTimeString, LockedInUser, msg);
            }

            public Task<bool> RunAsConsole()
            {
                Console.WriteLine("Making chat-sender...");
                while (!CancelTokenSource.Token.IsCancellationRequested)
                {
                    string message = Console.ReadLine();
                    SendMessage(message);
                }
                return Task.FromResult(true);
            }
        }
    }
}
