using HardwareShop.Core.Bases;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
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
        public ResponseResultLanguage Language { get; set; }
        public ResponseResultConfiguration(ResponseResultLanguage language)
        {
            Language = language;
        }
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
    public class ResponseResult
    {
        private readonly ResponseResultConfiguration configuration;

        public ResponseResult(ResponseResultConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IDictionary<string, string>? Errors { get; set; }
        public string? Message { get; internal set; }
        private ResponseResultType type = ResponseResultType.Json;
        public int StatusCode { get; set; } = 200;
        public Object? Data { get; set; }
        public int? TotalItems { get; set; }
        public void SetMessage(IDictionary<ResponseResultLanguage, string> message)
        {
            this.Message = message[configuration.Language];
        }
        public void SetUpdatedMessage()
        {
            SetMessage(ResponseMessages.UpdatedMessage);
        }
        public void SetDeletedMessage()
        {
            SetMessage(ResponseMessages.DeletedMessage);
        }
        public void SetCreatedObject<T>(EntityBase<T> entity) where T : struct
        {
            Data = new { Id = entity.Id };
            StatusCode = 201;
        }
        public string fileName = "data.txt";
        public byte[]? bytes;
        public string? contentType;
        public void SetNoContent()
        {
            StatusCode = 200;
            Data = null;
            type = ResponseResultType.Json;
        }

        public void SetFile(byte[] bytes, string contentType, string fileName)
        {
            this.bytes = bytes;
            this.contentType = contentType;
            this.fileName = fileName;
            type = ResponseResultType.File;
        }

        public IActionResult ToResult()
        {
            switch (type)
            {
                case ResponseResultType.Json:
                    return new ObjectResult(new
                    {
                        TotalItems = TotalItems,
                        Type = type,
                        Data = Data,
                        Errors = Errors,
                        Message = Message
                    })
                    { StatusCode = this.StatusCode };

                case ResponseResultType.File:
                    var result = new FileContentResult(bytes ?? new Byte[0], contentType ?? "text/plain");
                    result.FileDownloadName = fileName;
                    return result;
                default:
                    return new ObjectResult(new
                    {
                        TotalItems = TotalItems,
                        Type = type,
                        Data = Data,
                        Errors = Errors,
                        Message = Message
                    })
                    { StatusCode = this.StatusCode };
            }
        }

    }
}
