#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["3thParty/BankEmulator/BankEmulator.csproj", "3thParty/BankEmulator/"]
RUN dotnet restore "3thParty/BankEmulator/BankEmulator.csproj"
COPY . .
WORKDIR "/src/3thParty/BankEmulator"
RUN dotnet build "BankEmulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankEmulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankEmulator.dll"]