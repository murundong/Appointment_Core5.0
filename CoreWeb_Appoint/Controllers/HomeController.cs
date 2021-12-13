using CoreApplication.Services.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb_Appoint.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _env;
        private IOssService _ossService;
        public HomeController(IConfiguration configuration, IWebHostEnvironment env, IOssService ossService)
        {
            _configuration = configuration;
            _ossService = ossService;
            _env = env;
        }

        public IActionResult UploadFile(IFormCollection fromCollection)
        {
            string dir = _configuration.GetValue<string>("UploadFile");
            string OssDir = _configuration.GetValue<string>("BucketDir");
            bool UseOSS = _configuration.GetValue<bool>("UseOSS");

            string dirName = _env.WebRootPath + Path.Combine(dir, DateTime.Now.ToString("yyyyMMdd"));
            List<string> lstRealNames = new List<string>();
            var files = (FormFileCollection)fromCollection.Files;
            if (files != null && files.Count > 0)
            {
                foreach (var item in files)
                {
                    var newFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(item.FileName);
                
                    if (UseOSS)
                    {
                        string ossUploadPath = $"{OssDir}{newFileName}";
                        _ossService.UploadFile(ossUploadPath, item.OpenReadStream());
                        lstRealNames.Add(ossUploadPath);
                    }
                    else
                    {
                        if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
                        var fileName = Path.Combine(dirName, newFileName);
                        using (var fs = System.IO.File.Create(fileName))
                        {
                            item.CopyTo(fs);
                            fs.Flush();
                        }
                        lstRealNames.Add(fileName);
                    }
                }
            }
            return Json(new { data = string.Join(",",lstRealNames) });
        }

    }
}
