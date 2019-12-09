using System;
namespace UrlQueryParser.Expressions
{
    public class PageClause : Expression
    {
        public int Page { get; private set; }
        public int Limit { get; private set; }

        public PageClause(int page, int limit)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page Number must be greater then or equal to 1");
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Page Limit must be greater then or equal to 0");

            Page = page;
            Limit = limit;
        }

        public int PageOffset =>
            Limit * (Page - 1);

        public override string ToString() =>
            $"{{ page: {Page}, limit: {Limit} }}";
    }
}
