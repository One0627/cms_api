using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.Role.Dto
{
    public class RoleSelectDto
    {
        public int roleId { get; set; }
        public string roleNo { get; set; }
        public string roleName { get; set; }
        public int[] permission { get; set; }
    }
}
