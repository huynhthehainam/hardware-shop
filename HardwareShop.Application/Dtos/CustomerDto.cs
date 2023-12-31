

namespace HardwareShop.Application.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? PhonePrefix { get; set; }
        public int? PhoneCountryId { get; set; }
        public bool IsFamiliar { get; set; }
        public double Debt { get; set; }
    }
}