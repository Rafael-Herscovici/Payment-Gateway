# Payment Gateway exercise

#### Requirments summary

Build a Gateway to charge a _Shopper_ for a _Merchant_, using the shopper's _Bank_.

#### Extra mile points:

1. Application logging - Done, (Serilog) Logs to Consols and File, minimal
2. Application metrics - Done, (ApplicationInsights) Added very basic application insights telemetry (see PaymentController:ProcessPaymentAsync), needs to be spec'd out
3. Containerization    - Done, (Docker) Dockerfile for each container and Docker-Compose file included, Please note SSL was disabled due to limitations of localhost cert.
4. Authentication      - Not done. While very easy to implement, it requires a spec (e.g what kinds of authentication? JWT / OAuth / AD / Windows / Database).
5. API Client          - Not done, missing spec of what the api client should do, on the other hand, the BankEmulator is an API and the GatewayAPI has a typed client communicating with it.
6. Build script / CI   - Not done, Provider dependant.
7. Performance testing - Not done, Provider dependant, Creating local performance tests is not providing any value.
8. Encryption          - Done, CardData on the Gateway is encrypted decrypted, Please note since the Bank is not the main project here, no encryption was used.
9. Data Storage        - (LocalDb/MSSQL) While its a vogue description, i guess it refers to persistance storage, Provided both with LocalDb when running locally, and SQLServer when running in docker.

#### Other implementations
1. Automapper
2. Typed HttpClient using HttpClientFactory
3. Polly for HttpClient retry mechanism
4. Swagger documentation
5. EntityFramework (+Migrations +DbMigrator service)
6. Custom validation attributes (See CardExpiryDateValidator), Peronally i prefer FluentValidation
7. CurrencyExchange project which runs as a timed background service fetching the VALID currencies for the bank and their rates.

#### Personal notes:
1. Please be aware there are quick a few "Dev note:" comments in the code, Mainly explaining choices i have made.
2. The docker image of SQLServer has known issues, if Docker-Compose dosent run in first try, Please try a few more times.
3. Unit testing was done for PaymentGateway only, with a 98% coverage, please note i have ommited verification tests for Telemtry/Logging, even though those should be added.
4. Merchant id is not validated, since we have no authentication scheme.
5. The bank database is seeded with a single account and the following payload could be used (on either API) to charge from the account:

```json
{
  "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", // Can be any guid
  "amount": 10, // Can be any amount, note: the seeded account has 1000 EUR initially
  "currency": "GBP", // Can be any VALID currency (See CurrencyExchange project)
  "cardDetails": {
    "cardNumber": "4532345562363802", // Pre seeded account card number
    "cardExpiryDate": "12-29", // Pre Seeded account expiry date
    "cardSecurityCode": "123" // Pre Seeded account security code
  }
}
```
> This was a really fun exercise and i might have went a bit overboard with implementation, since i was enjoying myself (actually forgot im supposed to hand it over...).
