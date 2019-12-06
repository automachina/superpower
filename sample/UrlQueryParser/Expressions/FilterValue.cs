namespace UrlQueryParser.Expressions
{
    public class FilterValue
    {
        public ValueType ValueType { get; set; }
        public object Value { get; set; }

        public FilterValue(ValueType type, object value)
        {
            ValueType = type;
            Value = value;
        }

        public override string ToString() => $"{ValueType} : {Value}";
    }
}