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
        [ClassMapping(typeof(PaletteCv))]
        PALETTE_CV,
        [ClassMapping(typeof(PaletteJap))]
        PALETTE_JAP,
        [ClassMapping(typeof(PaletteNat))]
        PALETTE_NAT,
        [ClassMapping(typeof(Palette4))]
        PALETTE_4,
        [ClassMapping(typeof(Palette5))]
        PALETTE_5,

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
