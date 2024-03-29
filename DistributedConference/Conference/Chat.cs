﻿using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using dotSpace.Interfaces.Network;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using ProjectUtilities;

namespace Conference
{
    public partial class Chat
    {
        public ISpace ChatSpace { get; }
        private int K { get; set; }
        private string LoggedInUser { get; }
        private ObservableCollection<string> DataSource { get; }
        public Chat(string name, string uri, string conferenceName, IRepository chatRepo, ObservableCollection<string> dataSource)
        {   //For host
            LoggedInUser = name;

            chatRepo.AddSpace(NameHashingTool.GenerateUniqueSequentialSpaceName(conferenceName), new SequentialSpace());
            string remoteName = NameHashingTool.GenerateUniqueRemoteSpaceUri(uri, conferenceName);
            ChatSpace = new RemoteSpace(remoteName);

            DataSource = dataSource;
        }

        public Chat(string name, string uri, string conferenceName, ObservableCollection<string> dataSource)
        {   //For client
            LoggedInUser = name;
            DataSource = dataSource;
            string remoteName = NameHashingTool.GenerateUniqueRemoteSpaceUri(uri, conferenceName);
            ChatSpace = new RemoteSpace(remoteName);
        }


        private static string FormatMessage(string name, string message)
        {
            return $"[{DateTime.Now:HH\':\'mm\':\'ss}]  {name}: {message}";
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
                var temp = await ChatReader(ChatSpace, cancellationTokenSource, DataSource);
                //Console.WriteLine("Reader was terminated");
                return temp;
            }, cancellationTokenSource.Token);


            while (!cancellationTokenSource.IsCancellationRequested)
            {
            }
        }


        public Task<bool> ChatReader(ISpace chatSpace, CancellationTokenSource cancelTokenSource, ObservableCollection<string> dataSource)
        {
            while (!cancelTokenSource.Token.IsCancellationRequested)
            { 
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

                if (received == null)
                {
                    cancelTokenSource.Cancel();
                    return Task.FromResult(false);
                }

                K = (int)received[0];
                var receivedName = (string)received[2];
                var message = (string)received[3];
                var finalMsg = FormatMessage(receivedName, message);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    dataSource.Add(finalMsg);
                });
            }
            return Task.FromResult(true);
        }
    }
}
