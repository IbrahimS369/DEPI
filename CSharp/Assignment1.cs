using System;

class Person
{
    public string Name;
}

class Program
{
    static void Main()
    {
        Console.Write("Enter a number: ");
        int n;
        if (int.TryParse(Console.ReadLine(), out n))
            Console.WriteLine("You entered: " + n);
        else
            Console.WriteLine("Not a valid number");

        string s = "123abc";
        try
        {
            int x = Convert.ToInt32(s);
            Console.WriteLine(x);
        }
        catch (FormatException e)
        {
            Console.WriteLine("FormatException: " + e.Message);
        }
        int r;
        Console.WriteLine(int.TryParse(s, out r) ? "Parsed " + r : "TryParse returned false, value = " + r);
        Console.WriteLine("Convert.ToInt32 throws FormatException because the string has non-numeric characters, TryParse returns false and gives 0");

        double a = 0.1, b = 0.2;
        double sum = a + b;
        Console.WriteLine(sum);
        Console.WriteLine(sum == 0.3);
        Console.WriteLine(Math.Abs(sum - 0.3) < 1e-9);
        Console.WriteLine("The result is 0.30000000000000004 so == 0.3 is false, because floating-point numbers cannot store 0.1 exactly");

        string text = "Hello, World!";
        Console.WriteLine(text.Substring(7));
        Console.WriteLine(text.Substring(0, 5));

        int v1 = 10;
        int v2 = v1;
        v2 = 20;
        Console.WriteLine("v1 = " + v1 + ", v2 = " + v2);
        Console.WriteLine("v1 stays 10 because each value type variable holds its own copy");

        Person p1 = new Person { Name = "Ali" };
        Person p2 = p1;
        p2.Name = "Omar";
        Console.WriteLine("p1.Name = " + p1.Name + ", p2.Name = " + p2.Name);
        Console.WriteLine("Both are Omar because both variables point to the same object");

        string first = "Hello", second = "World";
        string joined = first + " " + second;
        Console.WriteLine(joined);

        // ********************************************************

        int d = Convert.ToInt32(!(30 < 20));
        Console.WriteLine(d);
        Console.WriteLine("Answer 8: B, a value 1 will be assigned to d");

        Console.WriteLine(13 / 2 + " " + 13 % 2);
        Console.WriteLine("Answer 9: D, 6 1");

        int num = 1, z = 5;
        if (!(num <= 0))
            Console.WriteLine(++num + z++ + " " + ++z);
        else
            Console.WriteLine(--num + z-- + " " + --z);
        Console.WriteLine("Answer 10: D, 7 7");
    }
}
