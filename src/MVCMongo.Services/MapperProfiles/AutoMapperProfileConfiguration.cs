namespace MVCMongo.Services.MapperProfiles
{
    using AutoMapper;
    using MVCMongo.Core.Model;
    using MVCMongo.Core.ViewModel;

    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductViewModel, Product>();
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
        }
    }
}
