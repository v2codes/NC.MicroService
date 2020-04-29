using NC.MicroService.EntityFrameworkCore.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.EntityFrameworkCore.EntityModel
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class EntityBase<TKey>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual TKey CreateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public virtual TKey UpdateUser { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public virtual int State { get; set; }
    }
}
