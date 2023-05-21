using System.Text.Json.Serialization;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

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
        public readonly static Dictionary<SupportedLanguage, string> UpdatedMessage = new()
        {
            {SupportedLanguage.English, "Updated"},
            {SupportedLanguage.Vietnamese, "Đã cập nhật" }
        };
        public readonly static Dictionary<SupportedLanguage, string> DeletedMessage = new()
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
        private IDictionary<string, List<string>>? error = null;
        private string? message;
        private ResponseResultType type = ResponseResultType.Json;
        private int statusCode = 200;
        private Object? data = null;
        private int? totalItems = null;
        public void SetMessage(IDictionary<SupportedLanguage, string> message)
        {
            var language = languageService.GetLanguage();
            this.message = message[language];
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
            switch (type)
            {
                case ResponseResultType.Json:
                    return new ObjectResult(new
                    {
                        TotalItems = totalItems,
                        Data = data,
                        Error = error,
                        Language = languageService.GetLanguage(),
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
                        Language = languageService.GetLanguage(),
                        Error = error,
                        Message = message
                    })
                    { StatusCode = this.statusCode };
            }
        }

        public void AddError(string fieldName, IDictionary<SupportedLanguage, string> message)
        {
            error ??= new Dictionary<string, List<string>>();

            if (!error.ContainsKey(fieldName))
            {
                error.Add(fieldName, new List<string>());
            }
            var language = languageService.GetLanguage();
            error[fieldName].Add(message[language]);

        }
        public void AddInvalidFieldError(string fieldName)
        {
            var invalidMessage = new Dictionary<SupportedLanguage, string>()
            {
                [SupportedLanguage.Vietnamese] = $"Trường {fieldName} không hợp lệ",
                [SupportedLanguage.English] = $"{fieldName} field is invalid"
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
        public void SetPageData<T>(PageData<T> pageData)
        {
            SetData(pageData.Items);
            totalItems = pageData.TotalRecords;
        }
    }
}
