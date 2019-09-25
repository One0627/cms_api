using System;
using System.Collections.Generic;

namespace CMS_Entity.Models
{
    public partial class TbPermission
    {
        public TbPermission()
        {
            TbPerRelation = new HashSet<TbPerRelation>();
        }

        public int PermissId { get; set; }
        public int MenuId { get; set; }
        public int? SearchState { get; set; }
        public int? AddState { get; set; }
        public int? DeleteState { get; set; }
        public int? UpdateState { get; set; }
        public int? ImportState { get; set; }
        public int? ErportState { get; set; }
        public int? LockState { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual TbMenu Menu { get; set; }
        public virtual ICollection<TbPerRelation> TbPerRelation { get; set; }
    }
}
