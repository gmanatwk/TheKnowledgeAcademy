# React with ASP.NET Core Cheat Sheet

## 🚀 Quick Reference for React Development

### React Fundamentals

#### Component Types

```typescript
// Functional Component (Recommended)
const MyComponent: React.FC<Props> = ({ name, age }) => {
  return <div>Hello {name}, you are {age} years old</div>;
};

// Component with Children
const Container: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return <div className="container">{children}</div>;
};

// Class Component (Legacy)
class LegacyComponent extends React.Component<Props, State> {
  render() {
    return <div>{this.props.name}</div>;
  }
}
```

#### React Hooks Reference

| Hook | Purpose | Example |
|------|---------|---------|
| `useState` | Local state | `const [count, setCount] = useState(0)` |
| `useEffect` | Side effects | `useEffect(() => { /* effect */ }, [deps])` |
| `useContext` | Consume context | `const value = useContext(MyContext)` |
| `useReducer` | Complex state | `const [state, dispatch] = useReducer(reducer, init)` |
| `useCallback` | Memoize callbacks | `const memoized = useCallback(() => {}, [deps])` |
| `useMemo` | Memoize values | `const value = useMemo(() => compute(), [deps])` |
| `useRef` | Mutable ref | `const ref = useRef(initialValue)` |

### State Management Patterns

#### useState Hook
```typescript
const [state, setState] = useState<string>('initial');

// Update state
setState('new value');

// Update based on previous
setState(prev => prev + ' updated');

// Complex state
const [user, setUser] = useState<User>({ name: '', age: 0 });
setUser(prev => ({ ...prev, name: 'John' }));
```

#### useEffect Patterns
```typescript
// On mount only
useEffect(() => {
  console.log('Component mounted');
  return () => console.log('Component unmounted');
}, []);

// On dependency change
useEffect(() => {
  console.log('userId changed:', userId);
}, [userId]);

// Async data fetching
useEffect(() => {
  const fetchData = async () => {
    const result = await api.getData();
    setData(result);
  };
  fetchData();
}, []);

// Cleanup subscription
useEffect(() => {
  const subscription = source.subscribe();
  return () => subscription.unsubscribe();
}, [source]);
```

### TypeScript with React

#### Type Definitions
```typescript
// Props interface
interface ButtonProps {
  label: string;
  onClick: () => void;
  disabled?: boolean;
  variant?: 'primary' | 'secondary';
}

// Event handlers
const handleClick = (e: React.MouseEvent<HTMLButtonElement>) => {
  e.preventDefault();
};

const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  setValue(e.target.value);
};

// Children types
type Props = {
  children: React.ReactNode;  // Any valid React child
  element: React.ReactElement; // Only React elements
  text: string;               // Only strings
};
```

### API Integration with ASP.NET Core

#### Fetch Pattern
```typescript
// GET request
const fetchData = async () => {
  try {
    const response = await fetch('/api/users');
    if (!response.ok) throw new Error('Failed to fetch');
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error:', error);
    throw error;
  }
};

// POST request
const createUser = async (user: User) => {
  const response = await fetch('/api/users', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(user),
  });
  return response.json();
};
```

#### Axios Pattern
```typescript
import axios from 'axios';

// Configure base URL
const api = axios.create({
  baseURL: '/api',
  timeout: 10000,
});

// Interceptors
api.interceptors.request.use(config => {
  config.headers.Authorization = `Bearer ${getToken()}`;
  return config;
});

// API calls
const getUsers = () => api.get<User[]>('/users');
const createUser = (user: User) => api.post<User>('/users', user);
const updateUser = (id: number, user: Partial<User>) => 
  api.put<User>(`/users/${id}`, user);
```

### React Router v6

#### Basic Setup
```typescript
import { BrowserRouter, Routes, Route, Link, useNavigate } from 'react-router-dom';

function App() {
  return (
    <BrowserRouter>
      <nav>
        <Link to="/">Home</Link>
        <Link to="/about">About</Link>
      </nav>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/about" element={<About />} />
        <Route path="/users/:id" element={<UserDetail />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

// Programmatic navigation
const MyComponent = () => {
  const navigate = useNavigate();
  
  const handleClick = () => {
    navigate('/users/123');
    // or navigate(-1) to go back
  };
};
```

