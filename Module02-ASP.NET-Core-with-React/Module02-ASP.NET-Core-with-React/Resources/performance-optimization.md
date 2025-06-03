# React + ASP.NET Core Performance Optimization Guide

## 🚀 Performance Best Practices

### Overview

This guide covers performance optimization techniques for React applications integrated with ASP.NET Core backends, focusing on both client-side and server-side optimizations.

## Client-Side (React) Optimizations

### 1. Code Splitting and Lazy Loading

#### Route-Based Code Splitting
```typescript
import React, { lazy, Suspense } from 'react';
import { Routes, Route } from 'react-router-dom';

// Lazy load route components
const Dashboard = lazy(() => import('./pages/Dashboard'));
const UserProfile = lazy(() => import('./pages/UserProfile'));
const Settings = lazy(() => import('./pages/Settings'));

// Loading component
const PageLoader = () => (
  <div className="page-loader">
    <div className="spinner" />
    <p>Loading...</p>
  </div>
);

function App() {
  return (
    <Suspense fallback={<PageLoader />}>
      <Routes>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/profile" element={<UserProfile />} />
        <Route path="/settings" element={<Settings />} />
      </Routes>
    </Suspense>
  );
}
```

#### Component-Level Code Splitting
```typescript
// Split heavy components
const HeavyChart = lazy(() => 
  import(/* webpackChunkName: "charts" */ './components/HeavyChart')
);

// Split libraries
const loadMoment = () => import(/* webpackChunkName: "moment" */ 'moment');

// Usage
function DateComponent() {
  const [moment, setMoment] = useState(null);

  useEffect(() => {
    loadMoment().then(module => setMoment(module.default));
  }, []);

  if (!moment) return <div>Loading date library...</div>;
  
  return <div>{moment().format('MMMM Do YYYY')}</div>;
}
```

### 2. React Component Optimization

#### React.memo for Expensive Components
```typescript
interface ListItemProps {
  item: { id: number; name: string; value: number };
  onClick: (id: number) => void;
}

// Memoize component to prevent unnecessary re-renders
const ListItem = React.memo<ListItemProps>(
  ({ item, onClick }) => {
    console.log(`Rendering item ${item.id}`);
    
    return (
      <div onClick={() => onClick(item.id)}>
        <h3>{item.name}</h3>
        <p>{item.value}</p>
      </div>
    );
  },
  // Custom comparison function
  (prevProps, nextProps) => {
    return (
      prevProps.item.id === nextProps.item.id &&
      prevProps.item.name === nextProps.item.name &&
      prevProps.item.value === nextProps.item.value
    );
  }
);
```

#### useCallback and useMemo Hooks
```typescript
function ExpensiveComponent({ data }: { data: number[] }) {
  // Memoize expensive calculations
  const sortedData = useMemo(() => {
    console.log('Sorting data...');
    return [...data].sort((a, b) => b - a);
  }, [data]);

  const average = useMemo(() => {
    console.log('Calculating average...');
    return data.reduce((sum, n) => sum + n, 0) / data.length;
  }, [data]);

  // Memoize callbacks to prevent child re-renders
  const handleItemClick = useCallback((index: number) => {
    console.log(`Item ${index} clicked`);
  }, []);

  return (
    <div>
      <h2>Average: {average.toFixed(2)}</h2>
      <ul>
        {sortedData.map((item, index) => (
          <ListItem 
            key={index} 
            value={item} 
            onClick={() => handleItemClick(index)} 
          />
        ))}
      </ul>
    </div>
  );
}
```

### 3. Virtual Scrolling for Large Lists

```typescript
import { FixedSizeList, VariableSizeList } from 'react-window';
import AutoSizer from 'react-virtualized-auto-sizer';

// Fixed height items
function VirtualList({ items }: { items: any[] }) {
  const Row = ({ index, style }: { index: number; style: React.CSSProperties }) => (
    <div style={style}>
      <ListItem item={items[index]} />
    </div>
  );

  return (
    <AutoSizer>
      {({ height, width }) => (
        <FixedSizeList
          height={height}
          itemCount={items.length}
          itemSize={50}
          width={width}
        >
          {Row}
        </FixedSizeList>
      )}
    </AutoSizer>
  );
}

// Variable height items with caching
function DynamicVirtualList({ items }: { items: any[] }) {
  const rowHeights = useRef<{ [key: number]: number }>({});

  const getItemSize = (index: number) => {
    return rowHeights.current[index] || 50;
  };

  const Row = ({ index, style }: { index: number; style: React.CSSProperties }) => {
    const rowRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
      if (rowRef.current) {
        const height = rowRef.current.getBoundingClientRect().height;
        rowHeights.current[index] = height;
      }
    }, [index]);

    return (
      <div ref={rowRef} style={style}>
        <ListItem item={items[index]} />
      </div>
    );
  };

  return (
    <VariableSizeList
      height={600}
      itemCount={items.length}
      itemSize={getItemSize}
      width="100%"
    >
      {Row}
    </VariableSizeList>
  );
}
```

