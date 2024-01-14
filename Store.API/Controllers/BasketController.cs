using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.API.Dtos;
using Store.API.Errors;
using Store.Core.Entities;
using Store.Core.Repository;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static StackExchange.Redis.Role;
using Store.Repository;

namespace Store.API.Controllers
{
    public class BasketController : BaseController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId).ConfigureAwait(false);
            return Ok(basket ?? new CustomerBasket(basketId));
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto customerBasketDto)
        {

            var mappedBasket = _mapper.Map<CustomerBasket>(customerBasketDto);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
            if (updatedBasket is null) return BadRequest(new ApiResponse(404));

            return updatedBasket;
        }

        [HttpDelete("{basketId}")]
        public async Task<ActionResult> DeleteBasket(string basketId)
        {
            var deleted = await _basketRepository.DeleteBasketAsync(basketId).ConfigureAwait(false);

            if (deleted)
                return Ok(new ApiResponse(StatusCodes.Status200OK, "Basket deleted."));

            return NotFound(new ApiResponse(StatusCodes.Status404NotFound, "Basket not found."));
        }
    }
}
