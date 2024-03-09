using ClassevivaPCTO.Helpers;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassevivaPCTO.Services
{
    internal class AnimationService
    {
        
        public static async Task<double> GetAnimationValue()
        {
            int value = await ApplicationData.Current.LocalSettings.ReadAsync<int>("TransitionIndex");

            switch (value)
            {
                case 0:
                    return 0;
                case 1:
                    return 100;
                case 2:
                    return 250;
                case 3:
                    return 400;
                default:
                    return 0;
            }
        }
    }
}
