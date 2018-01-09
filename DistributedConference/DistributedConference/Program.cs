using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PdfHandler;
using System.Drawing;
using ChatApp;
using ConferenceLobbyUI;
using dotSpace.Objects.Network;
using System.IO;
using System.Net.Sockets;

namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {
            //testPdfService();

            var hostentry = Dns.GetHostEntry("").AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            string uri = "tcp://" + hostentry + ":5002";
            
            ChatTest(args, uri);
            //Console.WriteLine("Program has terminated");
            
            
        }

        private static void ChatTest(string[] args, string uri)
        {
            using (var spaceRepo = new SpaceRepository())
            {
                new Chat(args[0].Equals("host"), args[0], spaceRepo, args[0].Equals("host") ? uri : args[2], args[1]).InitializeChat();
                Console.WriteLine("Chat is done.");
                spaceRepo.Dispose();
            }
        }

        private static void testPdfService()
        {
            var url = "https://meltdownattack.com/meltdown.pdf";
            var client = new WebClient();
            IList<Image> images = new List<Image>();
            using (var stream = client.OpenRead(url))
            {
                PdfRasterizerService.GetImages(stream, 96, ref images);
            }
            IList<byte[]> bitstreams = new List<byte[]>();
            PdfRasterizerService.ConvertToPngBitstream(images, ref bitstreams);
            var i = 1;
            foreach (var stream in bitstreams)
            {
                File.WriteAllBytes($"image{i++}.png", stream);
            }
        }
    }
}

