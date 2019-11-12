using System;
using System.Threading;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestCap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestCapController : ControllerBase
    {

        static ILogger<TestCapController> _logger;

        public TestCapController(ILogger<TestCapController> logger, ICapPublisher publisher)
        {
            _logger = logger;
            _capPublisher = publisher;
        }

        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <param name="threadCount">线程数</param>
        /// <param name="interval">间隔时间毫秒</param>
        /// <returns></returns>
        [HttpGet("start")]
        public IActionResult StartBatch(int threadCount = 1, int interval = 100)
        {
            StartBatchSend(threadCount, interval);
            return Ok("开始批量消息发送");
        }

        static ICapPublisher _capPublisher;
        static object lockObj = new object();
        static bool stopSend;
        static int _interval;
        static int sendCount;

        public static void StartBatchSend(int threadCount, int interval)
        {
            stopSend = false;
            sendCount = 0;
            _interval = interval;
            for (var i = 0; i < threadCount; i++)
            {
                new Thread(new ThreadStart(Send)).Start();
            }
        }


        private static async void Send()
        {
            while (!stopSend)
            {
                int index = GetNext();
                var message = new MessageEventData()
                {
                    Id = index.ToString(),
                    From = "from" + sendCount,
                    Payload = new { a = DateTime.Now },
                    Time = DateTime.Now,
                    To = "to" + sendCount,
                    Topic = "topic" + sendCount,
                    Type = "type" + sendCount,
                    Extra = new { }
                };
                _logger.LogInformation($"发送消息:{sendCount}");
                _capPublisher.PublishAsync(LocalGlobalParam.EventBusMessage, message);
                Thread.Sleep(_interval);
            }
        }
        private static int GetNext()
        {
            lock (lockObj)
            {
                return sendCount++;
            }
        }

        [HttpGet("stop")]
        public IActionResult StopBatch()
        {
            stopSend = true;
            return Ok("停止批量消息发送");
        }
    }
}
