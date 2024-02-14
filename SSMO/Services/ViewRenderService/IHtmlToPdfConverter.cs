using DevExpress.Utils;
using ImageMagick;
using System;
using System.Diagnostics;
using System.IO;

namespace SSMO.Services.ViewRenderService
{
    public interface IHtmlToPdfConverter
    {
        byte[] Convert(string basePath, string htmlCode, FormatType formatType, OrientationType orientationType);
    }
}
