using System;
using System.Collections;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("1. Optimized Bubble Sort");
            Console.WriteLine("2. Range<T>");
            Console.WriteLine("3. Reverse ArrayList in-place");
            Console.WriteLine("4. Even numbers filter");
            Console.WriteLine("5. FixedSizeList<T>");
            Console.WriteLine("6. First non-repeated character");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();
            Console.WriteLine();
            switch (choice)
            {
                case "1": Part1(); break;
                case "2": Part2(); break;
                case "3": Part3(); break;
                case "4": Part4(); break;
                case "5": Part5(); break;
                case "6": Part6(); break;
                case "0": return;
                default: Console.WriteLine("Invalid choice."); break;
            }
            Console.WriteLine();
        }
    }

    static void Part1()
    {
        Console.WriteLine("Bubble Sort is O(n^2) in the worst and average cases because it keeps making full passes even when the array is already sorted.");
        Console.WriteLine("Optimization 1 (early exit): track a swapped flag on each pass. If no swaps happen in a full pass, the array is already sorted and we stop immediately, making the best case O(n).");
        Console.WriteLine("Optimization 2 (shrink the range): after every pass, the largest unsorted element bubbles to its final position, so each pass can ignore the last i elements and run on a shorter range.");
        Console.WriteLine("Both optimizations together reduce the number of comparisons without changing the result.");

        int[] arr = { 64, 34, 25, 12, 22, 11, 90 };
        Console.WriteLine("Before: " + string.Join(", ", arr));
        OptimizedBubbleSort(arr);
        Console.WriteLine("After:  " + string.Join(", ", arr));
    }

    static void OptimizedBubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                    swapped = true;
                }
            }
            if (!swapped)
                break;
        }
    }

    static void Part2()
    {
        Range<int> intRange = new Range<int>(1, 10);
        Console.WriteLine("Range [1, 10]");
        Console.WriteLine("IsInRange(5): " + intRange.IsInRange(5));
        Console.WriteLine("IsInRange(15): " + intRange.IsInRange(15));
        Console.WriteLine("Length: " + intRange.Length());

        Range<double> doubleRange = new Range<double>(2.5, 7.5);
        Console.WriteLine("Range [2.5, 7.5]");
        Console.WriteLine("IsInRange(3.14): " + doubleRange.IsInRange(3.14));
        Console.WriteLine("Length: " + doubleRange.Length());
    }

    static void Part3()
    {
        ArrayList list = new ArrayList { 1, 2, 3, 4, 5 };
        Console.Write("Before: ");
        PrintArrayList(list);
        ReverseArrayList(list);
        Console.Write("After:  ");
        PrintArrayList(list);
    }

    static void ReverseArrayList(ArrayList list)
    {
        int left = 0;
        int right = list.Count - 1;
        while (left < right)
        {
            object temp = list[left];
            list[left] = list[right];
            list[right] = temp;
            left++;
            right--;
        }
    }

    static void PrintArrayList(ArrayList list)
    {
        Console.WriteLine(string.Join(", ", list.ToArray()));
    }

    static void Part4()
    {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        List<int> evens = GetEvenNumbers(numbers);
        Console.WriteLine("Input: " + string.Join(", ", numbers));
        Console.WriteLine("Evens: " + string.Join(", ", evens));
    }

    static List<int> GetEvenNumbers(List<int> numbers)
    {
        List<int> evens = new List<int>();
        foreach (int n in numbers)
        {
            if (n % 2 == 0)
                evens.Add(n);
        }
        return evens;
    }

    static void Part5()
    {
        FixedSizeList<string> list = new FixedSizeList<string>(3);
        list.Add("A");
        list.Add("B");
        Console.WriteLine("Added A, B. Get(0): " + list.Get(0) + ", Get(1): " + list.Get(1));

        try
        {
            list.Add("C");
            list.Add("D");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Exceeding capacity -> " + ex.Message);
        }

        try
        {
            list.Get(10);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine("Invalid index -> " + ex.Message);
        }
    }

    static void Part6()
    {
        string input = "swiss";
        int index = FirstNonRepeatedCharIndex(input);
        Console.WriteLine("Input: " + input);
        Console.WriteLine("First non-repeated character index: " + index);

        string input2 = "aabb";
        int index2 = FirstNonRepeatedCharIndex(input2);
        Console.WriteLine("Input: " + input2);
        Console.WriteLine("First non-repeated character index: " + index2);
    }

    static int FirstNonRepeatedCharIndex(string s)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();
        foreach (char c in s)
        {
            if (counts.ContainsKey(c))
                counts[c]++;
            else
                counts[c] = 1;
        }
        for (int i = 0; i < s.Length; i++)
        {
            if (counts[s[i]] == 1)
                return i;
        }
        return -1;
    }
}

class Range<T> where T : IComparable<T>
{
    public T Min { get; set; }
    public T Max { get; set; }

    public Range(T min, T max)
    {
        Min = min;
        Max = max;
    }

    public bool IsInRange(T value)
    {
        return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
    }

    public dynamic Length()
    {
        return (dynamic)Max - (dynamic)Min;
    }
}

class FixedSizeList<T>
{
    private readonly T[] items;
    private int count;

    public int Capacity { get; }
    public int Count { get { return count; } }

    public FixedSizeList(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");
        Capacity = capacity;
        items = new T[capacity];
        count = 0;
    }

    public void Add(T item)
    {
        if (count >= Capacity)
            throw new InvalidOperationException("The list is full. Cannot add more than " + Capacity + " elements.");
        items[count] = item;
        count++;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException("index", "Invalid index. Valid range is 0 to " + (count - 1) + ".");
        return items[index];
    }
}
