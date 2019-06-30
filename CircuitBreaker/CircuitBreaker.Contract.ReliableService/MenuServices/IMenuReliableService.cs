using CircuitBreaker.Contract.ReliableService.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircuitBreaker.Contract.ReliableService.MenuServices
{
    public interface IMenuReliableService : IService
    {
        Task<Response<IEnumerable<Menu>>> Get();

        Task<Response<Menu>> Get(string id);
    }
}
