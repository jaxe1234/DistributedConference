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

        private HostingMode _mode;
        public HostingMode HostingMode {
            get { return _mode; }
            set {
                if (value != _mode)
                {
                    _mode = value;
                    if (_mode != HostingMode.Idle)
                    {
                        Cacelation = new CancellationTokenSource();
                        Task.Run(GetHostAction(_mode), Cacelation.Token);
                    }
                    else
                    {
                        Dispose();
                    }
                }
            }
        }

        protected abstract Action GetHostAction(HostingMode mode);

        public Consumer(ISpace space)
        {
            Space = space;
            PrivateSpace = new SequentialSpace();
        }

        public void GotoSlider(int number)
        {
            Space.GetAll("SlideControl", typeof(string), typeof(int));
            var identifiers = PrivateSpace.QueryAll("SessionKey", typeof(string), typeof(string)).Select(t => t.Get<string>(1));
            foreach (var id in identifiers)
            {
                Space.Put("SlideControl", id, number);
            }
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
