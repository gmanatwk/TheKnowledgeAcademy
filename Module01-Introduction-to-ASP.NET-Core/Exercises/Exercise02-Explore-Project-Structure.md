# Exercise 2: Explore ASP.NET Core Project Structure

## 🎯 Objective
Understand the default project structure of an ASP.NET Core application and the purpose of each component.

## ⏱️ Estimated Time
15-20 minutes

## 📋 Prerequisites
- Completed Exercise 1
- Have the MyFirstWebApp project created

## 📝 Instructions

### Part 1: Analyze Program.cs (5 minutes)

1. Open `Program.cs` in your MyFirstWebApp project

2. Identify and understand these key sections:
   ```csharp
   var builder = WebApplication.CreateBuilder(args);  // Creates the web host builder
   
   // Add services to the container
   builder.Services.AddRazorPages();  // Registers Razor Pages services
   
   var app = builder.Build();  // Builds the web application
   
   // Configure the HTTP request pipeline
   if (!app.Environment.IsDevelopment())
   {
       app.UseExceptionHandler("/Error");
       app.UseHsts();
   }
   
   app.UseHttpsRedirection();  // Redirects HTTP to HTTPS
   app.UseStaticFiles();       // Enables serving static files
   app.UseRouting();           // Enables routing
   app.UseAuthorization();     // Enables authorization
   app.MapRazorPages();        // Maps Razor Pages endpoints
   
   app.Run();  // Runs the application
   ```

3. **Task**: Add a custom middleware to log each request:
   ```csharp
   // Add this before app.UseHttpsRedirection()
   app.Use(async (context, next) =>
   {
       Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
       await next();
   });
   ```

### Part 2: Configuration Files (5 minutes)

1. **appsettings.json** - Open and examine:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

2. **Task**: Add custom configuration:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*",
     "MyAppSettings": {
       "AppName": "My First ASP.NET Core App",
       "Version": "1.0.0",
       "Author": "Your Name"
     }
   }
   ```

3. **appsettings.Development.json** - Environment-specific settings
   - Notice how this overrides settings for development

### Part 3: Project File Structure (5 minutes)

Examine the project structure and fill in the purpose of each folder/file:

```
MyFirstWebApp/
├── wwwroot/                 # [Your answer: What goes here?]
│   ├── css/                 # [Your answer: Purpose?]
│   ├── js/                  # [Your answer: Purpose?]
│   └── lib/                 # [Your answer: Purpose?]
├── Pages/                   # [Your answer: What's stored here?]
│   ├── Shared/              # [Your answer: Purpose?]
│   │   ├── _Layout.cshtml   # [Your answer: What is this?]
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Index.cshtml         # [Your answer: What type of file?]
│   ├── Index.cshtml.cs      # [Your answer: Relationship to .cshtml?]
│   ├── Privacy.cshtml
│   ├── Privacy.cshtml.cs
│   ├── Error.cshtml
│   ├── Error.cshtml.cs
│   ├── _ViewImports.cshtml  # [Your answer: Purpose?]
│   └── _ViewStart.cshtml    # [Your answer: When does this run?]
├── Properties/
│   └── launchSettings.json  # [Your answer: What does this configure?]
├── appsettings.json         # [Your answer: Purpose?]
├── appsettings.Development.json
├── Program.cs               # [Your answer: Purpose?]
└── MyFirstWebApp.csproj     # [Your answer: What is this?]
```

### Part 4: Explore Key Files (5 minutes)

1. **Open `Pages/Shared/_Layout.cshtml`**
   - This is the layout template for all pages
   - Find where `@RenderBody()` is called
   - Identify the navigation menu
   - Notice the script and CSS references

2. **Open `Pages/_ViewImports.cshtml`**
   ```razor
   @using MyFirstWebApp
   @namespace MyFirstWebApp.Pages
   @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
   ```
   - These directives apply to all Razor pages

3. **Open `MyFirstWebApp.csproj`**
   - This is the project file
   - Note the target framework
   - See any package references

### Part 5: Create a Documentation File (5 minutes)

Create a new file `PROJECT_STRUCTURE.md` in your project root with your findings:

```markdown
# MyFirstWebApp Project Structure Documentation

## Core Files

### Program.cs
Purpose: [Your description]
Key responsibilities:
- [List them]

### appsettings.json
Purpose: [Your description]
Usage: [How to access these settings]

## Folders

### wwwroot/
Purpose: [Your description]
Contents: [What goes here]

### Pages/
Purpose: [Your description]
File types: [What file types and their relationships]

### Pages/Shared/
Purpose: [Your description]
Key files: [List important files]

## Middleware Pipeline Order
1. [List the middleware in order]
2. [...]

## My Observations
[Write 2-3 observations about the project structure]
```

## ✅ Success Criteria

- [ ] Added custom middleware to Program.cs
- [ ] Added custom configuration to appsettings.json
- [ ] Completed the project structure analysis
- [ ] Created PROJECT_STRUCTURE.md documentation
- [ ] Application still runs correctly

## 🚀 Bonus Challenges

1. **Environment-Specific Configuration**:
   - Add different settings in appsettings.Development.json
   - Read and display the current environment on a page

2. **Custom Static Files**:
   - Add a new folder under wwwroot (e.g., "images")
   - Add an image and reference it in a page

3. **Explore Launch Settings**:
   - Modify the launch URL in launchSettings.json
   - Change the application ports

## 🤔 Reflection Questions

1. Why is wwwroot special compared to other folders?
2. What's the difference between `Pages/_ViewStart.cshtml` and `Pages/_ViewImports.cshtml`?
3. How does ASP.NET Core know which layout to use for a page?
4. What happens if you remove `app.UseStaticFiles()` from Program.cs?

## 📚 Additional Tasks

1. **Middleware Experiment**:
   - Comment out different middleware in Program.cs
   - Observe what functionality breaks
   - Document your findings

2. **Configuration Hierarchy**:
   - Add the same setting to both appsettings.json and appsettings.Development.json with different values
   - See which one is used when running in Development mode

## 🆘 Common Issues

**Issue**: Changes to static files not reflecting
**Solution**: Hard refresh browser (Ctrl+F5) or clear cache

**Issue**: Custom middleware not executing
**Solution**: Ensure it's added in the correct order in Program.cs

**Issue**: Configuration not reading correctly
**Solution**: Check JSON syntax and ensure proper nesting

---

**Next Exercise**: [Exercise 3 - Configuration and Middleware →](Exercise03-Configuration-and-Middleware.md)