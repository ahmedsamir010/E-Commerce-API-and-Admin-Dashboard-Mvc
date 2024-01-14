using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.API.Dtos;
using Store.Core.Entities;

namespace Store.API.Helpers
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly string baseUrl;

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            baseUrl = configuration["BaseUrl"];
        }

        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
            {
                return null;
            }
            string pictureUrl = source.PictureUrl;

            // Ensure the base URL ends with a slash
            string modifiedBaseUrl = baseUrl.EndsWith('/') ? baseUrl : baseUrl + '/';

            string resolvedUrl = modifiedBaseUrl + pictureUrl;

            return resolvedUrl;
        }
    }
}
