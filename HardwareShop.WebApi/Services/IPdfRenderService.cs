using HardwareShop.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;

namespace HardwareShop.WebApi.Services
{
    public interface IPdfRenderService
    {
        Task<byte[]> RenderToPdfAsync(string viewName, object model);
    }
    public class PdfRenderService : IPdfRenderService
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;
        private readonly IServiceProvider serviceProvider;
        public PdfRenderService(IRazorViewEngine razorViewEngine, IServiceProvider serviceProvider, ITempDataProvider tempDataProvider)
        {
            this.razorViewEngine = razorViewEngine;
            this.serviceProvider = serviceProvider;
            this.tempDataProvider = tempDataProvider;
        }
        private async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, "Home/Index", false);
                if (viewResult.View == null)
                {
                    return "";
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new TestViewModel()
                };
                var viewContext = new ViewContext(
                   actionContext,
                   viewResult.View,
                   viewDictionary,
                   new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                   sw,
                   new HtmlHelperOptions()
               );
                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();
            }

        }
        public async Task<byte[]> RenderToPdfAsync(string viewName, object model)
        {
            var htmlStr = await RenderToStringAsync(viewName, model);
            using MemoryStream ms = new();
            ConverterProperties properties = new();
            properties.SetFontProvider(new DefaultFontProvider(true, true, true));
            PdfDocument pdf = new(new PdfWriter(ms));
            Document document = new(pdf, PageSize.A4);
            HtmlConverter.ConvertToPdf(htmlStr, pdf, properties);
            var bytes = ms.ToArray();
            return bytes;
        }
    }
}
