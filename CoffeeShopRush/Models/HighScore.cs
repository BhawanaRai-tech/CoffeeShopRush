using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopRush.Models;

public class HighScore
{
    [Key]
    public int ScoreId { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    public int Score { get; set; }
    
    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
    
    public int? GameSessionId { get; set; }
    
    public virtual User? User { get; set; }
    public virtual GameSession? GameSession { get; set; }
}