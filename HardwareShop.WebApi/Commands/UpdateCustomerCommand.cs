namespace HardwareShop.WebApi.Commands
{
    public class UpdateCustomerCommand
    {

        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool? IsFamiliar { get; set; }
        public double? AmountOfCash {get;set;}
    }
}