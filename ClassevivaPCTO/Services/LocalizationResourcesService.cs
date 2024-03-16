using System.Threading.Tasks;
using Windows.Globalization;
using Windows.UI.Xaml.Resources;
using ClassevivaPCTO.Helpers;
using Crowdin.Net;

namespace ClassevivaPCTO.Services
{
    internal class LocalizationResourcesService : CustomXamlResourceLoader
    {
        public static async Task InitializeAsync()
        {
            DynamicResourcesLoader.GlobalOptions.DistributionHash = "7be9712a290193b21da47d7rnjd";

            string crowdinFileName;
            if (string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride) || ApplicationLanguages.PrimaryLanguageOverride == ApplicationLanguages.ManifestLanguages[0])
              crowdinFileName = ""; //do not use mapping as values are weird, chiedere sul forum crowdin //   crowdinFileName = $"mapping/main/ClassevivaPCTO/Strings/{ApplicationLanguages.ManifestLanguages[0]}/Resources.resw";
            else
             crowdinFileName = $"content/main/ClassevivaPCTO/Strings/{ApplicationLanguages.PrimaryLanguageOverride}/Resources.resw";

            DynamicResourcesLoader.GlobalOptions.UseCache = false;
            await DynamicResourcesLoader.LoadCrowdinStrings(crowdinFileName, ResourceExtensions.CrowdinDynamicResources, false);

            // Set the custom resource loader to use in the app
            CustomXamlResourceLoader.Current = new LocalizationResourcesService();
        }

        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            return resourceId.GetLocalizedStr();
        }
    }
}
