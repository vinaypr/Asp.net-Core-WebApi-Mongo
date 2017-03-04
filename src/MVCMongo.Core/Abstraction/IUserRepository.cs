namespace MVCMongo.Core.Abstraction
{
    using MVCMongo.Core.Model;

    public interface IUserRepository
    {
        User GetUserByName(string userName);
    }
}
