using DevExpress.Utils;
using ImageMagick;
using System;
using System.Diagnostics;
using System.IO;

namespace SSMO.Services.ViewRenderService
{
    public class HtmlToPdfConverter : IHtmlToPdfConverter
    {
        public byte[] Convert(string basePath, string htmlCode, FormatType formatType = FormatType.Numeric, OrientationType orientationType = OrientationType.TopLeft)
        {
            var inputFileName = $"input_{Guid.NewGuid()}.html";
            var outputFileName = $"output_{Guid.NewGuid()}.pdf";
            File.WriteAllText($"{basePath}/{inputFileName}", htmlCode);
            var startInfo = new ProcessStartInfo("phantomjs.exe")
            {
                WorkingDirectory = basePath,
                Arguments = $"rasterize.js \"{inputFileName}\" \"{outputFileName}\" \"{formatType}\" \"{orientationType.ToString().ToLower()}\"",
                UseShellExecute = true,
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();

            process.WaitForExit();

            var bytes = File.ReadAllBytes($"{basePath}/{outputFileName}");

            File.Delete($"{basePath}/{inputFileName}");
            File.Delete($"{basePath}/{outputFileName}");

            return bytes;
        }        
    }
}
