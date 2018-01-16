using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp;
using System.Collections.ObjectModel;
using dotSpace.Objects.Network;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Conference
{
    public class ConferenceInitializer
    {
        private string uri;
        private string name;
        public Chat.ChatSender ChatSender { get; private set; }
        private Chat Chat { get; }
        private CancellationTokenSource tokenSource;

        public ConferenceInitializer(string name, string conferenceName, ObservableCollection<string> dataSource, SpaceRepository spaceRepo)//For the host
        {
            this.name = name;
            var hostentry = NamingTools.IpFetcher.GetLocalIpAdress();
            this.uri = "tcp://" + hostentry + ":5002";
            this.Chat = new Chat(name,uri, conferenceName, spaceRepo,dataSource);
            tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(InitChat);
        }

        public ConferenceInitializer(string name, string conferenceName, string ip, ObservableCollection<string> dataSource)//For the client
        {
            this.name = name;
            this.uri = "tcp://" + ip + ":5002";
            this.Chat = new Chat(name, uri, conferenceName, dataSource);
            tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(InitChat);
        }

        private void InitChat()
        {
            this.ChatSender = new Chat.ChatSender(name, Chat.ChatSpace, tokenSource, Chat);
            Chat.InitializeChat();
        }
    }
}
