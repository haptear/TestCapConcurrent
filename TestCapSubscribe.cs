using DotNetCore.CAP;
using DotNetCore.CAP.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;

namespace TestCap
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DotNetCore.CAP.ICapSubscribe" />
    public class TestCapSubscribe : ICapSubscribe
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataHubApiManager"></param>
        /// <param name="schemaRepository"></param>
        /// <param name="extdataRepository"></param>
        /// <param name="webSocketManager"></param>
        public TestCapSubscribe(ILogger<TestCapSubscribe> logger)
        {
            Logger = logger;
        }


        protected ILogger Logger { get; set; }

        /// <summary>
        /// 数据发生变化
        /// </summary>
        /// <param name="message"></param>
        [CapSubscribe(LocalGlobalParam.EventEntityChange)]
        public async Task EventEntityChange(MessageEventData message)
        {
            System.Threading.Thread.Sleep(500);

            Logger.LogInformation($"开始发送消息 {message.Id}");
        }

        /// <summary>
        /// 消息订阅处理
        /// </summary>
        /// <param name="message"></param>
        [CapSubscribe(LocalGlobalParam.EventBusMessage)]
        public async Task EventBusMessage(MessageEventData message)
        {
            System.Threading.Thread.Sleep(500);

            Logger.LogInformation($"开始发送消息 {message.Id}");
        }
    }


    public class MessageEventData
    {
        public MessageEventData()
        {
            From = new object();
            To = string.Empty;
            Payload = new object();
            Extra = new object();
        }

        /// <summary>
        /// 消息标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 消息的发生时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 消息来源
        /// </summary>
        public Object From { get; set; }

        /// <summary>
        /// 发送目标
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 消息的编码，同一种信息编码唯一 格式:{categroy}:{sub}:{type}
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 消息的实体
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// 附加信息，需要回复时，回复消息会把此数据带回
        /// </summary>
        public object Extra { get; set; }
    }
}
