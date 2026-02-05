# Contributing to Jarvis

Thank you for your interest in contributing to the Jarvis project! This document outlines our contribution workflow, standards, and guidelines.

## Workflow Overview

All contributions follow a structured workflow to maintain code quality, traceability, and organization:

```
1. Create GitHub Work Item
     ↓
2. Create Feature Branch
     ↓
3. Make Changes & Tests
     ↓
4. Commit to Local Branch
     ↓
5. Push to Remote
     ↓
6. Create Pull Request
     ↓
7. Code Review
     ↓
8. Merge to Main
```

## Work Item Creation

**Every change must have a corresponding GitHub work item before implementation.**

### Feature Work Items
Create a **Feature** issue for:
- New API endpoints
- New services or utilities
- Infrastructure improvements
- Configuration enhancements
- Any new functionality

**Command:**
```bash
gh issue create --title "Feature: [Brief description]" --body "[Detailed description]"
```

**Example:**
```bash
gh issue create --title "Feature: Add user authentication endpoint" \
  --body "Implement OAuth 2.0 authentication

- Create /auth/login endpoint
- Implement token refresh mechanism
- Add authentication middleware
- Write unit tests"
```

### Bug Work Items
Create a **Bug** issue for:
- Defects in existing code
- Performance issues
- Security vulnerabilities
- Incorrect behavior
- Failed tests

**Command:**
```bash
gh issue create --title "Bug: [Brief description]" --body "[Detailed description with reproduction steps]"
```

**Example:**
```bash
gh issue create --title "Bug: API returns 500 on invalid input" \
  --body "Describe the issue and reproduction steps"
```

### Document Work Items in Code Commits

Reference the work item number in your commit messages:

```bash
git commit -m "Feature/#1 - Implement user authentication

Adds OAuth 2.0 endpoints for login and token refresh.
Includes unit tests and documentation.

Closes #1"
```

## Development Workflow

### 1. Create Feature Branch
Always branch from `main`:

```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

Branch naming conventions:
- Features: `feature/short-description`
- Bugs: `fix/short-description`
- Documentation: `docs/short-description`

### 2. Development Standards

#### Code Style
- Follow Microsoft C# Code Conventions
- Use meaningful variable and method names
- Keep methods focused and single-purpose
- Write self-documenting code

#### Testing
- Write unit tests for all new features
- Ensure all tests pass: `dotnet test`
- Aim for >80% code coverage
- Tests go in `tests/Jarvis.Tests/`

#### Commits
- Make small, logical commits
- Use imperative mood in commit messages ("Add feature", not "Added feature")
- Include work item reference: `Feature/#123 - Description`
- Keep commits atomic (one logical change per commit)

**Example commit message:**
```
Feature/#2 - Add health check endpoint

- Implement /health endpoint
- Return service status and version
- Add Application Insights integration

Relates to #2
```

### 3. Push to Remote

Push your feature branch:

```bash
git push origin feature/your-feature-name
```

### 4. Create Pull Request

Create a PR with a clear description:

```bash
gh pr create --title "Feature: [Brief description]" \
  --body "## Description
[What this PR does]

## Related Issue
Closes #[issue-number]

## Changes
- Change 1
- Change 2

## Testing
- How to test locally
- Screenshots if applicable

## Checklist
- [ ] Tests pass locally
- [ ] Code follows style guide
- [ ] No new warnings generated
- [ ] Documentation updated"
```

**PR Requirements:**
- All tests must pass
- At least one code review approval
- No merge conflicts
- Squash commits before merging (optional, based on preference)

### 5. Code Review Process

**As Author:**
- Respond to review comments
- Request re-review after making changes
- Keep PRs focused and reasonably sized

**As Reviewer:**
- Check functionality and logic
- Verify test coverage
- Ensure code style compliance
- Look for potential issues
- Approve or request changes

## Coding Standards

### C# Guidelines
```csharp
// Good: Clear, descriptive naming
public async Task<UserAuthResponse> AuthenticateUserAsync(string username, string password)
{
    // Implementation
}

// Avoid: Vague or abbreviated names
public async Task<Response> auth(string u, string p)
{
    // Implementation
}
```

