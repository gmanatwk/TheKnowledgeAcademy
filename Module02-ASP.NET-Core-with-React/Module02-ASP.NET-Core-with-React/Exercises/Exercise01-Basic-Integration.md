# Exercise 1: Setting Up React with ASP.NET Core

## 🎯 Objective
Create a full-stack application with React frontend and ASP.NET Core backend, establishing the foundation for modern web development.

## ⏱️ Estimated Time
45 minutes

## 📋 Prerequisites
- Completed Module 1
- .NET 8.0 SDK installed
- Node.js 18+ installed
- Basic React knowledge
- Understanding of RESTful APIs

## 📝 Instructions

### Part 1: Create ASP.NET Core Web API (10 minutes)

1. **Create a new Web API project**:
   ```bash
   dotnet new webapi -n ReactTodoApp --framework net8.0
   cd ReactTodoApp
   ```

2. **Add necessary NuGet packages**:
   ```bash
   dotnet add package Microsoft.AspNetCore.SpaServices.Extensions --version 8.0.0
   ```

3. **Create a Todo model** in `Models/Todo.cs`:
   ```csharp
   namespace ReactTodoApp.Models
   {
       public class Todo
       {
           public int Id { get; set; }
           public string Title { get; set; } = string.Empty;
           public bool IsCompleted { get; set; }
           public DateTime CreatedAt { get; set; }
       }
   }
   ```

