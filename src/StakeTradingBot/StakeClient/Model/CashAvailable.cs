using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class CashAvailable
    {
        [JsonPropertyName("cashAvailableForWithdrawal")]
        public float CashAvailableForWithdrawal { get; set; }

        [JsonPropertyName("cashAvailableForTrade")]
        public float CashAvailableForTrade { get; set; }

        [JsonPropertyName("cashBalance")]
        public float CashBalance { get; set; }

        [JsonPropertyName("reservedCash")]
        public float ReservedCash { get; set; }

        [JsonPropertyName("dwCashAvailableForWithdrawal")]
        public float DwCashAvailableForWithdrawal { get; set; }

        [JsonPropertyName("pendingOrdersAmount")]
        public float PendingOrdersAmount { get; set; }

        [JsonPropertyName("pendingWithdrawals")]
        public float PendingWithdrawals { get; set; }

        [JsonPropertyName("cardHoldAmount")]
        public float CardHoldAmount { get; set; }

        [JsonPropertyName("pendingPoliAmount")]
        public float PendingPoliAmount { get; set; }

        [JsonPropertyName("cashSettlement")]
        public CashSettlement[] CashSettlement { get; set; } = null!;
    }

    public class CashSettlement
    {
        [JsonPropertyName("utcTime")] 
        public string UtcTime { get; set; } = string.Empty;

        [JsonPropertyName("cash")]
        public float Cash { get; set; }
    }
}
