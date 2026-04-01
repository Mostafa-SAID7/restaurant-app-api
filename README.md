# M.Said's Restaurant Management System

A comprehensive restaurant management system built with .NET 8 Web API backend and Angular 19 frontend, featuring modern architecture patterns and clean code principles.

## 🚀 Features

- **Restaurant Management**: Complete CRUD operations for restaurants
- **Menu Management**: Dynamic menu creation and item management
- **User Authentication**: Secure user registration and API key-based authentication
- **Order Processing**: Full order lifecycle management with master orders
- **Shopping Cart**: Real-time cart management with item tracking
- **Image Upload**: Support for both file upload and base64 image handling
- **Clean Architecture**: Repository pattern with Unit of Work implementation
- **Modern Frontend**: Angular 19 with responsive design

## 🏗️ Architecture

### Backend (.NET 8 Web API)
- **Repository Pattern**: Clean data access layer with generic repositories
- **Unit of Work**: Transaction management and coordinated saves
- **Service Layer**: Business logic separation with dependency injection
- **AutoMapper**: Object-to-object mapping for DTOs
- **Swagger Documentation**: Comprehensive API documentation
- **Entity Framework Core**: Code-first database approach

### Frontend (Angular 19)
- **Component-based Architecture**: Modular and reusable components
- **Reactive Forms**: Type-safe form handling
- **HTTP Interceptors**: Centralized request/response handling
- **Responsive Design**: Mobile-first approach with modern UI

## 📋 Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

## 🛠️ Quick Start

### Backend Setup
```bash
cd backend/api
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend Setup
```bash
cd frontend/restaurant-app
npm install
ng serve
```

## 📚 Documentation

- [📖 Project Setup Guide](docs/PROJECT_SETUP.md)
- [🏛️ Architecture & Structure](docs/STRUCTURE.md)
- [🔧 Technologies Used](docs/TECHNOLOGIES.md)
- [✨ Features Overview](docs/FEATURES.md)
- [🎯 Use Cases](docs/USE_CASES.md)
- [🚀 Deployment Guide](docs/DEPLOYMENT.md)
- [🎨 Styling Guide](docs/STYLES.md)
- [📊 Database ERD](docs/ERD.md)
- [📝 Changelog](docs/CHANGELOG.md)
- [🔒 Security](docs/SECURITY.md)

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](docs/CONTRIBUTING.md) and [Code of Conduct](docs/CODE_OF_CONDUCT.md).

## 👥 Contributors

See [CONTRIBUTORS.md](docs/CONTRIBUTORS.md) for a list of contributors to this project.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions, please [open an issue](https://github.com/Mostafa-SAID7/restaurant-app-api/issues) on GitHub.

---

**Built with ❤️ by [M.Said](https://m-said-portfolio.netlify.app)**