# 📋 Changelog

All notable changes to **Restaurant API** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - 2026-04-01

### Added
- **Infrastructure**: Automated GitHub Releases with `release-drafter` configuration.
- **CI/CD**: Added `stale.yml`, `auto-merge.yml`, and `codeql.yml` for professional PR management.
- **Automation**: Setup automated labeling and PR tracking.
- **Naming**: Global standardization of DTO and Migration classes (ProperCase).

### Changed
- **Data Layer**: Migrated `AppDbContext` to use plural collection names (`Users`, `Carts`, `Orders`, `Restaurants`).
- **Repositories**: Standardized `UnitOfWork` and repository properties for domain adherence.

### Fixed
- **API**: Resolved `MasterOrderDTO` naming collisions and duplicate class definitions.
- **Swagger**: Fixed namespace ambiguity with `Microsoft.OpenApi.Models` aliasing.
- **Types**: Fixed several nullability and type warnings.

## [1.0.0] - 2026-03-18

### Added
- Initial project setup with Repository pattern and Unit of Work.
- Comprehensive service layer architecture with AutoMapper.
- Swagger API documentation with cinematic styling.
- Shopping cart and Order processing management.
- User authentication with API keys.
- Angular 19 frontend integration.

### Changed
- Refactored services to use dependency injection and clean architecture.
- Improved error handling and global exception management.

### Fixed
- Resolved duplicate method implementations and compilation errors.
- Improved null reference handling and data validation.

### Security
- API key-based authentication and CORS configuration.
- SQL injection prevention through EF Core patterns.

---

[1.1.0]: https://github.com/Mostafa-SAID7/restaurant-app-api/releases/tag/v1.1.0
[1.0.0]: https://github.com/Mostafa-SAID7/restaurant-app-api/releases/tag/v1.0.0

[⬅️ Back to Main README](../README.md)