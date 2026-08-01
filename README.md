Order Management System

A full-stack Order Management System built using ASP.NET Core Web API, Clean Architecture, CQRS, MediatR, Dapper, SQL Server, Redis, FluentValidation, AutoMapper, and React.js.

Technology Stack

Layer

Technology

Backend

ASP.NET Core Web API (.NET 9)

Architecture

Clean Architecture

Design Pattern

CQRS

Mediator

MediatR

Data Access

Dapper

Database

SQL Server

Validation

FluentValidation

Object Mapping

AutoMapper

Caching

Redis

API Documentation

Swagger / OpenAPI

Frontend

React.js

Package Manager

npm

Prerequisites

Install the following software:

.NET 9 SDK

Node.js 20 LTS or later

npm 10 or later

SQL Server 2022, SQL Server Express, or LocalDB

SQL Server Management Studio (SSMS)

Redis (optional if caching is disabled)

Visual Studio 2022 or later

Verify the installations:

dotnet --version
node --version
npm --version

Clone the Repository

git clone https://github.com/SiddhanathNikam29/OrderManagement.git

cd OrderManagement

Database Setup

Option A: Run the SQL Script Using SSMS

Open SQL Server Management Studio (SSMS).

Connect to your local SQL Server instance.

Open the following file:

Sql Script in repository

Execute the complete SQL script.

Verify that the database, tables, stored procedures, and required data were created successfully.

Use the database name defined in Database/Schema.sql when configuring the connection string.

Option B: Run the SQL Script Using SQLCMD

For Local SQL Server:

sqlcmd -S localhost -E -i Database/Schema.sql

For SQL Server Express:

sqlcmd -S .\SQLEXPRESS -E -i Database/Schema.sql

Configure the Backend

Open:

OrderManagement.API/appsettings.json

Update the SQL Server connection string according to your local SQL Server instance.

Example using SQL Server LocalDB:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OrderManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

Example using SQL Server Express:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=OrderManagement;Integrated Security=True;TrustServerCertificate=True"
  }
}

Example using SQL Server with Windows Authentication:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrderManagementDB;Integrated Security=True;TrustServerCertificate=True"
  }
}

Ensure that the connection-string key matches the key used by DapperContext.

Redis Configuration

Configure Redis in:

OrderManagement.API/appsettings.json

Example:

{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}

If Redis is installed locally, start the Redis service before running the API.

You can also run Redis using Docker:

docker run --name order-management-redis -p 6379:6379 -d redis:latest

Verify the Redis container:

docker ps

Backend Setup

From the repository root:

dotnet restore

dotnet build

Run the API:

cd OrderManagement.API

dotnet run

The API URLs are configured in:

OrderManagement.API/Properties/launchSettings.json

Frontend Setup

Open a new terminal and navigate to the React application:

cd OrderManagement/order-management-ui

Install dependencies:

npm install

Start the frontend:

npm start

The React application will normally start at:

http://localhost:3000

Frontend API Configuration

Create or update:

order-management-ui/.env

Add:

REACT_APP_API_URL=https://localhost:44308

Restart the React development server after changing the .env file:

npm start

Default URLs

Service

URL

React Frontend

http://localhost:3000

Backend API

https://localhost:44308

Swagger UI

https://localhost:44308/swagger

Health Check

https://localhost:44308/health

Simple Health Check

https://localhost:44308/health/simple

The backend port may differ on your machine. Check OrderManagement.API/Properties/launchSettings.json for the actual URL.

Run the Complete Application

Use two terminals.

Terminal 1 — Backend

cd OrderManagement

dotnet restore

dotnet build

cd OrderManagement.API

dotnet run

Terminal 2 — Frontend

cd OrderManagement/order-management-ui

npm install

npm start

Open the application:

http://localhost:3000

Verify the Application

Verify Swagger

Open:

https://localhost:44308/swagger

Swagger should display the available API endpoints.

Verify Health Check

Open:

https://localhost:44308/health/simple

Expected response:

{
  "status": "Healthy",
  "timestamp": "2026-08-01T10:00:00Z",
  "environment": "Development"
}

Verify the Frontend

Open:

http://localhost:3000

The Order Management application should load and connect to the backend API.

CORS Configuration

The backend is configured to allow the React application running at:

http://localhost:3000

The CORS policy is configured in:

OrderManagement.API/Program.cs

Example:

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

Project Structure

OrderManagement/
│
├── OrderManagement.API/
│   ├── Controllers/
│   ├── Mappings/
│   ├── Properties/
│   ├── appsettings.json
│   └── Program.cs
│
├── OrderManagement.Application/
│   ├── Behaviors/
│   ├── Commands/
│   ├── Common/
│   ├── DTOs/
│   ├── Queries/
│   ├── Services/
│   └── Validators/
│
├── OrderManagement.Domain/
│   ├── Entities/
│   └── Interfaces/
│
├── OrderManagement.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   │   ├── Read/
│   │   └── Write/
│   └── Services/
│
├── Database/
│   └── Schema.sql
│
├── order-management-ui/
│   ├── public/
│   ├── src/
│   ├── package.json
│   └── .env
│
├── OrderManagement.sln
└── README.md

Common Issues

SQL Server Connection Error

Check the following:

SQL Server service is running.

The database has been created successfully.

The database name is correct.

The SQL Server instance name is correct.

The connection string is correct.

Windows Authentication or SQL Authentication is configured correctly.

Backend Port Is Different

Check:

OrderManagement.API/Properties/launchSettings.json

Use the URL shown in the applicationUrl property.

Update the frontend .env file if the API port is different:

REACT_APP_API_URL=https://localhost:<API_PORT>

HTTPS Certificate Error

Run:

dotnet dev-certs https --clean

dotnet dev-certs https --trust

Then restart the API:

dotnet run

Port Already in Use

Check which process is using port 44308:

netstat -ano | findstr :44308

Stop the process:

taskkill /PID <PROCESS_ID> /F

Then start the API again:

dotnet run

React npm install Error

Delete node_modules and reinstall.

Windows PowerShell:

Remove-Item node_modules -Recurse -Force

Remove-Item package-lock.json -Force

npm install

npm start

CORS Error

Verify that:

The backend is running.

The frontend URL is included in the CORS policy.

The API URL in .env is correct.

Both backend and frontend were restarted after configuration changes.

Development Notes

Run Database/Schema.sql before starting the application.

Start SQL Server before starting the backend.

Start the backend before starting the React frontend.

Keep the backend running while testing the frontend.

Use Swagger to test API endpoints independently.

Use /health or /health/simple to verify API availability.

Do not commit connection strings containing production credentials.

Use appsettings.Development.json or environment variables for local secrets.