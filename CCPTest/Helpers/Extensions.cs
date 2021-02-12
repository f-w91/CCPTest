using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCP.Helpers
{
    public static class Extensions
    {
        public static string GetExtension(this Enum e)
        {
            var enumType = e.GetType();
            var name = Enum.GetName(enumType, e);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<ExtensionAttribute>().SingleOrDefault()?.Name;
        }
    }
}
