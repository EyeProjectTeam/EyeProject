using System.Collections.Generic;

namespace Mutone.Core.Utils.Excel
{
    /// <summary>
    /// Worksheet数据
    /// </summary>
    public class ExcelWorksheetDto<T>
    {
        /// <summary>
        /// 工作表名称
        /// </summary>
        public string WorksheetName { get; set; }

        /// <summary>
        /// 数据集合开始填充的起始行号，从1开始,0标识自动计算（默认是从表头部分结束的下一行）
        /// </summary>
        public int StartRowIndex { get; set; }

        /// <summary>
        /// 序号行列号，等于0表示不需要行序号列
        /// </summary>
        public int SerialNumberColumnIndex { get; set; }

        /// <summary>
        /// 表头
        /// </summary>
        public List<ExcelHeadDto> Heads { get; set; }

        /// <summary>
        /// 是否根据ExcelExporter特性，添加默认表头，行号根据 StartRowIndex-1确定
        /// </summary>
        public bool IsUseDefaultHeads { get; set; } = true;

        #region 数据集 配置

        /// <summary>
        /// 数据集
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 合计行
        /// </summary>
        public T Total { get; set; }

        #endregion

        #region FreezePanes

        /// <summary>
        /// 冻结行号
        /// </summary>
        public int FreezeRow { get; set; }

        /// <summary>
        /// 冻结列号
        /// </summary>
        public int FreezeColumn { get; set; }

        #endregion

    }

}
