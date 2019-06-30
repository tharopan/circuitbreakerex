using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.Contract
{
    public class CircuitBreakerManager
    {
        private IReliableStateManager stateManager;

        private readonly int resetTimeOut;

        public CircuitBreakerManager(IReliableStateManager stateManager, int resetTimeoutInMilliseconds)
        {
            this.stateManager = stateManager;
            this.resetTimeOut = resetTimeoutInMilliseconds;
        }

        public async Task Invoke(Func<Task> func, Func<Task> failAction)
        {
            var errorHistory = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, DateTime>>("errorHistory");
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();

            using (var trnx = this.stateManager.CreateTransaction())
            {
                var errorTime = await errorHistory.TryGetValueAsync(trnx, "errorTime");
                if (errorTime.HasValue)
                {
                    if ((DateTime.UtcNow - errorTime.Value).TotalMilliseconds<this.resetTimeOut)
                    {
                        await failAction();
                        return;
                    }
                }
                try
                {
                    await func();
                    await errorHistory.AddOrUpdateAsync(
                        trnx,
                        "errorTime",
                        key => DateTime.MinValue,
                        (key, value) => DateTime.MinValue);
                }
                catch (Exception)
                {
                    await failAction();
                    await errorHistory.AddOrUpdateAsync(
                        trnx,
                        "errorTime",
                        key => DateTime.UtcNow,
                        (key, value) => DateTime.UtcNow);
                }
                finally
                {
                    await trnx.CommitAsync();
                }
            }
        }
    }
}
