using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.EntityFrameworkCore.EntityModel
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public interface IEntity<TKey>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        TKey Id { get; set; }
    }
}
