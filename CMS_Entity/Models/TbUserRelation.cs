using System;
using System.Collections.Generic;

namespace CMS_Entity.Models
{
    public partial class TbUserRelation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public TbRole Role { get; set; }
        public TbUser User { get; set; }
    }
}
