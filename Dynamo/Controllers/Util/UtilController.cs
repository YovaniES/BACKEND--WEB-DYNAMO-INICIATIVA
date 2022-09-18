using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelBase.Exceptions;
using System;
using System.Collections.Generic;
using WebBase.Controllers.Base;

namespace Dynamo.Controllers.Util
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class UtilController : BaseController<UtilController>
    {
        private readonly IUtilService _utilService;
        public UtilController(IServiceProvider serviceProvider, IUtilService utilService) : base(serviceProvider)
        {
            _utilService = utilService;
        }
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
        }
        [HttpGet]
        public IEnumerable<object> GetProyecto()
        {
            return _utilService.GetQuery(1, null);
        }
        [HttpGet]
        public IEnumerable<object> GetCasoPrueba()
        {
            return _utilService.GetQuery(2, null);
        }
        [HttpGet]
        public IEnumerable<object> GetHallazgo()
        {
            return _utilService.GetQuery(3, null);
        }

    }
}
