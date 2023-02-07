using System;

namespace Mutone.Core.Utils.Excel
{
    /// <summary>
    /// Excel导入特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelImporterAttribute : Attribute
    {
        /// <summary>
        /// EXCEL导入特性
        /// </summary>
        /// <param name="columnName">Excel列标题名，为空时根据字段名匹配（区分大小写）</param>
        public ExcelImporterAttribute(string columnName = null)
        {
            ColumnName = columnName;
        }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; }

    }
}
