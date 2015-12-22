using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SBSC.Lib.Helpers
{
    public class EnumsHelper
    {
        public static String GetText(Object aEnum)
        {
            if (((Object)aEnum) == null) throw new ArgumentNullException("aEnum");
            var vEnumType = aEnum.GetType();
            if (!vEnumType.IsEnum)
                throw new Exception("Object is not an Enum");
            return GetText(vEnumType, aEnum.ToString());
        }

        public static String GetText(Type aEnumType, int aKey)
        {
            if (!aEnumType.IsEnum)
                throw new Exception("Type must be an Enum type");
            var vEnumName = aEnumType.GetEnumName(aKey);
            return GetText(aEnumType, vEnumName);
        }

        private static string GetText(Type aEnumType, string vEnumName)
        {
            var vMemberInfo = aEnumType.GetMember(vEnumName);
            var vAttributes = vMemberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            return vAttributes.Length > 0 ? ((DisplayAttribute)vAttributes[0]).Name : vEnumName;
        }

        public static Dictionary<int, String> GetValueDisplayDictionary(Type aEnumType)
        {
            if (((Object)aEnumType) == null) throw new ArgumentNullException("aEnumType");
            if (!aEnumType.IsEnum)
                throw new Exception("Type must be an Enum");

            return Enum.GetValues(aEnumType).Cast<object>().ToDictionary(vEnumValue => (int)vEnumValue, GetText);
        }

        public static List<SelectListItem> GetSelectListItems(Type aEnumType, int? value = null)
        {
            if (((Object)aEnumType) == null) throw new ArgumentNullException("aEnumType");
            if (!aEnumType.IsEnum)
                throw new Exception("Type must be an Enum");

            return (from object vEnumValue in Enum.GetValues(aEnumType)
                    select new SelectListItem { Value = ((int)vEnumValue).ToString(), Text = GetText(vEnumValue), Selected = value.HasValue && ((int)vEnumValue) == value.Value }).ToList();
        }
    }
}
