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
using dotSpaceUtilities;


namespace ChatApp
{
    public class Chat
    {
        private bool ContinueSession = true;
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
                Console.WriteLine("Conference name: " + RepoUtility.GenerateUniqueSequentialSpaceName(conferenceName));
                chatRepo.AddSpace(RepoUtility.GenerateUniqueSequentialSpaceName(conferenceName), chatSpace);
                chatRepo.AddGate(uri);
            }
            else
            {
                Console.WriteLine("You are a slave!");
                Console.WriteLine(RepoUtility.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
                try
                {
                    chatSpace = new RemoteSpace(RepoUtility.GenerateUniqueRemoteSpaceUri(uri, conferenceName));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception + "\t" + exception.Message);
                    throw;
                }
            }
        }

        public void InitializeChat()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancelToken = cancellationTokenSource.Token;

            // my own stackoverflow post on this topic:
            // https://stackoverflow.com/questions/48128273/cancelling-parallel-tasks-with-thread-blocking-operations

            // this is how to cancel a task running a thread-blocking operation:
            // https://stackoverflow.com/questions/22735533/how-do-i-cancel-a-blocked-task-in-c-sharp-using-a-cancellation-token
            // https://msdn.microsoft.com/en-us/library/dd321315(v=vs.110).aspx
            // https://msdn.microsoft.com/en-us/library/hh160373(v=vs.110).aspx
            // http://www.c-sharpcorner.com/UploadFile/80ae1e/canceling-a-running-task/
            // https://binary-studio.com/2015/10/23/task-cancellation-in-c-and-things-you-should-know-about-it/

            var reader = Task.Run(async () =>
            {
                var temp = await ChatReader(chatSpace);
                Console.WriteLine("Reader was terminated");
                return temp;
            });
            var sender = Task.Run(async () =>
            {
                var temp = await ChatSender(chatSpace);
                Console.WriteLine("Sender was terminated");
                return temp;
            });
            try
            {
                reader.Wait(cancelToken);
                sender.Wait(cancelToken);
            }
            catch (Exception ex)
            {
                // ignored
            }


            while (ContinueSession)
            {
            }
            cancellationTokenSource.Cancel();
        }


        public async Task<bool> ChatReader(ISpace ChatSpace)
        {
            Console.WriteLine("Making chat-reader...");
            while (ContinueSession)
            {
                //Console.WriteLine("Getting messages...");
                var received = ChatSpace.Query(K + 1, typeof(string), typeof(string));
                string receivedName = (string)received[1];
                string message = (string)received[2];
                try
                {
                    if (!receivedName.Equals(LockedInUser))
                    {
                        K++;
                    }
                    else if (message == "!quit" || message == "!exit")
                    {
                        throw new ConferenceTransmissionEndedException(
                            "You have ended your transmission.");
                    }
                }
                catch (ConferenceTransmissionEndedException e)
                {
                    ContinueSession = false;
                    return (false);
                }



                Console.WriteLine(K + ":\t" + receivedName + ": " + message);
            }
            return (false);
        }

        public async Task<bool> ChatSender(ISpace ChatSpace)
        {
            Console.WriteLine("Making chat-sender...");
            while (ContinueSession)
            {
                try
                {
                    string message = Console.ReadLine();
                    K++;
                    //Console.WriteLine("Your message was: " + message);

                    ChatSpace.Put(K, LockedInUser, message);
                    if (message == "!quit" || message == "!exit")
                    {
                        throw new ConferenceTransmissionEndedException(
                            "You have ended your transmission.");
                    }
                }
                catch (ConferenceTransmissionEndedException e)
                {
                    ContinueSession = false;
                    return (false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return (false);
        }
    }
}
