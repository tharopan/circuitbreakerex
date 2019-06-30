using System;

namespace CircuitBreaker.Contract.ReliableService
{
    public class RequestLog
    {
        public CircuitState CircuitState { get; set; }

        public double RequestDuration { get; set; }

        public DateTime RequestTime { get; set; }
    }
}
