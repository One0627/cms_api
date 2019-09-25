using System;
using System.Collections.Generic;

namespace CMS_Entity.Models
{
    public partial class TbPerRelation
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public TbPermission Permiss { get; set; }
        public TbRole Role { get; set; }
    }
}
