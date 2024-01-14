using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.API.Dtos;
using Store.API.Errors;
using Store.API.Helpers;
using Store.Core;
using Store.Core.Entities;
using Store.Core.Entities.Identity;
using Store.Core.Repository;
using Store.Core.Specification;
using Store.Core.Specification.OrderSpecification;
using Store.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static StackExchange.Redis.Role;

namespace Store.API.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> userManager;

        public ProductController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
             UserManager<AppUser> userManager

            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet("GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery] ProductSpecPrams productSpecPrams)
        {
            var spec = new ProductBrandAndTypeWithSpecification(productSpecPrams);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var countSpec = new ProductWithFilterationForCountSpecification(productSpecPrams);
            var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            var pagination = new Pagination<ProductDto>(productSpecPrams.PageSize, productSpecPrams.PageIndex, count, mappedProducts);

            return Ok(pagination);
        }

        [HttpGet("GetProduct/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {   
            var productSpec = new ProductBrandAndTypeWithSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(productSpec);

            if (product == null)
                return NotFound(new ApiResponse(404));

            var mappedProduct = _mapper.Map<ProductDto>(product);
            return Ok(mappedProduct);
        }








        [HttpGet("GetProductBrands")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ProductBrand>))]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("GetProductTypes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ProductType>))]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(types);
        }
        
        [Authorize]
        [HttpPost("AddRating")]
        public async Task<ActionResult<RatingDto>> AddRating(RatingDto RatingDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);
            var spec = new ProductRatingSpecification(RatingDto.ProductId, user.Email);
            var existingRating = await _unitOfWork.Repository<ProductRating>().GetEntityWithSpecAsync(spec);
            if (existingRating is not null)
            {
                existingRating.Rating = RatingDto.Rating;
                existingRating.Message = RatingDto.Message;
                _unitOfWork.Repository<ProductRating>().Update(existingRating);
            }
            else
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(RatingDto.ProductId);
                if (product is null) return BadRequest(new ApiResponse(404, "Product Not found"));
                var rating = new ProductRating()
                {
                    Email = user.Email,
                    ProductId = RatingDto.ProductId,
                    Rating = RatingDto.Rating,
                    Message = RatingDto.Message,
                    UserName = user.UserName
                };
                await _unitOfWork.Repository<ProductRating>().AddAsync(rating);
            }
            await _unitOfWork.CompleteAsync();
            return Ok(RatingDto);
        }
    }
}
