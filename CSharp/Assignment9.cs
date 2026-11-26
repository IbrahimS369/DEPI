using System;
using System.Collections.Generic;

namespace Assignment9
{
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }

        public Book(string _ISBN, string _Title, string[] _Authors, DateTime _PublicationDate, decimal _Price)
        {
            ISBN = _ISBN;
            Title = _Title;
            Authors = _Authors;
            PublicationDate = _PublicationDate;
            Price = _Price;
        }

        public override string ToString()
        {
            return $"ISBN: {ISBN}, Title: {Title}, Authors: {string.Join(", ", Authors)}, Published: {PublicationDate:yyyy-MM-dd}, Price: {Price:C}";
        }
    }

    public class BookFunctions
    {
        public static string GetTitle(Book B)
        {
            return B.Title;
        }

        public static string GetAuthors(Book B)
        {
            return string.Join(", ", B.Authors);
        }

        public static string GetPrice(Book B)
        {
            return B.Price.ToString("C");
        }

        public static string GetISBN(Book B)
        {
            return B.ISBN;
        }

        public static string GetPublicationDate(Book B)
        {
            return B.PublicationDate.ToString("yyyy-MM-dd");
        }
    }

    public delegate string BookDelegate(Book B);

    public class LibraryEngine
    {
        public static void ProcessBooks(List<Book> bList, BookDelegate fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }

        public static void ProcessBooks(List<Book> bList, Func<Book, string> fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public delegate decimal PriceCalculator(Order order);

    public static class OrderPricing
    {
        public static decimal CalculateTotal(Order order)
        {
            return order.Price * order.Quantity;
        }

        public static decimal CalculateTotalWithDiscount(Order order)
        {
            return order.Price * order.Quantity - 10m;
        }

        public static decimal CalculateOrderPrice(Order order, PriceCalculator calculator)
        {
            return calculator(order);
        }

        public static decimal CalculateOrderPrice(Order order, Func<Order, decimal> calculator)
        {
            return calculator(order);
        }
    }

    public static class OrderValidator
    {
        public static bool ValidateOrder(Order order, Predicate<Order> validationRule)
        {
            return validationRule(order);
        }
    }

    public class OrderService
    {
        private readonly Func<Order, decimal> pricingStrategy;

        public event Action<Order> OrderProcessed;

        public OrderService(Func<Order, decimal> pricingStrategy)
        {
            this.pricingStrategy = pricingStrategy;
        }

        public decimal GetPrice(Order order)
        {
            return pricingStrategy(order);
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"Processing order {order.Id}, total: {pricingStrategy(order):C}");
            OrderProcessed?.Invoke(order);
        }
    }

    public static class OrderActions
    {
        public static void ProcessOrder(Order order, Action<Order> action)
        {
            Console.WriteLine($"Order {order.Id} is being processed.");
            action(order);
        }
    }

    public class Program
    {
        static List<Book> MakeBooks()
        {
            return new List<Book>
            {
                new Book("111-1", "C# in Depth", new[] { "Jon Skeet" }, new DateTime(2019, 3, 25), 45.5m),
                new Book("222-2", "Clean Code", new[] { "Robert Martin" }, new DateTime(2008, 8, 1), 38m),
                new Book("333-3", "The Pragmatic Programmer", new[] { "Andrew Hunt", "David Thomas" }, new DateTime(1999, 10, 20), 42.75m)
            };
        }

        static Order MakeOrder()
        {
            return new Order { Id = 1, CustomerName = "Ahmed", Price = 50m, Quantity = 3 };
        }

        static void Section1()
        {
            Book b = new Book("999-9", "Sample", new[] { "Author One", "Author Two" }, new DateTime(2020, 1, 15), 10m);
            Console.WriteLine(b);
        }

        static void Section1Q2()
        {
            List<Book> books = MakeBooks();

            Console.WriteLine("a) User-defined delegate (GetTitle)");
            LibraryEngine.ProcessBooks(books, new BookDelegate(BookFunctions.GetTitle));

            Console.WriteLine("b) Built-in delegate Func<Book, string> (GetAuthors)");
            LibraryEngine.ProcessBooks(books, new Func<Book, string>(BookFunctions.GetAuthors));

            Console.WriteLine("c) Anonymous method (GetISBN)");
            LibraryEngine.ProcessBooks(books, (Func<Book, string>)delegate (Book B) { return B.ISBN; });

            Console.WriteLine("d) Lambda expression (GetPublicationDate)");
            LibraryEngine.ProcessBooks(books, (Func<Book, string>)(B => B.PublicationDate.ToString("yyyy-MM-dd")));

            Console.WriteLine("Price with built-in delegate");
            LibraryEngine.ProcessBooks(books, (Func<Book, string>)BookFunctions.GetPrice);
        }

        static void Part1()
        {
            Order order = MakeOrder();
            Console.WriteLine(OrderPricing.CalculateOrderPrice(order, OrderPricing.CalculateTotal));
            Console.WriteLine(OrderPricing.CalculateOrderPrice(order, OrderPricing.CalculateTotalWithDiscount));
        }

        static void Part2()
        {
            Order order = MakeOrder();
            Func<Order, decimal> normal = x => x.Price * x.Quantity;
            Func<Order, decimal> discounted = x => x.Price * x.Quantity * 0.9m;
            Console.WriteLine(OrderPricing.CalculateOrderPrice(order, normal));
            Console.WriteLine(OrderPricing.CalculateOrderPrice(order, discounted));
            Console.WriteLine("Func<> solves the need to declare a new custom delegate type for every signature. The framework already provides generic delegates for common shapes, so you write less code and every developer recognizes the type.");
        }

        static void Part3()
        {
            Order order = MakeOrder();
            Console.WriteLine(OrderValidator.ValidateOrder(order, o => o.Quantity > 0));
            Console.WriteLine(OrderValidator.ValidateOrder(order, o => o.Price > 0));
            Console.WriteLine(OrderValidator.ValidateOrder(order, o => !string.IsNullOrWhiteSpace(o.CustomerName)));
            Console.WriteLine("Predicate<Order> states the intent: it is a test or rule that answers yes or no about an order. Func<Order, bool> has the same shape but says nothing about purpose, so a Predicate reads as a validation rule and makes the API self-documenting.");
        }

        static void Part4()
        {
            Order order = MakeOrder();
            Action<Order> printOrder = o => Console.WriteLine($"Order {o.Id} processed.");
            Action<Order> sendConfirmation = o => Console.WriteLine($"Confirmation sent to {o.CustomerName}.");
            Action<Order> audit = o => Console.WriteLine($"Audit: order {o.Id} at {DateTime.Now:T}");
            OrderActions.ProcessOrder(order, printOrder);
            OrderActions.ProcessOrder(order, sendConfirmation);
            OrderActions.ProcessOrder(order, audit);
            Console.WriteLine("Different behaviors were passed to the same method without changing ProcessOrder.");
        }

        static void Part5()
        {
            Order order = MakeOrder();
            OrderService service = new OrderService(OrderPricing.CalculateTotal);
            service.OrderProcessed += o => Console.WriteLine($"Message: order {o.Id} completed.");
            service.OrderProcessed += o => Console.WriteLine($"Notification sent to {o.CustomerName}.");
            service.OrderProcessed += o => Console.WriteLine($"Audit: order {o.Id} logged.");
            service.ProcessOrder(order);
        }

        static void Handler1(Order o)
        {
            Console.WriteLine($"Handler1 called for order {o.Id}");
        }

        static void Handler2(Order o)
        {
            Console.WriteLine($"Handler2 called for order {o.Id}");
        }

        static void Part6()
        {
            Order order = MakeOrder();
            OrderService service = new OrderService(OrderPricing.CalculateTotal);
            service.OrderProcessed += Handler1;
            service.OrderProcessed += Handler2;
            Console.WriteLine("Both subscribed:");
            service.ProcessOrder(order);
            service.OrderProcessed -= Handler1;
            Console.WriteLine("After unsubscribing Handler1:");
            service.ProcessOrder(order);
        }

        static void Part7()
        {
            Order order = MakeOrder();
            Predicate<Order> isValid = o => o.Quantity > 0 && o.Price > 0 && !string.IsNullOrWhiteSpace(o.CustomerName);
            Func<Order, decimal> pricing = o => o.Price * o.Quantity;
            OrderService service = new OrderService(pricing);
            service.OrderProcessed += o => Console.WriteLine($"Handler1: order {o.Id} printed.");
            service.OrderProcessed += o => Console.WriteLine($"Handler2: notification sent to {o.CustomerName}.");
            service.OrderProcessed += o => Console.WriteLine($"Handler3: audit entry for order {o.Id}.");

            Console.WriteLine("Validate Order");
            bool valid = OrderValidator.ValidateOrder(order, isValid);
            Console.WriteLine($"Valid: {valid}");
            Console.WriteLine("Calculate Price");
            Console.WriteLine($"Price: {service.GetPrice(order):C}");
            Console.WriteLine("Process Order");
            Action<Order> process = service.ProcessOrder;
            Predicate<Order> gate = o => valid;
            Dictionary<bool, Action<Order>> flow = new Dictionary<bool, Action<Order>>
            {
                { true, process },
                { false, o => Console.WriteLine("Order rejected.") }
            };
            flow[gate(order)](order);
        }

        static void Questions()
        {
            Console.WriteLine("Q1: PriceCalculator is a custom delegate type declared for one specific signature (Order -> decimal). Func<Order, decimal> is a built-in generic delegate with the same signature. They are equivalent in behavior, but they are different types and are not interchangeable without conversion. Func avoids declaring a new type.");
            Console.WriteLine("Q2: Action<Order> takes an Order and returns nothing (void), so it is used to perform work and side effects. Func<Order, decimal> takes an Order and returns a decimal, so it is used to compute a value.");
            Console.WriteLine("Q3: Predicate<T> returns bool because it represents a condition or test on a value. It is designed for problems like validating, filtering, searching and matching, for example List<T>.Find and RemoveAll.");
            Console.WriteLine("Q4: A delegate is a type that references methods and can be invoked and reassigned by anyone with access to it. An event is a member built on a delegate that restricts outside code to subscribing and unsubscribing with += and -=. Only the declaring class can raise it.");
            Console.WriteLine("Q5: The event keyword limits access. Outside the declaring class you can only use += and -=. You cannot invoke it, assign it with = or clear it, which protects the publisher from outside interference and keeps the class in control of when the event is raised.");
            Console.WriteLine("Q6: All subscribed handlers are stored in the invocation list (multicast delegate) and are called one after another in the order they were subscribed, each receiving the same arguments. If one handler throws, the remaining handlers are not called.");
            Console.WriteLine("Q7: orderService.OrderProcessed += HandleOrderProcessed; OrderProcessed is the event on the service. += means add a subscriber to the event's invocation list without replacing the existing ones. = is the assignment part of the += operator, which combines the current list with the new handler. HandleOrderProcessed is the method that will be called when the event is raised, and it must match the event's delegate signature.");
            Console.WriteLine("Q8: Action<Order> exposed as a public field or property is just a delegate: any outside code can invoke it, replace it with = or wipe out other subscribers. event Action<Order> exposes only += and -= to outside code, and only the owning class can raise it. We use an event to protect the publisher and keep encapsulation.");
        }

        static void Bonus()
        {
            Order order = MakeOrder();
            Dictionary<string, Func<Order, decimal>> strategies = new Dictionary<string, Func<Order, decimal>>
            {
                { "Normal Price", o => o.Price * o.Quantity },
                { "10% Discount", o => o.Price * o.Quantity * 0.9m },
                { "20% Discount", o => o.Price * o.Quantity * 0.8m },
                { "VIP Discount", o => o.Price * o.Quantity * 0.7m }
            };

            foreach (KeyValuePair<string, Func<Order, decimal>> s in strategies)
            {
                OrderService service = new OrderService(s.Value);
                service.OrderProcessed += o => Console.WriteLine($"{s.Key} done for order {o.Id}");
                Console.WriteLine($"{s.Key}: {service.GetPrice(order):C}");
                service.ProcessOrder(order);
            }
            Console.WriteLine("OrderService never knows how the price is calculated. The caller passes the Func<Order, decimal> strategy, so new pricing can be added without modifying OrderService.");
        }

        static void Main()
        {
            Dictionary<string, Action> menu = new Dictionary<string, Action>
            {
                { "1", Section1 },
                { "2", Section1Q2 },
                { "3", Part1 },
                { "4", Part2 },
                { "5", Part3 },
                { "6", Part4 },
                { "7", Part5 },
                { "8", Part6 },
                { "9", Part7 },
                { "10", Questions },
                { "11", Bonus }
            };

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 Section 1 Q1 (Book)");
                Console.WriteLine("2 Section 1 Q2 (delegates with ProcessBooks)");
                Console.WriteLine("3 Part 1 User-defined delegate");
                Console.WriteLine("4 Part 2 Func");
                Console.WriteLine("5 Part 3 Predicate");
                Console.WriteLine("6 Part 4 Action");
                Console.WriteLine("7 Part 5 Events");
                Console.WriteLine("8 Part 6 Event subscription");
                Console.WriteLine("9 Part 7 Final application");
                Console.WriteLine("10 Questions Q1-Q8");
                Console.WriteLine("11 Bonus pricing strategies");
                Console.WriteLine("0 Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();
                if (choice == "0") break;
                Console.WriteLine();
                if (choice != null && menu.TryGetValue(choice, out Action run)) run();
                else Console.WriteLine("Invalid choice.");
            }
        }
    }
}
