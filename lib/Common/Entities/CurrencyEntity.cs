namespace Common.Entities
{
    public class CurrencyEntity : BaseEntity
    {
        public string Currency { get; set; } = null!;
        public decimal Rate { get; set; }
    }
}
