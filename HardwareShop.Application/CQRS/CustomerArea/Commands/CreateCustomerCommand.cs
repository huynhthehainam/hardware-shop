using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Commands;

public class CreateCustomerCommand : IRequest<ApplicationResponse<Guid>>
{
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public Guid? PhoneCountryId { get; set; }
    public string? Address { get; set; }
    public bool IsFamiliar { get; set; }
}