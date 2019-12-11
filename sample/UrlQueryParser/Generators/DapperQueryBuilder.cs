using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace UrlQueryParser.Generators
{
    public class DapperQueryBuilder
    {
        public DapperQueryBuilder(string baseSql, Dictionary<string, object> bindings)
        {
            ClauseBindings.Add(new DapperClauseBinding { SqlText = baseSql, Bindings = bindings });
            foreach (var binding in bindings)
                Bindings.Add(binding);
        }

        public string QueryText => QueryStringBuilder.ToString();

        public dynamic DapperBindings =>
            Bindings.Aggregate(new ExpandoObject() as IDictionary<string, object>, (acc, param) =>
            {
                acc.Add(param.Key, param.Value);
                return acc;
            });

        StringBuilder _queryStringBuilder; public StringBuilder QueryStringBuilder
        {
            get
            {
                if (_queryStringBuilder is null)
                    _queryStringBuilder = new StringBuilder();
                return _queryStringBuilder;
            }
        }

        IList<DapperClauseBinding> _dapperClauses; public IList<DapperClauseBinding> ClauseBindings
        {
            get
            {
                if (_dapperClauses is null)
                    _dapperClauses = new List<DapperClauseBinding>();
                return _dapperClauses;
            }
        }

        IDictionary<string, object> _bindings; public IDictionary<string, object> Bindings
        {
            get
            {
                if (_bindings is null)
                    _bindings = new Dictionary<string, object>();
                return _bindings;
            }
            private set => _bindings = value;
        }

        public void MergeClauseBindings(DapperClauseBinding clauseBinding, Action<StringBuilder, string> mergeAction = null)
        {
            var sqlText = clauseBinding.SqlText;
            if (clauseBinding?.Bindings.Count > 0)
            {
                var bindings = new List<object>();
                foreach (var binding in clauseBinding.Bindings)
                {
                    bindings.Add(AddDynamicBinding(binding));
                }
                sqlText = string.Format(clauseBinding.SqlText, bindings.ToArray());
            }
            ClauseBindings.Add(clauseBinding);

            if (mergeAction is null) return;
            mergeAction.Invoke(QueryStringBuilder, sqlText);
        }

        /// <summary>
        /// Attempts to add a new key value to the Bindings Dictionary; if the key is already present it will generate a key suffix and re-attempt
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>The dynamically generated binding name.</returns>
        public string AddDynamicBinding(string name, object value)
        {
            var key = name;
            var idx = 0;
            while (!Bindings.TryAdd(key, value))
            {
                if (idx <= 100)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unable to add key name: {name} to the parameters dictionary after 100 tries.");

                key = $"{key}{idx++}";
            }
            return key;
        }

        public string AddDynamicBinding(KeyValuePair<string, object> keyValue) =>
            AddDynamicBinding(keyValue.Key, keyValue.Value);

        public void Append(string source) => QueryStringBuilder.Append(source);
        public void AppendLine(string source) => QueryStringBuilder.AppendLine(source);
        public void AppendFormat(string format, object args) => QueryStringBuilder.AppendFormat(format, args);
        public void AppendFormat(string format, params object[] args) => QueryStringBuilder.AppendFormat(format, args);
    }
}
