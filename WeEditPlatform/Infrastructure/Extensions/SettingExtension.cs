using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class SettingExtension
    {
        public static string GetValueSettingByKey(this List<Setting> settings, string key)
        {
            return settings.FirstOrDefault(s => s.Key != null && s.Key == key).Value;
        }
    }
}
