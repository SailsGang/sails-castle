# Contributing to SailsEnergy

Thank you for your interest in contributing! 🎉

## Development Setup

1. **Fork and clone** the repository
2. **Install dependencies**: .NET 10 SDK, Docker
3. **Start services**: `docker compose up -d`
4. **Run tests**: `dotnet test`

## Code Style

-   Follow C# naming conventions
-   Use `record` for DTOs and commands
-   Handlers are `static` classes with `HandleAsync` methods
-   Use FluentValidation for request validation

## Branch Naming

-   `feature/short-description` - New features
-   `fix/issue-number-description` - Bug fixes
-   `refactor/description` - Code improvements

## Pull Request Process

1. Create a feature branch from `main`
2. Make your changes with clear commits
3. Ensure all tests pass
4. Update documentation if needed
5. Open a PR with a clear description

## Commit Messages

Use conventional commits:

```
feat: add energy export feature
fix: correct tariff calculation
docs: update API documentation
refactor: simplify gang authorization
test: add period report tests
```

## Questions?

Open an issue or discussion for any questions.