### 4. Image Optimization

#### Lazy Loading Images
```typescript
interface LazyImageProps {
  src: string;
  alt: string;
  placeholder?: string;
}

function LazyImage({ src, alt, placeholder }: LazyImageProps) {
  const [imageSrc, setImageSrc] = useState(placeholder || '');
  const [imageRef, setImageRef] = useState<HTMLImageElement | null>(null);

  useEffect(() => {
    let observer: IntersectionObserver;
    
    if (imageRef && imageSrc !== src) {
      observer = new IntersectionObserver(
        entries => {
          entries.forEach(entry => {
            if (entry.isIntersecting) {
              setImageSrc(src);
              observer.unobserve(imageRef);
            }
          });
        },
        { threshold: 0.1 }
      );
      observer.observe(imageRef);
    }
    
    return () => {
      if (observer) observer.disconnect();
    };
  }, [imageRef, imageSrc, src]);

  return (
    <img
      ref={setImageRef}
      src={imageSrc}
      alt={alt}
      loading="lazy"
      onError={() => setImageSrc('/fallback-image.png')}
    />
  );
}
```

#### Progressive Image Loading
```typescript
function ProgressiveImage({ src, placeholder, alt }: {
  src: string;
  placeholder: string;
  alt: string;
}) {
  const [currentSrc, setCurrentSrc] = useState(placeholder);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const img = new Image();
    img.src = src;
    img.onload = () => {
      setCurrentSrc(src);
      setLoading(false);
    };
  }, [src]);

  return (
    <div className={`progressive-image ${loading ? 'loading' : 'loaded'}`}>
      <img src={currentSrc} alt={alt} />
    </div>
  );
}
```

### 5. State Management Optimization

#### Normalized State Structure
```typescript
// Instead of nested data
interface BadState {
  posts: Array<{
    id: number;
    title: string;
    author: {
      id: number;
      name: string;
    };
    comments: Array<{
      id: number;
      text: string;
    }>;
  }>;
}

// Use normalized structure
interface GoodState {
  posts: {
    byId: { [id: number]: Post };
    allIds: number[];
  };
  authors: {
    byId: { [id: number]: Author };
  };
  comments: {
    byId: { [id: number]: Comment };
    byPostId: { [postId: number]: number[] };
  };
}

// Selector with memoization
const selectPostWithAuthor = createSelector(
  [(state: State) => state.posts.byId, 
   (state: State) => state.authors.byId,
   (state: State, postId: number) => postId],
  (posts, authors, postId) => {
    const post = posts[postId];
    return {
      ...post,
      author: authors[post.authorId]
    };
  }
);
```

### 6. API Call Optimization

#### Request Deduplication
```typescript
class ApiCache {
  private cache = new Map<string, Promise<any>>();
  private timestamps = new Map<string, number>();
  private ttl = 5 * 60 * 1000; // 5 minutes

  async get<T>(key: string, fetcher: () => Promise<T>): Promise<T> {
    const now = Date.now();
    const timestamp = this.timestamps.get(key);

    // Check if cache is still valid
    if (timestamp && now - timestamp < this.ttl) {
      const cached = this.cache.get(key);
      if (cached) return cached;
    }

    // Deduplicate in-flight requests
    const existing = this.cache.get(key);
    if (existing) return existing;

    // Make new request
    const promise = fetcher();
    this.cache.set(key, promise);
    this.timestamps.set(key, now);

    try {
      return await promise;
    } catch (error) {
      // Remove failed requests from cache
      this.cache.delete(key);
      this.timestamps.delete(key);
      throw error;
    }
  }

  invalidate(pattern?: string) {
    if (pattern) {
      const regex = new RegExp(pattern);
      for (const key of this.cache.keys()) {
        if (regex.test(key)) {
          this.cache.delete(key);
          this.timestamps.delete(key);
        }
      }
    } else {
      this.cache.clear();
      this.timestamps.clear();
    }
  }
}

const apiCache = new ApiCache();

// Usage
function useUser(userId: number) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    apiCache.get(`user-${userId}`, () => api.getUser(userId))
      .then(setUser)
      .finally(() => setLoading(false));
  }, [userId]);

  return { user, loading };
}
```

