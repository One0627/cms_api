using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS_Application.Menu.Dto;
using CMS_Entity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CMS_Application.Menu
{
    public class MenuService : IMenuService
    {
        protected new_TTS_OrderContext _dbContext;

        public MenuService(new_TTS_OrderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MenuDto> GetMenus()
        {
            List<MenuDto> recur(List<TbMenu> resource, int? parentId = null)
            {
                return resource.Where(x => x.MenuParentId == parentId).Select(x => new MenuDto
                {
                    id = x.MenuId,
                    name = x.MenuName,
                    menuNo = x.MenuNo,
                    url = x.MenuUrl,
                    icon = x.MenuIcon,
                    parentId = x.MenuParentId,
                    children = recur(x.InverseMenuParent.ToList(), x.MenuId)
                }).ToList();
            }
            var entity = _dbContext.TbMenu.Where(x=>x.IsDelete!=1).OrderBy(x => x.MenuNo).ToList();
            return recur(entity);
        }
        public bool AddOrEditMenu(MenuDto dto)
        {
            if (dto.id == 0)
            {
                var node = new TbMenu
                {
                    MenuName = dto.name,
                    MenuIcon = dto.icon,
                    MenuUrl = dto.url,
                    MenuNo = dto.menuNo,
                    MenuParentId = dto.parentId,
                    UpdateBy =dto.updateBy
                };
                using (_dbContext)
                {
                    _dbContext.Add(node);
                    return _dbContext.SaveChanges() > 0 ? true : false;
                }
            }
            else
            {
                var entity = _dbContext.TbMenu.First(x => x.MenuId == dto.id);
                entity.MenuName = dto.name;
                entity.MenuIcon = dto.icon;
                entity.MenuUrl = dto.url;
                entity.MenuNo = dto.menuNo;
                entity.UpdateBy = dto.updateBy;
                _dbContext.Update(entity);
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
        }
        public bool DeleteNode(int menuId,string updateBy)
        {
            using (_dbContext)
            {
                _dbContext.TbMenu.First(x => x.MenuId == menuId).IsDelete = 1;
                _dbContext.TbMenu.First(x => x.MenuId == menuId).UpdateBy = updateBy;
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
        }
        public bool UpNode(MenuDto dto)
        {
            var entity= _dbContext.TbMenu.Where(x => x.MenuParentId == dto.parentId &&(x.MenuNo==dto.menuNo||x.MenuNo==dto.menuNo-1)).ToList();
            return MoveNode(entity);
        }
        public bool DownNode(MenuDto dto)
        {
            var entity = _dbContext.TbMenu.Where(x => x.MenuParentId == dto.parentId && (x.MenuNo == dto.menuNo || x.MenuNo == dto.menuNo+1)).ToList();
            return MoveNode(entity);
        }
        private bool MoveNode(List<TbMenu> entity)
        {
            var temp = entity.First().MenuNo;
            entity.First().MenuNo = entity.Last().MenuNo;
            entity.Last().MenuNo = temp;
            using (_dbContext)
            {
                _dbContext.UpdateRange(entity);
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
        }
    }
}
