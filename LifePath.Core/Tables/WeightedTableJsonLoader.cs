using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LifePath.Core.Tables
{
    public static class WeightedTableJsonLoader
    {
        private sealed class Row
        {
            public int Low { get; set; }
            public int? High { get; set; }
            public string Result { get; set; }
        }

        public static Dictionary<string, WeightedTable> Load(string path)
        {
            return LoadFromJson(File.ReadAllText(path));
        }

        public static Dictionary<string, WeightedTable> LoadFromJson(string json)
        {
            var raw = JsonConvert.DeserializeObject<Dictionary<string, List<Row>>>(json);
            var result = new Dictionary<string, WeightedTable>();

            foreach (KeyValuePair<string, List<Row>> table in raw)
            {
                IEnumerable<WeightedTableRow> rows = table.Value
                    .Select(r => new WeightedTableRow(r.Low, r.High, r.Result));
                result[table.Key] = new WeightedTable(rows);
            }

            return result;
        }
    }
}
