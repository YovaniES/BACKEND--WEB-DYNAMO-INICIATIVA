using ConfiguradorModel.Filtro;
using ConfiguradorModel.Model;
using ConfiguradorModel.Model.Base;
using ModelBase.Paginacion;
using System.Collections.Generic;

namespace ConfiguradorBLL.IService
{
    public interface IConfiguradorService
    {
        IEnumerable<object> GetList(FiltroConfig filtro);

        IEnumerable<object> GetCustomList(string filtro);

        IEnumerable<object> GetCustomQuery(string filtros);
        ResultadoPaginado<FiltroConfig, object> GetListPaginated(FiltroConfig filtro);
        void LoadData();
        ResultConfig LoadPage(FiltroConfig filtro);
        ResultConfig ExecuteQuery(FiltroConfig filtro);
        List<ContainerPage> Login(decimal rolId);
        decimal? GetRolId(string rolSistemaId);
        MailNotification ProcessNotification(FiltroConfig filtro);
    }
}
