using System;
using Superpower.Model;

namespace UrlQueryParser
{
    public static class Log
    {
        public static void Tokenizer(TextSpan span, TokenizationState<QueryToken> state)
        {
            Console.WriteLine($"State: {state.Previous}, Span: {span}");
        }
    }
}
