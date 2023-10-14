using Windows.ApplicationModel.Resources;

namespace ClassevivaPCTO.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _resLoader = new();

        public static string GetLocalized(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }

        public static string GetLocalized(this string resourceKey, string tag)
        {
            resourceKey += "_" + tag;
            return GetLocalized(resourceKey);
        }
    }
}