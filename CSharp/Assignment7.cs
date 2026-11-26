using System;

namespace Session09Assignment
{
    class Patient(int id, string fullName, string phoneNumber, string medicalHistory)
    {
        public int Id { get; } = id;
        public string FullName { get; } = fullName;
        public string PhoneNumber { get; } = phoneNumber;
        public string MedicalHistory { get; } = medicalHistory;

        public override string ToString()
        {
            return $"Id: {Id} :: FullName: {FullName} :: PhoneNumber: {PhoneNumber} :: MedicalHistory: {MedicalHistory}";
        }
    }

    record PatientDto(int Id, string FullName, string PhoneNumber);

    static class PatientMapper
    {
        public static PatientDto MapFromModelToDto(Patient patient)
        {
            return new PatientDto(patient.Id, patient.FullName, patient.PhoneNumber);
        }
    }

    class AppLogger
    {
        private static AppLogger? instance = null;

        private AppLogger()
        {
        }

        public static AppLogger GetLogger()
        {
            if (instance == null)
            {
                instance = new AppLogger();
            }
            return instance;
        }
    }

    static class TextHelper
    {
        public static bool IsShorterThan(this string value, int length)
        {
            return value.Length < length;
        }

        public static string Repeat(this string value, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++)
            {
                result += value;
            }
            return result;
        }
    }

    class FineCalculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public int Divide(int a, int b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException();
            }
            return a / b;
        }
    }

    class Program
    {
        static void Part1()
        {
            Patient patient01 = new Patient(1, "Ali Hassan", "01012345678", "Asthma");
            Patient patient02 = new Patient(1, "Ali Hassan", "01012345678", "Asthma");
            Console.WriteLine(patient01);
            Console.WriteLine(patient01.GetHashCode());
            Console.WriteLine(patient02.GetHashCode());
            Console.WriteLine("Hash codes differ: Patient is a class, so the default hash code is based on the object reference, not the values.");
            Console.WriteLine(patient01.Equals(patient02));
            Console.WriteLine("False: the default Equals on a class compares references, and these are two different objects.");
            patient01 = patient02;
            Console.WriteLine(patient01.Equals(patient02));
            Console.WriteLine("True: now both variables point to the same object, so the references are equal.");

            PatientDto dto01 = new PatientDto(1, "Ali Hassan", "01012345678");
            PatientDto dto02 = new PatientDto(1, "Ali Hassan", "01012345678");
            Console.WriteLine(dto01);
            Console.WriteLine(dto01.GetHashCode());
            Console.WriteLine(dto02.GetHashCode());
            Console.WriteLine(dto01.Equals(dto02));
            Console.WriteLine(dto01 == dto02);
            Console.WriteLine("A record has value-based equality and hash code, so identical values give equal hash codes and Equals returns true. A class compares references.");

            PatientDto mapped = PatientMapper.MapFromModelToDto(patient01);
            Console.WriteLine(mapped);
        }

        static void Part2()
        {
            AppLogger l1 = AppLogger.GetLogger();
            AppLogger l2 = AppLogger.GetLogger();
            AppLogger l3 = AppLogger.GetLogger();
            AppLogger l4 = AppLogger.GetLogger();
            Console.WriteLine(l1.GetHashCode());
            Console.WriteLine(l2.GetHashCode());
            Console.WriteLine(l3.GetHashCode());
            Console.WriteLine(l4.GetHashCode());
            Console.WriteLine("All four hash codes are the same: the constructor is private and GetLogger creates the instance only on the first call, so every call returns the same object.");
        }

        static void Part3()
        {
            Patient p = new(2, "Sara Ahmed", "01198765432", "Diabetes");
            var p2 = new Patient(3, "Omar Khaled", "01255566677", "None");
            dynamic p3 = new Patient(4, "Mona Adel", "01544433322", "Allergy");
            Console.WriteLine(p);
            Console.WriteLine(p2);
            Console.WriteLine(p3);
            Console.WriteLine("var is resolved at compile time: the compiler infers the type from the right side and it never changes. dynamic is resolved at run time: type checking of member access is postponed until the program runs.");
        }

        static void Part4()
        {
            var doctor01 = new { Name = "Sara", Specialty = "Cardiology", ExperienceYears = 8, Salary = 25_000 };
            var doctor02 = new { Name = "Sara", Specialty = "Cardiology", ExperienceYears = 8, Salary = 25_000 };
            Console.WriteLine(doctor01.Name);
            Console.WriteLine(doctor01.Specialty);
            Console.WriteLine(doctor01.GetHashCode());
            Console.WriteLine(doctor02.GetHashCode());
            Console.WriteLine(doctor01.GetType());
            Console.WriteLine(doctor01.Equals(doctor02));
            Console.WriteLine(doctor01.ToString());
            Console.WriteLine("Anonymous types use value-based equality like records: same property names, types and values give equal objects and equal hash codes. A normal class compares references by default. A record compares values too, but it is a named type and can be extended.");
        }

        static void Part5()
        {
            Console.WriteLine("Stethoscope".IsShorterThan(5));
            Console.WriteLine("Ab".Repeat(4));
        }

        static void Part6()
        {
            FineCalculator calc = new FineCalculator();
            Console.WriteLine(calc.Add(2, 3));
            Console.WriteLine(calc.Subtract(5, 3));
            Console.WriteLine(calc.Multiply(-2, 4));
            try
            {
                calc.Divide(5, 0);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("DivideByZeroException thrown");
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1: Part 1, 2: Part 2, 3: Part 3, 4: Part 4, 5: Part 5, 6: Part 6, 0: Exit");
                string? input = Console.ReadLine();
                if (input == "0") break;
                switch (input)
                {
                    case "1": Part1(); break;
                    case "2": Part2(); break;
                    case "3": Part3(); break;
                    case "4": Part4(); break;
                    case "5": Part5(); break;
                    case "6": Part6(); break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }
    }
}

#if XUNIT
namespace Session09Assignment.Tests
{
    using Xunit;

    public class FineCalculatorTests
    {
        private readonly FineCalculator _calculator;

        public FineCalculatorTests()
        {
            _calculator = new FineCalculator();
        }

        [Fact]
        public void Add_ShouldReturnCorrectSum()
        {
            int a = 2, b = 3;
            int result = _calculator.Add(a, b);
            Assert.Equal(5, result);
        }

        [Fact]
        public void Add_ShouldReturnNotEqual()
        {
            int a = 2, b = 3;
            int result = _calculator.Add(a, b);
            Assert.NotEqual(6, result);
        }

        [Fact]
        public void Subtract_ShouldReturnCorrectDifference()
        {
            int a = 5, b = 3;
            int result = _calculator.Subtract(a, b);
            Assert.Equal(2, result);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(0, 5, 0)]
        [InlineData(-2, 4, -8)]
        public void Multiply_ShouldReturnCorrectResult(int a, int b, int expected)
        {
            int result = _calculator.Multiply(a, b);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2, 3, 7)]
        [InlineData(0, 5, 1)]
        [InlineData(-2, 4, 8)]
        public void Multiply_ShouldReturnNotEqual(int a, int b, int notExpected)
        {
            int result = _calculator.Multiply(a, b);
            Assert.NotEqual(notExpected, result);
        }

        [Fact]
        public void Divide_ByZero_ShouldThrowException()
        {
            int a = 5, b = 0;
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(a, b));
        }
    }
}
#endif
