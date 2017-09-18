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
    public class ElementItemModeController : Controller
    {
        DB _db;

        public ElementItemModeController(DB db)
        {
            this._db = db;
        }

        [HttpPost]
        public dynamic Post([FromBody] ElementItemModel model)
        {
            var db = _db.ElementItems.FirstOrDefault(x => x.Id == model.Id);
            db.ModeId = model.ModeId;
            _db.SaveChanges();

            return model;
        }
    }
}
