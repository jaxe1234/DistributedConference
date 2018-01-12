using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using System.Security.Cryptography;

namespace SlideCommunication
{
    public class HubConsumer : Consumer
    {
        public ISpace ConcealedSpace { get; }
        private ProxyConsumer Proxy { get; set; }
        public HubConsumer(ISpace space, ISpace concealedSpace) : base(space)
        {
            ConcealedSpace = concealedSpace;
        }

        protected override Action GetHostAction(HostingMode mode)
        {
            switch (mode)
            {
                case HostingMode.Hub:
                    return Listen;
                default:
                    return (() => { });
            }
        }

        private void Listen()
        {
            if (Proxy == null)
            {
                Proxy = new ProxyConsumer(PrivateSpace, Space, ConcealedSpace);
            }
            Proxy.HostingMode = HostingMode.Slave;
            while (true)
            {
                var request = Space.Get("Request", typeof(HubRequestType), typeof(string), typeof(string));
                var identifier = request.Get<string>(2);
                var requestType = request.Get<HubRequestType>(1);
                var secret = request.Get<string>(3);

                if (requestType == HubRequestType.EstablishSession)
                {
                    var t = PrivateSpace.QueryP("SessionSecret", typeof(string), identifier);
                    if (t == null)
                    {
                        using (var me = new ECDiffieHellmanCng())
                        {
                            var herKey = CngKey.Import(Convert.FromBase64String(secret), CngKeyBlobFormat.EccPublicBlob);
                            var ourKey = Convert.ToBase64String(me.DeriveKeyMaterial(herKey));
                            PrivateSpace.Put("SessionSecret", ourKey, identifier);
                            var myKey = Convert.ToBase64String(me.PublicKey.ToByteArray());
                            Space.Put("Response", identifier, HubRequestType.EstablishSession, myKey);
                        }
                    } 
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
