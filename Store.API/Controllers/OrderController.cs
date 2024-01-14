using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.API.Dtos;
using Store.API.Errors;
using Store.Core.Entities.Order;
using Store.Core.Services;
using Store.Service;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static StackExchange.Redis.Role;

namespace Store.API.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("CreateOrder")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var mappedShippingAddres = _mapper.Map<Address>(orderDto.ShippingAddress);
            var order = await _orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, mappedShippingAddres);
            if (order is null)
                return BadRequest(new ApiResponse(400));
            var mappedOrder = _mapper.Map<OrderToReturnDto>(order);
            return Ok(mappedOrder);
        }

        [HttpGet("GetOrderForUser")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);
            var mappedOrders = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(mappedOrders);
        }

        [HttpGet("GetDeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethod=await _orderService.GetDeliveryMethods();
            return Ok(deliveryMethod);
        }

        [HttpGet("GetOrderByIdForUser")]

        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int orderId)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForUserAsync(buyerEmail, orderId);
            var mappedOrder = _mapper.Map<OrderToReturnDto>(order);
            return Ok(mappedOrder);

        }

    }
}
