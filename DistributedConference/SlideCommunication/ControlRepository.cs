using dotSpace.Interfaces.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class ControlRepository
    {
        private ISpace ExposedControl { get; set; }
        private ISpace ConcealedControl { get; set; }


        public string ControlToken => ExposedControl.QueryP("token", typeof(string))?.Get<string>(1);

    }
}