4. **Create TodoController** in `Controllers/TodoController.cs`:
   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using ReactTodoApp.Models;

   namespace ReactTodoApp.Controllers
   {
       [ApiController]
       [Route("api/[controller]")]
       public class TodoController : ControllerBase
       {
           private static readonly List<Todo> _todos = new()
           {
               new Todo { Id = 1, Title = "Learn React", IsCompleted = false, CreatedAt = DateTime.Now },
               new Todo { Id = 2, Title = "Build API", IsCompleted = true, CreatedAt = DateTime.Now }
           };

           [HttpGet]
           public ActionResult<IEnumerable<Todo>> GetTodos()
           {
               return Ok(_todos);
           }

           [HttpGet("{id}")]
           public ActionResult<Todo> GetTodo(int id)
           {
               var todo = _todos.FirstOrDefault(t => t.Id == id);
               if (todo == null)
                   return NotFound();
               
               return Ok(todo);
           }

           [HttpPost]
           public ActionResult<Todo> CreateTodo(Todo todo)
           {
               todo.Id = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1;
               todo.CreatedAt = DateTime.Now;
               _todos.Add(todo);
               
               return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
           }

           [HttpPut("{id}")]
           public ActionResult UpdateTodo(int id, Todo todo)
           {
               var existingTodo = _todos.FirstOrDefault(t => t.Id == id);
               if (existingTodo == null)
                   return NotFound();
               
               existingTodo.Title = todo.Title;
               existingTodo.IsCompleted = todo.IsCompleted;
               
               return NoContent();
           }

           [HttpDelete("{id}")]
           public ActionResult DeleteTodo(int id)
           {
               var todo = _todos.FirstOrDefault(t => t.Id == id);
               if (todo == null)
                   return NotFound();
               
               _todos.Remove(todo);
               return NoContent();
           }
       }
   }
   ```

### Part 2: Create React Application (15 minutes)

1. **Create React app in the project root**:
   ```bash
   npx create-react-app clientapp --template typescript
   ```

2. **Install additional dependencies**:
   ```bash
   cd clientapp
   npm install axios react-router-dom @types/react-router-dom
   cd ..
   ```

3. **Configure ASP.NET Core to serve React app**. Update `Program.cs`:
   ```csharp
   var builder = WebApplication.CreateBuilder(args);

   // Add services to the container
   builder.Services.AddControllers();
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();

   // Add SPA static files
   builder.Services.AddSpaStaticFiles(configuration =>
   {
       configuration.RootPath = "clientapp/build";
   });

   var app = builder.Build();

   // Configure the HTTP request pipeline
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }

   app.UseHttpsRedirection();
   app.UseStaticFiles();
   app.UseSpaStaticFiles();

   app.UseRouting();
   app.UseAuthorization();

   app.MapControllers();

   // Configure SPA
   app.UseSpa(spa =>
   {
       spa.Options.SourcePath = "clientapp";

       if (app.Environment.IsDevelopment())
       {
           spa.UseReactDevelopmentServer(npmScript: "start");
       }
   });

   app.Run();
   ```

### Part 3: Create React Components (15 minutes)

1. **Create API service** in `clientapp/src/services/todoService.ts`:
   ```typescript
   import axios from 'axios';

   const API_BASE_URL = '/api/todo';

   export interface Todo {
     id?: number;
     title: string;
     isCompleted: boolean;
     createdAt?: string;
   }

   export const todoService = {
     getAll: async (): Promise<Todo[]> => {
       const response = await axios.get<Todo[]>(API_BASE_URL);
       return response.data;
     },

     getById: async (id: number): Promise<Todo> => {
       const response = await axios.get<Todo>(`${API_BASE_URL}/${id}`);
       return response.data;
     },

     create: async (todo: Todo): Promise<Todo> => {
       const response = await axios.post<Todo>(API_BASE_URL, todo);
       return response.data;
     },

     update: async (id: number, todo: Todo): Promise<void> => {
       await axios.put(`${API_BASE_URL}/${id}`, todo);
     },

     delete: async (id: number): Promise<void> => {
       await axios.delete(`${API_BASE_URL}/${id}`);
     }
   };
   ```

2. **Create TodoList component** in `clientapp/src/components/TodoList.tsx`:
   ```typescript
   import React, { useState, useEffect } from 'react';
   import { Todo, todoService } from '../services/todoService';

   const TodoList: React.FC = () => {
     const [todos, setTodos] = useState<Todo[]>([]);
     const [newTodo, setNewTodo] = useState('');
     const [loading, setLoading] = useState(true);
     const [error, setError] = useState<string | null>(null);

     useEffect(() => {
       loadTodos();
     }, []);

     const loadTodos = async () => {
       try {
         setLoading(true);
         const data = await todoService.getAll();
         setTodos(data);
         setError(null);
       } catch (err) {
         setError('Failed to load todos');
         console.error(err);
       } finally {
         setLoading(false);
       }
     };

     const handleAddTodo = async (e: React.FormEvent) => {
       e.preventDefault();
       if (!newTodo.trim()) return;

       try {
         const todo: Todo = {
           title: newTodo,
           isCompleted: false
         };
         const created = await todoService.create(todo);
         setTodos([...todos, created]);
         setNewTodo('');
       } catch (err) {
         setError('Failed to add todo');
       }
     };

     const handleToggle = async (todo: Todo) => {
       try {
         const updated = { ...todo, isCompleted: !todo.isCompleted };
         await todoService.update(todo.id!, updated);
         setTodos(todos.map(t => t.id === todo.id ? updated : t));
       } catch (err) {
         setError('Failed to update todo');
       }
     };

     const handleDelete = async (id: number) => {
       try {
         await todoService.delete(id);
         setTodos(todos.filter(t => t.id !== id));
       } catch (err) {
         setError('Failed to delete todo');
       }
     };

     if (loading) return <div>Loading...</div>;
     if (error) return <div className="error">{error}</div>;

     return (
       <div className="todo-container">
         <h1>Todo List</h1>
         
         <form onSubmit={handleAddTodo} className="add-todo-form">
           <input
             type="text"
             value={newTodo}
             onChange={(e) => setNewTodo(e.target.value)}
             placeholder="Add a new todo..."
             className="todo-input"
           />
           <button type="submit" className="add-button">Add</button>
         </form>

         <ul className="todo-list">
           {todos.map(todo => (
             <li key={todo.id} className={`todo-item ${todo.isCompleted ? 'completed' : ''}`}>
               <input
                 type="checkbox"
                 checked={todo.isCompleted}
                 onChange={() => handleToggle(todo)}
               />
               <span>{todo.title}</span>
               <button 
                 onClick={() => handleDelete(todo.id!)}
                 className="delete-button"
               >
                 Delete
               </button>
             </li>
           ))}
         </ul>
       </div>
     );
   };

   export default TodoList;
   ```

3. **Update App.tsx** in `clientapp/src/App.tsx`:
   ```typescript
   import React from 'react';
   import './App.css';
   import TodoList from './components/TodoList';

   function App() {
     return (
       <div className="App">
         <header className="App-header">
           <h1>React + ASP.NET Core Todo App</h1>
         </header>
         <main>
           <TodoList />
         </main>
       </div>
     );
   }

   export default App;
   ```

4. **Add styles** to `clientapp/src/App.css`:
   ```css
   .App {
     text-align: center;
     min-height: 100vh;
     background-color: #f5f5f5;
   }

   .App-header {
     background-color: #282c34;
     padding: 20px;
     color: white;
   }

   .todo-container {
     max-width: 600px;
     margin: 40px auto;
     padding: 20px;
     background: white;
     border-radius: 8px;
     box-shadow: 0 2px 4px rgba(0,0,0,0.1);
   }

   .add-todo-form {
     display: flex;
     gap: 10px;
     margin-bottom: 20px;
   }

   .todo-input {
     flex: 1;
     padding: 10px;
     border: 1px solid #ddd;
     border-radius: 4px;
     font-size: 16px;
   }

   .add-button {
     padding: 10px 20px;
     background-color: #4CAF50;
     color: white;
     border: none;
     border-radius: 4px;
     cursor: pointer;
   }

   .add-button:hover {
     background-color: #45a049;
   }

   .todo-list {
     list-style: none;
     padding: 0;
   }

   .todo-item {
     display: flex;
     align-items: center;
     padding: 10px;
     border-bottom: 1px solid #eee;
     gap: 10px;
   }

   .todo-item.completed span {
     text-decoration: line-through;
     color: #888;
   }

   .delete-button {
     margin-left: auto;
     padding: 5px 10px;
     background-color: #f44336;
     color: white;
     border: none;
     border-radius: 4px;
     cursor: pointer;
   }

   .delete-button:hover {
     background-color: #da190b;
   }

   .error {
     color: red;
     padding: 10px;
     background-color: #ffebee;
     border-radius: 4px;
     margin: 10px 0;
   }
   ```

### Part 4: Test the Application (5 minutes)

1. **Run the application**:
   ```bash
   dotnet run
   ```

2. **Test the features**:
   - The app should open at https://localhost:[port]
   - Add new todos
   - Toggle completion status
   - Delete todos
   - Check that API calls work in the Network tab

3. **Test the API directly**:
   - Navigate to https://localhost:[port]/swagger
   - Test the API endpoints

## ✅ Success Criteria

- [ ] ASP.NET Core Web API is running with Todo endpoints
- [ ] React application loads and displays todos
- [ ] Can add new todos
- [ ] Can toggle todo completion
- [ ] Can delete todos
- [ ] No console errors
- [ ] API calls visible in browser DevTools

## 🚀 Bonus Challenges

1. **Add Edit Functionality**:
   - Allow inline editing of todo titles
   - Add an edit button and save changes

2. **Add Filtering**:
   - Filter todos by status (All, Active, Completed)
   - Add a search box to filter by title

3. **Persist Data**:
   - Replace in-memory storage with Entity Framework Core
   - Add a SQLite database

4. **Add Authentication**:
   - Implement user authentication
   - Associate todos with users

## 🤔 Reflection Questions

1. How does the proxy configuration work between React and ASP.NET Core?
2. What are the benefits of separating frontend and backend?
3. How would you handle errors more gracefully?
4. What security considerations should you implement?

## 🆘 Troubleshooting

**Issue**: CORS errors
**Solution**: The SPA middleware handles this in development. For production, configure CORS properly.

**Issue**: React app not loading
**Solution**: Check that Node.js is installed and `npm install` was run in the clientapp folder.

**Issue**: API calls failing
**Solution**: Check the browser console and ensure the API is running on the correct port.

**Issue**: Hot reload not working
**Solution**: Make sure you're running in Development mode and the React dev server is running.

---

