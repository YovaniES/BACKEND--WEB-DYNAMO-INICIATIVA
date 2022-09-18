using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Base.Provider
{
    public interface IUnitOfWorkProvider
    {
        IUnitOfWork GetNewUnitOfWork();
        IUnitOfWork GetUnitOfWork();
    }
}
