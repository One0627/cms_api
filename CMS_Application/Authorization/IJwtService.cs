using CMS_Application.Authorization.Dto;
using CMS_Application.User.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Application.Authorization
{
    public interface IJwtService
    {
        /// <summary>
        ///创建Token
        /// </summary>
        /// <param name="dto">用户信息</param>
        /// <returns></returns>
        string CreateToken(UserLoginOutPutDto dto);
        /// <summary>
        ///刷新Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="dto">用户信息</param>
        /// <returns></returns>
        Task<JwtResponseDto> RefreshTokenAsync(string token, UserLoginInputDto dto);
        /// <summary>
        ///判断当前Token是否有效
        /// </summary>
        /// <returns></returns>
        Task<bool> IsCurrentActiveTokenAsync();
        /// <summary>
        ///停用当前Token
        /// </summary>
        /// <returns></returns>
        Task DeactivateCurrentTokenAsync();
        /// <summary>
        ///判断Token是否有效 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsActiveTokenAsync(string token);
        /// <summary>
        ///停用Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeactivateTokenAsync(string token);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> GetCurrentUserId();
    }
}
