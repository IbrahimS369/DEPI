using System;

namespace Assignment4
{
    enum SecurityLevel
    {
        Guest = 1,
        Developer = 2,
        Secretary = 4,
        DBA = 8,
        SecurityOfficer = 15
    }

    class HiringDate
    {
        private int day;
        private int month;
        private int year;

        public HiringDate()
        {
            day = 1;
            month = 1;
            year = 2000;
        }

        public HiringDate(int d, int m, int y)
        {
            Year = y;
            Month = m;
            Day = d;
        }

        public int Day
        {
            get { return day; }
            set
            {
                if (value >= 1 && value <= DateTime.DaysInMonth(year, month))
                    day = value;
                else
                    day = 1;
            }
        }

        public int Month
        {
            get { return month; }
            set
            {
                if (value >= 1 && value <= 12)
                    month = value;
                else
                    month = 1;
            }
        }

        public int Year
        {
            get { return year; }
            set
            {
                if (value >= 1900 && value <= 9999)
                    year = value;
                else
                    year = 2000;
            }
        }

        public override string ToString()
        {
            return Day + "/" + Month + "/" + Year;
        }
    }

    class Employee
    {
        private int id;
        private string name;
        private SecurityLevel level;
        private decimal salary;
        private HiringDate hireDate;
        private char gender;

        public Employee()
        {
            id = 0;
            name = "Unknown";
            level = SecurityLevel.Guest;
            salary = 0;
            hireDate = new HiringDate();
            gender = 'M';
        }

        public Employee(int id, string name, SecurityLevel level, decimal salary, HiringDate hireDate, char gender)
        {
            ID = id;
            Name = name;
            Level = level;
            Salary = salary;
            HireDate = hireDate;
            Gender = gender;
        }

        public int ID
        {
            get { return id; }
            set { id = value < 0 ? 0 : value; }
        }

        public string Name
        {
            get { return name; }
            set { name = string.IsNullOrWhiteSpace(value) ? "Unknown" : value; }
        }

        public SecurityLevel Level
        {
            get { return level; }
            set { level = value; }
        }

        public decimal Salary
        {
            get { return salary; }
            set { salary = value < 0 ? 0 : value; }
        }

        public HiringDate HireDate
        {
            get { return hireDate; }
            set { hireDate = value == null ? new HiringDate() : value; }
        }

