using MenuService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuService
{
    public class TestMenuBuilder
    {
        public IEnumerable<Menu> BuildList()
        {
            return new List<Menu>()
            {
                new Menu
                {
                    Id = Guid.NewGuid(),
                    Name = "Rice and Curry",
                    Description = "We've pulled together all our advertised offers into one place, so you won't miss out on a great deal.",
                    ImageCDNUrl = ""
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    Name = "Kotthu",
                    Description = "We've pulled together all our advertised offers into one place, so you won't miss out on a great deal.",
                    ImageCDNUrl = ""
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    Name = "Fried Rice",
                    Description = "We've pulled together all our advertised offers into one place, so you won't miss out on a great deal.",
                    ImageCDNUrl = ""
                }
            };
        }

        public Menu GetById(string id)
        {
            return BuildList().FirstOrDefault(x => x.Id == new Guid(id));
        }
    }
}
