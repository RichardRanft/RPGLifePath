using System;
using System.Collections.Generic;

namespace LifePath.Core.Tables
{
    public readonly struct WeightedTableRow
    {
        public int Low { get; }
        public int? High { get; }
        public string Result { get; }

        public WeightedTableRow(int low, int? high, string result)
        {
            Low = low;
            High = high;
            Result = result;
        }
    }

    public sealed class WeightedTable
    {
        private readonly List<WeightedTableRow> m_rows;

        public int RangeLow { get; }
        public int RangeHigh { get; }
        public IReadOnlyList<WeightedTableRow> Rows => m_rows;

        public WeightedTable(IEnumerable<WeightedTableRow> rows)
        {
            m_rows = new List<WeightedTableRow>(rows);

            int lval = 1;
            int hval = 1;
            foreach (WeightedTableRow row in m_rows)
            {
                if (row.Low < lval)
                    lval = row.Low;
                if (row.High.HasValue && row.High.Value > hval)
                    hval = row.High.Value;
            }
            RangeLow = lval;
            RangeHigh = hval;
        }

        public string Roll(Random rand)
        {
            int roll = rand.Next(RangeLow, RangeHigh + 1);
            foreach (WeightedTableRow row in m_rows)
            {
                bool isMatch = row.High.HasValue
                    ? roll >= row.Low && roll <= row.High.Value
                    : roll == row.Low;
                if (isMatch)
                    return row.Result == "#" ? roll.ToString() : row.Result;
            }
            return "";
        }
    }
}
