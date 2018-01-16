using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class ClientSession
    {

        public ISpace Space { get; }
        public ISpace LocalSpace { get; }
        public string Username { get; }

        private string _sessionSercret;
        private string SessionSecret
        {
            get
            {
                if (_sessionSercret == null)
                {
                    return (_sessionSercret = ConnectToSession()); 
                }
                return _sessionSercret;
            }
        }

        public ClientSession(ISpace space, string username)
        {
            Space = space;
            LocalSpace = new SequentialSpace();
            Username = username;
        }

        public RequestToken CreateToken()
        {
            return new RequestToken(SessionSecret);
        }

        public RequestToken CreateToken(string token)
        {
            return new RequestToken(token, SessionSecret);
        }

        private string ConnectToSession()
        {
            using (var me = new ECDiffieHellmanCng())
            {
                Space.Put("Request", HubRequestType.EstablishSession, Username, Convert.ToBase64String(me.PublicKey.ToByteArray()));
                var tuple = Space.Get("Response", Username, typeof(HubRequestType), typeof(string));
                var herKey = CngKey.Import(Convert.FromBase64String(tuple.Get<string>(3)), CngKeyBlobFormat.EccPublicBlob);
                var ourKey = Convert.ToBase64String(me.DeriveKeyMaterial(herKey));
                return ourKey;
            }
        }
    }
}
