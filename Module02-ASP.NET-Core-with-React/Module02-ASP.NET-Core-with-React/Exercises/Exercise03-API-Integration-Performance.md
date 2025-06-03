# Exercise 3: Advanced API Integration and Performance Optimization

## 🎯 Objective
Implement advanced API integration patterns, error handling, caching, and performance optimizations for a production-ready React + ASP.NET Core application.

## ⏱️ Estimated Time
35 minutes

## 📋 Prerequisites
- Completed Exercises 1 and 2
- .NET 8.0 SDK installed
- Understanding of async/await patterns
- Basic knowledge of performance optimization

## 📝 Instructions

### Part 1: Advanced API Client with Interceptors (10 minutes)

1. **Create an enhanced API client** in `clientapp/src/services/apiClient.ts`:
   ```typescript
   import axios, { AxiosInstance, AxiosRequestConfig, AxiosError } from 'axios';

   interface ApiError {
     message: string;
     statusCode: number;
     details?: any;
   }

   class ApiClient {
     private client: AxiosInstance;
     private requestInterceptor: number | null = null;
     private responseInterceptor: number | null = null;

     constructor() {
       this.client = axios.create({
         baseURL: '/api',
         timeout: 10000,
         headers: {
           'Content-Type': 'application/json',
         },
       });

       this.setupInterceptors();
     }

     private setupInterceptors(): void {
       // Request interceptor for auth token
       this.requestInterceptor = this.client.interceptors.request.use(
         (config) => {
           const token = localStorage.getItem('authToken');
           if (token) {
             config.headers.Authorization = `Bearer ${token}`;
           }
           
           // Add request ID for tracking
           config.headers['X-Request-ID'] = this.generateRequestId();
           
           return config;
         },
         (error) => {
           return Promise.reject(error);
         }
       );

       // Response interceptor for error handling
       this.responseInterceptor = this.client.interceptors.response.use(
         (response) => {
           // Log successful requests in development
           if (process.env.NODE_ENV === 'development') {
             console.log(`API Success: ${response.config.method?.toUpperCase()} ${response.config.url}`, response.data);
           }
           return response;
         },
         async (error: AxiosError) => {
           const apiError: ApiError = {
             message: 'An unexpected error occurred',
             statusCode: error.response?.status || 0,
             details: error.response?.data,
           };

           // Handle specific error cases
           switch (error.response?.status) {
             case 401:
               apiError.message = 'Authentication required';
               // Redirect to login
               window.location.href = '/login';
               break;
             case 403:
               apiError.message = 'You do not have permission to perform this action';
               break;
             case 404:
               apiError.message = 'The requested resource was not found';
               break;
             case 429:
               apiError.message = 'Too many requests. Please try again later';
               break;
             case 500:
               apiError.message = 'Server error. Please try again later';
               break;
           }

           // Network error
           if (!error.response) {
             apiError.message = 'Network error. Please check your connection';
           }

           return Promise.reject(apiError);
         }
       );
     }

     private generateRequestId(): string {
       return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
     }

     // Generic request methods with retry logic
     async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
       const response = await this.retryRequest(() => this.client.get<T>(url, config));
       return response.data;
     }

     async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
       const response = await this.retryRequest(() => this.client.post<T>(url, data, config));
       return response.data;
     }

     async put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
       const response = await this.retryRequest(() => this.client.put<T>(url, data, config));
       return response.data;
     }

     async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
       const response = await this.retryRequest(() => this.client.delete<T>(url, config));
       return response.data;
     }

     // Retry logic for failed requests
     private async retryRequest<T>(
       requestFn: () => Promise<T>,
       maxRetries: number = 3,
       delay: number = 1000
     ): Promise<T> {
       let lastError: any;

       for (let i = 0; i < maxRetries; i++) {
         try {
           return await requestFn();
         } catch (error: any) {
           lastError = error;
           
           // Don't retry on client errors (4xx)
           if (error.statusCode && error.statusCode >= 400 && error.statusCode < 500) {
             throw error;
           }

           // Wait before retrying
           if (i < maxRetries - 1) {
             await new Promise(resolve => setTimeout(resolve, delay * Math.pow(2, i)));
           }
         }
       }

       throw lastError;
     }

     // Cleanup method
     cleanup(): void {
       if (this.requestInterceptor !== null) {
         this.client.interceptors.request.eject(this.requestInterceptor);
       }
       if (this.responseInterceptor !== null) {
         this.client.interceptors.response.eject(this.responseInterceptor);
       }
     }
   }

   export default new ApiClient();
   ```

