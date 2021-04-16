using AutoMapper;
using Newtonsoft.Json;
using PaymentGatewayAPI.Services;
using PaymentGatewayDB.Entities;
using PaymentGatewayDB.Enums;

namespace PaymentGatewayAPI.Models.Mappings
{
    public class PaymentRequestEntityProfile : Profile
    {
        public PaymentRequestEntityProfile()
        {
            CreateMap<PaymentRequestEntity, PaymentResponse>()
                .ForMember(x => x.PaymentId, opt =>
                    opt.MapFrom(x => x.PaymentId))
                .ForMember(x => x.Status, opt =>
                    opt.MapFrom(x => x.PaymentStatus));

            CreateMap<PaymentRequestEntity, PaymentHistoric>()
                .ForMember(x => x.PaymentId, opt =>
                    opt.MapFrom(x => x.PaymentId))
                .ForMember(x => x.Status, opt =>
                    opt.MapFrom(x => x.PaymentStatus))
                .ForMember(x => x.MerchantId, opt =>
                    opt.MapFrom(x => x.MerchantId))
                .ForMember(x => x.Amount, opt =>
                    opt.MapFrom(x => x.Amount))
                .ForMember(x => x.Currency, opt =>
                    opt.MapFrom(x => x.Currency))
                .ForMember(x => x.CardDetails, opt =>
                    opt.MapFrom<MaskCardDetailsResolver>());

            CreateMap<PaymentRequest, PaymentRequestEntity>()
                .ForMember(x => x.MerchantId, opt =>
                    opt.MapFrom(x => x.MerchantId))
                .ForMember(x => x.Amount, opt =>
                    opt.MapFrom(x => x.Amount))
                .ForMember(x => x.Currency, opt =>
                    opt.MapFrom(x => x.Currency))
                .ForMember(x => x.PaymentStatus, opt =>
                    opt.MapFrom(x => PaymentStatus.Success))
                .ForMember(x => x.CardDetails, opt =>
                    opt.MapFrom<EncryptCardDetailsResolver>());
        }

        public class EncryptCardDetailsResolver : IValueResolver<PaymentRequest, PaymentRequestEntity, string>
        {
            private readonly Encryption _encryption;

            public EncryptCardDetailsResolver(Encryption encryption)
            {
                _encryption = encryption;
            }

            public string Resolve(PaymentRequest source, PaymentRequestEntity destination, string member, ResolutionContext context)
            {
                return _encryption.Encrypt(JsonConvert.SerializeObject(source.CardDetails));
            }
        }

        public class MaskCardDetailsResolver : IValueResolver<PaymentRequestEntity, PaymentHistoric, CardDetails>
        {
            private readonly Encryption _encryption;

            public MaskCardDetailsResolver(Encryption encryption)
            {
                _encryption = encryption;
            }

            public CardDetails Resolve(PaymentRequestEntity source, PaymentHistoric destination, CardDetails member, ResolutionContext context)
            {
                var cardDetails = JsonConvert.DeserializeObject<CardDetails>(
                    _encryption.Decrypt(source.CardDetails));
                return new CardDetails
                {
                    CardNumber = MaskValue(cardDetails.CardNumber, 12),
                    // Dev note: am not sure why we return these two, they could be omitted
                    CardExpiryDate = MaskExpiryDate(cardDetails.CardExpiryDate),
                    CardSecurityCode = MaskValue(cardDetails.CardSecurityCode, 3)
                };

                static string MaskExpiryDate(string str)
                {
                    if (string.IsNullOrEmpty(str) || str.Length != 5 || !str.Contains(Constants.DateSeparator))
                        return str;
                    var split = str.Split(Constants.DateSeparator);
                    return $"{MaskValue(split[0], 2)}{Constants.DateSeparator}{MaskValue(split[1], 2)}";
                }

                static string MaskValue(string str, int maskCount) =>
                    string.IsNullOrEmpty(str) || str.Length < maskCount
                        ? str
                        : string.Concat(new string(Constants.MaskCharacter, maskCount), str.Substring(maskCount));
            }
        }
    }
}
