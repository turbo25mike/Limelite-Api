using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Business.DataStores;

public static class DataTableExt
{
    public static DataTable CreateTable(IDictionary<string, object>[] data)
    {
        var table = new DataTable();
        var columns = new List<string>();
        columns = data[0].Select(d => d.Key).ToList();

        foreach (var column in columns)
            table.Columns.Add(column);

        foreach (var dictionary in data)
        {
            var row = table.NewRow();
            foreach (var keyVal in dictionary)
            {
                var itemMap = keyVal.Key;

                row[itemMap] = keyVal.Value;
            }

            table.Rows.Add(row);
        }

        return table;
    }
}