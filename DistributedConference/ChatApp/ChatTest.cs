using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    public class ChatTest
    {
        public ChatTest(string[] args)
        {
            new Chat(args[0].Equals("host"), args[0]);
        }
    }
}
