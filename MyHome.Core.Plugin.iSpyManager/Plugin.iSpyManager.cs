using Microsoft.Extensions.Configuration;
using MyHome.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHome.Core.Plugin.iSpyManager
{
    public class Plugin : IPlugin
    {
        IConfigurationRoot _cfg;
        ISignalRProxy _proxy;
        DB _db;

        public Plugin(IConfigurationRoot cfg, ISignalRProxy proxy, DB db)
        {
            _cfg = cfg;
            _proxy = proxy;
            _db = db;
        }

        private string KeyRoot = "MyHome.Core.Plugin.iSpyManager";

        public Task<bool> Init()
        {
            try
            {
                var element = _db.Elements.FirstOrDefault(x => x.Id == KeyRoot);
                if (element == null)
                {
                    element = new ElementModel
                    {
                        Id = KeyRoot,
                        ExternalId = KeyRoot,
                        Name = "Наблюдение",
                    };
                    _db.Elements.Add(element);
                    _db.SaveChanges();
                }

                var elementItem = _db.ElementItems.FirstOrDefault(x => x.Id == $"{KeyRoot}.Mode" & x.ElementId == KeyRoot);
                if (elementItem == null)
                {
                    elementItem = new ElementItemModel
                    {
                        Id = $"{KeyRoot}.Mode",
                        ElementId = KeyRoot,
                        Name = "Режим работы",
                        Type = ElementItemType.Manage,
                        AllowSchedule = false,
                        Description = "Режим работы",
                        IsEnum = true,
                        RefreshTime = 60 * 60 * 24,
                        EnumValues = new List<ElementItemEnumModel>() {
                                new ElementItemEnumModel { Id = $"{KeyRoot}.Mode.On", ElementItemId = $"{KeyRoot}.Mode", Value = "on", Name = "Все включены" },
                                new ElementItemEnumModel { Id = $"{KeyRoot}.Mode.Off", ElementItemId = $"{KeyRoot}.Mode", Value = "off", Name = "Все выключены" },
                                new ElementItemEnumModel { Id = $"{KeyRoot}.Mode.Security", ElementItemId = $"{KeyRoot}.Mode", Value = "security", Name = "Охрана" },
                            },
                    };
                    _db.ElementItems.Add(elementItem);
                    _db.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                Log.L("Exception MyHome.Service.iSpyManager.Init(): " + exc.Message);
            }
            return Task.FromResult(true);
        }

        public Task<bool> ChangeValue(ElementItemValueModel value, ElementItemValueModel oldValue)
        {
            try
            {

            }
            catch (Exception exc)
            {
                Log.L("Exception MyHome.Service.iSpyManager.ChangeValue(): " + exc.Message);
            }
            return Task.FromResult(true);
        }

        public Task<bool> Process()
        {
            try
            {

            }
            catch
            {
            }

            return Task.FromResult(true);
        }
    }
}
