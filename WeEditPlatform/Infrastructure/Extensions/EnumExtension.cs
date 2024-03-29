﻿namespace Infrastructure.Extensions
{
    public static class EnumExtension
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
