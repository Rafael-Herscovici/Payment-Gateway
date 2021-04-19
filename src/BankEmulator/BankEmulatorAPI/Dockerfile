#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/BankEmulator/BankEmulatorAPI/BankEmulatorAPI.csproj", "src/BankEmulator/BankEmulatorAPI/"]
COPY ["lib/CommonAPI/CommonAPI.csproj", "lib/CommonAPI/"]
COPY ["lib/CurrencyExchange/CurrencyExchange.csproj", "lib/CurrencyExchange/"]
COPY ["lib/Common/Common.csproj", "lib/Common/"]
COPY ["src/BankEmulator/BankEmulatorDB/BankEmulatorDB.csproj", "src/BankEmulator/BankEmulatorDB/"]
RUN dotnet restore "src/BankEmulator/BankEmulatorAPI/BankEmulatorAPI.csproj"
COPY . .
WORKDIR "/src/src/BankEmulator/BankEmulatorAPI"
RUN dotnet build "BankEmulatorAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankEmulatorAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankEmulatorAPI.dll"]