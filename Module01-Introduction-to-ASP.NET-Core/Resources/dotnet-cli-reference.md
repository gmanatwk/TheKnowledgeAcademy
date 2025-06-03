# .NET CLI Command Reference

## 🚀 Essential .NET CLI Commands for ASP.NET Core Development

### Quick Reference Table

| Task | Command | Example |
|------|---------|---------|
| Create new app | `dotnet new` | `dotnet new webapp -n MyApp` |
| Run app | `dotnet run` | `dotnet run --project ./MyApp` |
| Build app | `dotnet build` | `dotnet build -c Release` |
| Run tests | `dotnet test` | `dotnet test --logger "console"` |
| Add package | `dotnet add package` | `dotnet add package Serilog` |
| Publish app | `dotnet publish` | `dotnet publish -c Release -o ./publish` |

## 📦 Project Creation Commands

### Create New Projects

```bash
# Web Application (Razor Pages)
dotnet new webapp -n MyWebApp

# MVC Application
dotnet new mvc -n MyMvcApp

# Web API
dotnet new webapi -n MyApi

# Minimal API
dotnet new web -n MyMinimalApi

# Blazor Server App
dotnet new blazorserver -n MyBlazorApp

# Blazor WebAssembly App
dotnet new blazorwasm -n MyWasmApp

# Class Library
dotnet new classlib -n MyLibrary

# Console Application
dotnet new console -n MyConsoleApp

# gRPC Service
dotnet new grpc -n MyGrpcService
```

### Project Templates with Options

```bash
# Create with specific framework
dotnet new webapp -n MyApp -f net8.0

# Skip restore
dotnet new webapp -n MyApp --no-restore

# Use specific language
dotnet new console -lang F# -n MyFSharpApp

# Create in current directory
dotnet new webapp -o .

# Force overwrite existing files
dotnet new webapp -n MyApp --force

# List available templates
dotnet new list

# Search for templates
dotnet new search spa

# Install new template
dotnet new install Microsoft.AspNetCore.SpaTemplates
```

## 🏃‍♂️ Running Applications

### Basic Run Commands

```bash
# Run project in current directory
dotnet run

# Run specific project
dotnet run --project ./src/MyApp/MyApp.csproj

# Run with specific configuration
dotnet run -c Release

# Run with arguments
dotnet run -- arg1 arg2

# Run with environment variables
ASPNETCORE_ENVIRONMENT=Production dotnet run

# Run with specific launch profile
dotnet run --launch-profile "Production"
```

### Watch Mode (Hot Reload)

```bash
# Run with file watcher
dotnet watch run

# Watch with specific project
dotnet watch run --project ./MyApp

# Watch and run tests
dotnet watch test

# Suppress build optimizations for better hot reload
DOTNET_WATCH_SUPPRESS_MSBUILD_INCREMENTALISM=1 dotnet watch run

# Non-interactive mode (CI/CD)
dotnet watch --non-interactive run
```

## 🔨 Building Projects

### Build Commands

```bash
# Build current project
dotnet build

# Build in Release mode
dotnet build -c Release

# Build specific project
dotnet build ./src/MyApp/MyApp.csproj

# Build entire solution
dotnet build MySolution.sln

# Build without restore
dotnet build --no-restore

# Build with specific runtime
dotnet build -r win-x64

# Build self-contained
dotnet build -r win-x64 --self-contained

# Clean build output
dotnet clean

# Build with detailed verbosity
dotnet build -v detailed
```

## 📚 Package Management

### NuGet Package Commands

```bash
# Add package to project
dotnet add package Newtonsoft.Json

# Add specific version
dotnet add package Serilog --version 2.12.0

# Add prerelease package
dotnet add package Microsoft.AspNetCore.Mvc --prerelease

# Add package with specific source
dotnet add package MyPackage --source https://api.nuget.org/v3/index.json

# Remove package
dotnet remove package Newtonsoft.Json

# List packages
dotnet list package

# List outdated packages
dotnet list package --outdated

# List vulnerable packages
dotnet list package --vulnerable

# Update package
dotnet add package Newtonsoft.Json --version 13.0.2
```

### Package Restoration

```bash
# Restore packages
dotnet restore

# Restore with specific source
dotnet restore --source https://api.nuget.org/v3/index.json

# Clear NuGet cache
dotnet nuget locals all --clear

# Restore for specific runtime
dotnet restore -r linux-x64
```

## 🧪 Testing Commands

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test -v normal

# Run specific test project
dotnet test ./tests/MyApp.Tests/MyApp.Tests.csproj

# Filter tests
dotnet test --filter "FullyQualifiedName~UnitTests"
dotnet test --filter "Category=Integration"

# Generate code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in parallel
dotnet test --parallel

# Set timeout
dotnet test --blame-hang-timeout 60s

# Generate test results file
dotnet test --logger:"trx;LogFileName=results.trx"
```

## 📤 Publishing Applications

### Publish Commands

```bash
# Basic publish
dotnet publish

