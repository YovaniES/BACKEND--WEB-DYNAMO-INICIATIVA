using BLL;
using ConfiguradorBLL.IService;
using ConfiguradorModel.Exceptions;
using ConfiguradorModel.Filtro;
using ConfiguradorModel.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelBase.Paginacion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebBase.Controllers.Base;

namespace Dynamo.Controllers.Configurador
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ConfiguradorController : BaseController<ConfiguradorController>
    {
        private readonly IUtilService _utilService;
        private readonly IConfiguradorService configuradorService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _filter = "{queryId: 42, mapValue: {reqId: '2299'}, startRow: 0, endRow: 10}";
        public ConfiguradorController(IServiceProvider serviceProvider, IHttpClientFactory clientFactory, IUtilService utilService, IConfiguradorService configuradorService) : base(serviceProvider)
        {
            _utilService = utilService;
            _clientFactory = clientFactory;
            this.configuradorService = configuradorService;
            
        }
        [HttpPost]
        public ResultadoPaginado<FiltroConfig, object> GetListPaginated([FromBody]FiltroConfig filtro)
        {
            return configuradorService.GetListPaginated(filtro);
        }
        [HttpPost]
        public IEnumerable<object> GetList([FromBody] FiltroConfig filtro)
        {
            return configuradorService.GetList(filtro);
        }

        [HttpGet("{id}")]
        public IEnumerable<object> GetCustomList(string id)
        {
            return configuradorService.GetCustomList(id);
        }

        [HttpGet("{filtros}")]
        public IEnumerable<object> GetCustomQuery(string filtros)
        {
            return configuradorService.GetCustomQuery(filtros);
        }

        [HttpPost]
        public object GetItem([FromBody]FiltroConfig filtro)
        {
            return null;
        }
        [HttpPost]
        public ResultConfig LoadPage([FromBody]FiltroConfig filtro)
        {
            try
            {
                return this.configuradorService.LoadPage(ProcessFilter(filtro));
            }
            catch (Exception ex)
            {
                return new ResultConfig() { InternalError = ex.Message };
            }
        }

        
        public ResultConfig ExecuteQuery([FromBody]FiltroConfig filtro)
        {
            try
            {
                if (filtro.QueryId == null){
                    return new ResultConfig() { InternalError = "QueryId no puede ser nulo" };
                }
                
                var r = this.configuradorService.ExecuteQuery(filtro);
                if (r.ErrorMessage==null && r.InternalError == null && filtro.NotificationId != null)
                {
                    var map = filtro.MapValue ?? new Dictionary<string, string>();
                    map["CONFIG_ID"] = "" + r.ResultId;
                }
                return r;
            }
            catch (Exception ex)
            {
                return new ResultConfig() { InternalError = ex.Message };
            }
        }
        private decimal GetRolIdValido()
        {
            var rolId = this.configuradorService.GetRolId(this.GetRolId());
            if (rolId == null)
            {
                throw new ConfigException("Rol no identificado en el configurador");
            }
            return (decimal)rolId;
        }
        [HttpPost]
        public List<ContainerPage> Login([FromBody]FiltroConfig filtro)
        {
            return this.configuradorService.Login(GetRolIdValido());
        }
        /*
        [HttpGet]
        public IEnumerable<object> GetQuery(Dictionary<string, string> filtro)//[FromBody]Dictionary<string,string> filtro
        {
            if (filtro==null || !filtro.ContainsKey("id"))
            {
                throw new PresentationException("No se encuentra el id del query");
            }
            int idQuery = int.Parse(filtro["id"]);
            filtro.Remove("id");
            return _utilService.GetQuery(idQuery, filtro);
        }*/

        [HttpPost]
        public string ReloadData()
        {
            this.configuradorService.LoadData();
            return "Carga de data exitosa";
        }
        private FiltroConfig ProcessFilter(FiltroConfig filtro)
        {
            filtro.RolId = GetRolIdValido();
            filtro.UserId = GetUserIdDecimal();
            filtro.LoginId = GetLoginIdDecimal();
            return ProcessParamValueFilter(filtro);
        }
        private FiltroConfig ProcessParamValueFilter(FiltroConfig filtro)
        {
            var map = filtro.MapValue ?? new Dictionary<string, string>();
            map["SIS_ROL"] = "" + filtro.RolId;
            map["SIS_LOGIN"] = "" + filtro.LoginId;
            map["SIS_USER"] = "" + filtro.UserId;
            if (filtro.Params != null)
            {
                foreach (var key in filtro.Params.Keys)
                {
                    map["PAR_" + key.ToUpper()] = filtro.Params[key];
                }
            }
            filtro.MapValue = map;
            return filtro;
        }


        private void ProcessNotification(FiltroConfig filtro)
        {
            var not = this.configuradorService.ProcessNotification(filtro);
            //dynamic par = new { to = "mvarlic@gmail.com", subject = not.Subject, body = not.Body };
            //not.To + " <br> " +  //add to body parameter
            PostAsJsonAsync("http://indra.gestorqa.com/migracion/mail", new { to = "ana.rivas@telefonica.com, aayanez@indracompany.com", subject = not.Subject, body =  not.Body });
        }

        private void PostAsJsonAsync(string url, object obj)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(obj));//, Encoding.UTF8, "application/json"
            using (var httpClient = _clientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var r = Task.Run<HttpResponseMessage>(async () => await httpClient.PostAsync(url, httpContent)).Result;
                /*
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                else
                {
                    throw new BllException(response.ReasonPhrase);
                }*/
            }
        }

    }
}
