using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SortingBenchmarkUI
{
    class DataGridIndexerAdapter
    {
        DataTable _table = new();

        public DataGridIndexerAdapter(IEnumerable<string> rows, IEnumerable<string> columns)
        {
            foreach (string column in columns)
                _table.Columns.Add(new DataColumn(column));

            foreach (string row in rows)
                _table.Rows.Add(row);

            _table.PrimaryKey = new[] { _table.Columns[0] };
        }

        public DataView DataView => _table.DefaultView;

        public string this[string row, string column]
        {
            get => _table.Rows.Find(row)[column].ToString();
            set => _table.Rows.Find(row)[column] = value;
        }

        public void AddColumns(IEnumerable<string> columns) =>
            _table.Columns.AddRange(columns.Select(s => new DataColumn(s)).ToArray());
    }
}
