# ASP.NET Core Project Structure Best Practices

## 📁 Organizing Your ASP.NET Core Applications

### Overview

A well-organized project structure is crucial for maintainability, scalability, and team collaboration. This guide provides best practices for structuring ASP.NET Core applications of various sizes and complexities.

## 🏗️ Basic Project Structure

### Small Application (< 10 controllers/pages)

```
MyApp/
├── Controllers/              # API Controllers
├── Models/                   # Domain models and DTOs
├── Pages/ or Views/         # Razor Pages or MVC Views  
├── Services/                # Business logic
├── Data/                    # Database context and migrations
├── wwwroot/                 # Static files
│   ├── css/
│   ├── js/
│   ├── lib/                # Third-party libraries
│   └── images/
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── MyApp.csproj
```

### Medium Application (10-50 controllers/pages)

```
MyApp/
├── Controllers/
│   ├── Api/                 # API controllers
│   └── Mvc/                 # MVC controllers
├── Models/
│   ├── Domain/              # Business entities
│   ├── DTOs/                # Data transfer objects
│   └── ViewModels/          # View-specific models
├── Services/
│   ├── Interfaces/          # Service contracts
│   └── Implementations/     # Service implementations
├── Data/
│   ├── Entities/            # EF Core entities
│   ├── Configurations/      # Entity configurations
│   ├── Migrations/          # Database migrations
│   └── ApplicationDbContext.cs
├── Infrastructure/          # Cross-cutting concerns
│   ├── Middleware/
│   ├── Filters/
│   └── Extensions/
├── Pages/ or Views/
│   └── Shared/              # Shared components
├── wwwroot/
├── Configuration/           # Configuration classes
├── Constants/               # Application constants
└── Program.cs
```

### Large Application / Clean Architecture

```
Solution/
├── src/
│   ├── MyApp.Domain/        # Core business logic (no dependencies)
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   ├── MyApp.Application/   # Application business rules
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   ├── Mappings/
│   │   └── Validators/
│   ├── MyApp.Infrastructure/ # External concerns
│   │   ├── Data/
│   │   ├── Identity/
│   │   ├── FileStorage/
│   │   ├── Services/
│   │   └── Logging/
│   └── MyApp.Web/           # Presentation layer
│       ├── Controllers/
│       ├── Pages/
│       ├── ViewModels/
│       ├── Middleware/
│       ├── Filters/
│       └── wwwroot/
├── tests/
│   ├── MyApp.Domain.Tests/
│   ├── MyApp.Application.Tests/
│   ├── MyApp.Infrastructure.Tests/
│   └── MyApp.Web.Tests/
└── MyApp.sln
```

## 📋 Folder Purposes and Guidelines

### Controllers/
- **Purpose**: Handle HTTP requests and responses
- **Best Practices**:
  ```csharp
  // Group by feature or resource
  Controllers/
  ├── Auth/
  │   ├── LoginController.cs
  │   └── RegisterController.cs
  ├── Products/
  │   ├── ProductsController.cs
  │   └── ProductCategoriesController.cs
  ```

### Models/
- **Purpose**: Define data structures
- **Organization**:
  ```
  Models/
  ├── Domain/           # Business entities
  │   ├── Product.cs
  │   └── Category.cs
  ├── DTOs/            # Data transfer objects
  │   ├── ProductDto.cs
  │   └── CreateProductDto.cs
  └── ViewModels/      # View-specific models
      ├── ProductListViewModel.cs
      └── ProductDetailsViewModel.cs
  ```

### Services/
- **Purpose**: Contain business logic
- **Pattern**:
  ```csharp
  Services/
  ├── Interfaces/
  │   ├── IProductService.cs
  │   └── IEmailService.cs
  └── Implementations/
      ├── ProductService.cs
      └── SmtpEmailService.cs
  ```

### Data/
- **Purpose**: Database-related code
- **Contents**:
  ```
  Data/
  ├── ApplicationDbContext.cs
  ├── Entities/              # EF Core entities
  ├── Configurations/        # Fluent API configurations
  ├── Migrations/           # Auto-generated migrations
  ├── Repositories/         # Repository pattern (if used)
  └── Seeding/             # Database seed data
  ```

## 🎯 Naming Conventions

### Files and Folders

| Type | Convention | Example |
|------|------------|---------|
| **Folders** | PascalCase | `Controllers`, `Services` |
| **Classes** | PascalCase | `ProductService.cs` |
| **Interfaces** | IPascalCase | `IProductService.cs` |
| **Private fields** | _camelCase | `_productService` |
| **Properties** | PascalCase | `public string Name { get; set; }` |
| **Methods** | PascalCase | `GetProductById()` |
| **Parameters** | camelCase | `GetProduct(int productId)` |
| **Constants** | UPPER_CASE | `MAX_RETRY_COUNT` |

### Project Names

```
CompanyName.ProductName.LayerName
├── Contoso.Shop.Domain
├── Contoso.Shop.Application  
├── Contoso.Shop.Infrastructure
└── Contoso.Shop.Web
```

## 🔧 Configuration Organization

