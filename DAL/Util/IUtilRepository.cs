using ConfiguradorModel.Filtro;
using ConfiguradorModel.Model;
using ConfiguradorModel.Model.Base;
using IDALBase.Repository;
using ModelBase.Paginacion;
using System.Collections;
using System.Collections.Generic;

namespace DAL.Util
{
    public interface IUtilRepository : IGenericRepository<object>
    {
        string GetFechaBD();
        IEnumerable<object> GetQuery(int tipo, Dictionary<string, string> filtro);
        ResultadoPaginado<FiltroConfig, object> GetListPaginated(string sql, FiltroConfig filtro);
        IEnumerable<object> GetList(string sql, FiltroConfig filtro);
        object GetObject(string sql, FiltroConfig filtro);
        decimal GetDecimal(string sql, FiltroConfig filtro);
        string GetString(string sql, FiltroConfig filtro);
        ResultConfig ExecuteQuery(string sql, int type, FiltroConfig filtro);
        MailNotification GetMailNotification(FiltroConfig filtro);
    }
}