### Project Structure
```
src/Jarvis.Api/
├── Controllers/          # API endpoints
├── Services/             # Business logic
├── Models/               # Data models
├── Middleware/           # Request/response middleware
└── Program.cs            # Application configuration

tests/Jarvis.Tests/
├── Controllers/          # Controller tests
├── Services/             # Service tests
└── Mocks/                # Mock objects and helpers
```

### API Endpoint Standards

**Naming:**
- Use RESTful conventions
- Use plural nouns: `/users`, not `/user`
- Use hyphens in long names: `/user-profiles`

**HTTP Methods:**
- GET - Retrieve resource
- POST - Create resource
- PUT - Update entire resource
- PATCH - Partial resource update
- DELETE - Remove resource

**Response Format:**
```json
{
  "success": true,
  "data": {
    "id": "123",
    "name": "Example"
  },
  "message": "Operation successful",
  "timestamp": "2024-02-05T10:30:00Z"
}
```

## Testing Requirements

### Unit Tests
- Test business logic in Services
- Mock external dependencies
- Use MSTest framework
- File naming: `[Class]Tests.cs`

```csharp
[TestClass]
public class UserServiceTests
{
    [TestMethod]
    public async Task AuthenticateUser_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var service = new UserService();

        // Act
        var result = await service.AuthenticateAsync("user", "pass");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
    }
}
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~UserServiceTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

### Minimum Coverage
- New features: Minimum 80% coverage
- Bug fixes: Should include regression test
- Existing code: No coverage reduction

## Documentation

### Code Documentation
- Use XML documentation comments for public members
- Include examples for complex methods

```csharp
/// <summary>
/// Authenticates a user with the provided credentials.
/// </summary>
/// <param name="username">The username to authenticate</param>
/// <param name="password">The user's password</param>
/// <returns>An authentication response containing token if successful</returns>
/// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid</exception>
public async Task<AuthResponse> AuthenticateAsync(string username, string password)
{
    // Implementation
}
```

### README Updates
- Update README.md for new features affecting users
- Document new configuration options
- Add examples of new endpoints

### WORKLOG Updates
After significant features, update WORKLOG.md with:
- Summary of changes
- Architecture modifications (if any)
- Breaking changes (if any)
- Migration notes (if applicable)

## Performance Considerations

### Best Practices
- Use `async/await` for I/O operations
- Cache frequently accessed data
- Use connection pooling for databases
- Implement pagination for large datasets
- Monitor Application Insights metrics

### Before Optimization
1. Identify actual bottleneck (profile first)
2. Measure before/after performance
3. Ensure optimization worth the complexity

## Security Guidelines

### Secrets Management
- Never commit secrets or credentials
- Use Azure Key Vault for sensitive data
- Store environment variables in `.env` (gitignored)
- Use managed identities in Azure

### Input Validation
- Validate all user input
- Use parameterized queries (prevents SQL injection)
- Sanitize API inputs
- Implement rate limiting

### HTTPS & TLS
- Always use HTTPS in production
- Minimum TLS 1.2
- Keep SSL certificates current

## Deployment

### Local Testing
```bash
# Build locally
dotnet build

# Run tests
dotnet test

# Test endpoints locally
dotnet run

# Publish release build
dotnet publish -c Release -o publish
```

### Deployment to Azure
Use the deployment script:
```bash
./deploy.sh
```

### CI/CD Pipeline
All commits to `main` automatically:
1. Build the project
2. Run tests
3. Deploy to Azure App Service

Failed builds will block deployment.

## Common Issues & Troubleshooting

### Build Failures
```bash
# Clean and rebuild
dotnet clean
dotnet build

# Restore NuGet packages
dotnet restore
```

### Test Failures
```bash
# Run specific failing test with verbose output
dotnet test --filter "TestName" --verbosity normal

# Run with debugging
dotnet test --logger "console;verbosity=detailed"
```

### Git Issues
```bash
# Update local main
git fetch origin
git checkout main
git pull origin main

# Sync feature branch with main
git rebase main feature/your-feature
```

## Questions or Issues?

- Check existing issues: https://github.com/fjordsnapper/jarvis/issues
- Review WORKLOG.md for project history
- Read README.md for project overview
- Create a new issue for questions

## Code of Conduct

- Be respectful and professional
- Provide constructive feedback
- Help others when you can
- Report security issues privately

---

**Remember:** Quality over speed. Take time to write good code, comprehensive tests, and clear documentation.

Thank you for contributing to Jarvis!
