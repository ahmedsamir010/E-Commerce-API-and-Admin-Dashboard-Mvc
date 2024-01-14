using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Core.Entities.Order
{
    public class Order : BaseEntity
    {
        public Order()
        {
        }

        public Order(string buyerEmail,Address shippingAddress, ICollection<OrderItem> items, DeliveryMethod deliveryMethod, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            Items = items;
            DeliveryMethod = deliveryMethod;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public DeliveryMethod DeliveryMethod { get; set; }
        public decimal SubTotal { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;

        public decimal GetTotal()
        {
            return SubTotal + DeliveryMethod.Cost;
        }
    }
} 
