using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Library.CustomEventBus
{
    /// <summary>
    /// 领域对象事件处理程序，团队创建时，输出日志
    /// 领域对象发生变化时，通知EventBus触发所有的事件处理程序
    /// </summary>
    public class TeamCreateEventHandler : IEventHandler<Team>
    {
        public void Handle(Team @event)
        {
            Console.WriteLine("团队创建事件消息处理程序：Team ID={0}", @event.Id);
        }
    }
}
