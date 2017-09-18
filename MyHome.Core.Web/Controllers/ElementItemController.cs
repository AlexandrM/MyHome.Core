using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyHome.Shared;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MyHome.Core.Controllers
{
    [Route("api/[controller]")]
    public class ElementItemController : Controller
    {
        DB _db;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public ElementItemController(DB db, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            this._db = db;
            _env = env;
        }

        [HttpPost]
        public dynamic Post([FromBody] ElementItemModel model)
        {
            var db = _db.ElementItems.FirstOrDefault(x => x.Id == model.Id);
            if (db != null)
            {
                db.Name = model.Name;
                _db.SaveChanges();
            }
            return db;
        }
    }
}
