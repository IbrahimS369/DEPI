using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer("Server=.;Database=LibraryMvcDb;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddScoped<IBookRepository, BookRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<LibraryContext>().Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public List<Book> Books { get; set; } = new();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    public List<Loan> Loans { get; set; } = new();
}

public class Borrower
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
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

public class BookViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN is required")]
    [RegularExpression(@"^(\d{10}|\d{13}|\d{3}-\d{10})$", ErrorMessage = "Invalid ISBN")]
    public string ISBN { get; set; } = string.Empty;

    [Display(Name = "Author")]
    [Range(1, int.MaxValue, ErrorMessage = "Select an author")]
    public int AuthorId { get; set; }

    public string? AuthorName { get; set; }
    public IEnumerable<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
}

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

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(string? search);
    Task<Book?> GetByIdAsync(int id);
    Task<List<Author>> GetAuthorsAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
}

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync(string? search)
    {
        var query = _context.Books.Include(b => b.Author).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.ISBN.Contains(search));
        }
        return await query.OrderBy(b => b.Title).ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Author>> GetAuthorsAsync()
    {
        return await _context.Authors.OrderBy(a => a.Name).ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}

public class BooksController : Controller
{
    private readonly IBookRepository _repository;

    public BooksController(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var books = await _repository.GetAllAsync(search);
        var model = books.Select(b => new BookViewModel
        {
            Id = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            AuthorId = b.AuthorId,
            AuthorName = b.Author?.Name
        }).ToList();
        ViewData["Search"] = search;
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book == null) return NotFound();
        return View(new BookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.Name
        });
    }

    public async Task<IActionResult> Create()
    {
        var model = new BookViewModel { Authors = await LoadAuthors() };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Authors = await LoadAuthors();
            return View(model);
        }

        await _repository.AddAsync(new Book { Title = model.Title, ISBN = model.ISBN, AuthorId = model.AuthorId });
        TempData["Message"] = "Book created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book == null) return NotFound();
        return View(new BookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            AuthorId = book.AuthorId,
            Authors = await LoadAuthors()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            model.Authors = await LoadAuthors();
            return View(model);
        }

        await _repository.UpdateAsync(new Book { Id = model.Id, Title = model.Title, ISBN = model.ISBN, AuthorId = model.AuthorId });
        TempData["Message"] = "Book updated";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book == null) return NotFound();
        return View(new BookViewModel { Id = book.Id, Title = book.Title, ISBN = book.ISBN, AuthorName = book.Author?.Name });
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);
        TempData["Message"] = "Book deleted";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> LoadAuthors()
    {
        var authors = await _repository.GetAuthorsAsync();
        return authors.Select(a => new SelectListItem(a.Name, a.Id.ToString()));
    }
}
