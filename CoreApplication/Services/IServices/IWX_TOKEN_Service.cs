using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IWX_TOKEN_Service:IApplicationService
    {
        WX_TOKEN GetToken(string appid);

        bool InsertOrUpdateToken(WX_TOKEN model);
    }
}
