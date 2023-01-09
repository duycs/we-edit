namespace Infrastructure.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime GetDefaultDateTime()
        {
            return default(DateTime);
        }

        public static DateTime GetDateFromTicks(this long ticks)
        {
            return new DateTime(ticks);
        }
    }
}
