using System;
namespace UrlQueryParser.Expressions
{
    public class PageClause : Expression
    {
        public int Page { get; set; }
        public int Limit { get; set; }

        public PageClause(int page, int limit)
        {
            Page = page;
            Limit = limit;
        }

        public override string ToString() =>
            $"{{ page: {Page}, limit: {Limit} }}";
    }
}
