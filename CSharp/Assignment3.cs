using System;

namespace Assignment3
{
    enum WeekDays { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

    enum Season { Spring, Summer, Autumn, Winter }

    [Flags]
    enum Permissions { None = 0, Read = 1, Write = 2, Delete = 4, Execute = 8 }

    enum Colors { Red, Green, Blue }

    struct Person
    {
        public string Name;
        public int Age;
    }

    struct Point
    {
        public double X;
        public double Y;
    }

    class Program
    {
        static void ChangeValue(int x)
        {
            x = 100;
        }

        static void ChangeValueRef(ref int x)
        {
            x = 100;
        }

        static void ChangeArray(int[] arr)
        {
            arr[0] = 100;
            arr = new int[] { 7, 7, 7 };
        }

        static void ChangeArrayRef(ref int[] arr)
        {
            arr[0] = 100;
            arr = new int[] { 7, 7, 7 };
        }

        static void SumAndSub(int a, int b, int c, int d, out int sum, out int sub)
        {
            sum = a + b;
            sub = c - d;
        }

        static int SumOfDigits(int n)
        {
            int sum = 0;
            n = Math.Abs(n);
            while (n > 0)
            {
                sum += n % 10;
                n /= 10;
            }
            return sum;
        }

        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        static void MinMaxArray(int[] arr, ref int min, ref int max)
        {
            min = arr[0];
            max = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < min) min = arr[i];
                if (arr[i] > max) max = arr[i];
            }
        }

