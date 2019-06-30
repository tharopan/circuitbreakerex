using CircuitBreaker.Contract.ReliableService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MenuReliableService.ActionServices
{
    public class MenuFailActionService
    {
        public void InvokeGet(string id, out Response<Menu> result)
        {
            throw new NotImplementedException();
        }
    }
}
