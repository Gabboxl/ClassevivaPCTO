using System;
using System.Reflection;
using Windows.UI;
using static ClassevivaPCTO.Helpers.Palettes.Palettes;

namespace ClassevivaPCTO.Helpers.Palettes
{
    public interface IPalette
    {
        Color ColorRed { get; }
        Color ColorOrange { get; }
        Color ColorGreen { get; }
        Color ColorBlue { get; }
        Color ColorYellow { get; }
    }

    public enum PaletteType
    {
        [ClassMapping(typeof(PaletteCvv))] PALETTE_CVV,
        [ClassMapping(typeof(PaletteCon))] PALETTE_CON,
        [ClassMapping(typeof(PaletteNat))] PALETTE_NAT,
        [ClassMapping(typeof(PaletteFlo))] PALETTE_FLO,
        [ClassMapping(typeof(PaletteRet))] PALETTE_RET,
        [ClassMapping(typeof(PalettePas))] PALETTE_PAS,
        [ClassMapping(typeof(PaletteVin))] PALETTE_VIN,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ClassMappingAttribute : Attribute
    {
        public Type ClassType { get; }

        public ClassMappingAttribute(Type classType)
        {
            ClassType = classType;
        }
    }
}