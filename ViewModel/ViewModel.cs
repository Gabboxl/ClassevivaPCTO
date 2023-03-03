using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace ClassevivaPCTO.ViewModel
{
    public class ViewModel : ObservableObject
    {
        private LoginResult loginResult;


        public LoginResult LoginResult
        {
            get => loginResult;
            set => SetProperty(ref loginResult, value);
        }

    }
}
