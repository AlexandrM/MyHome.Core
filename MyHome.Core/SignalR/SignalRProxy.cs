using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Infrastructure;

using MyHome.Shared;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace MyHome.Core.SignalR
{
    public class SignalRProxy: ISignalRProxy
    {
        IConnectionManager _connectionManager;
        DB _db;

        public SignalRProxy(IConnectionManager connectionManager, DB db)
        {
            _connectionManager = connectionManager;
            _db = db;

            if (cacheElements.Count == 0)
            {
                var elements = _db.ElementItems.AsNoTracking().ToList();
                foreach(var element in elements)
                {
                    cacheElements.TryAdd(element.Id, element);

                    var last = _db.ElementItemValues
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
        /// <param name="elementItemValue"></param>
        public void ChangeElementItemValue(ElementItemValueModel elementItemValue)
        {
            var hub = _connectionManager.GetHubContext<ManageHub>();

            try
            {
                var now = DateTime.Now;

                ElementItemValueModel last;
                if (!cacheElementValues.TryGetValue(elementItemValue.ElementItemId, out last))
                {
                    last = _db.ElementItemValues
                        .AsNoTracking()
                        .OrderByDescending(x => x.Updated).ThenBy(x => x.DateTime)
                        .Where(x => x.ElementItemId == elementItemValue.ElementItemId)
                        .Take(1)
                        .FirstOrDefault();

                    cacheElementValues.TryAdd(elementItemValue.ElementItemId, last);
                }

                ElementItemModel element;
                if (!cacheElements.TryGetValue(elementItemValue.ElementItemId, out element))
                {
                    element = _db.ElementItems
                        .AsNoTracking()
                        .FirstOrDefault(x => x.Id == elementItemValue.ElementItemId);

                    cacheElements.TryAdd(elementItemValue.ElementItemId, element);
                }

                if (last != null)
                {
                    if (
                        ((elementItemValue.ValueId != null) && (last.ValueId == elementItemValue.ValueId))
                        || 
                        ((elementItemValue.ValueId == null) && (last.RawValue == elementItemValue.RawValue))
                        )
                    {
                        var lastDb = _db.ElementItemValues.FirstOrDefault(x => x.DateTime == last.DateTime & x.ElementItemId == last.ElementItemId);
                        lastDb.Updated = now;
                        _db.SaveChanges();
                        last.Updated = now;
                    }
                    /*if ((elementItemValue.ValueId == null) && (last.RawValue == elementItemValue.RawValue))
                    {
                        last.Updated = now;
                        //cacheElementValues.TryUpdate(elementItemValue.ElementItemId, last, last);
                    }*/
                }
                if ((last == null) || (last.Updated != now))
                {
                    elementItemValue.Updated = DateTime.Now;
                    elementItemValue.Value = _db.ElementItemEnums.FirstOrDefault(x => x.Id == elementItemValue.ValueId);
                    _db.ElementItemValues.Add(elementItemValue);
                    AfterChangeElementItemValue(elementItemValue);
                    if ((last != null) && (element.NotCollectChanges))
                    {
                        _db.ElementItemValues.Remove(last);
                    }
                    cacheElementValues.TryUpdate(elementItemValue.ElementItemId, elementItemValue, last);
                }

                _db.SaveChanges();
            }
            catch(Exception exc)
            {
            }
        }
        /// <summary>
        /// Called after value changed
        /// </summary>
        /// <param name="value"></param>
        public void AfterChangeElementItemValue(ElementItemValueModel value)
        {
            var hub = _connectionManager.GetHubContext<ManageHub>();
            hub.Clients.All.onAfterChangeElementItemValue(value);
        }

        public void TryChangeElementItemValue(ElementItemValueModel value)
        {
            ManageHub hub = (ManageHub) _connectionManager.GetHubContext<ManageHub>();
            hub.Clients.All.tryChangeElementItemValue(value);
        }
    }
}
