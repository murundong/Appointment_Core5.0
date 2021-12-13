using CoreEntityFramework.WeixData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.IWeixinApi
{
    public interface IWeixinService : IApplicationService
    {
        W_AUTH_RETURN GetOpenIdByCode(string code);

        W_TOKEN_RETURN GetToken();

        W_RETURN SendSubsCribe(W_SUBS_DATA_INPUT input);

    }
}
