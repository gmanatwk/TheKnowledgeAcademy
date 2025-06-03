# Exercise 2: Implementing State Management and Routing

## 🎯 Objective
Enhance the React application with proper state management using Context API and implement client-side routing with React Router.

## ⏱️ Estimated Time
40 minutes

## 📋 Prerequisites
- Completed Exercise 1
- .NET 8.0 SDK installed
- Basic understanding of React hooks
- Familiarity with routing concepts

## 📝 Instructions

### Part 1: Implement Global State Management (15 minutes)

1. **Create Context for Todo State** in `clientapp/src/context/TodoContext.tsx`:
   ```typescript
   import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
   import { Todo, todoService } from '../services/todoService';

   interface TodoContextType {
     todos: Todo[];
     loading: boolean;
     error: string | null;
     addTodo: (title: string) => Promise<void>;
     updateTodo: (id: number, todo: Todo) => Promise<void>;
     deleteTodo: (id: number) => Promise<void>;
     toggleTodo: (id: number) => Promise<void>;
     refreshTodos: () => Promise<void>;
   }

   const TodoContext = createContext<TodoContextType | undefined>(undefined);

   export const useTodos = () => {
     const context = useContext(TodoContext);
     if (!context) {
       throw new Error('useTodos must be used within a TodoProvider');
     }
     return context;
   };

   interface TodoProviderProps {
     children: ReactNode;
   }

   export const TodoProvider: React.FC<TodoProviderProps> = ({ children }) => {
     const [todos, setTodos] = useState<Todo[]>([]);
     const [loading, setLoading] = useState(true);
     const [error, setError] = useState<string | null>(null);

     useEffect(() => {
       refreshTodos();
     }, []);

     const refreshTodos = async () => {
       try {
         setLoading(true);
         setError(null);
         const data = await todoService.getAll();
         setTodos(data);
       } catch (err) {
         setError('Failed to load todos');
         console.error(err);
       } finally {
         setLoading(false);
       }
     };

     const addTodo = async (title: string) => {
       try {
         const newTodo: Todo = {
           title,
           isCompleted: false
         };
         const created = await todoService.create(newTodo);
         setTodos(prevTodos => [...prevTodos, created]);
       } catch (err) {
         setError('Failed to add todo');
         throw err;
       }
     };

     const updateTodo = async (id: number, todo: Todo) => {
       try {
         await todoService.update(id, todo);
         setTodos(prevTodos => 
           prevTodos.map(t => t.id === id ? { ...t, ...todo } : t)
         );
       } catch (err) {
         setError('Failed to update todo');
         throw err;
       }
     };

     const deleteTodo = async (id: number) => {
       try {
         await todoService.delete(id);
         setTodos(prevTodos => prevTodos.filter(t => t.id !== id));
       } catch (err) {
         setError('Failed to delete todo');
         throw err;
       }
     };

     const toggleTodo = async (id: number) => {
       const todo = todos.find(t => t.id === id);
       if (!todo) return;

       await updateTodo(id, { ...todo, isCompleted: !todo.isCompleted });
     };

     const value: TodoContextType = {
       todos,
       loading,
       error,
       addTodo,
       updateTodo,
       deleteTodo,
       toggleTodo,
       refreshTodos
     };

     return <TodoContext.Provider value={value}>{children}</TodoContext.Provider>;
   };
   ```

2. **Create Auth Context** in `clientapp/src/context/AuthContext.tsx`:
   ```typescript
   import React, { createContext, useContext, useState, ReactNode } from 'react';

   interface User {
     id: string;
     name: string;
     email: string;
   }

   interface AuthContextType {
     user: User | null;
     isAuthenticated: boolean;
     login: (email: string, password: string) => Promise<void>;
     logout: () => void;
   }

   const AuthContext = createContext<AuthContextType | undefined>(undefined);

   export const useAuth = () => {
     const context = useContext(AuthContext);
     if (!context) {
       throw new Error('useAuth must be used within an AuthProvider');
     }
     return context;
   };

   interface AuthProviderProps {
     children: ReactNode;
   }

   export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
     const [user, setUser] = useState<User | null>(() => {
       // Check localStorage for existing session
       const savedUser = localStorage.getItem('user');
       return savedUser ? JSON.parse(savedUser) : null;
     });

     const isAuthenticated = !!user;

     const login = async (email: string, password: string) => {
       // In a real app, this would call an API
       // For demo purposes, we'll simulate a successful login
       const mockUser: User = {
         id: '1',
         name: 'John Doe',
         email: email
       };
       
       setUser(mockUser);
       localStorage.setItem('user', JSON.stringify(mockUser));
     };

     const logout = () => {
       setUser(null);
       localStorage.removeItem('user');
     };

     const value: AuthContextType = {
       user,
       isAuthenticated,
       login,
       logout
     };

     return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
   };
   ```

