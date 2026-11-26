using System;
using System.Collections.Generic;

public abstract class LibraryItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }

    public abstract void DisplayInfo();

    public void PrintBasicInfo()
    {
        Console.WriteLine("Id: " + Id + ", Title: " + Title + ", Year: " + Year);
    }
}

public partial class Book : LibraryItem, IComparable<Book>, ICloneable
{
    public string Author { get; set; }
    public int PageCount { get; set; }

    public Book()
    {
        Title = "";
        Author = "";
    }

    public Book(int id, string title, string author, int year, int pageCount)
    {
        Id = id;
        Title = title;
        Author = author;
        Year = year;
        PageCount = pageCount;
    }

    public int CompareTo(Book other)
    {
        if (other == null) return 1;
        int result = Year.CompareTo(other.Year);
        if (result == 0)
            result = string.Compare(Title, other.Title, StringComparison.Ordinal);
        return result;
    }

    public object Clone()
    {
        return new Book(Id, Title, Author, Year, PageCount);
    }

    public static bool operator >(Book a, Book b)
    {
        if (ReferenceEquals(a, null)) return false;
        if (ReferenceEquals(b, null)) return true;
        return a.PageCount > b.PageCount;
    }

    public static bool operator <(Book a, Book b)
    {
        if (ReferenceEquals(a, null)) return false;
        if (ReferenceEquals(b, null)) return true;
        return a.PageCount < b.PageCount;
    }

    public static Book operator +(Book a, Book b)
    {
        return new Book(
            0,
            a.Title + " & " + b.Title,
            a.Author + " & " + b.Author,
            Math.Max(a.Year, b.Year),
            a.PageCount + b.PageCount);
    }
}

public partial class Book
{
    public override void DisplayInfo()
    {
        Console.WriteLine("Book: " + Title + " by " + Author + " (" + Year + "), " + PageCount + " pages");
    }

    public void PrintBookDetails()
    {
        PrintBasicInfo();
        Console.WriteLine("Author: " + Author + ", Pages: " + PageCount);
    }
}

public class Magazine : LibraryItem
{
    public int IssueNumber { get; set; }

    public Magazine(int id, string title, int year, int issueNumber)
    {
        Id = id;
        Title = title;
        Year = year;
        IssueNumber = issueNumber;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine("Magazine: " + Title + ", Issue " + IssueNumber + " (" + Year + ")");
    }
}

public static class LibraryUtilities
{
    public static decimal CalculateLateFee(int overdueDays)
    {
        if (overdueDays <= 0) return 0m;
        return overdueDays * LibraryConfiguration.Instance.LateFeePerDay;
    }

    public static string GenerateBookCode()
    {
        Random random = new Random();
        return "BK-" + random.Next(1000, 9999);
    }

    public static void PrintSeparator()
    {
        Console.WriteLine(new string('-', 40));
    }
}

public sealed class LibraryCard
{
    public string CardNumber { get; set; }
    public string MemberName { get; set; }
    public DateTime ExpirationDate { get; set; }

    public LibraryCard(string cardNumber, string memberName, DateTime expirationDate)
    {
        CardNumber = cardNumber;
        MemberName = memberName;
        ExpirationDate = expirationDate;
    }

    public void DisplayCard()
    {
        Console.WriteLine("Card Number: " + CardNumber);
        Console.WriteLine("Member Name: " + MemberName);
        Console.WriteLine("Expiration Date: " + ExpirationDate.ToShortDateString());
    }
}

public sealed class LibraryConfiguration
{
    private static readonly Lazy<LibraryConfiguration> instance =
        new Lazy<LibraryConfiguration>(() => new LibraryConfiguration());

    public static LibraryConfiguration Instance
    {
        get { return instance.Value; }
    }

    private LibraryConfiguration()
    {
        LibraryName = "City Central Library";
        MaximumBorrowDays = 14;
        LateFeePerDay = 5.0m;
    }

    public string LibraryName { get; private set; }
    public int MaximumBorrowDays { get; private set; }
    public decimal LateFeePerDay { get; private set; }
}