2. **Update todo service** to use the new API client in `clientapp/src/services/todoService.ts`:
   ```typescript
   import apiClient from './apiClient';

   export interface Todo {
     id?: number;
     title: string;
     isCompleted: boolean;
     createdAt?: string;
     updatedAt?: string;
   }

   export interface TodoStats {
     total: number;
     completed: number;
     active: number;
   }

   class TodoService {
     private readonly baseUrl = '/todo';

     async getAll(): Promise<Todo[]> {
       return apiClient.get<Todo[]>(this.baseUrl);
     }

     async getById(id: number): Promise<Todo> {
       return apiClient.get<Todo>(`${this.baseUrl}/${id}`);
     }

     async create(todo: Omit<Todo, 'id'>): Promise<Todo> {
       return apiClient.post<Todo>(this.baseUrl, todo);
     }

     async update(id: number, todo: Partial<Todo>): Promise<Todo> {
       return apiClient.put<Todo>(`${this.baseUrl}/${id}`, todo);
     }

     async delete(id: number): Promise<void> {
       return apiClient.delete<void>(`${this.baseUrl}/${id}`);
     }

     async getStats(): Promise<TodoStats> {
       return apiClient.get<TodoStats>(`${this.baseUrl}/stats`);
     }

     async bulkDelete(ids: number[]): Promise<void> {
       return apiClient.post<void>(`${this.baseUrl}/bulk-delete`, { ids });
     }

     async bulkComplete(ids: number[]): Promise<void> {
       return apiClient.post<void>(`${this.baseUrl}/bulk-complete`, { ids });
     }
   }

   export const todoService = new TodoService();
   ```

### Part 2: Implement Caching and Optimistic Updates (10 minutes)

1. **Create a cache service** in `clientapp/src/services/cacheService.ts`:
   ```typescript
   interface CacheItem<T> {
     data: T;
     timestamp: number;
     ttl: number;
   }

   class CacheService {
     private cache = new Map<string, CacheItem<any>>();
     private cleanupInterval: NodeJS.Timer | null = null;

     constructor() {
       // Clean up expired items every minute
       this.cleanupInterval = setInterval(() => this.cleanup(), 60000);
     }

     set<T>(key: string, data: T, ttlSeconds: number = 300): void {
       this.cache.set(key, {
         data,
         timestamp: Date.now(),
         ttl: ttlSeconds * 1000,
       });
     }

     get<T>(key: string): T | null {
       const item = this.cache.get(key);
       
       if (!item) {
         return null;
       }

       // Check if item has expired
       if (Date.now() - item.timestamp > item.ttl) {
         this.cache.delete(key);
         return null;
       }

       return item.data as T;
     }

     invalidate(key: string): void {
       this.cache.delete(key);
     }

     invalidatePattern(pattern: string): void {
       const regex = new RegExp(pattern);
       for (const key of this.cache.keys()) {
         if (regex.test(key)) {
           this.cache.delete(key);
         }
       }
     }

     clear(): void {
       this.cache.clear();
     }

     private cleanup(): void {
       const now = Date.now();
       for (const [key, item] of this.cache.entries()) {
         if (now - item.timestamp > item.ttl) {
           this.cache.delete(key);
         }
       }
     }

     destroy(): void {
       if (this.cleanupInterval) {
         clearInterval(this.cleanupInterval);
       }
       this.cache.clear();
     }
   }

   export default new CacheService();
   ```

