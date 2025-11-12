FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY App.sln ./
COPY src/App.Domain/App.Domain.csproj src/App.Domain/
COPY src/App.Data/App.Data.csproj src/App.Data/
COPY src/App.WebApi/App.WebApi.csproj src/App.WebApi/
COPY tests/App.Domain.Tests/App.Domain.Tests.csproj tests/App.Domain.Tests/
COPY tests/App.IntegrationTests/App.IntegrationTests.csproj tests/App.IntegrationTests/

RUN dotnet restore App.sln

COPY . .
RUN dotnet publish src/App.WebApi/App.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./
RUN mkdir -p Logs

ENTRYPOINT ["dotnet", "App.WebApi.dll"]
