using AutoMapper;
using Store.Core.Entities;
using AdminDashboard.Models;
using System;

namespace AdminDashboard.Helpers
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductViewModel, string>
    {
        private readonly string _baseUrl;

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            _baseUrl = configuration["BaseUrl"]; //   "BaseUrl": "https://localhost:7287/"
        }

        public string Resolve(Product source, ProductViewModel destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return null;

            // Construct the complete image URL using the base URL and relative path
            var imageUrl = $"{_baseUrl}{source.PictureUrl}";

            return imageUrl;
        }
}

}