2. **Create custom hooks with caching** in `clientapp/src/hooks/useTodos.ts`:
   ```typescript
   import { useState, useEffect, useCallback } from 'react';
   import { Todo, TodoStats, todoService } from '../services/todoService';
   import cacheService from '../services/cacheService';

   interface UseTodosResult {
     todos: Todo[];
     stats: TodoStats | null;
     loading: boolean;
     error: string | null;
     refetch: () => Promise<void>;
     createTodo: (title: string) => Promise<void>;
     updateTodo: (id: number, updates: Partial<Todo>) => Promise<void>;
     deleteTodo: (id: number) => Promise<void>;
     bulkDelete: (ids: number[]) => Promise<void>;
     bulkComplete: (ids: number[]) => Promise<void>;
   }

   export function useTodos(): UseTodosResult {
     const [todos, setTodos] = useState<Todo[]>([]);
     const [stats, setStats] = useState<TodoStats | null>(null);
     const [loading, setLoading] = useState(true);
     const [error, setError] = useState<string | null>(null);

     const CACHE_KEY = 'todos-list';
     const STATS_CACHE_KEY = 'todos-stats';

     const fetchTodos = useCallback(async () => {
       try {
         setLoading(true);
         setError(null);

         // Check cache first
         const cachedTodos = cacheService.get<Todo[]>(CACHE_KEY);
         if (cachedTodos) {
           setTodos(cachedTodos);
           setLoading(false);
         }

         // Fetch fresh data
         const [todosData, statsData] = await Promise.all([
           todoService.getAll(),
           todoService.getStats(),
         ]);

         setTodos(todosData);
         setStats(statsData);
         
         // Update cache
         cacheService.set(CACHE_KEY, todosData, 300); // 5 minutes
         cacheService.set(STATS_CACHE_KEY, statsData, 300);
       } catch (err: any) {
         setError(err.message || 'Failed to fetch todos');
       } finally {
         setLoading(false);
       }
     }, []);

     useEffect(() => {
       fetchTodos();
     }, [fetchTodos]);

     const createTodo = useCallback(async (title: string) => {
       // Optimistic update
       const tempId = -Date.now();
       const tempTodo: Todo = {
         id: tempId,
         title,
         isCompleted: false,
         createdAt: new Date().toISOString(),
       };

       setTodos(prev => [...prev, tempTodo]);

       try {
         const newTodo = await todoService.create({ title, isCompleted: false });
         setTodos(prev => prev.map(t => t.id === tempId ? newTodo : t));
         
         // Update stats
         setStats(prev => prev ? { ...prev, total: prev.total + 1, active: prev.active + 1 } : null);
         
         // Invalidate cache
         cacheService.invalidate(CACHE_KEY);
         cacheService.invalidate(STATS_CACHE_KEY);
       } catch (err: any) {
         // Revert optimistic update
         setTodos(prev => prev.filter(t => t.id !== tempId));
         throw err;
       }
     }, []);

     const updateTodo = useCallback(async (id: number, updates: Partial<Todo>) => {
       // Optimistic update
       const originalTodos = [...todos];
       setTodos(prev => prev.map(t => t.id === id ? { ...t, ...updates } : t));

       try {
         await todoService.update(id, updates);
         
         // Update stats if completion status changed
         if ('isCompleted' in updates) {
           setStats(prev => {
             if (!prev) return null;
             const delta = updates.isCompleted ? 1 : -1;
             return {
               ...prev,
               completed: prev.completed + delta,
               active: prev.active - delta,
             };
           });
         }

         // Invalidate cache
         cacheService.invalidate(CACHE_KEY);
         cacheService.invalidate(STATS_CACHE_KEY);
       } catch (err: any) {
         // Revert optimistic update
         setTodos(originalTodos);
         throw err;
       }
     }, [todos]);

     const deleteTodo = useCallback(async (id: number) => {
       // Optimistic update
       const todoToDelete = todos.find(t => t.id === id);
       if (!todoToDelete) return;

       setTodos(prev => prev.filter(t => t.id !== id));

       try {
         await todoService.delete(id);
         
         // Update stats
         setStats(prev => {
           if (!prev) return null;
           return {
             ...prev,
             total: prev.total - 1,
             completed: todoToDelete.isCompleted ? prev.completed - 1 : prev.completed,
             active: !todoToDelete.isCompleted ? prev.active - 1 : prev.active,
           };
         });

         // Invalidate cache
         cacheService.invalidate(CACHE_KEY);
         cacheService.invalidate(STATS_CACHE_KEY);
       } catch (err: any) {
         // Revert optimistic update
         setTodos(prev => [...prev, todoToDelete].sort((a, b) => (a.id || 0) - (b.id || 0)));
         throw err;
       }
     }, [todos]);

     const bulkDelete = useCallback(async (ids: number[]) => {
       const originalTodos = [...todos];
       setTodos(prev => prev.filter(t => !ids.includes(t.id!)));

       try {
         await todoService.bulkDelete(ids);
         cacheService.invalidate(CACHE_KEY);
         cacheService.invalidate(STATS_CACHE_KEY);
         await fetchTodos(); // Refetch to get updated stats
       } catch (err: any) {
         setTodos(originalTodos);
         throw err;
       }
     }, [todos, fetchTodos]);

     const bulkComplete = useCallback(async (ids: number[]) => {
       const originalTodos = [...todos];
       setTodos(prev => prev.map(t => ids.includes(t.id!) ? { ...t, isCompleted: true } : t));

       try {
         await todoService.bulkComplete(ids);
         cacheService.invalidate(CACHE_KEY);
         cacheService.invalidate(STATS_CACHE_KEY);
         await fetchTodos(); // Refetch to get updated stats
       } catch (err: any) {
         setTodos(originalTodos);
         throw err;
       }
     }, [todos, fetchTodos]);

     return {
       todos,
       stats,
       loading,
       error,
       refetch: fetchTodos,
       createTodo,
       updateTodo,
       deleteTodo,
       bulkDelete,
       bulkComplete,
     };
   }
   ```

