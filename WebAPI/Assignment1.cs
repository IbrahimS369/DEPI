using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }

    public class Borrower
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }

    public class Loan
    {
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int BorrowerId { get; set; }
        public Borrower Borrower { get; set; } = null!;
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class LibraryContext : DbContext
    {
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Borrower> Borrowers => Set<Borrower>();
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Book>(e =>
            {
                e.HasKey(b => b.Id);
                e.Property(b => b.Title).IsRequired().HasMaxLength(200);
                e.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
                e.HasOne(b => b.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Borrower>(e =>
            {
                e.HasKey(b => b.Id);
                e.Property(b => b.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Loan>(e =>
            {
                e.HasKey(l => new { l.BookId, l.BorrowerId });
                e.HasOne(l => l.Book)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(l => l.BookId);
                e.HasOne(l => l.Borrower)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(l => l.BorrowerId);
            });
        }
    }

    public class Program
    {
        public static void Main()
        {
            using var context = new LibraryContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var author1 = new Author { Name = "Naguib Mahfouz", BirthDate = new DateTime(1911, 12, 11) };
            var author2 = new Author { Name = "Taha Hussein", BirthDate = new DateTime(1889, 11, 15) };

            var book1 = new Book { Title = "Cairo Trilogy", ISBN = "978-0385264662", Author = author1 };
            var book2 = new Book { Title = "Midaq Alley", ISBN = "978-0385264778", Author = author1 };
            var book3 = new Book { Title = "The Days", ISBN = "978-9774160264", Author = author2 };

            var borrower1 = new Borrower { Name = "Ahmed Ali", MembershipDate = new DateTime(2023, 1, 10) };
            var borrower2 = new Borrower { Name = "Sara Hassan", MembershipDate = new DateTime(2024, 3, 5) };

            context.Authors.AddRange(author1, author2);
            context.Books.AddRange(book1, book2, book3);
            context.Borrowers.AddRange(borrower1, borrower2);

            context.Loans.AddRange(
                new Loan { Book = book1, Borrower = borrower1, LoanDate = DateTime.Today.AddDays(-14), ReturnDate = DateTime.Today.AddDays(-2) },
                new Loan { Book = book2, Borrower = borrower1, LoanDate = DateTime.Today.AddDays(-5) },
                new Loan { Book = book1, Borrower = borrower2, LoanDate = DateTime.Today.AddDays(-3) },
                new Loan { Book = book3, Borrower = borrower2, LoanDate = DateTime.Today.AddDays(-1) }
            );

            context.SaveChanges();

            var authors = context.Authors.Include(a => a.Books).ToList();
            foreach (var author in authors)
            {
                Console.WriteLine($"Author: {author.Name}");
                foreach (var book in author.Books)
                {
                    Console.WriteLine($"  Book: {book.Title} ({book.ISBN})");
                }
            }

            var loans = context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .ToList();
            foreach (var loan in loans)
            {
                var status = loan.ReturnDate.HasValue ? "Returned" : "Not returned";
                Console.WriteLine($"{loan.Borrower.Name} borrowed {loan.Book.Title} on {loan.LoanDate:yyyy-MM-dd} - {status}");
            }

            var borrowerBooks = context.Borrowers
                .Include(b => b.Loans)
                .ThenInclude(l => l.Book)
                .ToList();
            foreach (var borrower in borrowerBooks)
            {
                Console.WriteLine($"{borrower.Name} has {borrower.Loans.Count} loans");
            }

            var notReturned = context.Loans
                .Where(l => l.ReturnDate == null)
                .Select(l => new { l.Book.Title, l.Borrower.Name })
                .ToList();
            foreach (var item in notReturned)
            {
                Console.WriteLine($"Not returned: {item.Title} by {item.Name}");
            }
        }
    }
}
