using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Specification.OrderSpecification
{
    public class ProductRatingSpecification : BaseSpecification<ProductRating>
    {
        public ProductRatingSpecification(int productId, string email) : base(r => r.ProductId == productId && r.Email == email)
        {

        }
    }
}
