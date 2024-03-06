using AutoMapper;
using SWP_Final.Entities;
using SWP_Final.Models;

namespace SWP_Final.Helpers
{
    public class ApplicationMappers : Profile
    {
        public ApplicationMappers() 
        {
            CreateMap<Agency, AgencyModel>().ReverseMap();
            CreateMap<Apartment, ApartmentModel>().ReverseMap();
            CreateMap<Booking, BookingModel>().ReverseMap();
            CreateMap<Building, BuildingModel>().ReverseMap();
            CreateMap<Customer, CustomerModel>().ReverseMap();
            CreateMap<Order, OrderModel>().ReverseMap();
            CreateMap<Post, PostModel>().ReverseMap();
            CreateMap<Project, ProjectModel>().ReverseMap();
            CreateMap<ProjectUtility, ProjectUtilityModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<Utility, UtilityModel>().ReverseMap();
        }

    }
}
