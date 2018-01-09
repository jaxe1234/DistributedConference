using dotSpace.Interfaces.Space;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class Consumer
    {
        private ISpace Space { get; set; }
        private string Username { get; set; }
        private string SessionKey { get; set; }

        public Consumer(ISpace space)
        {
            Space = space;
        }

        public void ConnectToSession()
        {
            Space.Put("Request", "Session", Username);
            SessionKey = Space.Get("Ack", "Session", Username, typeof(string))[3] as string;
        }
    }
}
