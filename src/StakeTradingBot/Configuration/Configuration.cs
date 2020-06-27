using System.ComponentModel.DataAnnotations;

namespace StakeTradingBot.Configuration
{
    public class Configuration
    {
        [Required]
        public StakeConfiguration StakeClient { get; set; } = null!;
    }
}
