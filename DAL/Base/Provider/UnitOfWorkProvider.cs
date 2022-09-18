using DAL.UnitOfWork;
using IDALBase.DbContext;

namespace DAL.Base.Provider
{
    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        private IDbContextFactory _dbContextFactory;
        private IUnitOfWork _unitOfWork;

        public UnitOfWorkProvider(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IUnitOfWork GetNewUnitOfWork()
        {
            return new DAL.UnitOfWork.UnitOfWork(_dbContextFactory.GetDbContext());
        }

        public IUnitOfWork GetUnitOfWork()
        {
            if (_unitOfWork == null)
            {   //crea una unidad de trabajo donde todos compartiran el contexto
                _unitOfWork = new DAL.UnitOfWork.UnitOfWork(_dbContextFactory);
            }
            return _unitOfWork;
        }
    }
}
