# 🚢 Deployment Strategy

This document outlines the professional deployment procedures for the **Restaurant API**, ensuring a smooth transition from development to production for the **Apptunix** submission.

---

## 🐳 Docker Containerization (Primary)

The recommended way to deploy the API is using **Docker**. This ensures that the application, its dependencies, and the Cinematic UI are bundled together in a consistent environment.

### 1. Build the Image
```bash
docker build -t msaid-restaurant-api .
```

### 2. Run the Container
```bash
docker run -d -p 8080:80 --name restaurant-api msaid-restaurant-api
```

### 3. Multi-Stage Build Benefits
- **Optimization**: The `Dockerfile` uses a multi-stage build to keep the final image size minimal by only including the necessary runtime binaries.
- **Security**: The application runs in a lightweight, isolated environment.

---

## ☁️ Cloud & Server Deployment

### 1. Manual .NET Publish
If you are deploying to a Windows/Linux server without Docker:
```bash
dotnet publish -c Release -o ./publish
```
Then, copy the contents of the `./publish` directory to your web server (IIS, Nginx, or Apache).

### 2. Database Migration in Production
Ensure the production database schema is up-to-date before launching:
```bash
dotnet ef database update --connection "Your_Production_Connection_String"
```

---

## 🛠️ CI/CD Pipeline

The project includes a **GitHub Actions** workflow (`.github/workflows/dotnet.yml`) that automates the following on every push to the `main` branch:
1. **Restore dependencies**
2. **Build the project**
3. **Run automated tests** (if any)

This ensures that the "Cinematic" experience is never broken by new code changes.

---

## 🔒 Post-Deployment Checklist

- [ ] **HTTPS**: Verify that SSL certificates are installed and active.
- [ ] **Connection Strings**: Ensure sensitive database credentials are encrypted or stored in secure environment variables.
- [ ] **CORS**: Update allowed origins to match your production domain.
- [ ] **API Keys**: rotate any development API keys before going live.

---

[⬅️ Back to Main README](../README.md)