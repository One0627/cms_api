using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace CMS_WEB.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost host = WebHost.CreateDefaultBuilder(args).UseUrls("http://*:5000").UseStartup<Startup>().Build();
            //bool isService = !(Debugger.IsAttached || args.Contains("--console")); //Debug状态或者程序参数中带有--console都表示普通运行方式而不是Windows服务运行方式
            //if (isService)
            //{
            //    host.RunAsService(); //部署windows服务
            //}
            //else
            //{
            //    host.Run();
            //}
            host.Run();
        }
    }
}
