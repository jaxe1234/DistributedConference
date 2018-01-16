using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using NamingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class ControlProducer
    {
        private ISpace ConcealedSpace { get; set; }
        private ISlideShow SlideShower { get; }
        private string Identifier { get; }
        private string CollectionIdentifier { get; set; }
        private bool _controlling;
        private CancellationTokenSource _token;

        public int NumberOfPages { get; private set; }

        private int _pageNumber;
        public int PageNumber { get
            {
                return _pageNumber;
            }
            set {
                if (!_controlling || value <= 0 || value > NumberOfPages)
                {
                    return;
                }
                _pageNumber = value;
                ConcealedSpace?.Put("SlideChange", CollectionIdentifier, _pageNumber);
                var tuple = ConcealedSpace.QueryP("Frame", _pageNumber, typeof(FramePayload));
                SlideShower.UpdateSlide(tuple.Get<FramePayload>(2));
            }
        }

        public bool Controlling {
            get
            {
                return _controlling;
            }
            set
            {
                if (value != _controlling)
                {
                    if (value)
                    {
                        _token?.Cancel();
                        _token = new CancellationTokenSource();
                        Task.Run((Action)AssumeControl, _token.Token);
                    } else
                    {
                        _token?.Cancel();
                        ReleaseControl();
                    }
                }
            }
        }
        public ControlProducer(ISpace concealedSpace, ISlideShow slideShower, string identifier)
        {
            SlideShower = slideShower;
            ConcealedSpace = concealedSpace;
            Identifier = identifier;
        }

        private void AssumeControl()
        {
            var tuple = ConcealedSpace.Get("ControlLock", typeof(string), typeof(int));
            CollectionIdentifier = tuple.Get<string>(1);
            NumberOfPages = tuple.Get<int>(2);
            _controlling = true;
            SlideShower.InControl = true;
            SlideShower.NewCollection(NumberOfPages);
        }

        private void ReleaseControl()
        {
            if (_controlling)
            {
                ConcealedSpace.Put("ControlLock", CollectionIdentifier, NumberOfPages);
            }
            _controlling = false;
            SlideShower.InControl = false;
        }
    }
}
