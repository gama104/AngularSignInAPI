using System.Linq;

public interface IUserRepository
{
    User GetUser(string username, string password);
    void AddUser(User user);
    void SaveChanges();
}