#### Route Parameters
```typescript
import { useParams, useSearchParams } from 'react-router-dom';

// Path params: /users/:id
const UserDetail = () => {
  const { id } = useParams<{ id: string }>();
  return <div>User ID: {id}</div>;
};

// Query params: /search?q=react
const Search = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.get('q');
  
  const updateSearch = (newQuery: string) => {
    setSearchParams({ q: newQuery });
  };
};
```

### Performance Optimization

#### React.memo
```typescript
// Prevent unnecessary re-renders
const ExpensiveComponent = React.memo(({ data }: Props) => {
  return <div>{/* Complex rendering */}</div>;
}, (prevProps, nextProps) => {
  // Return true if props are equal (skip re-render)
  return prevProps.data.id === nextProps.data.id;
});
```

#### useMemo & useCallback
```typescript
// Memoize expensive computations
const expensiveValue = useMemo(() => {
  return computeExpensiveValue(input);
}, [input]);

// Memoize callbacks
const handleClick = useCallback((id: number) => {
  doSomething(id);
}, [doSomething]);

// Usage in child component
<ChildComponent onClick={handleClick} />
```

#### Code Splitting
```typescript
// Lazy loading components
const LazyComponent = React.lazy(() => import('./LazyComponent'));

// Usage with Suspense
<Suspense fallback={<Loading />}>
  <LazyComponent />
</Suspense>

// Route-based splitting
const routes = [
  {
    path: '/dashboard',
    element: React.lazy(() => import('./pages/Dashboard')),
  },
];
```

### Forms and Validation

#### Controlled Components
```typescript
const Form = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Submit logic
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        name="name"
        value={formData.name}
        onChange={handleChange}
      />
    </form>
  );
};
```

### Context API

#### Creating and Using Context
```typescript
// Create context
interface ThemeContextType {
  theme: 'light' | 'dark';
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

// Provider component
export const ThemeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [theme, setTheme] = useState<'light' | 'dark'>('light');
  
  const toggleTheme = () => {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  };

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
};

// Custom hook for using context
export const useTheme = () => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within ThemeProvider');
  }
  return context;
};
```

### Error Boundaries

```typescript
class ErrorBoundary extends React.Component<
  { children: React.ReactNode },
  { hasError: boolean }
> {
  state = { hasError: false };

  static getDerivedStateFromError(error: Error) {
    return { hasError: true };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error('Error caught:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return <h1>Something went wrong.</h1>;
    }
    return this.props.children;
  }
}
```

### Common Patterns

#### Custom Hooks
```typescript
// Data fetching hook
function useApi<T>(url: string) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await fetch(url);
        if (!response.ok) throw new Error('Failed to fetch');
        const result = await response.json();
        setData(result);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Unknown error');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [url]);

  return { data, loading, error };
}

// Usage
const { data, loading, error } = useApi<User[]>('/api/users');
```

#### Debounce Hook
```typescript
function useDebounce<T>(value: T, delay: number): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => clearTimeout(handler);
  }, [value, delay]);

  return debouncedValue;
}

// Usage
const debouncedSearch = useDebounce(searchTerm, 500);
```

### Testing Quick Reference

```typescript
// Component testing with React Testing Library
import { render, screen, fireEvent } from '@testing-library/react';

test('renders button and handles click', () => {
  const handleClick = jest.fn();
  render(<Button onClick={handleClick}>Click me</Button>);
  
  const button = screen.getByText('Click me');
  fireEvent.click(button);
  
  expect(handleClick).toHaveBeenCalledTimes(1);
});
```

### Build and Deployment

```bash
# Development
npm start

# Production build
npm run build

# Serve with ASP.NET Core
dotnet publish -c Release
```

---

**Pro Tips**:
- Always use TypeScript for better type safety
- Prefer functional components with hooks
- Memoize expensive operations
- Use React DevTools for debugging
- Keep components small and focused
- Test user interactions, not implementation details