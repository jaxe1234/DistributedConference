using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.BaseClasses;
using dotSpace.Enumerations;
using dotSpace.Interfaces;
using dotSpace.Interfaces.Space;
using dotSpace.Objects;
using dotSpace.Objects.Network;
using DistributedConference;

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
            Space = new RemoteSpace(RepoUtility.GenerateUniqueSpaceUri(uri, name));
            repo.AddSpace(ConferenceName + "ImageStream", Space);

        }
        


    }
}
