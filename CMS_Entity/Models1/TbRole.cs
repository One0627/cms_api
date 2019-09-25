using System;
using System.Collections.Generic;

namespace CMS_Entity.Models
{
    public partial class TbRole
    {
        public TbRole()
        {
            TbPerRelation = new HashSet<TbPerRelation>();
            TbUserRelation = new HashSet<TbUserRelation>();
        }

        public int RoleId { get; set; }
        public string RoleNo { get; set; }
        public string RoleName { get; set; }
        public int? IsDelete { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual ICollection<TbPerRelation> TbPerRelation { get; set; }
        public virtual ICollection<TbUserRelation> TbUserRelation { get; set; }
    }
}
