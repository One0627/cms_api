using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_Application.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CMS_WEB.API.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 获取 HTTP 请求上下文
        /// </summary>
        protected IHttpContextAccessor _httpContextAccessor;

        //public BaseController(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}
        /// <summary>
        /// 获取 HTTP 请求的 Token 值
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected string CurrentToken()
        {
            //http header
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            //token
            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();// bearer tokenvalue
        }
        /// <summary>
        /// 获取 HTTP 请求的UserId
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected async Task<string> CurrentClaimByType(cType type = cType.USERNAME)
        {
            var typeStr = string.Empty;
            switch (type)
            {
                case cType.USERID:
                    typeStr = "UserId";
                    break;
                case cType.USERNAME:
                    typeStr = "UserName";
                    break;
                default:
                    break;
            }
            try
            {
                var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
                if (auth.Succeeded)
                {
                    var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == typeStr); //在声明集合中获取ClaimTypes.NameIdentifier 的值就是用户ID
                    if (userCli == null || string.IsNullOrEmpty(userCli.Value))
                    {
                        return null;
                    }
                    return userCli.Value;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public enum cType
        {
            USERID = 0,
            USERNAME = 1
        }
    }
}
