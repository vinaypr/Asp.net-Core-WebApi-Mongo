namespace MVCMongo.Core.Abstraction
{
    using ViewModel;

    public interface IUserService
    {
        UserViewModel GetUserByName(string userName);
    }
}
