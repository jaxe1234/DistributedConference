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

namespace Conference
{
    public class ConferenceInitializer
    {
        private string uri;
        public Chat chat { get; }

        public ConferenceInitializer(string name, string conferenceName, ObservableCollection<string> dataSource, SpaceRepository spaceRepo)//For the host
        {
            var hostentry = Dns.GetHostEntry("").AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            this.uri = "tcp://" + hostentry + ":5002";
            this.chat = new Chat(name,uri, conferenceName, spaceRepo,dataSource);
        }

        public ConferenceInitializer(string name, string conferenceName, string ip, ObservableCollection<string> dataSource)//For the client
        {
            this.uri = "tcp://" + ip + "5002";
            //this.chat = new Chat(name, uri,)
        }
    }
}
