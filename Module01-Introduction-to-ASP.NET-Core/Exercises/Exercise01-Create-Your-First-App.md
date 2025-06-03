# Exercise 1: Create Your First ASP.NET Core Application

## 🎯 Objective
Create and run your first ASP.NET Core web application, explore its structure, and make basic modifications.

## ⏱️ Estimated Time
20-30 minutes

## 📋 Prerequisites
- .NET 8.0 SDK installed
- Visual Studio 2022 or VS Code
- Basic knowledge of C#

## 📝 Instructions

### Part 1: Create a New Project (5 minutes)

1. Open your terminal or command prompt
2. Navigate to your desired project directory
3. Create a new ASP.NET Core Web App:
   ```bash
   dotnet new webapp -n MyFirstWebApp
   cd MyFirstWebApp
   ```

### Part 2: Explore the Project Structure (5 minutes)

Open the project in your IDE and examine these key files:

1. **Program.cs** - Application entry point and configuration
2. **appsettings.json** - Application configuration
3. **Pages folder** - Contains Razor Pages
   - Index.cshtml - Home page view
   - Index.cshtml.cs - Home page code-behind
4. **wwwroot folder** - Static files (CSS, JS, images)

### Part 3: Run the Application (5 minutes)

1. In the terminal, run:
   ```bash
   dotnet run
   ```

2. Open your browser and navigate to:
   - https://localhost:5001 (or the URL shown in the terminal)

3. You should see the default ASP.NET Core welcome page

4. Try the hot reload feature:
   ```bash
   dotnet watch run
   ```

### Part 4: Modify the Home Page (10 minutes)

1. Open `Pages/Index.cshtml`

2. Replace the content with:
   ```html
   @page
   @model IndexModel
   @{
       ViewData["Title"] = "My First ASP.NET Core App";
   }

   <div class="text-center">
       <h1 class="display-4">Welcome to My First App!</h1>
       <p>Hello, ASP.NET Core! The current time is: @DateTime.Now.ToString("F")</p>
       
       <div class="alert alert-info mt-4">
           <h4>Quick Facts:</h4>
           <ul class="text-start">
               <li>This app is running on @System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription</li>
               <li>Operating System: @System.Runtime.InteropServices.RuntimeInformation.OSDescription</li>
               <li>Your name is: @Model.UserName</li>
           </ul>
       </div>
   </div>
   ```

3. Open `Pages/Index.cshtml.cs` and add a property:
   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.AspNetCore.Mvc.RazorPages;

   namespace MyFirstWebApp.Pages
   {
       public class IndexModel : PageModel
       {
           public string UserName { get; set; } = "ASP.NET Core Developer";

           public void OnGet()
           {
               // This method runs when the page is requested
           }
       }
   }
   ```

4. Save the files and refresh your browser (if using `dotnet watch`, it should auto-refresh)

### Part 5: Add Custom Styling (5 minutes)

1. Open `wwwroot/css/site.css`

2. Add the following custom styles at the end:
   ```css
   /* Custom styles for our first app */
   .display-4 {
       color: #5c2d91;
       font-weight: bold;
   }

   .alert-info {
       background-color: #e7f3ff;
       border: 2px solid #004e8c;
   }

   .alert-info h4 {
       color: #004e8c;
   }
   ```

3. Save and refresh your browser to see the changes

## ✅ Success Criteria

You have successfully completed this exercise if:
- [ ] Created a new ASP.NET Core web application
- [ ] Successfully ran the application
- [ ] Modified the home page content
- [ ] Added custom styling
- [ ] The application displays the current time and system information

## 🚀 Bonus Challenges

1. **Add a New Page**:
   - Create a new Razor Page called "About"
   - Add a link to it from the home page
   - Display information about yourself

2. **Add Interactivity**:
   - Add a button that updates the displayed time without refreshing the page
   - Hint: Use JavaScript or explore Blazor components

3. **Configuration**:
   - Add a custom setting to `appsettings.json`
   - Read and display this setting on your page

## 🤔 Reflection Questions

1. What files were created by the `dotnet new webapp` command?
2. What is the purpose of the `@page` directive in Razor Pages?
3. How does the code-behind file (`.cshtml.cs`) relate to the view file (`.cshtml`)?
4. What is the wwwroot folder used for?

## 📚 Resources
- [ASP.NET Core Razor Pages](https://docs.microsoft.com/aspnet/core/razor-pages/)
- [Static files in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/static-files)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)

## 🆘 Troubleshooting

**Issue**: Port already in use error
**Solution**: Change the port in `Properties/launchSettings.json` or stop the other application

**Issue**: HTTPS certificate error
**Solution**: Run `dotnet dev-certs https --trust`

**Issue**: Application won't start
**Solution**: Check that you're in the correct directory and all packages are restored (`dotnet restore`)

---

**Next Exercise**: [Exercise 2 - Explore Project Structure →](Exercise02-Explore-Project-Structure.md)