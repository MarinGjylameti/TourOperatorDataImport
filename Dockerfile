FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln .
COPY TourOperatorDataImport.API/*.csproj ./TourOperatorDataImport.API/
COPY TourOperatorDataImport.Application/*.csproj ./TourOperatorDataImport.Application/
COPY TourOperatorDataImport.Core/*.csproj ./TourOperatorDataImport.Core/
COPY TourOperatorDataImport.Infrastructure/*.csproj ./TourOperatorDataImport.Infrastructure/
RUN dotnet restore
COPY . .
WORKDIR "/src/TourOperatorDataImport.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Install SQL Server tools for health check
RUN apt-get update && apt-get install -y curl && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools

COPY --from=build /app/publish .
COPY wait-for-sql.sh /app/
RUN chmod +x /app/wait-for-sql.sh

ENTRYPOINT ["/bin/bash", "-c", "/app/wait-for-sql.sh sqlserver dotnet TourOperatorDataImport.API.dll"]