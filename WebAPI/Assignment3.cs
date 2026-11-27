using System.Diagnostics;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<AuditInterceptor>();
builder.Services.AddSingleton<SlowQueryInterceptor>();
builder.Services.AddDbContext<LibraryContext>((sp, options) =>
    options.UseSqlServer("Server=.;Database=LibraryMiddlewareDb;Trusted_Connection=True;TrustServerCertificate=True")
        .AddInterceptors(sp.GetRequiredService<AuditInterceptor>(), sp.GetRequiredService<SlowQueryInterceptor>()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<LibraryContext>().Database.EnsureCreated();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

public class Author : IAuditable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Book> Books { get; set; } = new();
}

public class Book : IAuditable
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Loan> Loans { get; set; } = new();
}

public class Borrower : IAuditable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Loan> Loans { get; set; } = new();
}

public class Loan
{
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int BorrowerId { get; set; }
    public Borrower? Borrower { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}

public record BookDto(int Id, string Title, string ISBN, int AuthorId, string AuthorName);
public record BookCreateDto(string Title, string ISBN, int AuthorId);

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrower> Borrowers => Set<Borrower>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().Property(a => a.Name).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Book>(e =>
        {
            e.Property(b => b.Title).IsRequired().HasMaxLength(200);
            e.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            e.HasOne(b => b.Author).WithMany(a => a.Books).HasForeignKey(b => b.AuthorId);
        });
        modelBuilder.Entity<Borrower>().Property(b => b.Name).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Loan>(e =>
        {
            e.HasKey(l => new { l.BookId, l.BorrowerId });
            e.HasOne(l => l.Book).WithMany(b => b.Loans).HasForeignKey(l => l.BookId);
            e.HasOne(l => l.Borrower).WithMany(b => b.Loans).HasForeignKey(l => l.BorrowerId);
        });
    }
}

public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void Apply(DbContext? context)
    {
        if (context == null) return;
        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

public class SlowQueryInterceptor : DbCommandInterceptor
{
    private readonly ILogger<SlowQueryInterceptor> _logger;

    public SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger)
    {
        _logger = logger;
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Check(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        Check(command, eventData);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void Check(DbCommand command, CommandExecutedEventData eventData)
    {
        if (eventData.Duration.TotalMilliseconds > 500)
        {
            _logger.LogWarning("Slow query ({Ms} ms): {Sql}", eventData.Duration.TotalMilliseconds, command.CommandText);
        }
    }
}

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
        await _next(context);
        watch.Stop();
        _logger.LogInformation("Response: {Status} in {Ms} ms", context.Response.StatusCode, watch.ElapsedMilliseconds);
    }
}

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred", traceId = context.TraceIdentifier });
        }
    }
}

public class ApiKeyMiddleware
{
    private const string HeaderName = "X-Api-Key";
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var expected = _configuration["ApiKey"] ?? "library-secret-key";
        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) || provided != expected)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing API key" });
            return;
        }

        await _next(context);
    }
}

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibraryContext _context;

    public BooksController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var books = await _context.Books
            .Select(b => new BookDto(b.Id, b.Title, b.ISBN, b.AuthorId, b.Author!.Name))
            .ToListAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null) return NotFound();
        return Ok(new BookDto(book.Id, book.Title, book.ISBN, book.AuthorId, book.Author!.Name));
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(BookCreateDto dto)
    {
        var author = await _context.Authors.FindAsync(dto.AuthorId);
        if (author == null) return BadRequest("Author not found");
        var book = new Book { Title = dto.Title, ISBN = dto.ISBN, AuthorId = dto.AuthorId };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, new BookDto(book.Id, book.Title, book.ISBN, book.AuthorId, author.Name));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BookCreateDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        book.Title = dto.Title;
        book.ISBN = dto.ISBN;
        book.AuthorId = dto.AuthorId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("error")]
    public IActionResult ThrowError()
    {
        throw new InvalidOperationException("Test exception");
    }
}
