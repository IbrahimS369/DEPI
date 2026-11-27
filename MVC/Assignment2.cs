using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer("Server=.;Database=LibraryMvcDb;Trusted_Connection=True;TrustServerCertificate=True"));

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
    pattern: "{controller=Loans}/{action=Index}/{id?}");

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

public class LoanViewModel
{
    [Display(Name = "Book")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a book")]
    public int BookId { get; set; }

    [Display(Name = "Borrower")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a borrower")]
    public int BorrowerId { get; set; }

    public string? BookTitle { get; set; }
    public string? BorrowerName { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public IEnumerable<SelectListItem> Books { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Borrowers { get; set; } = new List<SelectListItem>();
}

public class AuthorViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Birth date")]
    public DateTime BirthDate { get; set; }

    public int BooksCount { get; set; }
}

public class BorrowerViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Membership date")]
    public DateTime MembershipDate { get; set; }
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

public class AuthorsController : Controller
{
    private readonly LibraryContext _context;

    public AuthorsController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _context.Authors
            .Select(a => new AuthorViewModel { Id = a.Id, Name = a.Name, BirthDate = a.BirthDate, BooksCount = a.Books.Count })
            .ToListAsync();
        return View(authors);
    }

    public IActionResult Create()
    {
        return View(new AuthorViewModel { BirthDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Authors.Add(new Author { Name = model.Name, BirthDate = model.BirthDate });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        return View(new AuthorViewModel { Id = author.Id, Name = author.Name, BirthDate = author.BirthDate });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        author.Name = model.Name;
        author.BirthDate = model.BirthDate;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
        if (author == null) return NotFound();
        if (author.Books.Any())
        {
            TempData["Error"] = "Cannot delete an author who has books";
            return RedirectToAction(nameof(Index));
        }
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

public class BorrowersController : Controller
{
    private readonly LibraryContext _context;

    public BorrowersController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var borrowers = await _context.Borrowers
            .Select(b => new BorrowerViewModel { Id = b.Id, Name = b.Name, MembershipDate = b.MembershipDate })
            .ToListAsync();
        return View(borrowers);
    }

    public IActionResult Create()
    {
        return View(new BorrowerViewModel { MembershipDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BorrowerViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Borrowers.Add(new Borrower { Name = model.Name, MembershipDate = model.MembershipDate });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var borrower = await _context.Borrowers.FindAsync(id);
        if (borrower == null) return NotFound();
        return View(new BorrowerViewModel { Id = borrower.Id, Name = borrower.Name, MembershipDate = borrower.MembershipDate });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BorrowerViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        var borrower = await _context.Borrowers.FindAsync(id);
        if (borrower == null) return NotFound();
        borrower.Name = model.Name;
        borrower.MembershipDate = model.MembershipDate;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var borrower = await _context.Borrowers.FindAsync(id);
        if (borrower == null) return NotFound();
        _context.Borrowers.Remove(borrower);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

public class LoansController : Controller
{
    private readonly LibraryContext _context;

    public LoansController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Borrower)
            .OrderByDescending(l => l.LoanDate)
            .Select(l => new LoanViewModel
            {
                BookId = l.BookId,
                BorrowerId = l.BorrowerId,
                BookTitle = l.Book!.Title,
                BorrowerName = l.Borrower!.Name,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate
            })
            .ToListAsync();
        return View(loans);
    }

    public async Task<IActionResult> Create()
    {
        var model = new LoanViewModel();
        await FillLists(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoanViewModel model)
    {
        if (ModelState.IsValid)
        {
            var onLoan = await _context.Loans.AnyAsync(l => l.BookId == model.BookId && l.ReturnDate == null);
            if (onLoan)
            {
                ModelState.AddModelError(nameof(model.BookId), "This book is already on loan");
            }

            var exists = await _context.Loans.AnyAsync(l => l.BookId == model.BookId && l.BorrowerId == model.BorrowerId);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.BorrowerId), "This borrower already has a loan record for this book");
            }
        }

        if (!ModelState.IsValid)
        {
            await FillLists(model);
            return View(model);
        }

        _context.Loans.Add(new Loan { BookId = model.BookId, BorrowerId = model.BorrowerId, LoanDate = DateTime.UtcNow });
        await _context.SaveChangesAsync();
        TempData["Message"] = "Loan created";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int bookId, int borrowerId)
    {
        var loan = await _context.Loans.FindAsync(bookId, borrowerId);
        if (loan == null) return NotFound();
        if (loan.ReturnDate == null)
        {
            loan.ReturnDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task FillLists(LoanViewModel model)
    {
        model.Books = await _context.Books
            .OrderBy(b => b.Title)
            .Select(b => new SelectListItem(b.Title, b.Id.ToString()))
            .ToListAsync();
        model.Borrowers = await _context.Borrowers
            .OrderBy(b => b.Name)
            .Select(b => new SelectListItem(b.Name, b.Id.ToString()))
            .ToListAsync();
    }
}
