using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfHandler;
using System.Drawing;
using System.Net;

namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "https://meltdownattack.com/meltdown.pdf";
            var client = new WebClient();
            using (var stream = client.OpenRead(url))
            {
                IList<Image> images = new List<Image>();
                PdfRasterizerService.GetImages(stream, ref images);
                images = images;
            }

            //new ChatApp.ChatTest(args);
        }
    }
}
