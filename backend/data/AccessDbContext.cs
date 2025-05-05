using backend.entity;
using Microsoft.EntityFrameworkCore;

namespace backend.data;

public class AccessDbContext : DbContext
{
    public AccessDbContext(DbContextOptions<AccessDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
}