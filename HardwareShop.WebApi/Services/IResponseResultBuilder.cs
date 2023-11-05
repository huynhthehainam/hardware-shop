
using System.Text.Json.Serialization;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HardwareShop.WebApi.Services
{
    public interface IResponseResultBuilder
    {
        IActionResult Build();
        void SetUpdatedMessage();
        void SetData(object? data);
        void SetDeletedMessage();
        void SetMessage(string message);
        void SetNoContent();
        void SetFile(byte[] bytes, string contentType, string fileName);

        void SetCreatedObject<T>(T entity);
        void AddInvalidFieldError(string fieldName);
        void AddExistedEntityError(string entityName);
        void AddNotFoundEntityError(string entityName);
        void AddNotPermittedError();
        void SetPageData<T>(PageData<T> pageData);
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseResultType
    {
        Json,
        File,
    }
    public class ResponseResultBuilder : IResponseResultBuilder
    {
        private readonly IStringLocalizer<ResponseResultBuilder> stringLocalizer;
        public ResponseResultBuilder(IStringLocalizer<ResponseResultBuilder> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }
        private IDictionary<string, List<string>>? error = null;
        private string? message;
        private ResponseResultType type = ResponseResultType.Json;
        private int statusCode = 200;
        private Object? data = null;
        private int? totalItems = null;
        public void SetMessage(string message)
        {
            this.message = stringLocalizer[message].Value;
        }
        public void SetUpdatedMessage()
        {
            SetMessage("Updated");
        }
        public void SetDeletedMessage()
        {
            SetMessage("Deleted");
        }
        public void SetCreatedObject<T>(T entity)
        {
            statusCode = 201;
        }
        private string fileName = "data.txt";
        private byte[]? bytes;
        private string? contentType;
        public void SetNoContent()
        {
            statusCode = 200;
            data = null;
            type = ResponseResultType.Json;
        }

        public void SetFile(byte[] bytes, string contentType, string fileName)
        {
            this.bytes = bytes;
            this.contentType = contentType;
            this.fileName = fileName;
            type = ResponseResultType.File;
        }

        public IActionResult Build()
        {
            switch (type)
            {
                case ResponseResultType.Json:
                    return new ObjectResult(new
                    {
                        TotalItems = totalItems,
                        Data = data,
                        Error = error,
                        Message = message,
                    })
                    { StatusCode = this.statusCode };

                case ResponseResultType.File:
                    var result = new FileContentResult(bytes ?? Array.Empty<byte>(), contentType ?? "text/plain")
                    {
                        FileDownloadName = fileName
                    };
                    return result;
                default:
                    return new ObjectResult(new
                    {
                        TotalItems = totalItems,
                        Data = data,
                        Error = error,
                        Message = message
                    })
                    { StatusCode = this.statusCode };
            }
        }

        public void AddError(string fieldName, string message)
        {
            error ??= new Dictionary<string, List<string>>();

            if (!error.ContainsKey(fieldName))
            {
                error.Add(fieldName, new List<string>());
            }
            error[fieldName].Add(stringLocalizer[message].Value);

        }
        public void AddInvalidFieldError(string fieldName)
        {
            AddError(fieldName, stringLocalizer["{0} field is invalid", fieldName].Value);
            statusCode = 400;
        }
        public void AddExistedEntityError(string entityName)
        {

            AddError(entityName, stringLocalizer["{0} entity is already exists", entityName]);
            statusCode = 400;
        }
        public void AddNotFoundEntityError(string entityName)
        {
            
            AddError(entityName, stringLocalizer["{0} is not found", entityName]);
            statusCode = 404;
        }

        public void SetData(object? data)
        {
            this.data = data;
            statusCode = 200;
            type = ResponseResultType.Json;
        }

        public void AddNotPermittedError()
        {
            statusCode = 403;
        }
        public void SetPageData<T>(PageData<T> pageData)
        {
            SetData(pageData.ToArray());
            totalItems = pageData.TotalRecords;
        }
    }
    public static class ResponseResultBuilderExtensions
    {
        public static void SetAsset(this IResponseResultBuilder responseResultBuilder, CachedAssetDto cachedAsset)
        {
            responseResultBuilder.SetFile(cachedAsset.Bytes, cachedAsset.ContentType, cachedAsset.Filename);
        }
        public static void SetApplicationResponse<T>(this IResponseResultBuilder responseResultBuilder, ApplicationResponse<T> response, Action<IResponseResultBuilder, T>? onSuccess = null)
        {
            if (response.Error != null)
            {
                switch (response.Error.Type)
                {
                    case ApplicationErrorType.Invalid:
                        responseResultBuilder.AddInvalidFieldError(response.Error.Message ?? "");
                        break;
                    case ApplicationErrorType.NotFound:
                        responseResultBuilder.AddNotFoundEntityError(response.Error.Message ?? "");
                        break;
                    case ApplicationErrorType.NotPermitted:
                        responseResultBuilder.AddNotPermittedError();
                        break;
                    case ApplicationErrorType.Existed:
                        responseResultBuilder.AddExistedEntityError(response.Error.Message ?? "");
                        break;
                }
            }
            else
            {
                if (response.Result == null)
                {
                    throw new Exception("Result must not be null");
                }
                if (onSuccess == null)
                {
                    responseResultBuilder.SetData(response.Result);
                }
                else
                    onSuccess(responseResultBuilder, response.Result);
            }
        }
    }
}
