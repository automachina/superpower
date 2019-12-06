using System;
namespace UrlQueryParser.Expressions
{
    public class SearchClause : Expression
    {
        public string Term { get; set; }

        public SearchClause(string term)
        {
            Term = term;
        }

        public override string ToString() =>
            $"{{ term: {Term} }}";
    }
}
