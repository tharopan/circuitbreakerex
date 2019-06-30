using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreaker.Contract.ReliableService.Models
{
    public class Menu : Model<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageCDNUrl { get; set; }
    }
}
