using System;
using System.Threading.Tasks;

using MyHome.Shared;
using System.Collections.Generic;

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

        public Task changeElementItemValue(ElementItemValueModel value)
        {
            Web.Controllers.DebugController.AddMessage($"ManageHub.onChangeElementItemValue -> {value.ElementItemId} = {value.RawValue} / {value.ValueId}");
            return Clients.All.InvokeAsync("onChangeElementItemValue", value);
        }

        /// <summary>
        /// Try to change value in plugins from web
        /// </summary>
        /// <param name="value"></param>
        public async void tryChangeElementItemValue(ElementItemValueModel value)
        {
            foreach(var plugin in _plugins)
            {
                if (await plugin.ChangeValue(value, null))
                {
                    Web.Controllers.DebugController.AddMessage($"ManageHub.tryChangeElementItemValue -> {value.ElementItemId} = {value.RawValue} / {value.ValueId}");
                }
            }
        }

        public Task afterChangeElementItemValue(ElementItemValueModel value)
        {
            if (value.ElementItem != null)
            {
                if (value.ElementItem.Element != null)
                {
                    if (value.ElementItem.Element.Items != null)
                    {
                        value.ElementItem.Element.Items = null;
                    }
                }
            }

            Web.Controllers.DebugController.AddMessage($"ManageHub.onAfterChangeElementItemValue -> {value.ElementItemId} = {value.RawValue} / {value.ValueId}");
            return Clients.All.InvokeAsync("onAfterChangeElementItemValue", value);
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
