using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Infrastructure.Pagging
{
    public class PaginationFilterOrder
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// eg: FullName_DESC,Code_ASC,Email_ASC
        /// </summary>
        public string ColumnOrders { get; set; }

        public PaginationFilterOrder()
        {
            PageNumber = 1;
            PageSize = 10;
            ColumnOrders = "";
        }

        public PaginationFilterOrder(int pageNumber, int pageSize, string columnOrders = "")
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
            ColumnOrders = columnOrders ?? "";
        }
    }
}
