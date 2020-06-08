using Example.Library.CustomEventBus;
using System;

namespace Example.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEventBus();

            Console.ReadKey();
        }

        /// <summary>
        /// 事件总线简单实现 ==> https://www.cnblogs.com/tiancai/p/7266858.html
        /// </summary>
        private static void TestEventBus()
        {
            // 订阅事件
            EventBus.Instance.Subscribe(new TeamCreateEventHandler());

            // 创建领域模型 --> 团队
            var team = new Team { Id = Guid.NewGuid() };
            Console.WriteLine("创建新的团队操作，ID={0}", team.Id);

            // 发布领域事件
            EventBus.Instance.Publish(team);
        }
    }
}
