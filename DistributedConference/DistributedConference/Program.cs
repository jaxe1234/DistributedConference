using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Network;


namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {
            SpaceRepository spaceRepo = new SpaceRepository();
            new ChatApp.ChatTest(args, spaceRepo);
        }
    }
}
