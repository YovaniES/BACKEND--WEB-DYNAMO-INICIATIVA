using BLLBase;
using ConfiguradorBLL.IService;
using ConfiguradorModel.Exceptions;
using ConfiguradorModel.Filtro;
using ConfiguradorModel.Model;
using ConfiguradorModel.Model.Auth;
using ConfiguradorModel.Model.Base;
using ConfiguradorModel.Model.Types;
using ConfiguradorUtil.Service;
using ModelBase.Paginacion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguradorBLL.Service
{
    public class ConfiguradorService : ServiceBase<ConfiguradorService>, IConfiguradorService 
    {
        private ConfiguradorCacheService cacheUtil;
        public ConfiguradorService(IServiceProvider provider, ConfiguradorCacheService cacheUtil) : base(provider)
        {
            this.cacheUtil = cacheUtil;
        }
        public ResultadoPaginado<FiltroConfig, object> GetListPaginated(FiltroConfig filtro) { 
            return this.GetUnitOfWork().GetUtilRepository().GetListPaginated(GetQuerySql(filtro.QueryId), filtro);
        }

        public IEnumerable<object> GetList(FiltroConfig filtro)
        {
            return this.GetUnitOfWork().GetUtilRepository().GetList(GetQuerySql(filtro.QueryId), filtro);
        }

        public IEnumerable<object> GetCustomList(string filtro)
        {
            string[] _filtro;
            _filtro = filtro.Split("_");

            Dictionary<string, string> openWith =  new Dictionary<string, string>();
            openWith.Add("PAR_" + _filtro[1].ToUpper(), _filtro[2]);

            FiltroConfig _filter = new FiltroConfig();
            _filter.MapValue = openWith;
            return this.GetUnitOfWork().GetUtilRepository().GetList(GetQuerySql(Convert.ToDecimal(_filtro[0])), _filter);
        }

        public IEnumerable<object> GetCustomQuery(string filtros)
        {
            string[] _filtro;
            _filtro = filtros.Split("-");

            Dictionary<string, string> openWith = new Dictionary<string, string>();
            openWith.Add(_filtro[1], _filtro[2]);

            FiltroConfig _filter = new FiltroConfig();
            _filter.MapValue = openWith;
            return this.GetUnitOfWork().GetUtilRepository().GetList(GetQuerySql(Convert.ToDecimal(_filtro[0])), _filter);
        }

        public object GetObject(FiltroConfig filtro)
        {
            return this.GetUnitOfWork().GetUtilRepository().GetObject(GetQuerySql(filtro.QueryId), filtro);
        }
        public decimal? GetDecimal(FiltroConfig filtro)
        {
            return this.GetUnitOfWork().GetUtilRepository().GetDecimal(GetQuerySql(filtro.QueryId), filtro);
        }
        public string GetString(FiltroConfig filtro)
        {
            return this.GetUnitOfWork().GetUtilRepository().GetString(GetQuerySql(filtro.QueryId), filtro);
        }
        public void LoadData()
        {
            cacheUtil.LoadData();
        }
        public ResultConfig LoadPage(FiltroConfig filtro)
        {
            decimal rolId = (decimal)filtro.RolId;
            decimal pageId = (decimal)filtro.PageId;
            //preparamos la pagina segun el modo, si no hay acceso lanza excepcion
            var page = cacheUtil.ProccessMode(rolId, pageId, GetPageModeIdByQuery(filtro, pageId));
            //preparamos data si tiene dataQuery
            var result = new ResultConfig() { Page = page };
            ProcessLoadData(page, filtro);
            return result;
        }

        private void ProcessLoadData(ContainerPage page, FiltroConfig filtro)
        {
            if (page.Components != null){
                foreach(var comp in page.Components){
                    ProcessLoadDataComponent((BasePage)comp, filtro);
                }
            }
        }
        private void ProcessLoadDataComponent(BasePage component, FiltroConfig filtro)
        {
            if (component.Type == 2){
                var page = (DetailPage)component;
                if (page.Form != null && ValidateExecuteQueryParam(page.Form, filtro)){
                    try
                    {
                        page.InitialValue = GetObject(new FiltroConfig() { QueryId = page.Form.DataQueryId, MapValue = filtro.MapValue });
                    }
                    catch (Exception ex)
                    {
                        throw new ConfigException("Problemas ejecutando dataQuery", ex.Message);
                    }
                }
            }
            
        }


        private decimal? GetPageModeIdByQuery(FiltroConfig filtro, decimal pageId)
        {
            var page = cacheUtil.GetPage(pageId);
            decimal? modeId=null;
            if (page.ModeQueryId != null){ 
                try
                {
                    modeId = (decimal)GetDecimal(new FiltroConfig() { QueryId = page.ModeQueryId, MapValue = filtro.MapValue });
                    modeId = modeId == 0 ? 1 : modeId;//si el query retorna nulo vamos al modo por defecto 1
                }
                catch (Exception ex)
                {
                    throw new ConfigException("Problemas ejecutando modeQuery", ex.Message);
                }
            }
            return modeId;
        }
        



        

        private Boolean ValidateExecuteQueryParam(FormComp form, FiltroConfig filtro)
        {
            if (form.DataQueryId == null){
                return false;
            }
            if ((form.DataParams == null || form.DataParams.Count == 0)){
                return true;
            }
            if (filtro.Params == null || filtro.Params.Count == 0){
                return false;
            }
            if (filtro.Params.Count != form.DataParams.Count){
                return false;
            }
            foreach (var par in form.DataParams){
                if (!filtro.Params.ContainsKey(par))
                    return false;
            }
            return true;
        }

        public List<ContainerPage> Login(decimal rolId)
        {
            return cacheUtil.Login(rolId);
        }
        public decimal? GetRolId(string rolSistemaId)
        {
            return cacheUtil.GetRolId(rolSistemaId);
        }
        public ResultConfig ExecuteQuery(FiltroConfig filtro)
        {
            var q = GetQuery(filtro.QueryId);
            if (q.Type > 6){
                throw new ConfigException($"No existe el tipo de query");
            }
            var r = this.GetUnitOfWork().GetUtilRepository().ExecuteQuery(q.Sql, q.Type, filtro);
            if (filtro.Compress && string.IsNullOrEmpty(r.ErrorMessage) && string.IsNullOrEmpty(r.InternalError))
            {
                var textData = JsonConvert.SerializeObject(r);
                //string compress = null; LZString.LzString.Compress(textData);  
                r = new ResultConfig() { CompressData = LZString.LzString.Compress(textData) };
            }
            return r;
        }

        private Query GetQuery(decimal? queryId)
        {
            if (queryId == null){
                throw new ConfigException($"El queryId no puede ser nulo");
            }
            var q = cacheUtil.GetQuery((decimal)queryId);
            if (q == null){
                throw new ConfigException($"El query solicitado no existe => [queryId: @{queryId}]");
            }
            else if (string.IsNullOrEmpty(q.Sql)){
                throw new ConfigException($"El query solicitado esta vacio => [queryId: @{queryId}]");
            }
            return q;
        }
        private string GetQuerySql(decimal? queryId){
            return GetQuery(queryId).Sql;
        }


        public MailNotification ProcessNotification(FiltroConfig filtro)
        {
            var noti = this.GetUnitOfWork().GetUtilRepository().GetMailNotification(filtro);
            noti.Subject = noti.Subject;
            noti.To = GetString(new FiltroConfig() { QueryId = noti.ToQueryId, MapValue = filtro.MapValue });
            var data = GetObject(new FiltroConfig() { QueryId = noti.DataQueryId, MapValue = filtro.MapValue }) as IDictionary<string, object>;
            if (data != null){
                foreach (var pair in data) {
                    if (pair.Value != null){
                        noti.Body = noti.Body.Replace("@{" + pair.Key + "}", $"{pair.Value}");
                        noti.Subject = noti.Subject.Replace("@{" + pair.Key + "}", $"{pair.Value}");
                    }
                }
            }
            return noti;
        }

        
    }
}
