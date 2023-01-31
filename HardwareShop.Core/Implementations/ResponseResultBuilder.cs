using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Core.Implementations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseResultType
    {
        Json,
        File,
    }

    public static class ResponseMessages
    {
        public readonly static Dictionary<SupportedLanguage, string> UpdatedMessage = new Dictionary<SupportedLanguage, string>
        {
            {SupportedLanguage.English, "Updated"},
            {SupportedLanguage.Vietnamese, "Đã cập nhật" }
        };
        public readonly static Dictionary<SupportedLanguage, string> DeletedMessage = new Dictionary<SupportedLanguage, string>
        {
            {SupportedLanguage.English, "Deleted"},
            {SupportedLanguage.Vietnamese, "Đã xoá" }
        };
    }
    public class ResponseResultBuilder : IResponseResultBuilder
    {
        private readonly ILanguageService languageService;

        public ResponseResultBuilder(ILanguageService languageService)
        {
            this.languageService = languageService;
        }
        private IDictionary<string, List<string>>? error { get; set; }
        private string? message;
        private ResponseResultType type = ResponseResultType.Json;
        private int statusCode { get; set; } = 200;
        private Object? data { get; set; }
        private int? totalItems { get; set; }
        public void SetMessage(IDictionary<SupportedLanguage, string> message)
        {
            var configuration = languageService.GetConfiguration();
            this.message = message[configuration.Language];
        }
        public void SetUpdatedMessage()
        {
            SetMessage(ResponseMessages.UpdatedMessage);
        }
        public void SetDeletedMessage()
        {
            SetMessage(ResponseMessages.DeletedMessage);
        }
        public void SetCreatedObject<T>(T entity) where T : EntityBase
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

            var dynamicError = new { };

            switch (type)
            {
                case ResponseResultType.Json:
                    return new ObjectResult(new
                    {
                        TotalItems = totalItems,
                        Type = type,
                        Data = data,
                        Error = error,
                        Message = message,
                    })
                    { StatusCode = this.statusCode };

                case ResponseResultType.File:
                    var result = new FileContentResult(bytes ?? new Byte[0], contentType ?? "text/plain");
                    result.FileDownloadName = fileName;
                    return result;
                default:
                    return new ObjectResult(new
                    {
                        TotalItems = totalItems,
                        Type = type,
                        Data = data,
                        Error = error,
                        Message = message
                    })
                    { StatusCode = this.statusCode };
            }
        }

        public void AddError(string fieldName, IDictionary<SupportedLanguage, string> message)
        {
            if (error is null)
            {
                error = new Dictionary<string, List<string>>();
            }

            if (!error.ContainsKey(fieldName))
            {
                error.Add(fieldName, new List<string>());
            }
            var configuration = languageService.GetConfiguration();
            error[fieldName].Add(message[configuration.Language]);

        }
        public void AddInvalidFieldError(string fieldName)
        {

            var invalidMessage = new Dictionary<SupportedLanguage, string>()
            {
                [SupportedLanguage.Vietnamese] = $"Trường {fieldName} không hợp lệ",
                [SupportedLanguage.English] = $"{fileName} field is invalid"
            };

            AddError(fieldName, invalidMessage);
            statusCode = 400;
        }
        public void AddExistedEntityError(string entityName)
        {
            var invalidMessage = new Dictionary<SupportedLanguage, string>()
            {
                [SupportedLanguage.Vietnamese] = $"{entityName} đã tồn tại",
                [SupportedLanguage.English] = $"{entityName} entity is already exists"
            };
            AddError(entityName, invalidMessage);
            statusCode = 400;
        }
        public void AddNotFoundEntityError(string entityName)
        {
            var invalidMessage = new Dictionary<SupportedLanguage, string>()
            {
                [SupportedLanguage.Vietnamese] = $"{entityName} không tìm thấy",
                [SupportedLanguage.English] = $"{entityName} is not found"
            };
            AddError(entityName, invalidMessage);
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

        public void SetAsset(IAssetTable asset)
        {
            SetFile(asset.Bytes, asset.ContentType, asset.Filename);
        }
    }
}
