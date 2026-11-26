using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Day_01_G03
{

	class Product : IComparable<Product>
	{
		public long ProductID { get; set; }
		public string ProductName { get; set; }
		public string Category { get; set; }
		public decimal UnitPrice { get; set; }
		public int UnitsInStock { get; set; }

		public int CompareTo(Product? other)
			=> this.UnitPrice.CompareTo(other?.UnitPrice);
		

		public override string ToString()
			=> $"ProductID:{ProductID},ProductName:{ProductName},Category{Category},UnitPrice:{UnitPrice},UnitsInStock:{UnitsInStock}";

	}
	class Customer
	{
		public string CustomerID { get; set; }
		public string CustomerName { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public Order[] Orders { get; set; }
		public Customer(string customerID, string customerName)
		{
			CustomerID = customerID;
			CustomerName = customerName;
			Orders = new Order[10];
		}
		public Customer()
		{

		}
		public override string ToString()
			=> $"{CustomerID}, {CustomerName}, {Address}, {City}, {Region}, {PostalCode}, {Country}, {Phone}, {Fax}";
	}
	class Order
	{

		public int OrderID { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal Total { get; set; }

		public Order(int orderID, DateTime orderDate, decimal total)
		{
			OrderID = orderID;
			OrderDate = orderDate;
			Total = total;
		}
		public Order()
		{

		}

		public override string ToString()
			=> $"Order Id: {OrderID}, Date: {OrderDate.ToShortDateString()}, Total: {Total}";
	}
	internal static class ListGenerator
	{
		public static List<Product> ProductsList { get; set; }
		public static List<Customer> CustomersList { get; set; }

		static ListGenerator()
		{
			ProductsList = new List<Product>()
			{

						new Product() {ProductID = 1, ProductName = "Chai", Category = "Beverages",
							UnitPrice = 18.00M, UnitsInStock = 100},
						new Product{ ProductID = 2, ProductName = "Chang", Category = "Beverages",
						UnitPrice = 19.0000M, UnitsInStock = 17 },
					  new Product{ ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments",
						UnitPrice = 10.0000M, UnitsInStock = 13 },
					  new Product{ ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", Category = "Condiments",
						UnitPrice = 22.0000M, UnitsInStock = 53 },
					  new Product{ ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", Category = "Condiments",
						UnitPrice = 21.3500M, UnitsInStock = 0 },
					  new Product{ ProductID = 6, ProductName = "Grandma's Boysenberry Spread", Category = "Condiments",
						UnitPrice = 25.0000M, UnitsInStock = 120 },
					  new Product{ ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", Category = "Produce",
						UnitPrice = 30.0000M, UnitsInStock = 15 },
					  new Product{ ProductID = 8, ProductName = "Northwoods Cranberry Sauce", Category = "Condiments",
						UnitPrice = 40.0000M, UnitsInStock = 6 },
					  new Product{ ProductID = 9, ProductName = "Mishi Kobe Niku", Category = "Meat/Poultry",
						UnitPrice = 97.0000M, UnitsInStock = 29 },
					  new Product{ ProductID = 10, ProductName = "Ikura", Category = "Seafood",
						UnitPrice = 31.0000M, UnitsInStock = 31 },
					  new Product{ ProductID = 11, ProductName = "Queso Cabrales", Category = "Dairy Products",
						UnitPrice = 21.0000M, UnitsInStock = 22 },
					  new Product{ ProductID = 12, ProductName = "Queso Manchego La Pastora", Category = "Dairy Products",
						UnitPrice = 38.0000M, UnitsInStock = 86 },
					  new Product() { ProductID = 13, ProductName = "Konbu", Category = "Seafood",
						UnitPrice = 6.0000M, UnitsInStock = 24 },
					  new Product() { ProductID = 14, ProductName = "Tofu", Category = "Produce",
						UnitPrice = 23.2500M, UnitsInStock = 35 },
					  new Product() { ProductID = 15, ProductName = "Genen Shouyu", Category = "Condiments",
						UnitPrice = 15.5000M, UnitsInStock = 39 },
					  new Product() { ProductID = 16, ProductName = "Pavlova", Category = "Confections",
						UnitPrice = 17.4500M, UnitsInStock = 29 },
					  new Product() { ProductID = 17, ProductName = "Alice Mutton", Category = "Meat/Poultry",
						UnitPrice = 39.0000M, UnitsInStock = 0 },
					  new Product() { ProductID = 18, ProductName = "Carnarvon Tigers", Category = "Seafood",
						UnitPrice = 62.5000M, UnitsInStock = 42 },
					  new Product() { ProductID = 19, ProductName = "Teatime Chocolate Biscuits", Category = "Confections",
						UnitPrice = 9.2000M, UnitsInStock = 25 },
					  new Product() { ProductID = 20, ProductName = "Sir Rodney's Marmalade", Category = "Confections",
						UnitPrice = 81.0000M, UnitsInStock = 40 },
					  new Product() { ProductID = 21, ProductName = "Sir Rodney's Scones", Category = "Confections",
						UnitPrice = 10.0000M, UnitsInStock = 3 },
					  new Product() { ProductID = 22, ProductName = "Gustaf's Knäckebröd", Category = "Grains/Cereals",
						UnitPrice = 21.0000M, UnitsInStock = 104 },
					  new Product() { ProductID = 23, ProductName = "Tunnbröd", Category = "Grains/Cereals",
						UnitPrice = 9.0000M, UnitsInStock = 61 },
					  new Product() { ProductID = 24, ProductName = "Guaraná Fantástica", Category = "Beverages",
						UnitPrice = 4.5000M, UnitsInStock = 20 },
					  new Product() { ProductID = 25, ProductName = "NuNuCa Nuß-Nougat-Creme", Category = "Confections",
						UnitPrice = 14.0000M, UnitsInStock = 76 },
					  new Product() { ProductID = 26, ProductName = "Gumbär Gummibärchen", Category = "Confections",
						UnitPrice = 31.2300M, UnitsInStock = 15 },
					  new Product() { ProductID = 27, ProductName = "Schoggi Schokolade", Category = "Confections",
						UnitPrice = 43.9000M, UnitsInStock = 49 },
					  new Product() { ProductID = 28, ProductName = "Rössle Sauerkraut", Category = "Produce",
						UnitPrice = 45.6000M, UnitsInStock = 26 },
					  new Product() { ProductID = 29, ProductName = "Thüringer Rostbratwurst", Category = "Meat/Poultry",
						UnitPrice = 123.7900M, UnitsInStock = 0 },
					  new Product() { ProductID = 30, ProductName = "Nord-Ost Matjeshering", Category = "Seafood",
						UnitPrice = 25.8900M, UnitsInStock = 10 },
					  new Product() { ProductID = 31, ProductName = "Gorgonzola Telino", Category = "Dairy Products",
						UnitPrice = 12.5000M, UnitsInStock = 0 },
					  new Product() { ProductID = 32, ProductName = "Mascarpone Fabioli", Category = "Dairy Products",
						UnitPrice = 32.0000M, UnitsInStock = 9 },
					  new Product() { ProductID = 33, ProductName = "Geitost", Category = "Dairy Products",
						UnitPrice = 2.5000M, UnitsInStock = 112 },
					  new Product() { ProductID = 34, ProductName = "Sasquatch Ale", Category = "Beverages",
						UnitPrice = 14.0000M, UnitsInStock = 111 },
					  new Product() { ProductID = 35, ProductName = "Steeleye Stout", Category = "Beverages",
						UnitPrice = 18.0000M, UnitsInStock = 20 },
					  new Product() { ProductID = 36, ProductName = "Inlagd Sill", Category = "Seafood",
						UnitPrice = 19.0000M, UnitsInStock = 112 },
					  new Product() { ProductID = 37, ProductName = "Gravad lax", Category = "Seafood",
						UnitPrice = 26.0000M, UnitsInStock = 11 },
					  new Product() { ProductID = 38, ProductName = "Côte de Blaye", Category = "Beverages",
						UnitPrice = 263.5000M, UnitsInStock = 17 },
					  new Product() { ProductID = 39, ProductName = "Chartreuse verte", Category = "Beverages",
						UnitPrice = 18.0000M, UnitsInStock = 69 },
					  new Product() { ProductID = 40, ProductName = "Boston Crab Meat", Category = "Seafood",
						UnitPrice = 18.4000M, UnitsInStock = 123 },
					  new Product() { ProductID = 41, ProductName = "Jack's New England Clam Chowder", Category = "Seafood",
						UnitPrice = 9.6500M, UnitsInStock = 85 },
					  new Product() { ProductID = 42, ProductName = "Singaporean Hokkien Fried Mee", Category = "Grains/Cereals",
						UnitPrice = 14.0000M, UnitsInStock = 26 },
					  new Product() { ProductID = 43, ProductName = "Ipoh Coffee", Category = "Beverages",
						UnitPrice = 46.0000M, UnitsInStock = 17 },
					  new Product() { ProductID = 44, ProductName = "Gula Malacca", Category = "Condiments",
						UnitPrice = 19.4500M, UnitsInStock = 27 },
					  new Product() { ProductID = 45, ProductName = "Rogede sild", Category = "Seafood",
						UnitPrice = 9.5000M, UnitsInStock = 5 },
					  new Product() { ProductID = 46, ProductName = "Spegesild", Category = "Seafood",
						UnitPrice = 12.0000M, UnitsInStock = 95 },
					  new Product() { ProductID = 47, ProductName = "Zaanse koeken", Category = "Confections",
						UnitPrice = 9.5000M, UnitsInStock = 36 },
					  new Product() { ProductID = 48, ProductName = "Chocolade", Category = "Confections",
						UnitPrice = 12.7500M, UnitsInStock = 15 },
					  new Product() { ProductID = 49, ProductName = "Maxilaku", Category = "Confections",
						UnitPrice = 20.0000M, UnitsInStock = 10 },
					  new Product() { ProductID = 50, ProductName = "Valkoinen suklaa", Category = "Confections",
						UnitPrice = 16.2500M, UnitsInStock = 65 },
					  new Product() { ProductID = 51, ProductName = "Manjimup Dried Apples", Category = "Produce",
						UnitPrice = 53.0000M, UnitsInStock = 20 },
					  new Product() { ProductID = 52, ProductName = "Filo Mix", Category = "Grains/Cereals",
						UnitPrice = 7.0000M, UnitsInStock = 38 },
					  new Product() { ProductID = 53, ProductName = "Perth Pasties", Category = "Meat/Poultry",
						UnitPrice = 32.8000M, UnitsInStock = 0 },
					  new Product() { ProductID = 54, ProductName = "Tourtière", Category = "Meat/Poultry",
						UnitPrice = 7.4500M, UnitsInStock = 21 },
					  new Product() { ProductID = 55, ProductName = "Pâté chinois", Category = "Meat/Poultry",
						UnitPrice = 24.0000M, UnitsInStock = 115 },
					  new Product() { ProductID = 56, ProductName = "Gnocchi di nonna Alice", Category = "Grains/Cereals",
						UnitPrice = 38.0000M, UnitsInStock = 21 },
					  new Product() { ProductID = 57, ProductName = "Ravioli Angelo", Category = "Grains/Cereals",
						UnitPrice = 19.5000M, UnitsInStock = 36 },
					  new Product() { ProductID = 58, ProductName = "Escargots de Bourgogne", Category = "Seafood",
						UnitPrice = 13.2500M, UnitsInStock = 62 },
					  new Product() { ProductID = 59, ProductName = "Raclette Courdavault", Category = "Dairy Products",
						UnitPrice = 55.0000M, UnitsInStock = 79 },
					  new Product() { ProductID = 60, ProductName = "Camembert Pierrot", Category = "Dairy Products",
						UnitPrice = 34.0000M, UnitsInStock = 19 },
					  new Product() { ProductID = 61, ProductName = "Sirop d'érable", Category = "Condiments",
						UnitPrice = 28.5000M, UnitsInStock = 113 },
					  new Product() { ProductID = 62, ProductName = "Tarte au sucre", Category = "Confections",
						UnitPrice = 49.3000M, UnitsInStock = 17 },
					  new Product() { ProductID = 63, ProductName = "Vegie-spread", Category = "Condiments",
						UnitPrice = 43.9000M, UnitsInStock = 24 },
					  new Product() { ProductID = 64, ProductName = "Wimmers gute Semmelknödel", Category = "Grains/Cereals",
						UnitPrice = 33.2500M, UnitsInStock = 22 },
					  new Product() { ProductID = 65, ProductName = "Louisiana Fiery Hot Pepper Sauce", Category = "Condiments",
						UnitPrice = 21.0500M, UnitsInStock = 76 },
					  new Product() { ProductID = 66, ProductName = "Louisiana Hot Spiced Okra", Category = "Condiments",
						UnitPrice = 17.0000M, UnitsInStock = 4 },
					  new Product() { ProductID = 67, ProductName = "Laughing Lumberjack Lager", Category = "Beverages",
						UnitPrice = 14.0000M, UnitsInStock = 52 },
					  new Product() { ProductID = 68, ProductName = "Scottish Longbreads", Category = "Confections",
						UnitPrice = 12.5000M, UnitsInStock = 6 },
					  new Product() { ProductID = 69, ProductName = "Gudbrandsdalsost", Category = "Dairy Products",
						UnitPrice = 36.0000M, UnitsInStock = 26 },
					  new Product() { ProductID = 70, ProductName = "Outback Lager", Category = "Beverages",
						UnitPrice = 15.0000M, UnitsInStock = 15 },
					  new Product() { ProductID = 71, ProductName = "Flotemysost", Category = "Dairy Products",
						UnitPrice = 21.5000M, UnitsInStock = 26 },
					  new Product() { ProductID = 72, ProductName = "Mozzarella di Giovanni", Category = "Dairy Products",
						UnitPrice = 34.8000M, UnitsInStock = 14 },
					  new Product() { ProductID = 73, ProductName = "Röd Kaviar", Category = "Seafood",
						UnitPrice = 15.0000M, UnitsInStock = 101 },
					  new Product() { ProductID = 74, ProductName = "Longlife Tofu", Category = "Produce",
						UnitPrice = 10.0000M, UnitsInStock = 4 },
					  new Product() { ProductID = 75, ProductName = "Rhönbräu Klosterbier", Category = "Beverages",
						UnitPrice = 7.7500M, UnitsInStock = 125 },
					  new Product() { ProductID = 76, ProductName = "Lakkalikööri", Category = "Beverages",
						UnitPrice = 18.0000M, UnitsInStock = 57 },
					  new Product() { ProductID = 77, ProductName = "Original Frankfurter grüne Soße", Category = "Condiments",
						UnitPrice = 13.0000M, UnitsInStock = 32 }
			};

			CustomersList = (from e in XDocument.Load("Customers.xml").Root.Elements("customer")
							 select new Customer()
							 {
								 CustomerID = (string)e.Element("id"),
								 CustomerName = (string)e.Element("name"),
								 Address = (string)e.Element("address"),
								 City = (string)e.Element("city"),
								 Region = (string)e.Element("region"),
								 PostalCode = (string)e.Element("postalcode"),
								 Country = (string)e.Element("country"),
								 Phone = (string)e.Element("phone"),
								 Fax = (string)e.Element("fax"),
								 Orders = (
									  from o in e.Elements("orders").Elements("order")
									  select new Order
									  {
										  OrderID = (int)o.Element("id"),
										  OrderDate = (DateTime)o.Element("orderdate"),
										  Total = (decimal)o.Element("total")
									  }).ToArray()
							 }).ToList();
		}
	}

	class CaseInsensitiveComparer : IComparer<string>
	{
		public int Compare(string? x, string? y)
			=> string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
	}

	class Program
	{
		static string[] Digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
		static int[] Numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
		static string[] Fruits = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

		static string[] LoadWords()
			=> File.ReadAllLines("dictionary_english.txt");

		static void Show<T>(IEnumerable<T> items)
		{
			foreach (var item in items)
				Console.WriteLine(item);
		}

		static void Restriction()
		{
			Console.WriteLine("1. Products out of stock:");
			Show(ListGenerator.ProductsList.Where(p => p.UnitsInStock == 0));

			Console.WriteLine("2. Products in stock and cost more than 3.00:");
			Show(ListGenerator.ProductsList.Where(p => p.UnitsInStock > 0 && p.UnitPrice > 3.00M));

			Console.WriteLine("3. Digits whose name is shorter than their value:");
			Show(Digits.Where((d, i) => d.Length < i));
		}

		static void Element()
		{
			Console.WriteLine("1. First product out of stock:");
			Console.WriteLine(ListGenerator.ProductsList.First(p => p.UnitsInStock == 0));

			Console.WriteLine("2. First product with price > 1000:");
			var expensive = ListGenerator.ProductsList.FirstOrDefault(p => p.UnitPrice > 1000);
			Console.WriteLine(expensive is null ? "null" : expensive.ToString());

			Console.WriteLine("3. Second number greater than 5:");
			Console.WriteLine(Numbers.Where(n => n > 5).Skip(1).First());
		}

		static void Aggregate()
		{
			Console.WriteLine("1. Count of odd numbers:");
			Console.WriteLine(Numbers.Count(n => n % 2 != 0));

			Console.WriteLine("2. Customers and their order counts:");
			foreach (var c in ListGenerator.CustomersList)
				Console.WriteLine($"{c.CustomerName}: {c.Orders.Length}");

			Console.WriteLine("3. Categories and product counts:");
			foreach (var g in ListGenerator.ProductsList.GroupBy(p => p.Category))
				Console.WriteLine($"{g.Key}: {g.Count()}");

			Console.WriteLine("4. Sum of numbers:");
			Console.WriteLine(Numbers.Sum());

			var words = LoadWords();

			Console.WriteLine("5. Total characters of all words:");
			Console.WriteLine(words.Sum(w => w.Length));

			Console.WriteLine("6. Shortest word length:");
			Console.WriteLine(words.Min(w => w.Length));

			Console.WriteLine("7. Longest word length:");
			Console.WriteLine(words.Max(w => w.Length));

			Console.WriteLine("8. Average word length:");
			Console.WriteLine(words.Average(w => w.Length));
		}

		static void Ordering()
		{
			var comparer = new CaseInsensitiveComparer();

			Console.WriteLine("1. Products sorted by name:");
			Show(ListGenerator.ProductsList.OrderBy(p => p.ProductName));

			Console.WriteLine("2. Words sorted case-insensitively:");
			Show(Fruits.OrderBy(w => w, comparer));

			Console.WriteLine("3. Products sorted by units in stock, highest first:");
			Show(ListGenerator.ProductsList.OrderByDescending(p => p.UnitsInStock));

			Console.WriteLine("4. Digits sorted by name length then alphabetically:");
			Show(Digits.OrderBy(d => d.Length).ThenBy(d => d));

			Console.WriteLine("5. Words sorted by length then case-insensitively:");
			Show(Fruits.OrderBy(w => w.Length).ThenBy(w => w, comparer));

			Console.WriteLine("6. Products sorted by category then unit price descending:");
			Show(ListGenerator.ProductsList.OrderBy(p => p.Category).ThenByDescending(p => p.UnitPrice));

			Console.WriteLine("7. Words sorted by length then case-insensitive descending:");
			Show(Fruits.OrderBy(w => w.Length).ThenByDescending(w => w, comparer));

			Console.WriteLine("8. Digits whose second letter is 'i', reversed:");
			Show(Digits.Where(d => d[1] == 'i').Reverse());
		}

		static void Transformation()
		{
			Console.WriteLine("1. Product names:");
			Show(ListGenerator.ProductsList.Select(p => p.ProductName));

			Console.WriteLine("2. Uppercase and lowercase versions of words:");
			string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };
			foreach (var w in words.Select(w => new { Upper = w.ToUpper(), Lower = w.ToLower() }))
				Console.WriteLine($"Uppercase: {w.Upper}, Lowercase: {w.Lower}");

			Console.WriteLine("3. Product properties with UnitPrice renamed to Price:");
			foreach (var p in ListGenerator.ProductsList.Select(p => new { p.ProductID, p.ProductName, Price = p.UnitPrice }))
				Console.WriteLine($"{p.ProductID}, {p.ProductName}, {p.Price}");

			Console.WriteLine("4. Number: In-place?");
			foreach (var r in Numbers.Select((n, i) => new { Number = n, InPlace = n == i }))
				Console.WriteLine($"{r.Number}: {r.InPlace}");

			Console.WriteLine("5. Pairs where a < b:");
			int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
			int[] numbersB = { 1, 3, 5, 7, 8 };
			var pairs = from a in numbersA
						from b in numbersB
						where a < b
						select new { a, b };
			foreach (var pair in pairs)
				Console.WriteLine($"{pair.a} is less than {pair.b}");

			Console.WriteLine("6. Orders with total less than 500.00:");
			Show(ListGenerator.CustomersList.SelectMany(c => c.Orders).Where(o => o.Total < 500.00M));

			Console.WriteLine("7. Orders made in 1998 or later:");
			Show(ListGenerator.CustomersList.SelectMany(c => c.Orders).Where(o => o.OrderDate.Year >= 1998));
		}

		static void Main()
		{
			while (true)
			{
				Console.WriteLine();
				Console.WriteLine("1. Restriction Operators");
				Console.WriteLine("2. Element Operators");
				Console.WriteLine("3. Aggregate Operators");
				Console.WriteLine("4. Ordering Operators");
				Console.WriteLine("5. Transformation Operators");
				Console.WriteLine("0. Exit");
				Console.Write("Choose: ");
				string? choice = Console.ReadLine();
				Console.WriteLine();
				switch (choice)
				{
					case "1": Restriction(); break;
					case "2": Element(); break;
					case "3": Aggregate(); break;
					case "4": Ordering(); break;
					case "5": Transformation(); break;
					case "0": return;
					default: Console.WriteLine("Invalid choice"); break;
				}
			}
		}
	}
}
