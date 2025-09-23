# TourOperator Data Import API

A high-performance data import engine for tour operator pricing data.

## 🚀 Quick Start

### Using Docker

git clone https://github.com/MarinGjylameti/TourOperatorDataImport.git
cd TourOperatorDataImport
docker-compose up -d

What's Included

JWT Authentication - Secure login system
CSV File Upload - Process pricing data files
Real-time Progress - See upload status live
Redis Caching - Fast data access
Role-based Access - Admin vs TourOperator permissions

Docker Services
When you run docker-compose up -d, it starts:

API Server (port 8080)
SQL Server Database
Redis Cache

Project Structure
TourOperatorDataImport/
├── API/                 - Web controllers and hubs
├── Application/         - Business logic
├── Infrastructure/     - Data access layer
└── Core/              - Shared models
