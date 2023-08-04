namespace ClassevivaPCTO.Helpers.Palettes
{
    internal class PaletteFactory
    {
        private IPalette _currentPalette;

        public IPalette GetCurrentPalette()
        {
            return _currentPalette;
        }

        public void SetCurrentPalette(IPalette palette)
        {
            _currentPalette = palette;
        }
    }
}
