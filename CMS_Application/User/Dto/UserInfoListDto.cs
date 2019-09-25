using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.User.UserInfo.Dto
{
    public class UserInfoListDto
    {
        public int userId { get; set; }
        public string userNo { get; set; }
        public string userName { get; set; }
        public string userTel { get; set; }
        public string passWord { get; set; }
        public int userState { get; set; }
        public int[] roleIds { get; set; }
        public string updateBy { get; set; }
    }
}
