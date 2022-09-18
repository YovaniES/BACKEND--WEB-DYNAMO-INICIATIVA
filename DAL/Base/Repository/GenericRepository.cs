using IDALBase.DbContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DAL.Base.Repository
{
    public class GenericRepository<TEntity> : DALBaseMySql.Repository.GenericRepository<TEntity> where TEntity : class
    {
        public GenericRepository(IDbContextFactory dbFactory) : base(dbFactory)
        {
             
        }

        public GenericRepository(IDbConnection dbContext) : base(dbContext)
        {
        }

        public GenericRepository(IDbContextFactory dbFactory, IDbConnection dbContext) : base(dbFactory, dbContext)
        {
        }
    }
}
