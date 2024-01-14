using Store.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Specification
{
    public class OrderWithPaymentIntentIdSpecifications: BaseSpecification<Order>
    {
        public OrderWithPaymentIntentIdSpecifications(string paymentIntentId):base(p=>p.PaymentIntentId == paymentIntentId)
        {
            
        }
    }
}