class Program
{
    static void Part1()
    {
        Console.WriteLine("=== Part 1: Abstract Class ===");
        LibraryItem item = new Book(1, "Clean Code", "Robert C. Martin", 2008, 464);
        item.DisplayInfo();
        item.PrintBasicInfo();
        LibraryItem mag = new Magazine(2, "National Geographic", 2024, 5);
        mag.DisplayInfo();
        mag.PrintBasicInfo();
        Console.WriteLine("Q: Why is LibraryItem abstract?");
        Console.WriteLine("A: Because it represents a general concept that has no meaning on its own. A library only holds concrete items like books and magazines, so the shared state and behavior live in the base class while each derived class defines its own details.");
        Console.WriteLine("Q: Why can't we create: new LibraryItem();");
        Console.WriteLine("A: Because it is marked abstract. An abstract class is incomplete by design since DisplayInfo() has no body, so the compiler forbids instantiating it directly. Only non-abstract derived classes can be instantiated.");
        Console.WriteLine("Q: What is the difference between the abstract DisplayInfo() method and the concrete PrintBasicInfo() method?");
        Console.WriteLine("A: DisplayInfo() is abstract: it has no implementation in LibraryItem and every derived class must provide its own version, which enables polymorphism. PrintBasicInfo() is concrete: it is fully implemented once in the base class and shared by all derived classes as-is.");
    }

    static void Part2()
    {
        Console.WriteLine("=== Part 2: IComparable<T> ===");
        List<Book> books = new List<Book>
        {
            new Book(1, "Clean Code", "Robert C. Martin", 2008, 464),
            new Book(2, "The Pragmatic Programmer", "Andrew Hunt", 1999, 352),
            new Book(3, "Design Patterns", "GoF", 1994, 395),
            new Book(4, "C# in Depth", "Jon Skeet", 2019, 528)
        };
        books.Sort();
        foreach (Book b in books)
            Console.WriteLine(b.Year + " - " + b.Title);
        Console.WriteLine("Q: What does a negative value from CompareTo() mean?");
        Console.WriteLine("A: The current object comes before the other object in the sort order.");
        Console.WriteLine("Q: What does 0 mean?");
        Console.WriteLine("A: The two objects are equal in the sort order.");
        Console.WriteLine("Q: What does a positive value mean?");
        Console.WriteLine("A: The current object comes after the other object in the sort order.");
        Console.WriteLine("Q: Why does implementing IComparable<Book> allow List<Book>.Sort() to work without providing a separate comparison function?");
        Console.WriteLine("A: Because Sort() uses the type's default comparer, which for a type implementing IComparable<Book> calls its CompareTo method. The comparison logic travels with the type itself, so no extra function is needed.");
    }

    static void Part3()
    {
        Console.WriteLine("=== Part 3: ICloneable ===");
        Book originalBook = new Book(1, "Clean Code", "Robert C. Martin", 2008, 464);
        Book clonedBook = (Book)originalBook.Clone();
        clonedBook.Title = "Clean Code - Copy";
        Console.WriteLine("Original: Title: " + originalBook.Title);
        Console.WriteLine("Clone: Title: " + clonedBook.Title);
        Console.WriteLine("Q: What is the difference between copying an object reference and cloning an object?");
        Console.WriteLine("A: Copying a reference only creates a second variable that points to the same object in memory, so changes through one variable are visible through the other. Cloning creates a new, independent object with the same values, so changes to the clone do not affect the original.");
        Console.WriteLine("Q: What happens if you do: Book clone = original;");
        Console.WriteLine("A: clone becomes another reference to the same object. Changing clone.Title would also change original.Title, because there is only one object in memory.");
        Console.WriteLine("Q: Why does ICloneable.Clone() return object?");
        Console.WriteLine("A: Because the interface was designed to work with any type, and object is the base type of all reference types in C#. The caller must cast the result back to the real type, as in (Book)originalBook.Clone().");
        Console.WriteLine("Q: Research and explain one limitation or criticism of ICloneable.");
        Console.WriteLine("A: The interface does not specify whether Clone() performs a shallow copy or a deep copy, so two implementations can behave very differently while both satisfy the contract. This ambiguity is why many developers prefer writing their own copy constructor or a typed Clone method instead.");
    }