#### Batch API Requests
```typescript
class BatchRequestManager {
  private batchQueue: Map<string, Set<any>> = new Map();
  private batchTimeout: NodeJS.Timeout | null = null;
  private batchDelay = 10; // ms

  async add<T>(
    endpoint: string,
    id: any,
    resolver: (ids: any[]) => Promise<Map<any, T>>
  ): Promise<T> {
    return new Promise((resolve, reject) => {
      // Add to batch queue
      if (!this.batchQueue.has(endpoint)) {
        this.batchQueue.set(endpoint, new Set());
      }
      
      const batch = this.batchQueue.get(endpoint)!;
      batch.add({ id, resolve, reject });

      // Schedule batch execution
      if (this.batchTimeout) {
        clearTimeout(this.batchTimeout);
      }

      this.batchTimeout = setTimeout(() => {
        this.executeBatch(endpoint, resolver);
      }, this.batchDelay);
    });
  }

  private async executeBatch<T>(
    endpoint: string,
    resolver: (ids: any[]) => Promise<Map<any, T>>
  ) {
    const batch = this.batchQueue.get(endpoint);
    if (!batch || batch.size === 0) return;

    this.batchQueue.delete(endpoint);

    const items = Array.from(batch);
    const ids = items.map(item => item.id);

    try {
      const results = await resolver(ids);
      
      items.forEach(({ id, resolve, reject }) => {
        const result = results.get(id);
        if (result) {
          resolve(result);
        } else {
          reject(new Error(`No result for ID ${id}`));
        }
      });
    } catch (error) {
      items.forEach(({ reject }) => reject(error));
    }
  }
}

// Usage
const batchManager = new BatchRequestManager();

function useUsers(userIds: number[]) {
  const [users, setUsers] = useState<Map<number, User>>(new Map());

  useEffect(() => {
    const fetchUsers = async () => {
      const promises = userIds.map(id =>
        batchManager.add('users', id, async (ids) => {
          const response = await api.post('/users/batch', { ids });
          return new Map(response.data.map(user => [user.id, user]));
        })
      );

      const results = await Promise.all(promises);
      const userMap = new Map(results.map((user, index) => [userIds[index], user]));
      setUsers(userMap);
    };

    fetchUsers();
  }, [userIds]);

  return users;
}
```

## Server-Side (ASP.NET Core) Optimizations

### 1. Response Caching

```csharp
// Program.cs
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// Configure cache profiles
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default300",
        new CacheProfile()
        {
            Duration = 300,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "v" }
        });
});

app.UseResponseCaching();

// Controller usage
[HttpGet]
[ResponseCache(CacheProfileName = "Default300")]
public async Task<IActionResult> GetProducts([FromQuery] int? categoryId)
{
    var products = await _productService.GetProductsAsync(categoryId);
    return Ok(products);
}

// Dynamic cache
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(int id)
{
    // Check memory cache first
    if (!_memoryCache.TryGetValue($"product_{id}", out Product product))
    {
        product = await _productService.GetProductAsync(id);
        
        // Set cache options
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
            .SetPriority(CacheItemPriority.Normal);
        
        _memoryCache.Set($"product_{id}", product, cacheOptions);
    }
    
    return Ok(product);
}
```

### 2. Database Query Optimization

