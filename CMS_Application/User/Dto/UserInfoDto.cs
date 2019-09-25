using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.User.UserInfo.Dto
{
    public class UserInfoDto
    {
        public int userId { get; set; }
        public string userNo { get; set; }
        public string userName { get; set; }
        public string userTel { get; set; }
        public List<Permissions> permissions { get; set; }
    }
    public class Permissions
    {
        public string path { get; set; }
        public string name { get; set; }
        public int menuNo { get; set; }
        public meta meta { get; set; }
        public List<Permissions> children { get; set; }
    }

    public class meta
    {
        public string icon { get; set; }
        public string title { get; set; }
        public bool search { get; set; }
        public bool add { get; set; }
        public bool delete { get; set; }
        public bool update { get; set; }
        public bool import { get; set; }
        public bool export { get; set; }
        public bool _lock { get; set; }
    }
}
