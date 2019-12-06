using System;
namespace UrlQueryParser.Expressions
{
    public class EndClause : Expression
    {
        public int Index { get; set; }

        public EndClause(int index)
        {
            Index = index;
        }
    }
}
