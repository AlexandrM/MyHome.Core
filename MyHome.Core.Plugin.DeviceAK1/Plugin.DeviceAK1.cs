using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using MyHome.Shared;

namespace MyHome.Core.Plugin.DeviceAK1
{
    /*
    There are device with output
    [0,4,21.5C]
    [1,3,1.8C,2573V]
    [2,7,17.3C,3058V]
    Heater is OFF. Turn ON
    AutoMode is ON. Turn OFF
    AutoMode range(C)=21.1,21.5

    Element1 [0,4,21.5C] - ID,LastUpdate,Temperature (2 level of house)
    Element1 Heater is OFF. Turn ON - GAS HeaterState
    Element1 AutoMode is ON. Turn OFF - AutoModeState
    Element1 AutoMode range(C)=21.1,21.5 - AutoMode values MIN,MAX (change HeaterState to ON),(change HeaterState to OFF)
    Element2 [1,3,1.8C,2573V] - ID,LastUpdate,Temperature,BatteryState (street)
    Element3 [2,7,17.3C,3058V] - ID,LastUpdate,Temperature,BatteryState (1 level of house)
    */
    public class Plugin : IPlugin
    {
        #region IDS

        /// <summary>
        /// Element1 (Коробка)
        /// </summary>
        public static string gElement0 = "DeviceAK1.0";
        /// <summary>
        /// Element1 Temperature (Температура коробка)
        /// </summary>
        public static string gElement0IndicatorT = "DeviceAK1.0T";

        /// <summary>
        /// Element1 HeaterState (Включен выключен)
        /// </summary>
        public static string gElement0ManageOnOff = "DeviceAK1.0E";
        /// <summary>
        /// Element1 HeaterState On (Включен)
        /// </summary>
        public static string gElement0ManageOn = "DeviceAK1.0EOn";
        /// <summary>
        /// Element HeaterState Off (Выключен)
        /// </summary>
        public static string gElement0ManageOff = "DeviceAK1.0EOff";

        /// <summary>
        /// Element1 AutoMode (Авторежим)
        /// </summary>
        public static string gElement0ManageAuto = "DeviceAK1.0A";
        /// <summary>
        /// Element1 AutoMode On (Включен)
        /// </summary>
        public static string gElement0ManageAutoOn = "DeviceAK1.0AOn";
        /// <summary>
        /// Element1 AutoMode Off (Выключен)
        /// </summary>
        public static string gElement0ManageAutoOff = "DeviceAK1.0AOff";

        /// <summary>
        /// Element1 AutoMode MIN (Минимальная температура)
        /// </summary>
        public static string gElement0ManageAutoMin = "DeviceAK1.0Min";
        /// <summary>
        /// Element1 AutoMode MAX (Максимальная температура)
        /// </summary>
        public static string gElement0ManageAutoMax = "DeviceAK1.0Max";

        /// <summary>
        /// Element2 (Датчик улица)
        /// </summary>
        public static string gElement1 = "DeviceAK1.1";
        /// <summary>
        /// Element2 Temperature (Температура улица)
        /// </summary>
        public static string gElement1IndicatorT = "DeviceAK1.1T";
        /// <summary>
        /// Element2 BatteryState (Батарейка улица)
        /// </summary>
        public static string gElement1IndicatorB = "DeviceAK1.1B";

        /// <summary>
        /// Element3 (Датчик 2 этаж)
        /// </summary>
        public static string gElement2 = "DeviceAK1.2";
        /// <summary>
        /// Element3 Temperature (Температура 2 этаж)
        /// </summary>
        public static string gElement2IndicatorT = "DeviceAK1.2T";
        /// <summary>
        /// Element3 BatteryState (Батарейка 2 этаж)
        /// </summary>
        public static string gElement2IndicatorB = "DeviceAK1.2B";

        #endregion IDS

        IConfigurationRoot _cfg;
        ISignalRProxy _proxy;
        DB _db;

