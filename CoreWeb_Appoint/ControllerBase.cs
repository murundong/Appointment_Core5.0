using CoreEntityFramework.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb_Appoint
{
    public class ControllerBase: Controller
    {

        public IActionResult ReturnJsonResult(object data, string msg = "", Enum_ReturnRes errCode = Enum_ReturnRes.Success)
        {
            return Json(new { errCode = errCode, msg = (string.IsNullOrWhiteSpace(msg) ? errCode.ToString() : msg), data = data });
        }


        public IActionResult ReturnPageResult<T>(T data)
        {
            return Json(data);
        }
        public IActionResult ReturnJsonResult(string msg = "", Enum_ReturnRes errCode = Enum_ReturnRes.Success)
        {
            return ReturnJsonResult(null, msg, errCode);
        }

        public IActionResult ReturnJsonResult()
        {
            return ReturnJsonResult("", Enum_ReturnRes.Success);
        }
    }
}
