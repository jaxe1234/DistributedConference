using dotSpace.Interfaces.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class FramePayload
    {
        public int PageNumber { get; set; }
        public byte[] Bitstream { get; set; }

        public FramePayload() { }
    }
}
