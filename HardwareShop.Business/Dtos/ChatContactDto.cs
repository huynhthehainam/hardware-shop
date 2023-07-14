namespace HardwareShop.Business.Dtos
{


    public sealed class ChatContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Unread { get; set; }
        public string Status { get; set; } = "offline";
        public long AssetId { get; set; }
        public bool IsGroupChat { get; set; }
    }
}