using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace ClassevivaPCTO.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _defaultResLoader = new();
        public static Dictionary<string, string> CrowdinDynamicResources = new();

        public static string GetLocalizedStr(this string resourceKey, bool isNotTranslatable = false, string? resourceFileName = null)
        {
            if (CrowdinDynamicResources.TryGetValue(resourceKey, out string? localizedStr))
            {
                return localizedStr;
            }

            ResourceLoader resLoader = _defaultResLoader;

            if (isNotTranslatable)
                resLoader = new ResourceLoader("untranslatable");
            else if (!string.IsNullOrEmpty(resourceFileName))
                resLoader = new ResourceLoader(resourceFileName);

            return resLoader.GetString(resourceKey);
        }

        // GetLocalizedStr with tag
        public static string GetLocalizedStr(this string resourceKey, string tag, bool isNotTranslatable = false, string? resourceFileName = null)
        {
            resourceKey += "_" + tag;
            return GetLocalizedStr(resourceKey, isNotTranslatable, resourceFileName);
        }
    }
}