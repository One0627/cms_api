using CMS_Application._TableDto;
using CMS_Application.Role.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.Role
{
    public interface IRoleInfoService
    {
        List<RolePermisDto> RoleSelectDto();

        TableOutputDto<RolePermisDto> RoleInfoList(TableInputDto dto);

        bool AddOrEditRolePermis(RolePermisDto dto);
    }
}
