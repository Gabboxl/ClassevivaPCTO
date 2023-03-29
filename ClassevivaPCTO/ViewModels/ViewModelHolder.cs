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
    public static class ViewModelHolder
    {
        private static AppViewModel viewModel;

        public static AppViewModel getViewModel()
        {
            if(viewModel == null)
            {
                viewModel = new AppViewModel();
            }

            return viewModel;
        }


    }
}
