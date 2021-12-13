using AutoMapper;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using CoreEntityFramework.Repository;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Appoint.Application.Services
{
    public class WX_TOKEN_Service : IWX_TOKEN_Service
    {
        private IRepository<App_DbContext, WX_TOKEN> _repository;
        public WX_TOKEN_Service(IRepository<App_DbContext, WX_TOKEN> repository)
        {
            _repository = repository;
        }
       
        public WX_TOKEN GetToken(string appid)
        {
            return _repository.FirstOrDefault(s=>s.appid==appid);
        }

        public bool InsertOrUpdateToken(WX_TOKEN model)
        {
            string sql = @"merge into  [dbo].[WX_TOKEN] T
                using (select access_token=@access_token,expires_in=@expires_in,appid=@appid) S
				on T.appid=S.appid
				when matched then
					update set T.access_token=S.access_token,T.expires_in =S.expires_in,T.create_time=getdate()
                when not matched then
	                insert (appid,access_token,expires_in,create_time) values(S.appid,S.access_token,S.expires_in,getdate());";
            try
            {
                SqlParameter[] SqlParm = new SqlParameter[]
                {
                    new SqlParameter("@access_token",model.access_token),
                    new SqlParameter("@expires_in",model.expires_in),
                    new SqlParameter("@appid",model.appid),
                };
                return _repository.ExecuteSqlCommand(sql, SqlParm) > 0;
            }
            catch (Exception ex) 
            {
            }
            return false;
        }
    }
}
