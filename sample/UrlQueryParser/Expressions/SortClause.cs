using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UrlQueryParser.Expressions
{
    public class SortClause : Expression
    {
        public IList<string> Fields { get; set; }
        public IList<SortOrder> Orders { get; set; }

        public SortClause(string[] fields, SortOrder[] orders)
        {
            if (fields is null || fields.Length == 0)
                throw new ArgumentNullException(nameof(Fields), $"{nameof(SortClause)}: Requires 1 or more fields to be defined");

            Fields = fields.ToList();

            if (orders != null && orders.Length != fields.Length)
                Orders = fields.Length < orders.Length ?
                    orders.Take(fields.Length).ToList() :
                    orders.Concat(Enumerable.Repeat(SortOrder.Asending, fields.Length - orders.Length)).ToList();
            else
                Orders = orders?.ToList() ?? Enumerable.Repeat(SortOrder.Asending, fields.Length).ToList();
        }

        public static string SortOrderToString(SortOrder sort) =>
            typeof(SortOrder).GetField($"{sort}").
                    CustomAttributes.
                    Where(a => a.AttributeType == typeof(DescriptionAttribute)).
                    SelectMany(d => d.ConstructorArguments).
                    Select(a => a.Value).FirstOrDefault()?.ToString();

        public override string ToString()
        {
            var fields = Fields.Aggregate("", (acc, item) => string.IsNullOrEmpty(acc) ? acc = item : $"{acc},{item}");
            var orders = Orders.Aggregate("", (acc, item) =>
            {
                var order = SortOrderToString(item);
                if (string.IsNullOrEmpty(acc))
                    return $"{order}";

                return $"{acc},{order}";
            });
            return $"{{ fields: [{fields}], orders: [{orders}] }}";
        }
    }
}
