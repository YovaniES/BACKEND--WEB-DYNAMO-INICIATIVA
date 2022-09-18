using DAL.Util;
using DALBase.UnitOfWork;
using IDALBase.DbContext;
using IDALBase.UnitOfWork;
using System;
using System.Data;

namespace DAL.UnitOfWork
{
    public partial interface IUnitOfWork : IUnitOfWorkBase, IDisposable
    {
        IUtilRepository GetUtilRepository();
    }

    public partial class UnitOfWork : UnitOfWorkBase, IUnitOfWork
    {
        public UnitOfWork(IDbContextFactory dbContextFactory) : base(dbContextFactory) { }
        public UnitOfWork(IDbConnection dbContext) : base(dbContext) { }
        private IUtilRepository _utilRepository;

        public IUtilRepository GetUtilRepository()
        {
            if (_utilRepository == null)
            {
                _utilRepository = new UtilRepository(_dbContextFactory, _dbContext);
            }
            return _utilRepository;
        }

    }
	
}