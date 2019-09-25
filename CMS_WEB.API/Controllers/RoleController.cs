using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_Application._TableDto;
using CMS_Application.Role;
using CMS_Application.Role.Dto;
using CMS_WEB.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS_WEB.API.Controllers
{
    [ApiFilter]
    [Route("[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IRoleInfoService _roleInfoService;

        public RoleController(ILogger<UserController> logger, IRoleInfoService roleInfoService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _logger = logger;
            _roleInfoService = roleInfoService;
        }
        /// <summary>
        /// 角色
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("RoleSelect")]
        public IActionResult RoleSelect()
        {
            return Ok(_roleInfoService.RoleSelectDto());
        }
        /// <summary>
        /// 角色信息表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("RoleInfoList")]
        public IActionResult RoleInfolist(TableInputDto dto)
        {
            var res = _roleInfoService.RoleInfoList(dto);
            return Ok(res);
        }
        /// <summary>
        /// 修改角色权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("AddOrEditRolePermis")]
        public IActionResult AddOrEditRolePermis(RolePermisDto dto)
        {
            dto.updateBy = GetCurrentUserId().Result;
            var state = _roleInfoService.AddOrEditRolePermis(dto);
            var message = state ? "保存成功" : "保存失败";
            return Ok(new { state, message });
        }
    }
}