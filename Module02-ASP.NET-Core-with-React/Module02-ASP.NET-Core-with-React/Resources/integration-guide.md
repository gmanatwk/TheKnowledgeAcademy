# React + ASP.NET Core Integration Guide

## 🏗️ Step-by-Step Integration Guide

### Overview

This guide provides detailed instructions for integrating React.js with ASP.NET Core, covering multiple integration patterns and best practices.

## Integration Patterns

### 1. SPA with Separate API (Recommended for Large Apps)

```
┌─────────────────┐     ┌──────────────────┐
│   React SPA     │────▶│ ASP.NET Core API │
│  (Port 3000)    │     │   (Port 5000)    │
└─────────────────┘     └──────────────────┘
```

**Pros:**
- Complete separation of concerns
- Independent deployment
- Easy to scale
- Different teams can work independently

**Cons:**
- CORS configuration required
- More complex deployment
- Separate hosting needed

### 2. Integrated SPA (Recommended for Medium Apps)

```
┌────────────────────────────┐
│    ASP.NET Core Host       │
│  ┌──────────────────────┐  │
│  │   React SPA Assets   │  │
│  └──────────────────────┘  │
│  ┌──────────────────────┐  │
│  │      Web API         │  │
│  └──────────────────────┘  │
└────────────────────────────┘
```

**Pros:**
- Single deployment unit
- No CORS issues
- Simplified hosting
- Shared authentication

**Cons:**
- Coupled deployment
- Larger build size
- Less flexibility

## Setting Up Integrated SPA

### Step 1: Create ASP.NET Core Project

```bash
# Create new Web API project
dotnet new webapi -n MyReactApp
cd MyReactApp

# Add SPA services package
dotnet add package Microsoft.AspNetCore.SpaServices.Extensions
```

### Step 2: Configure ASP.NET Core

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SPA static files
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Only in production - serve SPA static files
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// SPA middleware should be last
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        // Use React development server
        spa.UseReactDevelopmentServer(npmScript: "start");
        
        // Alternative: Use proxy to existing React dev server
        // spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    }
});

app.Run();
```

### Step 3: Create React App

```bash
# In project root
npx create-react-app clientapp --template typescript
cd clientapp

# Install additional dependencies
npm install axios react-router-dom @types/react-router-dom
```

### Step 4: Configure Proxy for Development

**ClientApp/package.json:**
```json
{
  "name": "clientapp",
  "version": "0.1.0",
  "private": true,
  "proxy": "https://localhost:7000",
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "axios": "^1.3.0",
    "react-router-dom": "^6.8.0"
  },
  "scripts": {
    "start": "react-scripts start",
    "build": "react-scripts build",
    "test": "react-scripts test",
    "eject": "react-scripts eject"
  }
}
```

### Step 5: Update .csproj File

**MyReactApp.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <TypeScriptCompileBlocked>true</TypeScriptCompileBlocked>
    <TypeScriptToolsVersion>Latest</TypeScriptToolsVersion>
    <IsPackable>false</IsPackable>
    <SpaRoot>ClientApp\</SpaRoot>
    <DefaultItemExcludes>$(DefaultItemExcludes);$(SpaRoot)node_modules\**</DefaultItemExcludes>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.SpaServices.Extensions" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- Don't publish the SPA source files, but do show them in the project files list -->
    <Content Remove="$(SpaRoot)**" />
    <None Remove="$(SpaRoot)**" />
    <None Include="$(SpaRoot)**" Exclude="$(SpaRoot)node_modules\**" />
  </ItemGroup>

  <Target Name="DebugEnsureNodeEnv" BeforeTargets="Build" Condition=" '$(Configuration)' == 'Debug' And !Exists('$(SpaRoot)node_modules') ">
    <!-- Ensure Node.js is installed -->
    <Exec Command="node --version" ContinueOnError="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
    <Error Condition="'$(ErrorCode)' != '0'" Text="Node.js is required to build and run this project." />
    <Message Importance="high" Text="Restoring dependencies using 'npm'. This may take several minutes..." />
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm install" />
  </Target>

  <Target Name="PublishRunWebpack" AfterTargets="ComputeFilesToPublish">
    <!-- Build React app -->
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm install" />
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm run build" />

    <!-- Include the built files in publish output -->
    <ItemGroup>
      <DistFiles Include="$(SpaRoot)build\**" />
      <ResolvedFileToPublish Include="@(DistFiles->'%(FullPath)')" Exclude="@(ResolvedFileToPublish)">
        <RelativePath>%(DistFiles.Identity)</RelativePath>
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
        <ExcludeFromSingleFile>true</ExcludeFromSingleFile>
      </ResolvedFileToPublish>
    </ItemGroup>
  </Target>
</Project>
```

## API Communication

### Setting Up API Service

