using Infrastructure.Models;

namespace Infrastructure.Pagging
{
    public static class QueriesExtensions
    {
        public static List<ColumnOrder> ToColumnOrders(this string queriesString)
        {
            if (string.IsNullOrEmpty(queriesString))
                return new List<ColumnOrder>();

            var columnOrders = new List<ColumnOrder>();
            var queries = queriesString.Split(',');

            // only 1 column order
            if (!queries.Any())
            {
                var items = queriesString.Split("_");
                if (items.Any() && items.Length == 2)
                {
                    var order = (Order)Enum.Parse(typeof(Order), items[1], true);
                    var columnOrder = new ColumnOrder(items[0], order);
                    columnOrders.Add(columnOrder);
                }
            }
            else
            {
                // many column order
                foreach (var query in queries)
                {
                    var items = query.Split("_");
                    if (items.Any() && items.Length == 2)
                    {
                        var order = (Order)Enum.Parse(typeof(Order), items[1], true);
                        var columnOrder = new ColumnOrder(items[0], order);
                        columnOrders.Add(columnOrder);
                    }
                }
            }

            return columnOrders;
        }

        public static string ToIdsQueries(this int[] ids, bool isInclude)
        {
            var queryIds = "";
            if (ids is not null && ids.Any())
            {
                foreach (var id in ids)
                {
                    queryIds += $"ids={id}&";
                }
            }

            return queryIds += $"isInclude={isInclude}";
        }

        public static string ToQueries(this Dictionary<string, string> keyValueParams, bool isInclude)
        {
            var queries = "";
            if (keyValueParams is not null && keyValueParams.Any())
            {
                foreach (var keyValue in keyValueParams)
                {
                    if (keyValue.Value is not null)
                    {
                        queries += $"{keyValue.Key}={keyValue.Value}&";
                    }
                }
            }

            return queries += $"isInclude={isInclude}";
        }
    }
}
