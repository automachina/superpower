using System;
using System.Globalization;
using System.Linq;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace UrlQueryParser
{
    public static class QueryTextParser
    {
        public static TextParser<string> FieldParser { get; } =
            from chars in Character.ExceptIn('&', '=', '_', '%').Many()
            select new string(chars);

        public static TextParser<string> StringParser { get; } =
            from chars in Character.ExceptIn('&', '=', ',').
                Or(Character.EqualTo('%').
                IgnoreThen(
                    Span.MatchedBy(Character.HexDigit.Repeat(2)).
                    Apply(Numerics.HexDigitsUInt32).
                    Select(h => (char)h)
                ).Named("Hex Sequence")).Many()
            select new string(chars);

        public static TextParser<double> NumericParser { get; } =
            from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
            from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
            from frac in Character.EqualTo('.')
                .IgnoreThen(Numerics.Natural)
                .Select(n => double.Parse(n.ToStringValue()) * Math.Pow(10, -n.Length))
                .OptionalOrDefault()
            from exp in Character.EqualToIgnoreCase('e')
                .IgnoreThen(Character.EqualTo('+').Value(1.0)
                    .Or(Character.EqualTo('-').Value(-1.0))
                    .OptionalOrDefault(1.0))
                .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
                .OptionalOrDefault()
            select (whole + frac) * sign * Math.Pow(10, exp);

        public static TextParser<DateTime> DateTimeParser { get; } =
            from _ in Span.EqualToIgnoreCase("_date")
            from date in Character.Digit.Repeat(14).
                Where(d => DateTime.TryParseExact(new string(d), "yyyyMMddHHmmss", new CultureInfo("en-US"), DateTimeStyles.None, out var outDate)).
                Select(c => DateTime.ParseExact(new string(c), "yyyyMMddHHmmss", new CultureInfo("en-US"), DateTimeStyles.None))
            select date;


    }
}
