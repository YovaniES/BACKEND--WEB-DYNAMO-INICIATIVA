using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Mvc;
using Util.Fecha;
using Util.File;
using WebBase.Controllers.Base;

namespace Dynamo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : BaseController<TestController>
    {
        private readonly IUtilService _utilService;
        public TestController(IServiceProvider serviceProvider, IUtilService utilService): base(serviceProvider)
        {
            _utilService = utilService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            string fecha = "";
            try
            {
                fecha = this._utilService.GetFechaBD();
            }
            catch (Exception ex)
            {
                fecha = ex.Message;
            }
            LogInformation("testetando");
            return new string[] { " Fecha Sistema: " + DateTime.Now, " Fecha Base de Datos: " + fecha};
        }


        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            

            if (id == "log")
            {
                string rutaLog = this.GetConfiguration()["Logging:PathFormat"].Replace("{Date}", DateUtil.NowToYYYYMMDD());
                //rutaLog = string.Format(rutaLog,"nanaaa");
                LogInformation(rutaLog);
                return FileUtil.LeerTextoEnUso(rutaLog);

            }
            else if (id == "error")
            {
                throw new Exception("Probando Error");
            }

            return id;
        }

    }
}
