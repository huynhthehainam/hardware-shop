


using System.Text.Json;

namespace HardwareShop.Application.Dtos
{
    public class CustomerDebtHistoryDto
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public double OldDebt { get; set; }
        public double ChangeOfDebt { get; set; }
        public double NewDebt { get; set; }
        public string? Reason { get; set; }
        public JsonDocument? ReasonParams { get; set; }
    }
}