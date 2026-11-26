using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter question number (1-28): ");
        int q = int.Parse(Console.ReadLine());
        switch (q)
        {
            case 1: Q1(); break;
            case 2: Q2(); break;
            case 3: Q3(); break;
            case 4: Q4(); break;
            case 5: Q5(); break;
            case 6: Q6(); break;
            case 7: Q7(); break;
            case 8: Q8(); break;
            case 9: Q9(); break;
            case 10: Q10(); break;
            case 11: Q11(); break;
            case 12: Q12(); break;
            case 13: Q13(); break;
            case 14: Q14(); break;
            case 15: Q15(); break;
            case 16: Q16(); break;
            case 17: Q17(); break;
            case 18: Q18(); break;
            case 19: Q19(); break;
            case 20: Q20(); break;
            case 21: Q21(); break;
            case 22: Q22(); break;
            case 23: Q23(); break;
            case 24: Q24(); break;
            case 25: Q25(); break;
            case 26: Q26(); break;
            case 27: Q27(); break;
            case 28: Q28(); break;
        }
    }

    static void Q1()
    {
        Console.Write("Enter a number: ");
        int n = int.Parse(Console.ReadLine());
        if (n % 3 == 0 && n % 4 == 0)
            Console.WriteLine("Yes");
        else
            Console.WriteLine("No");
    }

    static void Q2()
    {
        Console.Write("Enter an integer: ");
        int n = int.Parse(Console.ReadLine());
        if (n < 0)
            Console.WriteLine("negative");
        else
            Console.WriteLine("positive");
    }

    static void Q3()
    {
        Console.Write("Enter 3 integers: ");
        string[] parts = Console.ReadLine().Split(' ', ',');
        int a = int.Parse(parts[0]);
        int b = int.Parse(parts[1]);
        int c = int.Parse(parts[2]);
        int max = a;
        if (b > max) max = b;
        if (c > max) max = c;
        int min = a;
        if (b < min) min = b;
        if (c < min) min = c;
        Console.WriteLine("max element = " + max);
        Console.WriteLine("min element = " + min);
    }

    static void Q4()
    {
        Console.Write("Enter an integer: ");
        int n = int.Parse(Console.ReadLine());
        if (n % 2 == 0)
            Console.WriteLine("even");
        else
            Console.WriteLine("odd");
    }

    static void Q5()
    {
        Console.Write("Enter a character: ");
        char ch = char.Parse(Console.ReadLine());
        ch = char.ToLower(ch);
        if (ch == 'a' || ch == 'e' || ch == 'i' || ch == 'o' || ch == 'u')
            Console.WriteLine("vowel");
        else
            Console.WriteLine("consonant");
    }

    static void Q6()
    {
        Console.Write("Enter a number: ");
        int n = int.Parse(Console.ReadLine());
        for (int i = 1; i <= n; i++)
        {
            Console.Write(i);
            if (i < n) Console.Write(", ");
        }
        Console.WriteLine();
    }

    static void Q7()
    {
        Console.Write("Enter a number: ");
        int n = int.Parse(Console.ReadLine());
        for (int i = 1; i <= 12; i++)
            Console.Write(n * i + " ");
        Console.WriteLine();
    }

    static void Q8()
    {
        Console.Write("Enter a number: ");
        int n = int.Parse(Console.ReadLine());
        for (int i = 2; i <= n; i += 2)
            Console.Write(i + " ");
        Console.WriteLine();
    }

    static void Q9()
    {
        Console.Write("Enter two integers: ");
        string[] parts = Console.ReadLine().Split(' ');
        int b = int.Parse(parts[0]);
        int p = int.Parse(parts[1]);
        int result = 1;
        for (int i = 0; i < p; i++)
            result *= b;
        Console.WriteLine(result);
    }

    static void Q10()
    {
        Console.Write("Enter Marks of five subjects: ");
        string[] parts = Console.ReadLine().Split(' ');
        int total = 0;
        for (int i = 0; i < 5; i++)
            total += int.Parse(parts[i]);
        double avg = total / 5.0;
        Console.WriteLine("Total marks = " + total);
        Console.WriteLine("Average Marks = " + (int)avg);
        Console.WriteLine("Percentage = " + (int)avg);
    }

    static void Q11()
    {
        Console.Write("Month Number: ");
        int m = int.Parse(Console.ReadLine());
        int days;
        if (m == 2)
            days = 28;
        else if (m == 4 || m == 6 || m == 9 || m == 11)
            days = 30;
        else
            days = 31;
        Console.WriteLine("Days in Month: " + days);
    }

    static void Q12()
    {
        Console.Write("Enter first number: ");
        double x = double.Parse(Console.ReadLine());
        Console.Write("Enter operator (+ - * /): ");
        char op = char.Parse(Console.ReadLine());
        Console.Write("Enter second number: ");
        double y = double.Parse(Console.ReadLine());
        double result = 0;
        switch (op)
        {
            case '+': result = x + y; break;
            case '-': result = x - y; break;
            case '*': result = x * y; break;
            case '/': result = x / y; break;
        }
        Console.WriteLine("Result = " + result);
    }

    static void Q13()
    {
        Console.Write("Enter a string: ");
        string s = Console.ReadLine();
        for (int i = s.Length - 1; i >= 0; i--)
            Console.Write(s[i]);
        Console.WriteLine();
    }

    static void Q14()
    {
        Console.Write("Enter an integer: ");
        int n = int.Parse(Console.ReadLine());
        int rev = 0;
        while (n != 0)
        {
            rev = rev * 10 + n % 10;
            n /= 10;
        }
        Console.WriteLine(rev);
    }

    static void Q15()
    {
        Console.Write("Input starting number of range: ");
        int start = int.Parse(Console.ReadLine());
        Console.Write("Input ending number of range: ");
        int end = int.Parse(Console.ReadLine());
        Console.WriteLine("The prime numbers between " + start + " and " + end + " are:");
        for (int n = start; n <= end; n++)
        {
            if (n < 2) continue;
            bool prime = true;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    prime = false;
                    break;
                }
            }
            if (prime) Console.Write(n + " ");
        }
        Console.WriteLine();
    }

    static void Q16()
    {
        Console.Write("Enter a number to convert: ");
        int n = int.Parse(Console.ReadLine());
        string binary = "";
        while (n > 0)
        {
            binary = (n % 2) + binary;
            n /= 2;
        }
        Console.WriteLine("The Binary is " + binary);
    }

    static void Q17()
    {
        Console.Write("Enter x1 y1: ");
        string[] p1 = Console.ReadLine().Split(' ');
        Console.Write("Enter x2 y2: ");
        string[] p2 = Console.ReadLine().Split(' ');
        Console.Write("Enter x3 y3: ");
        string[] p3 = Console.ReadLine().Split(' ');
        int x1 = int.Parse(p1[0]), y1 = int.Parse(p1[1]);
        int x2 = int.Parse(p2[0]), y2 = int.Parse(p2[1]);
        int x3 = int.Parse(p3[0]), y3 = int.Parse(p3[1]);
        if ((y2 - y1) * (x3 - x1) == (y3 - y1) * (x2 - x1))
            Console.WriteLine("The points lie on a single straight line");
        else
            Console.WriteLine("The points do not lie on a single straight line");
    }

    static void Q18()
    {
        Console.Write("Enter the time taken (in hours): ");
        double hours = double.Parse(Console.ReadLine());
        if (hours >= 2 && hours <= 3)
            Console.WriteLine("Highly efficient");
        else if (hours > 3 && hours <= 4)
            Console.WriteLine("Increase your speed");
        else if (hours > 4 && hours <= 5)
            Console.WriteLine("Training to enhance speed");
        else if (hours > 5)
            Console.WriteLine("Required to leave the company");
    }

    static void Q19()
    {
        Console.Write("Enter n: ");
        int n = int.Parse(Console.ReadLine());
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                    Console.Write("1 ");
                else
                    Console.Write("0 ");
            }
            Console.WriteLine();
        }
    }

    static void Q20()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] arr = new int[n];
        Console.Write("Enter " + n + " elements: ");
        string[] parts = Console.ReadLine().Split(' ');
        int sum = 0;
        for (int i = 0; i < n; i++)
        {
            arr[i] = int.Parse(parts[i]);
            sum += arr[i];
        }
        Console.WriteLine("Sum = " + sum);
    }

    static void Q21()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] a = new int[n];
        int[] b = new int[n];
        int[] merged = new int[2 * n];
        Console.Write("Enter first array: ");
        string[] p1 = Console.ReadLine().Split(' ');
        Console.Write("Enter second array: ");
        string[] p2 = Console.ReadLine().Split(' ');
        for (int i = 0; i < n; i++)
        {
            a[i] = int.Parse(p1[i]);
            b[i] = int.Parse(p2[i]);
        }
        for (int i = 0; i < n; i++)
        {
            merged[i] = a[i];
            merged[n + i] = b[i];
        }
        Array.Sort(merged);
        Console.WriteLine("Merged sorted array:");
        for (int i = 0; i < 2 * n; i++)
            Console.Write(merged[i] + " ");
        Console.WriteLine();
    }

    static void Q22()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] arr = new int[n];
        bool[] counted = new bool[n];
        Console.Write("Enter elements: ");
        string[] parts = Console.ReadLine().Split(' ');
        for (int i = 0; i < n; i++)
            arr[i] = int.Parse(parts[i]);
        for (int i = 0; i < n; i++)
        {
            if (counted[i]) continue;
            int freq = 1;
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j] == arr[i])
                {
                    freq++;
                    counted[j] = true;
                }
            }
            Console.WriteLine(arr[i] + " occurs " + freq + " time(s)");
        }
    }

    static void Q23()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] arr = new int[n];
        Console.Write("Enter elements: ");
        string[] parts = Console.ReadLine().Split(' ');
        for (int i = 0; i < n; i++)
            arr[i] = int.Parse(parts[i]);
        int max = arr[0], min = arr[0];
        for (int i = 1; i < n; i++)
        {
            if (arr[i] > max) max = arr[i];
            if (arr[i] < min) min = arr[i];
        }
        Console.WriteLine("Maximum element = " + max);
        Console.WriteLine("Minimum element = " + min);
    }

    static void Q24()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] arr = new int[n];
        Console.Write("Enter elements: ");
        string[] parts = Console.ReadLine().Split(' ');
        for (int i = 0; i < n; i++)
            arr[i] = int.Parse(parts[i]);
        int first = int.MinValue, second = int.MinValue;
        for (int i = 0; i < n; i++)
        {
            if (arr[i] > first)
            {
                second = first;
                first = arr[i];
            }
            else if (arr[i] > second && arr[i] < first)
            {
                second = arr[i];
            }
        }
        Console.WriteLine("Second largest element = " + second);
    }

    static void Q25()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] arr = new int[n];
        Console.Write("Enter elements: ");
        string[] parts = Console.ReadLine().Split(' ');
        for (int i = 0; i < n; i++)
            arr[i] = int.Parse(parts[i]);
        int longest = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = n - 1; j > i; j--)
            {
                if (arr[j] == arr[i])
                {
                    int dist = j - i - 1;
                    if (dist > longest) longest = dist;
                    break;
                }
            }
        }
        Console.WriteLine("Longest distance = " + longest);
    }

    static void Q26()
    {
        Console.Write("Enter a sentence: ");
        string s = Console.ReadLine();
        string[] words = s.Split(' ');
        Console.WriteLine(string.Join(" ", words.Reverse()));
    }

    static void Q27()
    {
        Console.Write("Enter rows: ");
        int rows = int.Parse(Console.ReadLine());
        Console.Write("Enter columns: ");
        int cols = int.Parse(Console.ReadLine());
        int[,] first = new int[rows, cols];
        int[,] second = new int[rows, cols];
        Console.WriteLine("Enter elements of first array:");
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                first[i, j] = int.Parse(Console.ReadLine());
            }
        }
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                second[i, j] = first[i, j];
            }
        }
        Console.WriteLine("Second array:");
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
                Console.Write(second[i, j] + " ");
            Console.WriteLine();
        }
    }

    static void Q28()
    {
        Console.Write("Enter array size: ");
        int n = int.Parse(Console.ReadLine());
        int[] arr = new int[n];
        Console.Write("Enter elements: ");
        string[] parts = Console.ReadLine().Split(' ');
        for (int i = 0; i < n; i++)
            arr[i] = int.Parse(parts[i]);
        Console.WriteLine("Array in reverse order:");
        for (int i = n - 1; i >= 0; i--)
            Console.Write(arr[i] + " ");
        Console.WriteLine();
    }
}
