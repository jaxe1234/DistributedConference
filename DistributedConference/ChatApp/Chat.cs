using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    public class Chat
    {
        private SpaceRepository ChatSpace;
        String uri = "tcp://127.0.0.1:5001";
        private String ConferenceName = "room";
        public Chat(bool isHost)
        {
            if (isHost)
            {
                ChatSpace = new SpaceRepository();
            }
            else
            {
                ChatSpace = new RemoteRepository(uri + "/" + ConferenceName);
            }
        }
    }
}
