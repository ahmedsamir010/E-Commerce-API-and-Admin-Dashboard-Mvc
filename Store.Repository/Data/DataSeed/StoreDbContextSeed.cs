using Store.Core.Entities;
using Store.Core.Entities.Order;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Repository.Data.DataSeed
{
    public static class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext context)
        {
            await SeedProductBrandsAsync(context);
            await SeedProductTypesAsync(context);
            await SeedDeliveryMethodsAsync(context);
            await SeedProductsAsync(context);
        }

        private static async Task SeedProductBrandsAsync(StoreDbContext context)
        {
            if (!context.ProductBrands.Any())
            {
                var productBrandsJson = await File.ReadAllTextAsync("../Store.Repository/Data/DataSeed/JsonDataSeed/brands.json");
                var productBrandsData = JsonSerializer.Deserialize<List<ProductBrand>>(productBrandsJson);
                if (productBrandsData?.Any() == true)
                {
                    await context.ProductBrands.AddRangeAsync(productBrandsData);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedProductTypesAsync(StoreDbContext context)
        {
            if (!context.ProductTypes.Any())
            {
                var productTypesJson = await File.ReadAllTextAsync("../Store.Repository/Data/DataSeed/JsonDataSeed/types.json");
                var productTypesData = JsonSerializer.Deserialize<List<ProductType>>(productTypesJson);
                if (productTypesData?.Any() == true)
                {
                    await context.ProductTypes.AddRangeAsync(productTypesData);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedDeliveryMethodsAsync(StoreDbContext context)
        {
            if (!context.DeliveryMethods.Any())
            {
                var deliveryMethodsJson = await File.ReadAllTextAsync("../Store.Repository/Data/DataSeed/JsonDataSeed/delivery.json");
                var deliveryMethodsData = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsJson);
                if (deliveryMethodsData?.Any() == true)
                {
                    await context.DeliveryMethods.AddRangeAsync(deliveryMethodsData);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedProductsAsync(StoreDbContext context)
        {
            if (!context.Products.Any())
            {
                var productsJson = await File.ReadAllTextAsync("../Store.Repository/Data/DataSeed/JsonDataSeed/products.json");
                var productsData = JsonSerializer.Deserialize<List<Product>>(productsJson);
                if (productsData?.Any() == true)
                {
                    await context.Products.AddRangeAsync(productsData);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
