using System;
using System.ComponentModel.DataAnnotations;
using StakeTradingBot.StakeClient.Model;

namespace StakeTradingBot
{
    public class Order
    { 
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public string Symbol { get; set; } = string.Empty;

        public float Quantity { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime Time { get; set; }
    }
}
