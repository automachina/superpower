using System;
using System.Collections.Generic;
using System.Globalization;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace UrlQueryParser
{
    public class QueryTokenizer : Tokenizer<QueryToken>
    {
        delegate Func<TokenizationState<T>, TextParser<T>> StatefullTextParser<T>(TextSpan span, T token);

        static TextParser<QueryToken> CreateKeywordToken(string keyword, QueryToken token) =>
            from open in Character.EqualTo('_')
            from cmd in Span.EqualToIgnoreCase(keyword).Value(token).Try()
            select token;

        static TextParser<QueryToken> CreateValueToken(string keyword, QueryToken token) =>
                from open in Character.EqualTo('_')
                from ind in Span.EqualToIgnoreCase(keyword).Value(token).Try()
                select token;

        static TextParser<QueryToken> PageCommandToken { get; } = CreateKeywordToken("page", QueryToken.Page);
        static TextParser<QueryToken> LimitCommandToken { get; } = CreateKeywordToken("limit", QueryToken.Limit);
        static TextParser<QueryToken> StartCommandToken { get; } = CreateKeywordToken("start", QueryToken.Start);
        static TextParser<QueryToken> EndCommandToken { get; } = CreateKeywordToken("end", QueryToken.End);
        static TextParser<QueryToken> SortCommandToken { get; } = CreateKeywordToken("sort", QueryToken.Sort);
        static TextParser<QueryToken> OrderCommandToken { get; } = CreateKeywordToken("order", QueryToken.Order);
        static TextParser<QueryToken> SearchCommandToken { get; } = CreateKeywordToken("q", QueryToken.Search);

        static TextParser<QueryToken> GreaterThanOperatorToken { get; } = CreateKeywordToken("gt", QueryToken.GreaterThan);
        static TextParser<QueryToken> GreaterThanOrEqualToOperatorToken { get; } = CreateKeywordToken("gte", QueryToken.GreaterThanOrEqualTo);
        static TextParser<QueryToken> LessThanOperatorToken { get; } = CreateKeywordToken("lt", QueryToken.LessThan);
        static TextParser<QueryToken> LessThanOrEqualToOperatorToken { get; } = CreateKeywordToken("lte", QueryToken.LessThanOrEqualTo);
        static TextParser<QueryToken> NotEqualToOperatorToken { get; } = CreateKeywordToken("ne", QueryToken.NotEqual);
        static TextParser<QueryToken> LikeOperatorToken { get; } = CreateKeywordToken("like", QueryToken.Like);
        static TextParser<QueryToken> IncludesOperatorToken { get; } = CreateKeywordToken("in", QueryToken.Includes);

        static TextParser<QueryToken> StringValueToken { get; } =
            from _ in Character.ExceptIn('&', '=', ',').Many().Try()
            select QueryToken.String;

        static TextParser<QueryToken> NumericValueToken { get; } =
            from sign in Character.EqualTo('-').OptionalOrDefault()
            from first in Character.Digit
            from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
            select QueryToken.Numeric;

        static TextParser<QueryToken> DateTimeValueToken { get; } =
            from _ in Span.EqualToIgnoreCase("_date")
            from date in Character.Digit.Repeat(14).Where(d => DateTime.TryParseExact(new string(d), "yyyyMMddHHmmss", new CultureInfo("en-US"), DateTimeStyles.None, out var outDate))
            select QueryToken.DateTime;

        static TextParser<QueryToken> TrueValueToken { get; } = CreateValueToken("true", QueryToken.True);
        static TextParser<QueryToken> FalseValueToken { get; } = CreateValueToken("false", QueryToken.False);
        static TextParser<QueryToken> NullValueToken { get; } = CreateValueToken("null", QueryToken.Null);
        static TextParser<QueryToken> AscendingToken { get; } =
            from _ in Span.EqualToIgnoreCase("asc").Try()
            select QueryToken.Ascending;
        static TextParser<QueryToken> DescendingToken { get; } =
            from _ in Span.EqualToIgnoreCase("desc").Try()
            select QueryToken.Descending;

        static TextParser<QueryToken> AndToken { get; } = Character.EqualTo('&').Value(QueryToken.And);
        static TextParser<QueryToken> EqualToken { get; } = Character.EqualTo('=').Value(QueryToken.Equals);
        static TextParser<QueryToken> CommaToken { get; } = Character.EqualTo(',').Value(QueryToken.Comma);

        static TextParser<QueryToken> FieldToken { get; } =
            from _ in Character.ExceptIn('=', '&', '_').IgnoreMany().Try()
            select QueryToken.Field;

        protected override IEnumerable<Result<QueryToken>> Tokenize(TextSpan span, TokenizationState<QueryToken> state)
        {
            while (Character.WhiteSpace(span).HasValue)
                span.ConsumeChar();

            if (span.IsAtEnd)
                yield break;

            do
            {
                //Log.Tokenizer(span, state);
                if (PageCommandToken(span) is Result<QueryToken> pageResult && pageResult.HasValue)
                {
                    span = pageResult.Remainder;
                    yield return pageResult;
                }
                else
                if (LimitCommandToken(span) is Result<QueryToken> limitResult && limitResult.HasValue)
                {
                    span = limitResult.Remainder;
                    yield return limitResult;
                }
                else
                if (StartCommandToken(span) is Result<QueryToken> startResult && startResult.HasValue)
                {
                    span = startResult.Remainder;
                    yield return startResult;
                }
                else
                if (EndCommandToken(span) is Result<QueryToken> endResult && endResult.HasValue)
                {
                    span = endResult.Remainder;
                    yield return endResult;
                }
                else
                if (SortCommandToken(span) is Result<QueryToken> sortResult && sortResult.HasValue)
                {
                    span = sortResult.Remainder;
                    yield return sortResult;
                }
                else
                if (OrderCommandToken(span) is Result<QueryToken> orderResult && orderResult.HasValue)
                {
                    span = orderResult.Remainder;
                    yield return orderResult;
                }
                else
                if (SearchCommandToken(span) is Result<QueryToken> searchResult && searchResult.HasValue)
                {
                    span = searchResult.Remainder;
                    yield return searchResult;
                }
                else
                if (GreaterThanOrEqualToOperatorToken(span) is Result<QueryToken> gteResult && gteResult.HasValue)
                {
                    span = gteResult.Remainder;
                    yield return gteResult;
                }
                else
                if (GreaterThanOperatorToken(span) is Result<QueryToken> gtResult && gtResult.HasValue)
                {
                    span = gtResult.Remainder;
                    yield return gtResult;
                }
                else
                if (LessThanOrEqualToOperatorToken(span) is Result<QueryToken> lteResult && lteResult.HasValue)
                {
                    span = lteResult.Remainder;
                    yield return lteResult;
                }
                else
                if (LessThanOperatorToken(span) is Result<QueryToken> ltResult && ltResult.HasValue)
                {
                    span = ltResult.Remainder;
                    yield return ltResult;
                }
                else
                if (NotEqualToOperatorToken(span) is Result<QueryToken> neResult && neResult.HasValue)
                {
                    span = neResult.Remainder;
                    yield return neResult;
                }
                else
                if (LikeOperatorToken(span) is Result<QueryToken> likeResult && likeResult.HasValue)
                {
                    span = likeResult.Remainder;
                    yield return likeResult;
                }
                else
                if (IncludesOperatorToken(span) is Result<QueryToken> inResult && inResult.HasValue)
                {
                    span = inResult.Remainder;
                    yield return inResult;
                }
                else
                if ((!state.Previous.HasValue || state.Previous.Value.Kind == QueryToken.And) && FieldToken(span) is Result<QueryToken> fieldResult && fieldResult.HasValue)
                {
                    span = fieldResult.Remainder;
                    yield return fieldResult;
                }
                else
                if (AndToken(span) is Result<QueryToken> andResult && andResult.HasValue)
                {
                    span = andResult.Remainder;
                    yield return andResult;
                }
                else
                if (CommaToken(span) is Result<QueryToken> commaResult && commaResult.HasValue)
                {
                    span = commaResult.Remainder;
                    yield return commaResult;
                }
                else
                if (EqualToken(span) is Result<QueryToken> equalResult && equalResult.HasValue)
                {
                    span = equalResult.Remainder;
                    yield return equalResult;
                }
                else
                if (state.Previous.HasValue && (state.Previous.Value.Kind == QueryToken.Equals || state.Previous.Value.Kind == QueryToken.Comma))
                {
                    if (TrueValueToken(span) is Result<QueryToken> trueRsesult && trueRsesult.HasValue)
                    {
                        span = trueRsesult.Remainder;
                        yield return trueRsesult;
                    }
                    else
                    if (FalseValueToken(span) is Result<QueryToken> falseRsesult && falseRsesult.HasValue)
                    {
                        span = falseRsesult.Remainder;
                        yield return falseRsesult;
                    }
                    else
                    if (NullValueToken(span) is Result<QueryToken> nullRsesult && nullRsesult.HasValue)
                    {
                        span = nullRsesult.Remainder;
                        yield return nullRsesult;
                    }
                    else
                    if (AscendingToken(span) is Result<QueryToken> ascRsesult && ascRsesult.HasValue)
                    {
                        span = ascRsesult.Remainder;
                        yield return ascRsesult;
                    }
                    else
                    if (DescendingToken(span) is Result<QueryToken> descRsesult && descRsesult.HasValue)
                    {
                        span = descRsesult.Remainder;
                        yield return descRsesult;
                    }
                    else
                    if (NumericValueToken(span) is Result<QueryToken> numResult && numResult.HasValue)
                    {
                        span = numResult.Remainder;
                        yield return numResult;
                    }
                    else
                    if (DateTimeValueToken(span) is Result<QueryToken> dtResult && dtResult.HasValue)
                    {
                        span = dtResult.Remainder;
                        yield return dtResult;
                    }
                    else
                    if (StringValueToken(span) is Result<QueryToken> strResult && strResult.HasValue)
                    {
                        span = strResult.Remainder;
                        yield return strResult;
                    }
                }
                else
                if (!span.IsAtEnd && span.ConsumeChar() is Result<char> pending && pending.HasValue)
                {
                    yield return Result.Empty<QueryToken>(pending.Location, $"Unrecognized '{pending}'");
                }

            } while (!span.IsAtEnd);
        }
    }
}
