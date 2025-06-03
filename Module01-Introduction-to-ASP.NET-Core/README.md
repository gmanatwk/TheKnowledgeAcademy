# Module 1: Introduction to ASP.NET Core

## 🎯 Learning Objectives

By the end of this module, students will be able to:
- ✅ Understand the fundamentals and benefits of ASP.NET Core
- ✅ Explain the ASP.NET Core application lifecycle
- ✅ Set up a complete development environment
- ✅ Create their first ASP.NET Core application
- ✅ Understand the project structure and key components

## 📖 Module Overview

**Duration**: 2 hours  
**Skill Level**: Beginner  
**Prerequisites**: Basic C# knowledge

### Topics Covered:
1. **Overview and Benefits of ASP.NET Core**
2. **ASP.NET Core Application Lifecycle**
3. **Setting Up Development Environment**
4. **Creating Your First Application**
5. **Understanding Project Structure**

## 🏗️ Project Structure

```
Module01-Introduction-to-ASP.NET-Core/
├── README.md (this file)
├── SourceCode/
│   ├── BasicWebApp/          # Simple ASP.NET Core Web App
│   ├── MinimalAPI/           # Minimal API example
│   └── Examples/             # Code demonstrations
├── Exercises/
│   ├── Exercise01/           # Create first app
│   ├── Exercise02/           # Explore project structure
│   └── Solutions/            # Exercise solutions
└── Resources/
    ├── slides.md             # Presentation content
    ├── cheatsheet.md         # Quick reference
    └── additional-reading.md  # Extra resources
```

## 🚀 Getting Started

### 1. Prerequisites Check
Before starting, ensure you have:
- ✅ .NET 8.0 SDK installed
- ✅ Visual Studio 2022 or VS Code
- ✅ Basic C# programming knowledge

### 2. Verify Installation
Open a terminal and run:
```bash
dotnet --version
```
You should see version 8.0.x or later.

### 3. Explore the Examples
1. Navigate to `SourceCode/BasicWebApp`
2. Run the application:
   ```bash
   cd SourceCode/BasicWebApp
   dotnet run
   ```
3. Open your browser to `https://localhost:5001`

## 📚 Key Concepts

### What is ASP.NET Core?
ASP.NET Core is a modern, open-source, and cross-platform framework for building:
- 🌐 Web applications
- 🔌 Web APIs
- 🎯 Real-time applications
- ☁️ Cloud-native applications

### Key Benefits:
1. **Cross-Platform Compatibility** - Runs on Windows, Linux, and macOS
2. **Performance and Scalability** - High-performance with efficient resource usage
3. **Unified Development Model** - MVC, Razor Pages, and APIs in one framework
4. **Cloud-Ready Architecture** - Built for cloud deployment
5. **Open Source and Community Driven** - Active community and contributions
6. **Improved Security Features** - Built-in security mechanisms
7. **Dependency Injection by Default** - Promotes clean, testable code

### Application Lifecycle:
1. **Application Startup** - Host initialization and service configuration
2. **Request Pipeline Configuration** - Middleware setup
3. **Request Processing** - Handling incoming HTTP requests
4. **Response Generation** - Creating and sending responses
5. **Application Shutdown** - Graceful cleanup and resource disposal

## 🛠️ Hands-On Activities

### Activity 1: Environment Setup (15 minutes)
1. Install .NET 8.0 SDK
2. Install Visual Studio 2022 or VS Code
3. Verify installation with `dotnet --version`

### Activity 2: Create First Application (20 minutes)
1. Create a new ASP.NET Core Web App
2. Run the application
3. Explore the default project structure

### Activity 3: Modify the Application (25 minutes)
1. Add a new page to the application
2. Modify the home page content
3. Add custom CSS styling

## 📝 Quick Commands Reference

### Creating New Projects:
```bash
# Create a Web App with Razor Pages
dotnet new webapp -n MyWebApp

# Create an MVC application
dotnet new mvc -n MyMvcApp

# Create a Web API
dotnet new webapi -n MyApi

# Create a minimal API
dotnet new web -n MyMinimalApi
```

### Running Applications:
```bash
# Run the application
dotnet run

# Run with hot reload
dotnet watch run

# Build the application
dotnet build

# Restore packages
dotnet restore
```

## 💡 Best Practices

1. **Follow Naming Conventions**
   - Use PascalCase for public members
   - Use camelCase for private fields
   - Use descriptive names

2. **Project Organization**
   - Keep related files together
   - Use folders to group functionality
   - Follow the standard ASP.NET Core structure

3. **Configuration Management**
   - Use appsettings.json for configuration
   - Separate settings by environment
   - Use the Options pattern for strongly-typed configuration

## ❓ Quiz Questions

1. What are the key benefits of ASP.NET Core over traditional ASP.NET?
2. Name the five stages of the ASP.NET Core application lifecycle.
3. Which command creates a new ASP.NET Core Web API project?
4. What is the default web server used by ASP.NET Core?

## 🎯 Exercises

### Exercise 1: Create Your First App
**Objective**: Create and run a basic ASP.NET Core application
**Time**: 20 minutes
**Instructions**: See `Exercises/Exercise01/README.md`

### Exercise 2: Explore Project Structure
**Objective**: Understand the default project structure
**Time**: 15 minutes
**Instructions**: See `Exercises/Exercise02/README.md`

## 📖 Additional Resources

- 📚 [Official ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- 🎥 [ASP.NET Core Video Series](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oW8nviYduHq7bmKode-p8Wy)
- 📝 [ASP.NET Core Best Practices](https://docs.microsoft.com/aspnet/core/fundamentals/best-practices)
- 🛠️ [ASP.NET Core Tools](https://docs.microsoft.com/aspnet/core/fundamentals/tools)

## 🔗 Next Module

Ready for the next challenge? Move on to:
**[Module 2: Implementing ASP.NET Core with React.js](../Module02-ASP.NET-Core-with-React/README.md)**

## 🆘 Need Help?

- 💬 Ask questions during the live session
- 📚 Check the Resources folder for additional materials
- 🔍 Review the example code in SourceCode folder

---

**Remember**: Practice makes perfect! Don't just read the code - run it, modify it, and experiment with it.

**Happy Coding! 🚀**