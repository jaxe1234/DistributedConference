﻿using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using ProjectUtilities;
using PdfHandler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SlideCommunication
{
    public class SlideHostFacade : IDisposable
    {
        private SpaceRepository _repo;

        private ISlideShow SlideShower { get; set; }

        private ISpace _exposedSpace;
        private string _concealedSpaceTarget;
        private ISpace _concealedSpace;

        private string _identifier;

        private HubTransformer _hub;
        private FrameTransformer _frame;
        private ControlProducer _controlProducer;

        private HubTransformer Hub => _hub ?? (_hub = new HubTransformer(_exposedSpace, _concealedSpace));
        private FrameTransformer Frame => _frame ?? (_frame = new FrameTransformer(_exposedSpace, _concealedSpace, SlideShower));
        public ControlProducer Control => _controlProducer ?? (_controlProducer = new ControlProducer(_concealedSpace, SlideShower, _identifier));

        private string _concealedSpacePassword;
        public string ConcealedSpacePassword {
            get {
                return _concealedSpacePassword;
            }
            set {
                if (!string.IsNullOrEmpty(_concealedSpaceTarget))
                {
                    _repo.CloseGate(_concealedSpaceTarget);
                }
                _concealedSpacePassword = value;
                var hashPassword = NameHashingTool.GetSHA256String(_concealedSpacePassword);
                _concealedSpaceTarget = NameHashingTool.GetSHA256String(_concealedSpacePassword + hashPassword);
                _exposedSpace.Put("ConcealedIdentifier", hashPassword);
                _repo.AddSpace(_concealedSpaceTarget, _concealedSpace);
            }
        }

        public SlideHostFacade(SpaceRepository repo, string identifier, ISlideShow slideShower)
        {
            _repo = repo;
            SlideShower = slideShower;
            _identifier = identifier;
            _exposedSpace = new SequentialSpace();
            _concealedSpace = new SequentialSpace();
            _repo.AddSpace("hub", _exposedSpace);
            Hub.Running = true;
            SlideShower.IsHost = true;
        }

        public void PrepareToHost(Stream stream)
        {
            IList<byte[]> bitstreams = new List<byte[]>();
            PdfRasterizerService.GetImages(stream, 96, ref bitstreams);
            Frame.Bitstreams = bitstreams;
            Frame.Running = true;
            Control.PageNumber = 1;
        }

        public void Dispose()
        {
            _frame?.Dispose();
            _hub?.Dispose();
            _repo?.Dispose();        
        }
    }
}
