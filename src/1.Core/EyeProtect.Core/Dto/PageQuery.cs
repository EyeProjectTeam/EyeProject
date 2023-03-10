using System;
using System.Linq.Expressions;
using EyeProtect.Core.Utils;

namespace EyeProject.Core.Dto
{
    /// <summary>
    /// 分页查询
    /// </summary>
    public class PageQuery : PageSortInfo, IPageQuery
    {
        /// <summary>
        /// 指定查询条件
        /// </summary>
        protected LambdaExpression Filter { get; set; }

        /// <summary>
        /// 并且
        /// </summary>
        public virtual void And<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            Filter = (Filter as Expression<Func<TEntity, bool>>).And(filter);
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <typeparam name="TEntity">要查询的实体类型</typeparam>
        public Expression<Func<TEntity, bool>> GetFilter<TEntity>() where TEntity : class
        {
            return (Filter as Expression<Func<TEntity, bool>>).And(this.GetQueryExpression<TEntity>());
        }
    }
}