using Store.Core.Entities;
using Store.Core.Specification;
using System.Linq.Expressions;

namespace Store.Core.Specification
{
    public class ProductWithFilterationForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFilterationForCountSpecification(ProductSpecPrams specPrams)
            : base(BuildCriteria(specPrams))
        {

        }

        private static Expression<Func<Product, bool>> BuildCriteria(ProductSpecPrams specPrams)
        {
            var searchLower = specPrams.Search?.ToLower();

            return p => (string.IsNullOrEmpty(searchLower) || p.Name.ToLower().Contains(searchLower))
                        && (!specPrams.BrandId.HasValue || p.ProductBrandId == specPrams.BrandId)
                        && (!specPrams.TypeId.HasValue || p.ProductTypeId == specPrams.TypeId);
        }
    }
}
