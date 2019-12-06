using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UrlQueryParser.Expressions
{
    public class FilterCluase : Expression
    {
        public string Field { get; set; }
        public Operator Operator { get; set; }
        public IList<FilterValue> Values { get; set; }

        public FilterCluase(string field, Operator @operator, params FilterValue[] values)
        {
            Field = field;
            Operator = @operator;
            Values = values?.ToList();
        }

        public override string ToString()
        {
            var values = Values?.Aggregate("", (acc, item) => string.IsNullOrEmpty(acc) ? acc = $"{item}" : $"{acc},{item}");
            var op = typeof(Operator).GetField($"{Operator}")?.
                CustomAttributes.
                Where(a => a.AttributeType == typeof(DescriptionAttribute)).
                SelectMany(des => des.ConstructorArguments).
                Select(a => a.Value).FirstOrDefault();
            return $"{{ {Field} {op} {values} }}";
        }
    }
}
