using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectUtilities;

namespace NamingToolsTests
{
    [TestClass()]
    public class NameHashingToolTests
    {
        [TestMethod()]
        public void GenerateUniqueRemoteSpaceUriTest()
        {
            var generatedName = NameHashingTool.GenerateUniqueRemoteSpaceUri("127.0.0.1:5002", "Kartoffel");
            Debug.Assert(generatedName ==
                "127.0.0.1:5002/ConferenceSUSTSWVVSX?CONN");
        }

        [TestMethod()]
        public void GenerateUniqueSequentialSpaceNameTest()
        {
            Debug.Assert(NameHashingTool.GenerateUniqueSequentialSpaceName("Kartoffel") ==
                         "ConferenceSUSTSWVVSX");
        }

        

        [TestMethod()]
        public void CreateSequentialSpaceWithSequentialSpaceNameGeneratorTest()
        {
            using (var spaceRepo = new SpaceRepository())
            {
                string uri = "tcp://127.0.0.1:5005";
                string testName = NameHashingTool.GenerateUniqueSequentialSpaceName("ThisNameDoesNotActuallyMatter");
                ISpace testSpace = new SequentialSpace();
                spaceRepo.AddSpace(testName, testSpace);
                spaceRepo.AddGate(uri + "?CONN");
                var testElement = "This string is a test";
                testSpace.Put(testElement);
                testSpace.Get(testElement);
                Debug.Assert(!testSpace.GetAll().Any()); 
                // putting and getting the element should leave us with an empty space
            }
            
        }

        [TestMethod()]
        public void CreateRemoteSpaceWithRemoteSpaceNameGeneratorTest()
        {
            using (var spaceRepo = new SpaceRepository())
            {
                string uri = "tcp://127.0.0.1:5002";
                string testRemoteName = "ThisNameDoesNotActuallyMatter";
                string testSeqName = NameHashingTool.GenerateUniqueSequentialSpaceName(testRemoteName);
                Console.WriteLine(testSeqName);
                var testSeqSpace = new SequentialSpace();
                spaceRepo.AddSpace(testSeqName, testSeqSpace);
                spaceRepo.AddGate(uri);

                var remoteHash = NameHashingTool.GenerateUniqueRemoteSpaceUri(uri, testRemoteName);
                Console.WriteLine(remoteHash);
                var testRemoteSpace = new RemoteSpace(remoteHash);
            }
            

        }
    }
}