### Part 3: Performance Optimizations (10 minutes)

1. **Create virtualized todo list** for large datasets in `clientapp/src/components/VirtualizedTodoList.tsx`:
   ```typescript
   import React, { useCallback, useMemo } from 'react';
   import { FixedSizeList as List } from 'react-window';
   import { Todo } from '../services/todoService';

   interface VirtualizedTodoListProps {
     todos: Todo[];
     onToggle: (id: number) => void;
     onDelete: (id: number) => void;
     height: number;
   }

   interface TodoItemProps {
     index: number;
     style: React.CSSProperties;
     data: {
       todos: Todo[];
       onToggle: (id: number) => void;
       onDelete: (id: number) => void;
     };
   }

   const TodoItem: React.FC<TodoItemProps> = React.memo(({ index, style, data }) => {
     const { todos, onToggle, onDelete } = data;
     const todo = todos[index];

     return (
       <div style={style} className={`todo-item ${todo.isCompleted ? 'completed' : ''}`}>
         <input
           type="checkbox"
           checked={todo.isCompleted}
           onChange={() => onToggle(todo.id!)}
         />
         <span>{todo.title}</span>
         <button onClick={() => onDelete(todo.id!)} className="delete-button">
           ×
         </button>
       </div>
     );
   });

   const VirtualizedTodoList: React.FC<VirtualizedTodoListProps> = ({ 
     todos, 
     onToggle, 
     onDelete, 
     height 
   }) => {
     const itemData = useMemo(
       () => ({ todos, onToggle, onDelete }),
       [todos, onToggle, onDelete]
     );

     return (
       <List
         height={height}
         itemCount={todos.length}
         itemSize={50}
         width="100%"
         itemData={itemData}
       >
         {TodoItem}
       </List>
     );
   };

   export default React.memo(VirtualizedTodoList);
   ```

2. **Add performance monitoring** in `clientapp/src/components/PerformanceMonitor.tsx`:
   ```typescript
   import React, { useEffect, useState } from 'react';

   interface PerformanceMetrics {
     renderTime: number;
     apiCallTime: number;
     memoryUsage: number;
   }

   const PerformanceMonitor: React.FC = () => {
     const [metrics, setMetrics] = useState<PerformanceMetrics>({
       renderTime: 0,
       apiCallTime: 0,
       memoryUsage: 0,
     });

     useEffect(() => {
       // Monitor render performance
       const observer = new PerformanceObserver((list) => {
         for (const entry of list.getEntries()) {
           if (entry.entryType === 'measure' && entry.name.startsWith('render-')) {
             setMetrics(prev => ({ ...prev, renderTime: entry.duration }));
           }
         }
       });

       observer.observe({ entryTypes: ['measure'] });

       // Monitor memory usage
       const memoryInterval = setInterval(() => {
         if ('memory' in performance) {
           const memory = (performance as any).memory;
           setMetrics(prev => ({
             ...prev,
             memoryUsage: memory.usedJSHeapSize / 1048576, // Convert to MB
           }));
         }
       }, 1000);

       return () => {
         observer.disconnect();
         clearInterval(memoryInterval);
       };
     }, []);

     if (process.env.NODE_ENV !== 'development') {
       return null;
     }

     return (
       <div className="performance-monitor">
         <h4>Performance Metrics</h4>
         <div>Render Time: {metrics.renderTime.toFixed(2)}ms</div>
         <div>Memory Usage: {metrics.memoryUsage.toFixed(2)}MB</div>
       </div>
     );
   };

   export default PerformanceMonitor;
   ```

