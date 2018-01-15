using System.Diagnostics;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NamingTools;

namespace NamingToolsTests
{
    [TestClass()]
    public class NameHashingToolTests
    {
        [TestMethod()]
        public void GenerateUniqueRemoteSpaceUriTest()
        {
            Debug.Assert(NameHashingTool.GenerateUniqueRemoteSpaceUri("127.0.0.1:5002","Kartoffel") ==
                "127.0.0.1:5002/ConferenceSUSTSWVVSX");
        }

        [TestMethod()]
        public void GenerateUniqueSequentialSpaceNameTest()
        {
            Debug.Assert(NameHashingTool.GenerateUniqueSequentialSpaceName("Kartoffel") ==
                         "ConferenceSUSTSWVVSX");
        }

        [TestMethod()]
        public void CreateRemoteSpaceWithRemoteSpaceNameGeneratorTest()
        {
            var spaceRepo = new SpaceRepository();
            string uri = "tcp:\\127.0.0.1:5005";
            string testName = "ThisNameDoesNotActuallyMatter";
            ISpace testSpace = new RemoteSpace(NameHashingTool.GenerateUniqueRemoteSpaceUri(uri, testName));

        }

        [TestMethod()]
        public void CreateSequentialSpaceWithSequentialSpaceNameGeneratorTest()
        {
            var spaceRepo = new SpaceRepository();
            string uri = "tcp:\\127.0.0.1:5005";
            string testName = "ThisNameDoesNotActuallyMatter";
            ISpace testSpace = new RemoteSpace(NameHashingTool.GenerateUniqueSequentialSpaceName(testName));
            spaceRepo.AddSpace(testName, testSpace);
            spaceRepo.AddGate(uri+"?CONN");

        }
    }
}