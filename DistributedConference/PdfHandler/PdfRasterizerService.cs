using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PdfHandler
{
    public class PdfRasterizerService
    {
        public static void GetImages(Stream stream, int dpi, ref IList<Image> images)
        {
            var gvi = new GhostscriptVersionInfo("lib/gsdll32.dll");

            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(stream, gvi, true);

                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    var img = rasterizer.GetPage(dpi, dpi, pageNumber);
                    images.Add(img);
                }
            }
        }

        public static void ConvertToPngBitstream(IEnumerable<Image> images, ref IList<byte[]> bitstreams)
        {
            foreach (var img in images)
            {
                using (var stream = new MemoryStream())
                {
                    img.Save(stream, ImageFormat.Png);
                    bitstreams.Add(stream.ToArray());
                }
            }
        }
    }
}
