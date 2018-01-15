using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using NamingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class SlideClientFacade : IDisposable
    {
        private ClientSession Session { get; }

        private ISpace ConcealedSpace { get; set; }
        private ISpace Space { get; }
        private string UriFormat { get; }
        private string Username { get; }

        public ISlideShow SlideShower { get; private set; }

        private bool _running;
        public bool Running { get { return _running; }
            set
            {
                _running = value;
                if (FrameTransformer != null) {
                    FrameTransformer.Running = _running;
                }
                _frameConsumer.Running = _running;
                _controlConsumer.Running = _running;
            }

        }

        private FrameTransformer _frameTransformer;
        private FrameConsumer _frameConsumer;
        private ControlConsumer _controlConsumer;
        private ControlProducer _controlProducer;
        public ControlProducer ControlProducer {
            get {
                return IsPrivileged ? _controlProducer : null;
            }
        }
        public FrameTransformer FrameTransformer
        {
            get
            {
                return IsPrivileged ? _frameTransformer : null;
            }
        }

        public bool IsPrivileged => ConcealedSpace != null && _controlProducer != null;

        public SlideClientFacade(ISlideShow slideShow, string conn, string username)
        {
            var regex = new Regex(@"\/(\?.*)?$");
            UriFormat = regex.Replace(conn, "/{0}$1");
            Username = username;
            SlideShower = slideShow;

            Space = new RemoteSpace(string.Format(UriFormat, "hub"));
            Session = new ClientSession(Space, Username);
            _frameConsumer = new FrameConsumer(Session);
            _controlConsumer = new ControlConsumer(Session, SlideShower);
        }

        public void UpgradePrivileges(string passwd)
        {
            var tuple = Space.QueryP("ConcealedIdentifier", typeof(string));
            var token = tuple?.Get<string>(1);
            var hash = NameHashingTool.GetSHA256String(passwd);
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(passwd) && token == hash)
            {
                var id = NameHashingTool.GetSHA256String(passwd + token);
                ConcealedSpace = new RemoteSpace(string.Format(UriFormat, id));
                _controlProducer = new ControlProducer(ConcealedSpace, SlideShower, Username);
            }
        }

        public void Dispose()
        {
            _controlConsumer.Dispose();
            _frameConsumer.Dispose();
            _frameTransformer.Dispose();
        }
    }
}
