using DAL.Base.Provider;
using DAL.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace BLLBase
{
    public abstract class ServiceBase<T>
    {
        private IUnitOfWorkProvider _uwp;  
        private readonly ILogger _log;

        public ServiceBase(IServiceProvider provider)
        {
            this._uwp = provider.GetRequiredService<IUnitOfWorkProvider>();
            this._log = provider.GetRequiredService<ILogger<T>>();
        }

        protected IUnitOfWork GetUnitOfWork()
        {
            return _uwp.GetUnitOfWork();
        }

        protected IUnitOfWork GetNewUnitOfWork()
        {
            return _uwp.GetNewUnitOfWork();
        }
        protected void LogInformation(string message)
        {

            _log.LogInformation(message);
        }

        protected void LogDebug(string message)
        {
            _log.LogDebug(message);
        }
        protected void LogError(string message)
        {
            _log.LogError(message);
        }
    }
}
