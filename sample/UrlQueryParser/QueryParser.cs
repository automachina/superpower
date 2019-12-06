using System;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using UrlQueryParser.Expressions;
using ValueType = UrlQueryParser.Expressions.ValueType;

namespace UrlQueryParser
{
    public static class QueryParser
    {
        static TokenListParser<QueryToken, string> QueryField { get; } =
            Token.EqualTo(QueryToken.Field).
            Apply(QueryTextParser.FieldParser).
            Select(f => f);

        static TokenListParser<QueryToken, string> QueryString { get; } =
            Token.EqualTo(QueryToken.String).
            Apply(QueryTextParser.StringParser).
            Select(s => s);

        static TokenListParser<QueryToken, double> QueryNumeric { get; } =
            Token.EqualTo(QueryToken.Numeric).
            Apply(QueryTextParser.NumericParser).
            Select(n => n);

        static TokenListParser<QueryToken, DateTime> QueryDateTime { get; } =
            Token.EqualTo(QueryToken.DateTime).
            Apply(QueryTextParser.DateTimeParser).
            Select(d => d);

        static TokenListParser<QueryToken, bool> QueryTrue { get; } =
            Token.EqualTo(QueryToken.True).Value(true);

        static TokenListParser<QueryToken, bool> QueryFalse { get; } =
            Token.EqualTo(QueryToken.True).Value(false);

        static TokenListParser<QueryToken, object> QueryNull { get; } =
            Token.EqualTo(QueryToken.True).Value((object)null);

        static TokenListParser<QueryToken, SortOrder> QuerySortOrder { get; } =
            from order in Token.EqualTo(QueryToken.Ascending).Value(SortOrder.Asending).
                Or(Token.EqualTo(QueryToken.Descending).Value(SortOrder.Descending))
            select order;

        static TokenListParser<QueryToken, FilterValue> QueryValue { get; } =
            from val in QueryNumeric.Select(v => new FilterValue(ValueType.Numeric, v)).
                Or(QueryString.Select(v => new FilterValue(ValueType.String, v))).
                Or(QueryDateTime.Select(v => new FilterValue(ValueType.DateTime, v))).
                Or(QueryTrue.Select(v => new FilterValue(ValueType.True, v))).
                Or(QueryFalse.Select(v => new FilterValue(ValueType.False, v))).
                Or(QueryNull.Select(v => new FilterValue(ValueType.Null, v)))
            select val;

        static TokenListParser<QueryToken, Operator> QueryOperator { get; } =
            from op in Token.EqualTo(QueryToken.GreaterThan).Value(Operator.GreaterThan).
                Or(Token.EqualTo(QueryToken.GreaterThanOrEqualTo).Value(Operator.GreaterThanOrEqualTo)).
                Or(Token.EqualTo(QueryToken.LessThan).Value(Operator.LessThan)).
                Or(Token.EqualTo(QueryToken.LessThanOrEqualTo).Value(Operator.LessThanOrEqualTo)).
                Or(Token.EqualTo(QueryToken.Like).Value(Operator.Like)).
                Or(Token.EqualTo(QueryToken.Includes).Value(Operator.Includes))
            select op;

        static TokenListParser<QueryToken, object> QueryLimit { get; } =
            from c1 in Token.EqualTo(QueryToken.Limit)
            from e1 in Token.EqualTo(QueryToken.Equals)
            from limit in QueryNumeric
            select (object)limit;

        static TokenListParser<QueryToken, Expression> QueryPage { get; } =
            from c1 in Token.EqualTo(QueryToken.Page)
            from e1 in Token.EqualTo(QueryToken.Equals)
            from page in QueryNumeric
            from c in Token.EqualTo(QueryToken.Comma)
            from limit in QueryNumeric
            select (Expression)new PageClause((int)page, (int)limit);

        static TokenListParser<QueryToken, Expression> QueryFilter { get; } =
            from field in QueryField
            from op in QueryOperator.OptionalOrDefault(Operator.Equals)
            from values in Token.EqualTo(QueryToken.Equals).IgnoreThen(QueryValue.
                ManyDelimitedBy(Token.EqualTo(QueryToken.Comma)))
            select (Expression)new FilterCluase(field, op, values);

        static TokenListParser<QueryToken, SortOrder[]> QuerySortOrders { get; } =
            from _ in Token.EqualTo(QueryToken.Order)
            from orders in Token.EqualTo(QueryToken.Equals).
                IgnoreThen(QuerySortOrder.ManyDelimitedBy(Token.EqualTo(QueryToken.Comma)))
            select orders;

        static TokenListParser<QueryToken, string[]> QuerySortFields { get; } =
            from sort in Token.EqualTo(QueryToken.Sort)
            from fields in Token.EqualTo(QueryToken.Equals).
                IgnoreThen(QueryString.ManyDelimitedBy(Token.EqualTo(QueryToken.Comma)))
            select fields;

        static TokenListParser<QueryToken, Expression> QuerySort { get; } =
            from fields in QuerySortFields
            from orders in Token.EqualTo(QueryToken.And).IgnoreThen(QuerySortOrders)
            select (Expression)new SortClause(fields, orders);

        static TokenListParser<QueryToken, Expression> QuerySearch { get; } =
            from q in Token.EqualTo(QueryToken.Search)
            from eq in Token.EqualTo(QueryToken.Equals)
            from term in QueryString
            select (Expression)new SearchClause(term);

        static TokenListParser<QueryToken, Expression> QueryStart { get; } =
            from start in Token.EqualTo(QueryToken.Start)
            from eq in Token.EqualTo(QueryToken.Equals)
            from idx in QueryNumeric
            select (Expression)new StartClause((int)idx);

        static TokenListParser<QueryToken, Expression> QueryEnd { get; } =
            from end in Token.EqualTo(QueryToken.End)
            from eq in Token.EqualTo(QueryToken.Equals)
            from idx in QueryNumeric
            select (Expression)new EndClause((int)idx);

        static TokenListParser<QueryToken, Expression> Clause { get; } =
            from clause in QueryPage.
                Or(QueryFilter).
                Or(QuerySort).
                Or(QuerySearch).
                Or(QueryStart).
                Or(QueryEnd)
            select clause;

        static TokenListParser<QueryToken, Query> Query { get; } =
            from clauses in Clause.ManyDelimitedBy(Token.EqualTo(QueryToken.And)).AtEnd()
            select new Query(clauses);

        public static bool TryParse(string queryString, out Query query, out string error, out Position errorPosition)
        {
            var tokens = new QueryTokenizer().TryTokenize(queryString);
            if (!tokens.HasValue)
            {
                query = null;
                error = tokens.ToString();
                errorPosition = tokens.ErrorPosition;
                return false;
            }

            var parser = Query.TryParse(tokens.Value);
            if (!parser.HasValue)
            {
                query = null;
                error = parser.ToString();
                errorPosition = parser.ErrorPosition;
                return false;
            }

            query = parser.Value;
            error = null;
            errorPosition = Position.Empty;
            return true;
        }
    }
}
