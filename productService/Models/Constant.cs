using productService.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace productService.Models
{
    public enum Constant
    {
        [EnumDescription("普通")]
        Normal = 1,
    }
}
