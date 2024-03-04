using ClassevivaPCTO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassevivaPCTO.Utils
{
    public class Animations
    {
        double result;
        
        private async void readvalue()
        {
            result = await ApplicationData.Current.LocalSettings.ReadAsync<double>("AnimationsValue");
        }

        public double GetAnimationsValue()
        {
            return result;
        }
    }
}
