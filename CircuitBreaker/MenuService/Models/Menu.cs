using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MenuService.Models
{
    public class Menu : Model<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageCDNUrl { get; set; }
    }
}
