using System.ComponentModel.DataAnnotations;

namespace CoffeeShopRush.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string? GoogleId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public int TotalScore { get; set; }
    
    public int GamesPlayed { get; set; }
}