```csharp
// Use projection to select only needed fields
public async Task<IEnumerable<ProductDto>> GetProductsOptimized()
{
    return await _context.Products
        .Where(p => p.IsActive)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
            // Don't include heavy fields like Description
        })
        .AsNoTracking() // No change tracking for read-only queries
        .ToListAsync();
}

// Use compiled queries for frequently used queries
private static readonly Func<AppDbContext, int, Task<Product>> GetProductByIdQuery =
    EF.CompileAsyncQuery((AppDbContext context, int id) =>
        context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id));

public Task<Product> GetProductById(int id)
{
    return GetProductByIdQuery(_context, id);
}

// Batch loading to avoid N+1 queries
public async Task<IEnumerable<OrderDto>> GetOrdersWithItems()
{
    var orders = await _context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .AsSplitQuery() // Split into multiple queries for better performance
        .ToListAsync();
    
    return orders.Select(o => new OrderDto
    {
        Id = o.Id,
        Items = o.OrderItems.Select(oi => new OrderItemDto
        {
            ProductName = oi.Product.Name,
            Quantity = oi.Quantity
        })
    });
}
```

### 3. Compression

```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "application/javascript", "text/css" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

// Use before static files
app.UseResponseCompression();
```

### 4. Asynchronous Processing

```csharp
// Use async/await throughout
public async Task<IActionResult> ProcessDataAsync()
{
    // Parallel processing for independent operations
    var task1 = _service1.GetDataAsync();
    var task2 = _service2.GetDataAsync();
    var task3 = _service3.GetDataAsync();
    
    await Task.WhenAll(task1, task2, task3);
    
    var result = new
    {
        Data1 = task1.Result,
        Data2 = task2.Result,
        Data3 = task3.Result
    };
    
    return Ok(result);
}

// Background processing for long operations
public class BackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public BackgroundTaskQueue(int capacity)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(
        Func<CancellationToken, ValueTask> workItem)
    {
        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}
```

### 5. Static File Optimization

```csharp
// Configure static file caching
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 year
        const int durationInSeconds = 365 * 24 * 60 * 60;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            $"public,max-age={durationInSeconds}";
    }
});

// Serve pre-compressed files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "dist")),
    RequestPath = "/dist",
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.Name.EndsWith(".js.gz"))
        {
            ctx.Context.Response.Headers[HeaderNames.ContentType] = "application/javascript";
            ctx.Context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
        }
    }
});
```

## Monitoring and Profiling

### React Performance Profiling

```typescript
// Use React DevTools Profiler
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
  // Log render performance
  console.log({
    componentId: id,
    phase,
    actualDuration,
    baseDuration
  });
}

<Profiler id="TodoList" onRender={onRenderCallback}>
  <TodoList items={todos} />
</Profiler>
```

### ASP.NET Core Performance Monitoring

```csharp
// Add performance counters
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            if (elapsedMs > 500) // Log slow requests
            {
                _logger.LogWarning(
                    "Slow request: {Method} {Path} took {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs);
            }
            
            // Add timing header
            context.Response.Headers.Add("X-Response-Time", $"{elapsedMs}ms");
        }
    }
}
```

## Bundle Size Optimization

### Webpack Configuration (React)

```javascript
// webpack.config.js
module.exports = {
  optimization: {
    splitChunks: {
      chunks: 'all',
      cacheGroups: {
        vendor: {
          test: /[\\/]node_modules[\\/]/,
          name: 'vendors',
          priority: 10
        },
        common: {
          minChunks: 2,
          priority: 5,
          reuseExistingChunk: true
        }
      }
    },
    usedExports: true,
    minimize: true,
    sideEffects: false
  }
};
```

### Analyze Bundle Size

```json
// package.json
{
  "scripts": {
    "analyze": "source-map-explorer 'build/static/js/*.js'",
    "build:analyze": "npm run build && npm run analyze"
  }
}
```

## Performance Checklist

### React Checklist
- [ ] Implement code splitting for routes
- [ ] Use React.memo for expensive components
- [ ] Implement virtual scrolling for large lists
- [ ] Optimize images with lazy loading
- [ ] Use production builds
- [ ] Enable source map exploration
- [ ] Implement proper error boundaries
- [ ] Use Web Workers for heavy computations
- [ ] Minimize bundle size
- [ ] Use CDN for static assets

### ASP.NET Core Checklist
- [ ] Enable response caching
- [ ] Implement memory caching
- [ ] Use async/await throughout
- [ ] Enable response compression
- [ ] Optimize database queries
- [ ] Use projection for DTOs
- [ ] Implement pagination
- [ ] Use background services for long tasks
- [ ] Enable HTTP/2
- [ ] Monitor performance metrics

---

**Remember**: Always measure before and after optimization. Not all optimizations are necessary for every application. Focus on the bottlenecks identified through profiling!