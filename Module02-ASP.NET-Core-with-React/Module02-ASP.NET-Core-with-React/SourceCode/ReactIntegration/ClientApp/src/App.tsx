import React, { useState, useEffect } from 'react';
import './App.css';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

function App() {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedForecast, setSelectedForecast] = useState<WeatherForecast | null>(null);

  useEffect(() => {
    fetchWeatherData();
  }, []);

  const fetchWeatherData = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/weatherforecast');
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      setForecasts(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const fetchSingleForecast = async (id: number) => {
    try {
      const response = await fetch(`/api/weatherforecast/${id}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      setSelectedForecast(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>React + ASP.NET Core Integration</h1>
        <p>Weather Forecast Demo</p>
      </header>
      
      <main className="App-main">
        <div className="container">
          <h2>5-Day Weather Forecast</h2>
          
          <div className="actions">
            <button onClick={fetchWeatherData} className="refresh-button">
              Refresh Data
            </button>
          </div>

          {loading && <div className="loading">Loading weather data...</div>}
          
          {error && (
            <div className="error">
              <p>Error: {error}</p>
              <button onClick={fetchWeatherData}>Try Again</button>
            </div>
          )}
          
          {!loading && !error && forecasts.length > 0 && (
            <div className="weather-grid">
              {forecasts.map((forecast, index) => (
                <div 
                  key={index} 
                  className="weather-card"
                  onClick={() => fetchSingleForecast(index + 1)}
                >
                  <h3>{formatDate(forecast.date)}</h3>
                  <div className="temperature">
                    <span className="temp-c">{forecast.temperatureC}°C</span>
                    <span className="temp-f">({forecast.temperatureF}°F)</span>
                  </div>
                  <p className="summary">{forecast.summary}</p>
                </div>
              ))}
            </div>
          )}

          {selectedForecast && (
            <div className="selected-forecast">
              <h3>Selected Forecast Details</h3>
              <div className="forecast-details">
                <p><strong>Date:</strong> {formatDate(selectedForecast.date)}</p>
                <p><strong>Temperature:</strong> {selectedForecast.temperatureC}°C / {selectedForecast.temperatureF}°F</p>
                <p><strong>Conditions:</strong> {selectedForecast.summary}</p>
              </div>
              <button onClick={() => setSelectedForecast(null)}>Close</button>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

export default App;