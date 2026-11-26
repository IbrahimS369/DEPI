using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public string Category { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public override string ToString() => $"{ProductID} {ProductName} | {Category} | {UnitPrice} | Stock {UnitsInStock}";
}

public class Order
{
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}

public class Customer
{
    public string CustomerID { get; set; }
    public string CompanyName { get; set; }
    public string Region { get; set; }
    public Order[] Orders { get; set; }
}

public class AnagramComparer : IEqualityComparer<string>
{
    public bool Equals(string x, string y) => Key(x) == Key(y);
    public int GetHashCode(string obj) => Key(obj).GetHashCode();
    private static string Key(string s) => new string(s.Trim().ToCharArray().OrderBy(c => c).ToArray());
}

public static class Program
{
    static List<Product> ProductList = new List<Product>
    {
        new Product { ProductID = 1, ProductName = "Chai", Category = "Beverages", UnitPrice = 18m, UnitsInStock = 39 },
        new Product { ProductID = 2, ProductName = "Chang", Category = "Beverages", UnitPrice = 19m, UnitsInStock = 17 },
        new Product { ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", UnitPrice = 10m, UnitsInStock = 13 },
        new Product { ProductID = 4, ProductName = "Chef Anton's Gumbo Mix", Category = "Condiments", UnitPrice = 21.35m, UnitsInStock = 0 },
        new Product { ProductID = 5, ProductName = "Alice Mutton", Category = "Meat/Poultry", UnitPrice = 39m, UnitsInStock = 0 },
        new Product { ProductID = 6, ProductName = "Thüringer Rostbratwurst", Category = "Meat/Poultry", UnitPrice = 123.79m, UnitsInStock = 0 },
        new Product { ProductID = 7, ProductName = "Côte de Blaye", Category = "Beverages", UnitPrice = 263.5m, UnitsInStock = 17 },
        new Product { ProductID = 8, ProductName = "Manjimup Dried Apples", Category = "Produce", UnitPrice = 53m, UnitsInStock = 20 },
        new Product { ProductID = 9, ProductName = "Ikura", Category = "Seafood", UnitPrice = 31m, UnitsInStock = 31 },
        new Product { ProductID = 10, ProductName = "Carnarvon Tigers", Category = "Seafood", UnitPrice = 62.5m, UnitsInStock = 42 },
        new Product { ProductID = 11, ProductName = "Sir Rodney's Marmalade", Category = "Confections", UnitPrice = 81m, UnitsInStock = 40 },
        new Product { ProductID = 12, ProductName = "Mishi Kobe Niku", Category = "Meat/Poultry", UnitPrice = 97m, UnitsInStock = 29 },
        new Product { ProductID = 13, ProductName = "Tofu", Category = "Produce", UnitPrice = 23.25m, UnitsInStock = 35 },
        new Product { ProductID = 14, ProductName = "Gorgonzola Telino", Category = "Dairy Products", UnitPrice = 12.5m, UnitsInStock = 0 },
        new Product { ProductID = 15, ProductName = "Mascarpone Fabioli", Category = "Dairy Products", UnitPrice = 32m, UnitsInStock = 9 },
        new Product { ProductID = 16, ProductName = "Sasquatch Ale", Category = "Beverages", UnitPrice = 14m, UnitsInStock = 111 },
        new Product { ProductID = 17, ProductName = "Konbu", Category = "Seafood", UnitPrice = 6m, UnitsInStock = 24 },
        new Product { ProductID = 18, ProductName = "Singaporean Hokkien Fried Mee", Category = "Grains/Cereals", UnitPrice = 14m, UnitsInStock = 26 }
    };

    static List<Customer> CustomerList;
    static string[] Dictionary;
    static int[] Arr = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

    static void LoadCustomers()
    {
        string path = File.Exists("Customers.xml") ? "Customers.xml" : Path.Combine(AppContext.BaseDirectory, "Customers.xml");
        if (!File.Exists(path)) { CustomerList = new List<Customer>(); return; }
        CustomerList = XDocument.Load(path).Root.Elements("customer").Select(c => new Customer
        {
            CustomerID = (string)c.Element("id"),
            CompanyName = (string)c.Element("name"),
            Region = (string)c.Element("region"),
            Orders = c.Element("orders") == null ? new Order[0] : c.Element("orders").Elements("order").Select(o => new Order
            {
                OrderID = (int)o.Element("id"),
                OrderDate = (DateTime)o.Element("orderdate"),
                Total = (decimal)o.Element("total")
            }).ToArray()
        }).ToList();
    }

    static void LoadDictionary()
    {
        string path = File.Exists("dictionary_english.txt") ? "dictionary_english.txt" : Path.Combine(AppContext.BaseDirectory, "dictionary_english.txt");
        Dictionary = File.Exists(path) ? File.ReadAllLines(path) : new string[0];
    }

    static void Header(string t) { Console.WriteLine(); Console.WriteLine("=== " + t + " ==="); }

    static void Elements()
    {
        Header("LINQ - Element Operators");
        Console.WriteLine("1) " + ProductList.FirstOrDefault(p => p.UnitsInStock == 0));
        var q2 = ProductList.FirstOrDefault(p => p.UnitPrice > 1000);
        Console.WriteLine("2) " + (q2 == null ? "null" : q2.ToString()));
        Console.WriteLine("3) " + Arr.Where(n => n > 5).Skip(1).First());
    }

    static void Aggregates()
    {
        Header("LINQ - Aggregate Operators");
        Console.WriteLine("1) Odd count: " + Arr.Count(n => n % 2 == 1));
        Console.WriteLine("2) Customers and order counts:");
        foreach (var c in CustomerList) Console.WriteLine($"   {c.CustomerID} {c.CompanyName}: {c.Orders.Length}");
        Console.WriteLine("3) Categories and product counts:");
        foreach (var g in ProductList.GroupBy(p => p.Category)) Console.WriteLine($"   {g.Key}: {g.Count()}");
        Console.WriteLine("4) Sum: " + Arr.Sum());
        Console.WriteLine("5) Total characters: " + Dictionary.Sum(w => w.Length));
        Console.WriteLine("6) Shortest length: " + (Dictionary.Length == 0 ? 0 : Dictionary.Min(w => w.Length)));
        Console.WriteLine("7) Longest length: " + (Dictionary.Length == 0 ? 0 : Dictionary.Max(w => w.Length)));
        Console.WriteLine("8) Average length: " + (Dictionary.Length == 0 ? 0 : Dictionary.Average(w => w.Length)));
        Console.WriteLine("9) Units in stock per category:");
        foreach (var g in ProductList.GroupBy(p => p.Category)) Console.WriteLine($"   {g.Key}: {g.Sum(p => p.UnitsInStock)}");
        Console.WriteLine("10) Cheapest price per category:");
        foreach (var g in ProductList.GroupBy(p => p.Category)) Console.WriteLine($"   {g.Key}: {g.Min(p => p.UnitPrice)}");
        Console.WriteLine("11) Cheapest products per category (let):");
        var cheapest = from p in ProductList
                       group p by p.Category into g
                       let min = g.Min(x => x.UnitPrice)
                       select new { Category = g.Key, Products = g.Where(x => x.UnitPrice == min) };
        foreach (var g in cheapest) { Console.WriteLine("   " + g.Category); foreach (var p in g.Products) Console.WriteLine("      " + p); }
        Console.WriteLine("12) Most expensive price per category:");
        foreach (var g in ProductList.GroupBy(p => p.Category)) Console.WriteLine($"   {g.Key}: {g.Max(p => p.UnitPrice)}");
        Console.WriteLine("13) Most expensive products per category:");
        var priciest = from p in ProductList
                       group p by p.Category into g
                       let max = g.Max(x => x.UnitPrice)
                       select new { Category = g.Key, Products = g.Where(x => x.UnitPrice == max) };
        foreach (var g in priciest) { Console.WriteLine("   " + g.Category); foreach (var p in g.Products) Console.WriteLine("      " + p); }
        Console.WriteLine("14) Average price per category:");
        foreach (var g in ProductList.GroupBy(p => p.Category)) Console.WriteLine($"   {g.Key}: {g.Average(p => p.UnitPrice):F2}");
    }

    static void Sets()
    {
        Header("LINQ - Set Operators");
        Console.WriteLine("1) Unique categories: " + string.Join(", ", ProductList.Select(p => p.Category).Distinct()));
        var pf = ProductList.Select(p => p.ProductName[0]);
        var cf = CustomerList.Select(c => c.CompanyName[0]);
        Console.WriteLine("2) Unique first letters (union): " + string.Join(", ", pf.Union(cf)));
        Console.WriteLine("3) Common first letters: " + string.Join(", ", pf.Intersect(cf)));
        Console.WriteLine("4) Product-only first letters: " + string.Join(", ", pf.Except(cf)));
        var l3 = ProductList.Select(p => p.ProductName).Concat(CustomerList.Select(c => c.CompanyName))
            .Select(n => n.Length >= 3 ? n.Substring(n.Length - 3) : n);
        Console.WriteLine("5) Last three characters (with duplicates): " + string.Join(", ", l3));
    }

    static void Partitioning()
    {
        Header("LINQ - Partitioning Operators");
        var wa = CustomerList.Where(c => c.Region == "WA").SelectMany(c => c.Orders.Select(o => new { c.CustomerID, o.OrderID, o.OrderDate, o.Total }));
        Console.WriteLine("1) First 3 orders from Washington:");
        foreach (var o in wa.Take(3)) Console.WriteLine($"   {o.CustomerID} {o.OrderID} {o.OrderDate:d} {o.Total}");
        Console.WriteLine("2) All but the first 2 orders from Washington:");
        foreach (var o in wa.Skip(2)) Console.WriteLine($"   {o.CustomerID} {o.OrderID} {o.OrderDate:d} {o.Total}");
        Console.WriteLine("3) TakeWhile n >= index: " + string.Join(", ", Arr.TakeWhile((n, i) => n >= i)));
        Console.WriteLine("4) From first divisible by 3: " + string.Join(", ", Arr.SkipWhile(n => n % 3 != 0)));
        Console.WriteLine("5) From first element less than its position: " + string.Join(", ", Arr.SkipWhile((n, i) => n >= i)));
    }

    static void Quantifiers()
    {
        Header("LINQ - Quantifiers");
        Console.WriteLine("1) Any word contains 'ei': " + Dictionary.Any(w => w.Contains("ei")));
        Console.WriteLine("2) Categories with at least one product out of stock:");
        var some = ProductList.GroupBy(p => p.Category).Where(g => g.Any(p => p.UnitsInStock == 0));
        foreach (var g in some) { Console.WriteLine("   " + g.Key); foreach (var p in g) Console.WriteLine("      " + p); }
        Console.WriteLine("3) Categories with all products in stock:");
        var all = ProductList.GroupBy(p => p.Category).Where(g => g.All(p => p.UnitsInStock > 0));
        foreach (var g in all) { Console.WriteLine("   " + g.Key); foreach (var p in g) Console.WriteLine("      " + p); }
    }

    static void Grouping()
    {
        Header("LINQ - Grouping Operators");
        List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        Console.WriteLine("1) Numbers grouped by remainder of division by 5:");
        foreach (var g in numbers.GroupBy(n => n % 5).OrderBy(g => g.Key))
        {
            Console.WriteLine($"   Numbers with a remainder of {g.Key} when divided by 5:");
            foreach (var n in g) Console.WriteLine("      " + n);
        }
        Console.WriteLine("2) Words grouped by first letter (first 5 groups shown):");
        foreach (var g in Dictionary.Where(w => w.Length > 0).GroupBy(w => char.ToUpper(w[0])).OrderBy(g => g.Key).Take(5))
            Console.WriteLine($"   {g.Key}: {g.Count()} words, e.g. {string.Join(", ", g.Take(5))}");
        Console.WriteLine("3) Anagram groups:");
        string[] words = { "from", "salt", "earn", " last", "near", "form" };
        foreach (var g in words.GroupBy(w => w, new AnagramComparer()))
        {
            Console.WriteLine("   " + string.Join(", ", g.Select(w => w.Trim())));
        }
    }

    public static void Main()
    {
        LoadCustomers();
        LoadDictionary();
        if (CustomerList.Count == 0) Console.WriteLine("Customers.xml not found next to the program, customer queries will be empty.");
        if (Dictionary.Length == 0) Console.WriteLine("dictionary_english.txt not found next to the program, dictionary queries will be empty.");
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1 Element  2 Aggregate  3 Set  4 Partitioning  5 Quantifiers  6 Grouping  7 All  0 Exit");
            Console.Write("Choice: ");
            string c = Console.ReadLine();
            if (c == "0") break;
            switch (c)
            {
                case "1": Elements(); break;
                case "2": Aggregates(); break;
                case "3": Sets(); break;
                case "4": Partitioning(); break;
                case "5": Quantifiers(); break;
                case "6": Grouping(); break;
                case "7": Elements(); Aggregates(); Sets(); Partitioning(); Quantifiers(); Grouping(); break;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }
    }
}
