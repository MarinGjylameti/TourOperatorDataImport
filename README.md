TourOperator Data Import API
A high-performance data import engine for tour operator pricing and seat allocation files, built with .NET 8 using Clean Architecture principles. Now with Docker support!

🎯 Project Overview
This API processes large CSV files containing tour operator pricing data with real-time progress reporting, Redis caching, and secure role-based authentication.

🏗️ Architecture
text
TourOperatorDataImport/
├── API/                 - Controllers, Hubs, Middleware
├── Application/         - Business Logic, DTOs, Interfaces
├── Infrastructure/     - Data Access, Repositories, External Services
└── Core/              - Domain Models, Exceptions, Common
Design Pattern: Clean Architecture with separation of concerns
Database: SQL Server with Entity Framework Core
Caching: Redis for performance optimization
Real-time: SignalR for progress reporting
Containerization: Docker with multi-stage builds

🚀 Quick Deployment
Option 1: Docker Compose (Recommended)
bash
# Clone repository
git clone https://github.com/MarinGjylameti/TourOperatorDataImport.git
cd TourOperatorDataImport

# Start all services with Docker Compose
docker-compose up -d

# Access the application at:
# API: http://localhost:8080
# Swagger: http://localhost:8080/swagger
Option 2: Manual Setup
bash
# Prerequisites: .NET 8.0 SDK, SQL Server, Redis

# Configure connection strings in appsettings.json
# Run database migrations
dotnet ef database update

# Start application
dotnet run
🐳 Docker Services
The docker-compose.yml sets up a complete environment:

api: .NET 8 Web API (port 8080)

sqlserver: SQL Server 2022 (port 1433)

redis: Redis 7 (port 6379)

📈 Architecture Benefits
Clean Architecture: Separation of concerns, testable, maintainable

Dockerized: Consistent development/production environments

Scalable: Microservices-ready architecture

Production Ready: Health checks, logging, error handling
