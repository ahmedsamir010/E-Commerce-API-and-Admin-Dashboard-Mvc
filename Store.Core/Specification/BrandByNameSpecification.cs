﻿using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Specification
{
    public class BrandByNameSpecification : BaseSpecification<ProductBrand>
    {
        public BrandByNameSpecification(string brandName)
            : base(b => b.Name.ToLower() == brandName.ToLower())
        {
        }
    }
}
