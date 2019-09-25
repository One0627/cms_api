using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMS_Application._TableDto;
using CMS_Application.Role.Dto;
using CMS_Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS_Application.Role
{
    public class RoleInfoService : IRoleInfoService
    {
        protected new_TTS_OrderContext _dbContext;

        public RoleInfoService(new_TTS_OrderContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<RolePermisDto> RoleSelectDto()
        {
            var data = _dbContext.TbRole.ToList().Select(x => new RolePermisDto
            {
                roleId = x.RoleId,
                roleName = x.RoleName,
                roleNo = x.RoleNo
            }).ToList();
            return data;
        }

        public TableOutputDto<RolePermisDto> RoleInfoList(TableInputDto dto)
        {
            var entity = _dbContext.TbRole.Include("TbPerRelation.Permiss.Menu.InverseMenuParent").AsQueryable();
            if (!string.IsNullOrWhiteSpace(dto.QueryString.Trim()))
            {
                switch (dto.QueryType)
                {
                    case "roleNo":
                        entity = entity.Where(x => x.RoleNo.Contains(dto.QueryString));
                        break;
                    case "roleName":
                        entity = entity.Where(x => x.RoleName.Contains(dto.QueryString));
                        break;
                }
            }
            var total = entity.Where(x => x.IsDelete != 1).Count();
            var list = entity.Where(x => x.IsDelete != 1).OrderBy(x => x.RoleId).Skip((dto.currentPage - 1) * dto.pageSize).Take(dto.pageSize).ToList().Select(x =>
            {
                var per = x.TbPerRelation.Where(y => y.Permiss.Menu.InverseMenuParent.Where(z=>z.IsDelete!=1).Count() == 0).Select(z => z.Permiss).ToList();
                return new RolePermisDto
                {
                    roleId = x.RoleId,
                    roleNo = x.RoleNo,
                    roleName = x.RoleName,
                    permissions = per.Select(y => new Permission
                    {
                        permissId = y.PermissId,
                        menuId = y.MenuId,
                        search = y.SearchState ?? true,
                        add = y.AddState ?? true,
                        delete = y.DeleteState ?? true,
                        update = y.UpdateState ?? true,
                        import = y.ImportState ?? true,
                        export = y.ErportState ?? true,
                        _lock = y.LockState ?? true
                    }).ToList()
                };
            }).ToList();
            return new TableOutputDto<RolePermisDto> { TableData = list, Total = total };
        }
        public bool AddOrEditRolePermis(RolePermisDto dto)
        {
            var newPermis = dto.permissions.Select(x => new TbPermission
            {
                MenuId = x.menuId,
                SearchState = x.search,
                AddState = x.add,
                DeleteState = x.delete,
                UpdateState = x.update,
                ImportState = x.import,
                ErportState = x.export,
                LockState = x._lock,
                UpdateBy = dto.updateBy

            });
            var newPerRelation = newPermis.Select(x => new TbPerRelation { Permiss = x }).ToList();
            if (dto.roleId != 0)
            {
                using (_dbContext)
                {
                    var entity = _dbContext.TbRole.Include("TbPerRelation.Permiss").FirstOrDefault(x => x.RoleId == dto.roleId);
                    _dbContext.TbPermission.RemoveRange(entity.TbPerRelation.Select(x => x.Permiss));
                    entity.RoleName = dto.roleName;
                    entity.TbPerRelation = newPerRelation;
                    _dbContext.TbRole.Update(entity);
                    return _dbContext.SaveChanges() > 0 ? true : false;
                }
            }
            else
            {
                using (_dbContext)
                {
                    var newRole = new TbRole
                    {
                        RoleId = dto.roleId,
                        RoleNo = dto.roleNo,
                        RoleName = dto.roleName,
                        TbPerRelation = newPerRelation,
                        UpdateBy = dto.updateBy
                    };
                    _dbContext.TbRole.AddRange(newRole);
                    return _dbContext.SaveChanges() > 0 ? true : false;
                }
            }
        }
    }
}
