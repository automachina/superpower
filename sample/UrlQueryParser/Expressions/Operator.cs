using System.ComponentModel;

namespace UrlQueryParser.Expressions
{
    public enum Operator
    {
        [Description("=")]
        Equals,
        [Description(">")]
        GreaterThan,
        [Description(">=")]
        GreaterThanOrEqualTo,
        [Description("<")]
        LessThan,
        [Description("<=")]
        LessThanOrEqualTo,
        [Description("<>")]
        NotEqual,
        [Description("LIKE")]
        Like,
        [Description("IN")]
        Includes,
        [Description("BETWEEN")]
        Between
    }
}