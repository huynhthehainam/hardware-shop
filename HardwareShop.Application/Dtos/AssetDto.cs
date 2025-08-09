
namespace HardwareShop.Application.Dtos
{
    public class AssetDto
    {
        public byte[] Bytes { get; set; } = new byte[0];
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}