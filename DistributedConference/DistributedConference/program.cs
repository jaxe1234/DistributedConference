using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PdfHandler;
using System.Drawing;
using ChatApp;
using dotSpace.Objects.Network;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using ProjectUtilities;
using SlideCommunication;

//using SlideCommunication;

namespace DistributedConference
{
    class Program
    {
        static void Main(string[] args)
        {
            //testPdfService();

            //DiningPhil(args);

            //testJson();

            new Thread(TestSlideServer).Start();
            new Thread(TestSlideClient).Start();

            //Console.WriteLine("Program has terminated");


        }

        private static void ChatTest(string[] args, string uri)
        {
            var name = args[0];
            var conferenceName = args[1];
            var dataSource = new ObservableCollection<string>();

            if (name.Equals("host"))
            {
                using (var spaceRepo = new SpaceRepository())
                {
                    new Chat(name, uri, conferenceName, spaceRepo, dataSource).InitializeChat();
                    Console.WriteLine("Chat is done.");
                    spaceRepo.Dispose();
                }
            }
            else
            {
                new Chat(name, uri, conferenceName, dataSource).InitializeChat();
            }

        }

        private static IEnumerable<byte[]> TestPdfService()
        {
            var url = "https://meltdownattack.com/spectre.pdf";
            var client = new WebClient();
            IList<Image> images = new List<Image>();
            using (var stream = client.OpenRead(url))
            {
                PdfRasterizerService.GetImages(stream, 96, ref images);
            }
            IList<byte[]> bitstreams = new List<byte[]>();
            PdfRasterizerService.ConvertToPngBitstream(images, ref bitstreams);
            return bitstreams;
        }

        class TemplateField
        {
            public Type Type { get; set; }
            public object Value { get; set; }
        }

        class Student
        {
            public string Name { get; set; }
            public string StudentId { get; set; }
            public ICollection<int> Courses { get; set; }
        }

        private static void TestJson()
        {
            var setting = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            var o = new TemplateField { Type = typeof(Student), Value = new Student { Name = "Asger Moeberg", StudentId = "s164407", Courses = new List<int>() { 2148 } } };
            var s = JsonConvert.SerializeObject(o, setting);
            var oa = JsonConvert.DeserializeObject<TemplateField>(s, setting);

        }

        private class SimpleSlideShow : ISlideShow
        {
            private string Name { get; }

            public ControlProducer Producer { private get; set; }

            public bool InControl
            {
                get => throw new NotImplementedException();

                set => throw new NotImplementedException();
            }

            public bool IsHost
            {
                get => throw new NotImplementedException();

                set => throw new NotImplementedException();
            }

            public SimpleSlideShow(string name)
            {
                Name = name;
            }

            public void Draw(System.Windows.Shapes.Shape figure, Point position)
            {
                throw new NotImplementedException();
            }

            public void UpdateSlide(FramePayload payload)
            {
                Console.WriteLine($"{Name}: Switched slide to page {payload.PageNumber}");
            }

            public void GrantControl()
            {
                Console.WriteLine("{0}: Granted control", Name);
                Producer.PageNumber = 1;
            }

            public void GrantHostStatus()
            {
                throw new NotImplementedException();
            }

            public void RevokeControl()
            {
                Console.WriteLine("{0}: Revoked control", Name);
            }

            public void RevokeHostStatus()
            {
                throw new NotImplementedException();
            }

            public void NewCollection(int pages)
            {
                throw new NotImplementedException();
            }
        }

        private static void TestSlideServer()
        {
            using (var repo = new SpaceRepository())
            {
                repo.AddGate("tcp://127.0.0.1:15432?CONN");
                var slide = new SimpleSlideShow("host");
                var server = new SlideHostFacade(repo, "Host", slide);
                server.ConcealedSpacePassword = "AsgerAsger";
                var url = "https://meltdownattack.com/spectre.pdf";
                var client = new WebClient();
                using (var mstream = new MemoryStream())
                {
                    using (var stream = client.OpenRead(url))
                    {
                        stream?.CopyTo(mstream);
                    }
                    server.PrepareToHost(mstream);
                }
                var control = server.Control;
                slide.Producer = control;
                control.Controlling = true;
                //control.ChangeSlider(2);
                Task.Delay(-1).Wait();
            }
        }

        private static void TestSlideClient()
        {
            Thread.Sleep(500);
            const string uri = "tcp://127.0.0.1:15432/?CONN";
            ISlideShow slide = new SimpleSlideShow("client");

            var client1 = new SlideClientFacade(slide, uri, "aMoe") {Running = true};
            //client1.UpgradePrivileges("AsgerAsger");
            //var control = client1.Control;
            //control.ChangeSlider(1);
            Task.Delay(-1).Wait();
        }
    }
}

