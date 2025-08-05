# Server.Application.Tests

This project contains comprehensive unit tests for the Server.Application layer, covering all services, mappers, and business logic.

## Project Structure

```
Server.Application.Tests/
├── Services/                    # Service layer tests
│   ├── AgentServiceTests.cs     # Tests for AgentService
│   ├── ConfigurationServiceTests.cs # Tests for ConfigurationService
│   ├── PolicyServiceTests.cs    # Tests for PolicyService
│   └── ProcessServiceTests.cs   # Tests for ProcessService
├── Mappers/                     # Mapper tests
│   ├── DomainToDtoMapperTests.cs # Tests for domain to DTO mapping
│   └── DtoToDomainMapperTests.cs # Tests for DTO to domain mapping
├── Helpers/                     # Test utilities and helpers
│   └── TestDataHelper.cs        # Common test data factory
└── README.md                    # This file
```

## Testing Approach

### 1. Service Layer Testing
Each service is tested with comprehensive unit tests covering:
- **CRUD Operations**: Create, Read, Update operations for all entities
- **Edge Cases**: Empty collections, null values, non-existent entities
- **Repository Integration**: Proper interaction with repository layer through UnitOfWork
- **Data Persistence**: Verification that SaveChanges is called appropriately
- **Error Handling**: Null returns when entities don't exist

### 2. Mapper Testing
Both mapping directions are thoroughly tested:
- **Domain to DTO**: Ensuring proper conversion from domain models to DTOs
- **DTO to Domain**: Ensuring proper conversion from DTOs to domain models
- **Edge Cases**: Empty strings, null values, empty collections
- **Data Integrity**: Verification that all properties are mapped correctly

### 3. Test Data Management
The `TestDataHelper` class provides:
- **Factory Methods**: Easy creation of test data for all entity types
- **Consistent Data**: Standardized test data across all tests
- **Edge Case Data**: Specialized methods for testing boundary conditions
- **Reusability**: Centralized test data creation to reduce duplication

## Test Categories

### Happy Path Tests
- Successful CRUD operations
- Proper data mapping
- Correct repository interactions

### Edge Case Tests
- Empty collections and strings
- Null values (where applicable)
- Non-existent entities
- Boundary values

### Integration Tests
- Service-to-repository communication
- UnitOfWork pattern verification
- SaveChanges invocation

## Running Tests

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Command Line
```bash
# Navigate to test project directory
cd src/Tests/Server.Application.Tests

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~AgentServiceTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~GetAgentsAsync_ShouldReturnAllAgents"
```

### Visual Studio
1. Open the solution in Visual Studio
2. Open Test Explorer (Test > Test Explorer)
3. Run all tests or specific test categories

## Test Naming Convention

Tests follow a clear naming pattern:
```
[MethodName]_[Scenario]_[ExpectedResult]
```

Examples:
- `GetAgentsAsync_ShouldReturnAllAgents`
- `UpdateAgentAsync_WhenAgentDoesNotExist_ShouldReturnNull`
- `CreateAgentAsync_ShouldCallSaveChanges`

## Mocking Strategy

The tests use Moq for mocking dependencies:
- **IUnitOfWork**: Mocked to control repository access
- **Repository Interfaces**: Mocked to return controlled test data
- **Verification**: Ensures proper method calls and parameters

## Coverage Areas

### AgentService
- ✅ Get all agents
- ✅ Get agent by ID
- ✅ Create new agent
- ✅ Update existing agent
- ✅ Handle non-existent agents
- ✅ Verify SaveChanges calls

### ConfigurationService
- ✅ Get all configurations
- ✅ Get configuration by ID
- ✅ Create new configuration
- ✅ Update existing configuration
- ✅ Handle complex configuration with agents, processes, and policies
- ✅ Handle non-existent configurations

### PolicyService
- ✅ Get all policies
- ✅ Get policy by ID
- ✅ Create new policy
- ✅ Update existing policy
- ✅ Handle all policy properties (registry settings)
- ✅ Handle non-existent policies

### ProcessService
- ✅ Get all processes
- ✅ Get process by ID
- ✅ Create new process
- ✅ Update existing process
- ✅ Handle non-existent processes
- ✅ Edge cases with empty names

### Mappers
- ✅ Domain to DTO mapping for all entities
- ✅ DTO to Domain mapping for all entities
- ✅ Edge cases with empty/zero values
- ✅ Collection mapping
- ✅ Complex object mapping

## Best Practices

1. **Arrange-Act-Assert**: All tests follow the AAA pattern
2. **Descriptive Names**: Test names clearly describe the scenario and expected outcome
3. **Single Responsibility**: Each test focuses on one specific behavior
4. **Mock Verification**: Verify that mocked dependencies are called correctly
5. **Edge Case Coverage**: Test boundary conditions and error scenarios
6. **Test Data Reuse**: Use TestDataHelper for consistent test data
7. **Async Testing**: Proper async/await patterns for all asynchronous operations

## Maintenance

When adding new features to the Application layer:
1. Add corresponding tests for new service methods
2. Update TestDataHelper if new test data patterns are needed
3. Ensure mapper tests cover new DTOs or domain models
4. Maintain test coverage above 90%

## Dependencies

- **NUnit**: Testing framework
- **Moq**: Mocking framework
- **Microsoft.NET.Test.Sdk**: Test SDK
- **coverlet.collector**: Code coverage collection 