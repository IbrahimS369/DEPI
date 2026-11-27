using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LogActionFilter>();
});
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer("Server=.;Database=LibraryIdentityDb;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LibraryContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    context.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Member" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}

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

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

public class LibraryContext : IdentityDbContext<AppUser>
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrower> Borrowers => Set<Borrower>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;

    public LogActionFilter(ILogger<LogActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("User {User} executing {Action}", context.HttpContext.User.Identity?.Name ?? "anonymous", context.ActionDescriptor.DisplayName);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new AppUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, "Member");
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Books");
    }

    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Books");
        }

        ModelState.AddModelError(string.Empty, result.IsLockedOut ? "Account locked, try again later" : "Invalid email or password");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
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

[Authorize]
public class BooksController : Controller
{
    private readonly IBookRepository _repository;

    public BooksController(IBookRepository repository)
    {
        _repository = repository;
    }

    [AllowAnonymous]
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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var model = new BookViewModel { Authors = await LoadAuthors() };
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book == null) return NotFound();
        return View(new BookViewModel { Id = book.Id, Title = book.Title, ISBN = book.ISBN, AuthorName = book.Author?.Name });
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
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
