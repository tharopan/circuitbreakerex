namespace CircuitBreaker.Contract.ReliableService.Models
{
    public class Response<TData>
    {
        public TData Data { get; set; }

        public CircuitState CircuitState { get; set; }

        public double ResponseTimeInSeconds { get; set; }
    }
}
