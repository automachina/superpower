using System;
using System.Collections.Generic;
using System.Linq;

namespace UrlQueryParser.Expressions
{
    public class Query
    {
        public IList<Expression> QueryClauses { get; set; }

        public Query(params Expression[] clauses)
        {
            QueryClauses = clauses?.ToList();
        }
    }
}
