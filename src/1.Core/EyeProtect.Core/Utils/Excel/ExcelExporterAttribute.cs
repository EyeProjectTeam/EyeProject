using System;

namespace Mutone.Core.Utils.Excel
{
    /// <summary>
    /// Excel导出特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelExporterAttribute : Attribute
    {
        /// <summary>
        /// Excel导出特性
        /// </summary>
        /// <param name="columnIndex">列号，从1开始</param>
        /// <param name="columnName"></param>
        /// <param name="width">列宽设置，0表示自动</param>
        public ExcelExporterAttribute(int columnIndex,string columnName,int width = 0)
        {
            ColumnIndex = columnIndex;
            ColumnName = columnName;
            Width = width;
        }

        /// <summary>
        /// 列名，从1开始
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width { get; set; }
    }
}
