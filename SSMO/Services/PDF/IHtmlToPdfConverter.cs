using DevExpress.Utils;
using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Diagnostics;
using System.IO;


namespace SSMO.Services.PDF
{
    public interface IHtmlToPdfConverter
    {
      //  byte[] Convert(string basePath, string htmlCode, Rectangle formatType, PdfNumber orientationType);
        byte[] Convert(string htmlCode);

    }
}
