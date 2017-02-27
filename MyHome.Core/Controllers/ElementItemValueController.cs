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
    public class ElementItemValueController : Controller
    {
        DB _db;

        public ElementItemValueController(DB db)
        {
            this._db = db;
        }

        /*[HttpGet]
        public dynamic Get()
        {
            var model = db.ElementItemValues
                .GroupBy(x => x.ElementItemId)
                .Select(x => x.OrderByDescending(z => z.Updated).FirstOrDefault())
                .ToList();

            model.ForEach(x => x.Value = db.ElementItemEnums.FirstOrDefault(z => z.Id == x.ValueId));

            return model;
        }*/

        private DateTime PrepareDate(DateTime dt, int accuracy)
        {
            if (accuracy != 0)
            {
                dt = dt.AddMilliseconds(-dt.Millisecond);
                dt = dt.AddSeconds(-dt.Second);
                dt = dt.AddMinutes(-(dt.Minute % accuracy));
            }

            return dt;
        }

        [HttpGet]
        public dynamic GetList(List<string> ids, DateTime from, DateTime to, int accuracy)
        {
            var model = (from element in _db.ElementItems.Where(x => ids.Contains(x.Id))
                         select new
                         {
                             key = element.Name,
                             values = _db.ElementItemValues
                                 .Where(x => x.ElementItemId == element.Id & x.DateTime >= @from & x.DateTime <= to).OrderBy(x => x.DateTime)
                                 .ToList()
                         }).ToList();

            var model2 = model.Select(x => new
            {
                key = x.key,
                values = x.values.Select(z => new
                {
                    DateTime = PrepareDate(z.DateTime, accuracy),
                    RawValue = z.RawValue.AsDecimal()
                })
            });

            var model3 = model2.Select(x => new
            {
                key = x.key,
                values = x.values
                    .GroupBy(z => z.DateTime)
                    .Select(g => new
                    {
                        DateTime = g.Key,
                        RawValue = Math.Round(g.Average(a => a.RawValue), 2),
                    })
            });

            return model3.ToList();
        }
    }
}
