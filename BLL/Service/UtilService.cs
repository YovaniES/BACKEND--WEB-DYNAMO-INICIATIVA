using BLLBase;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class UtilService : ServiceBase<UtilService>, IUtilService
{
        public UtilService(IServiceProvider provider) : base(provider)
        {
            
        }
        public string GetFechaBD()
        {
            return this.GetUnitOfWork().GetUtilRepository().GetFechaBD();
        }
        public IEnumerable<object> GetQuery(int tipo, Dictionary<string, string> filtro)
        {
            return this.GetUnitOfWork().GetUtilRepository().GetQuery(tipo, filtro);
        }
    }
}
