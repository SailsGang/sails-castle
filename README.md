# 🔋 SailsEnergy

> EV charging cost-sharing platform for gangs (groups)

[![.NET](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

SailsEnergy helps EV owners share charging costs fairly within groups ("gangs"). Track energy consumption per vehicle, set tariffs, and generate period-based settlement reports.

## ✨ Features

-   🚗 **Car Management** - Register and manage multiple EVs
-   👥 **Gang System** - Create groups with members and roles (Owner, Admin, Member)
-   ⚡ **Energy Logging** - Track kWh charged per car/session
-   💰 **Tariff Management** - Set and update electricity prices
-   📊 **Period Reports** - Generate settlement reports with per-member breakdowns
-   🔔 **Real-time Notifications** - SignalR-based updates for gang members
-   🔐 **Authentication** - JWT-based auth with refresh tokens

## 🛠 Tech Stack

| Layer              | Technology                    |
| ------------------ | ----------------------------- |
| **API**            | .NET 10, Minimal APIs         |
| **CQRS/Messaging** | Wolverine                     |
| **Database**       | PostgreSQL (EF Core + Marten) |
| **Identity**       | ASP.NET Core Identity         |
| **Message Broker** | RabbitMQ                      |
| **Caching**        | Redis                         |
| **Real-time**      | SignalR                       |
| **Observability**  | OpenTelemetry                 |
| **Deployment**     | Kubernetes, Docker            |

## 🚀 Quick Start

### Prerequisites

-   .NET 10 SDK
-   Docker & Docker Compose
-   PostgreSQL, Redis, RabbitMQ (or use Docker)

### Local Development

```bash
# Clone the repository
git clone https://github.com/SailsGang/sails-castle.git
cd sails-castle

# Start all services (API + dependencies)
docker compose up -d

# API available at http://localhost:5209
# API docs at http://localhost:5209/scalar/v1
```

### Run Tests

```bash
# Unit tests
dotnet test tests/SailsEnergy.Application.Tests

# Integration tests (requires Docker)
dotnet test tests/SailsEnergy.Api.IntegrationTests
```

## 📁 Project Structure

```
sails-castle/
├── src/
│   ├── SailsEnergy.Api/           # Minimal API endpoints
│   ├── SailsEnergy.Application/   # CQRS commands, queries, handlers
│   ├── SailsEnergy.Domain/        # Entities, value objects, events
│   └── SailsEnergy.Infrastructure/# EF Core, Marten, external services
├── tests/
│   ├── SailsEnergy.Domain.Tests/
│   ├── SailsEnergy.Application.Tests/
│   └── SailsEnergy.Api.IntegrationTests/
├── k8s/                           # Kubernetes manifests
└── docker-compose.yml
```

## 🔌 API Endpoints

| Method | Endpoint               | Description                      |
| ------ | ---------------------- | -------------------------------- |
| POST   | `/api/auth/register`   | Register new user                |
| POST   | `/api/auth/login`      | Login and get tokens             |
| GET    | `/api/gangs`           | List user's gangs                |
| POST   | `/api/gangs`           | Create new gang                  |
| POST   | `/api/gangs/{id}/cars` | Add car to gang                  |
| POST   | `/api/energy`          | Log energy consumption           |
| POST   | `/api/periods/close`   | Close period and generate report |

See full API documentation at `/scalar/v1` when running locally.

## 🧪 Testing

| Type              | Count | Command                                              |
| ----------------- | ----- | ---------------------------------------------------- |
| Unit Tests        | 166   | `dotnet test tests/SailsEnergy.Application.Tests`    |
| Integration Tests | 22    | `dotnet test tests/SailsEnergy.Api.IntegrationTests` |

## 🐳 Docker

```bash
# Build image
docker build -t sails-energy:latest -f src/SailsEnergy.Api/Dockerfile .

# Run with compose
docker compose up -d
```

## ☸️ Kubernetes

Kubernetes manifests are in the `k8s/` directory:

```bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/
```

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file.
