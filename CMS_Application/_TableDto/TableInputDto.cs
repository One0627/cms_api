using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS_Application._TableDto
{
    public class TableInputDto
    {
        public string QueryType { get; set; }
        public string QueryString { get; set; }
        public int currentPage { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
