using Microsoft.AspNetCore.Builder;
using System;
using System.Linq;
using System.Net;

namespace TestCap
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalGlobalParam
    {
        static string serviceIp = string.Empty;

        static LocalGlobalParam()
        {
        }

        /// <summary>
        /// 应用的Guid
        /// </summary>
        /// <value>The unique identifier.</value>
        public static Guid Guid
        {
            get { return new Guid("{00000000-0000-0000-0000-000000000001}"); }
        }

        /// <summary>
        /// 应用的名称.
        /// </summary>
        /// <value>The module.</value>
        public static string Module
        {
            get { return "DataHub"; }
        }

        /// <summary>
        /// 应用的中文名
        /// </summary>
        public static string Name
        {
            get { return "数据中心"; }
        }

        /// <summary>
        /// 数据版本
        /// </summary>
        public static double DataVersion
        {
            get { return 1.0; }
        }

        /// <summary>
        /// API版本
        /// </summary>
        public static double ApiVersion
        {
            get { return 1.0; }
        }

        /// <summary>
        /// Gets or sets the application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        public static IApplicationBuilder ApplicationBuilder { get; set; }


        /// <summary>
        /// 获取主机的ip地址
        /// </summary>
        /// <returns></returns>
        public static string GetHoseIp()
        {
            if (string.IsNullOrEmpty(serviceIp))
            {
                serviceIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "127.0.0.1";
            }
            return serviceIp;
        }

        /// <summary>
        /// 当数据中心的数据发生变化后，会主动推送此消息给在线的第三方平台
        /// </summary>
        public const string EventEntityChange = "system.entity.change";

        /// <summary>
        /// 
        /// </summary>
        public const string EventBusMessage = "datahub.eventbus.notify";
    }
}
