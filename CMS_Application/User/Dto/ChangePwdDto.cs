using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS_Application.User.UserInfo.Dto
{
    public class ChangePwdDto
    {
        [Required(ErrorMessage ="账号不能为空")]
        public string userNo { get; set; }
        [Required(ErrorMessage = "旧密码不能为空")]
        public string oldPwd { get; set; }
        [Required(ErrorMessage = "新密码不能为空")]
        public string newPwd { get; set; }
        public DateTime dateTime { get; set; }
        public string updateBy { get; set; }
    }
}
