namespace CurrencyExchangeDB.Entities
{
    public class CurrencyEntity
    {
        public string Currency { get; set; } = null!;
        public decimal Rate { get; set; }
    }
}
