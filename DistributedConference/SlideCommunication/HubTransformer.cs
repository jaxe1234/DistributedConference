using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using System.Security.Cryptography;
using dotSpace.Objects.Space;

namespace SlideCommunication
{
    public class HubTransformer : Consumer
    {
        public ISpace ConcealedSpace { get; }
        protected ISpace PrivateSpace { get; }
        private Publisher Proxy { get; set; }
        public HubTransformer(ISpace space, ISpace concealedSpace) : base(space)
        {
            ConcealedSpace = concealedSpace;
            PrivateSpace = new SequentialSpace();
        }

        protected override Action GetHostAction()
        {
            return Listen;
        }

        private void Listen()
        {
            if (Proxy == null)
            {
                Proxy = new Publisher(PrivateSpace, Space, ConcealedSpace);
            }
            Proxy.Running = true;
            while (true)
            {
                var request = Space.Get("Request", typeof(HubRequestType), typeof(string), typeof(string));
                var identifier = request.Get<string>(2);
                var requestType = request.Get<HubRequestType>(1);
                var secret = request.Get<string>(3);

                if (requestType == HubRequestType.EstablishSession)
                {
                    var t = PrivateSpace.QueryP("SessionSecret", typeof(string), identifier);
                    if (t != null)
                    {
                        PrivateSpace.GetAll("SessionSecret", typeof(string), identifier);
                    }
                    using (var me = new ECDiffieHellmanCng())
                    {
                        var herKey = CngKey.Import(Convert.FromBase64String(secret), CngKeyBlobFormat.EccPublicBlob);
                        var ourKey = Convert.ToBase64String(me.DeriveKeyMaterial(herKey));
                        PrivateSpace.Put("SessionSecret", ourKey, identifier);
                        var myKey = Convert.ToBase64String(me.PublicKey.ToByteArray());
                        Space.Put("Response", identifier, HubRequestType.EstablishSession, myKey);
                    }
                    Proxy.SyncIncomingUser(identifier);
                } else if (requestType == HubRequestType.TerminateSession)
                {
                    PrivateSpace.GetP("SessionSecret", typeof(string), identifier);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Proxy?.Dispose();
        }
    }

    public enum HubRequestType
    {
        Unspecified,
        EstablishSession,
        TerminateSession,
    }
}
