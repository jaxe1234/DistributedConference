using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;

namespace Conference
{
    public partial class Chat
    {
        public class ChatSender
        {
            public string LoggedInUser { get; }
            public ISpace ChatSpace { get; }
            public CancellationTokenSource CancelTokenSource { get; }
            public Chat Chat { get; }

            public ChatSender(string user, ISpace space, CancellationTokenSource source, Chat chat)
            {
                LoggedInUser = user;
                ChatSpace = space;
                CancelTokenSource = source;
                Chat = chat;
            }

            public string SendMessage(string msg)
            {
                var formattedTimeString = DateTime.Now.ToString("HH':'mm':'ss");
                try
                {
                    Chat.K++;
                    ChatSpace.Put(Chat.K, formattedTimeString, LoggedInUser, msg);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    CancelTokenSource.Cancel();
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return FormatMessage(LoggedInUser, msg);
            }

            public Task<bool> RunAsConsole()
            {
                Console.WriteLine("Making chat-sender...");
                while (!CancelTokenSource.Token.IsCancellationRequested)
                {
                    var message = Console.ReadLine();
                    SendMessage(message);
                    if (message == "!quit" || message == "!exit")
                    {
                        CancelTokenSource.Cancel();
                    }
                }
                return Task.FromResult(true);
            }
        }
    }
}
