using System;
using System.Collections.Generic;
using System.Text;

namespace CMS_Application._TableDto
{
    public class TableOutputDto<T> where T:class
    {
        public int Total { get; set; }
        public List<T> TableData { get; set; }
    }
}
