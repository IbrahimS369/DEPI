using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer("Server=.;Database=LibraryApiDb;Trusted_Connection=True;TrustServerCertificate=True"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<LibraryContext>().Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
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

public record AuthorDto(int Id, string Name, DateTime BirthDate);
public record AuthorCreateDto(string Name, DateTime BirthDate);
public record BookDto(int Id, string Title, string ISBN, int AuthorId, string AuthorName);
public record BookCreateDto(string Title, string ISBN, int AuthorId);
public record BorrowerDto(int Id, string Name, DateTime MembershipDate);
public record BorrowerCreateDto(string Name, DateTime MembershipDate);
public record LoanDto(int BookId, string BookTitle, int BorrowerId, string BorrowerName, DateTime LoanDate, DateTime? ReturnDate);
public record LoanCreateDto(int BookId, int BorrowerId);

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

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly LibraryContext _context;

    public AuthorsController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll()
    {
        var authors = await _context.Authors
            .Select(a => new AuthorDto(a.Id, a.Name, a.BirthDate))
            .ToListAsync();
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        return Ok(new AuthorDto(author.Id, author.Name, author.BirthDate));
    }

    [HttpGet("{id}/books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(int id)
    {
        if (!await _context.Authors.AnyAsync(a => a.Id == id)) return NotFound();
        var books = await _context.Books
            .Where(b => b.AuthorId == id)
            .Select(b => new BookDto(b.Id, b.Title, b.ISBN, b.AuthorId, b.Author!.Name))
            .ToListAsync();
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create(AuthorCreateDto dto)
    {
        var author = new Author { Name = dto.Name, BirthDate = dto.BirthDate };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, new AuthorDto(author.Id, author.Name, author.BirthDate));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AuthorCreateDto dto)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        author.Name = dto.Name;
        author.BirthDate = dto.BirthDate;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return NoContent();
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
            .Include(b => b.Author)
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
        if (!await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId)) return BadRequest("Author not found");
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
}

[ApiController]
[Route("api/[controller]")]
public class BorrowersController : ControllerBase
{
    private readonly LibraryContext _context;

    public BorrowersController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BorrowerDto>>> GetAll()
    {
        var borrowers = await _context.Borrowers
            .Select(b => new BorrowerDto(b.Id, b.Name, b.MembershipDate))
            .ToListAsync();
        return Ok(borrowers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BorrowerDto>> GetById(int id)
    {
        var borrower = await _context.Borrowers.FindAsync(id);
        if (borrower == null) return NotFound();
        return Ok(new BorrowerDto(borrower.Id, borrower.Name, borrower.MembershipDate));
    }

    [HttpGet("{id}/loans")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans(int id)
    {
        if (!await _context.Borrowers.AnyAsync(b => b.Id == id)) return NotFound();
        var loans = await _context.Loans
            .Where(l => l.BorrowerId == id)
            .Select(l => new LoanDto(l.BookId, l.Book!.Title, l.BorrowerId, l.Borrower!.Name, l.LoanDate, l.ReturnDate))
            .ToListAsync();
        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<BorrowerDto>> Create(BorrowerCreateDto dto)
    {
        var borrower = new Borrower { Name = dto.Name, MembershipDate = dto.MembershipDate };
        _context.Borrowers.Add(borrower);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = borrower.Id }, new BorrowerDto(borrower.Id, borrower.Name, borrower.MembershipDate));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BorrowerCreateDto dto)
    {
        var borrower = await _context.Borrowers.FindAsync(id);
        if (borrower == null) return NotFound();
        borrower.Name = dto.Name;
        borrower.MembershipDate = dto.MembershipDate;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var borrower = await _context.Borrowers.FindAsync(id);
        if (borrower == null) return NotFound();
        _context.Borrowers.Remove(borrower);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LibraryContext _context;

    public LoansController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
    {
        var loans = await _context.Loans
            .Select(l => new LoanDto(l.BookId, l.Book!.Title, l.BorrowerId, l.Borrower!.Name, l.LoanDate, l.ReturnDate))
            .ToListAsync();
        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Borrow(LoanCreateDto dto)
    {
        var book = await _context.Books.FindAsync(dto.BookId);
        var borrower = await _context.Borrowers.FindAsync(dto.BorrowerId);
        if (book == null || borrower == null) return BadRequest("Book or borrower not found");

        var alreadyOut = await _context.Loans.AnyAsync(l => l.BookId == dto.BookId && l.ReturnDate == null);
        if (alreadyOut) return Conflict("Book is already on loan");

        var existing = await _context.Loans.FindAsync(dto.BookId, dto.BorrowerId);
        if (existing != null) return Conflict("Loan already exists for this book and borrower");

        var loan = new Loan { BookId = dto.BookId, BorrowerId = dto.BorrowerId, LoanDate = DateTime.UtcNow };
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return Ok(new LoanDto(book.Id, book.Title, borrower.Id, borrower.Name, loan.LoanDate, loan.ReturnDate));
    }

    [HttpPut("return")]
    public async Task<IActionResult> Return(LoanCreateDto dto)
    {
        var loan = await _context.Loans.FindAsync(dto.BookId, dto.BorrowerId);
        if (loan == null) return NotFound();
        if (loan.ReturnDate != null) return BadRequest("Book already returned");
        loan.ReturnDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
