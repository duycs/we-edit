namespace Domain
{
    public class Customer : EntityBase
    {
        public string CoCode { get; set; }
        public Location Location { get; set; }
        public Currency Currency { get; set; }
        public string CodeProduct { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public string Saler { get; set; }
        public string Note { get; set; }
        public double PSE { get; set; }
        public double PSW { get; set; }


        public static Customer Create(string coCode, Location location, Currency currency,
            string codeProduct, string productName = "", double unitPrice = 0, string saler = "",
            string note = "", double pse = 0, double psw = 0)
        {
            return new Customer()
            {
                CoCode = coCode,
                Location = location,
                Currency = currency,
                CodeProduct = codeProduct,
                ProductName = productName,
                UnitPrice = unitPrice,
                Saler = saler,
                Note = note,
                PSE = pse,
                PSW = psw
            };
        }

        public string GetCompanyProductCode()
        {
            return this.CoCode + this.CodeProduct;
        }
    }
}
