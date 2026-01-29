using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public sealed class Ticket : AuditableEntityBase
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
    }

}