using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MyHome.Shared;
using System.Runtime.Loader;
using System.Reflection;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Hangfire;
using Microsoft.Extensions.Configuration;

namespace MyHome.Core.Plugin
{
    public class PluginManager
    {      
        IServiceProvider _sp;
        IConfigurationRoot _cfg;

        public PluginManager(IServiceProvider sp, IConfigurationRoot cfg)
        {
            _sp = sp;
            _cfg = cfg;
        }

        public List<Type> Load()
        {
            var _plugins = new List<Type>();

            var root = new System.IO.FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory;
            var plugins = _cfg.GetSection("Plugins").GetValue<string>("Folder");

            var copyFrom = _cfg.GetSection("Plugins").GetSection("CopyFrom");
            if (copyFrom != null)
            { 
                var copyList = copyFrom.GetChildren().ToList();
                foreach(var item in copyList)
                {
                    if (System.IO.File.Exists(item.Value))
                    {
                        var fi = new FileInfo(item.Value);
                        System.IO.File.Copy(item.Value, String.Format("{0}\\{1}\\{2}", root, plugins, fi.Name), true);
                    }
                }
            }
            
            if (!System.IO.Directory.Exists(String.Format("{0}\\{1}", root, plugins)))
            {
                System.IO.Directory.CreateDirectory(String.Format("{0}\\{1}", root, plugins));
            }
            var files = Directory.GetFiles(String.Format("{0}\\{1}", root, plugins), "*.dll");

            foreach (var file in files)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                    foreach (var type in assembly.GetTypes())
                    {
                        if ((typeof(IPlugin).GetTypeInfo().IsAssignableFrom(type)) & (!type.GetTypeInfo().IsInterface))
                        {
                            _plugins.Add(type);
                        }
                    }
                }
                catch
                {

                }
            }

            return _plugins;
        }
    }
}
