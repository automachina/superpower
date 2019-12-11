using System;
using System.Collections.Generic;
using System.Linq;
using UrlQueryParser.Expressions;

namespace UrlQueryParser.Generators
{
    public static class MySqlDapperGenerators
    {
        public static DapperClauseBinding ToDapper(this FilterClause filter, string tableAlias = "")
        {
            var binding = new DapperClauseBinding();
            var op = filter.OperatorToString();
            switch (filter.Operator)
            {
                case Operator.Includes:
                    binding.AddDynamicBinding(filter.Field, filter.Values?.Select(v => v.Value));
                    binding.SqlText = $"{tableAlias}.{filter.Field} {op} (@{{0}})";
                    break;
                case Operator.Between:
                    var from = filter.Values?.Select(v => v.Value).First();
                    var to = filter.Values?.Select(v => v.Value).Skip(1).First();
                    binding.AddDynamicBinding($"{filter.Field}_from", from);
                    binding.AddDynamicBinding($"{filter.Field}_to", to);
                    binding.SqlText = $"{tableAlias}.{filter.Field} {op} @{{0}} AND @{{1}}";
                    break;
                case Operator.Like:
                    binding.AddDynamicBinding(filter.Field, filter.Values?.SingleOrDefault()?.Value);
                    binding.SqlText = $"{tableAlias}.{filter.Field} {op} '%@{{0}}%'";
                    break;
                default:
                    binding.SqlText = $"{tableAlias}.{filter.Field} {op} @{{0}}";
                    binding.AddDynamicBinding(filter.Field, filter.Values?.SingleOrDefault()?.Value);
                    break;

            }
            return binding;
        }

        public static DapperClauseBinding ToDapper(this SortClause sort, string tableAlias = "")
        {
            return new DapperClauseBinding
            {
                SqlText = sort.Fields.
                Zip(sort.Orders, (field, order) => $"{tableAlias}.{field} {SortClause.SortOrderToString(order)}").
                Aggregate("", (acc, item) => string.IsNullOrEmpty(acc) ? $"ORDER BY {item}" : $"{acc}, {item}")
            };
        }

        public static DapperClauseBinding ToDapper(this PageClause page)
        {
            var binding = new DapperClauseBinding();
            binding.AddDynamicBinding(nameof(PageClause.PageOffset), page.PageOffset);
            binding.AddDynamicBinding(nameof(PageClause.Limit), page.Limit);
            binding.SqlText = "LIMIT @{0}, @{1}";
            return binding;
        }

        public static DapperClauseBinding ToDapper(this SearchClause search, IList<string> fields, string tableAlias = "")
        {
            var binding = new DapperClauseBinding();
            if (string.IsNullOrEmpty(search.Term?.Trim())) return binding;
            binding.AddDynamicBinding("SearchTerm", search.Term);
            binding.SqlText = fields.Aggregate("", (acc, item) =>
                string.IsNullOrEmpty(acc) ? $"({tableAlias}.{item} LIKE '%@{{0}}%'" : $"{acc}\nOR {tableAlias}.{item} LIKE '%@{{0}}%'")?.
                Append(')').Aggregate("", (acc, chr) => $"{acc}{chr}");
            return binding;
        }
    }
}
