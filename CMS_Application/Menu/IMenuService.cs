using System.Collections.Generic;
using CMS_Application.Menu.Dto;

namespace CMS_Application.Menu
{
    public interface IMenuService
    {
        bool AddOrEditMenu(MenuDto dto);
        bool DeleteNode(int menuId,string updateBy);
        bool DownNode(MenuDto dto);
        List<MenuDto> GetMenus();
        bool UpNode(MenuDto dto);
    }
}