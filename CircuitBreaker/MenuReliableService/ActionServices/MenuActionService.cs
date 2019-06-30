using CircuitBreaker.Contract.ReliableService;
using CircuitBreaker.Contract.ReliableService.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MenuReliableService.ActionServices
{
    public class MenuActionService
    {
        public void InvokeGet(string id, ref Response<Menu> result)
        {
            var client = new RestClient("URLLLLLLL");
            var request = new RestRequest("?id={id}", Method.GET);
            request.AddUrlSegment("id", id);
            request.Timeout = TimeSpan.FromSeconds(10).Milliseconds;
            var response = client.Execute<Menu>(request);
            if (response?.Data != null)
            {
                result.Data = response.Data;
                result.CircuitState = CircuitState.Open;
            }
            else
            {
                throw new ApplicationException();
            }
        }
    }
}
