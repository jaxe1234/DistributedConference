using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Network;

namespace ChatApp
{
    public class ChatTest
    {
        public ChatTest(string[] args, SpaceRepository spaceRepo, string uri)
        {
            new Chat(args[0].Equals("host"), args[0], spaceRepo, uri, args[1]);
        }
    }
}
