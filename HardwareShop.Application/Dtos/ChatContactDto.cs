namespace HardwareShop.Application.Dtos
{

    public class ContactUserDto
    {
        public Guid UserGuid { get; set; }
        public long AssetId { get; set; }
    }
    public class ChatContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Unread { get; set; }
        public string Status { get; set; } = "offline";
        public long AssetId { get; set; }
        public bool IsGroupChat { get; set; }
        public ContactUserDto[] Users { get; set; } = new ContactUserDto[0];
        public List<Guid> AffectedUserIds { get; set; } = new List<Guid>();
    }
}