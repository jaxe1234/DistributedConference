﻿using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
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

        private ISpace _exposedSpace;
        private string _controlSpaceTarget;
        private ISpace _concealedSpace;

        private HubConsumer _hub;
        private FrameConsumer _frame;
        //private Producer _producer;

        private HubConsumer Hub => _hub ?? (_hub = new HubConsumer(_exposedSpace, _concealedSpace));
        private FrameConsumer Frame => _frame ?? (_frame = new FrameConsumer(_exposedSpace, _concealedSpace));
        //private Producer Producer => _producer ?? (_producer = new Producer(this, _exposedSpace, _username));

        public SlideHostFacade(SpaceRepository repo)
        {
            _repo = repo;
            _exposedSpace = new SequentialSpace();
            _concealedSpace = new SequentialSpace();
            _repo.AddSpace("hub", _exposedSpace);
            _repo.AddSpace("control", _concealedSpace);
            Hub.HostingMode = HostingMode.Hub;
        }


        public void PrepareToHost(Stream stream)
        {
            IList<Image> bitmaps = new List<Image>();
            PdfRasterizerService.GetImages(stream, 96, ref bitmaps);
            IList<byte[]> bitstream = new List<byte[]>();
            PdfRasterizerService.ConvertToPngBitstream(bitmaps, ref bitstream);
            Frame.Bitstreams = bitstream;
            Frame.HostingMode = HostingMode.Slave;
        }

        public void Dispose()
        {
            _frame?.Dispose();
            _hub?.Dispose();
            _repo?.Dispose();        
        }

        //public void Draw(Shape figure, Point position)
        //{
        //    SlideShower.Draw(figure, position);
        //}


        //public void GotoSlide(int page)
        //{
        //    SlideShower.GotoSlide(page);
        //}

        //public void GrantControl()
        //{
        //    SlideShower.GrantControl();
        //}

        //public void GrantHostStatus()
        //{
        //    if (!_canHost)
        //    {
        //        Frame.HostingMode = HostingMode.Idle;
        //    }
        //    SlideShower.GrantHostStatus();
        //}

        //public void RevokeHostStatus()
        //{
        //    SlideShower.RevokeHostStatus();
        //}

        //public void RevokeControl()
        //{
        //    SlideShower.RevokeControl();
        //}
    }
}
