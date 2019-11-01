using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS_Application.User.Dto
{
    public class UserLoginInputDto
    {
        [Required(ErrorMessage ="用户名不能为空")]
        public string userNo { get; set; }
        [Required(ErrorMessage ="密码不能为空")]
        public string passWord { get; set; }
    }
}
