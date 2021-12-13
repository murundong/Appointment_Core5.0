using AutoMapper;
using CoreApplication;
using CoreBaseClass;
using Enyim.Caching;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb.Test.Controllers
{
    public class HomeController : Controller
    {
        private IMapper _mapper;
        private IMemcachedClient _memcachedClient;
        private IDatabase _redisClient;
        public HomeController( IMapper mapper, IMemcachedClient memcachedClient, RedisHelper redisHelper)
        {
            _mapper = mapper;
            _memcachedClient = memcachedClient;
            _redisClient = redisHelper.GetDatabase();
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> AddCache()
        {
            string key = "2021_11_2_rodencached";
            string msg = "hello_cache";

            //var res = await _memcachedClient.AddAsync(key, msg, 60);
            //return Json(res);
            bool _lock=  await _redisClient.LockTakeAsync(key,msg,TimeSpan.FromSeconds(60));
            
            if (!_lock)
            {
                return Json("Click too fast！");
            }
            return Json("ok");
        }

        public async Task<IActionResult> GetCache()
        {
            string key = "2021_11_2_rodencached";
            var res = await _memcachedClient.GetAsync<string>(key);
            return Json(res.Value);
        }

        
    }
}
