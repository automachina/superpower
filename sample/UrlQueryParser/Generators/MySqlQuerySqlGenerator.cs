using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UrlQueryParser.Expressions;

namespace UrlQueryParser.Generators
{
    public class MySqlQuerySqlGenerator
    {
        private string sql;
        private StringBuilder sqlBuilder;
        private QueryExpression query;
        private IList<string> fields;
        bool whereClauseSet = false;

        public MySqlQuerySqlGenerator(string baseSql, QueryExpression queryExpression, IList<string> searchFields)
        {
            sql = baseSql;
            query = queryExpression;
            fields = searchFields;
        }

        public string Generate()
        {
            sqlBuilder = new StringBuilder($"SELECT * FROM\n({sql}) AS A\n");

            if (query.QueryClauses.Where(c => c is FilterClause) is IEnumerable<Expression> filters)
            {
                sqlBuilder.Append($"WHERE ");
                whereClauseSet = true;
                var first = true;
                foreach (var filter in filters.Cast<FilterClause>())
                {
                    if (first)
                    {
                        sqlBuilder.AppendLine($"{filter.ToSql("A")}");
                        first = false;
                    }
                    else
                    {
                        sqlBuilder.AppendLine($"AND {filter.ToSql("A")}");
                    }
                }
            }

            if (query.QueryClauses.Where(c => c is SearchClause).SingleOrDefault() is SearchClause search)
                sqlBuilder.AppendLine($"{search.ToSql(fields, "A")}");

            if (query.QueryClauses.Where(c => c is SortClause).SingleOrDefault() is SortClause sort)
                sqlBuilder.AppendLine($"{sort.ToSql("A")}");

            if (query.QueryClauses.Where(c => c is PageClause).SingleOrDefault() is PageClause page)
                sqlBuilder.AppendLine($"{page.ToSql()}");

            return sqlBuilder.ToString();
        }

        public static string Apply(string baseQuery, QueryExpression queryExpression, IList<string> queryFields) =>
            new MySqlQuerySqlGenerator(baseQuery, queryExpression, queryFields).Generate();
    }
}
