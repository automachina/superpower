using Superpower.Display;

namespace UrlQueryParser
{
    public enum QueryToken
    {
        Undefined,
        [Token(Category = "Command", Example = "_page")]
        Page,
        [Token(Category = "Command", Example = "_limit")]
        Limit,
        [Token(Category = "Command", Example = "_start")]
        Start,
        [Token(Category = "Command", Example = "_end")]
        End,
        [Token(Category = "Command", Example = "_sort")]
        Sort,
        [Token(Category = "Command", Example = "_order")]
        Order,
        [Token(Category = "Command", Example = "_q")]
        Search,
        [Token(Category = "Delimiter", Example = "&")]
        And,
        [Token(Category = "Delimiter", Example = ",")]
        Comma,
        [Token(Category = "Escape", Example = "%##")]
        Hex,
        [Token(Category = "Operator", Example = "=")]
        Equals,
        [Token(Category = "Operator", Example = "_gt")]
        GreaterThan,
        [Token(Category = "Operator", Example = "_gte")]
        GreaterThanOrEqualTo,
        [Token(Category = "Operator", Example = "_lt")]
        LessThan,
        [Token(Category = "Operator", Example = "_lte")]
        LessThanOrEqualTo,
        [Token(Category = "Operator", Example = "_ne")]
        NotEqual,
        [Token(Category = "Operator", Example = "_like")]
        Like,
        [Token(Category = "Operator", Example = "_in")]
        Includes,
        [Token(Category = "Operator", Example = "_between")]
        Between,
        [Token(Category = "Content")]
        Field,
        [Token(Category = "Content")]
        String,
        [Token(Category = "Content")]
        Numeric,
        [Token(Category = "Content")]
        DateTime,
        [Token(Category = "Indicator", Example = "true")]
        True,
        [Token(Category = "Indicator", Example = "false")]
        False,
        [Token(Category = "Indicator", Example = "null")]
        Null,
        [Token(Category = "Indicator", Example = "asc")]
        Ascending,
        [Token(Category = "Indicator", Example = "desc")]
        Descending
    }
}
