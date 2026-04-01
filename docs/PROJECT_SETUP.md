# 🚀 Project Setup & Installation

This guide will walk you through setting up **Restaurant API** for local development and testing.

---

## 📋 Prerequisites

Ensure you have the following installed on your machine:

- **.NET 8.0 SDK** (Required for building and running the API)
- **Docker Desktop** (Recommended for containerized execution)
- **SQL Server** (LocalDB or a full instance)
- **Git** (For cloning the repository)

---

## 📥 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/Mostafa-SAID7/restaurant-app-api.git
cd restaurant-app-api
```

### 2. Configure the Database
Edit `api/appsettings.json` to update your connection string if necessary:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RestaurantDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

---

## 🛠️ Local Development Setup

### 1. Restore & Build
Navigate to the `api` directory and prepare the project:
```bash
cd api
dotnet restore
dotnet build
```

### 2. Update the Database
Apply Entity Framework migrations to create your local schema:
```bash
dotnet ef database update
```

### 3. Launch the API
```bash
dotnet run
```
The API will be available at `http://localhost:5124`. The **Cinematic UI** will be hosted at the root `/`.

---

## 🐳 Docker Setup (Recommended)

For a consistent environment across all platforms, use the provided Docker configuration.

### 1. Build & Run with Compose
From the root directory:
```bash
docker-compose up --build
```

This will orchestrate:
- **API Container**: Running the .NET 8 application.
- **Database Container**: (If configured in compose) or connecting to your cloud/local DB.

---

## ✅ Verification

Once running, you can verify the setup by visiting:

1. **Cinematic Landing Page**: [http://localhost:5124/Home.html](http://localhost:5124/Home.html)
2. **Interactive Docs**: [http://localhost:5124/Docs.html](http://localhost:5124/Docs.html)
3. **Swagger UI**: [http://localhost:5124/index.html](http://localhost:5124/index.html)

---

[⬅️ Back to Main README](../README.md)