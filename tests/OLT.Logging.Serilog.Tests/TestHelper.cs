using System.Collections;
using System.Collections.Generic;

namespace OLT.Logging.Serilog.Tests
{
    public static  class TestHelper
    {
        public static Dictionary<string, string?> ToDictionary(IDictionary data)
        {
            var dict = new Dictionary<string, string?>();
            foreach (DictionaryEntry dictionaryEntry in data)
            {
                dict.Add(dictionaryEntry.Key.ToString() ?? string.Empty, dictionaryEntry.Value?.ToString());
            }
            return dict;
        }
    }
}
