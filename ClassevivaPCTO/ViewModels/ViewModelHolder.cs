namespace ClassevivaPCTO.ViewModels
{
    public static class ViewModelHolder
    {
        private static AppViewModel viewModel;

        public static AppViewModel getViewModel()
        {
            if (viewModel == null)
            {
                viewModel = new AppViewModel();
            }

            return viewModel;
        }
    }
}
