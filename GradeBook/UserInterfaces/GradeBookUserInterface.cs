using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");

            while(!Quit)
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine(">> What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a valid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine();
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollment Types:");
            Console.WriteLine("Campus - Students who are in the same district as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}
