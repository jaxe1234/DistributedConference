using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string _name;
        public Chat.ChatSender ChatSender { get; private set; }
        private Chat Chat { get; }
        private readonly CancellationTokenSource _tokenSource;

        public SlideClientFacade Client { get; private set; }
        public SlideHostFacade Host { get; private set; }

        public ConferenceInitializer(string username, string conferenceName, ObservableCollection<string> dataSource, SpaceRepository spaceRepo, ISlideShow slideShower)//For the host
        {//for host
            _name = username;

            var hostentry = ProjectUtilities.IpFetcher.GetLocalIpAdress();
            var uri = $"tcp://{hostentry}:5002";
            spaceRepo.AddGate(uri + "?CONN");
            Chat = new Chat(username, uri, conferenceName, spaceRepo, dataSource);
            Host = new SlideHostFacade(spaceRepo, username, slideShower);
            Host.Control.Controlling = true;
            _tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(InitChat);
        }

        public ConferenceInitializer(string username, string conferenceName, string ip, ObservableCollection<string> dataSource, ISlideShow slideShower)//For the client
        {//for client
            _name = username;
            var uri = "tcp://" + ip + ":5002";
            Chat = new Chat(username, uri, conferenceName, dataSource);
            _tokenSource = new CancellationTokenSource();
            Client = new SlideClientFacade(slideShower, uri + "/?CONN", username);
            Client.Running = true;

            Task.Factory.StartNew(InitChat);
        }

        private void InitChat()
        {
            ChatSender = new Chat.ChatSender(_name, Chat.ChatSpace, _tokenSource, Chat);
            Chat.InitializeChat();
        }
    }
}
