using System.Collections.Generic;
using System.Linq;

namespace Mutone.Core.Utils.Excel
{
    /// <summary>
    /// 
    /// </summary>
    public class ExcelResult<TDto>
    {
        public ExcelResult()
        {
            Data = new List<TDto>();
            ErrorList = new List<ExcelErrorItem>();
        }

        /// <summary>
        /// 错误列表
        /// </summary>
        public List<ExcelErrorItem> ErrorList { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<TDto> Data { get; set; }

        /// <summary>
        /// Excel有内容且无错误
        /// </summary>
        /// <returns></returns>
        public bool IsOk()
        {
            return Data.Any() && !ErrorList.Any();
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrorMessage()
        {
            var list = new List<string>();
            if (!Data.Any())
            {
                list.Add("Excel无数据");
            }

            if (ErrorList?.Any() == true)
            {
                foreach (var item in ErrorList)
                {
                    list.Add($"Sheet{item.SheetIndex + 1}第{item.Row}行第{item.Column}列 {item.Message}");
                }
            }

            return string.Join("；", list);
        }
    }

    public class ExcelErrorItem
    {
        /// <summary>
        /// 工作表索引，从0开始
        /// </summary>
        public int SheetIndex { get; set; }

        /// <summary>
        /// 行号 从1开始
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 列号 从1开始
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}
