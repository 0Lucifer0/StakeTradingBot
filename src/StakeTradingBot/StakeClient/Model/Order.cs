using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class Order
    {
        public string Symbol { get; set; } = string.Empty;

        public float Quantity { get; set; }

        public TransactionType TransactionType { get; set; }
    }
}
