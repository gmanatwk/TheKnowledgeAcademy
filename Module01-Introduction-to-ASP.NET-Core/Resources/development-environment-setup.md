# Development Environment Setup Guide

## 🛠️ Complete Setup Guide for ASP.NET Core Development

### Prerequisites Overview

Before starting ASP.NET Core development, you'll need:
- ✅ .NET 8.0 SDK or later
- ✅ A code editor (Visual Studio 2022, VS Code, or Rider)
- ✅ Git for version control
- ✅ A terminal/command prompt
- ✅ (Optional) Docker for containerization

## 📦 Installing .NET SDK

### Windows

1. **Download the installer**:
   - Visit [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
   - Choose .NET 8.0 SDK (or latest)
   - Download the Windows installer (x64 or x86)

2. **Run the installer**:
   - Execute the downloaded .exe file
   - Follow the installation wizard
   - Restart your terminal after installation

3. **Verify installation**:
   ```powershell
   dotnet --version
   dotnet --list-sdks
   ```

### macOS

1. **Using Homebrew** (Recommended):
   ```bash
   # Install Homebrew if not already installed
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   
   # Install .NET SDK
   brew install --cask dotnet-sdk
   ```

2. **Using the installer**:
   - Download the .pkg installer from Microsoft
   - Run the installer
   - Follow the installation steps

3. **Verify installation**:
   ```bash
   dotnet --version
   ```

### Linux (Ubuntu/Debian)

1. **Add Microsoft package repository**:
   ```bash
   # Get Ubuntu version
   declare repo_version=$(if command -v lsb_release &> /dev/null; then lsb_release -r -s; else grep -oP '(?<=^VERSION_ID=).+' /etc/os-release | tr -d '"'; fi)
   
   # Download Microsoft signing key and repository
   wget https://packages.microsoft.com/config/ubuntu/$repo_version/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   
   # Install repository
   sudo dpkg -i packages-microsoft-prod.deb
   
   # Update package database
   sudo apt-get update
   ```

2. **Install .NET SDK**:
   ```bash
   sudo apt-get install -y dotnet-sdk-8.0
   ```

3. **Verify installation**:
   ```bash
   dotnet --version
   ```

## 💻 IDE/Editor Setup

### Visual Studio 2022 (Windows/Mac)

**Pros**: Full-featured IDE, excellent debugging, integrated tools

1. **Download**:
   - Visit [https://visualstudio.microsoft.com/](https://visualstudio.microsoft.com/)
   - Choose Community (free), Professional, or Enterprise

2. **Installation**:
   - Run the installer
   - Select workloads:
     - ✅ ASP.NET and web development
     - ✅ .NET desktop development
     - ✅ Azure development (optional)

3. **Essential Extensions**:
   - Web Essentials
   - Productivity Power Tools
   - CodeMaid

### Visual Studio Code (All Platforms)

**Pros**: Lightweight, cross-platform, highly customizable

1. **Download**:
   - Visit [https://code.visualstudio.com/](https://code.visualstudio.com/)
   - Download for your platform

2. **Essential Extensions**:
   ```bash
   # Install via command palette or terminal
   code --install-extension ms-dotnettools.csharp
   code --install-extension ms-dotnettools.csdevkit
   code --install-extension ms-dotnettools.vscode-dotnet-runtime
   code --install-extension formulahendry.dotnet-test-explorer
   code --install-extension jchannon.csharpextensions
   code --install-extension jorgeserrano.vscode-csharp-snippets
   ```

3. **Recommended Settings** (`.vscode/settings.json`):
   ```json
   {
     "editor.formatOnSave": true,
     "editor.formatOnPaste": true,
     "files.trimTrailingWhitespace": true,
     "files.insertFinalNewline": true,
     "omnisharp.enableRoslynAnalyzers": true,
     "omnisharp.enableEditorConfigSupport": true,
     "dotnet.defaultSolution": "disable"
   }
   ```

### JetBrains Rider (All Platforms)

**Pros**: Powerful refactoring, excellent performance, built-in tools

1. **Download**:
   - Visit [https://www.jetbrains.com/rider/](https://www.jetbrains.com/rider/)
   - 30-day free trial available

2. **Configuration**:
   - Enable .NET Core support
   - Configure code style settings
   - Set up version control integration

## 🔧 Essential Tools

### Git Setup

1. **Install Git**:
   - Windows: Download from [https://git-scm.com/](https://git-scm.com/)
   - macOS: `brew install git`
   - Linux: `sudo apt-get install git`

2. **Configure Git**:
   ```bash
   git config --global user.name "Your Name"
   git config --global user.email "your.email@example.com"
   git config --global init.defaultBranch main
   ```

### .NET Tools

Install useful global tools:

```bash
# Entity Framework Core tools
dotnet tool install --global dotnet-ef

# Code formatting tool
dotnet tool install --global dotnet-format

# Project dependency analyzer
dotnet tool install --global dotnet-outdated-tool

# HTTP REPL for testing APIs
dotnet tool install --global Microsoft.dotnet-httprepl

# Performance analysis
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace

# Verify installations
dotnet tool list --global
```

### Docker (Optional)

1. **Install Docker Desktop**:
   - Download from [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)
   - Follow platform-specific installation

2. **Verify Docker**:
   ```bash
   docker --version
   docker run hello-world
   ```

## 🎯 Environment Verification

Create a verification script to test your setup:

```bash
#!/bin/bash
# save as verify-setup.sh

echo "🔍 Checking Development Environment..."
echo ""

# Check .NET
echo "✓ .NET SDK:"
dotnet --version
echo ""

# Check Git
echo "✓ Git:"
git --version
echo ""

# Check IDE/Editor
echo "✓ Checking for editors..."
command -v code >/dev/null 2>&1 && echo "  - VS Code installed"
command -v rider >/dev/null 2>&1 && echo "  - Rider installed"
echo ""

# Check global tools
echo "✓ .NET Global Tools:"
dotnet tool list --global
echo ""

# Create test project
echo "✓ Creating test project..."
mkdir -p test-env && cd test-env
dotnet new webapp -n TestApp -o . --force
dotnet build
echo ""

echo "✅ Environment setup complete!"
```

## 🚀 Quick Start Commands

```bash
# Create new projects
dotnet new webapp -n MyWebApp
dotnet new mvc -n MyMvcApp
dotnet new webapi -n MyApi
dotnet new blazor -n MyBlazorApp

# Run with hot reload
dotnet watch run

# Add packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Serilog.AspNetCore

# Run tests
dotnet test

# Build for production
dotnet publish -c Release -o ./publish
```

## 📋 Environment Variables

### Development

Create a `.env` file or set environment variables:

```bash
# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_WATCH_SUPPRESS_MSBUILD_INCREMENTALISM="1"

# macOS/Linux
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_WATCH_SUPPRESS_MSBUILD_INCREMENTALISM=1
```

### Launch Settings

Configure `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
  }
}
```

## 🛡️ SSL Certificate Setup

```bash
# Trust the development certificate
dotnet dev-certs https --trust

# Clean and regenerate if having issues
dotnet dev-certs https --clean
dotnet dev-certs https
dotnet dev-certs https --trust
```

## 📚 Recommended Learning Path

1. **Week 1**: Environment setup and basic projects
2. **Week 2**: Understand project structure and middleware
3. **Week 3**: Work with Razor Pages and MVC
4. **Week 4**: Build your first API

## 🔍 Troubleshooting Common Issues

### Issue: "dotnet: command not found"
**Solution**: Add .NET to PATH
```bash
# macOS/Linux
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

# Windows: Add to System PATH via Environment Variables
```

### Issue: HTTPS certificate errors
**Solution**: Clean and reinstall certificates
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Issue: Port already in use
**Solution**: Change port in launchSettings.json or find and kill the process:
```bash
# Find process using port 5000
# Windows
netstat -ano | findstr :5000

# macOS/Linux
lsof -i :5000
```

### Issue: NuGet package restore fails
**Solution**: Clear NuGet cache
```bash
dotnet nuget locals all --clear
```

## 🎉 Next Steps

1. Create your first project following Exercise 1
2. Explore the project structure with Exercise 2
3. Join the .NET community:
   - [.NET Blog](https://devblogs.microsoft.com/dotnet/)
   - [r/dotnet on Reddit](https://www.reddit.com/r/dotnet/)
   - [.NET Foundation](https://dotnetfoundation.org/)

---

**Remember**: A good development environment is the foundation of productive coding. Take time to customize it to your preferences!