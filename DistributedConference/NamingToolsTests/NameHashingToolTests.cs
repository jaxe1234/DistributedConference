using System.Diagnostics;
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
        
    }
}