### Part 2: Set Up Routing (10 minutes)

1. **Create page components**:

   **Home Page** - `clientapp/src/pages/HomePage.tsx`:
   ```typescript
   import React from 'react';
   import { Link } from 'react-router-dom';
   import { useAuth } from '../context/AuthContext';

   const HomePage: React.FC = () => {
     const { isAuthenticated, user } = useAuth();

     return (
       <div className="home-page">
         <h1>Welcome to Todo App</h1>
         {isAuthenticated ? (
           <div>
             <p>Hello, {user?.name}!</p>
             <Link to="/todos" className="btn btn-primary">
               Go to Todos
             </Link>
           </div>
         ) : (
           <div>
             <p>Please login to manage your todos.</p>
             <Link to="/login" className="btn btn-primary">
               Login
             </Link>
           </div>
         )}
       </div>
     );
   };

   export default HomePage;
   ```

   **Login Page** - `clientapp/src/pages/LoginPage.tsx`:
   ```typescript
   import React, { useState } from 'react';
   import { useNavigate } from 'react-router-dom';
   import { useAuth } from '../context/AuthContext';

   const LoginPage: React.FC = () => {
     const [email, setEmail] = useState('');
     const [password, setPassword] = useState('');
     const [error, setError] = useState('');
     const { login } = useAuth();
     const navigate = useNavigate();

     const handleSubmit = async (e: React.FormEvent) => {
       e.preventDefault();
       try {
         await login(email, password);
         navigate('/todos');
       } catch (err) {
         setError('Login failed. Please try again.');
       }
     };

     return (
       <div className="login-page">
         <h2>Login</h2>
         {error && <div className="error">{error}</div>}
         <form onSubmit={handleSubmit} className="login-form">
           <div className="form-group">
             <label>Email:</label>
             <input
               type="email"
               value={email}
               onChange={(e) => setEmail(e.target.value)}
               required
               className="form-control"
             />
           </div>
           <div className="form-group">
             <label>Password:</label>
             <input
               type="password"
               value={password}
               onChange={(e) => setPassword(e.target.value)}
               required
               className="form-control"
             />
           </div>
           <button type="submit" className="btn btn-primary">
             Login
           </button>
         </form>
         <p className="hint">Hint: Any email/password will work for this demo</p>
       </div>
     );
   };

   export default LoginPage;
   ```

   **Todos Page** - `clientapp/src/pages/TodosPage.tsx`:
   ```typescript
   import React, { useState } from 'react';
   import { useTodos } from '../context/TodoContext';

   const TodosPage: React.FC = () => {
     const { todos, loading, error, addTodo, toggleTodo, deleteTodo } = useTodos();
     const [newTodoTitle, setNewTodoTitle] = useState('');
     const [filter, setFilter] = useState<'all' | 'active' | 'completed'>('all');

     const handleAddTodo = async (e: React.FormEvent) => {
       e.preventDefault();
       if (!newTodoTitle.trim()) return;
       
       try {
         await addTodo(newTodoTitle);
         setNewTodoTitle('');
       } catch (err) {
         console.error('Failed to add todo:', err);
       }
     };

     const filteredTodos = todos.filter(todo => {
       if (filter === 'active') return !todo.isCompleted;
       if (filter === 'completed') return todo.isCompleted;
       return true;
     });

     if (loading) return <div className="loading">Loading todos...</div>;
     if (error) return <div className="error">{error}</div>;

     return (
       <div className="todos-page">
         <h2>My Todos</h2>
         
         <form onSubmit={handleAddTodo} className="add-todo-form">
           <input
             type="text"
             value={newTodoTitle}
             onChange={(e) => setNewTodoTitle(e.target.value)}
             placeholder="What needs to be done?"
             className="todo-input"
           />
           <button type="submit" className="add-button">
             Add Todo
           </button>
         </form>

         <div className="filter-buttons">
           <button
             className={filter === 'all' ? 'active' : ''}
             onClick={() => setFilter('all')}
           >
             All ({todos.length})
           </button>
           <button
             className={filter === 'active' ? 'active' : ''}
             onClick={() => setFilter('active')}
           >
             Active ({todos.filter(t => !t.isCompleted).length})
           </button>
           <button
             className={filter === 'completed' ? 'active' : ''}
             onClick={() => setFilter('completed')}
           >
             Completed ({todos.filter(t => t.isCompleted).length})
           </button>
         </div>

         <ul className="todo-list">
           {filteredTodos.map(todo => (
             <li key={todo.id} className={`todo-item ${todo.isCompleted ? 'completed' : ''}`}>
               <input
                 type="checkbox"
                 checked={todo.isCompleted}
                 onChange={() => toggleTodo(todo.id!)}
               />
               <span>{todo.title}</span>
               <button
                 onClick={() => deleteTodo(todo.id!)}
                 className="delete-button"
               >
                 ×
               </button>
             </li>
           ))}
         </ul>

         {filteredTodos.length === 0 && (
           <p className="empty-state">No todos to show</p>
         )}
       </div>
     );
   };

   export default TodosPage;
   ```

