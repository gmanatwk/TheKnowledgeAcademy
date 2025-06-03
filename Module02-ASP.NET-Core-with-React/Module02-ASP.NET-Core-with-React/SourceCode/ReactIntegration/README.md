# React + ASP.NET Core Integration Demo

This project demonstrates how to integrate a React frontend with an ASP.NET Core Web API backend.

## Prerequisites

- .NET 8.0 SDK or later
- Node.js (version 14.0 or later)
- npm (usually comes with Node.js)

## Project Structure

```
ReactIntegration/
├── Controllers/                 # ASP.NET Core API controllers
│   └── WeatherForecastController.cs
├── ClientApp/                   # React application
│   ├── public/                  # Static files
│   ├── src/                     # React source code
│   │   ├── App.tsx             # Main React component
│   │   ├── App.css             # Styles
│   │   └── index.tsx           # Entry point
│   ├── package.json            # Node dependencies
│   └── tsconfig.json           # TypeScript configuration
├── Properties/
│   └── launchSettings.json     # Launch profiles
├── Program.cs                   # ASP.NET Core entry point
├── ReactIntegration.csproj      # Project file
└── appsettings.json            # Configuration
```

## Features

- **ASP.NET Core Web API**: Provides weather forecast data through RESTful endpoints
- **React Frontend**: TypeScript-based React app that consumes the API
- **SPA Integration**: Seamless integration using Microsoft.AspNetCore.SpaServices.Extensions
- **Hot Module Replacement**: Development server with automatic refresh
- **Swagger UI**: API documentation available at `/swagger`

## Running the Application

### Option 1: Using Visual Studio
1. Open `ReactIntegration.csproj` in Visual Studio
2. Press F5 to run the application
3. The browser will open automatically

### Option 2: Using Command Line
1. Navigate to the project directory:
   ```bash
   cd ReactIntegration
   ```

2. Install npm dependencies (first time only):
   ```bash
   cd ClientApp
   npm install
   cd ..
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to:
   - Application: https://localhost:7000
   - Swagger UI: https://localhost:7000/swagger

## How It Works

1. **Backend (ASP.NET Core)**:
   - Provides a `/api/weatherforecast` endpoint that returns weather data
   - Configured to serve the React app in production
   - Uses SPA proxy in development for hot module replacement

2. **Frontend (React)**:
   - Fetches data from the API using the Fetch API
   - Displays weather information in a responsive grid
   - Includes error handling and loading states
   - Click on any weather card to fetch individual forecast details

3. **Integration**:
   - In development, the React dev server runs on port 3000
   - API requests are proxied through the React dev server
   - In production, the React app is built and served as static files

## API Endpoints

- `GET /api/weatherforecast` - Returns a 5-day weather forecast
- `GET /api/weatherforecast/{id}` - Returns a specific day's forecast (id: 1-5)

## Building for Production

1. Build the project:
   ```bash
   dotnet publish -c Release
   ```

2. The React app will be automatically built and included in the publish output

## Troubleshooting

- **Port already in use**: Change the ports in `launchSettings.json` and `ClientApp/package.json`
- **npm install fails**: Delete `node_modules` folder and `package-lock.json`, then try again
- **CORS issues**: Check that the CORS policy in `Program.cs` matches your development setup

## Key Learning Points

1. **SPA Integration**: How to configure ASP.NET Core to work with a SPA framework
2. **API Communication**: Using Fetch API to communicate between React and ASP.NET Core
3. **Development Workflow**: Hot module replacement for efficient development
4. **Production Build**: How the build process combines both frontend and backend
5. **TypeScript**: Type-safe frontend development with React