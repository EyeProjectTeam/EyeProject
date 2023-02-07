using System.ComponentModel.DataAnnotations;
using OfficeOpenXml.Style;

namespace Mutone.Core.Utils.Excel
{
    /// <summary>
    /// 
    /// </summary>
    public class ExcelHeadDto
    {
        /// <summary>
        /// 表头名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表头所在行
        /// </summary>
        [Range(1, 10)]
        public int RowIndex { get; set; } = 1;

        /// <summary>
        /// 表头开始列
        /// </summary>
        public int ColumnIndex { get; set; } = 1;

        /// <summary>
        /// 要合并单元格 例入 new {2,3,2,4} 合并2行3列,2行4列
        /// </summary>
        public int[] SpanCells { get; set; }

        /// <summary>
        /// 是否启用数据筛选
        /// </summary>
        public bool AutoFilter { get; set; }

        /// <summary>
        /// 是否加粗
        /// </summary>
        public bool IsBold { get; set; } = true;

        /// <summary>
        /// 水平居中
        /// </summary>
        public ExcelHorizontalAlignment ExcelHorizontalAlignment { get; set; } = ExcelHorizontalAlignment.Center;

        /// <summary>
        /// 垂直居中
        /// </summary>
        public ExcelVerticalAlignment ExcelVerticalAlignment { get; set; } = ExcelVerticalAlignment.Center;

        /// <summary>
        /// 设置边框
        /// </summary>
        public ExcelBorderStyle ExcelBorderStyle { get; set; } = ExcelBorderStyle.Thin;

        /// <summary>
        /// 自动列宽最小值
        /// </summary>
        public int AutoFitMinWidth { get; set; }

        /// <summary>
        /// 自动列宽最小值
        /// </summary>
        public int AutoFitMaxWidth { get; set; }
    }
}
