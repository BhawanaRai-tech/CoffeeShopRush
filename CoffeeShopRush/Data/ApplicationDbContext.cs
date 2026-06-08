using Microsoft.EntityFrameworkCore;
using CoffeeShopRush.Models;

namespace CoffeeShopRush.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<HighScore> HighScores { get; set; }
}