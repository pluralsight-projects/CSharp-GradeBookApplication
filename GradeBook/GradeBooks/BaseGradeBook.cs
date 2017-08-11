using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public bool IsWeighted { get; set; }
        public GradeBookType Type { get; set; }

        protected BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            IsWeighted = isWeighted;
            Students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach(var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if(!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    BaseGradeBook gradebook;
                    var json = reader.ReadToEnd();
                    var jobject =  JsonConvert.DeserializeObject<JObject>(json);
                    var type = Enum.Parse(typeof(GradeBookType), jobject.GetValue("Type").ToString(), true);
                    switch(type)
                    {
                        case GradeBookType.Standard:
                            gradebook = JsonConvert.DeserializeObject<StandardGradeBook>(json);
                            break;
                        case GradeBookType.Ranked:
                            gradebook = JsonConvert.DeserializeObject<RankedGradeBook>(json);
                            break;
                        default:
                            gradebook = JsonConvert.DeserializeObject<StandardGradeBook>(json);
                            break;
                    }
                    return gradebook;
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            if (IsWeighted)
            {
                switch (studentType)
                {
                    case StudentType.DuelEnrolled:
                    case StudentType.Honors:
                        {
                            switch (letterGrade)
                            {
                                case 'A':
                                    return 5;
                                case 'B':
                                    return 4;
                                case 'C':
                                    return 3;
                                case 'D':
                                    return 2;
                                case 'F':
                                    return 1;
                            }
                            break;
                        }
                }
            }

            switch (letterGrade)
            {
                case 'A':
                    return 4;
                case 'B':
                    return 3;
                case 'C':
                    return 2;
                case 'D':
                    return 1;
                case 'F':
                    return 0;
            }
            return 0;
        }
        
        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var duelEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}): GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DuelEnrolled:
                        duelEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (duelEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (duelEnrolledPoints / Students.Where(e => e.Type == StudentType.DuelEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}): GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public abstract char GetLetterGrade(double averageGrade);
    }
}
