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
        public bool? SearchState { get; set; }
        public bool? AddState { get; set; }
        public bool? DeleteState { get; set; }
        public bool? UpdateState { get; set; }
        public bool? ImportState { get; set; }
        public bool? ErportState { get; set; }
        public bool? LockState { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public TbMenu Menu { get; set; }
        public ICollection<TbPerRelation> TbPerRelation { get; set; }
    }
}
