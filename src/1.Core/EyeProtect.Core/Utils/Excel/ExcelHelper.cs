//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using EyeProtect.Core.Utils;
using Mutone.Core.Utils.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Mutone.Core.Utils
{
    /// <summary>
    /// Excel导入导出
    /// </summary>
    public static class ExcelHelper
    {

        #region 导入：读取Excel数据到List<T>

        /*
         * 导入：
         * 1.根据ExcelImporterAttribute中的列名（为空时，根据字段名称匹配，区分大小写）匹配excel中的标题
         * 2.需要指定标题行，默认标题行下一行就是 数据起始行，行号都从1开始
         */


        /// <summary>
        /// 导入：读取Excel数据到List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelPath">导入文件路径</param>
        /// <param name="titleRowIndex">标题行索引，从1开始</param>
        /// <param name="dataStartRowIndex">数据行索引，从1开始</param>
        /// <param name="sheetIndex">工作表索引，从1开始</param>
        /// <returns></returns>
        public static ExcelResult<T> Read<T>(string excelPath, int titleRowIndex, int dataStartRowIndex = 0, int sheetIndex = 0) where T : class, new()
        {
            using var pagePackage = new ExcelPackage(new FileInfo(excelPath));
            return ReadExcel<T>(pagePackage, titleRowIndex, dataStartRowIndex, sheetIndex);
        }

        /// <summary>
        /// 导入：读取Excel数据到List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">导入文件路径</param>
        /// <param name="titleRowIndex">标题行索引，从1开始</param>
        /// <param name="dataStartRowIndex">数据行索引，从1开始</param>
        /// <param name="sheetIndex">工作表索引，从1开始</param>
        /// <returns></returns>
        public static ExcelResult<T> Read<T>(Stream stream, int titleRowIndex, int dataStartRowIndex = 0, int sheetIndex = 0) where T : class, new()
        {
            using var pagePackage = new ExcelPackage(stream);
            //5.x版本 许可证，必须添加许可证，否则会报错
            //ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            return ReadExcel<T>(pagePackage, titleRowIndex, dataStartRowIndex, sheetIndex);
        }

        #endregion


        #region 导出：生成单sheet 的 Excel并填充数据

        /// <summary>
        /// 生成单sheet 的 Excel并填充数据
        /// </summary>
        public static byte[] WriteExcel<T>(ExcelWorksheetDto<T> sheetData, int sheetIndex = 0) where T : class
        {
            using var package = new ExcelPackage();
            package.WriteSheet(sheetData, sheetIndex);
            return package.GetAsByteArray();
        }

        #endregion

        #region 导出：生成多sheet 的 Excel并填充数据

        /// <summary>
        /// 生成多sheet 的 Excel并填充数据
        /// </summary>
        public static byte[] WriteExcel<T>(List<ExcelWorksheetDto<T>> sheetDataList, int startSheetIndex = 0) where T : class
        {
            using var package = new ExcelPackage();
            for (var i = 0; i < sheetDataList.Count; i++)
            {
                package.WriteSheet(sheetDataList[i], startSheetIndex + i);
            }

            return package.GetAsByteArray();
        }

        #endregion

        #region 导出：ExcelPackage数据填充

        /// <summary>
        /// 填充sheet数据
        /// </summary>
        /// <typeparam name="T">数据集合类型</typeparam>
        /// <param name="excel">excel</param>
        /// <param name="sheetData">填充工作的数据</param>
        /// <param name="sheetIndex">写入第n个工作表，从0开始</param>
        public static void WriteSheet<T>(this ExcelPackage excel, ExcelWorksheetDto<T> sheetData, int sheetIndex) where T : class
        {
            //var worksheet = excel.Workbook.Worksheets.Add(item.WorksheetName);
            ExcelWorksheet worksheet;
            if (excel.Workbook.Worksheets.Count < sheetIndex + 1)
            {
                worksheet = excel.Workbook.Worksheets.Add(sheetData.WorksheetName);
            }
            else
            {
                worksheet = excel.Workbook.Worksheets[sheetIndex];
                worksheet.Name = sheetData.WorksheetName;
            }

            var map = new Dictionary<string, ExcelDtoMemberInfo>(); //字段名：info
            #region 获取需要导入的字段

            var properties = typeof(T).GetProperties();
            foreach (var info in properties)
            {
                var attr = info.GetCustomAttribute<ExcelExporterAttribute>();
                if (attr != null)
                {
                    map.Add(info.Name, new ExcelDtoMemberInfo()
                    {
                        PropertyInfo = info,
                        ColumnIndex = attr.ColumnIndex,
                        TitleName = attr.ColumnName ?? info.Name,
                        ColumnWidth = attr.Width,
                    });
                }
            }

            #endregion

            GenerateSheet(sheetData, worksheet, map);

        }

        /// <summary>
        /// 组织表格数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetData"></param>
        /// <param name="worksheet"></param>
        /// <param name="map"></param>
        private static void GenerateSheet<T>(ExcelWorksheetDto<T> sheetData, ExcelWorksheet worksheet, Dictionary<string, ExcelDtoMemberInfo> map) where T : class
        {
            #region 自动计算数据起始行

            if (sheetData.StartRowIndex == 0)
            {
                var row = 1;
                if (sheetData.Heads?.Any() == true)
                    row = sheetData.Heads.Select(x => x.SpanCells[2]).Max() + 1;

                if (sheetData.IsUseDefaultHeads)
                    row += 1;
                sheetData.StartRowIndex = row;
            }
            #endregion

            //设置自定义
            SetHeader(worksheet, sheetData.Heads);

            //填充默认表头
            if (sheetData.IsUseDefaultHeads)
            {
                SetDefaultHeader(worksheet, map, sheetData.StartRowIndex - 1);
                // 设置序号
                if (sheetData.SerialNumberColumnIndex > 0)
                {
                    var cells = worksheet.Cells[sheetData.StartRowIndex - 1, sheetData.SerialNumberColumnIndex];
                    SetNormalHeaderStyle(cells, "序号");
                }
            }

            //填充数据
            SetRowsList(worksheet, sheetData.Data, map, sheetData.StartRowIndex, sheetData.SerialNumberColumnIndex, sheetData.Total);

            //冻结
            if (sheetData.FreezeRow > 0 || sheetData.FreezeColumn > 0)
            {
                worksheet.View.FreezePanes(sheetData.FreezeRow + 1, sheetData.FreezeColumn + 1);
            }

            //自动列宽度
            foreach (var item in map)
            {
                if (item.Value.ColumnWidth > 0)
                {
                    worksheet.Column(item.Value.ColumnIndex).Width = item.Value.ColumnWidth;
                }
                else
                {
                    worksheet.Column(item.Value.ColumnIndex).AutoFit();
                }
            }
        }

        #endregion


        #region 私有方法

        #region Excel数据读取

        /// <summary>
        /// 读取Excel数据
        /// </summary>
        private static ExcelResult<T> ReadExcel<T>(ExcelPackage excelPackage, int titleRowIndex, int dataStartRowIndex = 0, int sheetIndex = 0) where T : class, new()
        {
            var result = new ExcelResult<T>();
            if (dataStartRowIndex == 0)
            {
                dataStartRowIndex = titleRowIndex + 1;
            }

            var worksheet = excelPackage.Workbook.Worksheets[sheetIndex];
            var endRowIndex = worksheet.Dimension.End.Row;
            var endColIndex = worksheet.Dimension.End.Column;

            var excelDtoMap = new Dictionary<string, ExcelDtoMemberInfo>(); //字段名：info
            #region 获取需要导入的字段

            //var accessor = TypeAccessor.Create(typeof(T));
            //var members = accessor.GetMembers();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<ExcelImporterAttribute>();
                if (attr != null)
                {
                    excelDtoMap.Add(prop.Name, new ExcelDtoMemberInfo()
                    {
                        PropertyInfo = prop,
                        TitleName = attr.ColumnName ?? prop.Name,
                    });
                }
            }

            #endregion

            #region 获取列号及对应的标题
            //获取列号及对应的标题
            for (var col = 1; col <= endColIndex; col++)
            {
                var title = worksheet.Cells[titleRowIndex, col]?.Value?.ToString()?.Trim();
                if (!title.IsNullOrWhiteSpace())
                {
                    if (excelDtoMap.Any(x => x.Value.TitleName == title))
                    {
                        var map = excelDtoMap.FirstOrDefault(x => x.Value.TitleName == title);
                        map.Value.ColumnIndex = col;
                    }
                    //else
                    //{
                    //    AddReadErrorInfo(result, titleRowIndex, col, $"【{title}】列不是需要导入的列。");
                    //}
                }
            }

            #endregion

            //excel表头不符合要求，缺少列
            foreach (var column in excelDtoMap)
            {
                var columnIndex = column.Value.ColumnIndex;
                var columnTitle = column.Value.TitleName;
                if (columnIndex == 0)
                {
                    AddReadErrorInfo(result, titleRowIndex, columnIndex, $"表格中不存在【{columnTitle}】列。");
                    return result;
                }
            }

            for (var rowIndex = dataStartRowIndex; rowIndex <= endRowIndex; rowIndex++)
            {
                var t = new T();

                foreach (var column in excelDtoMap)
                {
                    var columnIndex = column.Value.ColumnIndex;
                    var columnTitle = column.Value.TitleName;

                    PropertyInfo member = column.Value.PropertyInfo;
                    var propertyType = member.PropertyType;

                    var cell = worksheet.Cells[rowIndex, columnIndex];
                    var cellValue = cell.Value?.ToString();
                    try
                    {
                        #region 枚举类型赋值
                        var isEnum = propertyType.IsEnum;
                        var underlyingType = Nullable.GetUnderlyingType(propertyType);
                        var isNullable = underlyingType != null;
                        if (isNullable)
                        {
                            isEnum = underlyingType.IsEnum;
                        }

                        if (!string.IsNullOrWhiteSpace(cellValue) && isEnum)
                        {
                            var enumDisplayNameValues = isNullable ? underlyingType.GetEnumDisplayNameValues() : propertyType.GetEnumDisplayNameValues();

                            if (enumDisplayNameValues.TryGetValue(cellValue, out int enumValue))
                            {
                                var v = isNullable ? Enum.ToObject(underlyingType, enumValue) : enumValue;
                                member.SetValue(t, v);
                                //accessor[t, member.Name] = isNullable ? Enum.ToObject(underlyingType, enumValue) : enumValue;
                                continue;
                            }
                        }

                        if (isEnum && !isNullable)
                        {
                            AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，不存在的选项值。");
                            continue;
                        }
                        #endregion

                        // 可空类型空值赋值
                        if (string.IsNullOrWhiteSpace(cellValue) && isNullable)
                        {
                            member.SetValue(t, null);
                            continue;
                        }

                        #region 其它类型赋值

                        var typeName = isNullable ? underlyingType.Name.ToLower() : propertyType.Name.ToLower();

                        switch (typeName)
                        {
                            case "string":
                                member.SetValue(t, cellValue?.Trim());
                                break;
                            case "byte":
                                {
                                    if (!byte.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的整型数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "int16":
                                {
                                    if (!short.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的整型数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "int32":
                                {
                                    if (!int.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的整型数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "int64":
                                {
                                    if (!long.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的整型数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "decimal":
                                {
                                    if (!decimal.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的小数数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "double":
                                {
                                    if (!double.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的小数数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "single":
                                {
                                    if (!float.TryParse(cellValue, out var value))
                                    {
                                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的小数数值。");
                                        break;
                                    }
                                    member.SetValue(t, value);
                                }
                                break;
                            case "datetime":
                                {
                                    if (DateTime.TryParse(cellValue, out var value1))
                                    {
                                        member.SetValue(t, value1);
                                        break;
                                    }

                                    if (double.TryParse(cellValue, out var value2))
                                    {
                                        member.SetValue(t, DateTime.FromOADate(value2));
                                        break;
                                    }

                                    AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值无效，请填写正确的日期。");
                                }
                                break;
                            case "boolean":
                                {
                                    switch (cellValue)
                                    {
                                        case "是":
                                            member.SetValue(t, true);
                                            break;
                                        case "否":
                                            member.SetValue(t, false);
                                            break;
                                        default:
                                            AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{cellValue} 值不合法。");
                                            break;
                                    }
                                }
                                break;

                            default:
                                member.SetValue(t, cellValue);
                                break;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{ex.Message}");
                    }
                }

                // 校验值的合法性
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(t, new ValidationContext(t), validationResults, true);
                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        var memberName = validationResult.MemberNames.First();
                        if (excelDtoMap.TryGetValue(memberName, out var v))
                        {
                            AddReadErrorInfo(result, rowIndex, v.ColumnIndex, $"【{v.TitleName}】{validationResult.ErrorMessage}");
                        }
                    }
                }

                result.Data.Add(t);
            }

            return result;
        }


        #endregion

        #region 添加错误信息记录

        /// <summary>
        /// 添加错误信息记录
        /// </summary>
        private static void AddReadErrorInfo<T>(ExcelResult<T> result, int rowIndex, int colIndex, string error, int sheetIndex = 0)
        {
            if (error.IsNullOrWhiteSpace())
            {
                return;
            }

            var errorInfo = result.ErrorList.FirstOrDefault(p => sheetIndex == p.SheetIndex && p.Row == rowIndex);
            if (errorInfo == null)
            {
                result.ErrorList.Add(new ExcelErrorItem() { Row = rowIndex, Column = colIndex, SheetIndex = sheetIndex, Message = error });
            }
            else
            {
                errorInfo.Message = $"{errorInfo.Message},{error}";
            }
        }

        #endregion


        #region 私有方法： 导出=》ExcelPackage数据填充

        /// <summary>
        /// 填充指定Cell
        /// </summary>
        private static void SetHeader(ExcelWorksheet worksheet, List<ExcelHeadDto> headCells)
        {
            if (headCells?.Any() == true)
            {
                #region 设置表头

                var headIndex = 0; //如果columnIndex=0,则获取表头数组索引
                foreach (var head in headCells)
                {
                    #region 设置默认列号
                    headIndex++;
                    if (head.ColumnIndex == 0)
                    {
                        head.ColumnIndex = head.SpanCells == null ? headIndex : head.SpanCells[1];
                    }
                    #endregion

                    if (head.SpanCells == null)
                    {
                        head.SpanCells = new int[] { head.RowIndex, head.ColumnIndex, head.RowIndex, head.ColumnIndex };
                    }
                    var cells = worksheet.Cells[head.SpanCells[0], head.SpanCells[1], head.SpanCells[2], head.SpanCells[3]];
                    cells.Merge = true;
                    cells.Style.Font.Bold = head.IsBold; //加粗
                    cells.Style.HorizontalAlignment = head.ExcelHorizontalAlignment;//水平居中
                    cells.Style.VerticalAlignment = head.ExcelVerticalAlignment;//垂直居中
                    cells.Style.Border.BorderAround(head.ExcelBorderStyle); //设置边框
                    cells.Value = head.Name;
                    cells.AutoFilter = head.AutoFilter;
                    if (head.AutoFitMinWidth > 0 && head.AutoFitMaxWidth > 0)
                    {
                        cells.AutoFitColumns(head.AutoFitMinWidth, head.AutoFitMaxWidth);
                    }

                }

                #endregion
            }
        }

        /// <summary>
        /// 根据ExcelExporter特性，添加默认表头
        /// </summary>
        private static void SetDefaultHeader(ExcelWorksheet worksheet, Dictionary<string, ExcelDtoMemberInfo> map, int row)
        {
            foreach (var item in map)
            {
                var cells = worksheet.Cells[row, item.Value.ColumnIndex];
                SetNormalHeaderStyle(cells, item.Value.TitleName);
            }
        }

        /// <summary>
        /// 填充数据集
        /// </summary>
        private static void SetRowsList<T>(ExcelWorksheet worksheet, IEnumerable<T> data, Dictionary<string, ExcelDtoMemberInfo> map, int dataStartRowIndex, int serialNumberColumnIndex
            , T totalData = null)
            where T : class
        {
            if (data == null || !data.Any())
            {
                return;
            }

            var i = 0;
            var dataLength = data.Count();//数据总行数
            foreach (var foo in data)
            {
                SetExcelRowValue(worksheet, foo, map, dataStartRowIndex + i);

                // 设置序号
                if (serialNumberColumnIndex > 0)
                {
                    var cells = worksheet.Cells[dataStartRowIndex + i, serialNumberColumnIndex];
                    SetNormalCellStyle(cells);
                    cells.Value = i + 1;
                }

                i++;
            }

            if (totalData != null)
            {
                worksheet.Cells[dataStartRowIndex + dataLength, 1].Value = "合计";
                worksheet.Cells[dataStartRowIndex + dataLength, 1].Style.Font.Bold = true;
                worksheet.Cells[dataStartRowIndex + dataLength, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[dataStartRowIndex + dataLength, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                SetExcelRowValue(worksheet, totalData, map, dataStartRowIndex + dataLength);
            }

        }


        #endregion

        #region 写入一行数据

        private static void SetExcelRowValue<T>(ExcelWorksheet worksheet, T item,
            Dictionary<string, ExcelDtoMemberInfo> map, int rowIndex)
        {
            foreach (var col in map)
            {
                var propertyType = col.Value.PropertyInfo.PropertyType;
                var value = col.Value.PropertyInfo.GetValue(item);

                var cells = worksheet.Cells[rowIndex, col.Value.ColumnIndex];
                SetNormalCellStyle(cells);

                #region 处理枚举类型
                var isEnum = propertyType.IsEnum;
                var underlyingType = Nullable.GetUnderlyingType(propertyType);
                var isNullable = underlyingType != null;
                if (isNullable)
                {
                    isEnum = underlyingType.IsEnum;
                }

                if (value != null && isEnum)
                {
                    var enumDisplayNameValues = isNullable ? underlyingType.GetEnumDisplayNameValues() : propertyType.GetEnumDisplayNameValues();
                    var enumDisplayNameValue = enumDisplayNameValues.FirstOrDefault(p => p.Value == (int)value);
                    cells.Value = enumDisplayNameValue.Key;
                    continue;
                }
                #endregion

                #region 处理布尔类型
                var typeName = isNullable ? underlyingType.Name.ToLower() : propertyType.Name.ToLower();
                if (value != null && typeName == "boolean")
                {
                    cells.Value = (bool)value ? "是" : "否";
                    continue;
                }
                #endregion


                var displayFormatAttr = col.Value.PropertyInfo.GetCustomAttribute<DisplayFormatAttribute>();
                string formatString = null;
                if (displayFormatAttr != null)
                {
                    formatString = displayFormatAttr.DataFormatString;
                }

                #region 处理日期类型

                if ("datetime" == typeName)
                {
                    if (value != null)
                    {
                        var d = Convert.ToDateTime(value);
                        cells.Value = !string.IsNullOrWhiteSpace(formatString) ? d.ToString(formatString) : d.ToDateTimeString();
                    }
                    continue;
                }

                #endregion

                #region 默认处理

                //if (!string.IsNullOrWhiteSpace(formatString) && value != null)
                //{
                //    cells.Style.Numberformat.Format = formatString;
                //}
                cells.Value = value;

                #endregion

            }
        }

        /// <summary>
        /// 设置正常单元格样式
        /// </summary>
        /// <param name="cells"></param>
        private static void SetNormalCellStyle(ExcelRange cells)
        {
            cells.Style.Font.Bold = false; //加粗         
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
            cells.Style.Border.BorderAround(ExcelBorderStyle.Thin); //设置边框

        }

        private static void SetNormalHeaderStyle(ExcelRange cells, string title)
        {
            cells.Merge = true;
            cells.Style.Font.Bold = true; //加粗
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
            cells.Style.Border.BorderAround(ExcelBorderStyle.Thin); //设置边框
            cells.Value = title;
        }

        #endregion

        #endregion

        #region 动态表头Excel操作

        #region 导出动态列数据到Excel
        public static byte[] WriteDynamicSheet(ExcelWorksheetDto<DynamicRow> sheetData, int sheetIndex = 0)
        {
            using var excel = new ExcelPackage();
            ExcelWorksheet worksheet;
            if (excel.Workbook.Worksheets.Count < sheetIndex + 1)
            {
                worksheet = excel.Workbook.Worksheets.Add(sheetData.WorksheetName);
            }
            else
            {
                worksheet = excel.Workbook.Worksheets[sheetIndex];
                worksheet.Name = sheetData.WorksheetName;
            }

            var map = new Dictionary<string, ExcelDtoMemberInfo>(); //字段名：info
            #region 获取需要导入的字段

            var headerRow = sheetData.Data.FirstOrDefault();
            if (headerRow == null)
            {
                throw new ArgumentException("动态表头需至少包含一行数据");
            }
            if (headerRow.DynamicColumns == null || !headerRow.DynamicColumns.Any())
            {
                throw new ArgumentException("动态表头需至少包含一列数据");
            }
            var colIndex = 1;
            foreach (var info in headerRow.DynamicColumns)
            {
                map.Add(info.Name, new ExcelDtoMemberInfo()
                {
                    PropertyInfo = null,
                    ColumnIndex = colIndex++,
                    TitleName = info.Name
                });
            }

            #endregion

            GenerateDynamicSheet(sheetData, worksheet, map);
            return excel.GetAsByteArray();

        }
        /// <summary>
        /// 组织表格数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetData"></param>
        /// <param name="worksheet"></param>
        /// <param name="map"></param>
        private static void GenerateDynamicSheet(ExcelWorksheetDto<DynamicRow> sheetData, ExcelWorksheet worksheet, Dictionary<string, ExcelDtoMemberInfo> map)
        {
            #region 自动计算数据起始行

            if (sheetData.StartRowIndex == 0)
            {
                var row = 1;
                if (sheetData.Heads?.Any() == true)
                    row = sheetData.Heads.Select(x => x.SpanCells[2]).Max() + 1;

                if (sheetData.IsUseDefaultHeads)
                    row += 1;
                sheetData.StartRowIndex = row;
            }
            #endregion

            //设置自定义
            SetHeader(worksheet, sheetData.Heads);

            //填充默认表头
            if (sheetData.IsUseDefaultHeads)
            {
                SetDefaultHeader(worksheet, map, sheetData.StartRowIndex - 1);
                // 设置序号
                if (sheetData.SerialNumberColumnIndex > 0)
                {
                    var cells = worksheet.Cells[sheetData.StartRowIndex - 1, sheetData.SerialNumberColumnIndex];
                    SetNormalHeaderStyle(cells, "序号");
                }
            }

            //填充数据
            SetDynamicRowsList(worksheet, sheetData.Data, map, sheetData.StartRowIndex, sheetData.SerialNumberColumnIndex);

            //冻结
            if (sheetData.FreezeRow > 0 || sheetData.FreezeColumn > 0)
            {
                worksheet.View.FreezePanes(sheetData.FreezeRow + 1, sheetData.FreezeColumn + 1);
            }

            //自动列宽度
            foreach (var item in map)
            {
                if (item.Value.ColumnWidth > 0)
                {
                    worksheet.Column(item.Value.ColumnIndex).Width = item.Value.ColumnWidth;
                }
                else
                {
                    worksheet.Column(item.Value.ColumnIndex).AutoFit();
                }
            }
        }

        /// <summary>
        /// 填充数据集
        /// </summary>
        private static void SetDynamicRowsList(ExcelWorksheet worksheet, IEnumerable<DynamicRow> data, Dictionary<string, ExcelDtoMemberInfo> map, int dataStartRowIndex, int serialNumberColumnIndex)
        {
            if (data == null || !data.Any())
            {
                return;
            }

            var i = 0;
            var dataLength = data.Count();//数据总行数
            foreach (var foo in data)
            {
                SetExcelDynamicRowValue(worksheet, foo, map, dataStartRowIndex + i);

                // 设置序号
                if (serialNumberColumnIndex > 0)
                {
                    var cells = worksheet.Cells[dataStartRowIndex + i, serialNumberColumnIndex];
                    SetNormalCellStyle(cells);
                    cells.Value = i + 1;
                }

                i++;
            }
        }

        /// <summary>
        /// 填充一行数据
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="item"></param>
        /// <param name="map"></param>
        /// <param name="rowIndex"></param>
        private static void SetExcelDynamicRowValue(ExcelWorksheet worksheet, DynamicRow item,
            Dictionary<string, ExcelDtoMemberInfo> map, int rowIndex)
        {
            foreach (var col in map)
            {

                var value = item.DynamicColumns.FirstOrDefault(a => a.Name == col.Value.TitleName).Value;
                var cells = worksheet.Cells[rowIndex, col.Value.ColumnIndex];
                SetNormalCellStyle(cells);
                cells.Value = value;
            }
        }
        #endregion

        /// <summary>
        /// 导入：读取Excel数据到List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">导入文件路径</param>
        /// <param name="titleRowIndex">标题行索引，从1开始</param>
        /// <param name="dataStartRowIndex">数据行索引，从1开始</param>
        /// <param name="sheetIndex">工作表索引，从1开始</param>
        /// <returns></returns>
        public static ExcelResult<DynamicRow> ReadDynamicExcel(Stream stream, int titleRowIndex, int dataStartRowIndex = 0, int sheetIndex = 0)
        {
            using var excelPackage = new ExcelPackage(stream);
            var result = new ExcelResult<DynamicRow>();
            if (dataStartRowIndex == 0)
            {
                dataStartRowIndex = titleRowIndex + 1;
            }

            var worksheet = excelPackage.Workbook.Worksheets[sheetIndex];
            var endRowIndex = worksheet.Dimension.End.Row;
            var endColIndex = worksheet.Dimension.End.Column;

            var excelDtoMap = new Dictionary<string, ExcelDtoMemberInfo>(); 
            #region 获取列号及对应的标题
            //获取列号及对应的标题
            for (var col = 1; col <= endColIndex; col++)
            {
                var title = worksheet.Cells[titleRowIndex, col]?.Value?.ToString()?.Trim();
                if (!title.IsNullOrWhiteSpace())
                {
                    excelDtoMap.Add(title, new ExcelDtoMemberInfo()
                    {
                        TitleName = title,
                        ColumnIndex = col
                    });
                }
            }

            #endregion
            for (var rowIndex = dataStartRowIndex; rowIndex <= endRowIndex; rowIndex++)
            {
                var t = new DynamicRow
                {
                    DynamicColumns = new List<DynamicRow.DynamicColumn>()
                };

                foreach (var column in excelDtoMap)
                {
                    var columnIndex = column.Value.ColumnIndex;
                    var columnTitle = column.Value.TitleName;

                    var cell = worksheet.Cells[rowIndex, columnIndex];
                    var cellValue = cell.Value?.ToString().Trim();
                    try
                    {
                        t.DynamicColumns.Add(new DynamicRow.DynamicColumn
                        {
                            Name = columnTitle,
                            Value = cellValue
                        });
                    }
                    catch (Exception ex)
                    {
                        AddReadErrorInfo(result, rowIndex, columnIndex, $"【{columnTitle}】{ex.Message}");
                    }
                }
                result.Data.Add(t);
            }

            return result;
        }
        #endregion



    }


    public class DynamicRow
    {
        public List<DynamicColumn> DynamicColumns { get; set; }


        public class DynamicColumn
        {
            /// <summary>
            /// 列名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 单元格内容
            /// </summary>
            public string Value { get; set; }
        }


    }



    #region 字段与标题对应关系

    /// <summary>
    /// Dto Excel 字段与标题对应关系
    /// </summary>
    public class ExcelDtoMemberInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string TitleName { get; set; }

        /// <summary>
        /// 字段属性
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 列号
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public int ColumnWidth { get; set; }
    }


    #endregion

}
