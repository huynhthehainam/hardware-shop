using System.Text.Json.Serialization;

namespace HardwareShop.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserShopRole
{
    Admin,
    Staff,
}