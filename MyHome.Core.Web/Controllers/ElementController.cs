﻿using System;
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
    public class ElementController : Controller
    {
        DB _db;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public ElementController(DB db, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            this._db = db;
            _env = env;
        }

        [HttpGet]
        public dynamic Get()
        {
            var model = _db.Elements
                .AsNoTracking()
                .Include(x => x.Items).ThenInclude(item => item.EnumValues)
                .Include(x => x.Items).ThenInclude(item => item.Mode)
                .ToList();

            var q = _db.ElementItemValues
                .GroupBy(x => x.ElementItemId)
                .Select(x => x.OrderByDescending(z => z.Updated).First());
                
            var values = _db.ElementItemValues
                .GroupBy(x => x.ElementItemId)
                .Select(x => x.OrderByDescending(z => z.Updated).First()).ToList();

            model.ForEach(x =>
            {
                x.Items.ForEach(z => {
                    var item = values.FirstOrDefault(y => y.ElementItemId == z.Id);
                    if (item != null)
                    {
                        z.Values= new List<ElementItemValueModel>(new [] { item });
                    }
                });
            });

            return model;
        }
    }
}
