using AdminDashboard.Models;
using AutoMapper;
using Store.API.Helpers;
using Store.Core.Entities;

namespace AdminDashboard.Helpers
{
    public class MapsProfile : Profile
    {
        public MapsProfile()
        {
            CreateMap<Product, ProductViewModel>()
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<ProductPictureUrlResolver>()).ReverseMap();
        }
    }
}
