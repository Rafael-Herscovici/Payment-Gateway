namespace Common
{
    /// <summary>
    /// Api Constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Date separator
        /// </summary>
        public const char DateSeparator = '-';
        public static readonly string ExpiryDateFormat = $"MM{DateSeparator}yy";
        /// <summary>
        /// Character used to mask a character
        /// </summary>
        public const char MaskCharacter = '#';
        /// <summary>
        /// The default currency in the system, this is used to determine the 1.00 value of transactions
        /// </summary>
        public const string DefaultCurrency = "EUR";
    }
}
