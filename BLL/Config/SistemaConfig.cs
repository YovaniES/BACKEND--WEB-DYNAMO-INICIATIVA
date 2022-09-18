using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BLL.Config
{
    public class SistemaConfig
    {
        public static void AddInConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            Dictionary<string, Type> mapServices = Assembly.Load("BLL").GetTypes().Where(x => !x.IsAbstract && !x.IsInterface && x.Name.LastIndexOf("Service") > 0).ToDictionary(x => x.Name, x => x);
            Dictionary<string, Type> mapIServices = Assembly.Load("BLL").GetTypes().Where(x => x.IsInterface && x.Name.LastIndexOf("Service") > 0).ToDictionary(x => x.Name, x => x);
            foreach (string key in mapServices.Keys)
            {
                services.AddTransient(mapIServices["I" + key], mapServices[key]);
            }
        }
    }
}
