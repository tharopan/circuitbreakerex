using MenuService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MenuService.Controllers
{
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private TestMenuBuilder menuService;

        public MenuController()
        {
            menuService = new TestMenuBuilder();
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Menu> Get()
        {
            return menuService.BuildList();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Menu Get(string id)
        {
            return menuService.GetById(id);
        }
    }
}
