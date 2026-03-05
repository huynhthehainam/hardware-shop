using HardwareShop.Application.CQRS.CustomerArea.Queries;
using HardwareShop.Application.CQRS.OrderArea.Commands;
using HardwareShop.Application.CQRS.OrderArea.Queries;
using HardwareShop.Application.CQRS.ProductArea.Queries;
using HardwareShop.Application.CQRS.ShopArea.Commands;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Attributes;
using HardwareShop.WebApi.Extensions;
using HardwareShop.WebApi.Services;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiControllerBase
    {

        private readonly IMediator mediator;
        public ShopsController(IMediator mediator, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.mediator = mediator;
        }
        [HttpGet]
        [AdminRoleAuthorize]
        public async Task<IActionResult> GetShops([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost]
        [AdminRoleAuthorize]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {
            var response = await mediator.Send(command);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateLogo")]
        public async Task<IActionResult> UpdateLogo([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost("YourShop/UpdateLogo")]
        public async Task<IActionResult> UpdateYourShopLogo()
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateSetting")]
        public async Task<IActionResult> UpdateShopSettings([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Logo")]
        public async Task<IActionResult> GetYourShopLogo()
        {
            return responseResultBuilder.Build();
        }


        [HttpGet("YourShop/Users")]
        public async Task<IActionResult> GetUsersOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Customers")]
        public async Task<IActionResult> GetCustomersOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var response = await mediator.Send(new GetYourShopCustomersQuery
            {
                PagingModel = pagingModel,
                Search = search
            });
            responseResultBuilder.SetApplicationResponse(response, (builder, pageData) =>
            {
                builder.SetPageData(pageData);
            });
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Products")]
        public async Task<IActionResult> GetProductsOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var response = await mediator.Send(new GetYourShopProductsQuery
            {
                PagingModel = pagingModel,
                Search = search
            });
            responseResultBuilder.SetApplicationResponse(response, (builder, pageData) =>
            {
                builder.SetPageData(pageData);
            });
            return responseResultBuilder.Build();
        }


        [HttpPost("{id:int}/DeleteSoftly")]
        public async Task<IActionResult> DeleteShopSoftly([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/CreateAdminUser")]
        public async Task<IActionResult> CreateAdminUser([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost("YourShop/Orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var response = await mediator.Send(command);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Orders/{id:guid}")]
        public async Task<IActionResult> GetDetailedOrderById([FromRoute] Guid id)
        {
            var response = await mediator.Send(new GetDetailedOrderByIdQuery
            {
                Id = id
            });
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }

        [HttpPost("YourShop/Orders/{id:guid}/Print")]
        public async Task<IActionResult> PrintOrder([FromRoute] Guid id)
        {
            var response = await mediator.Send(new GetDetailedOrderByIdQuery
            {
                Id = id
            });
            responseResultBuilder.SetApplicationResponse(response, (builder, detailedOrder) =>
            {
                var fileBytes = GenerateOrderPdf(detailedOrder);
                builder.SetFile(fileBytes, "application/pdf", $"order-{detailedOrder.Id}.pdf");
            });
            return responseResultBuilder.Build();
        }

        private static byte[] GenerateOrderPdf(DetailedOrderDto order)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, PageSize.A4);

            document.Add(new Paragraph(order.ShopName ?? "Hardware Shop").SetFontSize(18));
            document.Add(new Paragraph($"Order: {order.Id}").SetFontSize(11));
            document.Add(new Paragraph($"Date: {order.CreatedDate:yyyy-MM-dd HH:mm:ss}").SetFontSize(11));
            document.Add(new Paragraph($"Customer: {order.CustomerName ?? "N/A"} - {order.CustomerPhone ?? "N/A"}").SetFontSize(11));
            document.Add(new Paragraph("\n"));

            var table = new Table(new float[] { 4, 2, 2, 2, 3 }).UseAllAvailableWidth();
            table.AddHeaderCell("Product");
            table.AddHeaderCell("Qty");
            table.AddHeaderCell("Unit");
            table.AddHeaderCell("Unit Price");
            table.AddHeaderCell("Amount");

            foreach (var item in order.Details)
            {
                table.AddCell(item.ProductName ?? string.Empty);
                table.AddCell(item.Quantity.ToString("N2"));
                table.AddCell(item.UnitName ?? string.Empty);
                table.AddCell(item.UnitPrice.ToString("N2"));
                table.AddCell(item.TotalAmount.ToString("N2"));
            }

            document.Add(table);
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph($"Total Amount: {order.TotalAmount:N2}"));
            document.Add(new Paragraph($"Paid Amount: {order.PaidAmount:N2}"));
            document.Add(new Paragraph($"Debt After Order: {order.DebtAfterOrder:N2}"));

            document.Close();
            return stream.ToArray();
        }

    }
}
