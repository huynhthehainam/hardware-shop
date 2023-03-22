namespace HardwareShop.Core.Bases
{
    public static class ContentTypeConstants
    {
        public const string JpegContentType = "image/jpeg";
        public const string PngContentType = "image/png";
    }
    public interface IAssetTable : ITrackingDate
    {
        byte[] Bytes { get; set; }
        string Filename { get; set; }
        string AssetType { get; set; }
        string ContentType { get; set; }
    }

    public static class AssetTableExtensions
    {
        public static string ConvertToImgSrc(this IAssetTable asset)
        {
            return $"data:{asset.ContentType};base64,{Convert.ToBase64String(asset.Bytes)}";
        }
    }
}