    static void Part4()
    {
        Console.WriteLine("=== Part 4: Static Class ===");
        Console.WriteLine("Late fee for 5 days: " + LibraryUtilities.CalculateLateFee(5) + " EGP");
        Console.WriteLine("Generated code: " + LibraryUtilities.GenerateBookCode());
        LibraryUtilities.PrintSeparator();
        Console.WriteLine("Q: Why can't a static class be instantiated?");
        Console.WriteLine("A: Because a static class is only a container for static members and has no instance state. The compiler enforces this: it has no instance constructor and cannot be used with new.");
        Console.WriteLine("Q: When is a static class appropriate?");
        Console.WriteLine("A: When you need a group of utility or helper methods that operate only on their inputs and do not need to store per-object state, such as Math or a utilities class.");
        Console.WriteLine("Q: What is the difference between a static class and a normal class containing static methods?");
        Console.WriteLine("A: A static class can contain only static members and can never be instantiated or inherited. A normal class can mix static and instance members, can be instantiated, and can be inherited; the static methods in it still belong to the type rather than to any object.");
    }

    static void Part5()
    {
        Console.WriteLine("=== Part 5: Sealed Class ===");
        LibraryCard card = new LibraryCard("LC-1001", "Ibrahim Saleh", new DateTime(2027, 12, 31));
        card.DisplayCard();
        Console.WriteLine("The line 'public class PremiumLibraryCard : LibraryCard { }' is invalid because LibraryCard is sealed, and the compiler rejects any attempt to inherit from a sealed class.");
        Console.WriteLine("Q: What does sealed mean?");
        Console.WriteLine("A: It means the class cannot be used as a base class. No other class may inherit from it.");
        Console.WriteLine("Q: Why might a developer intentionally prevent inheritance?");
        Console.WriteLine("A: To protect the class's behavior from being changed or broken by derived classes, to keep a security-sensitive or carefully designed type exactly as written, and sometimes to allow runtime optimizations since the type can never be overridden.");
        Console.WriteLine("Q: Can a sealed class inherit from another class?");
        Console.WriteLine("A: Yes. Sealed only stops others from inheriting from it; the sealed class itself can inherit from a base class normally.");
        Console.WriteLine("Q: Can a sealed class implement an interface?");
        Console.WriteLine("A: Yes. Sealing restricts inheritance, not interface implementation.");
    }

    static void Part6()
    {
        Console.WriteLine("=== Part 6: Partial Class ===");
        Book b = new Book(1, "Clean Code", "Robert C. Martin", 2008, 464);
        b.DisplayInfo();
        b.PrintBookDetails();
        Console.WriteLine("Book is declared as 'public partial class Book' in two places: one part holds the properties, constructor, IComparable implementation and operators, the other holds DisplayInfo() and PrintBookDetails(). In this single-file submission both parts appear in the same file, which the compiler treats exactly like the two-file layout Book.cs and Book.Display.cs.");
        Console.WriteLine("Q: What does partial do?");
        Console.WriteLine("A: It lets the definition of one class be split across multiple files. The compiler merges all the parts into a single class at compile time.");
        Console.WriteLine("Q: Does partial create two different Book classes?");
        Console.WriteLine("A: No. All partial parts with the same name in the same namespace are combined into one class at compile time.");
        Console.WriteLine("Q: Does partial have any effect on the runtime type?");
        Console.WriteLine("A: None at all. There is exactly one Book type at runtime; partial exists only at the source-code level.");
        Console.WriteLine("Q: Give one real-world situation where partial classes are useful.");
        Console.WriteLine("A: When part of a class is generated by a tool, such as a WinForms or WPF designer file, developers keep the generated code in one partial file and their own logic in another so the tool can regenerate its file without touching hand-written code.");
    }

