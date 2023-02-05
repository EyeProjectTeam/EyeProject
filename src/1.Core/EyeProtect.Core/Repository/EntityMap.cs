using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.Domain.Entities;

namespace EyeProtect.Core.Repository
{
    /// <summary>
    /// 实体类型映射
    /// </summary>
    public abstract class EntityMap<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 配置实体
        /// </summary>
        public virtual void Configure(EntityTypeBuilder<TEntity> b)
        {
            ConfigTableName(b);
        }

        /// <summary>
        /// 配置表名和主键
        /// </summary>
        protected virtual void ConfigTableName(EntityTypeBuilder<TEntity> b)
        {
            b.ToTable(typeof(TEntity).Name);
        }
    }

    /// <summary>
    /// 实体类型映射
    /// </summary>
    public abstract class EntityMap<TEntity, TKey> : EntityMap<TEntity>
        where TEntity : class, IEntity<TKey>
    {

    }
}
