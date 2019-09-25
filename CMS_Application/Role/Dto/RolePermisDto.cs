using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.Role.Dto
{
    public class RolePermisDto
    {
        public int roleId { get; set; }
        public string roleNo { get; set; }
        public string roleName { get; set; }
        public string updateBy { get; set; }
        public List<Permission> permissions { get; set; }
    }
    public class Permission
    {
        public int permissId { get; set; }
        public int menuId { get; set; }
        public bool? search { get; set; } = null;
        public bool? add { get; set; } = null;
        public bool? delete { get; set; } = null;
        public bool? update { get; set; } = null;
        public bool? import { get; set; } = null;
        public bool? export { get; set; } = null;
        public bool? _lock { get; set; } = null;
    }
}
