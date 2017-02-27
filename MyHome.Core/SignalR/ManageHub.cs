﻿using System;
using System.Threading.Tasks;

using MyHome.Shared;
using System.Collections.Generic;
using Autofac;

namespace MyHome.Core.SignalR
{
    //For sending to frontend
    public class ManageHub : Microsoft.AspNetCore.SignalR.Hub
    {
        DB _db;

        IEnumerable<IPlugin> _plugins;

        public ManageHub(DB db, IEnumerable<IPlugin> plugins)
        {
            _db = db;
            _plugins = plugins;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public void changeElementItemValue(ElementItemValueModel value)
        {
            Clients.All.onChangeElementItemValue(value);
        }

        /// <summary>
        /// Try to change value in plugins from web
        /// </summary>
        /// <param name="value"></param>
        public void tryChangeElementItemValue(ElementItemValueModel value)
        {
            foreach(var plugin in _plugins)
            {
                plugin.ChangeValue(value, null);
            }
        }

        public void afterChangeElementItemValue(ElementItemValueModel value)
        {
            Clients.All.onAfterChangeElementItemValue(value);
        }
        
        /// <summary>
        /// Get last value from cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ElementItemValueModel getLastElementItemValue(string id)
        {
            ElementItemValueModel value;
            if (SignalRProxy.CacheElementValues.TryGetValue(id, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Get last valued from cache
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<ElementItemValueModel> getLastElementItemValues(List<string> ids)
        {
            var ret = new List<ElementItemValueModel>();
            foreach (var id in ids)
            {
                ElementItemValueModel value;
                if (SignalRProxy.CacheElementValues.TryGetValue(id, out value))
                {
                    ret.Add(value);
                }
            }
            return ret;
        }
    }
}
