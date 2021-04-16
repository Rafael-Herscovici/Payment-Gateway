namespace CommonAPI.Models
{
    public class ApiOptions
    {
        /// <summary>
        /// The aes key used for encrypt/decrypt
        /// </summary>
        public string AesEncryptionKey { get; set; } = null!;
    }
}
