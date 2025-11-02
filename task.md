---
title: task description for agent
date: '2025-11-02'

author:
  full_name: Jan Ritt
  github:
    username: IxI-Enki
    url: 'https://github.com/IxI-Enki'

agent:
  model: 'Claude 4.5 Sonnet thinking'
  provider: Anthropic
  integrated_development_environment: Cursor
---

## Instruction for 🤖 Agent

- **Take a look at:**

  - [assignment](files/angabe/devices.md)

  - [exercise project](clean_architecture_08)

  - [original reference project (template)](files/template/)

  - [data](files/data.csv)

---

- **Create a task list for yourself below**

  - [x] <task_01>: **Analyze Assignment & Template Structure**
    > - Description: Deep analysis of devices.md assignment and template project structure
    > - Expected Result: Complete understanding of Clean Architecture + CQRS pattern, validation layers, and data flow
    > - Notes: Template follows: Domain (Entities, Validations) → Application (DTOs, Commands/Queries, Handlers) → Infrastructure (Repositories, DbContext) → Api (Controllers). Key patterns: Factory methods for entities, ISensorUniquenessChecker for domain services, Result<T> pattern, ValidationBehavior, Mapster for mapping, StartupDataSeeder for CSV import

  - [x] <task_02>: **Setup Project Structure in clean_architecture_08**
    > - Description: Create complete solution with all projects (Domain, Application, Infrastructure, Api, Tests)
    > - Expected Result: Matching folder structure and project references as in template
    > - Notes: Must include: Domain.csproj, Application.csproj, Infrastructure.csproj, Api.csproj, Api.Tests.csproj, Domain.Tests.csproj

  - [x] <task_03>: **Domain Layer - Entities & Enums**
    > - Description: Create Device, Person, Usage entities + DeviceType enum with validation specifications
    > - Expected Result: BaseEntity inheritance, private setters, factory methods (CreateAsync/Create), domain validations
    > - Notes: Device (SerialNumber >=3, DeviceName >=2, DeviceType enum), Person (Email unique, no empty fields, valid email), Usage (date validations, no overlaps, future bookings only except import)

  - [x] <task_04>: **Domain Layer - Validation Specifications**
    > - Description: Create DeviceSpecifications, PersonSpecifications, UsageSpecifications similar to SensorSpecifications
    > - Expected Result: Static validation methods returning DomainValidationResult for each property
    > - Notes: Follow exact pattern from template: CheckXxx methods with DomainValidationResult.Failure/Success

  - [x] <task_05>: **Domain Layer - Domain Services & Contracts**
    > - Description: Create IPersonUniquenessChecker (email), IUsageOverlapChecker interfaces
    > - Expected Result: Domain service contracts for cross-entity validations
    > - Notes: Similar to ISensorUniquenessChecker - must be implemented in Application layer

  - [x] <task_06>: **Application Layer - DTOs**
    > - Description: Create GetDeviceDto, GetPersonDto, GetUsageDto, GetDeviceWithCountDto
    > - Expected Result: Simple record structs for API responses
    > - Notes: Follow template pattern: readonly record struct with properties matching entity

  - [x] <task_07>: **Application Layer - Repositories & UnitOfWork**
    > - Description: Create IDeviceRepository, IPersonRepository, IUsageRepository interfaces
    > - Expected Result: Generic repository pattern with specific methods (e.g., GetByEmailAsync for Person)
    > - Notes: Must extend IGenericRepository<T>, add entity-specific query methods

  - [x] <task_08>: **Application Layer - Commands (Create/Update/Delete)**
    > - Description: Create all CQRS commands with handlers and validators for Device, Person, Usage
    > - Expected Result: Command records, CommandHandlers, FluentValidation validators (mostly empty as in template)
    > - Notes: 9 commands total: Create/Update/Delete for each entity. Follow exact pattern from template

  - [x] <task_09>: **Application Layer - Queries (GetAll/GetById/Special)**
    > - Description: Create all queries with handlers: GetAll, GetById for each entity + GetDevicesWithCounts
    > - Expected Result: Query records, QueryHandlers returning Result<T> or Result<IEnumerable<T>>
    > - Notes: Special query: GetDevicesWithCountsQuery (devices with usage count)

  - [x] <task_10>: **Application Layer - Domain Services Implementation**
    > - Description: Implement PersonUniquenessChecker, UsageOverlapChecker in Application/Services
    > - Expected Result: Services using UnitOfWork to check uniqueness/overlaps
    > - Notes: Follow SensorUniquenessChecker pattern exactly

  - [x] <task_11>: **Infrastructure Layer - AppDbContext Configuration**
    > - Description: Configure DbContext with DbSets and OnModelCreating for all 3 entities
    > - Expected Result: Fluent API config: max lengths, indexes, unique constraints, relationships, RowVersion
    > - Notes: Device: unique SerialNumber; Person: unique Email; Usage: FK to Device & Person, index on dates

  - [x] <task_12>: **Infrastructure Layer - Repository Implementations**
    > - Description: Implement GenericRepository, DeviceRepository, PersonRepository, UsageRepository
    > - Expected Result: Concrete repository classes with entity-specific queries
    > - Notes: PersonRepository needs GetByEmailAsync, UsageRepository needs overlap checking queries

  - [x] <task_13>: **Infrastructure Layer - StartupDataSeeder**
    > - Description: Implement CSV parser to import devices.csv data into Device, Person, Usage
    > - Expected Result: Idempotent seeder that parses CSV, creates entities, handles relationships
    > - Notes: CSV format: SerialNumber; DeviceName; DeviceType; PersonLastName; PersonFirstName; MailAddress; From; To. Must parse dates (dd.MM.yyyy), create Persons (unique by email), Devices (unique by SerialNumber), Usages

  - [x] <task_14>: **Infrastructure Layer - DependencyInjection Setup**
    > - Description: Configure Infrastructure.DependencyInjection.cs with all services
    > - Expected Result: Extension method AddInfrastructure registering DbContext, Repositories, UoW, Seeder
    > - Notes: Follow template pattern exactly

  - [x] <task_15>: **Api Layer - Controllers**
    > - Description: Create DevicesController, PeopleController, UsagesController with all endpoints
    > - Expected Result: Full CRUD + special endpoint /api/devices/with-counts
    > - Notes: Follow SensorsController pattern: [HttpGet], [HttpPost], [HttpPut], [HttpDelete], result.ToActionResult(this)

  - [x] <task_16>: **Api Layer - Program.cs & Configuration**
    > - Description: Configure Program.cs, appsettings.json, copy data.csv to Api project
    > - Expected Result: Swagger setup, connection string, CSV path configuration
    > - Notes: Copy files/angabe/data.csv to Api project root, update CSV path in Program.cs

  - [x] <task_17>: **Create Initial Migration**
    > - Description: Run dotnet ef migrations add Initial and create database schema
    > - Expected Result: Migration files created, database schema ready
    > - Notes: Command: dotnet ef migrations add Initial --project ./Infrastructure --startup-project ./Api --output-dir ./Persistence/Migrations

  - [x] <task_18>: **Testing - Domain.Tests**
    > - Description: Create unit tests for Device, Person, Usage entity validations
    > - Expected Result: Tests covering all validation rules, edge cases
    > - Notes: Follow Domain.Tests/SensorTests.cs pattern

  - [x] <task_19>: **Testing - Api.Tests**
    > - Description: Create integration tests for all controllers and endpoints
    > - Expected Result: Tests using CustomWebApplicationFactory, covering CRUD operations
    > - Notes: Follow Api.Tests pattern with WebApplicationFactory

  - [x] <task_20>: **Manual Testing with Swagger**
    > - Description: Start application, test all endpoints via Swagger UI
    > - Expected Result: All endpoints working, validations correct, error messages matching expected
    > - Notes: Test overlap validation, uniqueness checks, date validations

  - [x] <task_21>: **Git Commits & Documentation**
    > - Description: Make regular commits during development, ensure clean git history
    > - Expected Result: Logical commits with clear messages
    > - Notes: Commit after each major milestone (entities done, commands done, etc.)

---

❗ Assignment has to be implemented in the [exercise project](clean_architecture_08)

- 🛑 **Do not deviate from given patterns and code structure** → *strictly adhere to patterns & structures of given project/scriptum* ❗

- 🛑 **Do not implement new patterns or code structures** → *strictly adhere to patterns & structures of given project/scriptum* ❗

- 🛑 **Do not implement validations in wrong layers** → *strictly adhere to layers of given project/scriptum* ❗
