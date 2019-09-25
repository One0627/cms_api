using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application.Menu.Dto
{
    public class MenuDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public int menuNo { get; set; }
        public string url { get; set; }
        public string icon { get; set; }
        public int? parentId { get; set; }
        public string updateBy { get; set; }
        public List<MenuDto> children { get; set; }
    }
}
