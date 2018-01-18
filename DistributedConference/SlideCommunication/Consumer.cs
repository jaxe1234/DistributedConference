using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using ProjectUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlideCommunication
{
    /// <summary>
    /// This class is not in a 1:1 relation to pattern described in the course. 
    /// Classes derived from this class could implemented other pattern than the 
    /// consumer pattern and in some cases not even implement the consumer pattern at all.
    /// </summary>
    public abstract class Consumer : IDisposable
    {
        protected ISpace Space { get; }

        private CancellationTokenSource Cancelation { get; set; }

        private bool _running;
        public bool Running {
            get { return _running; }
            set {
                _running = value;
                if (_running)
                {
                    Cancelation?.Cancel();
                    Cancelation = new CancellationTokenSource();
                    Task.Run(GetHostAction(), Cancelation.Token);
                }
                else
                {
                    Dispose();
                }
            }
        }

        protected abstract Action GetHostAction();

        public Consumer(ISpace space)
        {
            Space = space;
        }

        public virtual void Dispose()
        {
            Cancelation?.Cancel();
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
