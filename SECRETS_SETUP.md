# Secrets Management Guide

## Overview
This project uses environment variables and User Secrets for local development to keep sensitive data out of source control.

## Local Development Setup (Windows)

### Option 1: Using User Secrets (.NET CLI)
User Secrets is the recommended approach for local development.

1. **Initialize User Secrets for the project:**
   ```powershell
   cd api
   dotnet user-secrets init
   ```

2. **Set connection string and other sensitive values:**
   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER; Database=YOUR_DB; User Id=YOUR_USER; Password=YOUR_PASSWORD; Encrypt=True; MultipleActiveResultSets=True; TrustServerCertificate=False;"
   ```

3. **Verify secrets are set:**
   ```powershell
   dotnet user-secrets list
   ```

**User Secrets Location:**
- Windows: `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`
- Linux: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`
- macOS: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`

### Option 2: Using Local appsettings File
Create a local appsettings file with your actual credentials (this is already in .gitignore):

1. **Create `api/appsettings.Development.local.json`:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER; Database=YOUR_DB; User Id=YOUR_USER; Password=YOUR_PASSWORD; Encrypt=True; MultipleActiveResultSets=True; TrustServerCertificate=False;"
     }
   }
   ```

The `*.local.json` file is automatically ignored and will override values from `appsettings.Development.json`.

## Production Deployment

### Using Environment Variables
For production, set environment variables on your hosting platform:

**Docker:**
```dockerfile
ENV ConnectionStrings__DefaultConnection="Server=prod-server; Database=prod-db; User Id=prod-user; Password=prod-pass; Encrypt=True; TrustServerCertificate=False;"
```

**Azure App Service:**
Set in Configuration > Application settings:
- Name: `ConnectionStrings__DefaultConnection`
- Value: Your production connection string

**AWS / Other Platforms:**
Set OS-level environment variables or use secrets manager:
- AWS Secrets Manager
- Azure Key Vault
- HashiCorp Vault

### Environment Variable Naming Convention
For nested configuration (e.g., `ConnectionStrings:DefaultConnection`), use double underscore:
- `ConnectionStrings__DefaultConnection`

## Configuration Hierarchy (Applied Order)
1. `appsettings.json` (base configuration with placeholders)
2. `appsettings.{Environment}.json` (environment-specific, no secrets)
3. `appsettings.{Environment}.local.json` (local overrides, in .gitignore)
4. User Secrets (User Secrets ID in project file)
5. Environment Variables (highest priority)

## Testing
After setting up secrets:
```powershell
cd api
dotnet run
```

The API should connect to your database without errors.

## CI/CD Integration

### GitHub Actions
In your workflow, set environment variables:
```yaml
env:
  ConnectionStrings__DefaultConnection: ${{ secrets.DB_CONNECTION_STRING }}
```

### Local CI/CD Testing
For local testing with Docker:
```bash
docker build -t restaurant-api .
docker run -e "ConnectionStrings__DefaultConnection=Server=db; Database=restaurant; User Id=sa; Password=YourPassword123!" restaurant-api
```

## Security Best Practices

✅ **DO:**
- Store all passwords and API keys in User Secrets or environment variables
- Use strong database passwords (minimum 12 characters, mixed case, numbers, symbols)
- Rotate credentials regularly in production
- Use `Encrypt=True` for all connection strings
- Use HTTPS-only in production
- Keep `.gitignore` updated with all secret file patterns

❌ **DON'T:**
- Commit `appsettings.Development.json` with real credentials (already in .gitignore)
- Share User Secrets across team members (each developer has their own)
- Use default or weak passwords in production
- Log connection strings or API keys
- Disable encryption in production connections

## Troubleshooting

**Connection String Not Loading:**
1. Check User Secrets: `dotnet user-secrets list`
2. Check environment variables: `$env:ConnectionStrings__DefaultConnection`
3. Check `appsettings.{Environment}.json` files
4. Verify project UserSecretsId is set in `.csproj` file

**User Secrets Command Not Found:**
Ensure .NET CLI tools are installed:
```powershell
dotnet tool install --global dotnet-ef
```

**Cannot Connect to Database:**
- Verify connection string server name/IP is correct
- Check firewall rules allow connection
- Verify credentials are correct
- Test connection outside the app (SQL Server Management Studio, Azure Data Studio)
