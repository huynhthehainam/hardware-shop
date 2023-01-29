using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseResultLanguage
    {
        Vietnamese,
        English
    }
    public class ResponseResultConfiguration
    {
        public ResponseResultLanguage Language { get; set; } = ResponseResultLanguage.English;
    }
    public static class ResponseMessages
    {
        public readonly static Dictionary<ResponseResultLanguage, string> UpdatedMessage = new Dictionary<ResponseResultLanguage, string>
        {
            {ResponseResultLanguage.English, "Updated"},
            {ResponseResultLanguage.Vietnamese, "Đã cập nhật" }
        };
        public readonly static Dictionary<ResponseResultLanguage, string> DeletedMessage = new Dictionary<ResponseResultLanguage, string>
        {
            {ResponseResultLanguage.English, "Deleted"},
            {ResponseResultLanguage.Vietnamese, "Đã xoá" }
        };
    }
    public class ResponseResultBuilder : IResponseResultBuilder
    {
        private readonly ResponseResultConfiguration configuration;
        public ResponseResultConfiguration GetConfiguration()
        {
            return configuration;
        }

        public ResponseResultBuilder(IOptions<ResponseResultConfiguration> options)
        {
            this.configuration = options.Value;
        }
        private IDictionary<string, List<string>>? errors { get; set; }
        private string? message;
        private ResponseResultType type = ResponseResultType.Json;
        private int statusCode { get; set; } = 200;
        private Object? data { get; set; }
        private int? totalItems { get; set; }
        public void SetMessage(IDictionary<ResponseResultLanguage, string> message)
        {
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
            switch (type)
            {
                case ResponseResultType.Json:
                    return new ObjectResult(new
                    {
                        TotalItems = totalItems,
                        Type = type,
                        Data = data,
                        Errors = errors,
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
                        Errors = errors,
                        Message = message
                    })
                    { StatusCode = this.statusCode };
            }
        }

        public void AddError(string fieldName, IDictionary<ResponseResultLanguage, string> message)
        {
            if (errors is null)
            {
                errors = new Dictionary<string, List<string>>();
            }

            if (!errors.ContainsKey(fieldName))
            {
                errors.Add(fieldName, new List<string>());
            }

            errors[fieldName].Add(message[configuration.Language]);

        }
        public void AddInvalidFieldError(string fieldName)
        {

            var invalidMessage = new Dictionary<ResponseResultLanguage, string>()
            {
                [ResponseResultLanguage.Vietnamese] = $"Trường {fieldName} không hợp lệ",
                [ResponseResultLanguage.English] = $"{fileName} field is invalid"
            };

            AddError(fieldName, invalidMessage);
            statusCode = 400;
        }
        public void AddExistedEntityError(string entityName)
        {
            var invalidMessage = new Dictionary<ResponseResultLanguage, string>()
            {
                [ResponseResultLanguage.Vietnamese] = $"{entityName} đã tồn tại",
                [ResponseResultLanguage.English] = $"{entityName} entity is already exists"
            };
            AddError(entityName, invalidMessage);
            statusCode = 400;
        }
        public void AddNotFoundEntityError(string entityName)
        {
            var invalidMessage = new Dictionary<ResponseResultLanguage, string>()
            {
                [ResponseResultLanguage.Vietnamese] = $"{entityName} không tìm thấy",
                [ResponseResultLanguage.English] = $"{entityName} is not found"
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
