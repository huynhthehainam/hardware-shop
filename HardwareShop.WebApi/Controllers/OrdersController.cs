using HardwareShop.Application.CQRS.OrderArea.Commands;
using HardwareShop.Application.CQRS.OrderArea.Queries;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class OrdersController : AuthorizedApiControllerBase
    {
        private readonly IMediator mediator;
        public OrdersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IMediator mediator) : base(responseResultBuilder, currentUserService)
        {
            this.mediator = mediator;
        }
        
    }
}
