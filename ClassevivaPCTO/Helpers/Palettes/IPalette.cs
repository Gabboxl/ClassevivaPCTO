using System;
using System.Reflection;
using Windows.UI;
using static ClassevivaPCTO.Helpers.Palettes.Palettes;

namespace ClassevivaPCTO.Helpers.Palettes
{
    internal interface IPalette
    {
        Color ColorRed { get; }
        Color ColorOrange { get; }
        Color ColorYellow { get; }
        Color ColorGreen { get; }

    }

    public enum PaletteType
    {
        [ClassMapping(typeof(PaletteCv))]
        PALETTE_CV,
        [ClassMapping(typeof(PaletteJap))]
        PALETTE_JAP,
        [ClassMapping(typeof(PaletteNat))]
        PALETTE_NAT
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
