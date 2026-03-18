# Deployment Guide

## Overview

This guide covers deployment strategies for the NooR Restaurant Management System across different environments and platforms.

## Prerequisites

- Docker (optional, for containerized deployment)
- Cloud provider account (Azure, AWS, or Google Cloud)
- Domain name (for production)
- SSL certificate (for HTTPS)

## Environment Setup

### Development Environment
- Local development with hot reload
- SQLite or SQL Server LocalDB
- Development configuration files

### Staging Environment
- Production-like environment for testing
- Shared database instance
- SSL certificates
- Performance monitoring

### Production Environment
- High availability setup
- Production database with backups
- CDN for static assets
- Monitoring and logging

## Backend Deployment (.NET 8 Web API)

### 1. Prepare for Deployment

#### Update Configuration
```json
// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Production connection string"
  },
  "AllowedHosts": "yourdomain.com"
}
```

#### Build for Production
```bash
cd backend/api
dotnet publish -c Release -o ./publish
```

### 2. Database Migration

#### Production Database Setup
```bash
# Update connection string in appsettings.Production.json
# Run migrations
dotnet ef database update --configuration Release
```

#### Backup Strategy
- Automated daily backups
- Point-in-time recovery
- Cross-region backup replication

### 3. IIS Deployment (Windows Server)

#### Install Prerequisites
- .NET 8 Hosting Bundle
- IIS with ASP.NET Core Module

#### Deploy Application
1. Copy published files to IIS directory
2. Create application pool (.NET CLR Version: No Managed Code)
3. Create website pointing to application directory
4. Configure SSL certificate

#### web.config Example
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" arguments=".\FakeRestuarantAPI.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" />
  </system.webServer>
</configuration>
```

### 4. Linux Deployment (Ubuntu/CentOS)

#### Install .NET Runtime
```bash
# Ubuntu
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0

# CentOS
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install aspnetcore-runtime-8.0
```

#### Create Service File
```bash
# /etc/systemd/system/noor-api.service
[Unit]
Description=NooR Restaurant API
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/noor-api/FakeRestuarantAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=noor-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

#### Start Service
```bash
sudo systemctl enable noor-api.service
sudo systemctl start noor-api.service
sudo systemctl status noor-api.service
```

### 5. Docker Deployment

#### Dockerfile
```dockerfile
# Backend Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FakeRestuarantAPI.csproj", "."]
RUN dotnet restore "./FakeRestuarantAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FakeRestuarantAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FakeRestuarantAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FakeRestuarantAPI.dll"]
```

#### Build and Run
```bash
docker build -t noor-api .
docker run -d -p 8080:80 --name noor-api-container noor-api
```

## Frontend Deployment (Angular 19)

### 1. Build for Production

```bash
cd frontend/restaurant-app
npm run build --prod
```

### 2. Static File Hosting

#### Nginx Configuration
```nginx
server {
    listen 80;
    server_name yourdomain.com;
    root /var/www/noor-frontend/dist;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

#### Apache Configuration
```apache
<VirtualHost *:80>
    ServerName yourdomain.com
    DocumentRoot /var/www/noor-frontend/dist
    
    <Directory "/var/www/noor-frontend/dist">
        RewriteEngine On
        RewriteBase /
        RewriteRule ^index\.html$ - [L]
        RewriteCond %{REQUEST_FILENAME} !-f
        RewriteCond %{REQUEST_FILENAME} !-d
        RewriteRule . /index.html [L]
    </Directory>
</VirtualHost>
```

### 3. CDN Deployment

#### AWS CloudFront
1. Upload build files to S3 bucket
2. Create CloudFront distribution
3. Configure custom domain and SSL
4. Set up cache behaviors

#### Azure CDN
1. Upload files to Azure Storage
2. Create CDN profile and endpoint
3. Configure custom domain
4. Enable HTTPS

## Cloud Platform Deployment

### Azure App Service

#### Backend Deployment
```bash
# Install Azure CLI
az login
az webapp create --resource-group myResourceGroup --plan myAppServicePlan --name noor-api --runtime "DOTNETCORE|8.0"
az webapp deployment source config-zip --resource-group myResourceGroup --name noor-api --src ./publish.zip
```

#### Frontend Deployment
```bash
az storage account create --name noorstaticsite --resource-group myResourceGroup --location eastus --sku Standard_LRS
az storage blob service-properties update --account-name noorstaticsite --static-website --index-document index.html --404-document index.html
```

### AWS Deployment

#### Backend (Elastic Beanstalk)
```bash
# Install EB CLI
eb init
eb create production-api
eb deploy
```

#### Frontend (S3 + CloudFront)
```bash
# Upload to S3
aws s3 sync ./dist s3://noor-frontend-bucket --delete
# Invalidate CloudFront cache
aws cloudfront create-invalidation --distribution-id EDFDVBD6EXAMPLE --paths "/*"
```

### Google Cloud Platform

#### Backend (App Engine)
```yaml
# app.yaml
runtime: aspnetcore
env: standard

automatic_scaling:
  min_instances: 1
  max_instances: 10
```

```bash
gcloud app deploy
```

## SSL/HTTPS Configuration

### Let's Encrypt (Free SSL)
```bash
# Install Certbot
sudo apt-get install certbot python3-certbot-nginx

# Obtain certificate
sudo certbot --nginx -d yourdomain.com

# Auto-renewal
sudo crontab -e
# Add: 0 12 * * * /usr/bin/certbot renew --quiet
```

### Commercial SSL Certificate
1. Purchase SSL certificate from CA
2. Generate CSR (Certificate Signing Request)
3. Install certificate on web server
4. Configure HTTPS redirect

## Monitoring and Logging

### Application Insights (Azure)
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog Configuration
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/noor-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Health Checks
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContext<AppDbContext>();

app.MapHealthChecks("/health");
```

## Performance Optimization

### Backend Optimization
- Enable response compression
- Implement caching strategies
- Use connection pooling
- Optimize database queries

### Frontend Optimization
- Enable gzip compression
- Implement lazy loading
- Use CDN for static assets
- Optimize images and assets

## Security Considerations

### Production Security Checklist
- [ ] HTTPS enabled with valid SSL certificate
- [ ] Security headers configured
- [ ] CORS properly configured
- [ ] API rate limiting enabled
- [ ] Database connection encrypted
- [ ] Sensitive data in environment variables
- [ ] Regular security updates applied
- [ ] Backup and disaster recovery plan

### Security Headers
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

## Backup and Disaster Recovery

### Database Backup
- Automated daily backups
- Point-in-time recovery
- Cross-region replication
- Regular restore testing

### Application Backup
- Source code in version control
- Configuration management
- Infrastructure as Code
- Deployment automation

## Troubleshooting

### Common Issues
- Port conflicts
- SSL certificate problems
- Database connection issues
- CORS configuration errors
- File permission problems

### Monitoring Tools
- Application performance monitoring
- Error tracking and logging
- Uptime monitoring
- Resource usage monitoring