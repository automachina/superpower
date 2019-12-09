using System;
using System.Collections.Generic;
using System.Linq;

namespace UrlQueryParser.Expressions
{
    public class QueryExpression
    {
        public IList<Expression> QueryClauses { get; set; }

        public QueryExpression(params Expression[] clauses)
        {
            QueryClauses = clauses?.ToList();
        }
    }
}
