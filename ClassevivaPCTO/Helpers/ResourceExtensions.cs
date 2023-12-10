using Windows.ApplicationModel.Resources;

namespace ClassevivaPCTO.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _defaultResLoader = new();

        public static string GetLocalizedStr(this string resourceKey, bool isNotTranslatable = false, string resourceFileName = null)
        {
            ResourceLoader resLoader = _defaultResLoader;

            if (isNotTranslatable)
            {
                resLoader = new ResourceLoader("untranslatable");
            }

            if (!string.IsNullOrEmpty(resourceFileName)) {
                resLoader = new ResourceLoader(resourceFileName);
            }

            return resLoader.GetString(resourceKey); 
        }

        public static string GetLocalizedStr(this string resourceKey, string tag, bool isNotTranslatable = false, string resourceFileName = null)
        {
            resourceKey += "_" + tag;
            return GetLocalizedStr(resourceKey, isNotTranslatable, resourceFileName);
        }
    }
}