        public Plugin(IConfigurationRoot cfg, ISignalRProxy proxy, DB db)
        {
            _cfg = cfg;
            _proxy = proxy;
            _db = db;

            Interval = new TimeSpan(0, 10, 0);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// On plugin load fill database
        /// </summary>
        public async Task<bool> Init()
        {
            try
            {
                ElementModel e;

                e = _db.Elements.FirstOrDefault(x => x.ExternalId == "MyHome.Service.DeviceAK1.0");
                if (e == null)
                {
                    _db.Elements.Add(new ElementModel
                    {
                        Id = gElement0,
                        ExternalId = "MyHome.Service.DeviceAK1.0",
                        Name = "2 этаж",
                        Items = new List<ElementItemModel> {
                        new ElementItemModel
                        {
                            Id = gElement0IndicatorT,
                            ElementId = gElement0,
                            Name = "Температура",
                            Description = "Температура на 2 этаже",
                            Type = ElementItemType.Indicator,
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                        new ElementItemModel
                        {
                            Id = gElement0ManageOnOff,
                            ElementId = gElement0,
                            Name = "Состояние",
                            Description = "Состояние котла (вкл/выкл)",
                            IsEnum = true,
                            Type = ElementItemType.Manage,
                            AllowSchedule = true,
                            EnumValues = new List<ElementItemEnumModel>() {
                                new ElementItemEnumModel { Id = gElement0ManageOn, ElementItemId = gElement0ManageOnOff, Value = "on", Name = "Включен" },
                                new ElementItemEnumModel { Id = gElement0ManageOff, ElementItemId = gElement0ManageOnOff, Value = "off", Name = "Выключен" },
                            },
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                        new ElementItemModel
                        {
                            Id = gElement0ManageAuto, ElementId
                            = gElement0,
                            Name = "Авторежим",
                            Description = "Автоматический режим",
                            IsEnum = true,
                            Type = ElementItemType.Manage,
                            AllowSchedule = true,
                            EnumValues = new List<ElementItemEnumModel>() {
                                new ElementItemEnumModel { Id = gElement0ManageAutoOn, ElementItemId = gElement0ManageOnOff, Value = "on", Name = "Включен" },
                                new ElementItemEnumModel { Id = gElement0ManageAutoOff, ElementItemId = gElement0ManageOnOff, Value = "off", Name = "Выключен" },
                            },
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },

                        new ElementItemModel
                        {
                            Id = gElement0ManageAutoMin,
                            ElementId = gElement0,
                            Name = "Минимальная",
                            Description = "Минимальная темепература",
                            IsEnum = false,
                            Type = ElementItemType.Manage,
                            AllowSchedule = true,
                            EnumValues = new List<ElementItemEnumModel>() {
                            },
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },

                        new ElementItemModel {
                            Id = gElement0ManageAutoMax,
                            ElementId = gElement0,
                            Name = "Максимальная",
                            Description = "Максимальная темепература",
                            IsEnum = false,
                            Type = ElementItemType.Manage,
                            AllowSchedule = true,
                            EnumValues = new List<ElementItemEnumModel>() {
                            },
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                    }
                    });
                }

                e = _db.Elements.FirstOrDefault(x => x.ExternalId == "MyHome.Service.DeviceAK1.2");
                if (e == null)
                {
                    _db.Elements.Add(new ElementModel
                    {
                        Id = gElement2,
                        ExternalId = "MyHome.Service.DeviceAK1.2",
                        Name = "1 этаж",
                        Items = new List<ElementItemModel> {
                        new ElementItemModel {
                            Id = gElement2IndicatorT,
                            ElementId = gElement2,
                            Name = "Температура",
                            Description = "Температура на 1 этаже",
                            Type = ElementItemType.Indicator,
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                        new ElementItemModel {
                            Id = gElement2IndicatorB,
                            ElementId = gElement2,
                            Name = "Батарея",
                            Description = "Заряд батареи на 1 этаже",
                            Type = ElementItemType.Indicator,
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                    }
                    });
                }

                e = _db.Elements.FirstOrDefault(x => x.ExternalId == "MyHome.Service.DeviceAK1.1");
                if (e == null)
                {
                    _db.Elements.Add(new ElementModel
                    {
                        Id = gElement1,
                        ExternalId = "MyHome.Service.DeviceAK1.1",
                        Name = "Улица",
                        Items = new List<ElementItemModel> {
                        new ElementItemModel {
                            Id = gElement1IndicatorT,
                            ElementId = gElement1,
                            Name = "Температура",
                            Description = "Температура на улице",
                            Type = ElementItemType.Indicator,
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                        new ElementItemModel {
                            Id = gElement1IndicatorB,
                            ElementId = gElement1,
                            Name = "Батарея",
                            Description = "Заряд батареи на улице",
                            Type = ElementItemType.Indicator,
                            RefreshTime = System.Convert.ToInt32(Interval.TotalSeconds),
                        },
                    }
                    });
                }

                _db.SaveChanges();
            }
            catch (Exception exc)
            {
                Log.L("Exception MyHome.Service.DeviceAK1.Init(): " + exc.Message);
            }

            return true;
        }

        public TimeSpan Interval { get; set; }

        Dictionary<string, ElementItemValueModel> elementItemValues = new Dictionary<string, ElementItemValueModel>()
        {
            { "0", new ElementItemValueModel { ElementItemId = gElement0IndicatorT, RawValue = "-9999" } },

            { "1", new ElementItemValueModel { ElementItemId = gElement1IndicatorT, RawValue = "-9999" } },
            { "11", new ElementItemValueModel { ElementItemId = gElement1IndicatorB, RawValue = "-9999" } },

            { "2", new ElementItemValueModel { ElementItemId = gElement2IndicatorT, RawValue = "-9999" } },
            { "22", new ElementItemValueModel { ElementItemId = gElement2IndicatorB, RawValue = "-9999" } },

            { "00", new ElementItemValueModel { ElementItemId = gElement0ManageOnOff, ValueId = null } },

            { "000", new ElementItemValueModel { ElementItemId = gElement0ManageAuto, ValueId = null } },

            { "0001", new ElementItemValueModel { ElementItemId = gElement0ManageAutoMin, RawValue = "-9999" } },
            { "0002", new ElementItemValueModel { ElementItemId = gElement0ManageAutoMax, RawValue = "-9999"  } },
        };

        public async Task<bool> Process()
        {
            try
            {
                //< html >< body >[0, 12, 25.7C] < br >[1, 0, 27.1C, 2226V] < br >[2, 1, 24.1C, 2598V] < br > Heater is OFF. < a href = '/?heater=on' > Turn ON </ a >< br > AutoMode is OFF. < a href = '/?automode=on' > Turn ON </ a >< br >
                using (var wc = new HttpClient())
                {
                    DateTime now = DateTime.Now;

                    var url = _cfg.GetSection("Plugins").GetSection("MyHome.Core.Plugin.DeviceAK1").GetValue<string>("Url");
                    var content = await wc.GetAsync(url);
                    var html = await content.Content.ReadAsStringAsync();

                    //Term and battery
                    while (true)
                    {
                        int p1 = html.IndexOf("[");
                        int p2 = html.IndexOf("]");
                        if ((p1 == -1) | (p2 == -1))
                        {
                            break;
                        }

                        var si = html.Substring(p1 + 1, p2 - p1 - 1);
                        html = html.Substring(p2 + 1);
                        if (si.ToLower().IndexOf("c") == -1)
                        {
                            continue;
                        }
                        var sis = si.Split(',');

                        ElementItemValueModel ElementIndicatorValueT = elementItemValues[sis[0]];
                        ElementItemValueModel ElementIndicatorValueB = sis.Length > 3 ? elementItemValues[sis[0] + sis[0]] : null;

                        var t = sis[2].ToLower().Replace("c", "").Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                        decimal td = decimal.Parse(t);

                        var b = sis.Length > 3 ? sis[3].ToLower().Replace("v", "") : "0";
                        long bl = long.Parse(b);

                        ElementIndicatorValueT.RawValue = td.ToString();
                        ElementIndicatorValueT.DateTime = now;
                        _proxy.ChangeElementItemValue(ElementIndicatorValueT);

                        if (ElementIndicatorValueB != null)
                        {
                            ElementIndicatorValueB.RawValue = bl.ToString();
                            ElementIndicatorValueB.DateTime = now;
                            _proxy.ChangeElementItemValue(ElementIndicatorValueB);
                        }
                    }

                    ElementItemValueModel ElementItemValue;
                    string val;

                    //On Off
                    ElementItemValue = elementItemValues["00"];
                    val = gElement0ManageOn;
                    if (html.ToLower().IndexOf("/?heater=on") != -1)
                    {
                        val = gElement0ManageOff;
                    }
                    ElementItemValue.ValueId = val;
                    ElementItemValue.DateTime = now;
                    _proxy.ChangeElementItemValue(ElementItemValue);

                    //Auto mode
                    ElementItemValue = elementItemValues["000"];
                    val = gElement0ManageAutoOn;
                    if (html.ToLower().IndexOf("/?automode=on") != -1)
                    {
                        val = gElement0ManageAutoOff;
                    }
                    ElementItemValue.ValueId = val;
                    ElementItemValue.DateTime = now;
                    _proxy.ChangeElementItemValue(ElementItemValue);

                    //AutoMin
                    //AutoMode range(C) = 21.3, 21.6
                    int p = html.ToLower().IndexOf("range(c)=");
                    if (p != -1)
                    {
                        string ds;
                        decimal d;
                        string[] minmax = html.Substring(p + 9).Split(',');

                        ElementItemValue = elementItemValues["0001"];
                        ds = minmax[0].ToLower().Replace("c", "").Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                        d = decimal.Parse(ds);
                        ElementItemValue.RawValue = d.ToString();
                        ElementItemValue.DateTime = now;
                        _proxy.ChangeElementItemValue(ElementItemValue);

                        ElementItemValue = elementItemValues["0002"];
                        ds = minmax[1].ToLower().Replace("c", "").Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                        d = decimal.Parse(ds);
                        ElementItemValue.RawValue = d.ToString();
                        ElementItemValue.DateTime = now;
                        _proxy.ChangeElementItemValue(ElementItemValue);
                    }
                }
            }
            catch (Exception exc)
            {
                Log.L("Exception MyHome.Service.DeviceAK1.Process(): " + exc.Message);
            }

            return true;
        }

        /// <summary>
        /// Change value from WEB or Scheduler
        /// </summary>
        /// <param name="value">New value</param>
        /// <param name="oldValue">Old value</param>
        /// <returns></returns>
        public async Task<bool> ChangeValue(ElementItemValueModel value, ElementItemValueModel oldValue)
        {
            try
            {
                using (var wc = new HttpClient())
                {
                    var url = _cfg.GetSection("Plugins").GetSection("MyHome.Core.Plugin.DeviceAK1").GetValue<string>("Url");
                    if (value.ElementItemId == gElement0ManageOnOff)
                    {
                        if ((value.ValueId == gElement0ManageOn) & (oldValue == null || value.ValueId == gElement0ManageOff))
                        {
                            var resp = await wc.GetAsync(String.Format("{0}/?heater=on", url));
                            if (resp.IsSuccessStatusCode)
                            {
                                return await Process();
                            }
                        }
                        if ((value.ValueId == gElement0ManageOff) & (oldValue == null || value.ValueId == gElement0ManageOn))
                        {
                            var resp = await wc.GetAsync(String.Format("{0}/?heater=off", url));
                            if (resp.IsSuccessStatusCode)
                            {
                                return await Process();
                            }
                        }
                    }

                    if (value.ElementItemId == gElement0ManageAuto)
                    {
                        if ((value.ValueId == gElement0ManageAutoOn) & (oldValue == null || oldValue.ValueId == gElement0ManageAutoOff))
                        {
                            var resp = await wc.GetAsync(String.Format("{0}/?automode=on", url));
                            if (resp.IsSuccessStatusCode)
                            {
                                return await Process();
                            }
                        }
                        if ((value.ValueId == gElement0ManageAutoOff) & (oldValue == null || oldValue.ValueId == gElement0ManageAutoOn))
                        {
                            var resp = await wc.GetAsync(String.Format("{0}/?automode=off", url));
                            if (resp.IsSuccessStatusCode)
                            {
                                return await Process();
                            }
                        }
                    }

                    if ((value.ElementItemId == gElement0ManageAutoMin) | (value.ElementItemId == gElement0ManageAutoMax))
                    {
                        var mode = "l";
                        if (value.ElementItemId == gElement0ManageAutoMax)
                        {
                            mode = "h";
                        }
                        decimal d1;
                        value.RawValue = value.RawValue
                            .Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)
                            .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

                        if (decimal.TryParse(value.RawValue, out d1))
                        {
                            decimal dnew;
                            decimal dold;
                            decimal.TryParse(value.RawValue, out dnew);
                            if (oldValue != null)
                            {
                                decimal.TryParse(oldValue.RawValue, out dold);
                            }
                            else
                            {
                                dold = dnew + 1;
                            }

                            bool changed = (oldValue == null || dnew != dold);
                            if (changed)
                            {
                                value.RawValue = Math.Round(d1 * 16m, 0m).ToString();

                                var resp = await wc.GetAsync(String.Format("{0}/?tmp{1}={2}", url, mode, value.RawValue.ToString().Replace(",", ".")));
                                if (resp.IsSuccessStatusCode)
                                {
                                    return await Process();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.L("Exception MyHome.Service.DeviceAK1.ChangeValue(): " + exc.Message);
            }

            return false;
        }
    }
}