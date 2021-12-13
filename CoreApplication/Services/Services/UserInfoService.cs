using AutoMapper;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Enum;
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
    public class UserInfoService : IUserInfoService
    {
        private IRepository<App_DbContext, UserInfos> _repository;
        private IMapper _mapper;

        public UserInfoService(
            IRepository<App_DbContext, UserInfos> repository,
            IMapper mapper
            )
        {
            _repository = repository;
            _mapper = mapper;
        }

        public View_UinfoOutput GetUserInfo(string openid)
        {
            var res = _repository.FirstOrDefault(s => s.open_id == openid);
            return _mapper.Map<View_UinfoOutput>(res);
        }

        public bool CheckHasUser(string openid)
        {
            return _repository.Count(s => s.open_id == openid) > 0;
        }

        public void AddUser(UserInfos model)
        {
            _repository.Insert(model);
            _repository.dbContext.SaveChanges();
        }

        public bool SaveUserInfo(UserInfos model)
        {
             _repository.Insert(model);
            return _repository.dbContext.SaveChanges() > 0;
        }

      
        public View_UinfoOutput UpdateUserInfo_home(UserInfos model)
        {
            var item = _repository.FirstOrDefault(s => s.open_id == model.open_id);
            if (item==null || item.uid<=0)
            {
                _repository.Insert(model);
                _repository.dbContext.SaveChanges();
                return _mapper.Map<View_UinfoOutput>(model);
            }
            else
            {
                item.avatar = model.avatar;
                item.gender = model.gender;
                item.nick_name = model.nick_name;
                item.initial = model.initial;
                _repository.Update(item);
                _repository.dbContext.SaveChanges();
                return _mapper.Map<View_UinfoOutput>(item);
            }
            
        }

        public View_UinfoOutput UpdateUserInfo_setting(UserInfos model)
        {
            var item = _repository.FirstOrDefault(s => s.open_id == model.open_id);
            if (item?.uid <= 0)
            {
                _repository.Insert(model);
                _repository.dbContext.SaveChanges();
                return _mapper.Map<View_UinfoOutput>(model);
            }
            else
            {
                item.avatar = model.avatar;
                item.birthday =  model.birthday ;
                item.tel = model.tel ;
                item.real_name = model.real_name ;
                item.gender = model.gender;
                item.nick_name = model.nick_name;
                _repository.Update(item);
                _repository.dbContext.SaveChanges();
                return _mapper.Map<View_UinfoOutput>(item);
            }
            
        }

        public View_UinfoOutput GetUinfoByOpenid(string openid)
        {
            var res = _repository.FirstOrDefault(s => s.open_id == openid);
            return _mapper.Map<View_UinfoOutput>(res);
        }

        public View_InitialUserInfoOutput GetUserLst_Admin(string nick)
        {
            View_InitialUserInfoOutput return_res = new View_InitialUserInfoOutput();
            var query = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(nick)) query = query.Where(s => s.nick_name.Contains(nick) || s.remark.Contains(nick));
            var res = _mapper.Map<List<View_UinfoOutput>>(query.OrderBy(s=>s.initial));
            List<string> lstLetters = res.Select(s => s.initial)?.Distinct()?.ToList();
            return_res.initials = lstLetters;
            if (lstLetters?.Count > 0)
            {
                lstLetters.ForEach(s =>
                {
                    View_InitialUserInfoItemOutput item = new View_InitialUserInfoItemOutput()
                    {
                        initial = s,
                        uinfos = res.Where(p => p.initial == s)?.ToList()
                    };
                    return_res.uinfos.Add(item);
                });
            }
            return return_res;
        }

        public bool AllocRole(int uid, Enum_UserRole role)
        {
            var entity = _repository.FirstOrDefault(s => s.uid == uid);
            if(entity!=null && entity.uid > 0)
            {
                entity.role = role;
                _repository.Update(entity);
                return _repository.dbContext.SaveChanges() > 0;
            }
            return false;
        }

        public bool SetUSerRemark(int uid, string remark)
        {
            var entity = _repository.FirstOrDefault(s => s.uid == uid);
            if (entity != null && entity.uid > 0)
            {
                entity.remark = remark;
                _repository.Update(entity);
                return _repository.dbContext.SaveChanges() > 0;
            }
            return false;
        }
    }
}