3. **Update ASP.NET Core API** with new endpoints in `Controllers/TodoController.cs`:
   ```csharp
   [HttpGet("stats")]
   public ActionResult<TodoStatsDto> GetStats()
   {
       var stats = new TodoStatsDto
       {
           Total = _todos.Count,
           Completed = _todos.Count(t => t.IsCompleted),
           Active = _todos.Count(t => !t.IsCompleted)
       };
       
       return Ok(stats);
   }

   [HttpPost("bulk-delete")]
   public ActionResult BulkDelete([FromBody] BulkOperationDto dto)
   {
       _todos.RemoveAll(t => dto.Ids.Contains(t.Id));
       return NoContent();
   }

   [HttpPost("bulk-complete")]
   public ActionResult BulkComplete([FromBody] BulkOperationDto dto)
   {
       foreach (var todo in _todos.Where(t => dto.Ids.Contains(t.Id)))
       {
           todo.IsCompleted = true;
       }
       
       return NoContent();
   }

   // DTOs
   public class TodoStatsDto
   {
       public int Total { get; set; }
       public int Completed { get; set; }
       public int Active { get; set; }
   }

   public class BulkOperationDto
   {
       public List<int> Ids { get; set; } = new();
   }
   ```

### Part 4: Testing and Optimization (5 minutes)

1. **Add performance profiling to your app**:
   ```typescript
   // In App.tsx
   import { Profiler } from 'react';

   function onRenderCallback(
     id: string,
     phase: "mount" | "update",
     actualDuration: number,
     baseDuration: number,
     startTime: number,
     commitTime: number,
     interactions: Set<any>
   ) {
     console.log(`Component ${id} (${phase}) took ${actualDuration}ms`);
   }

   // Wrap components with Profiler
   <Profiler id="TodoList" onRender={onRenderCallback}>
     <TodosPage />
   </Profiler>
   ```

2. **Test the application**:
   - Generate many todos (100+) to test virtualization
   - Monitor network tab for caching behavior
   - Test optimistic updates by throttling network
   - Check performance metrics in development

## ✅ Success Criteria

- [ ] API client handles errors gracefully
- [ ] Requests include authentication headers
- [ ] Caching reduces unnecessary API calls
- [ ] Optimistic updates provide instant feedback
- [ ] Large todo lists render efficiently
- [ ] Bulk operations work correctly
- [ ] Performance metrics are visible in development

## 🚀 Bonus Challenges

1. **Add Real-time Updates**:
   - Implement SignalR for real-time todo updates
   - Show when other users modify todos
   - Add presence indicators

2. **Advanced Caching**:
   - Implement cache invalidation strategies
   - Add background sync
   - Use IndexedDB for offline support

3. **Performance Dashboard**:
   - Create comprehensive performance monitoring
   - Track API response times
   - Monitor bundle size impact

## 🤔 Reflection Questions

1. How do optimistic updates improve user experience?
2. What are the trade-offs of client-side caching?
3. When should you use virtualization vs pagination?
4. How would you implement offline support?

## 🆘 Troubleshooting

**Issue**: Optimistic updates cause flickering
**Solution**: Ensure stable IDs and proper React keys.

**Issue**: Cache causes stale data
**Solution**: Implement proper cache invalidation and TTL.

**Issue**: Performance metrics not showing
**Solution**: Ensure you're running in development mode.

---

