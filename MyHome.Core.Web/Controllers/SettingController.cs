using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyHome.Shared;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MyHome.Core.Controllers
{
    //[Microsoft.AspNetCore.Cors.EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SettingController : Controller
    {
        DB db;

        public SettingController(DB db)
        {
            this.db = db;
        }

        [HttpGet]
        public dynamic Get([FromQuery]string id, [FromQuery]string group)
        {
            var q = db.Settings.AsQueryable();
            if (id != null)
            {
                q = q.Where(x => x.Id == id);
            }
            if (group != null)
            {
                q = q.Where(x => x.Group == group);
            }

            return q.ToList();
        }

        [HttpPost]
        public dynamic Post([FromBody] SettingModel m)
        {
            var model = db.Settings.Find(m.Id);
            if (model == null)
            {
                model = m;
                if (String.IsNullOrEmpty(m.Id))
                {
                    model.Id = Guid.NewGuid().ToString();
                }
                db.Settings.Add(model);
            }
            else
            {
                model.Name = m.Name;
                model.Value = m.Value;
            }
            db.SaveChanges();

            return model;
        }

        [HttpDelete]
        public dynamic Delete([FromQuery] string id)
        {
            var model = db.Settings.Find(id);
            if (model != null)
            {
                db.Settings.Remove(model);
                db.SaveChanges();
            }

            return model;
        }
    }
}
