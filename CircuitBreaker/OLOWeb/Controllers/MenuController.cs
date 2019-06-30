﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLOWeb.Controllers
{
    public class MenuController : Controller
    {

        private static readonly string RequestLog = nameof(RequestLog);
        private static readonly Uri MenuServiceUri;
        private readonly IMenuService menuServiceClient;

        public MenuController()
        {
            MenuServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/MenuService");
            menuServiceClient = ServiceProxy.Create<IWeatherService>(MenuServiceUri, new ServicePartitionKey("basic"));
        }

        public IActionResult Get(string id)
        {
            List<RequestLog> requestLog = null;
            if (this.HttpContext.Session.Keys.Contains(RequestLog))
            {
                var value = this.HttpContext.Session.GetString(RequestLog);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    requestLog = JsonConvert.DeserializeObject<List<RequestLog>>(value);
                }
            }

            var watch = Stopwatch.StartNew();
            var result = await this.menuServiceClient.Get("testtttt");
            watch.Stop();
            result.ResponseTimeInSeconds = watch.Elapsed.TotalSeconds;
            if (requestLog == null)
            {
                requestLog = new List<RequestLog>();
            }

            requestLog.Add(new RequestLog { CircuitState = result.CircuitState, RequestDuration = result.ResponseTimeInSeconds, RequestTime = DateTime.UtcNow });
            this.HttpContext.Session.SetString(RequestLog, JsonConvert.SerializeObject(requestLog));
            return this.View(result);
        }

        public IActionResult Get()
        {
            List<RequestLog> requestLog = null;
            if (this.HttpContext.Session.Keys.Contains(RequestLog))
            {
                var value = this.HttpContext.Session.GetString(RequestLog);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    requestLog = JsonConvert.DeserializeObject<List<RequestLog>>(value);
                }
            }

            var watch = Stopwatch.StartNew();
            var result = await this.menuServiceClient.Get();
            watch.Stop();
            result.ResponseTimeInSeconds = watch.Elapsed.TotalSeconds;
            if (requestLog == null)
            {
                requestLog = new List<RequestLog>();
            }

            requestLog.Add(new RequestLog { CircuitState = result.CircuitState, RequestDuration = result.ResponseTimeInSeconds, RequestTime = DateTime.UtcNow });
            this.HttpContext.Session.SetString(RequestLog, JsonConvert.SerializeObject(requestLog));
            return this.View(result);
        }
    }
}
