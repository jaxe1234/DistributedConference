using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class Producer : IDisposable
    {
        private ISpace Space { get; set; }
        
        // SessionKey -> username
        private IDictionary<string, string> Sessions { get; set; }

        private CancellationTokenSource Cacelation { get; set; }

        public Producer(ISpace space)
        {
            Space = space;

            Cacelation = new CancellationTokenSource();

            var task = Task.Run(() => Listen(), Cacelation.Token);
        }

        public void Initialize()
        {

        }

        private void Listen()
        {
            while (true)
            {
                var request = Space.Get("Request", typeof(string), typeof(string));
            }
        }

        public void Dispose()
        {
            Cacelation.Cancel();
        }
    }
}
