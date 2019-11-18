using CMS_Application._TableDto;
using CMS_Application.Role;
using CMS_Infrastructure.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CMS_WEB.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRoleInfoService _roleInfoService;
        public TestController(IHttpContextAccessor httpContextAccessor, IHostingEnvironment env, IRoleInfoService roleInfoService)
        {
            _hostingEnvironment = env;
            _roleInfoService = roleInfoService;
        }
        /// <summary>
        /// 上传文件并返回链接
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file != null)
            {
                var fileDir = _hostingEnvironment.WebRootPath + $@"\Files";
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                //文件名称
                string projectFileName = file.FileName;

                //上传的文件的路径
                string filePath = fileDir + $@"\{projectFileName}";
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                var Result = $"{HttpContext.Request.Scheme}://{ HttpContext.Request.Host.Value}/src/{projectFileName}"; //构造返回路径     
                return Ok(Result);
            }
            else
            {
                return Ok("请选择文件.");
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        [Route("ExcelExport")]
        [HttpPost]
        public async Task<FileResult> ExcelExport()
        {
            var filename = "excel.xls";
            string[] listTitle = { "序号", "英文名", "中文名", "最后操作时间" };
            var list =await _roleInfoService.RoleInfoList(new TableInputDto());
            var stream = ExcelHelper.ListExportMemoryStream(list.TableData, listTitle);
            return File(stream, "application/vnd.ms-excel", filename);
        }
        public class Test
        {
            public int no { get; set; }
            public string roleNo { get; set; }
            public string roleName { get; set; }
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        [Route("ImportExcel")]
        [HttpPost]
        public IActionResult ImportExcel(IFormFile file)
        {
            MemoryStream ms = new MemoryStream();

            file.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var list = ExcelHelper.MemoryStreamImportToList<Test>(ms);

            return Ok(list);
        }
       
    }
}