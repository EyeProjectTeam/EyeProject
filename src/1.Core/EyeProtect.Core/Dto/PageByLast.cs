using System.ComponentModel;

namespace EyeProject.Core.Dto
{
    /// <summary>
    /// 增量分页
    /// </summary>
    public class PageByLast : IPageByLast
    {
        /// <summary>
        /// 分页
        /// </summary>
        public PageByLast()
        {
            PageSize = 20;
        }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageByLast(int pageSize)
        {
            PageSize = pageSize;
        }

        /// <summary>
        /// 增量Id
        /// </summary>
        /// <example>5</example>
        public int? LastId { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        /// <example>30</example>
        [DefaultValue(20)]
        public int PageSize { get; set; }
    }
}