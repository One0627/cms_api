using System;
using System.Collections.Generic;

namespace CMS_Entity.Models
{
    public partial class TbMenu
    {
        public TbMenu()
        {
            InverseMenuParent = new HashSet<TbMenu>();
            TbPermission = new HashSet<TbPermission>();
        }

        public int MenuId { get; set; }
        public string MenuNo { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int? MenuParentId { get; set; }
        public int? IsDelete { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual TbMenu MenuParent { get; set; }
        public virtual ICollection<TbMenu> InverseMenuParent { get; set; }
        public virtual ICollection<TbPermission> TbPermission { get; set; }
    }
}
