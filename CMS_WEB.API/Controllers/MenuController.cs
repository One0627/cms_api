﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_Application.Menu;
using CMS_Application.Menu.Dto;
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
    public class MenuController : BaseController
    {
        private readonly ILogger<MenuController> _logger;
        private readonly IMenuService _menuService;

        public MenuController(ILogger<MenuController> logger, IMenuService menuService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _logger = logger;
            _menuService = menuService;
        }
        /// <summary>
        /// 导航菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        [Route("MenuTree")]
        public IActionResult MenuTree()
        {
            var res = _menuService.GetMenus();
            return Ok(res);
        }
        [HttpPost, Authorize]
        [Route("AddOrEditNode")]
        public IActionResult AddOrEditNode(MenuDto dto)
        {
            dto.updateBy = GetCurrentUserId().Result;
            var state = _menuService.AddOrEditMenu(dto);
            var message = state ? "保存成功" : "保存失败";
            return Ok(new { state, message });
        }
        [HttpPost, Authorize]
        [Route("DeleteNode")]
        public IActionResult DeleteNode(MenuDto dto)
        {
            var state = _menuService.DeleteNode(dto.id,GetCurrentUserId().Result);
            var message = state ? "删除成功" : "删除失败";
            return Ok(new { state, message });
        }
        [HttpPost, Authorize]
        [Route("UpNode")]
        public IActionResult UpNode(MenuDto dto)
        {
            var state = _menuService.UpNode(dto);
            var message = state ? "成功" : "失败";
            return Ok(new { state, message });
        }
        [HttpPost, Authorize]
        [Route("DownNode")]
        public IActionResult DownNode(MenuDto dto)
        {
            var state = _menuService.DownNode(dto);
            var message = state ? "成功" : "失败";
            return Ok(new { state, message });
        }
    }
}