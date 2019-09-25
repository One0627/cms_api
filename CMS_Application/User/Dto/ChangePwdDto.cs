using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.User.UserInfo.Dto
{
    public class ChangePwdDto
    {
        public string userNo { get; set; }
        public string oldPwd { get; set; }
        public string newPwd { get; set; }
        public DateTime dateTime { get; set; }
        public string updateBy { get; set; }
    }
}
