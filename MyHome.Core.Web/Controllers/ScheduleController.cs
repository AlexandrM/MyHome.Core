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
    public class ScheduleController : Controller
    {
        DB _db;

        public ScheduleController(DB db)
        {
            this._db = db;
        }

        [HttpGet]
        public dynamic Get(string id)
        {
            IQueryable<ScheduleModel> q = _db.Schedules;
            q = q.AsNoTracking();
            q = q.Include(x => x.ElementItem);
            q = q.Include(x => x.ElementItem.EnumValues);
            q = q.Include(x => x.ScheduleHours);
            /*q = q.Include(x => x.ScheduleHours.Select(z => new ScheduleHourModel
            {
                DayOfWeek = z.DayOfWeek,
                Hour = z.Hour,
                RawValue = z.RawValue,
                ScheduleId = z.ScheduleId,
                ValueId = z.ValueId,
            }));*/

            if (id != null)
            {
                q = q.Where(x => x.ElementItemId == id);
            }

            var model = new
            {
                List = q.ToList(),
                Element = _db.ElementItems
                    .Include(x => x.EnumValues)
                    .FirstOrDefault(x => x.Id == id)
            };

            return model;
        }

        [HttpPost]
        public dynamic Post([FromBody] ScheduleModel schedule)
        {
            var model = _db.Schedules
                .Include(x => x.ScheduleHours)
                .FirstOrDefault(x => x.Id == schedule.Id);

            if (model == null)
            {
                model = schedule;
                model.Id = Guid.NewGuid().ToString();
                _db.Schedules.Add(model);
            }
            else
            {
                model.Name = schedule.Name;
                if (schedule.ScheduleHours != null)
                {
                    if (model.ScheduleHours == null)
                    {
                        model.ScheduleHours = new List<ScheduleHourModel>();
                    }
                    else
                    {
                        model.ScheduleHours.Clear();
                        _db.SaveChanges();
                    }
                    model.ScheduleHours.AddRange(schedule.ScheduleHours);
                }
            }
            _db.SaveChanges();

            return model;
        }

        [HttpDelete]
        public dynamic Delete([FromQuery] string id)
        {
            var model = _db.Schedules.FirstOrDefault(x => x.Id == id);
            _db.Schedules.Remove(model);
            _db.SaveChanges();

            return model;
        }
    }
}
