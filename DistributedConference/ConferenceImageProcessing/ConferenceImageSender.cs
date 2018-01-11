using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using NamingTools;

namespace ConferenceImageProcessing
{
    class ConferenceImageSender
    {
        private static readonly string uri = "tcp://127.0.0.1:5001";

        private ISpace Space { get; set; }
        public string ConferenceName { get; private set; }

        public ConferenceImageSender(string name, SpaceRepository repo)
        {
            ConferenceName = name.Replace(" ", "");
            Space = new RemoteSpace(NamingTool.GenerateUniqueRemoteSpaceUri(uri, name));
            repo.AddSpace(ConferenceName + "ImageStream", Space);
        }
        


    }
}
