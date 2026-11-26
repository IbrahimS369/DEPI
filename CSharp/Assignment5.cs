using System;

namespace Assignment5
{
    class Point3D : IComparable, ICloneable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point3D() : this(0, 0, 0)
        {
        }

        public Point3D(int x) : this(x, 0, 0)
        {
        }

        public Point3D(int x, int y) : this(x, y, 0)
        {
        }

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return "Point Coordinates: (" + X + ", " + Y + ", " + Z + ")";
        }

        public override bool Equals(object obj)
        {
            Point3D p = obj as Point3D;
            if (p == null)
                return false;
            return X == p.X && Y == p.Y && Z == p.Z;
        }

        public override int GetHashCode()
        {
            return X * 31 * 31 + Y * 31 + Z;
        }

        public static bool operator ==(Point3D a, Point3D b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(Point3D a, Point3D b)
        {
            return !(a == b);
        }

        public int CompareTo(object obj)
        {
            Point3D p = (Point3D)obj;
            if (X != p.X)
                return X.CompareTo(p.X);
            return Y.CompareTo(p.Y);
        }

        public object Clone()
        {
            return new Point3D(X, Y, Z);
        }
    }

    class Maths
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static int Subtract(int a, int b)
        {
            return a - b;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public static double Divide(int a, int b)
        {
            return (double)a / b;
        }
    }

    class Duration
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public Duration()
        {
        }

        public Duration(int hours, int minutes, int seconds)
        {
            Set(hours * 3600 + minutes * 60 + seconds);
        }

        public Duration(int totalSeconds)
        {
            Set(totalSeconds);
        }

        private void Set(int total)
        {
            if (total < 0)
                total = 0;
            Hours = total / 3600;
            Minutes = (total % 3600) / 60;
            Seconds = total % 60;
        }

        public int TotalSeconds()
        {
            return Hours * 3600 + Minutes * 60 + Seconds;
        }

        public override string ToString()
        {
            if (Hours == 0)
                return "Minutes :" + Minutes + ", Seconds :" + Seconds;
            return "Hours: " + Hours + ", Minutes :" + Minutes + ", Seconds :" + Seconds;
        }

        public override bool Equals(object obj)
        {
            Duration d = obj as Duration;
            if (d == null)
                return false;
            return TotalSeconds() == d.TotalSeconds();
        }

        public override int GetHashCode()
        {
            return TotalSeconds().GetHashCode();
        }

        public static Duration operator +(Duration a, Duration b)
        {
            return new Duration(a.TotalSeconds() + b.TotalSeconds());
        }

        public static Duration operator +(Duration a, int seconds)
        {
            return new Duration(a.TotalSeconds() + seconds);
        }

        public static Duration operator +(int seconds, Duration a)
        {
            return new Duration(a.TotalSeconds() + seconds);
        }

        public static Duration operator -(Duration a, Duration b)
        {
            return new Duration(a.TotalSeconds() - b.TotalSeconds());
        }

        public static Duration operator ++(Duration a)
        {
            return new Duration(a.TotalSeconds() + 60);
        }

        public static Duration operator --(Duration a)
        {
            return new Duration(a.TotalSeconds() - 60);
        }

        public static bool operator >(Duration a, Duration b)
        {
            return a.TotalSeconds() > b.TotalSeconds();
        }

        public static bool operator <(Duration a, Duration b)
        {
            return a.TotalSeconds() < b.TotalSeconds();
        }

        public static bool operator >=(Duration a, Duration b)
        {
            return a.TotalSeconds() >= b.TotalSeconds();
        }

        public static bool operator <=(Duration a, Duration b)
        {
            return a.TotalSeconds() <= b.TotalSeconds();
        }

        public static bool operator true(Duration a)
        {
            return a.TotalSeconds() > 0;
        }

        public static bool operator false(Duration a)
        {
            return a.TotalSeconds() <= 0;
        }

        public static explicit operator DateTime(Duration a)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, a.Hours % 24, a.Minutes, a.Seconds);
        }
    }

    class Car0
    {
        public void MoveForward()
        {
            Console.WriteLine("Car moves forward");
        }

        public void MoveBackward()
        {
            Console.WriteLine("Car moves backward");
        }
    }

    interface IMoveable
    {
        void MoveForward();
        void MoveBackward();
    }

    interface IFlyable
    {
        void MoveUp();
        void MoveDown();
    }

    class Car : IMoveable
    {
        public void MoveForward()
        {
            Console.WriteLine("Car moves forward on the ground");
        }

        public void MoveBackward()
        {
            Console.WriteLine("Car moves backward on the ground");
        }
    }

    class Ship : IMoveable
    {
        void IMoveable.MoveForward()
        {
            Console.WriteLine("Ship moves forward on the sea");
        }

        public void MoveBackward()
        {
            Console.WriteLine("Ship moves backward on the sea");
        }
    }

    class Airplane : IMoveable, IFlyable
    {
        public void MoveForward()
        {
            Console.WriteLine("Airplane moves forward in the air");
        }

        public void MoveBackward()
        {
            Console.WriteLine("Airplane moves backward in the air");
        }

        public void MoveUp()
        {
            Console.WriteLine("Airplane moves up in the air");
        }

        public void MoveDown()
        {
            Console.WriteLine("Airplane moves down in the air");
        }
    }

    interface IVehicle : IMoveable, IFlyable
    {
    }

    class Vehicle : IVehicle
    {
        public virtual void MoveForward()
        {
            Console.WriteLine("Vehicle moves forward");
        }

        public virtual void MoveBackward()
        {
            Console.WriteLine("Vehicle moves backward");
        }

        public virtual void MoveUp()
        {
            Console.WriteLine("Vehicle moves up");
        }

        public virtual void MoveDown()
        {
            Console.WriteLine("Vehicle moves down");
        }
    }

    class Program
    {
        static int ReadInt(string message)
        {
            int n;
            Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out n))
            {
                Console.Write("Invalid input, try again: ");
            }
            return n;
        }

        static Point3D ReadPoint(string name)
        {
            Console.WriteLine("Enter coordinates of " + name);
            int x = ReadInt("X: ");
            int y = ReadInt("Y: ");
            int z = ReadInt("Z: ");
            return new Point3D(x, y, z);
        }

        static void FirstProject()
        {
            Point3D p = new Point3D(10, 10, 10);
            Console.WriteLine(p.ToString());

            Point3D p1 = ReadPoint("P1");
            Point3D p2 = ReadPoint("P2");

            if (p1 == p2)
                Console.WriteLine("P1 == P2");
            else
                Console.WriteLine("P1 != P2");

            Point3D[] points = new Point3D[4];
            points[0] = p1;
            points[1] = p2;
            points[2] = new Point3D(5, 7, 1);
            points[3] = new Point3D(5, 2, 9);
            Array.Sort(points);
            Console.WriteLine("Sorted points:");
            for (int i = 0; i < points.Length; i++)
            {
                Console.WriteLine(points[i]);
            }

            Point3D copy = (Point3D)p1.Clone();
            Console.WriteLine("Clone: " + copy);
            Console.WriteLine("Same reference: " + ReferenceEquals(p1, copy));
        }

        static void SecondProject()
        {
            int a = ReadInt("First number: ");
            int b = ReadInt("Second number: ");
            Console.WriteLine("Add = " + Maths.Add(a, b));
            Console.WriteLine("Subtract = " + Maths.Subtract(a, b));
            Console.WriteLine("Multiply = " + Maths.Multiply(a, b));
            if (b != 0)
                Console.WriteLine("Divide = " + Maths.Divide(a, b));
            else
                Console.WriteLine("Cannot divide by zero");
        }

        static void ThirdProject()
        {
            Duration d1 = new Duration(1, 10, 15);
            Console.WriteLine(d1.ToString());
            Duration d = new Duration(3600);
            Console.WriteLine(d.ToString());
            Duration d2 = new Duration(7800);
            Console.WriteLine(d2.ToString());
            Duration d3 = new Duration(666);
            Console.WriteLine(d3.ToString());

            d3 = d1 + d2;
            Console.WriteLine("D3 = D1 + D2: " + d3);
            d3 = d1 + 7800;
            Console.WriteLine("D3 = D1 + 7800: " + d3);
            d3 = 666 + d3;
            Console.WriteLine("D3 = 666 + D3: " + d3);
            d3 = ++d1;
            Console.WriteLine("D3 = ++D1: " + d3);
            d3 = --d2;
            Console.WriteLine("D3 = --D2: " + d3);
            d1 = d1 - d2;
            Console.WriteLine("D1 = D1 - D2: " + d1);

            if (d1 > d2)
                Console.WriteLine("D1 > D2");
            else
                Console.WriteLine("D1 is not greater than D2");

            if (d1 <= d2)
                Console.WriteLine("D1 <= D2");
            else
                Console.WriteLine("D1 is not less than or equal to D2");

            if (d1)
                Console.WriteLine("D1 is not empty");
            else
                Console.WriteLine("D1 is empty");

            DateTime obj = (DateTime)d2;
            Console.WriteLine("DateTime: " + obj);
        }

        static void Q9()
        {
            Car0 car = new Car0();
            car.MoveForward();
            car.MoveBackward();
            Console.WriteLine("If we add MoveUp and MoveDown to every vehicle, a car or a ship would be forced to implement methods that make no sense for it.");
            Console.WriteLine("Interfaces solve this because each class implements only the interfaces that fit it.");
        }

        static void Q10()
        {
            Console.WriteLine("IMoveable (MoveForward, MoveBackward) and IFlyable (MoveUp, MoveDown) are defined above in the code.");
        }

        static void Q11()
        {
            Car car = new Car();
            Ship ship = new Ship();
            Airplane plane = new Airplane();
            car.MoveForward();
            car.MoveBackward();
            ((IMoveable)ship).MoveForward();
            ship.MoveBackward();
            plane.MoveForward();
            plane.MoveBackward();
            plane.MoveUp();
            plane.MoveDown();
        }

        static void Q12()
        {
            Car car = new Car();
            Airplane plane = new Airplane();
            car.MoveForward();
            plane.MoveUp();

            IMoveable carRef = new Car();
            IMoveable planeRef = new Airplane();
            carRef.MoveForward();
            carRef.MoveBackward();
            planeRef.MoveForward();
            planeRef.MoveBackward();

            Console.WriteLine("Can we call MoveUp on planeRef? No, because IMoveable does not have MoveUp.");
            Console.WriteLine("We need a reference of type IFlyable or Airplane.");
            IFlyable flyRef = (IFlyable)planeRef;
            flyRef.MoveUp();
        }

        static void Q13()
        {
            Vehicle v = new Vehicle();
            v.MoveForward();
            v.MoveBackward();
            v.MoveUp();
            v.MoveDown();
            Console.WriteLine("An interface that inherits other interfaces groups them in one type, so a class implements one interface and an IVehicle reference can call all four methods.");
        }

        static void Q14()
        {
            Ship ship = new Ship();
            Console.WriteLine("Calling ship.MoveForward() directly gives a compile error because an explicit implementation is hidden from the class type.");
            IMoveable m = ship;
            m.MoveForward();
            ((IMoveable)ship).MoveBackward();
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 First Project (Point3D)");
                Console.WriteLine("2 Second Project (Maths)");
                Console.WriteLine("3 Third Project (Duration)");
                Console.WriteLine("9-14 Interfaces questions Q9 to Q14");
                Console.WriteLine("0 Exit");
                int choice = ReadInt("Choose: ");

                if (choice == 0)
                    break;

                switch (choice)
                {
                    case 1: FirstProject(); break;
                    case 2: SecondProject(); break;
                    case 3: ThirdProject(); break;
                    case 9: Q9(); break;
                    case 10: Q10(); break;
                    case 11: Q11(); break;
                    case 12: Q12(); break;
                    case 13: Q13(); break;
                    case 14: Q14(); break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }
    }
}
