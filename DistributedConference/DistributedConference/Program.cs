using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Network;


namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "tcp://" + Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString() + ":5002";

            SpaceRepository spaceRepo = new SpaceRepository();
            new ChatApp.ChatTest(args, spaceRepo, uri);
        }
    }
}
