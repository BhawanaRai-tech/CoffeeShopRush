using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopRush.Models;

public class GameSession
{
    [Key]
    public int SessionId { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    
    public int FinalScore { get; set; }
    
    public int CustomersServed { get; set; }
    
    public int CustomersLost { get; set; }
    
    public int SessionDuration { get; set; }
    
    public virtual User? User { get; set; }
}