using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS_Application._TableDto;
using CMS_Application.User.Dto;
using CMS_Application.User.UserInfo.Dto;
using CMS_Entity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Application.User.UserInfo
{
    public class UserInfoService : IUserInfoService
    {
        protected new_TTS_OrderContext _dbContext;
        private readonly IHostingEnvironment _env;

        public UserInfoService(new_TTS_OrderContext dbContext, IHostingEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<UserLoginOutPutDto> Login(UserLoginInputDto dto)
        {
            var entity = await _dbContext.TbUser.FirstOrDefaultAsync(x => x.UserNo == dto.userNo && x.UserPassword == dto.passWord && x.UserState == 1 && x.IsDelete != 1);
            if (entity != null)
            {
                return new UserLoginOutPutDto
                {
                    userId = entity.UserId,
                    userName = entity.UserName
                };
            }
            return null;
        }

        #region 获取当前登录用户信息及权限
        /// <summary>
        /// 获取个人信息及权限
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<UserInfoDto> GetUserInfoBy(int UserId)
        {
            var entity = await _dbContext.TbUser.FirstOrDefaultAsync(x => x.UserId == UserId);
            List<Permissions> pers;
            if (UserId == 1 || _env.IsDevelopment())
            {
                pers = GetPermissions(UserId, true);
            }
            else
            {
                pers = GetPermissions(UserId);
            }
            return new UserInfoDto
            {
                userId = entity.UserId,
                userNo = entity.UserNo,
                userTel = entity.UserTel,
                userName = entity.UserName,
                permissions = pers
            };
        }
        #endregion
        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否为管理员</param>
        /// <returns></returns>
        private List<Permissions> GetPermissions(int userId, bool isAdmin = false)
        {
            var roleIds = _dbContext.TbUserRelation.Where(x => x.UserId == userId).Select(x => x.RoleId).ToArray();
            var permisIds = _dbContext.TbPerRelation.Where(x => roleIds.Contains(x.RoleId)).Select(x => x.PermissId).ToArray();
            var permiss = _dbContext.TbPermission.Where(x => permisIds.Contains(x.PermissId)).ToList();
            var perGroup = permiss.GroupBy(x => x.MenuId).ToList();
            var menus =isAdmin ? _dbContext.TbMenu.Where(x=>x.IsDelete!=1).ToList(): _dbContext.TbMenu.Where(x => perGroup.Select(y => y.Key).Contains(x.MenuId)).ToList();
            List<Permissions> Permissions(IEnumerable<TbMenu> menu)
            {
                if (menu.Count() == 0) return null;
                List<Permissions> list = new List<Permissions>();
                list = menu.Select(x => new Permissions
                {
                    path = x.MenuUrl,
                    name = x.MenuUrl.Split("/").Count() > 1 ? x.MenuUrl.Split("/")[1] : x.MenuUrl,
                    menuNo = x.MenuNo,
                    meta = new meta
                    {
                        icon = x.MenuIcon,
                        title = x.MenuName,
                        add = permiss.Where(y => y.AddState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                        delete = permiss.Where(y => y.DeleteState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                        update = permiss.Where(y => y.UpdateState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                        search = permiss.Where(y => y.SearchState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                        import = permiss.Where(y => y.ImportState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                        export = permiss.Where(y => y.ErportState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                        _lock = permiss.Where(y => y.LockState != false && y.MenuId == x.MenuId).Count() > 0 | isAdmin ? true : false,
                    },
                    children = Permissions(menus.Where(z => z.MenuParentId == x.MenuId))
                }).OrderBy(x => x.menuNo).ToList();
                return list;
            }
            return Permissions(menus.Where(x => x.MenuParentId == null));
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool ChangePwd(ChangePwdDto dto)
        {
            var entity = _dbContext.TbUser.FirstOrDefault(x => x.UserNo == dto.userNo && x.UserPassword == dto.oldPwd);
            if (entity != null)
            {
                entity.UserPassword = dto.newPwd;
                entity.UpdateTime = DateTime.Now;
                entity.UpdateBy = entity.UserId.ToString();
                using (_dbContext)
                {
                    _dbContext.TbUser.Update(entity);
                    return _dbContext.SaveChanges() > 0 ? true : false;
                }
            }
            return false;
        }
        /// <summary>
        /// 用户信息集合
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TableOutputDto<UserInfoListDto> UserInfoList(TableInputDto dto)
        {
            var entity = _dbContext.TbUser.Include("TbUserRelation").AsQueryable();
            if (!string.IsNullOrWhiteSpace(dto.QueryString.Trim()))
            {
                switch (dto.QueryType)
                {
                    case "userNo":
                        entity = entity.Where(x => x.UserNo.Contains(dto.QueryString));
                        break;
                    case "userName":
                        entity = entity.Where(x => x.UserName.Contains(dto.QueryString));
                        break;
                    case "userTel":
                        entity = entity.Where(x => x.UserTel.Contains(dto.QueryString));
                        break;
                    case "userState":
                        entity = entity.Where(x => x.UserState == int.Parse(dto.QueryString));
                        break;

                }
            }
            var total = entity.Where(x => x.IsDelete != 1).Count();
            var list = entity.Where(x => x.IsDelete != 1).OrderBy(x => x.UserId).Skip((dto.currentPage - 1) * dto.pageSize).Take(dto.pageSize).ToList().Select(x =>
                {
                    List<int> ids = new List<int>();
                    foreach (var item in x.TbUserRelation)
                    {
                        ids.Add(item.RoleId);
                    }
                    return new UserInfoListDto
                    {
                        userId = x.UserId,
                        userName = x.UserName,
                        userNo = x.UserNo,
                        userTel = x.UserTel,
                        userState = x.UserState,
                        roleIds = ids.ToArray()
                    };
                }).ToList();

            return new TableOutputDto<UserInfoListDto> { TableData = list, Total = total };

        }
        /// <summary>
        /// 添加或编辑用户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool AddOrEditUserInfo(UserInfoListDto dto)
        {
            var entity = _dbContext.TbUser.Include("TbUserRelation").FirstOrDefault(x => x.UserNo == dto.userNo);
            if (dto.userId != 0)
            {
                entity.UserName = dto.userName;
                entity.UserState = dto.userState;
                entity.UserTel = dto.userTel;
                entity.UpdateBy = dto.updateBy;
                entity.TbUserRelation = dto.roleIds.Select(x => new TbUserRelation
                {
                    UserId = dto.userId,
                    RoleId = x
                }).ToList();
                using (_dbContext)
                {
                    _dbContext.TbUser.Update(entity);
                    return _dbContext.SaveChanges() > 0 ? true : false;
                }
            }
            else if (entity == null)
            {
                entity = new TbUser
                {
                    UserNo = dto.userNo,
                    UserName = dto.userName,
                    UserPassword = dto.passWord,
                    UserState = dto.userState,
                    UserTel = dto.userTel,
                    TbUserRelation = dto.roleIds.Select(x => new TbUserRelation
                    {
                        UserId = dto.userId,
                        RoleId = x
                    }).ToList()
                };
                using (_dbContext)
                {
                    _dbContext.TbUser.Add(entity);
                    return _dbContext.SaveChanges() > 0 ? true : false;
                }
            }
            return false;
        }

        public bool DeleteUser(int userId,string updateBy)
        {
            using (_dbContext)
            {
                _dbContext.TbUser.First(x => x.UserId == userId).IsDelete = 1;
                _dbContext.TbUser.First(x => x.UserId == userId).UpdateBy = updateBy;
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
        }
    }
}
