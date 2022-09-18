using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAL.Base.Provider;
using DALBaseMySql.DbContext;

namespace Dynamo.Config
{
    public class SistemaConfig
    {
        public static void AddInConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            ConfiguradorBLL.Config.SistemaConfig.AddInConfigureServices(configuration, services);
            ConfiguradorUtil.Config.SistemaConfig.AddInConfigureServices(configuration, services);
            BLL.Config.SistemaConfig.AddInConfigureServices(configuration, services);
            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUnitOfWorkProvider>(new UnitOfWorkProvider(new DbContextFactory(configuration.GetConnectionString("DefaultConnection"))));
            services.AddMvc();
        }

        public static void AddInConfigure(IApplicationBuilder app)
        {

        }
    }
}
