using System.Linq;

public class UserRepository : IUserRepository
{
    private readonly IDbContext _dbContext;

    public UserRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User GetUser(string username, string password)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public void AddUser(User user)
    {
        _dbContext.Users.Add(user);
    }
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}
