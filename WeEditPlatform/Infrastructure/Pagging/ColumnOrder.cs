using Infrastructure.Models;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Infrastructure.Pagging
{
    public class ColumnOrder
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Order Order { get; set; }

        public ColumnOrder(string name, Order order)
        {
            Name = name;
            Order = order;
        }
    }
}