**ClientApp/src/services/api.ts:**
```typescript
import axios, { AxiosInstance } from 'axios';

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: '/api',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Handle unauthorized
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Generic methods
  get<T>(url: string) {
    return this.client.get<T>(url);
  }

  post<T>(url: string, data: any) {
    return this.client.post<T>(url, data);
  }

  put<T>(url: string, data: any) {
    return this.client.put<T>(url, data);
  }

  delete<T>(url: string) {
    return this.client.delete<T>(url);
  }
}

export default new ApiService();
```

### Using the API Service

**ClientApp/src/services/userService.ts:**
```typescript
import api from './api';

export interface User {
  id: number;
  name: string;
  email: string;
}

export const userService = {
  getAll: () => api.get<User[]>('/users'),
  getById: (id: number) => api.get<User>(`/users/${id}`),
  create: (user: Omit<User, 'id'>) => api.post<User>('/users', user),
  update: (id: number, user: Partial<User>) => api.put<User>(`/users/${id}`, user),
  delete: (id: number) => api.delete<void>(`/users/${id}`),
};
```

## Authentication Integration

### JWT Authentication Setup

**ASP.NET Core Configuration:**
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add authorization
builder.Services.AddAuthorization();

// In the pipeline
app.UseAuthentication();
app.UseAuthorization();
```

**React Authentication Hook:**
```typescript
// ClientApp/src/hooks/useAuth.ts
import { useState, useContext, createContext, useEffect } from 'react';
import api from '../services/api';

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in
    const token = localStorage.getItem('token');
    if (token) {
      // Validate token with API
      api.get<User>('/auth/me')
        .then(response => setUser(response.data))
        .catch(() => localStorage.removeItem('token'))
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, []);

  const login = async (email: string, password: string) => {
    const response = await api.post<{ token: string; user: User }>('/auth/login', {
      email,
      password,
    });
    
    localStorage.setItem('token', response.data.token);
    setUser(response.data.user);
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
```

## Environment Configuration

### Development vs Production

**ASP.NET Core appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyReactApp;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "ThisIsMySecretKeyForJwtToken",
    "Issuer": "MyReactApp",
    "Audience": "MyReactAppUsers"
  }
}
```

**React Environment Variables:**
```bash
# ClientApp/.env.development
REACT_APP_API_URL=https://localhost:7000/api
REACT_APP_ENVIRONMENT=development

# ClientApp/.env.production
REACT_APP_API_URL=/api
REACT_APP_ENVIRONMENT=production
```

## Deployment Strategies

### 1. Single Deployment (IIS/Azure App Service)

```bash
# Build for production
dotnet publish -c Release -o ./publish

# The publish folder contains both API and React app
```

### 2. Separate Deployment

**React (Static Hosting):**
```bash
cd ClientApp
npm run build
# Deploy build folder to CDN/Static hosting
```

**API (App Service/Container):**
```bash
dotnet publish -c Release -o ./publish
# Deploy without ClientApp folder
```

### 3. Docker Deployment

**Dockerfile:**
```dockerfile
# Build stage for React
FROM node:18-alpine as react-build
WORKDIR /app
COPY ClientApp/package*.json ./
RUN npm ci
COPY ClientApp/ ./
RUN npm run build

# Build stage for .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MyReactApp.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=react-build /app/build ./ClientApp/build
EXPOSE 80
ENTRYPOINT ["dotnet", "MyReactApp.dll"]
```

## Performance Optimization

### 1. Enable Response Compression

```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Use it before static files
app.UseResponseCompression();
```

### 2. Enable Response Caching

```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// In controllers
[HttpGet]
[ResponseCache(Duration = 300)] // Cache for 5 minutes
public IActionResult GetData()
{
    return Ok(data);
}
```

### 3. React Build Optimization

**ClientApp/package.json:**
```json
{
  "scripts": {
    "build": "react-scripts build",
    "build:analyze": "source-map-explorer 'build/static/js/*.js'"
  }
}
```

## Troubleshooting Common Issues

### Issue: CORS Errors in Development

**Solution:** Use the proxy configuration in package.json or configure CORS:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}
```

### Issue: React Routes Return 404

**Solution:** Configure fallback routing:

```csharp
app.MapFallbackToFile("index.html");
```

### Issue: Hot Reload Not Working

**Solution:** Ensure WebSocket connection is not blocked:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseWebSockets();
}
```

## Security Best Practices

1. **Content Security Policy:**
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline';");
    await next();
});
```

2. **API Rate Limiting:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

3. **Secure Headers:**
```csharp
app.UseSecurityHeaders(policies =>
    policies
        .AddFrameOptionsDeny()
        .AddXssProtectionBlock()
        .AddContentTypeOptionsNoSniff()
        .RemoveServerHeader()
);
```

---

**Remember**: The integration pattern you choose depends on your specific requirements for scalability, deployment complexity, and team structure. Start simple and evolve as needed!