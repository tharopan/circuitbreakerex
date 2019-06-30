using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CircuitBreaker.Contract;
using CircuitBreaker.Contract.ReliableService;
using CircuitBreaker.Contract.ReliableService.MenuServices;
using CircuitBreaker.Contract.ReliableService.Models;
using MenuReliableService.ActionServices;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace MenuReliableService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class MenuReliableService : StatefulService, IMenuReliableService
    {
        private CircuitBreakerManager circuitBreaker;
        private MenuActionService menuActionService;
        private MenuFailActionService menuFailActionService;

        public MenuReliableService(StatefulServiceContext context)
            : base(context)
        {
            this.circuitBreaker = new CircuitBreakerManager(this.StateManager, 50000);
            this.menuActionService = new MenuActionService();
            this.menuFailActionService = new MenuFailActionService();
        }

        public async  Task<Response<IEnumerable<Menu>>> Get()
        {
            throw new NotImplementedException();
        }


        public async Task<Response<Menu>> Get(string id)
        {
            var counterState = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(
                "menuState");

            Response<Menu> result = null;

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();

            this.circuitBreaker.Invoke(
                async () =>
                {
                    //menuActionService.InvokeGet(id, out result);

                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        await counterState.AddOrUpdateAsync(
                            tx,
                            "savedMenu",
                            key => JsonConvert.SerializeObject(result),
                            (key, val) => JsonConvert.SerializeObject(result));
                        await tx.CommitAsync();
                    }

                    //TODO: Store to redis cache
                },
                async () =>
                {
                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        // service faulted. read old value and populate.
                        var value = await counterState.TryGetValueAsync(tx, "savedMenu");
                        if (value.HasValue)
                        {
                            result = JsonConvert.DeserializeObject<Response<Menu>>(value.Value);
                        }
                        else
                        {
                            //TODO:: Get from redis Cache
                            //result = // read from cache
                            await counterState.AddOrUpdateAsync(
                                tx,
                                "savedMenu",
                                key => JsonConvert.SerializeObject(result),
                                (key, val) => JsonConvert.SerializeObject(result));
                            await tx.CommitAsync();
                        }

                        result.CircuitState = CircuitState.Closed;
                    }
                });

            return result;
        }



        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
