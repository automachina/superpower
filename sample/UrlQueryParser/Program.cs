using System;
using System.Collections.Generic;
using System.Linq;
using UrlQueryParser.Generators;

namespace UrlQueryParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var queryString = "_page=1,10&uid_gte=7257&name_like=Sam&enabled=_true&updated=_date20191205000000&_q=something&_sort=uid,name&_order=asc,desc";
            queryString = "_page=1,10&event=error&tdatetime_gte=_date20191101000000&_sort=uid,version&_order=asc,desc";
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

                    Console.WriteLine();
                    Console.WriteLine(MySqlQuerySqlGenerator.Apply("SELECT * FROM client_telemetry WHERE clientid = 'b45b923a-b25a-4ae6-b940-36eef51cb2da'", query, new List<string> { "userid", "deviceId" }));
                }
                else
                {
                    Console.WriteLine($"Query Parser Failed: {error} @ {position}");
                }

                queryString = "_page=3,10&email_like=nwcu.com&createdatetime_lte=_date20190101000000&_sort=createdatetime&_order=desc";

                if (QueryParser.TryParse(queryString, out var query1, out var error1, out var position1))
                {
                    Console.WriteLine(new string(Enumerable.Repeat('-', 100).ToArray()));
                    foreach (var clause in query1.QueryClauses)
                        Console.WriteLine($"{clause.GetType().Name} : {clause}");

                    Console.WriteLine();
                    Console.WriteLine(MySqlQuerySqlGenerator.Apply("SELECT * FROM user_profiles WHERE clientid = 'b45b923a-b25a-4ae6-b940-36eef51cb2da'", query1, new List<string> { "userid", "deviceId" }));
                }
                else
                {
                    Console.WriteLine($"Query Parser Failed: {error1} @ {position1}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
