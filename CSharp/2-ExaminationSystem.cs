using System;
using System.Collections.Generic;

namespace ExaminationSystem
{
    public class Answer : ICloneable, IComparable<Answer>
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }

        public Answer() : this(0, "") { }

        public Answer(int id, string text)
        {
            AnswerId = id;
            AnswerText = text;
        }

        public object Clone()
        {
            return new Answer(AnswerId, AnswerText);
        }

        public int CompareTo(Answer other)
        {
            if (other == null) return 1;
            return AnswerId.CompareTo(other.AnswerId);
        }

        public override string ToString()
        {
            return AnswerId + ". " + AnswerText;
        }
    }

    public abstract class Question : ICloneable, IComparable<Question>
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public int Mark { get; set; }
        public Answer[] AnswerList { get; set; }
        public Answer RightAnswer { get; set; }
        public Answer UserAnswer { get; set; }

        protected Question() : this("", "", 0) { }

        protected Question(string header, string body, int mark)
        {
            Header = header;
            Body = body;
            Mark = mark;
            AnswerList = new Answer[0];
        }

        protected Question(string header, string body, int mark, Answer[] answers, Answer right) : this(header, body, mark)
        {
            AnswerList = answers;
            RightAnswer = right;
        }

        public abstract void Ask();

        public bool IsCorrect()
        {
            return UserAnswer != null && RightAnswer != null && UserAnswer.AnswerId == RightAnswer.AnswerId;
        }

        public abstract object Clone();

        public int CompareTo(Question other)
        {
            if (other == null) return 1;
            return Mark.CompareTo(other.Mark);
        }

        public override string ToString()
        {
            return Header + " (" + Mark + " marks)\n" + Body;
        }
    }

    public class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion() : base() { }

        public TrueFalseQuestion(string body, int mark, bool correct)
            : base("True or False Question", body, mark,
                   new Answer[] { new Answer(1, "True"), new Answer(2, "False") },
                   correct ? new Answer(1, "True") : new Answer(2, "False"))
        {
        }

        public override void Ask()
        {
            Console.WriteLine(this);
            foreach (Answer a in AnswerList)
                Console.WriteLine(a);
            UserAnswer = ReadAnswer(AnswerList);
        }

        public static Answer ReadAnswer(Answer[] list)
        {
            int choice;
            while (true)
            {
                Console.Write("Your answer (id): ");
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    foreach (Answer a in list)
                        if (a.AnswerId == choice) return a;
                }
                Console.WriteLine("Invalid choice, try again.");
            }
        }

        public override object Clone()
        {
            TrueFalseQuestion q = new TrueFalseQuestion();
            q.Header = Header;
            q.Body = Body;
            q.Mark = Mark;
            q.AnswerList = (Answer[])AnswerList.Clone();
            q.RightAnswer = RightAnswer;
            return q;
        }
    }

    public class MCQQuestion : Question
    {
        public MCQQuestion() : base() { }

        public MCQQuestion(string body, int mark, Answer[] answers, int rightAnswerId)
            : base("MCQ Question", body, mark, answers, null)
        {
            foreach (Answer a in answers)
                if (a.AnswerId == rightAnswerId) RightAnswer = a;
        }

        public override void Ask()
        {
            Console.WriteLine(this);
            foreach (Answer a in AnswerList)
                Console.WriteLine(a);
            UserAnswer = TrueFalseQuestion.ReadAnswer(AnswerList);
        }

        public override object Clone()
        {
            MCQQuestion q = new MCQQuestion();
            q.Header = Header;
            q.Body = Body;
            q.Mark = Mark;
            Answer[] copy = new Answer[AnswerList.Length];
            for (int i = 0; i < copy.Length; i++)
                copy[i] = (Answer)AnswerList[i].Clone();
            q.AnswerList = copy;
            q.RightAnswer = RightAnswer == null ? null : (Answer)RightAnswer.Clone();
            return q;
        }
    }

    public abstract class Exam : ICloneable, IComparable<Exam>
    {
        public int Time { get; set; }
        public int NumberOfQuestions { get; set; }
        public Subject Subject { get; set; }
        public List<Question> Questions { get; set; }

        protected Exam() : this(0, 0, null) { }

        protected Exam(int time, int numberOfQuestions, Subject subject)
        {
            Time = time;
            NumberOfQuestions = numberOfQuestions;
            Subject = subject;
            Questions = new List<Question>();
        }

        public abstract void ShowExam();

        public int TotalMark()
        {
            int total = 0;
            foreach (Question q in Questions) total += q.Mark;
            return total;
        }

        public int Grade()
        {
            int grade = 0;
            foreach (Question q in Questions)
                if (q.IsCorrect()) grade += q.Mark;
            return grade;
        }

        public abstract object Clone();

        public int CompareTo(Exam other)
        {
            if (other == null) return 1;
            return Time.CompareTo(other.Time);
        }

        public override string ToString()
        {
            return GetType().Name + " | Subject: " + (Subject == null ? "-" : Subject.SubjectName) +
                   " | Time: " + Time + " min | Questions: " + NumberOfQuestions;
        }
    }

    public class FinalExam : Exam
    {
        public FinalExam() : base() { }

        public FinalExam(int time, int numberOfQuestions, Subject subject) : base(time, numberOfQuestions, subject) { }

        public override void ShowExam()
        {
            Console.WriteLine("===== Final Exam: " + Subject.SubjectName + " (" + Time + " min) =====");
            for (int i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Q" + (i + 1) + ") ");
                Questions[i].Ask();
            }
            Console.WriteLine();
            Console.WriteLine("===== Result =====");
            for (int i = 0; i < Questions.Count; i++)
            {
                Question q = Questions[i];
                Console.WriteLine("Q" + (i + 1) + ") " + q.Body);
                Console.WriteLine("   Your answer: " + (q.UserAnswer == null ? "-" : q.UserAnswer.AnswerText));
                Console.WriteLine("   Mark: " + (q.IsCorrect() ? q.Mark : 0) + "/" + q.Mark);
            }
            Console.WriteLine("Grade: " + Grade() + " / " + TotalMark());
        }

        public override object Clone()
        {
            FinalExam e = new FinalExam(Time, NumberOfQuestions, Subject);
            foreach (Question q in Questions) e.Questions.Add((Question)q.Clone());
            return e;
        }
    }

    public class PracticalExam : Exam
    {
        public PracticalExam() : base() { }

        public PracticalExam(int time, int numberOfQuestions, Subject subject) : base(time, numberOfQuestions, subject) { }

        public override void ShowExam()
        {
            Console.WriteLine("===== Practical Exam: " + Subject.SubjectName + " (" + Time + " min) =====");
            for (int i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Q" + (i + 1) + ") ");
                Questions[i].Ask();
            }
            Console.WriteLine();
            Console.WriteLine("===== Right Answers =====");
            for (int i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine("Q" + (i + 1) + ") " + Questions[i].Body);
                Console.WriteLine("   Right answer: " + Questions[i].RightAnswer);
            }
        }

        public override object Clone()
        {
            PracticalExam e = new PracticalExam(Time, NumberOfQuestions, Subject);
            foreach (Question q in Questions) e.Questions.Add((Question)q.Clone());
            return e;
        }
    }

    public class Subject : ICloneable, IComparable<Subject>
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Exam Exam { get; set; }

        public Subject() : this(0, "") { }

        public Subject(int id, string name)
        {
            SubjectId = id;
            SubjectName = name;
        }

        public void CreateExam()
        {
            Console.WriteLine("Choose exam type: 1) Final  2) Practical");
            int type;
            while (!int.TryParse(Console.ReadLine(), out type) || (type != 1 && type != 2))
                Console.Write("Enter 1 or 2: ");

            int time = ReadInt("Time of exam (minutes): ", 1, 600);
            int count = ReadInt("Number of questions: ", 1, 50);

            if (type == 1) Exam = new FinalExam(time, count, this);
            else Exam = new PracticalExam(time, count, this);

            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine();
                Console.WriteLine("--- Question " + i + " ---");
                int kind = 2;
                if (type == 1)
                {
                    Console.WriteLine("Question type: 1) True or False  2) MCQ");
                    kind = ReadInt("Choice: ", 1, 2);
                }

                Console.Write("Question body: ");
                string body = Console.ReadLine();
                int mark = ReadInt("Mark: ", 1, 100);

                if (kind == 1)
                {
                    Console.WriteLine("Right answer: 1) True  2) False");
                    int right = ReadInt("Choice: ", 1, 2);
                    Exam.Questions.Add(new TrueFalseQuestion(body, mark, right == 1));
                }
                else
                {
                    int n = ReadInt("Number of choices: ", 2, 10);
                    Answer[] answers = new Answer[n];
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write("Choice " + (j + 1) + " text: ");
                        answers[j] = new Answer(j + 1, Console.ReadLine());
                    }
                    int right = ReadInt("Right choice id: ", 1, n);
                    Exam.Questions.Add(new MCQQuestion(body, mark, answers, right));
                }
            }
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value < min || value > max)
                Console.Write("Enter a number between " + min + " and " + max + ": ");
            return value;
        }

        public object Clone()
        {
            Subject s = new Subject(SubjectId, SubjectName);
            if (Exam != null)
            {
                s.Exam = (Exam)Exam.Clone();
                s.Exam.Subject = s;
            }
            return s;
        }

        public int CompareTo(Subject other)
        {
            if (other == null) return 1;
            return SubjectId.CompareTo(other.SubjectId);
        }

        public override string ToString()
        {
            return "Subject " + SubjectId + ": " + SubjectName;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Subject id: ");
            int id = Subject.ReadInt("", 1, int.MaxValue);
            Console.Write("Subject name: ");
            string name = Console.ReadLine();

            Subject subject = new Subject(id, name);
            Console.WriteLine(subject);
            subject.CreateExam();

            Console.WriteLine();
            Console.WriteLine(subject.Exam);
            Console.Write("Start the exam? (y/n): ");
            if ((Console.ReadLine() ?? "").Trim().ToLower() == "y")
            {
                Console.Clear();
                subject.Exam.ShowExam();
            }
        }
    }
}
