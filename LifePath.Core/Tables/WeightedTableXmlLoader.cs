using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LifePath.Core.Tables
{
    public static class WeightedTableXmlLoader
    {
        public static Dictionary<string, WeightedTable> Load(string path)
        {
            return LoadFrom(XDocument.Load(path));
        }

        public static Dictionary<string, WeightedTable> LoadFrom(XDocument doc)
        {
            var result = new Dictionary<string, WeightedTable>();

            foreach (IGrouping<string, XElement> group in doc.Root.Elements().GroupBy(e => e.Name.LocalName))
            {
                var rows = new List<WeightedTableRow>();
                bool isWeightedTable = true;

                foreach (XElement element in group)
                {
                    XElement lowElement = element.Element("rlow");
                    if (lowElement == null)
                    {
                        isWeightedTable = false;
                        break;
                    }

                    int low = int.Parse(lowElement.Value);
                    string highText = element.Element("rhigh")?.Value;
                    int? high = string.IsNullOrEmpty(highText) ? (int?)null : int.Parse(highText);
                    string resultText = element.Element("result")?.Value ?? "";

                    rows.Add(new WeightedTableRow(low, high, resultText));
                }

                if (isWeightedTable)
                    result[group.Key] = new WeightedTable(rows);
            }

            return result;
        }
    }
}