### AppSettings Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Features": {
    "EnableNewUI": true,
    "MaxUploadSize": 10485760
  },
  "ExternalServices": {
    "EmailService": {
      "ApiKey": "...",
      "BaseUrl": "..."
    }
  }
}
```

### Configuration Classes

```csharp
Configuration/
├── AppSettings.cs
├── DatabaseSettings.cs
├── EmailSettings.cs
└── FeatureFlags.cs
```

## 🛡️ Security Best Practices

### Secrets Management

```
// Never commit these files
├── appsettings.Development.json  # Use user secrets instead
├── .env                          # Environment variables
└── secrets.json                  # Any secrets file

// Use User Secrets in development
dotnet user-secrets init
dotnet user-secrets set "ApiKey" "12345"
```

### Sensitive Data Organization

```
Infrastructure/
├── Security/
│   ├── Encryption/
│   ├── Authentication/
│   └── Authorization/
```

## 📦 Static Files Organization

```
wwwroot/
├── css/
│   ├── site.css           # Main stylesheet
│   └── components/        # Component-specific styles
├── js/
│   ├── site.js           # Main JavaScript
│   └── modules/          # JavaScript modules
├── lib/                  # Third-party libraries (via LibMan)
│   ├── bootstrap/
│   └── jquery/
├── images/
│   ├── icons/
│   └── content/
└── uploads/              # User-uploaded content (if stored locally)
```

## 🧩 Feature-Based Organization (Alternative)

Instead of organizing by technical layers, organize by features:

```
MyApp/
├── Features/
│   ├── Products/
│   │   ├── ProductsController.cs
│   │   ├── ProductService.cs
│   │   ├── ProductRepository.cs
│   │   ├── ProductDto.cs
│   │   └── Views/
│   ├── Orders/
│   │   ├── OrdersController.cs
│   │   ├── OrderService.cs
│   │   ├── OrderRepository.cs
│   │   └── OrderDto.cs
│   └── Auth/
│       ├── AuthController.cs
│       ├── AuthService.cs
│       └── Views/
├── Shared/
│   ├── BaseController.cs
│   ├── Extensions/
│   └── Middleware/
└── Infrastructure/
```

## 🎨 Frontend Assets Organization

### For Razor Pages/MVC

```
Pages/
├── Shared/
│   ├── _Layout.cshtml
│   ├── _ValidationScriptsPartial.cshtml
│   └── Components/
│       ├── Header.cshtml
│       └── Footer.cshtml
├── Products/
│   ├── Index.cshtml
│   ├── Index.cshtml.cs
│   ├── Details.cshtml
│   └── Details.cshtml.cs
```

### For SPA Integration (React/Angular/Vue)

```
ClientApp/
├── src/
│   ├── components/
│   ├── services/
│   ├── models/
│   └── assets/
├── public/
└── package.json
```

## 📝 Documentation Structure

```
Project/
├── docs/
│   ├── architecture/
│   │   ├── decisions/      # Architecture Decision Records
│   │   └── diagrams/
│   ├── api/               # API documentation
│   ├── deployment/        # Deployment guides
│   └── development/       # Development guides
├── README.md             # Project overview
├── CONTRIBUTING.md       # Contribution guidelines
└── CHANGELOG.md         # Version history
```

## ✅ Checklist for Good Structure

- [ ] **Separation of Concerns**: Each folder has a clear, single responsibility
- [ ] **Consistency**: Similar items are grouped together
- [ ] **Scalability**: Structure can grow without becoming unwieldy
- [ ] **Discoverability**: New developers can find things intuitively
- [ ] **Testability**: Business logic is separated from infrastructure
- [ ] **Security**: Sensitive data is properly isolated
- [ ] **Configuration**: Settings are organized and environment-specific

## 🚫 Anti-Patterns to Avoid

1. **God Folders**: Avoid having hundreds of files in a single folder
2. **Deep Nesting**: Avoid more than 3-4 levels of folder nesting
3. **Mixed Concerns**: Don't mix different types of files in the same folder
4. **Inconsistent Naming**: Stick to one naming convention throughout
5. **Business Logic in Controllers**: Keep controllers thin
6. **Large Files**: Break up files larger than 500 lines

## 🔄 Refactoring Tips

### When to Restructure

- When folders have more than 20 files
- When finding files becomes difficult
- When onboarding new developers takes too long
- When the same code is duplicated in multiple places

### Gradual Migration

```csharp
// Step 1: Create new structure alongside old
// Step 2: Move files gradually, updating references
// Step 3: Update imports and namespaces
// Step 4: Remove old structure
// Step 5: Update documentation
```

## 🎯 Examples by Application Type

### REST API
```
├── Controllers/
├── Models/
├── Services/
├── Repositories/
├── Middleware/
└── Extensions/
```

### MVC Application
```
├── Controllers/
├── Models/
├── Views/
├── ViewModels/
├── Services/
└── wwwroot/
```

### Blazor Application
```
├── Pages/
├── Shared/
├── Components/
├── Services/
├── Models/
└── wwwroot/
```

---

**Remember**: The best structure is one that your team understands and can maintain consistently. Start simple and refactor as needed!