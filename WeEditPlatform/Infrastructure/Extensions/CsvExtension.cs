namespace Infrastructure.Extensions
{
    public static class CsvExtension
    {
        public static string[] GetHeaders(this string csvfile, string[] requiredHeaders, string[]? optionalHeaders = null)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',').Select(s => s.Trim()).ToArray();

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }

            if (optionalHeaders is not null)
            {
                if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
                {
                    throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
                }
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader.ToLower()))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        public static string GetPathToFile(string csvFile, string contentRootPath, string seedDataFolder)
        {
            return Path.Combine(contentRootPath, seedDataFolder, csvFile);
        }

        public static string GetColumnValue(this string columnName, string[] columns, string[] headers)
        {
            return columns[Array.IndexOf(headers, columnName.ToLower())].Trim('"').Trim();
        }

        public static string GetColumnValueIgnoreCase(string[] columns, string[] headers, string columnName)
        {
            return columns[Array.IndexOf(headers, columnName.ToLower())].Trim('"').Trim().ToLower();
        }
    }
}