2. **Create Navigation component** - `clientapp/src/components/Navigation.tsx`:
   ```typescript
   import React from 'react';
   import { Link, useNavigate } from 'react-router-dom';
   import { useAuth } from '../context/AuthContext';

   const Navigation: React.FC = () => {
     const { isAuthenticated, user, logout } = useAuth();
     const navigate = useNavigate();

     const handleLogout = () => {
       logout();
       navigate('/');
     };

     return (
       <nav className="navigation">
         <Link to="/" className="nav-brand">
           Todo App
         </Link>
         <div className="nav-links">
           <Link to="/">Home</Link>
           {isAuthenticated ? (
             <>
               <Link to="/todos">Todos</Link>
               <span className="user-info">{user?.email}</span>
               <button onClick={handleLogout} className="logout-button">
                 Logout
               </button>
             </>
           ) : (
             <Link to="/login">Login</Link>
           )}
         </div>
       </nav>
     );
   };

   export default Navigation;
   ```

### Part 3: Wire Everything Together (10 minutes)

1. **Create PrivateRoute component** - `clientapp/src/components/PrivateRoute.tsx`:
   ```typescript
   import React from 'react';
   import { Navigate } from 'react-router-dom';
   import { useAuth } from '../context/AuthContext';

   interface PrivateRouteProps {
     children: React.ReactElement;
   }

   const PrivateRoute: React.FC<PrivateRouteProps> = ({ children }) => {
     const { isAuthenticated } = useAuth();
     
     return isAuthenticated ? children : <Navigate to="/login" />;
   };

   export default PrivateRoute;
   ```

2. **Update App.tsx** with routing and context providers:
   ```typescript
   import React from 'react';
   import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
   import './App.css';
   import { AuthProvider } from './context/AuthContext';
   import { TodoProvider } from './context/TodoContext';
   import Navigation from './components/Navigation';
   import PrivateRoute from './components/PrivateRoute';
   import HomePage from './pages/HomePage';
   import LoginPage from './pages/LoginPage';
   import TodosPage from './pages/TodosPage';

   function App() {
     return (
       <Router>
         <AuthProvider>
           <TodoProvider>
             <div className="App">
               <Navigation />
               <main className="main-content">
                 <Routes>
                   <Route path="/" element={<HomePage />} />
                   <Route path="/login" element={<LoginPage />} />
                   <Route path="/todos" element={
                     <PrivateRoute>
                       <TodosPage />
                     </PrivateRoute>
                   } />
                 </Routes>
               </main>
             </div>
           </TodoProvider>
         </AuthProvider>
       </Router>
     );
   }

   export default App;
   ```

