using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Implementations
{
    public class ResponseResultFactory : IResponseResultFactory
    {
        private readonly ResponseResultConfiguration configuration;
        public ResponseResultFactory(IOptions<ResponseResultConfiguration> options)
        {
            this.configuration = options.Value;
        }
        public ResponseResult Create()
        {
            return new ResponseResult(configuration);
        }
    }
}
