using Microsoft.AspNetCore.Mvc;
using Store.API.Errors;
using Store.API.Helpers;
using Store.Core;
using Store.Core.Repository;
using Store.Core.Services;
using Store.Repository;
using Store.Service;

namespace Store.API.ExtentionMethods
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            services.AddScoped(typeof(ITokenService), typeof(TokenService));
            services.AddScoped(typeof(IEmailSettings), typeof(EmailSettings));
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddApiBehaviorOptions();
            services.AddHttpContextAccessor();

            return services;
        }

        public static void AddApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var errorResponse = new ApiValidationErrorResponse { Errors = errors };
                    return new BadRequestObjectResult(errorResponse);
                };
            });
           

        }
    }
}
