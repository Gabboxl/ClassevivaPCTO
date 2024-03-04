using System;
using System.Reflection;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ApiValueAttribute : Attribute
    {
        public string ShortName { get; }
        public string LongName { get; }

        public ApiValueAttribute(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }
    }

    public static class EnumExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Enum enumValue,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            Type enumType = enumValue.GetType();
            MemberInfo[] memberInfoArray = enumType.GetMember(enumValue.ToString());
            if (memberInfoArray.Length == 0)
            {
                throw new ArgumentException($"No member found for enum value {enumValue}");
            }

            TAttribute attribute = memberInfoArray[0].GetCustomAttribute<TAttribute>();
            if (attribute == null)
            {
                throw new ArgumentException($"No attribute found for enum value {enumValue}");
            }

            return valueSelector(attribute);
        }

        public static string GetShortName(this Enum enumValue)
        {
            return enumValue.GetAttributeValue((ApiValueAttribute attr) => attr.ShortName);
        }

        public static string GetLongName(this Enum enumValue)
        {
            return enumValue.GetAttributeValue((ApiValueAttribute attr) => attr.LongName);
        }
    }
}