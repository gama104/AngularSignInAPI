using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class AppDbContext : DbContext, IDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext()
    {
        // Default parameterless constructor
    }

    public User GetUser(string username, string password)
    {
        return Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    int IDbContext.SaveChanges()
    {
        return base.SaveChanges();
    }

    public virtual DbSet<User> Users { get; set; }
}