using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Specification
{
    public class ProductSpecPrams
    {
        public int MaxPageSize = 40;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ?MaxPageSize : value; }
        }

        public int PageIndex { get; set; } = 1;
        public string? Sort { get; set; }
        private int pageSize = 5;
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }    

        public string? search;

        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }


    }
}
