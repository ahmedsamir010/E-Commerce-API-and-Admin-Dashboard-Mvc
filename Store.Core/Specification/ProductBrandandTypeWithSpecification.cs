using Store.Core.Entities;
using Store.Core.Specification;
using System;
using System.Linq.Expressions;

namespace Store.Core.Specification
{
    public class ProductBrandAndTypeWithSpecification : BaseSpecification<Product>
    {
        public ProductBrandAndTypeWithSpecification(ProductSpecPrams spec)
            : base(BuildCriteria(spec))
        {
            IncludeProductBrandAndType();
            ApplySorting(spec.Sort);
            ApplyPagination((spec.PageIndex - 1) * spec.PageSize, spec.PageSize);
        }

        public ProductBrandAndTypeWithSpecification(int id): base(p => p.Id == id)
        {
            IncludeProductBrandAndType();
        }

        private static Expression<Func<Product, bool>> BuildCriteria(ProductSpecPrams spec)
        {
            return p => (string.IsNullOrEmpty(spec.Search) || p.Name.ToLower().Contains(spec.Search.ToLower()))
                        && (!spec.BrandId.HasValue || p.ProductBrandId == spec.BrandId)
                        && (!spec.TypeId.HasValue || p.ProductTypeId == spec.TypeId);
        }

        private void IncludeProductBrandAndType()
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);
            Includes.Add(P => P.ProductRating);
        }

        private void ApplySorting(string sort)
        {
            switch (sort?.ToLower())
            {
                case "priceasc":
                    ApplyOrderBy(p => p.Price);
                    break;
                case "pricedesc":
                    ApplyOrderByDescending(p => p.Price);
                    break;
                default:
                    ApplyDefaultSorting();
                    break;
            }
        }

        private void ApplyDefaultSorting()
        {
            ApplyOrderBy(p => p.Id); // Default sorting by product ID
        }
    }
}
