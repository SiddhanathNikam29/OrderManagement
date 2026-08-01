\# 🛒 Order Management System



\## AI Software Engineer Tech Test Submission



A complete order management system built with \*\*.NET Core 8\*\* (CQRS + Dapper) and \*\*React TypeScript\*\* for the technical test. This solution demonstrates effective AI tool usage, clean architecture, and production-ready code.



\---



\## 📋 Table of Contents



\- \[Overview](#overview)

\- \[Features](#features)

\- \[Technology Stack](#technology-stack)

\- \[Architecture](#architecture)

\- \[Quick Start](#quick-start)

\- \[Setup Instructions](#setup-instructions)

\- \[API Endpoints](#api-endpoints)

\- \[Testing](#testing)

\- \[AI Usage](#ai-usage)

\- \[Project Structure](#project-structure)

\- \[Deployment](#deployment)

\- \[Contributing](#contributing)

\- \[License](#license)



\---



\## 📌 Overview



This order management system fulfills all requirements from the technical test:



\*\*Part One:\*\* Core order management with products, taxes, discounts, and order totals  

\*\*Part Two:\*\* Fair 3-way order splitting with rounding remainder allocation



\### Time Investment

\- \*\*Total Time:\*\* \~3 hours

\- \*\*AI Tools Used:\*\* ChatGPT 4, GitHub Copilot, Cursor

\- \*\*Human Oversight:\*\* Business logic, edge cases, architecture decisions



\---



\## ✨ Features



\### ✅ Part One - Core Requirements



| Feature | Description |

|---------|-------------|

| \*\*Product Catalogue\*\* | Browse products with taxable/zero-rated items |

| \*\*Order Management\*\* | Create and manage orders with multiple line items |

| \*\*Tax Calculation\*\* | 20% standard rate with proportional discount handling |

| \*\*Discount Application\*\* | Percentage or fixed amount discounts |

| \*\*Order Summary\*\* | Complete breakdown: subtotal, discount, tax, total |



\### ✅ Part Two - Split Extension



| Feature | Description |

|---------|-------------|

| \*\*3-Way Split\*\* | Split order total into equal shares |

| \*\*Fair Rounding\*\* | Remainder distributed fairly (first shares get extra cent) |

| \*\*Visual Display\*\* | Clear UI showing each share amount |



\---



\## 🛠 Technology Stack



\### Backend



| Technology | Version | Purpose |

|------------|---------|---------|

| .NET Core | 8 | Framework |

| C# | 12 | Language |

| MediatR | 12.2.0 | CQRS implementation |

| Dapper | 2.1.24 | Micro-ORM |

| SQL Server | 2022 | Database |

| Redis | Latest | Caching |

| AutoMapper | 12.0.1 | Object mapping |

| FluentValidation | 11.8.0 | Validation |

| Swagger/OpenAPI | 6.5.0 | API documentation |



\### Frontend



| Technology | Version | Purpose |

|------------|---------|---------|

| React | 18.2.0 | UI Framework |

| TypeScript | 4.9.5 | Type safety |

| React Bootstrap | 2.9.1 | UI Components |

| Axios | 1.6.2 | HTTP Client |

| React Router | 6.20.0 | Navigation |



\---



\## 🏗 Architecture

┌─────────────────────────────────────────────────────────────┐

│ React Frontend │

│ (TypeScript + Bootstrap) │

└─────────────────┬───────────────────────────────────────────┘

│ HTTP/REST

┌─────────────────▼───────────────────────────────────────────┐

│ API Controllers │

│ (REST Endpoints) │

└─────────────────┬───────────────────────────────────────────┘

│

┌─────────────────▼───────────────────────────────────────────┐

│ MediatR Pipeline │

│ (Logging, Validation, Performance) │

└────────┬──────────────────────────────────────┬────────────┘

│ │

┌────────▼─────────┐ ┌───────────▼──────────┐

│ Write Side │ │ Read Side │

│ (Commands) │ │ (Queries) │

│ │ │ │

│ • CreateOrder │ │ • GetOrderById │

│ • AddItem │ │ • GetOrderSummary │

│ • ApplyDiscount │ │ • GetProductCatalogue │

│ • RemoveItem │ │ • GetAllOrders │

└────────┬─────────┘ └───────────┬──────────┘

│ │

┌────────▼─────────┐ ┌───────────▼──────────┐

│ Write DB │ │ Read DB + Cache │

│ (SQL Server) │ │ (Redis) │

└──────────────────┘ └───────────────────────┘






\### CQRS Pattern Benefits



\- \*\*Separation of Concerns\*\*: Commands (write) vs Queries (read)

\- \*\*Performance\*\*: Optimized read models with caching

\- \*\*Scalability\*\*: Independent scaling of read/write

\- \*\*Maintainability\*\*: Clear separation of business logic



\---



\## 🚀 Quick Start



\### Prerequisites



```bash

\# Check versions

dotnet --version    # Should show 8.x

node --version      # Should show 18.x

npm --version       # Should show 9.x+

sqlcmd -?           # SQL Server installed

redis-server -v     # Redis installed

Clone and Setup



\# Clone repository

git clone <repository-url>

cd OrderManagement



\# Backend setup

dotnet restore

dotnet build

cd OrderManagement.API

dotnet run



\# Frontend setup (new terminal)

cd order-management-ui

npm install

npm start





Default URLs

Service	URL

Backend API	https://localhost:44308

Swagger UI	https://localhost:44308/swagger

Frontend	http://localhost:3000



📦 Setup Instructions

1\. Database Setup

Option A: Run SQL Script

Execute the complete SQL script from Database/Schema.sql:

-- Create database

CREATE DATABASE OrderManagement\_CQRS;

GO



USE OrderManagement\_CQRS;

GO



\-- Run all scripts in order:

\-- 1. Tables

\-- 2. Indexes

\-- 3. Views

\-- 4. Stored Procedures

\-- 5. Seed Data


2\. Backend Configuration

appsettings.json

json

{

&#x20; "Logging": {

&#x20;   "LogLevel": {

&#x20;     "Default": "Information",

&#x20;     "Microsoft.AspNetCore": "Warning"

&#x20;   }

&#x20; },

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "Server=localhost;Database=OrderManagement\_CQRS;Trusted\_Connection=True;TrustServerCertificate=True;",

&#x20;   "RedisConnection": "localhost:6379"

&#x20; },

&#x20; "AllowedHosts": "\*"

}

Run Backend

bash

cd OrderManagement.API

dotnet run



\# Or with hot reload

dotnet watch run

3\. Frontend Configuration

.env

env

REACT\_APP\_API\_URL=https://localhost:5001/api

Run Frontend

bash

cd order-management-ui

npm install

npm start

📡 API Endpoints

Orders

Method	Endpoint	Description

POST	/api/orders	Create new order

GET	/api/orders/{id}	Get order details

GET	/api/orders/{id}/summary	Get order summary

GET	/api/orders	Get all orders (paged)

POST	/api/orders/{id}/items	Add item to order

DELETE	/api/orders/{id}/items/{itemId}	Remove item

PATCH	/api/orders/{id}/discount	Apply discount

POST	/api/orders/{id}/split	Split order

Products

Method	Endpoint	Description

GET	/api/products	Get product catalogue

Health

Method	Endpoint	Description

GET	/health	Detailed health check

GET	/health/simple	Simple health status

Example: Create Order

bash

curl -X POST https://localhost:5001/api/orders \\

&#x20; -H "Content-Type: application/json" \\

&#x20; -d '{"customerName":"John Doe","customerEmail":"john@example.com"}'

Example: Add Item

bash

curl -X POST https://localhost:5001/api/orders/1/items \\

&#x20; -H "Content-Type: application/json" \\

&#x20; -d '{"orderId":1,"productId":1,"quantity":2}'

Example: Split Order

bash

curl -X POST https://localhost:5001/api/orders/1/split \\

&#x20; -H "Content-Type: application/json" \\

&#x20; -d '{"orderId":1,"numberOfShares":3}'

🧪 Testing

Test Scenarios

1\. Basic Order Flow

Step	Action	Expected

1	Create order	Order with ID and number

2	Add 2 Laptops ($1299.99 each)	Subtotal: $2599.98

3	Add 3 Sandwiches ($6.99 each)	Subtotal: $2620.95

4	Apply 10% discount	Discount: $262.10

5	Check totals	Tax: $471.77, Total: $2830.62

2\. Split Test

Input	Expected Output

Total: $10.00, Shares: 3	$3.34, $3.33, $3.33

Total: $0.02, Shares: 3	$0.01, $0.01, $0.00

Total: $100.00, Shares: 4	$25.00, $25.00, $25.00, $25.00

Backend Tests

bash

cd OrderManagement.Tests

dotnet test

Frontend Tests

bash

cd order-management-ui

npm test

🤖 AI Usage

Tools Used

Tool	Purpose	Effectiveness

ChatGPT 4	Architecture, code generation, debugging	⭐⭐⭐⭐⭐

GitHub Copilot	Boilerplate, completion, tests	⭐⭐⭐⭐

Cursor	Refactoring, error fixing	⭐⭐⭐⭐⭐

AI Effectiveness Metrics

Metric	Value

Code Generated by AI	\~70%

Code Modified/Corrected	\~30%

AI Suggestions Accepted	85%

AI Suggestions Rejected	15%

Time Saved	\~60%

Bugs Found by AI	12

Bugs Missed by AI	3

Key Corrections Made

Issue	AI Suggestion	Human Fix

IsTaxable in OrderItems	Added column	Removed - comes from Products

Connection management	OpenAsync()	Used synchronous Open()

AutoMapper overload	typeof(Program).Assembly	Inline configuration

Swagger middleware	Wrong order	Fixed middleware order

Split edge case	Missed < 1 cent	Added validation

Example AI Interaction

text

\[User] "Implement fair rounding for splitting an amount 3 ways"



\[AI] Provided base algorithm



\[User] "What if base amount is $0.00 and total is $0.02 split 3 ways?"



\[AI] Added edge case handling → Final solution now handles all cases

📁 Project Structure

text

OrderManagement/

├── OrderManagement.Domain/

│   ├── Entities/

│   │   ├── Order.cs

│   │   ├── OrderItem.cs

│   │   └── Product.cs

│   ├── Common/

│   │   └── BaseEntity.cs

│   └── Interfaces/

│       ├── IWriteRepository.cs

│       └── IReadRepository.cs

│

├── OrderManagement.Application/

│   ├── Commands/

│   │   ├── Orders/

│   │   │   ├── CreateOrder/

│   │   │   ├── AddItem/

│   │   │   ├── ApplyDiscount/

│   │   │   └── RemoveItem/

│   │   └── Products/

│   ├── Queries/

│   │   ├── Orders/

│   │   │   ├── GetOrderById/

│   │   │   ├── GetOrderSummary/

│   │   │   ├── GetAllOrders/

│   │   │   └── SplitOrder/

│   │   └── Products/

│   │       └── GetProductCatalogue/

│   ├── DTOs/

│   │   ├── OrderDto.cs

│   │   ├── ProductDto.cs

│   │   └── OrderSplitResultDto.cs

│   ├── Behaviors/

│   │   ├── LoggingBehavior.cs

│   │   └── ValidationBehavior.cs

│   └── Services/

│       ├── IOrderCalculator.cs

│       └── OrderCalculator.cs

│

├── OrderManagement.Infrastructure/

│   ├── Data/

│   │   ├── DapperContext.cs

│   │   └── StoredProcedureNames.cs

│   ├── Repositories/

│   │   ├── Write/

│   │   │   ├── OrderWriteRepository.cs

│   │   │   └── ProductWriteRepository.cs

│   │   └── Read/

│   │       ├── OrderReadRepository.cs

│   │       └── ProductReadRepository.cs

│   └── Services/

│       ├── RedisCacheService.cs

│       └── ICacheService.cs

│

├── OrderManagement.API/

│   ├── Controllers/

│   │   ├── OrdersController.cs

│   │   └── ProductsController.cs

│   ├── Middleware/

│   │   └── ExceptionHandlingMiddleware.cs

│   ├── Program.cs

│   ├── appsettings.json

│   └── appsettings.Development.json

│

├── order-management-ui/

│   ├── src/

│   │   ├── api/

│   │   │   ├── client.ts

│   │   │   ├── orderApi.ts

│   │   │   └── productApi.ts

│   │   ├── components/

│   │   │   ├── common/

│   │   │   │   ├── LoadingSpinner.tsx

│   │   │   │   └── ErrorMessage.tsx

│   │   │   ├── catalogue/

│   │   │   │   ├── ProductCatalogue.tsx

│   │   │   │   └── ProductCard.tsx

│   │   │   ├── order/

│   │   │   │   ├── OrderBuilder.tsx

│   │   │   │   ├── OrderSummary.tsx

│   │   │   │   ├── OrderItemRow.tsx

│   │   │   │   └── DiscountControl.tsx

│   │   │   ├── split/

│   │   │   │   ├── SplitOrder.tsx

│   │   │   │   └── SplitResult.tsx

│   │   │   └── modals/

│   │   │       └── CreateOrderModal.tsx

│   │   ├── hooks/

│   │   │   ├── useOrder.ts

│   │   │   └── useProducts.ts

│   │   ├── types/

│   │   │   └── index.ts

│   │   ├── utils/

│   │   │   └── formatters.ts

│   │   ├── App.tsx

│   │   ├── App.css

│   │   └── index.tsx

│   ├── package.json

│   ├── tsconfig.json

│   └── .env

│

├── Database/

│   └── Schema.sql

│

├── Documentation/

│   ├── AI\_USAGE\_LOG.md

│   ├── SESSION\_LOGS.md

│   └── API\_REFERENCE.md

│

├── README.md

├── CONTRIBUTING.md

└── LICENSE



