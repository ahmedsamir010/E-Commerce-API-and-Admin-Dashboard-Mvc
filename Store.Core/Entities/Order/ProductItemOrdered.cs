using Microsoft.EntityFrameworkCore;

namespace Store.Core.Entities.Order
{
    [Owned]
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
        }

        public ProductItemOrdered(int productId, string productName, string pictureUrl)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
        }

            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PictureUrl { get; set; }
    }
}
