using System.ComponentModel.DataAnnotations;

namespace CommonModels
{
    // Dev Note: In a real world example we would have a preset dictionary of currencies.
    public class Currency
    {
        [StringLength(3)]  // ISO 4217
        public string Code { get; set; } = null!;
    }
}
