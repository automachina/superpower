using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UrlQueryParser.Expressions;
using UrlQueryParser.Generators;

namespace UrlQueryParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var queryString = "_page=1,10&uid_gte=7257&name_like=Sam&enabled=_true&updated=_date20191205000000&_q=something&_sort=uid,name&_order=asc,desc";
            queryString = "_page=1,10&event=error&tdatetime_gte=_date20191101000000&_sort=uid,version&_order=asc,desc";
            PrintQueryString(queryString);
            try
            {
                PrintTokens(queryString);

                if (QueryParser.TryParse(queryString, out var query, out var error, out var position))
                {
                    PrintExpressions(query);

                    Console.WriteLine();
                    Console.WriteLine(MySqlQuerySqlGenerator.Apply("SELECT * FROM client_telemetry WHERE clientid = 'b45b923a-b25a-4ae6-b940-36eef51cb2da'", query, new List<string> { "userid", "deviceId" }));
                }
                else
                {
                    Console.WriteLine($"Query Parser Failed: {error} @ {position}");
                }

                queryString = "_page=3,10&email_like=nwcu.com&createdatetime_between=_date20190101000000,_date20190110000000&_q=Position&_sort=createdatetime&_order=desc";

                PrintQueryString(queryString);
                PrintTokens(queryString);

                if (QueryParser.TryParse(queryString, out var query1, out var error1, out var position1))
                {
                    PrintExpressions(query1);

                    Console.WriteLine();
                    var parameters = new Dictionary<string, object>
                    {
                        { "ClientId", Guid.Parse("b45b923a-b25a-4ae6-b940-36eef51cb2da") }
                    };
                    var builder = MySqlDapperQueryGenerator.Apply("SELECT * FROM user_profiles WHERE clientid = @ClientId", query1, new List<string> { "userid", "deviceId", "name", "email", "phone" }, parameters);
                    Console.WriteLine(builder.QueryText);
                    var paramObj = builder.DapperBindings;
                    foreach (var p in paramObj)
                    {
                        Console.WriteLine(p);
                    }
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

        static void PrintQueryString(string queryString)
        {
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
            Console.WriteLine(queryString);
            foreach (var idx in Enumerable.Range(0, queryString.Length - 1))
                Console.Write($"{idx}>[{queryString[idx]}]");
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
        }

        static void PrintTokens(string queryString)
        {
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
            var tokenizer = new QueryTokenizer();
            var tokens = tokenizer.Tokenize(queryString);
            foreach (var token in tokens)
                Console.WriteLine(token);
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
        }

        static void PrintExpressions(QueryExpression query)
        {
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
            foreach (var clause in query.QueryClauses)
                Console.WriteLine($"{clause.GetType().Name} : {clause}");
            Console.WriteLine(Environment.NewLine + new string(Enumerable.Repeat('-', 100).ToArray()));
        }
    }
}
