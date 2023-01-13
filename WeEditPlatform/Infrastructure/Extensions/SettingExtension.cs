using Domain;

namespace Infrastructure.Extensions
{
    public static class SettingExtension
    {
        public static string GetValueSettingByKey(this List<Setting> settings, string key)
        {
            var setting = settings.FirstOrDefault(s => s.Key != null && s.Key.ToLower() == key.ToLower());

            if (setting == null)
            {
                throw new Exception($"Not found Setting key {key}");
            }

            return setting.Value;
        }
    }
}
