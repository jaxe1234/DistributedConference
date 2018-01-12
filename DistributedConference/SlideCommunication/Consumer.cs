using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using NamingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public abstract class Consumer : IDisposable
    {
        protected ISpace Space { get; }
        protected ISpace PrivateSpace { get; }
        
        private CancellationTokenSource Cacelation { get; set; }

        private bool _running;
        public bool Running {
            get { return _running; }
            set {
                if (value != _running)
                {
                    _running = value;
                    if (!_running)
                    {
                        Cacelation = new CancellationTokenSource();
                        Task.Run(GetHostAction(), Cacelation.Token);
                    }
                    else
                    {
                        Dispose();
                    }
                }
            }
        }

        protected abstract Action GetHostAction();

        public Consumer(ISpace space)
        {
            Space = space;
            PrivateSpace = new SequentialSpace();
        }

        public virtual void Dispose()
        {
            Cacelation.Cancel();
        }
    }

    public enum HostingMode
    {
        Unspecified,
        Idle,
        Master,
        Hub,
        Slave
    }
}
