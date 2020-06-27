using System.ComponentModel.DataAnnotations;

namespace StakeTradingBot.Configuration
{
    public class AlpacaPaperTradingClientConfiguration
    {
        [Required] 
        public string ApiKey { get; set; } = null!;

        [Required]
        public string ApiSecret { get; set; } = null!;
    }
}