﻿using System;
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
            ISpace chatSpace;
            if (isHost)
            {
                Console.WriteLine("You are host!");
                chatSpace = new SequentialSpace();
                Console.WriteLine(RepoUtility.GenerateUniqueSequentialSpaceName(conferenceName));
                chatRepo.AddSpace(RepoUtility.GenerateUniqueSequentialSpaceName(conferenceName), chatSpace);
                chatRepo.AddGate(uri);
                
                InitializeChat(chatSpace);
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

                InitializeChat(chatSpace);
            }
            
        }

        public void InitializeChat(ISpace chatSpace)
        {
            Task<bool> reader = Task.Run(() => ChatReader(chatSpace));
            Task<bool> sender = Task.Run(() => ChatSender(chatSpace));
            if (reader == Task.FromResult(false) && sender == Task.FromResult(false))
            {
                Console.WriteLine("Chat session was eneded");
                return;
            }

            while (ContinueSession)
            {
            }

            /*
                if (Task.Run(async () => await ChatReader(chatSpace)) == Task.FromResult(false) ||
                    Task.Run(async () => await ChatSender(chatSpace)) == Task.FromResult(false))
                {
                    // https://stackoverflow.com/questions/31513409/async-method-to-return-true-or-false-in-a-task
                    // the idea of this if-statement is to run the sender and reader in a constant check for a return statement
                    // if either one returns false we can simply terminate the chat client altogether
                    // this can be done by putting "done" in the ChatSpace
                    return;
                }
                chatSpace.Get("done");
             */
        }


        public Task<bool> ChatReader(ISpace ChatSpace)
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
                    return Task.FromResult(false);
                }
                


                Console.WriteLine(receivedName + ": " + message);
            }
            return Task.FromResult(false);
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
                    return Task.FromResult(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return Task.FromResult(false);
        }
    }
}
