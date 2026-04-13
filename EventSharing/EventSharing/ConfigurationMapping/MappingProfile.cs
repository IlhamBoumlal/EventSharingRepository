using AutoMapper;
using EventSharing.Models;
using EventSharing.ViewModels;

namespace EventSharing.ConfigurationMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Event,EventViewModel>()
                //lorsqu'on a les meme propriétés dans les deux classes,
                //automapper les map automatiquement, mais lorsqu'on a des propriétés
                //différentes, on doit leur dire comment les mapper
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => 
                src.Category.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src =>
                src.Category.Id))
                .ReverseMap();
            CreateMap<Category, CategoryViewModel>()
                .ReverseMap();
            CreateMap<User, UserViewModel>()
                .ReverseMap();
        }

    }
}
