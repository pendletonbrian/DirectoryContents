using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DirectoryContents.Classes
{
    public static class Extensions
    {
        public static string GetDescription(this Enum val)
        {
            var attribute = (DescriptionAttribute) val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault();

            return attribute == default(DescriptionAttribute) ? val.ToString() : attribute.Description;
        }

        
    }
}
