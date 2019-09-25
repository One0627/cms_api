using System;
using System.Collections.Generic;

namespace CMS_Entity.Models
{
    public partial class TbUser
    {
        public TbUser()
        {
            TbUserRelation = new HashSet<TbUserRelation>();
        }

        public int UserId { get; set; }
        public string UserNo { get; set; }
        public string UserName { get; set; }
        public string UserTel { get; set; }
        public string UserPassword { get; set; }
        public int? IsDelete { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual ICollection<TbUserRelation> TbUserRelation { get; set; }
    }
}
