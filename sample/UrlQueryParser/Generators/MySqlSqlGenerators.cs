using System;
using System.Collections.Generic;
using System.Linq;
using UrlQueryParser.Expressions;
using ValueType = UrlQueryParser.Expressions.ValueType;

namespace UrlQueryParser.Generators
{
    public static class MySqlSqlGenerators
    {
        public static string ToSql(this FilterClause filter, string tableAlias)
        {
            string toValue(FilterValue filterValue)
            {
                switch (filterValue.ValueType)
                {
                    case ValueType.Numeric:
                        return $"{filterValue.Value}";
                    case ValueType.True:
                    case ValueType.False:
                    case ValueType.Null:
                        return $"{filterValue.ValueType}";
                    case ValueType.DateTime:
                        var date = filterValue.Value as DateTime?;
                        if (date.HasValue)
                            return $"STR_TO_DATE('{date.Value.ToString("yyyy-MM-dd HH:mm:ss")}','%Y-%m-%d %H:%i:%s')";
                        return "NOW()";
                    case ValueType.String:
                        return $"'{filterValue.Value}'";
                    default:
                        return $"'{filterValue.Value}'";
                }
            }

            var values = filter.Values.Aggregate("", (acc, item) => string.IsNullOrEmpty(acc) ? $"{toValue(item)}" : $"{acc}, {toValue(item)}");
            var op = filter.OperatorToString();
            if (filter.Operator == Operator.Includes)
                return $"{tableAlias}.{filter.Field} {op} ({values})";
            if (filter.Operator == Operator.Like)
                return $"{tableAlias}.{filter.Field} {op} '%{values.Trim('\'')}%'";
            return $"{tableAlias}.{filter.Field} {op} {values}";
        }

        public static string ToSql(this SortClause sort, string tableAlias) =>
            sort.Fields.
                Zip(sort.Orders, (field, order) => $"{tableAlias}.{field} {SortClause.SortOrderToString(order)}").
                Aggregate("", (acc, item) => string.IsNullOrEmpty(acc) ? $"ORDER BY {item}" : $"{acc}, {item}");

        public static string ToSql(this PageClause page) =>
            $"LIMIT {page.PageOffset}, {page.Limit}";

        public static string ToSql(this SearchClause search, IList<string> fields, string tableAlias)
        {
            if (string.IsNullOrEmpty(search.Term?.Trim())) return "";
            return fields.Aggregate("", (acc, item) =>
                string.IsNullOrEmpty(acc) ? $"({tableAlias}.{item} LIKE '%{search.Term}%'" : $"{acc}\nOR {tableAlias}.{item} LIKE '%{search.Term}%'").
                Append(')').ToString();
        }

    }
}
