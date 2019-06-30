using System;
using System.Collections.Generic;
using System.Text;

namespace MenuReliableService.ActionServices
{
    public class MenuActionService
    {
        public void InvokeGet(string id, out Response<Menu> result)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["weatherapi"]);
            var request = new RestRequest("?id={id}", Method.GET);
            request.AddUrlSegment("id", id);
            request.Timeout = TimeSpan.FromSeconds(10).Milliseconds;
            var response = client.Execute<WeatherReport>(request);
            if (response?.Data != null)
            {
                result.Data = response.Data;
                result.CircuitState = CircuitStatus.Open;
            }
            else
            {
                throw new ApplicationException();
            }
        }
    }
}
