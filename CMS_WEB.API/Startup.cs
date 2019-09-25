using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CMS_Application.Authorization;
using CMS_Entity.Models;
using CMS_WEB.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.SignalR;
using CMS_Application.Hubs;

namespace CMS_WEB.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Cors
            //跨域自定义url
            //services.AddMvcCore().AddAuthorization().AddJsonFormatters().AddApiExplorer();
            //var corsUrl = "http://localhost:8081";//Configuration[AppConsts.CorsOrigins].Split(','); 允许访问的地址 例："b.com","a.com"
            //services.AddCors(options => options.AddPolicy("AllowCors", p => p.WithOrigins(corsUrl).AllowAnyMethod().AllowAnyHeader()));

            //跨域 允许任意url
            services.AddCors(_options => _options.AddPolicy("AllowCors", _builder =>
            {
                _builder.AllowAnyOrigin()//.WithOrigins("http://192.168.8.105","http://localhost")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            }));
            #endregion

            #region Jwt Authentication
            // 添加jwt 验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,//是否验证Issuer
                         ValidateAudience = true,//是否验证Audience
                         ValidateLifetime = true,//是否验证失效时间
                         ValidateIssuerSigningKey = true,//是否验证SecurityKey
                         ValidAudience = Configuration["Jwt:Issuer"],//Audience   
                         ValidIssuer = Configuration["Jwt:Issuer"],//Issuer，这两项和前面签发jwt的设置一致
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))//拿到SecurityKey
                     };
                     options.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             var accessToken = context.Request.Query["access_token"];

                             // If the request is for our hub...
                             var path = context.HttpContext.Request.Path;
                             if (!string.IsNullOrEmpty(accessToken) &&
                                 (path.StartsWithSegments("/chathub")))
                             {
                                 // Read the token out of the query string
                                 context.Token = accessToken;
                             }
                             return Task.CompletedTask;
                         }
                     };
                 });
            #endregion

            #region MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc(options =>
            {
                options.Filters.Add<Filters.HttpGlobalExceptionFilter>();// 自定义全局捕获异常
            });
            //.AddJsonOptions(options =>
            // {
            //     //忽略循环引用
            //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //     //不使用驼峰样式的key
            //     options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            // })
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MyApi", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                //Add Jwt Authorize to http header
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//Jwt default param name
                    In = "header",//Jwt store address
                    Type = "apiKey"//Security scheme type
                });
                //Add authentication type
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
            #endregion

            #region 批量注入
            foreach (var item in GetClassName("CMS_Application"))
            {
                foreach (var typeArray in item.Value)
                {
                    services.AddScoped(typeArray, item.Key);
                }
            }
            #endregion

            #region Other
            services.AddDbContext<new_TTS_OrderContext>(options =>
            {
                options.UseSqlServer(Configuration["SqlServer:ConnectionString"], b => b.UseRowNumberForPaging());
                options.UseLazyLoadingProxies(false); // 延迟加载 false关闭
            });
            services.AddSignalR();
            services.AddHttpContextAccessor();
            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseCors("AllowCors");
            app.UseAuthentication();

            app.UseMiddleware<HttpContextMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsSystem API V1");
            });
            app.UseSignalR(route =>
            {
                route.MapHub<ChatHub>("/chathub");
            });
            //app.UseHttpsRedirection();
            app.UseMvc();

        }

        /// <summary>  
        /// 获取程序集中的实现类对应的多个接口
        /// </summary>  
        /// <param name="assemblyName">程序集</param>
        public Dictionary<Type, Type[]> GetClassName(string assemblyName)
        {
            if (!string.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();

                var result = new Dictionary<Type, Type[]>();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    result.Add(item, interfaceType);
                }
                return result;
            }
            return new Dictionary<Type, Type[]>();
        }
    }
}
