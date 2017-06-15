using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using MyHome.Shared;
using System.Diagnostics;

namespace MyHome.Core.Plugin.ValueKeeper
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

        private string KeyRoot = "MyHome.Core.Plugin.ValueKeeper";

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
                        Name = "Показания",
                    };
                    _db.Elements.Add(element);
                    _db.SaveChanges();
                }

                var items = _cfg.GetSection("Plugins").GetSection(KeyRoot).GetChildren();
                foreach(var item in items)
                {
                    var name = item.GetValue<string>("Name");
                    var description = item.GetValue<string>("Description");
                    var type = item.GetValue<int>("Type");

                    var elementItem = _db.ElementItems.FirstOrDefault(x => x.Id == $"{KeyRoot}.{item.Key}" & x.ElementId == KeyRoot);
                    if (elementItem == null)
                    {
                        elementItem = new ElementItemModel
                        {
                            Id = $"{KeyRoot}.{item.Key}",
                            ElementId = KeyRoot,
                            Name = name,
                            Type = (ElementItemType) type,
                            AllowSchedule = false,
                            Description = description,
                            IsEnum = false,
                            RefreshTime = 60 * 60 * 24,
                        };
                        _db.ElementItems.Add(elementItem);
                        _db.SaveChanges();
                    }
                }
            }
            catch(Exception exc)
            {
                Log.L("Exception MyHome.Service.ValueKeeper.Init(): " + exc.Message);
            }

            return Task.FromResult(true);
        }

        public Task<bool> ChangeValue(ElementItemValueModel value, ElementItemValueModel oldValue)
        {
            try
            {

                if (value.ElementItemId.StartsWith(KeyRoot))
                {
                    _proxy.ChangeElementItemValue(value);

                    var element = _db.ElementItems.FirstOrDefault(x => x.Id == value.ElementItemId);
                    if (element.Type == ElementItemType.Manage)
                    {
                        var d = value.RawValue.AsDecimal();

                        ElementItemValueModel valueP = null;
                        var countCM = _db.ElementItemValues.Where(x => x.ElementItemId == value.ElementItemId & x.DateTime.Year == DateTime.Now.Year & x.DateTime.Month == DateTime.Now.Month).Count();
                        if (countCM == 1)
                        {
                            var dt = DateTime.Now.AddMonths(-1);
                            valueP = _db.ElementItemValues.Where(x => x.ElementItemId == value.ElementItemId & x.DateTime.Year == dt.Year & x.DateTime.Month >= dt.Month).OrderBy(x => x.DateTime).FirstOrDefault();
                        }
                        else if (countCM > 0)
                        {
                            valueP = _db.ElementItemValues.Where(x => x.ElementItemId == value.ElementItemId & x.DateTime.Year == DateTime.Now.Year & x.DateTime.Month >= DateTime.Now.Month).OrderBy(x => x.DateTime).FirstOrDefault();
                        }

                        if (valueP != null)
                        {
                            var count = d - valueP.RawValue.AsDecimal();
                            var elementKey = value.ElementItemId.Replace($"{KeyRoot}.", "");
                            var calcKey = _cfg.GetSection("Plugins").GetSection(KeyRoot).GetSection(elementKey).GetValue<string>("Info");

                            var calcElement = _db.ElementItems.FirstOrDefault(x => x.Id == $"{KeyRoot}.{calcKey}");
                            var calcValue = new ElementItemValueModel
                            {
                                DateTime = DateTime.Now,
                                ElementItemId = calcElement.Id,
                                Updated = DateTime.Now,
                                RawValue = "",
                            };
                            var section = _cfg.GetSection("Plugins").GetSection(KeyRoot).GetSection(calcKey);

                            var limits = section.GetSection("Limits").GetChildren().Select(x => new { Limit = x.Key.AsDecimal(), Price = x.Value.AsDecimal() });
                            var result = 0m;
                            foreach (var limit in limits.OrderBy(x => x.Limit))
                            {
                                if (count > limit.Limit)
                                {
                                    result += limit.Limit * limit.Price;
                                    count -= limit.Limit;
                                }
                                else
                                {
                                    result += count * limit.Price;
                                    break;
                                }
                            }

                            calcValue.RawValue = $"{Math.Round(result, 2).ToString("0.##")} ({Math.Round(d - valueP.RawValue.AsDecimal(), 2).ToString("0.##")})";
                            _proxy.ChangeElementItemValue(calcValue);
                        }
                    }
                }
            }
            catch
            {
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
