using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP;
using DotNetCore.CAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

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


        private static void Send()
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

                Task.Run(async () =>
                {
                    //using (var connection = new MySqlConnection(AppDbContext.ConnectionString))
                    //{
                    //    await connection.ExecuteAsync(PrepareSql(), new CapPublishedMessage()
                    //    {
                    //        Id = index,
                    //        Added = DateTime.Now,
                    //        Content = "{}",
                    //        ExpiresAt = DateTime.Now.AddDays(1),
                    //        Name = "datahub.eventbus.notify",
                    //        Retries = 0,
                    //        StatusName = "Succeeded"
                    //    });
                    //    return;
                    //}

                    await _capPublisher.PublishAsync(LocalGlobalParam.EventBusMessage, message);
                });

                Thread.Sleep(_interval);
            }
        }

        private static string PrepareSql()
        {
            return
                $"INSERT INTO `cap.published` (`Id`,`Version`,`Name`,`Content`,`Retries`,`Added`,`ExpiresAt`,`StatusName`)" +
                $"VALUES(@Id,'v1',@Name,@Content,@Retries,@Added,@ExpiresAt,@StatusName);";
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
