#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OrderBook.Worker/OrderBook.Worker.csproj", "OrderBook.Worker/"]
COPY ["Common.Logging/Common.Logging.csproj", "Common.Logging/"]
COPY ["OrderBook.Application/OrderBook.Application.csproj", "OrderBook.Application/"]
COPY ["OrderBook.Core/OrderBook.Core.csproj", "OrderBook.Core/"]
COPY ["OrderBook.Infrastructure/OrderBook.Infrastructure.csproj", "OrderBook.Infrastructure/"]
RUN dotnet restore "OrderBook.Worker/OrderBook.Worker.csproj"
COPY . .
WORKDIR "/src/OrderBook.Worker"
RUN dotnet build "OrderBook.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderBook.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderBook.Worker.dll"]