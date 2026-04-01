# 🤝 Contributing to the Restaurant API

Thank you for your interest in contributing! This document provides guidelines and information for contributors.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally
3. **Create a feature branch** from `main`
4. **Make your changes** following our coding standards
5. **Test your changes** thoroughly
6. **Submit a pull request**

## Development Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Git

### Backend Setup
```bash
cd api
dotnet restore
dotnet ef database update
dotnet run
```

## Coding Standards

### Backend (.NET)
- Follow C# naming conventions (PascalCase for public members)
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Follow Repository pattern and dependency injection
- Write unit tests for new functionality
- Use async/await for database operations

## Pull Request Process

1. **Update documentation** if needed
2. **Add tests** for new features
3. **Ensure all tests pass**
4. **Update [CHANGELOG.md](CHANGELOG.md)** with your changes
5. **Request review** from maintainers

## Reporting Issues

When reporting issues, please include:
- Clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, .NET version, etc.)
- Screenshots if applicable

## Feature Requests

For new features:
- Check existing issues first
- Provide detailed use case
- Explain the benefit to users
- Consider implementation complexity

## Code Review Guidelines

- Be respectful and constructive
- Focus on code quality and maintainability
- Suggest improvements, don't just point out problems
- Test the changes locally when possible

---

[⬅️ Back to Main README](../README.md)