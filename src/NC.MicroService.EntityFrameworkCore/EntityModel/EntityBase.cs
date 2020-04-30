using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.EntityFrameworkCore.EntityModel
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class EntityBase : IEntity<Guid>
    {
        public EntityBase()
        {
            var dtNow = DateTime.Now;
            this.Id = Guid.NewGuid();
            this.CreateTime = dtNow;
            this.CreateUser = Guid.Empty;
            this.State = 0;
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? CreateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public Guid? UpdateUser { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int? State { get; set; }
    }
}