    static void Part7()
    {
        Console.WriteLine("=== Part 7: Operator Overloading ===");
        Book book1 = new Book(1, "Clean Code", "Robert C. Martin", 2008, 464);
        Book book2 = new Book(2, "The Pragmatic Programmer", "Andrew Hunt", 1999, 352);
        if (book1 > book2)
            Console.WriteLine("Book 1 has more pages.");
        if (book2 < book1)
            Console.WriteLine("Book 2 has fewer pages.");
        Book combined = book1 + book2;
        Console.WriteLine("Combined PageCount: " + combined.PageCount + " (expected " + (book1.PageCount + book2.PageCount) + ")");
        Console.WriteLine("Decision for the other properties of the combined book: the combined book is a synthetic result, so its Id is 0, its Title joins both titles with '&', its Author joins both authors, and its Year is the later of the two years. Only PageCount is truly meaningful, and it equals the sum of both page counts.");
        Console.WriteLine("Q: Why can operators be overloaded in C#?");
        Console.WriteLine("A: Because the language lets a type define what operators like >, < and + mean for its objects, so custom types can be used with the same natural syntax as built-in types.");
        Console.WriteLine("Q: What makes operator overloading useful?");
        Console.WriteLine("A: It makes code read naturally when the operation has an obvious meaning for the type, such as comparing books by pages with book1 > book2 instead of book1.PageCount > book2.PageCount.");
        Console.WriteLine("Q: When can operator overloading make code harder to understand?");
        Console.WriteLine("A: When the operator's meaning is surprising or does not match its usual intuition, for example if + did something other than combining. Readers then have to look up the implementation to understand ordinary-looking code.");
        Console.WriteLine("Q: Why should overloaded operators behave intuitively?");
        Console.WriteLine("A: Because operators carry strong built-in expectations. If an operator does what readers naturally expect, the code is self-explanatory; if it does not, it becomes a source of bugs and confusion.");
    }

    static void Part8()
    {
        Console.WriteLine("=== Part 8: Singleton Design Pattern ===");
        var config1 = LibraryConfiguration.Instance;
        var config2 = LibraryConfiguration.Instance;
        Console.WriteLine("Library: " + config1.LibraryName);
        Console.WriteLine("Max borrow days: " + config1.MaximumBorrowDays);
        Console.WriteLine("Late fee per day: " + config1.LateFeePerDay + " EGP");
        Console.WriteLine("Same instance: " + ReferenceEquals(config1, config2));
        Console.WriteLine("Q: Why is the constructor private?");
        Console.WriteLine("A: So that no external code can create instances with new. The only way to get the object is through the Instance property, which is what guarantees a single instance.");
        Console.WriteLine("Q: Why is the instance static?");
        Console.WriteLine("A: Because it must belong to the class itself, not to any object, so that exactly one instance exists for the whole application and is reachable from anywhere via the class name.");
        Console.WriteLine("Q: Why is the class sealed?");
        Console.WriteLine("A: So no derived class can be created that might add extra instances or change the singleton's behavior, which would break the single-instance guarantee.");
        Console.WriteLine("Q: How does the implementation guarantee a single instance?");
        Console.WriteLine("A: The single instance is created once inside a static readonly Lazy<LibraryConfiguration>. The CLR initializes a static field exactly once, and Lazy caches the created object, so every access to Instance returns that same object.");
        Console.WriteLine("Q: How does it behave when multiple threads access it simultaneously?");
        Console.WriteLine("A: Lazy<T> is thread-safe by default: if several threads hit Instance at the same time, only one of them runs the factory and creates the object, and all threads receive that same instance. Static field initialization in the CLR is also guaranteed to run only once.");
    }

