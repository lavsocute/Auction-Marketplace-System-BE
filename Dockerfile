# Sử dụng .NET 9 SDK để build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy từng csproj và restore (KHÔNG bao gồm IntegrationTests)
COPY AuctionSys.Domain/*.csproj ./AuctionSys.Domain/
COPY AuctionSys.Infrastructure/*.csproj ./AuctionSys.Infrastructure/
COPY AuctionSys.Application/*.csproj ./AuctionSys.Application/
COPY AuctionSys.Api/*.csproj ./AuctionSys.Api/

RUN dotnet restore AuctionSys.Api/AuctionSys.Api.csproj

# Copy toàn bộ mã nguồn (trừ những gì trong .dockerignore)
COPY . ./

# Build và publish project
WORKDIR /app/AuctionSys.Api
RUN dotnet publish -c Release -o out --no-restore

# Chạy app bằng .NET 9 ASP.NET Core Runtime (dung lượng nhẹ hơn)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/AuctionSys.Api/out ./

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuctionSys.Api.dll"]
