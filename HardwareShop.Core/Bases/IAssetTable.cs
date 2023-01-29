using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Bases
{
    public static class ContentTypeConstants
    {
        public static string JpegContentType = "image/jpeg";
        public static string PngContentType = "image/png";
    }
    public interface IAssetTable : ITrackingDate
    {
        byte[] Bytes { get; set; }
        string Filename { get; set; }
        string AssetType { get; set; }
        string ContentType { get; set; }
    }
}