        public char Gender
        {
            get { return gender; }
            set
            {
                char g = char.ToUpper(value);
                if (g == 'M' || g == 'F')
                    gender = g;
                else
                    gender = 'M';
            }
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Name: {1}, Security Level: {2}, Salary: {3:C}, Hire Date: {4}, Gender: {5}",
                ID, Name, Level, Salary, HireDate, Gender);
        }
    }

    class Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Shape(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Area()
        {
            return Width * Height;
        }

        public override string ToString()
        {
            return "(Width = " + Width + ", Height = " + Height + ")";
        }
    }

    class Cube : Shape
    {
        public double Depth { get; set; }

        public Cube(double width, double height, double depth) : base(width, height)
        {
            Depth = depth;
        }

        public new double Area()
        {
            return base.Area() * Depth;
        }

        public void Print()
        {
            Console.WriteLine("Width = " + Width + ", Height = " + Height + ", Depth = " + Depth);
        }

        public override string ToString()
        {
            return "(Width = " + Width + ", Height = " + Height + ", Depth = " + Depth + ")";
        }
    }

    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public void Greet()
        {
            Console.WriteLine("I am a person's basic data. Person.");
        }

        public virtual void Display()
        {
            Console.WriteLine("ID: " + ID + ", Name: " + Name + ", Age: " + Age);
        }
    }

    class Doctor : Person
    {
        public string Specialty { get; set; }

        public new void Greet()
        {
            Console.WriteLine("I am a doctor.");
        }

        public override void Display()
        {
            Console.WriteLine("ID: " + ID + ", Name: " + Name + ", Age: " + Age + ", Specialty: " + Specialty);
        }
    }

    class Engineer : Person
    {
        public string Field { get; set; }

        public new void Greet()
        {
            Console.WriteLine("I am an engineer.");
        }

        public override void Display()
        {
            Console.WriteLine("ID: " + ID + ", Name: " + Name + ", Age: " + Age + ", Field: " + Field);
        }
    }

    class Program
    {
        static int ReadInt(string message)
        {
            int result;
            Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Invalid input, try again: ");
            }
            return result;
        }

        static decimal ReadDecimal(string message)
        {
            decimal result;
            Console.Write(message);
            while (!decimal.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Invalid input, try again: ");
            }
            return result;
        }

        static char ReadGender()
        {
            Console.Write("Gender (M/F): ");
            string s = Console.ReadLine();
            while (string.IsNullOrEmpty(s) || (char.ToUpper(s[0]) != 'M' && char.ToUpper(s[0]) != 'F'))
            {
                Console.Write("Gender must be M or F: ");
                s = Console.ReadLine();
            }
            return char.ToUpper(s[0]);
        }

        static SecurityLevel ReadLevel()
        {
            Console.WriteLine("1 Guest, 2 Developer, 3 Secretary, 4 DBA, 5 Security Officer");
            int c = ReadInt("Choose security level: ");
            while (c < 1 || c > 5)
            {
                c = ReadInt("Choose a number from 1 to 5: ");
            }
            switch (c)
            {
                case 1: return SecurityLevel.Guest;
                case 2: return SecurityLevel.Developer;
                case 3: return SecurityLevel.Secretary;
                case 4: return SecurityLevel.DBA;
                default: return SecurityLevel.SecurityOfficer;
            }
        }

        static void Part1()
        {
            Employee[] EmpArr = new Employee[3];
            SecurityLevel[] levels = { SecurityLevel.DBA, SecurityLevel.Guest, SecurityLevel.SecurityOfficer };
            string[] titles = { "DBA", "Guest", "Security Officer" };

            for (int i = 0; i < EmpArr.Length; i++)
            {
                Console.WriteLine("Enter data of the " + titles[i] + " employee");
                int id = ReadInt("ID: ");
                Console.Write("Name: ");
                string name = Console.ReadLine();
                decimal salary = ReadDecimal("Salary: ");
                int d = ReadInt("Hire day: ");
                int m = ReadInt("Hire month: ");
                int y = ReadInt("Hire year: ");
                char g = ReadGender();
                EmpArr[i] = new Employee(id, name, levels[i], salary, new HiringDate(d, m, y), g);
            }

            Console.WriteLine();
            for (int i = 0; i < EmpArr.Length; i++)
            {
                Console.WriteLine(EmpArr[i]);
            }
        }

        static void Q1()
        {
            Shape s = new Shape(2, 3);
            Console.WriteLine(s);
            Console.WriteLine("Area = " + s.Area());
        }

        static void Q2()
        {
            Cube c = new Cube(2, 3, 4);
            c.Print();
            Console.WriteLine("Area = " + c.Area());
        }

        static void Q3()
        {
            Shape shape = new Shape(2, 3);
            Console.WriteLine("shape.Area() = " + shape.Area());

            Cube cube = new Cube(2, 3, 4);
            Console.WriteLine("cube.Area() = " + cube.Area());

            Shape shapeRef = new Cube(2, 3, 4);
            Console.WriteLine("shapeRef.Area() = " + shapeRef.Area());

            Console.WriteLine("shapeRef is of type Shape, so the compiler calls Shape.Area() and prints 6.");
            Console.WriteLine("cube is of type Cube, so Cube.Area() runs and prints 24.");
            Console.WriteLine("Area() is hidden with new, not overridden, so the method is chosen by the reference type at compile time.");
            Console.WriteLine("This is called early/static binding.");
        }

        static void Q4()
        {
            object obj = new Cube(1, 2, 3);
            Console.WriteLine(obj.ToString());
            Console.WriteLine("Cube.ToString() runs, because ToString() is virtual in object and Cube overrides it.");
            Console.WriteLine("The method is chosen at run time by the real type of the object. This is late/dynamic binding.");
            Console.WriteLine("Area() did not behave polymorphically because it is not virtual and was only hidden with new.");
        }

        static void Q5()
        {
            Person p = new Person { ID = 1, Name = "Ali", Age = 30 };
            p.Greet();
            p.Display();
        }

        static void Q6()
        {
            Doctor d = new Doctor { ID = 2, Name = "Sara", Age = 40, Specialty = "Cardiology" };
            Engineer e = new Engineer { ID = 3, Name = "Omar", Age = 35, Field = "Civil" };
            d.Greet();
            d.Display();
            e.Greet();
            e.Display();
        }

        static void ProcessPerson(Person person)
        {
            person.Greet();
            person.Display();
        }

        static void Q7()
        {
            Person doc = new Doctor { ID = 2, Name = "Sara", Age = 40, Specialty = "Cardiology" };
            ProcessPerson(doc);
            Person eng = new Engineer { ID = 3, Name = "Omar", Age = 35, Field = "Civil" };
            ProcessPerson(eng);
            Console.WriteLine("Greet() is resolved at compile time, so the base version prints.");
            Console.WriteLine("Display() is resolved at run time, so the derived version prints.");
            Console.WriteLine("Greet() is hidden with new and not virtual, so the Person reference calls Person.Greet().");
            Console.WriteLine("Display() is virtual and overridden, so the real object type decides which version runs.");
        }

        static void Q8()
        {
            Console.WriteLine("If virtual is removed from Display() in Person, the derived classes give a compile error:");
            Console.WriteLine("cannot override inherited member because it is not marked virtual, abstract, or override.");
            Console.WriteLine("It means a method can only be overridden if the base class allows it with virtual.");
            Console.WriteLine("If override is replaced by nothing, the compiler gives a warning that the member hides the inherited member and asks for the new keyword.");
        }

        static void Q15()
        {
            Console.WriteLine("Feature                      | Static Binding (new)        | Dynamic Binding (override)");
            Console.WriteLine("Keyword in base              | none (any method)           | virtual");
            Console.WriteLine("Keyword in derived           | new                         | override");
            Console.WriteLine("Resolved at                  | Compile time                | Run time");
            Console.WriteLine("Behavior via base reference  | Calls the base version      | Calls the derived version");
        }

        static void Q16()
        {
            Console.WriteLine("virtual tells the compiler that the base method is allowed to be replaced, and it creates an entry in the virtual table so the call is decided at run time.");
            Console.WriteLine("Without virtual, the method is called directly by the reference type, so override has nothing to replace.");
            Console.WriteLine("new does not replace the base method. It only hides it with a new method that has the same name, so it works on any method.");
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("0 Part 1 (Employee)");
                Console.WriteLine("1-4 Static and Dynamic Binding: Q1 to Q4");
                Console.WriteLine("5-8 Dynamic Binding: Q5 to Q8");
                Console.WriteLine("15, 16 Theory: Q15, Q16");
                Console.WriteLine("-1 Exit");
                int choice = ReadInt("Choose: ");

                if (choice == -1) break;

                switch (choice)
                {
                    case 0: Part1(); break;
                    case 1: Q1(); break;
                    case 2: Q2(); break;
                    case 3: Q3(); break;
                    case 4: Q4(); break;
                    case 5: Q5(); break;
                    case 6: Q6(); break;
                    case 7: Q7(); break;
                    case 8: Q8(); break;
                    case 15: Q15(); break;
                    case 16: Q16(); break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }
    }
}
