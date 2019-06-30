using CircuitBreaker.Contract.ReliableService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircuitBreaker.Contract.ReliableService.MenuServices
{
    public interface IMenuReliableService : IService
    {
        Task<IEnumerable<Menu>> Get();

        Task<Menu> Get(string id);
    }
}
