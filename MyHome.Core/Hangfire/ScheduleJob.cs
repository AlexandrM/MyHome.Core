using MyHome.Shared;
using System;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyHome.Core.SignalR;
using System.Threading.Tasks;

namespace MyHome.Core.Hangfire
{
    public class ScheduleJob
    {
        DB _db;
        IEnumerable<IPlugin> _plugins;

        public ScheduleJob(DB db, IEnumerable<IPlugin> plugins)
        {
            _db = db;
            _plugins = plugins;
        }

        public async Task<bool> Process()
        {
            var now = DateTime.Now;
            int dw = (int)DateTime.Now.DayOfWeek;
            dw = dw == 0 ? dw = 6 : dw - 1;


            var scheduleHours = _db.ScheduleHours
                            .AsNoTracking()
                            .Include(x => x.Schedule)
                            .Where(x => x.Schedule.ElementItem.ModeId == x.Schedule.Id && x.Hour == DateTime.Now.Hour && x.DayOfWeek == dw)
                            .ToList();

            foreach (var sh in scheduleHours)
            {
                var value = new ElementItemValueModel
                {
                    ElementItemId = sh.Schedule.ElementItemId
                };
                if (sh.ValueId != null)
                {
                    value.ValueId = sh.ValueId;
                }
                else
                {
                    value.RawValue = sh.RawValue;
                }

                ElementItemValueModel oldValue;
                SignalRProxy.CacheElementValues.TryGetValue(sh.Schedule.ElementItemId, out oldValue);

                foreach (var plugin in _plugins)
                {
                    await plugin.ChangeValue(value, oldValue);
                }
            }

            return true;
        }
    }
}
