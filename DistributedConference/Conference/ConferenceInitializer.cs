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
using SlideCommunication;

namespace Conference
{
    public class ConferenceInitializer
    {
        private string _uri;
        private string _name;
        public Chat.ChatSender ChatSender { get; private set; }
        private Chat Chat { get; }
        private CancellationTokenSource tokenSource;

        public SlideClientFacade Client { get; private set; }
        public SlideHostFacade Host { get; private set; }

        public ConferenceInitializer(string username, string conferenceName, ObservableCollection<string> dataSource, SpaceRepository spaceRepo, ISlideShow slideShower)//For the host
        {
            _name = username;
            var hostentry = NamingTools.IpFetcher.GetLocalIpAdress();
            _uri = "tcp://" + hostentry + ":5002";
            Chat = new Chat(username, _uri, conferenceName, spaceRepo, dataSource);
            Host = new SlideHostFacade(spaceRepo, username, slideShower);

            tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(InitChat);
        }

        public ConferenceInitializer(string username, string conferenceName, string ip, ObservableCollection<string> dataSource, ISlideShow slideShower)//For the client
        {
            _name = username;
            _uri = "tcp://" + ip + ":5002";
            Chat = new Chat(username, _uri, conferenceName, dataSource);
            tokenSource = new CancellationTokenSource();
            Client = new SlideClientFacade(slideShower, _uri, username);

            Task.Factory.StartNew(InitChat);
        }

        private void InitChat()
        {
            ChatSender = new Chat.ChatSender(_name, Chat.ChatSpace, tokenSource, Chat);
            Chat.InitializeChat();
        }
    }
}
