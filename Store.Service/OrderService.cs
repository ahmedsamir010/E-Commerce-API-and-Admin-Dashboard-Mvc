using Store.Core;
using Store.Core.Entities;
using Store.Core.Entities.Order;
using Store.Core.Repository;
using Store.Core.Services;
using Store.Core.Specification;
using Store.Core.Specification.OrderSpecification;
using Store.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IBasketRepository basketRepository , IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            var basket = await _basketRepository.GetBasketAsync(basketId);
            var orderItems = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var productItemOrdered = new ProductItemOrdered(item.Id, item.ProductName, item.PictureUrl);
                if (product is null)
                    return null;
                if (item.Price != product.Price)
                    item.Price = product.Price;
                var orderItem = new OrderItem(productItemOrdered, item.Price, item.Quantity);
                orderItems.Add(orderItem);
            }
            var subTotal = orderItems.Sum(OI => OI.Price * OI.Quantity);
            var spec = new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (order != null)
            {
                order.ShippingAddress = shippingAddress;
                order.DeliveryMethod = deliveryMethod;
                order.SubTotal = subTotal;
                order.Items = orderItems;
                _unitOfWork.Repository<Order>().Update(order);
            }
            else
            {
                order = new Order(buyerEmail, shippingAddress, orderItems,deliveryMethod, subTotal, basket.PaymentIntentId);
                await _unitOfWork.Repository<Order>().AddAsync(order);
            }
            await _unitOfWork.CompleteAsync();
            return order;
        }
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethods()
           => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        public async Task<Order> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);  
            var order=await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            return order;
        }
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }



    }
}
