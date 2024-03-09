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

            double result;

            switch (value)
            {
                case 0:
                    result = 0;
                    break;
                case 1:
                    result = 100;
                    break;
                case 2:
                    result = 250;
                    break;
                case 3:
                    result = 500;
                    break;
                default:
                    result = 0;
                    break;
            }

            return result;
        }
    }
}
