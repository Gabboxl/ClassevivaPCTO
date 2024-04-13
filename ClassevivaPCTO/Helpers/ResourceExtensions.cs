using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace ClassevivaPCTO.Helpers
{
    internal static class ResourceExtensions
    {
        private static readonly ResourceLoader DefaultResLoader = new();
        public static readonly Dictionary<string, string> CrowdinDynamicResources = new();

        public static string GetLocalizedStr(this string resourceKey, string? tag = null, string? resourceFileName = null)
        {
            if(!string.IsNullOrEmpty(tag))
                resourceKey += "_" + tag;

            if (CrowdinDynamicResources.TryGetValue(resourceKey, out string? localizedStr))
                return localizedStr;

            var resLoader = !string.IsNullOrEmpty(resourceFileName) ? new ResourceLoader(resourceFileName) : new ResourceLoader("untranslatable");
            localizedStr = resLoader.GetString(resourceKey);

            return string.IsNullOrEmpty(localizedStr) ? DefaultResLoader.GetString(resourceKey) : localizedStr;
        }
    }
}