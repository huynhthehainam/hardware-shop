using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public sealed class Ticket : AuditableEntityBase
    {
        public int Id { get; set; }
    }

}