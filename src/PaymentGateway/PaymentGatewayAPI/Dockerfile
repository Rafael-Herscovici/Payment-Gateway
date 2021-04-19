#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/PaymentGateway/PaymentGatewayAPI/PaymentGatewayAPI.csproj", "src/PaymentGateway/PaymentGatewayAPI/"]
COPY ["lib/CommonAPI/CommonAPI.csproj", "lib/CommonAPI/"]
COPY ["lib/CurrencyExchange/CurrencyExchange.csproj", "lib/CurrencyExchange/"]
COPY ["lib/Common/Common.csproj", "lib/Common/"]
COPY ["src/PaymentGateway/PaymentGatewayDB/PaymentGatewayDB.csproj", "src/PaymentGateway/PaymentGatewayDB/"]
RUN dotnet restore "src/PaymentGateway/PaymentGatewayAPI/PaymentGatewayAPI.csproj"
COPY . .
WORKDIR "/src/src/PaymentGateway/PaymentGatewayAPI"
RUN dotnet build "PaymentGatewayAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGatewayAPI.csproj" -c Release -o /app/publish

FROM base AS final
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGatewayAPI.dll"]