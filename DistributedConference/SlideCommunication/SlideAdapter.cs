using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using NamingTools;
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
        private string _concealedSpaceTarget;
        private ISpace _concealedSpace;

        private HubConsumer _hub;
        private FrameConsumer _frame;
        //private Producer _producer;

        private HubConsumer Hub => _hub ?? (_hub = new HubConsumer(_exposedSpace, _concealedSpace));
        private FrameConsumer Frame => _frame ?? (_frame = new FrameConsumer(_exposedSpace, _concealedSpace));
        //private Producer Producer => _producer ?? (_producer = new Producer(this, _exposedSpace, _username));

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

        public SlideHostFacade(SpaceRepository repo)
        {
            _repo = repo;
            _exposedSpace = new SequentialSpace();
            _concealedSpace = new SequentialSpace();
            _repo.AddSpace("hub", _exposedSpace);
            Hub.Running = true;
        }

        public void PrepareToHost(Stream stream)
        {
            IList<Image> bitmaps = new List<Image>();
            PdfRasterizerService.GetImages(stream, 96, ref bitmaps);
            IList<byte[]> bitstream = new List<byte[]>();
            PdfRasterizerService.ConvertToPngBitstream(bitmaps, ref bitstream);
            Frame.Bitstreams = bitstream;
            Frame.Running = true;
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
