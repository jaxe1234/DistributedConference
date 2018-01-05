using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedConference
{
    public class RepoUtility
    {
        public static string GenerateUniqueSpaceUri(string uri, string name)
        {
            return uri + "/" + name.GetHashCode();
        }
    }
}
