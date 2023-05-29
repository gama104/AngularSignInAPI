using Microsoft.EntityFrameworkCore;

public interface IDbContext
{
    DbSet<User> Users { get; set; }
    User GetUser(string username, string password);
    int SaveChanges();
}
