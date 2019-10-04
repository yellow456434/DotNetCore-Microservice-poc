using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace productService.Attributes
{
    public class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
