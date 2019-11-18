using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace CMS_Infrastructure.Helper
{
    public class ExcelHelper
    {
        /// <summary>
        /// List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据集</param>
        /// <param name="listTitle">标题集合</param>
        /// <returns></returns>
        public static MemoryStream ListExportMemoryStream<T>(List<T> data, string[] listTitle) where T : class
        {
            IWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet("Sheet1");
            IRow rowTitle = sheet.CreateRow(0);
            ICellStyle style = book.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;//垂直居中  

            Type entityType = data[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();


            for (int i = 0; i < listTitle.Length; i++)
            {
                rowTitle.CreateCell(i).SetCellValue(listTitle[i]);
            }
            if (data != null)
            {
                int start = 0;//记录同组开始行号
                int end = 0;//记录同组结束行号
                string temp = "";//记录上一行的值
                for (int i = 0; i < data.Count; i++)
                {

                    IRow row = sheet.CreateRow(i + 1);

                    object entity = data[i];
                    for (int j = 0; j < listTitle.Length; j++)
                    {
                        object[] entityValues = new object[entityProperties.Length];
                        entityValues[j] = entityProperties[j].GetValue(entity);
                        row.CreateCell(j).SetCellValue(entityValues[j]?.ToString());
                    }

                    row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellType(CellType.String);
                    var cellText = row.Cells[0].StringCellValue;//获取当前行 第1列的单元格的值

                    if (cellText == temp)//上下行相等，记录要合并的最后一行
                    {
                        end = i;
                    }
                    else//上下行不等，记录
                    {
                        if (start != end)
                        {
                            //设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                            //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                            CellRangeAddress region = new CellRangeAddress(start + 1, end + 1, 0, 0);
                            sheet.AddMergedRegion(region);
                        }
                        start = i;
                        end = i;
                        temp = cellText;
                    }
                }
            }
            for (int i = 0; i < listTitle.Length; i++)
            {
                sheet.AutoSizeColumn(i);//i：根据标题的个数设置自动列宽
            }

            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }

        public static List<T> MemoryStreamImportToList<T>(MemoryStream ms) where T : class, new()
        {
            List<T> list = new List<T> { };
            IWorkbook workbook;
            try
            {
                workbook = new XSSFWorkbook(ms);
            }
            catch (Exception)
            {
                workbook = new HSSFWorkbook(ms);
            }


            ISheet sheet = workbook.GetSheetAt(0);
            IRow cellNum = sheet.GetRow(0);
            var propertys = typeof(T).GetProperties();
            string value = null;
            int num = cellNum.LastCellNum;

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                var obj = new T();
                for (int j = 0; j < propertys.Length; j++)
                {
                    value = row.GetCell(j).ToString();
                    string str = (propertys[j].PropertyType).FullName;
                    if (str == "System.String")
                    {
                        propertys[j].SetValue(obj, value, null);
                    }
                    else if (str == "System.DateTime")
                    {
                        DateTime pdt = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                        propertys[j].SetValue(obj, pdt, null);
                    }
                    else if (str == "System.Boolean")
                    {
                        bool pb = Convert.ToBoolean(value);
                        propertys[j].SetValue(obj, pb, null);
                    }
                    else if (str == "System.Int16")
                    {
                        short pi16 = Convert.ToInt16(value);
                        propertys[j].SetValue(obj, pi16, null);
                    }
                    else if (str == "System.Int32")
                    {
                        int pi32 = Convert.ToInt32(value);
                        propertys[j].SetValue(obj, pi32, null);
                    }
                    else if (str == "System.Int64")
                    {
                        long pi64 = Convert.ToInt64(value);
                        propertys[j].SetValue(obj, pi64, null);
                    }
                    else if (str == "System.Byte")
                    {
                        byte pb = Convert.ToByte(value);
                        propertys[j].SetValue(obj, pb, null);
                    }
                    else
                    {
                        propertys[j].SetValue(obj, null, null);
                    }
                }

                list.Add(obj);
            }

            return list;
        }
    }
}
