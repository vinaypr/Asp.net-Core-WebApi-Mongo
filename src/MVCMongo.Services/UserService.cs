namespace MVCMongo.Services
{
    using MVCMongo.Core.Abstraction;
    using MVCMongo.Core.ViewModel;
    using AutoMapper;

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public UserViewModel GetUserByName(string userName)
        {
           var user = _userRepository.GetUserByName(userName);
            return Mapper.Map<UserViewModel>(user);
        }
    }
}
