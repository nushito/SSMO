using System.Diagnostics;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SSMO.Services.PDF
{
    public class HtmlToPdfConverter : IHtmlToPdfConverter
    {
        public byte[] Convert(string htmlCode)
        {
            var inputFileName = "input.html";
            var outputFileName = "output.pdf";
            File.WriteAllText(inputFileName, htmlCode);
            var startInfo = new ProcessStartInfo("phantomjs.exe")
            {
                WorkingDirectory = Environment.CurrentDirectory,
                Arguments = string.Format(
                                        "rasterize.js \"{0}\" {1} \"A4\"",
                                        inputFileName,
                                        outputFileName),
                UseShellExecute = true,
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();

            process.WaitForExit();

            var bytes = File.ReadAllBytes(outputFileName);

            File.Delete(inputFileName);
            File.Delete(outputFileName);

            return bytes;
        }

        public object Convert(Task<string> stringForPrint)
        {
            throw new NotImplementedException();
        }
    }
}
