using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
	public class RankedGradeBook : BaseGradeBook
	{
        public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
        {
            Type = GradeBookType.Ranked;
        }
        public override char GetLetterGrade(double averageGrade)
        {   
            // Number of students must not be less than 5
            if (Students.Count < 5)
            {
                throw new InvalidOperationException("Ranked - grading requires a minimum of 5 students to work");
            }

            // Get the number of students and find out how many in the top 20%.
            var threshold = (int)Math.Ceiling(Students.Count * 0.2);
            // Get the grades that are in order and select them and put into a list.
            var grades = Students.OrderByDescending(x => x.AverageGrade).Select(x => x.AverageGrade).ToList();

            // If the grades are within the top 20%, return A
            if (grades[threshold - 1] <= averageGrade)
                return 'A';
            // Else if the grades are between 20% and 40%, return B
            else if (grades[(threshold * 2) - 1] <= averageGrade)
                return 'B';
            // Else if the grades are between 40% and 60%, return C
            else if (grades[(threshold * 3) - 1] <= averageGrade)
                return 'C';
            // Else if the grades are between 60% and 80%, return D
            else if (grades[(threshold * 4) - 1] <= averageGrade)
                return 'D';
            // Otherwise, return F
            return 'F';
        }
        public override void CalculateStatistics()
        {
            // Number of students must not be less than 5
            if (Students.Count < 5)
            {
                Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
                return;
            }
            base.CalculateStatistics();
        }
        public override void CalculateStudentStatistics(string name)
        {
            // Number of students must not be less than 5
            if (Students.Count < 5)
            {
                Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
                return;
            }
            base.CalculateStudentStatistics(name);
        }
    }
}
