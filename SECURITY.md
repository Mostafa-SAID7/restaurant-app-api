# 🔒 Security Architecture & Policy

Security is a foundational pillar of the **Restaurant API**. This document outlines the protocols and measures implemented to ensure data integrity and system resilience for the **Apptunix** submission.

## 🛡️ Core Security Measures

### 1. API Key Authentication
- Every non-public endpoint is guarded by a **Custom API Key Authorization Filter**.
- API Keys are unique to users and must be provided in the `apiKey` header.
- This ensures that only registered and authorized clients can interact with sensitive data (Orders, Cart, etc.).

### 2. Data Integrity & Injection Prevention
- **EF Core Parameterization**: All database interactions use Entity Framework Core's parameterized queries, providing native protection against SQL Injection.
- **Input Validation**: Robust validation using Data Annotations and custom logic to sanitize all incoming DTOs.
- **AutoMapper**: Prevents over-posting attacks by using dedicated DTOs (Data Transfer Objects) instead of exposing database entities directly.

### 3. Infrastructure Security
- **CORS Policy**: Configured to allow only trusted origins, preventing unauthorized cross-domain requests.
- **Secure Headers**: The API is configured to serve essential security headers to mitigate common web vulnerabilities.
- **Environment Isolation**: Sensitive configurations (Connection Strings, API Keys) are managed via `appsettings.json` and environmental variables.

---

## 🚀 Vulnerability Management

While this project is a technical demonstration for **Apptunix**, we adhere to professional security practices:

| Priority | Strategy |
| :--- | :--- |
| **Detection** | Regular dependency scanning and logging of unauthorized access attempts. |
| **Response** | Rapid patching of identified vulnerabilities in the .NET runtime or NuGet packages. |
| **Isolation** | Containerization via Docker provides an additional layer of process isolation. |

---

## 🛠️ Best Practices for Developers

- **Header Management**: Always include the `apiKey` header for protected routes.
- **Error Handling**: Middleware captures exceptions and returns sanitized JSON responses, ensuring stack traces are never exposed in production.
- **Least Privilege**: The database user is configured with the minimum permissions required for API operations.

---

[⬅️ Back to Main README](../README.md)