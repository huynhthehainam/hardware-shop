
using System.Text.Json;
using HardwareShop.Core.Constants;

namespace HardwareShop.Business.Helpers
{
    public static class CustomerDebtHistoryHelper
    {
        public static Tuple<string, JsonDocument> GenerateDebtReasonWhenBuying(string invoiceCode)
        {
            return new Tuple<string, JsonDocument>("DEBT_REASON_WHEN_BUYING", JsonDocument.Parse(JsonSerializer.Serialize(new { InvoiceCode = invoiceCode }, JsonSerializerConstants.CamelOptions)));
        }
    }
}