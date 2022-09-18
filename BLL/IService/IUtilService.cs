using ModelBase.Paginacion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public interface IUtilService
    {
        string GetFechaBD();
        IEnumerable<object> GetQuery(int tipo, Dictionary<string, string> filtro);
    }
}
