using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR.Infrastructure;

using MyHome.Shared;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace MyHome.Core.SignalR
{
    public class SignalRProxy: ISignalRProxy
    {
        IHubContext<ManageHub> hub;

        public SignalRProxy(IHubContext<ManageHub> hub, DB db)
        {
            this.hub = hub;

            if (cacheElements.Count == 0)
            {
                var elements = db.ElementItems.AsNoTracking().ToList();
                foreach(var element in elements)
                {
                    cacheElements.TryAdd(element.Id, element);

                    var last = db.ElementItemValues
                        .AsNoTracking()
                        .OrderByDescending(x => x.Updated).ThenBy(x => x.DateTime)
                        .Where(x => x.ElementItemId == element.Id)
                        .FirstOrDefault();

                    if (last != null)
                    {
                        cacheElementValues.TryAdd(element.Id, last);
                    }
                }
            }
        }

        static ConcurrentDictionary<string, ElementItemModel> cacheElements = new ConcurrentDictionary<string, ElementItemModel>();
        static ConcurrentDictionary<string, ElementItemValueModel> cacheElementValues = new ConcurrentDictionary<string, ElementItemValueModel>();

        public static ConcurrentDictionary<string, ElementItemValueModel>  CacheElementValues
        {
            get
            {
                return cacheElementValues;
            }
        }

        /// <summary>
        /// Called from Plugin for try change value to elementItemValue
        /// If elementItemValue is different of cacheElementValues[elementItemValue.ElementItemId] - called AfterChangeElementItemValue()
        /// </summary>
        /// <param name="value"></param>
        public void ChangeElementItemValue(ElementItemValueModel value)
        {
            try
            {
                using (var _db = new DB())
                {
                    var now = DateTime.Now;

                    ElementItemValueModel last;
                    if (!cacheElementValues.TryGetValue(value.ElementItemId, out last))
                    {
                        last = _db.ElementItemValues
                            .AsNoTracking()
                            .OrderByDescending(x => x.Updated).ThenBy(x => x.DateTime)
                            .Where(x => x.ElementItemId == value.ElementItemId)
                            .Take(1)
                            .FirstOrDefault();

                        cacheElementValues.TryAdd(value.ElementItemId, last);
                    }

                    ElementItemModel element;
                    if (!cacheElements.TryGetValue(value.ElementItemId, out element))
                    {
                        element = _db.ElementItems
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == value.ElementItemId);

                        cacheElements.TryAdd(value.ElementItemId, element);
                    }

                    if (last != null)
                    {
                        if (
                            ((value.ValueId != null) && (last.ValueId == value.ValueId))
                            ||
                            ((value.ValueId == null) && (last.RawValue == value.RawValue))
                            )
                        {
                            var lastDb = _db.ElementItemValues.FirstOrDefault(x => x.DateTime == last.DateTime & x.ElementItemId == last.ElementItemId);
                            lastDb.Updated = now;
                            _db.SaveChanges();
                            last.Updated = now;
                        }
                    }
                    if ((last == null) || (last.Updated != now))
                    {
                        value.Updated = DateTime.Now;
                        value.Value = _db.ElementItemEnums.FirstOrDefault(x => x.Id == value.ValueId);
                        _db.ElementItemValues.Add(value);
                        AfterChangeElementItemValue(value);
                        if ((last != null) && (element.NotCollectChanges))
                        {
                            _db.ElementItemValues.Remove(last);
                        }
                        cacheElementValues.TryUpdate(value.ElementItemId, value, last);
                    }

                    _db.SaveChanges();
                }
            }
            catch(Exception exc)
            {
                Web.Controllers.DebugController.AddMessage($"[!!!] ManageHub.ChangeElementItemValue -> {value.ElementItemId} = {value.RawValue} / {value.ValueId} ({exc.Message} : {(exc.InnerException == null ? "" : exc.InnerException.Message)})");
            }
        }
        /// <summary>
        /// Called after value changed
        /// </summary>
        /// <param name="value"></param>
        public void AfterChangeElementItemValue(ElementItemValueModel value)
        {
            Web.Controllers.DebugController.AddMessage($"ManageHub.onAfterChangeElementItemValue -> {value.ElementItemId} = {value.RawValue} / {value.ValueId}");
            hub.Clients.All.InvokeAsync("onAfterChangeElementItemValue", value);
        }

        public void TryChangeElementItemValue(ElementItemValueModel value)
        {
            Web.Controllers.DebugController.AddMessage($"ManageHub.tryChangeElementItemValue -> {value.ElementItemId} = {value.RawValue} / {value.ValueId}");
            hub.Clients.All.InvokeAsync("tryChangeElementItemValue", value);
        }
    }
}
