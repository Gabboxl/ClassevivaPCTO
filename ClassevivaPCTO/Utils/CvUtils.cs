using System.Net.Http;
using Newtonsoft.Json;
using Refit;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;

namespace ClassevivaPCTO.Utils
{
    public class CvUtils
    {
        public static async Task<object> GetApiLoginData(IClassevivaAPI apiClient, LoginData loginData)
        {
            var res = await apiClient.LoginAsync(loginData);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = await res.Content.ReadAsStringAsync();

                if (responseContent.Contains("choice"))
                {
                    LoginResultChoices loginResultchoices =
                        JsonConvert.DeserializeObject<LoginResultChoices>(responseContent);
                    return loginResultchoices;
                }
                else
                {
                    LoginResultComplete loginResult =
                        JsonConvert.DeserializeObject<LoginResultComplete>(responseContent);
                    return loginResult;
                }
            }
            else
            {
                // Create RefitSettings object
                RefitSettings refitSettings = new RefitSettings();

                // Create ApiException with request and response details
                throw await ApiException.Create(
                    res.RequestMessage,
                    HttpMethod.Get,
                    res,
                    refitSettings
                );
            }
        }


        public string GetCode(string userId)
        {
            return Regex.Replace(userId, @"[A-Za-z]+", "");
        }


        public static SolidColorBrush GetColorFromAbsenceCode(AbsenceEventCode valore)
        {
            SolidColorBrush brush = new SolidColorBrush();

            IPalette CurrentPalette = PaletteSelectorService.PaletteClass;

            switch (valore)
            {
                case AbsenceEventCode.ABA0:
                    brush.Color = CurrentPalette.ColorRed;
                    break;

                case AbsenceEventCode.ABR0:
                    brush.Color = CurrentPalette.ColorOrange;
                    break;

                case AbsenceEventCode.ABR1:
                    brush.Color = CurrentPalette.ColorOrange;
                    break;

                case AbsenceEventCode.ABU0:
                    brush.Color = CurrentPalette.ColorYellow;
                    break;
            }

            return brush;
        }
    }
}