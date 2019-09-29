using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CMS_Application.Authorization.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CMS_Infrastructure.Redis;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using CMS_Application.User.Dto;

namespace CMS_Application.Authorization
{
    public class JwtService : IJwtService
    {
        private readonly RedisCommon redis = RedisHelper.Default;
        private readonly string HashKey = "token:" + DateTime.Now.ToShortDateString();
        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly IConfiguration _configuration;
        
        /// <summary>
        /// 获取 HTTP 请求上下文
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public JwtService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string CreateToken(UserLoginOutPutDto dto)
        {
            var claims = new[]
             {
                   new Claim("UserId",dto.userId.ToString()),
                   new Claim("UserName",dto.userName)
             };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds));
            //redis.HashSet(HashKey, dto.userId, token);
            //redis.SetExpire(HashKey, TimeSpan.FromHours(8));
            return token;
        }

        public Task DeactivateCurrentTokenAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeactivateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsActiveTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsCurrentActiveTokenAsync()
        {
            var token = GetCurrentToken();
            var UserId = await GetCurrentUserId();
            var ActualToken = redis.HashGet(HashKey, UserId);
            return ActualToken == token ? true : false;
        }

        public Task<JwtResponseDto> RefreshTokenAsync(string token, UserLoginInputDto dto)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 获取 HTTP 请求的 Token 值
        /// </summary>
        /// <returns></returns>
        public string GetCurrentToken()
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
        public async Task<string> GetCurrentUserId()
        {
            var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
            if (auth.Succeeded)
            {
                var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == "UserId"); //在声明集合中获取ClaimTypes.NameIdentifier 的值就是用户ID
                if (userCli == null || string.IsNullOrEmpty(userCli.Value))
                {
                    return null;
                }
                return userCli.Value;
            }
            return null;
        }
    }
}
