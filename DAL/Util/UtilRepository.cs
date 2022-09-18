using System.Data;
using System.Collections.Generic;
using IDALBase.DbContext;
using DAL.Base.Repository;
using ModelBase.Paginacion;
using ConfiguradorModel.Filtro;
using ConfiguradorModel.Model;
using ConfiguradorModel.Model.Base;
using System;

namespace DAL.Util
{
    public class UtilRepository : DALBaseMySql.Repository.GenericRepository<object>, IUtilRepository
    {
        public UtilRepository(IDbContextFactory dbFactory, IDbConnection dbContext) : base(dbFactory, dbContext) { }

        public string GetFechaBD()
        {
            return ExecuteEscalar<string>("SELECT NOW()", null);
        }
        public IEnumerable<object> GetQuery(int tipo, Dictionary<string, string> filtro)
        {
            var sql = GetQuerySql(tipo);
            //sql = "select que_itm_sql from query_item where que_itm_id = :Valor ";
            //var param = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                if (filtro!=null && filtro.Count > 0)
                {
                    //foreach (var pair in filtro) param.Add(pair.Key, pair.Value);
                    foreach (var pair in filtro) sql = sql.Replace($":{pair.Key}", $"'{pair.Value}'");
                }
                return Query<object>(sql, null);
            }
            return null;
        }
        private string GetQuerySql(int idQuery)
        {
            string query = @"select que_itm_sql from query_item where que_itm_id=" + idQuery;
            return ExecuteEscalar<string>(query, null);
        }

        public IEnumerable<object> ExecuteQueryList(string sql, Dictionary<string, string> filtro)
        {
            if (!string.IsNullOrWhiteSpace(sql))
            {
                if (filtro != null && filtro.Count > 0)
                {
                    foreach (var pair in filtro) sql = sql.Replace($":{pair.Key}", $"'{pair.Value}'");
                }
                return Query<object>(sql, null);
            }
            return null;
        }