        static long Factorial(int n)
        {
            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        static string ChangeChar(string s, int position, char newChar)
        {
            if (position < 0 || position >= s.Length) return s;
            char[] chars = s.ToCharArray();
            chars[position] = newChar;
            return new string(chars);
        }

        static void Func1()
        {
            Console.WriteLine("Passing a value type by value sends a copy, so the original does not change.");
            Console.WriteLine("Passing it by reference (ref) sends the variable itself, so the original changes.");
            int a = 5;
            ChangeValue(a);
            Console.WriteLine("After by value: " + a);
            ChangeValueRef(ref a);
            Console.WriteLine("After by reference: " + a);
        }

        static void Func2()
        {
            Console.WriteLine("Passing a reference type by value sends a copy of the reference.");
            Console.WriteLine("Changing the object's content affects the original, but assigning a new object does not.");
            Console.WriteLine("Passing it by reference (ref) lets the method also replace the original object.");
            int[] arr = { 1, 2, 3 };
            ChangeArray(arr);
            Console.WriteLine("After by value: " + string.Join(", ", arr));
            int[] arr2 = { 1, 2, 3 };
            ChangeArrayRef(ref arr2);
            Console.WriteLine("After by reference: " + string.Join(", ", arr2));
        }

        static void Func3()
        {
            Console.Write("Enter first number: ");
            int a = int.Parse(Console.ReadLine());
            Console.Write("Enter second number: ");
            int b = int.Parse(Console.ReadLine());
            Console.Write("Enter third number: ");
            int c = int.Parse(Console.ReadLine());
            Console.Write("Enter fourth number: ");
            int d = int.Parse(Console.ReadLine());
            int sum, sub;
            SumAndSub(a, b, c, d, out sum, out sub);
            Console.WriteLine("Sum of " + a + " and " + b + " = " + sum);
            Console.WriteLine("Subtraction of " + c + " and " + d + " = " + sub);
        }

        static void Func4()
        {
            Console.Write("Enter a number: ");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("The sum of the digits of the number " + n + " is: " + SumOfDigits(n));
        }

        static void Func5()
        {
            Console.Write("Enter a number: ");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine(IsPrime(n));
        }

        static void Func6()
        {
            Console.Write("Enter array size: ");
            int size = int.Parse(Console.ReadLine());
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
            {
                Console.Write("Enter element " + (i + 1) + ": ");
                arr[i] = int.Parse(Console.ReadLine());
            }
            int min = 0, max = 0;
            MinMaxArray(arr, ref min, ref max);
            Console.WriteLine("Min = " + min);
            Console.WriteLine("Max = " + max);
        }

        static void Func7()
        {
            Console.Write("Enter a number: ");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("Factorial = " + Factorial(n));
        }

        static void Func8()
        {
            Console.Write("Enter a string: ");
            string s = Console.ReadLine();
            Console.Write("Enter position (0 based): ");
            int pos = int.Parse(Console.ReadLine());
            Console.Write("Enter new letter: ");
            char c = Console.ReadLine()[0];
            Console.WriteLine(ChangeChar(s, pos, c));
        }

        static void Enum1()
        {
            foreach (WeekDays day in Enum.GetValues(typeof(WeekDays)))
            {
                Console.WriteLine(day);
            }
        }

        static void Enum2()
        {
            Person[] people = new Person[3];
            for (int i = 0; i < people.Length; i++)
            {
                Console.Write("Enter name of person " + (i + 1) + ": ");
                people[i].Name = Console.ReadLine();
                Console.Write("Enter age of person " + (i + 1) + ": ");
                people[i].Age = int.Parse(Console.ReadLine());
            }
            for (int i = 0; i < people.Length; i++)
            {
                Console.WriteLine("Name: " + people[i].Name + ", Age: " + people[i].Age);
            }
        }

        static void Enum3()
        {
            Console.Write("Enter season name: ");
            Season season;
            if (Enum.TryParse(Console.ReadLine(), true, out season))
            {
                switch (season)
                {
                    case Season.Spring:
                        Console.WriteLine("March to May");
                        break;
                    case Season.Summer:
                        Console.WriteLine("June to August");
                        break;
                    case Season.Autumn:
                        Console.WriteLine("September to November");
                        break;
                    case Season.Winter:
                        Console.WriteLine("December to February");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid season");
            }
        }

        static void Enum4()
        {
            Permissions p = Permissions.Read | Permissions.Write;
            Console.WriteLine("Current: " + p);
            p |= Permissions.Execute;
            Console.WriteLine("After adding Execute: " + p);
            p &= ~Permissions.Write;
            Console.WriteLine("After removing Write: " + p);
            Console.WriteLine("Has Read? " + p.HasFlag(Permissions.Read));
            Console.WriteLine("Has Delete? " + p.HasFlag(Permissions.Delete));
        }

        static void Enum5()
        {
            Console.Write("Enter color name: ");
            Colors color;
            if (Enum.TryParse(Console.ReadLine(), true, out color) && Enum.IsDefined(typeof(Colors), color))
            {
                Console.WriteLine(color + " is a primary color");
            }
            else
            {
                Console.WriteLine("This is not a primary color");
            }
        }

        static void Enum6()
        {
            Point p1 = new Point();
            Point p2 = new Point();
            Console.Write("Enter X of point 1: ");
            p1.X = double.Parse(Console.ReadLine());
            Console.Write("Enter Y of point 1: ");
            p1.Y = double.Parse(Console.ReadLine());
            Console.Write("Enter X of point 2: ");
            p2.X = double.Parse(Console.ReadLine());
            Console.Write("Enter Y of point 2: ");
            p2.Y = double.Parse(Console.ReadLine());
            double distance = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            Console.WriteLine("Distance = " + distance);
        }

        static void Enum7()
        {
            Person[] people = new Person[3];
            for (int i = 0; i < people.Length; i++)
            {
                Console.Write("Enter name of person " + (i + 1) + ": ");
                people[i].Name = Console.ReadLine();
                Console.Write("Enter age of person " + (i + 1) + ": ");
                people[i].Age = int.Parse(Console.ReadLine());
            }
            int oldest = 0;
            for (int i = 1; i < people.Length; i++)
            {
                if (people[i].Age > people[oldest].Age) oldest = i;
            }
            Console.WriteLine("Oldest person: " + people[oldest].Name + ", Age: " + people[oldest].Age);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Functions: F1 to F8 = 1 to 8");
            Console.WriteLine("Enum and Struct: E1 to E7 = 9 to 15");
            Console.Write("Enter question number: ");
            int q = int.Parse(Console.ReadLine());
            switch (q)
            {
                case 1: Func1(); break;
                case 2: Func2(); break;
                case 3: Func3(); break;
                case 4: Func4(); break;
                case 5: Func5(); break;
                case 6: Func6(); break;
                case 7: Func7(); break;
                case 8: Func8(); break;
                case 9: Enum1(); break;
                case 10: Enum2(); break;
                case 11: Enum3(); break;
                case 12: Enum4(); break;
                case 13: Enum5(); break;
                case 14: Enum6(); break;
                case 15: Enum7(); break;
                default: Console.WriteLine("Invalid number"); break;
            }
        }
    }
}
