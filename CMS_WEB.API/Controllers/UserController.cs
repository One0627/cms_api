using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CMS_Application;
using CMS_Application._TableDto;
using CMS_Application.Authorization;
using CMS_Application.User.Dto;
using CMS_Application.User;
using CMS_Application.User.UserInfo;
using CMS_Application.User.UserInfo.Dto;
using CMS_WEB.API.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using CMS_Application.Hubs;
using CMS_Infrastructure.Helper;

namespace CMS_WEB.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IJwtService _jwtService;
        private readonly IUserInfoService _userInfoService;
        private readonly IHubContext<ChatHub> _hubContext;

        public UserController(ILogger<UserController> logger, IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IUserInfoService userInfoService, IHubContext<ChatHub> hubContext) 
        {
            _logger = logger;
            _jwtService = jwtService;
            _userInfoService = userInfoService;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 当前token是否活跃
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetAsync()
        {
            var res = await _jwtService.IsCurrentActiveTokenAsync();
            if (res)
                return Ok();
            return Forbid();
        }
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize,SkipFilter]
        [Route("CurrentUserInfo")]
        public async Task<IActionResult> CurrentUserInfo()
        {
            var UserId = await GetCurrentUserId();
            var userInfo = await _userInfoService.GetUserInfoBy(int.Parse(UserId));
            return Ok(userInfo);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [SkipFilter]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginInputDto user)
        {
            user.passWord = user.passWord.Length < 16 ?MD5Helper.MD5Encrypt32(user.passWord) : user.passWord;
            var dto = await _userInfoService.Login(user);
            if (dto != null)
            {
                var token = _jwtService.CreateToken(dto);
                return Ok(new { result = true, token, message = "恭喜你，登录成功" });
            }
            return Ok(new { result = false, message = "账户密码错误或状态被禁用" });
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [SkipFilter]
        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            var result = true;
            return Ok(result);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [Route("ChangePwd")]
        [HttpPost]
        public IActionResult ChangePwd(ChangePwdDto dto)
        {
            var res = _userInfoService.ChangePwd(dto);
            return Ok(res);
        }
        /// <summary>
        /// 用户信息表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost,Authorize, SkipFilter]
        [Route("UserInfoList")]
        public async Task<IActionResult> UserInfoList(TableInputDto dto)
        {
            var res = await _userInfoService.UserInfoList(dto);
            return Ok(res);
        }
        /// <summary>
        /// 添加和更改用户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [Route("AddOrEditUser")]
        [HttpPost]
        public IActionResult AddOrEditUser(UserInfoListDto dto)
        {
            dto.updateBy = GetCurrentUserId().Result;
            dto.passWord = dto.passWord.Length < 16 ? MD5Helper.MD5Encrypt32(dto.passWord) : dto.passWord;
            var state = _userInfoService.AddOrEditUserInfo(dto);
            var message = state ? "保存成功" : "账号已存在";
            _hubContext.Clients.All.SendAsync("ReceiveUpdate").Wait();
            return Ok(new { state, message });
        }
        /// <summary>
        /// 删除用户（伪删除）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [Route("DeleteUser")]
        [HttpPost]
        public IActionResult DeleteUser(UserInfoListDto dto)
        {
            var state = _userInfoService.DeleteUser(dto.userId,GetCurrentUserId().Result);
            var message = state ? "删除成功" : "删除失败";
            return Ok(new { state, message });
        }
    }
}
