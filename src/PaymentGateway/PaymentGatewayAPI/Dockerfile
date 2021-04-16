#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/PaymentGateway/PaymentGatewayAPI/PaymentGatewayAPI.csproj", "src/PaymentGateway/PaymentGatewayAPI/"]
COPY ["src/PaymentGateway/PaymentGatewayDB/PaymentGatewayDB.csproj", "src/PaymentGateway/PaymentGatewayDB/"]
COPY ["lib/Common/Common.csproj", "lib/Common/"]
RUN dotnet restore "src/PaymentGateway/PaymentGatewayAPI/PaymentGatewayAPI.csproj"
COPY . .
WORKDIR "/src/src/PaymentGateway/PaymentGatewayAPI"
RUN dotnet build "PaymentGatewayAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGatewayAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGatewayAPI.dll"]