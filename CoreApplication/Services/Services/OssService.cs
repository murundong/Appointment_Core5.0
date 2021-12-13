using Aliyun.OSS;
using CoreApplication.Services.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.Services
{
    public class OssService : IOssService
    {
        private IConfiguration _configuration;
        public OssService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string endpoint => _configuration.GetValue<string>("Endpoint");
        string AccessKeyID => _configuration.GetValue<string>("AccessKeyID");
        string AccessKeySecret => _configuration.GetValue<string>("AccessKeySecret");
        string BucketName => _configuration.GetValue<string>("BucketName");

        public void UploadFile(string name, Stream inputStream)
        {
            try
            {
                OssClient client = new OssClient("", "", "");
                var res = client.PutObject(BucketName, name, inputStream);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
