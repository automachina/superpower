using System;
namespace UrlQueryParser.Expressions
{
    public class StartClause : Expression
    {
        public int Index { get; set; }

        public StartClause(int index)
        {
            Index = index;
        }
    }
}
