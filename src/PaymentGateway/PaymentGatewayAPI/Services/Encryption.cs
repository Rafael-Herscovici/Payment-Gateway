using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.Services
{
    /// <summary>
    /// A utility class for encrypting/decrypting
    /// <remarks>Taken from https://github.com/jbubriski/Encryptamajig/blob/master/src/Encryptamajig/Encryptamajig/AesEncryptamajig.cs</remarks>
    /// </summary>
    public class Encryption
    {
        private const int SaltSize = 32;
        private readonly string _aesKey;

        /// <inheritdoc cref="Encryption"/>
        public Encryption(
            IOptions<PaymentGatewayOptions> options)
        {
            _aesKey = options.Value.AesEncryptionKey;
            if (string.IsNullOrEmpty(_aesKey))
                throw new ArgumentNullException(nameof(_aesKey));
        }

        /// <summary>
        /// Encrypts the plainText input using the given Key.
        /// A 128 bit random salt will be generated and prepended to the ciphertext before it is base64 encoded.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The salt and the ciphertext, Base64 encoded for convenience.</returns>
        public virtual string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            // Derive a new Salt and IV from the Key
            using var keyDerivationFunction = new Rfc2898DeriveBytes(_aesKey, SaltSize);
            var saltBytes = keyDerivationFunction.Salt;
            var keyBytes = keyDerivationFunction.GetBytes(32);
            var ivBytes = keyDerivationFunction.GetBytes(16);

            // Create an encryptor to perform the stream transform.
            // Create the streams used for encryption.
            using var aesManaged = new AesManaged();
            using var encryptor = aesManaged.CreateEncryptor(keyBytes, ivBytes);
            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                using var streamWriter = new StreamWriter(cryptoStream);
                // Send the data through the StreamWriter, through the CryptoStream, to the underlying MemoryStream
                streamWriter.Write(plainText);
            }

            // Return the encrypted bytes from the memory stream, in Base64 form so we can send it right to a database (if we want).
            var cipherTextBytes = memoryStream.ToArray();
            Array.Resize(ref saltBytes, saltBytes.Length + cipherTextBytes.Length);
            Array.Copy(cipherTextBytes, 0, saltBytes, SaltSize, cipherTextBytes.Length);

            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Decrypts the ciphertext using the Key.
        /// </summary>
        /// <param name="ciphertext">The ciphertext to decrypt.</param>
        /// <returns>The decrypted text.</returns>
        public virtual string Decrypt(string ciphertext)
        {
            if (string.IsNullOrEmpty(ciphertext))
                throw new ArgumentNullException(nameof(ciphertext));

            // Extract the salt from our cipher text
            var allTheBytes = Convert.FromBase64String(ciphertext);
            var saltBytes = allTheBytes.Take(SaltSize).ToArray();
            var ciphertextBytes = allTheBytes.Skip(SaltSize).Take(allTheBytes.Length - SaltSize).ToArray();

            using var keyDerivationFunction = new Rfc2898DeriveBytes(_aesKey, saltBytes);
            // Derive the previous IV from the Key and Salt
            var keyBytes = keyDerivationFunction.GetBytes(32);
            var ivBytes = keyDerivationFunction.GetBytes(16);

            // Create a decrytor to perform the stream transform.
            // Create the streams used for decryption.
            // The default Cipher Mode is CBC and the Padding is PKCS7 which are both good
            using var aesManaged = new AesManaged();
            using var decryptor = aesManaged.CreateDecryptor(keyBytes, ivBytes);
            using var memoryStream = new MemoryStream(ciphertextBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            // Return the decrypted bytes from the decrypting stream.
            return streamReader.ReadToEnd();
        }
    }
}
