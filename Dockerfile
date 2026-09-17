FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CineCatalogo.sln ./
COPY src/CineCatalogo.Domain/CineCatalogo.Domain.csproj src/CineCatalogo.Domain/
COPY src/CineCatalogo.Application/CineCatalogo.Application.csproj src/CineCatalogo.Application/
COPY src/CineCatalogo.Infrastructure/CineCatalogo.Infrastructure.csproj src/CineCatalogo.Infrastructure/
COPY src/CineCatalogo.Api/CineCatalogo.Api.csproj src/CineCatalogo.Api/
COPY tests/CineCatalogo.Tests.Unit/CineCatalogo.Tests.Unit.csproj tests/CineCatalogo.Tests.Unit/
COPY tests/CineCatalogo.Tests.Integration/CineCatalogo.Tests.Integration.csproj tests/CineCatalogo.Tests.Integration/

RUN dotnet restore src/CineCatalogo.Api/CineCatalogo.Api.csproj

COPY . .

RUN dotnet publish src/CineCatalogo.Api/CineCatalogo.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CineCatalogo.Api.dll"]
