using System.ComponentModel.DataAnnotations;

namespace CommonModels
{
    // Dev Note: In a real world example we would have a preset dictionary of currencies.
    public class Currency
    {
        [StringLength(3)]
        public string Code { get; set; } // ISO 4217
    }
}