        public IEnumerable<object> GetQuery(string sql, Dictionary<string, string> filtro)
        {
            //sql = "select que_itm_sql from query_item where que_itm_id = :Valor ";
            //var param = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                if (filtro != null && filtro.Count > 0)
                {
                    //foreach (var pair in filtro) param.Add(pair.Key, pair.Value);
                    foreach (var pair in filtro) sql = sql.Replace($":{pair.Key}", $"'{pair.Value}'");
                }
                return Query<object>(sql, null);
            }
            return null;
        }
        private string SqlReplaceKeysValue(string sql, FiltroConfig filtro)
        {
            if (filtro?.MapValue?.Count > 0)
                foreach (var pair in filtro.MapValue){
                    
                    if (!string.IsNullOrEmpty(pair.Value)){    /*solo se reemplazan los campos distintos a nulo*/
                        if (pair.Key.Equals("offset")) 
                        {
                            sql = sql.Replace($"@{pair.Key}", $"{pair.Value}");
                        }
                        else
                        {
                            sql = sql.Replace($"@{pair.Key}", $"'{pair.Value}'");
                    }
                    }
                }
            return sql;
        }
        public ResultadoPaginado<FiltroConfig, object> GetListPaginated(string sql, FiltroConfig filtro)
        {
            if (filtro != null)
            {
                sql = SqlReplaceKeysValue(sql, filtro);
                filtro.MapValue = null;
            }
            return QueryPaginationCompleto<FiltroConfig, object>(sql, filtro);
        }
        public IEnumerable<object> GetList(string sql, FiltroConfig filtro)
        {
            sql = SqlReplaceKeysValue(sql, filtro);
            return Query<object>(sql, filtro);
        }
        public object GetObject(string sql, FiltroConfig filtro)
        {
            sql = SqlReplaceKeysValue(sql, filtro);
            return QueryFirst<object>(sql, filtro);
        }
        public decimal GetDecimal(string sql, FiltroConfig filtro)
        {
            sql = SqlReplaceKeysValue(sql, filtro);
            return ExecuteEscalar<decimal>(sql, filtro);
        }
        public string GetString(string sql, FiltroConfig filtro)
        {
            sql = SqlReplaceKeysValue(sql, filtro);
            return ExecuteEscalar<string>(sql, filtro);
        }


        public ResultConfig ExecuteQuery(string sql, int type, FiltroConfig filtro)
        {
            sql = SqlReplaceKeysValue(sql, filtro);
            Console.WriteLine("----------SQL-----------------");
            Console.WriteLine(sql);
            if (type == 0 || type == 1){/*lista*/
                return ExecuteQuerySelectList(sql, filtro);
            }
            else if (type == 2){/*element*/
                return ExecuteQuerySelectElement(sql, filtro);
            }
            else if (type == 3){/*scalar*/
                return ExecuteQuerySelectScalar(sql, filtro);
            }
            else if (type == 4){/*insert*/
                return ExecuteQueryInsert(sql, filtro);
            }
            else if (type == 5){/*UPDATE*/
                return ExecuteQueryUpdate(sql, filtro);
            }
            else if (type == 6){/*DELETE*/
                return ExecuteQueryDelete(sql, filtro);
            }
            return null;
        }


        private ResultConfig ExecuteQuerySelectScalar(string sql, FiltroConfig filtro)
        {
            var result = new ResultConfig() { MapValue = ExecuteEscalar<object>(sql, filtro) };
            return result;
        }

        private ResultConfig ExecuteQuerySelectElement(string sql, FiltroConfig filtro)
        {
            var result = new ResultConfig() { MapValue = QueryFirst<object>(sql, filtro) };
            return result;
        }
        private ResultConfig ExecuteQuerySelectList(string sql, FiltroConfig filtro)
        {
            var result = new ResultConfig() { List = Query<object>(sql, filtro) };
            return result;
        }
        private ResultConfig ExecuteQueryInsert(string sql, FiltroConfig filtro)
        {
            //Execute(sql, filtro);
            var r = QueryFirst<dynamic>(sql + "; SELECT @CONFIG_OUT_ID resultId, @CONFIG_OUT_MSG_ERROR errorMessage, @CONFIG_OUT_MSG_EXITO exitoMessage", filtro);
            var result = new ResultConfig() { ResultId = r.resultId, ErrorMessage = r.errorMessage, ExitoMessage = r.exitoMessage };
            return result;
        }
        private ResultConfig ExecuteQueryUpdate(string sql, FiltroConfig filtro)
        {
            var r = QueryFirst<dynamic>(sql + "; SELECT @CONFIG_OUT_ID resultId, @CONFIG_OUT_MSG_ERROR errorMessage, @CONFIG_OUT_MSG_EXITO exitoMessage", filtro);
            //var result = new ResultConfig() { ResultId = r.resultId, ErrorMessage = r.errorMessage, ExitoMessage = r.exitoMessage };
            var result = new ResultConfig() { ResultId = r.resultId, ErrorMessage = r.errorMessage, ExitoMessage = r.exitoMessage };
            //var result = new ResultConfig() { };
            return result;
        }
        private ResultConfig ExecuteQueryDelete(string sql, FiltroConfig filtro)
        {
            var r = QueryFirst<dynamic>(sql + "; SELECT @CONFIG_OUT_ID resultId, @CONFIG_OUT_MSG_ERROR errorMessage, @CONFIG_OUT_MSG_EXITO exitoMessage", filtro);
            var result = new ResultConfig() { ResultId = r.resultId, ErrorMessage = r.errorMessage, ExitoMessage = r.exitoMessage };
            //var result = new ResultConfig() {};
            return result;
        }
        public MailNotification GetMailNotification(FiltroConfig filtro)
        {
            var sql = @"
            select
                t2.mnot_body body,
                t2.mnot_input_query_id dataQueryId,
                t2.mnot_subject subject,
                t2.mnot_to_query_id toQueryId
            from
                notification t1 left outer join
                mail_notification t2 on t1.mnot_id = t2.mnot_id
            where
                t1.mnot_id = @NotificationId";
            return QueryFirst<MailNotification>(sql, filtro);
        }
    }
    
}