# Publish to specific folder
dotnet publish -c Release -o ./publish

# Publish for specific runtime
dotnet publish -c Release -r win-x64

# Self-contained publish
dotnet publish -c Release -r win-x64 --self-contained true

# Framework-dependent publish
dotnet publish -c Release --self-contained false

# Single file publish
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true

# Ready to run (R2R)
dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true

# Trim unused code
dotnet publish -c Release -r win-x64 -p:PublishTrimmed=true

# AOT compilation (Native)
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

## 🛠️ Global Tools

### Managing Global Tools

```bash
# Install global tool
dotnet tool install --global dotnet-ef

# Install specific version
dotnet tool install --global dotnet-format --version 5.0.0

# Update global tool
dotnet tool update --global dotnet-ef

# Uninstall global tool
dotnet tool uninstall --global dotnet-ef

# List global tools
dotnet tool list --global

# Install as local tool
dotnet new tool-manifest
dotnet tool install dotnet-ef

# Restore local tools
dotnet tool restore

# Run local tool
dotnet tool run dotnet-ef
```

### Useful Global Tools

```bash
# Entity Framework Core
dotnet tool install --global dotnet-ef

# Code formatting
dotnet tool install --global dotnet-format

# API development
dotnet tool install --global Microsoft.dotnet-httprepl

# Performance analysis
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-dump

# Security scanning
dotnet tool install --global security-scan

# Outdated dependencies
dotnet tool install --global dotnet-outdated-tool
```

## 🔧 Solution Management

### Solution Commands

```bash
# Create new solution
dotnet new sln -n MySolution

# Add project to solution
dotnet sln add ./src/MyApp/MyApp.csproj

# Add multiple projects
dotnet sln add ./src/**/*.csproj

# Remove project from solution
dotnet sln remove ./src/MyApp/MyApp.csproj

# List projects in solution
dotnet sln list

# Build solution
dotnet build MySolution.sln
```

## 🔍 Project Information

### Information Commands

```bash
# Show .NET info
dotnet --info

# List installed SDKs
dotnet --list-sdks

# List installed runtimes
dotnet --list-runtimes

# Show project dependencies
dotnet list reference

# Show package dependencies
dotnet list package

# Show available templates
dotnet new list
```

## 🎯 Entity Framework Core

### EF Core Commands

```bash
# Install EF tools
dotnet tool install --global dotnet-ef

# Add EF package to project
dotnet add package Microsoft.EntityFrameworkCore.Design

# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script

# Drop database
dotnet ef database drop

# List migrations
dotnet ef migrations list

# Database context info
dotnet ef dbcontext info

# Scaffold from database
dotnet ef dbcontext scaffold "Server=.;Database=MyDb;Trusted_Connection=true;" Microsoft.EntityFrameworkCore.SqlServer
```

## 🎨 Code Quality Tools

### Format and Analyze

```bash
# Format code
dotnet format

# Format and verify
dotnet format --verify-no-changes

# Analyze code
dotnet build -warnaserror

# Run code analysis
dotnet build /p:RunCodeAnalysis=true

# Style checking
dotnet format style

# Whitespace formatting
dotnet format whitespace
```

## 🐛 Debugging Commands

### Debug-Specific Commands

```bash
# Build with debug symbols
dotnet build -c Debug

# Run with debugging
dotnet run -c Debug

# Attach debugger on launch
dotnet run --launch-profile "Debug"

# Generate dumps
dotnet dump collect -p <PID>

# Analyze dumps
dotnet dump analyze ./core_dump
```

## 💡 Productivity Tips

### Aliases and Shortcuts

```bash
# Create aliases (bash/zsh)
alias dn='dotnet new'
alias dr='dotnet run'
alias db='dotnet build'
alias dt='dotnet test'
alias dw='dotnet watch'

# PowerShell aliases
Set-Alias -Name dn -Value dotnet new
Set-Alias -Name dr -Value dotnet run
```

### Common Workflows

```bash
# Quick API setup
dotnet new webapi -n MyApi && cd MyApi && dotnet add package Swashbuckle.AspNetCore && dotnet run

# Test-driven development
dotnet watch test --project ./tests/MyApp.Tests

# Full rebuild
dotnet clean && dotnet restore && dotnet build

# Package update routine
dotnet restore && dotnet list package --outdated && dotnet test
```

## ⚡ Performance Commands

### Optimization and Analysis

```bash
# Measure startup time
dotnet run -c Release --performance

# Generate performance trace
dotnet trace collect -p <PID>

# CPU sampling
dotnet counters monitor -p <PID>

# Memory dump
dotnet dump collect -p <PID>
```

---

**Pro Tip**: Create a `.dotnet-tools.json` file in your repository to ensure all team members use the same tool versions:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-ef": {
      "version": "8.0.0",
      "commands": ["dotnet-ef"]
    }
  }
}
```

Then run `dotnet tool restore` to install all tools.