
using CMS_Application._TableDto;
using CMS_Application.User.Dto;
using CMS_Application.User.UserInfo.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Application.User.UserInfo
{
    public interface IUserInfoService
    {
        Task<UserLoginOutPutDto> Login(UserLoginInputDto dto);

        Task<UserInfoDto> GetUserInfoBy(int UserId);

        bool ChangePwd(ChangePwdDto dto);

        Task<TableOutputDto<UserInfoListDto>> UserInfoList(TableInputDto dto);

        bool AddOrEditUserInfo(UserInfoListDto dto);

        bool DeleteUser(int userId,string updateBy);
    }
}
