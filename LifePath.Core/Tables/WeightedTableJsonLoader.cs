using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace LifePath.Core.Tables
{
    public static class WeightedTableJsonLoader
    {
        public static Dictionary<string, WeightedTable> Load(string path)
        {
            return LoadFromJson(File.ReadAllText(path));
        }

        public static Dictionary<string, WeightedTable> LoadFromJson(string json)
        {
            var raw = JsonConvert.DeserializeObject<Dictionary<string, List<WeightedTableRow>>>(json);
            var result = new Dictionary<string, WeightedTable>();

            foreach (KeyValuePair<string, List<WeightedTableRow>> table in raw)
                result[table.Key] = new WeightedTable(table.Value);

            return result;
        }
    }
}
