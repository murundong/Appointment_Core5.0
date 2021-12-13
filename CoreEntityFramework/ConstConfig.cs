using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework
{
    public class ConstConfig
    {
        private static IConfiguration _configuration;
        public ConstConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string APPID =>_configuration.GetValue<string>("APPID");
        public static string APPSECRET =>_configuration.GetValue<string>("APPSECRET");
        public static string template_cancel =>_configuration.GetValue<string>("template_cancel");
        public static string template_notice =>_configuration.GetValue<string>("template_notice");
        public static string template_queue =>_configuration.GetValue<string>("template_queue");
        public static string template_apply_curator =>_configuration.GetValue<string>("template_apply_curator");
        public static string template_agree_curator =>_configuration.GetValue<string>("template_agree_curator");
        public static string ErrorImg => _configuration.GetValue<string>("ErrorImg");
        public static string SYS_OPENID = "odMBJ49aydSHPVtW1VmrlanhFj14";
        public const string auth_code2Session = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";
        public const string get_accessToken = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        public const string send_subscribe = "https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={0}";
    }
}
