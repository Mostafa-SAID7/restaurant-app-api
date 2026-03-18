# Project Setup Guide

## Prerequisites

Before setting up the NooR Restaurant Management System, ensure you have the following installed:

### Required Software
- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **npm** (comes with Node.js)
- **SQL Server** (LocalDB or full instance) - [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Git** - [Download here](https://git-scm.com/)

### Recommended Tools
- **Visual Studio 2022** or **VS Code**
- **SQL Server Management Studio (SSMS)**
- **Postman** for API testing

## Clone the Repository

```bash
git clone https://github.com/Mostafa-SAID7/NooR.git
cd NooR
```

## Backend Setup (.NET 8 Web API)

### 1. Navigate to Backend Directory
```bash
cd backend/api
```

### 2. Restore NuGet Packages
```bash
dotnet restore
```

### 3. Update Database Connection String
Edit `appsettings.json` and update the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db44985.public.databaseasp.net; Database=db44985; User Id=db44985; Password=g_4C8S!z-9fN; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
  }
}
```

### 4. Apply Database Migrations
```bash
dotnet ef database update
```

### 5. Build the Project
```bash
dotnet build
```

### 6. Run the Backend
```bash
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5124`
- **HTTPS**: `https://localhost:7124`
- **Swagger UI**: `https://localhost:7124/swagger`

## Frontend Setup (Angular 19)

### 1. Navigate to Frontend Directory
```bash
cd frontend/restaurant-app
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Update API Base URL (if needed)
Edit the environment files in `src/environments/` to point to your backend URL:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7124/api'
};
```

### 4. Start Development Server
```bash
ng serve
```

The frontend will be available at:
- **Development**: `http://localhost:4200`

## Database Setup

### Using SQL Server LocalDB (Recommended for Development)
1. Install SQL Server Express LocalDB
2. The connection string in `appsettings.json` should work out of the box
3. Run migrations: `dotnet ef database update`

### Using Full SQL Server Instance
1. Install SQL Server
2. Create a new database
3. Update the connection string in `appsettings.json`
4. Run migrations: `dotnet ef database update`

## Environment Configuration

### Backend Environment Variables
Create `appsettings.Development.json` for local development:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Your local connection string here"
  }
}
```

### Frontend Environment Configuration
Update `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7124/api',
  imageUrl: 'https://localhost:7124'
};
```

## Verification Steps

### Backend Verification
1. Navigate to `https://localhost:7124/swagger`
2. You should see the Swagger UI with all API endpoints
3. Test the `/api/Restaurant` GET endpoint

### Frontend Verification
1. Navigate to `http://localhost:4200`
2. You should see the restaurant application homepage
3. Check browser console for any errors

## Troubleshooting

### Common Backend Issues

**Port Already in Use**
```bash
# Find process using port 5124
netstat -ano | findstr :5124
# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

**Database Connection Issues**
- Verify SQL Server is running
- Check connection string format
- Ensure database exists
- Run `dotnet ef database update` again

**Package Restore Issues**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
# Restore packages
dotnet restore
```

### Common Frontend Issues

**Node Modules Issues**
```bash
# Delete node_modules and package-lock.json
rm -rf node_modules package-lock.json
# Reinstall dependencies
npm install
```

**Angular CLI Issues**
```bash
# Install Angular CLI globally
npm install -g @angular/cli@19
```

**CORS Issues**
- Ensure CORS is properly configured in the backend
- Check that the frontend URL is allowed in CORS policy

## Next Steps

After successful setup:
1. Review the [Features Documentation](FEATURES.md)
2. Check the [API Documentation](https://localhost:7124/swagger)
3. Explore the [Architecture Guide](STRUCTURE.md)
4. Read the [Contributing Guidelines](CONTRIBUTING.md)