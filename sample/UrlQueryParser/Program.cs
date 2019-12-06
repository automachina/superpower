using System;
using System.Linq;

namespace UrlQueryParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var queryString = "_page=1,10&uid=100&name_like=Sam&enabled=_true&updated=_date20191205000000&_q=something&_sort=uid,name&_order=asc,desc";
            Console.WriteLine(queryString);
            foreach (var idx in Enumerable.Range(0, queryString.Length - 1))
                Console.Write($"{idx}>[{queryString[idx]}]");
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
            try
            {
                var tokenizer = new QueryTokenizer();
                var tokens = tokenizer.Tokenize(queryString);
                foreach (var token in tokens)
                    Console.WriteLine(token);

                if (QueryParser.TryParse(queryString, out var query, out var error, out var position))
                {
                    Console.WriteLine(new string(Enumerable.Repeat('-', 100).ToArray()));
                    foreach (var clause in query.QueryClauses)
                        Console.WriteLine($"{clause.GetType().Name} : {clause}");
                }
                else
                {
                    Console.WriteLine($"Query Parser Failed: {error} @ {position}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
