using System.ComponentModel.DataAnnotations;

namespace StakeTradingBot.Configuration
{
    public class StakeConfiguration
    {
        [Required] 
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}