3. **Update styles** in `clientapp/src/App.css`:
   ```css
   /* Previous styles... */

   .navigation {
     background-color: #282c34;
     padding: 1rem 2rem;
     display: flex;
     justify-content: space-between;
     align-items: center;
     color: white;
   }

   .nav-brand {
     font-size: 1.5rem;
     font-weight: bold;
     color: white;
     text-decoration: none;
   }

   .nav-links {
     display: flex;
     gap: 1rem;
     align-items: center;
   }

   .nav-links a {
     color: white;
     text-decoration: none;
     padding: 0.5rem 1rem;
     border-radius: 4px;
     transition: background-color 0.3s;
   }

   .nav-links a:hover {
     background-color: rgba(255, 255, 255, 0.1);
   }

   .user-info {
     color: #61dafb;
     margin: 0 1rem;
   }

   .logout-button {
     background-color: #f44336;
     color: white;
     border: none;
     padding: 0.5rem 1rem;
     border-radius: 4px;
     cursor: pointer;
   }

   .logout-button:hover {
     background-color: #da190b;
   }

   .main-content {
     min-height: calc(100vh - 60px);
     padding: 2rem;
   }

   .home-page, .login-page, .todos-page {
     max-width: 800px;
     margin: 0 auto;
   }

   .login-form {
     max-width: 400px;
     margin: 2rem auto;
   }

   .form-group {
     margin-bottom: 1rem;
   }

   .form-control {
     width: 100%;
     padding: 0.5rem;
     border: 1px solid #ddd;
     border-radius: 4px;
     font-size: 1rem;
   }

   .btn {
     padding: 0.5rem 1rem;
     border: none;
     border-radius: 4px;
     cursor: pointer;
     text-decoration: none;
     display: inline-block;
     transition: background-color 0.3s;
   }

   .btn-primary {
     background-color: #007bff;
     color: white;
   }

   .btn-primary:hover {
     background-color: #0056b3;
   }

   .filter-buttons {
     display: flex;
     gap: 0.5rem;
     margin: 1rem 0;
     justify-content: center;
   }

   .filter-buttons button {
     padding: 0.5rem 1rem;
     border: 1px solid #ddd;
     background-color: white;
     cursor: pointer;
     border-radius: 4px;
     transition: all 0.3s;
   }

   .filter-buttons button.active {
     background-color: #007bff;
     color: white;
     border-color: #007bff;
   }

   .empty-state {
     text-align: center;
     color: #666;
     margin: 2rem 0;
   }

   .hint {
     color: #666;
     font-style: italic;
     margin-top: 1rem;
   }

   .loading {
     text-align: center;
     color: #666;
     margin: 2rem 0;
   }
   ```

### Part 4: Test the Enhanced Application (5 minutes)

1. **Run the application**:
   ```bash
   dotnet run
   ```

2. **Test the features**:
   - Navigate between pages
   - Try accessing /todos without logging in
   - Login with any email/password
   - Add, complete, and delete todos
   - Use the filter buttons
   - Logout and verify redirect

## ✅ Success Criteria

- [ ] Context API manages todo state globally
- [ ] Authentication context tracks user state
- [ ] Routing works between all pages
- [ ] Private routes protect the todos page
- [ ] Navigation shows correct links based on auth state
- [ ] Todo filters work correctly
- [ ] Logout clears user state and redirects

## 🚀 Bonus Challenges

1. **Add Todo Details Page**:
   - Create a route like `/todos/:id`
   - Show full todo details
   - Allow editing todo title and description

2. **Persist Auth State**:
   - Store auth token in httpOnly cookie
   - Implement token refresh logic
   - Add proper API authentication

3. **Advanced State Management**:
   - Implement useReducer for complex state
   - Add optimistic updates
   - Implement undo/redo functionality

4. **Add Loading States**:
   - Show skeleton loaders
   - Add progress indicators
   - Implement error boundaries

## 🤔 Reflection Questions

1. What are the benefits of using Context API vs prop drilling?
2. How would you implement real authentication with JWT tokens?
3. What are the trade-offs between Context API and Redux?
4. How would you handle deep linking with private routes?

## 🆘 Troubleshooting

**Issue**: Context value is undefined
**Solution**: Ensure components are wrapped in the provider and using the hook correctly.

**Issue**: Routes not working
**Solution**: Check that BrowserRouter wraps the entire app and paths are correct.

**Issue**: State not persisting on refresh
**Solution**: This is expected. Implement localStorage or sessionStorage for persistence.

---

