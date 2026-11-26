using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ECommerce
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new();
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }

    public class OrderDetail
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class ECommerceContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(c => c.Email).IsRequired().HasMaxLength(150);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId);
        }
    }
}

namespace Library
{
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
        public Author Author { get; set; } = null!;
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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().Property(a => a.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Book>().Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Borrower>().Property(b => b.Name).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>().HasKey(l => new { l.BookId, l.BorrowerId, l.LoanDate });

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Borrower)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BorrowerId);
        }
    }
}

namespace HealthCare
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public List<Appointment> Appointments { get; set; } = new();
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public List<Appointment> Appointments { get; set; } = new();
    }

    public class Appointment
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
    }

    public class HealthCareContext : DbContext
    {
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=HealthCareDb;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Doctor>().Property(d => d.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Doctor>().Property(d => d.Specialization).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Appointment>().HasKey(a => new { a.PatientId, a.DoctorId, a.AppointmentDate });

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId);
        }
    }
}

namespace Assignment12
{
    public class Program
    {
        static void RunECommerce()
        {
            using var db = new ECommerce.ECommerceContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var electronics = new ECommerce.Category { Name = "Electronics" };
            var books = new ECommerce.Category { Name = "Books" };
            var laptop = new ECommerce.Product { Name = "Laptop", Price = 25000m, Category = electronics };
            var phone = new ECommerce.Product { Name = "Phone", Price = 12000m, Category = electronics };
            var novel = new ECommerce.Product { Name = "Novel", Price = 150m, Category = books };
            var customer = new ECommerce.Customer { Name = "Ahmed", Email = "ahmed@example.com" };
            var order = new ECommerce.Order { OrderDate = DateTime.Now, Customer = customer };
            order.OrderDetails.Add(new ECommerce.OrderDetail { Product = laptop, Quantity = 1 });
            order.OrderDetails.Add(new ECommerce.OrderDetail { Product = novel, Quantity = 3 });

            db.Categories.AddRange(electronics, books);
            db.Products.Add(phone);
            db.Orders.Add(order);
            db.SaveChanges();

            Console.WriteLine("Categories and their products (One-to-Many):");
            foreach (var c in db.Categories.Include(c => c.Products))
                Console.WriteLine($"{c.Name}: {string.Join(", ", c.Products.Select(p => p.Name))}");

            Console.WriteLine("Orders with customer and products (Many-to-Many through OrderDetail):");
            var orders = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .ToList();
            foreach (var o in orders)
            {
                Console.WriteLine($"Order {o.Id} by {o.Customer.Name} on {o.OrderDate:d}");
                foreach (var od in o.OrderDetails)
                    Console.WriteLine($"  {od.Product.Name} x{od.Quantity} = {od.Product.Price * od.Quantity}");
            }
        }

        static void RunLibrary()
        {
            using var db = new Library.LibraryContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var author = new Library.Author { Name = "Naguib Mahfouz", BirthDate = new DateTime(1911, 12, 11) };
            var book1 = new Library.Book { Title = "Palace Walk", ISBN = "9780385264662", Author = author };
            var book2 = new Library.Book { Title = "Midaq Alley", ISBN = "9780385264761", Author = author };
            var borrower = new Library.Borrower { Name = "Sara", MembershipDate = new DateTime(2024, 1, 15) };

            db.Authors.Add(author);
            db.Borrowers.Add(borrower);
            db.SaveChanges();

            db.Loans.Add(new Library.Loan { Book = book1, Borrower = borrower, LoanDate = DateTime.Today });
            db.Loans.Add(new Library.Loan { Book = book2, Borrower = borrower, LoanDate = DateTime.Today, ReturnDate = DateTime.Today.AddDays(7) });
            db.SaveChanges();

            Console.WriteLine("Authors and their books (One-to-Many):");
            foreach (var a in db.Authors.Include(a => a.Books))
                Console.WriteLine($"{a.Name}: {string.Join(", ", a.Books.Select(b => b.Title))}");

            Console.WriteLine("Loans (Many-to-Many through Loan):");
            var loans = db.Loans.Include(l => l.Book).Include(l => l.Borrower).ToList();
            foreach (var l in loans)
                Console.WriteLine($"{l.Borrower.Name} borrowed {l.Book.Title} on {l.LoanDate:d}, returned: {(l.ReturnDate.HasValue ? l.ReturnDate.Value.ToString("d") : "not yet")}");
        }

        static void RunHealthCare()
        {
            using var db = new HealthCare.HealthCareContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var patient1 = new HealthCare.Patient { Name = "Mona", DateOfBirth = new DateTime(1990, 5, 20) };
            var patient2 = new HealthCare.Patient { Name = "Omar", DateOfBirth = new DateTime(1985, 3, 2) };
            var doctor1 = new HealthCare.Doctor { Name = "Dr. Khaled", Specialization = "Cardiology" };
            var doctor2 = new HealthCare.Doctor { Name = "Dr. Laila", Specialization = "Dermatology" };

            db.Patients.AddRange(patient1, patient2);
            db.Doctors.AddRange(doctor1, doctor2);
            db.SaveChanges();

            db.Appointments.Add(new HealthCare.Appointment { Patient = patient1, Doctor = doctor1, AppointmentDate = DateTime.Today.AddDays(1) });
            db.Appointments.Add(new HealthCare.Appointment { Patient = patient1, Doctor = doctor2, AppointmentDate = DateTime.Today.AddDays(2) });
            db.Appointments.Add(new HealthCare.Appointment { Patient = patient2, Doctor = doctor1, AppointmentDate = DateTime.Today.AddDays(3) });
            db.SaveChanges();

            Console.WriteLine("Appointments (Many-to-Many through Appointment):");
            var appointments = db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).ToList();
            foreach (var a in appointments)
                Console.WriteLine($"{a.Patient.Name} with {a.Doctor.Name} ({a.Doctor.Specialization}) on {a.AppointmentDate:d}");

            Console.WriteLine("Doctors and their patients:");
            foreach (var d in db.Doctors.Include(d => d.Appointments).ThenInclude(a => a.Patient))
                Console.WriteLine($"{d.Name}: {string.Join(", ", d.Appointments.Select(a => a.Patient.Name))}");
        }

        static void PrintNavigationNotes()
        {
            Console.WriteLine("Navigation property: a property on an entity that points to a related entity (reference) or to a collection of related entities.");
            Console.WriteLine("Reference navigation: Product.Category. Collection navigation: Category.Products.");
            Console.WriteLine("It lets EF Core load and traverse related data with Include and ThenInclude instead of writing joins by hand.");
            Console.WriteLine("The foreign key property (CategoryId) holds the value in the database; the navigation property is the object-level link.");
            Console.WriteLine("Many-to-Many here uses an explicit join entity (OrderDetail, Loan, Appointment) with a composite key and two One-to-Many relationships.");
        }

        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("1) E-commerce  2) Library  3) Health Care  4) Navigation property notes  0) Exit");
                string choice = Console.ReadLine() ?? "0";
                switch (choice)
                {
                    case "1": RunECommerce(); break;
                    case "2": RunLibrary(); break;
                    case "3": RunHealthCare(); break;
                    case "4": PrintNavigationNotes(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }
    }
}
