using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UrlQueryParser.Expressions;

namespace UrlQueryParser.Generators
{
    public class MySqlDapperQueryGenerator
    {
        private DapperQueryBuilder queryBuilder;
        private string sql;
        private QueryExpression query;
        private IList<string> fields;
        bool whereClauseSet = false;

        public MySqlDapperQueryGenerator(string baseSql, QueryExpression queryExpression, IList<string> searchFields, Dictionary<string, object> baseBindings)
        {
            sql = baseSql;
            query = queryExpression;
            fields = searchFields;
            queryBuilder = new DapperQueryBuilder(baseSql, baseBindings);
        }

        public DapperQueryBuilder Build()
        {
            queryBuilder.Append($"SELECT * FROM\n({sql}) AS A\n");

            if (query.QueryClauses.Where(c => c is FilterClause) is IEnumerable<Expression> filters)
            {
                queryBuilder.Append($"WHERE ");
                whereClauseSet = true;
                var first = true;
                foreach (var filter in filters.Cast<FilterClause>())
                {
                    if (first)
                    {
                        queryBuilder.MergeClauseBindings(filter.ToDapper("A"), (sb, sql) => sb.AppendLine(sql));
                        first = false;
                    }
                    else
                    {
                        queryBuilder.MergeClauseBindings(filter.ToDapper("A"), (sb, sql) => sb.AppendLine($"AND {sql}"));
                    }
                }
            }

            if (query.QueryClauses.Where(c => c is SearchClause).SingleOrDefault() is SearchClause search)
            {
                if (!whereClauseSet)
                    queryBuilder.Append($"WHERE ");
                else
                    queryBuilder.Append($"AND ");
                queryBuilder.MergeClauseBindings(search.ToDapper(fields, "A"), (sb, sql) =>
                    sb.AppendLine(sql));
            }

            if (query.QueryClauses.Where(c => c is SortClause).SingleOrDefault() is SortClause sort)
                queryBuilder.MergeClauseBindings(sort.ToDapper("A"), (sb, sql) => sb.AppendLine(sql));

            if (query.QueryClauses.Where(c => c is PageClause).SingleOrDefault() is PageClause page)
                queryBuilder.MergeClauseBindings(page.ToDapper(), (sb, sql) => sb.AppendLine(sql));

            return queryBuilder;
        }

        public static DapperQueryBuilder Apply(string baseQuery, QueryExpression queryExpression, IList<string> queryFields, Dictionary<string, object> parameters) =>
            new MySqlDapperQueryGenerator(baseQuery, queryExpression, queryFields, parameters).Build();
    }
}
