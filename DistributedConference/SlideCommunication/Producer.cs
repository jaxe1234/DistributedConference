using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class Producer
    {
        private ISpace Space { get; set; }
        
        public Producer(ISpace space)
        {
            Space = space;
        }
    }
}
