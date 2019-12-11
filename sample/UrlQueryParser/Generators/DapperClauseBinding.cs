using System;
using System.Collections.Generic;

namespace UrlQueryParser.Generators
{
    public class DapperClauseBinding
    {
        public string SqlText { get; set; }
        IDictionary<string, object> _bindings; public IDictionary<string, object> Bindings
        {
            get
            {
                if (_bindings is null)
                    _bindings = new Dictionary<string, object>();
                return _bindings;
            }
            set => _bindings = value;
        }

        public DapperClauseBinding() { }
        public DapperClauseBinding(string sqlText, IDictionary<string, object> bindings)
        {
            SqlText = sqlText;
            Bindings = bindings;
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
    }
}
