using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Entities;

public class MyBoardsContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<MessageToUser> MessagesToUser { get; set; }

    public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(eb =>
        {
            eb.Property(u => u.Password).IsRequired();
            eb.HasMany(u => u.Inbox)
                .WithOne(m => m.User)
                .HasForeignKey(k => k.UserId);
        });

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433; Database=Users;User=sa; Password=1StrongPassword!");
    }
}