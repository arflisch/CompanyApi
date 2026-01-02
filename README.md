# ?? CompanyApi

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-9.5-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![Dapr](https://img.shields.io/badge/Dapr-1.16-0d2192?logo=dapr)](https://dapr.io/)
[![Avalonia](https://img.shields.io/badge/Avalonia-11.3-8B44AC)](https://avaloniaui.net/)

A modern, cloud-native company management system built with cutting-edge Microsoft technologies and distributed application patterns.

## ?? Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Key Technologies](#key-technologies)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Features](#features)
- [Configuration](#configuration)
- [Development](#development)
- [Observability](#observability)
- [Security](#security)
- [Contributing](#contributing)

## ?? Overview

CompanyApi is a full-stack distributed application demonstrating modern microservices architecture patterns with:

- **Backend API**: RESTful API built with ASP.NET Core 10.0
- **Desktop Frontend**: Cross-platform desktop application using Avalonia UI
- **Orchestration**: .NET Aspire for local development and orchestration
- **Distributed Runtime**: Dapr for microservices capabilities
- **Observability**: OpenTelemetry with structured logging via Serilog
- **Authentication**: Microsoft Identity Platform (Azure AD/Entra ID)

## ??? Architecture

```
???????????????????????????????????????????????????????????????
?                      .NET Aspire AppHost                     ?
?                  (Orchestration & Service Discovery)         ?
????????????????????????????????????????????????????????????????
                ?                                  ?
    ????????????????????????           ?????????????????????????
    ?   CompanyApi (BE)    ?           ?  Avalonia Frontend    ?
    ?   + Dapr Sidecar     ?           ?      (Desktop)        ?
    ?   Port: 7223         ?           ?                       ?
    ????????????????????????           ?????????????????????????
            │                              │
            │                    ┌─────────────────────────┐
            │                    │  NotificationService     │
            │                    │    + Dapr Sidecar        │
            │                    │    Port: 7026            │
            │                    └─────────────────────────┘
            │                              │
            ?  ???????????????????????????????????????????
            ???? PostgreSQL Database (Port: 5433)       ?
            ?  ???????????????????????????????????????????
            ?
            ?  ???????????????????????????????????????????
            ???? Redis (State Store + Cache)            ?
            ?  ?  + RedisCommander UI                    ?
            ?  ???????????????????????????????????????????
            ?
            ?  ???????????????????????????????????????????
            ???? RabbitMQ (Pub/Sub via Dapr)            ?
               ?  Events: companycreated                ?
               │  Subscribers: NotificationService      │
               ???????????????????????????????????????????
```

### Clean Architecture Layers

The backend follows Clean Architecture principles:

```
????????????????????????????????????????????
?        CompanyApi (Presentation)         ?
?   Controllers, Middleware, Auth          ?
????????????????????????????????????????????
               ?
????????????????????????????????????????????
?         Application (Use Cases)          ?
?   Commands, Services, DTOs, Validation   ?
????????????????????????????????????????????
               ?
????????????????????????????????????????????
?            Domain (Entities)             ?
?   Business Logic, Interfaces             ?
????????????????????????????????????????????
               ?
????????????????????????????????????????????
?      Database (Infrastructure)           ?
?   EF Core, Repositories, PostgreSQL      ?
????????????????????????????????????????????
```

## ?? Key Technologies

### Backend Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 10.0 | Core framework |
| **ASP.NET Core** | 10.0 | Web API framework |
| **.NET Aspire** | 9.5 | Cloud-native orchestration |
| **Dapr** | 1.16.1 | Distributed application runtime |
| **Entity Framework Core** | Latest | ORM for PostgreSQL |
| **PostgreSQL** | - | Primary database |
| **Redis** | - | State store & caching |
| **RabbitMQ** | - | Message broker (Pub/Sub) |

### Authentication & Authorization

| Technology | Purpose |
|------------|---------|
| **Microsoft Identity Web** | 4.2.0 - Azure AD integration |
| **JWT Bearer** | Token-based authentication |
| **Microsoft Entra ID** | Identity provider |

### Observability & Monitoring

| Technology | Purpose |
|------------|---------|
| **OpenTelemetry** | Distributed tracing & metrics |
| **Serilog** | Structured logging |
| **Seq** | Log aggregation |
| **Prometheus** | Metrics collection (via OpenTelemetry) |

### Validation & Documentation

| Technology | Purpose |
|------------|---------|
| **FluentValidation** | 12.1.1 - Request validation |
| **NSwag** | 14.6.3 - OpenAPI/Swagger generation |

### Frontend Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **Avalonia UI** | 11.3.10 | Cross-platform desktop UI |
| **CommunityToolkit.Mvvm** | 8.4.0 | MVVM framework |
| **Microsoft Identity Client** | 4.79.2 | MSAL authentication |

### DevOps & Tooling

- **Dapr CLI**: Local development with sidecars
- **RedisCommander**: Redis visualization
- **Docker**: Container support (optional)

## ?? Prerequisites

Before you begin, ensure you have the following installed:

### Required

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for Aspire AppHost)
- [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/) (v1.16+)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Redis, PostgreSQL, RabbitMQ)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.12+) or [VS Code](https://code.visualstudio.com/)

### Recommended

- [.NET Aspire Workload](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)
  ```bash
  dotnet workload install aspire
  ```
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (for Azure AD configuration)

## ?? Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/arflisch/CompanyApi.git
cd CompanyApi
```

### 2. Initialize Dapr

```bash
dapr init
```

### 3. Start Infrastructure Services

Using Docker Compose (create a `docker-compose.yml` or run individually):

```bash
# PostgreSQL
docker run -d --name postgres -p 5433:5432 \
  -e POSTGRES_USER=cae_user \
  -e POSTGRES_PASSWORD=cae \
  -e POSTGRES_DB=company_db \
  postgres:latest

# Redis
docker run -d --name redis -p 6379:6379 redis:latest

# RabbitMQ
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=toto \
  -e RABBITMQ_DEFAULT_PASS=toto1 \
  -e RABBITMQ_DEFAULT_VHOST=toto \
  rabbitmq:3-management
```

### 4. Configure Azure AD (Optional)

Update `BE/Company/CompanyApi/appsettings.json` with your Azure AD tenant:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-domain.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc",
    "Scopes": "access_as_user"
  }
}
```

### 5. Run the Application

#### Option A: Using .NET Aspire (Recommended)

```bash
cd CompanyApi.AppHost
dotnet run
```

The Aspire Dashboard will open automatically at `http://localhost:15000` or similar.

#### Option B: Manual Start

```bash
# Terminal 1: Start API with Dapr
cd BE/Company/CompanyApi
dapr run --app-id company-api --app-port 7223 --dapr-http-port 3500 \
  --resources-path ../../../BE/Dapr/Development -- dotnet run

# Terminal 2: Start Frontend
cd FE/CompanyFrontend
dotnet run
```

### 6. Access the Application

- **API**: https://localhost:7223
- **Swagger UI**: https://localhost:7223/swagger/facade
- **NotificationService API**: https://localhost:7026
- **Aspire Dashboard**: http://localhost:15000 (when using Aspire)
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Redis Commander**: http://localhost:8081 (when using Aspire)

## ?? Project Structure

```
CompanyApi/
??? BE/                                 # Backend solutions
?   ??? Company/
?   ?   ??? CompanyApi/                # API presentation layer
?   ?   ?   ??? Controllers/           # REST endpoints
?   ?   ?   ??? Program.cs             # Startup & DI
?   ?   ?   ??? appsettings.json       # Configuration
?   ?   ??? Application/               # Use cases & business logic
?   ?   ?   ??? Commands/              # CQRS commands
?   ?   ?   ??? Services/              # Application services
?   ?   ?   ??? Metrics/               # Custom metrics
?   ?   ?   ??? DTOs/                  # Data transfer objects
?   ?   ??? Domain/                    # Domain entities
?   ?   ??? Database/                  # EF Core & repositories
?   ?   ??? CompanySdk/                # Generated client SDK
?   ?   ??? Application.Test/          # Unit tests
?   ?   Notification/                # Notification microservice
?   ?   ?   NotificationService/      # Email notification service
?   ?   ?   ?   Controllers/          # Notification endpoints
?   ?   ?   ?   Models/               # Notification DTOs
?   ?   ?   ?   Program.cs            # Startup & Dapr integration
?   ??? CompanyApi.ServiceDefaults/    # Aspire shared services
?   ??? Dapr/
?       ??? Development/               # Dapr components
?           ??? statestore.yaml        # Redis state store
?           ??? RabbitMQ.yml           # Pub/Sub config
?           ??? subscription.yml       # Event subscriptions
??? FE/                                # Frontend
?   ??? CompanyFrontend/              # Avalonia desktop app
?       ??? Views/                     # XAML views
?       ??? ViewModels/                # MVVM view models
?       ??? Services/                  # Auth & API services
??? CompanyApi.AppHost/               # Aspire orchestration
    ??? Program.cs                     # Service composition
```

## ? Features

### Backend Capabilities

- ? **CRUD Operations**: Full company management (Create, Read, Update, Delete, Patch)
- ? **Distributed Caching**: Redis-based caching via Dapr state store
- ? **Event-Driven Architecture**: Pub/Sub with RabbitMQ (e.g., `companycreated` events)
- ? **Input Validation**: FluentValidation for DTOs
- ? **API Documentation**: Auto-generated OpenAPI specs with NSwag
- ? **Repository Pattern**: Clean separation of data access
- ? **Metrics**: Custom application metrics (e.g., company operations)
- ? **Query Optimization**: No-tracking queries for read operations
- 📧 **Notification Service**: Event-driven email notifications via RabbitMQ pub/sub

### Frontend Capabilities

- ? **Cross-Platform Desktop**: Windows, macOS, Linux support
- ? **Modern UI**: Fluent design with Avalonia
- ? **MVVM Architecture**: CommunityToolkit.Mvvm
- ? **Authentication**: MSAL integration with Azure AD
- ? **Responsive Design**: DataGrid for company listings

### Dapr Integration

- **State Management**: Redis state store for caching
- **Pub/Sub Messaging**: RabbitMQ for event-driven communication
- **Service Invocation**: Direct service-to-service calls
- **Observability**: Integrated tracing and metrics

## ?? Configuration

### Database Connection

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=company_db;Username=cae_user;Password=cae"
  }
}
```

### Dapr Components

**State Store** (`BE/Dapr/Development/statestore.yaml`):
```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore
spec:
  type: state.redis
  version: v1
  metadata:
  - name: redisHost
    value: localhost:6379
```

**Pub/Sub** (`BE/Dapr/Development/RabbitMQ.yml`):
```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: rabbitmq-pubsub
spec:
  type: pubsub.rabbitmq
  version: v1
  metadata:
  - name: connectionString
    value: "amqp://toto:toto1@localhost:5672/toto"
```

### Logging Configuration

Structured logging with Serilog to:
- Console (development)
- File (production)
- Seq (centralized logging)
- OpenTelemetry (distributed tracing)

## ??? Development

### Running Tests

```bash
cd BE/Company/Application.Test
dotnet test
```

### Database Migrations

```bash
cd BE/Company/Database
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Generating API Client SDK

The SDK is auto-generated on build (Debug mode):

```bash
cd BE/Company/CompanyApi
dotnet build
```

Generated client: `BE/Company/CompanySdk/`

### Debugging with Dapr

Visual Studio launch profiles are configured for Dapr integration. Use the "CompanyApi" profile to start with Dapr sidecar automatically.

## ?? Observability

### OpenTelemetry Integration

The application exports telemetry to:

- **Traces**: Distributed request tracing
- **Metrics**: Runtime and custom application metrics
- **Logs**: Structured logs correlated with traces

### Aspire Dashboard

Access comprehensive observability at `http://localhost:15000`:

- Live traces
- Metrics visualization
- Log streaming
- Service dependencies
- Resource health

### Custom Metrics

`Application.Metrics.CompanyMetrics` provides:
- Company creation counter
- Company update counter
- Operation duration histograms

## ?? Security

- **Authentication**: Microsoft Identity Platform (Azure AD)
- **Authorization**: JWT Bearer tokens
- **HTTPS**: Enforced in production
- **CORS**: Configurable for frontend origins
- **Validation**: FluentValidation for all inputs
- **Secrets Management**: User secrets in development, Azure Key Vault recommended for production

### Securing API Keys

```bash
# Set user secrets
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

## ?? Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Standards

- Follow C# coding conventions
- Maintain test coverage above 80%
- Update documentation for new features
- Use conventional commits

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Authors

- **arflisch** - *Initial work* - [GitHub](https://github.com/arflisch)

## ?? Acknowledgments

- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/) for cloud-native orchestration
- [Dapr](https://dapr.io/) for distributed application runtime
- [Avalonia UI](https://avaloniaui.net/) for cross-platform UI framework
- Microsoft Identity Platform for authentication

## ?? Additional Resources

- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/)
- [Dapr Documentation](https://docs.dapr.io/)
- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [Avalonia Documentation](https://docs.avaloniaui.net/)

---

**Built with ?? using cutting-edge .NET technologies**
