using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
	public class RankedGradeBook : BaseGradeBook
	{
        public RankedGradeBook(string name) : base(name)
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
            var grades = Students.OrderByDescending(x => x.AverageGrade).Select(x => x.AverageGrade).ToList();

            if (grades[threshold - 1] <= averageGrade)
                return 'A';
            else if (grades[(threshold * 2) - 1] <= averageGrade)
                return 'B';
            else if (grades[(threshold * 3) - 1] <= averageGrade)
                return 'C';
            else if (grades[(threshold * 4) - 1] <= averageGrade)
                return 'D';
            return 'F';
        }
    }
}
