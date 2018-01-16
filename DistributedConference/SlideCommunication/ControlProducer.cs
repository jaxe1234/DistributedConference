﻿using dotSpace.Interfaces.Space;
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
        private string HostIdentifer { get; set; }
        private bool _controlling;
        private CancellationTokenSource _token;
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
            var tuple = ConcealedSpace.Get("ControlLock", typeof(string));
            HostIdentifer = tuple.Get<string>(1);
            _controlling = true;
            SlideShower.GrantControl();
        }

        private void ReleaseControl()
        {
            if (_controlling)
            {
                ConcealedSpace.Put("ControlLock", HostIdentifer);
            }
            _controlling = false;
            SlideShower.RevokeControl();
        }

        public void ChangeSlider(int page)
        { 
            if (_controlling)
            {
                ConcealedSpace?.Put("SlideChange", HostIdentifer, page);
            }
        }
    }
}