    static void Part9()
    {
        Console.WriteLine("=== Part 9: Integration ===");
        Console.WriteLine("-- 1. Abstract class and polymorphism --");
        LibraryItem[] items =
        {
            new Book(1, "Clean Code", "Robert C. Martin", 2008, 464),
            new Magazine(2, "National Geographic", 2024, 5)
        };
        foreach (LibraryItem item in items)
            item.DisplayInfo();

        Console.WriteLine("-- 2. IComparable: sorting books --");
        List<Book> books = new List<Book>
        {
            new Book(1, "Clean Code", "Robert C. Martin", 2008, 464),
            new Book(2, "The Pragmatic Programmer", "Andrew Hunt", 1999, 352),
            new Book(3, "Design Patterns", "GoF", 1994, 395),
            new Book(4, "C# in Depth", "Jon Skeet", 2019, 528)
        };
        books.Sort();
        foreach (Book b in books)
            Console.WriteLine(b.Year + " - " + b.Title);

        Console.WriteLine("-- 3. ICloneable: cloning a book --");
        Book original = books[0];
        Book clone = (Book)original.Clone();
        clone.Title = original.Title + " - Copy";
        Console.WriteLine("Original: " + original.Title);
        Console.WriteLine("Clone: " + clone.Title);

        Console.WriteLine("-- 4. Static class utilities --");
        LibraryUtilities.PrintSeparator();
        Console.WriteLine("Late fee for 3 days: " + LibraryUtilities.CalculateLateFee(3) + " EGP");
        Console.WriteLine("New book code: " + LibraryUtilities.GenerateBookCode());

        Console.WriteLine("-- 5. Sealed class --");
        LibraryCard card = new LibraryCard("LC-1001", "Ibrahim Saleh", new DateTime(2027, 12, 31));
        card.DisplayCard();
        Console.WriteLine("LibraryCard is sealed, so no class can inherit from it.");

        Console.WriteLine("-- 6. Partial class --");
        original.PrintBookDetails();
        Console.WriteLine("Book is split into two partial declarations (data and operators in one part, display methods in the other) and behaves exactly as if it were one class.");

        Console.WriteLine("-- 7. Operator overloading --");
        Book book1 = new Book(1, "Clean Code", "Robert C. Martin", 2008, 464);
        Book book2 = new Book(2, "The Pragmatic Programmer", "Andrew Hunt", 1999, 352);
        Console.WriteLine("book1 > book2: " + (book1 > book2));
        Console.WriteLine("book1 < book2: " + (book1 < book2));
        Book combined = book1 + book2;
        Console.WriteLine("book1 + book2 PageCount: " + combined.PageCount);

        Console.WriteLine("-- 8. Singleton configuration --");
        var config1 = LibraryConfiguration.Instance;
        var config2 = LibraryConfiguration.Instance;
        Console.WriteLine("Library: " + config1.LibraryName + ", MaxBorrowDays: " + config1.MaximumBorrowDays + ", LateFeePerDay: " + config1.LateFeePerDay + " EGP");
        Console.WriteLine("ReferenceEquals(config1, config2): " + ReferenceEquals(config1, config2));
    }

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===== Library Management System =====");
            Console.WriteLine("1 - Part 1: Abstract Class");
            Console.WriteLine("2 - Part 2: IComparable<T>");
            Console.WriteLine("3 - Part 3: ICloneable");
            Console.WriteLine("4 - Part 4: Static Class");
            Console.WriteLine("5 - Part 5: Sealed Class");
            Console.WriteLine("6 - Part 6: Partial Class");
            Console.WriteLine("7 - Part 7: Operator Overloading");
            Console.WriteLine("8 - Part 8: Singleton");
            Console.WriteLine("9 - Part 9: Integration (runs all parts)");
            Console.WriteLine("0 - Exit");
            Console.Write("Choose: ");
            string input = Console.ReadLine();
            int choice;
            if (!int.TryParse(input, out choice))
            {
                Console.WriteLine("Invalid choice.");
                continue;
            }
            Console.WriteLine();
            switch (choice)
            {
                case 1: Part1(); break;
                case 2: Part2(); break;
                case 3: Part3(); break;
                case 4: Part4(); break;
                case 5: Part5(); break;
                case 6: Part6(); break;
                case 7: Part7(); break;
                case 8: Part8(); break;
                case 9: Part9(); break;
                case 0: return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }
}
