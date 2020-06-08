using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Library.CustomEventBus
{
    /// <summary>
    /// 领域模型对象
    /// 为了实现某个业务，而创建的实体类，它里面有事件所需要的数据，它继承了IEvent
    /// </summary>
    public class Team :IEvent
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
    }
}
