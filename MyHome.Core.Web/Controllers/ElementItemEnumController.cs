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
    public class ElementItemEnumController : Controller
    {
        DB _db;

        public ElementItemEnumController(DB db)
        {
            this._db = db;
        }

        [HttpGet]
        public dynamic Get()
        {
            var model = _db.ElementItemEnums
                .ToList();

            return model;
        }
    }
}
