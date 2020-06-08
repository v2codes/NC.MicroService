using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Example.Library.CustomEventBus
{
    /// <summary>
    /// 事件总线 - 核心功能代码
    /// 承载了事件的发布，订阅与取消订阅的逻辑，EventBus
    /// </summary>
    public class EventBus
    {
        private EventBus() { }

        /// <summary>
        /// 事件消息存储，这里用内存字典暂存（推荐RabbitMQ）
        /// </summary>
        private static Dictionary<Type, List<object>> _eventHandlerDict = new Dictionary<Type, List<object>>();

        /// <summary>
        /// 事件总线对象
        /// </summary>
        private static EventBus _eventBus = null;
        
        /// <summary>
        /// 附加领域模型处理句柄时，使用同步锁
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// 事件总线（单例）
        /// </summary>
        public static EventBus Instance
        {
            get
            {
                return _eventBus ?? (_eventBus = new EventBus());
            }
        }

        /// <summary>
        /// 判断事件处理对象是否相等
        /// </summary>
        private readonly Func<object, object, bool> _eventHandlerEquals = (o1, o2) =>
        {
            var o1Type = o1.GetType();
            var o2Type = o2.GetType();
            if (o1Type.IsGenericType && o2Type.IsGenericType)
            {
                return o1.Equals(o2);
            }
            return o1 == o2;
        };

        /// <summary>
        /// 通过XML文件初始化事件总线（订阅信息在XML里配置）
        /// 初始化事件发布与订阅者的关联关系
        /// TODO：实际情况下，该处可通过特性标记、接口实现等方式实现发布事件和订阅事件的初始化存储
        /// </summary>
        /// <returns></returns>
        public static EventBus InstanceFromXml()
        {
            if (_eventBus == null)
            {
                var root = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EventBus.xml"));
                foreach (var @event in root.Elements("Event"))
                {
                    var handlers = new List<object>();
                    var publishEventType = Type.GetType(@event.Element("PublishEvent").Value);
                    foreach (var subscritedEvt in @event.Elements("SubscribedEvents"))
                    {
                        foreach (var concreteEvt in subscritedEvt.Elements("SubscribedEvent"))
                        {
                            handlers.Add(Type.GetType(concreteEvt.Value));
                        }
                    }
                    _eventHandlerDict[publishEventType] = handlers;
                }
                _eventBus = new EventBus();
            }
            return _eventBus;
        }

        #region 时间订阅&取消订阅，可以扩展
        /// <summary>
        /// 订阅事件列表
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent
        {
            lock (_lockObj)
            {
                // 获取领域模型的类型
                var eventType = typeof(TEvent);
                // 如果此领域类型在事件总线中已注册过，更新，否则新增
                if (_eventHandlerDict.ContainsKey(eventType))
                {
                    var handlers = _eventHandlerDict[eventType];
                    if (handlers != null)
                    {
                        if (!handlers.Exists(deh => _eventHandlerEquals(deh, eventHandler)))
                        {
                            handlers.Add(eventHandler);
                        }
                    }
                    else
                    {
                        handlers = new List<object>();
                        handlers.Add(eventHandler);
                    }
                }
                else
                {
                    _eventHandlerDict.Add(eventType, new List<object> { eventHandler });
                }
            }
        }

        /// <summary>
        /// 订阅事件列表
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlers"></param>
        public void Subscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers) where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
            {
                Subscribe<TEvent>(eventHandler);
            }
        }

        /// <summary>
        /// 取消事件订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent
        {
            lock (_lockObj)
            {
                var eventType = typeof(TEvent);
                if (_eventHandlerDict.ContainsKey(eventType))
                {
                    var handlers = _eventHandlerDict[eventType];
                    if (handlers != null && handlers.Exists(deh => _eventHandlerEquals(deh, eventHandler)))
                    {
                        var handlerToRemove = handlers.First(deh => _eventHandlerEquals(deh, eventHandler));
                        handlers.Remove(handlerToRemove);
                    }
                }
            }
        }

        /// <summary>
        /// 取消事件订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlers"></param>
        public void Unsubscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers) where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
            {
                Unsubscribe<TEvent>(eventHandler);
            }
        }
        #endregion

        #region 事件发布
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            var eventType = @event.GetType();
            if (_eventHandlerDict.ContainsKey(eventType) && _eventHandlerDict[eventType] != null && _eventHandlerDict[eventType].Count > 0)
            {
                var handlers = _eventHandlerDict[eventType];
                foreach (var handler in handlers)
                {
                    var eventHandler = handler as IEventHandler<TEvent>;
                    eventHandler.Handle(@event);
                }
            }
        }
        #endregion
    }
}
