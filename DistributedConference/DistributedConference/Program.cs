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
using Newtonsoft.Json;
using System.Threading;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
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

            //var hostentry = Dns.GetHostEntry("").AddressList
            //    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            //string uri = "tcp://" + hostentry + ":5002";
            //ChatTest(args, uri);
            new Thread(() => TestSlideServer()).Start();
            new Thread(() => TestSlideClient()).Start();

            //var hostentry = Dns.GetHostEntry("").AddressList
            //    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            //string uri = "tcp://" + hostentry + ":5002";
            //ChatTest(args, uri);
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

        private static IEnumerable<byte[]> testPdfService()
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

        private static void testJson()
        {
            var setting = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            var o = new TemplateField { Type = typeof(Student), Value = new Student { Name = "Asger Moeberg", StudentId = "s164407", Courses = new List<int>() { 2148 } } };
            var s = JsonConvert.SerializeObject(o, setting);
            var oa = JsonConvert.DeserializeObject<TemplateField>(s, setting);

        }

        private static void TestSlideServer()
        {
            using (var repo = new SpaceRepository())
            {
                repo.AddGate("tcp://127.0.0.1:15432");
                ISpace space = new SequentialSpace();
                repo.AddSpace("space", space);
                space.Put("L", "O", "L", 108);
                var t = space.Get("Y", "O", "L", "O", typeof(int));
                var server = new Consumer(space, testPdfService().ToArray());
            }
        }

        private static void TestSlideClient()
        {
            Thread.Sleep(100);
            ISpace space = new RemoteSpace("tcp://127.0.0.1:15432/space");
            var t = space.Get("L", "O", "L", typeof(int));
            space.Put("Y", "O", "L", "O", 810);
            var client = new Producer(space, "aMoe");
            var frames = client.GetFrames(1, 2, 3, 4).ToList();
            var i = 0;
            foreach (var bs in frames)
            {
                File.WriteAllBytes($"image{i++}.png", bs);
            }
        }

        public static void DiningPhil(string[] args)
        {
            const string addr = "tcp://127.0.0.1{0}?KEEP";
            //var addr = args.FirstOrDefault(s => s.StartsWith("--server="))?.Replace("--server=", "");
            //int n;
            //if (string.IsNullOrEmpty(addr) || !int.TryParse(args.FirstOrDefault(s => s.StartsWith("--number="))?.Replace("--number=", ""), out n))
            //{
            //    return;
            //}
            //if (args.Any(s => s == "-c")) {
            //    Philosopher(addr);
            //} else
            //{
            //    Waiter(addr, n);
            //}
            Task.Run(() => Philosopher(string.Format(addr, "/table")));
            //Task.Run(() => Philosopher(addr + "/table"));
            Waiter(string.Format(addr, ""), 1);
        }

        static void Waiter(string conn, int n)
        {
            var repo = new SpaceRepository();
            repo.AddGate(conn);
            ISpace space = new SequentialSpace();
            repo.AddSpace("table", space);

            space.Put("list", new List<int> { 1, 3, 3, 7 });
            space.Put("student", new Student { StudentId = "s164407", Name = "Asger Moeberg", Courses = new List<int> { 2148 } });
            space.Put("student", new Student { StudentId = "s194407", Name = "Jo Moeberg", Courses = new List<int> { 2148 } });
            //var tt1 = space.Query("list", typeof(List<int>));

            space.Put("total", n);
            foreach (var i in Enumerable.Range(0, n))
            {
                space.Put("fork", i);
                space.Put("seat", i);
                if (i > 0 || i == (n - 1))
                {
                    space.Put("ticket");
                }
            }

            //space.Put("debug");

            //Task.Run(() => {
            //    var timer = new System.Threading.Timer((e) =>
            //    {
            //        var fs = space.QueryAll("fork", typeof(int)).Select(t => t[1]).ToArray();
            //        var c = space.QueryAll("ticket")?.Count() ?? 0;

            //        Console.WriteLine("Number of tickets: {0}. Forks {1}", c, new dotSpace.Objects.Space.Tuple(fs));

            //    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            //});

            //space.Put("ticket");
            space.Get("done");
        }

        static void Philosopher(string conn)
        {
            Thread.Sleep(100);
            ISpace space = new RemoteSpace(conn);

            //var t1 = space.Query("list", typeof(List<int>));
            //var l = t1.Get<List<int>>(1);
            space.Put("student", new Student { StudentId = "s214407", Name = "Lau Moeberg", Courses = new List<int> { 2148 } });
            var t2 = space.GetAll("student", typeof(Student)).ToList();

            space.Get("done");
            //var t1 = space.Query("total", typeof(int));

            //var n = (int)space.Query("total", typeof(int))[1];
            //var t = space.Get("seat", typeof(int));
            //var left = (int)t[1];
            //var rigth = (left + 1) % n;

            //var t2 = space.QueryP("arr", typeof(int[]));

            //while (true)
            //{
            //    space.Get("ticket");


            //    space.Get("fork", left);
            //    Console.WriteLine("Phil {0}: Got left fork", left);
            //    space.Get("fork", rigth);
            //    Console.WriteLine("Phil {0}: Got rigth fork", left);

            //    Console.WriteLine("Phil {0}: Eating...", left);


            //    space.Put("fork", left);
            //    space.Put("fork", rigth);

            //    Console.WriteLine("Phil {0}: Put both forks back", left);

            //    space.Put("ticket");

            //    Console.WriteLine("Phil {0}: Put ticket back", left);
            //}
        }
    }
}

