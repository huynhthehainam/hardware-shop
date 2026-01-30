using System.ComponentModel.DataAnnotations;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.ShopArea.Commands
{
    public class CreateShopCommand : IRequest<ApplicationResponse<CreatedShopDto>>
    {
        public required string Name { get; set; }
        public required int CashUnitId { get; set; }
        public required string Address { get; set; }
    }

}
