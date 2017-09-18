using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyHome.Shared;
using Microsoft.EntityFrameworkCore;

namespace MyHome.Core.Web.Controllers
{
    [Route("api/[controller]")]
    public class DebugController : Controller
    {
        public static bool Enabled = true;
        private static List<string> log = new List<string>();

        public static void AddMessage(string message)
        {
            if (!Enabled)
            {
                return;
            }

            lock (log)
            {
                log.Add($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
                if (log.Count > 110)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        log.RemoveAt(log.Count - i - 1);
                    }
                }
            }
        }

        [HttpGet]
        public dynamic Get()
        {
            return new
            {
                list = log.ToList(),
                ok = true
            };
        }

        [HttpPost]
        public dynamic Post([FromBody] bool enable)
        {
            Enabled = enable;
            return new
            {
                ok = true
            };
        }
    }
}
