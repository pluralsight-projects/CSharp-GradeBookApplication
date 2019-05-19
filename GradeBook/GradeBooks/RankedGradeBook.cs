using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
    public class RankedGradeBook : BaseGradeBook
    {
        public RankedGradeBook(string name) : base(name)
        {
            Type = Enums.GradeBookType.Ranked;
        }

        public override char GetLetterGrade(double averageGrade)
        {
            if (Students.Count > 5)
            {
                var threshold = (int)Math.Ceiling(Students.Count * 0.2);
                var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();

                if (averageGrade >= grades[threshold - 1])
                    return 'A';
                if (averageGrade >= grades[(threshold * 2) - 1])
                    return 'B';
                if (averageGrade >= grades[(threshold * 3) - 1])
                    return 'C';
                if (averageGrade >= grades[(threshold * 4) - 1])
                    return 'D';
                return 'F';
            }
            throw new InvalidOperationException("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        }
        public override void CalculateStatistics()
        {
            if (Students.Count < 5)
            {
                base.CalculateStatistics();
            }
            else
            {
                Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
                return;
            }
        }

        public override void CalculateStudentStatistics(string name)
        {
            if (Students.Count < 5)
            {
                Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
                return;
            }

            base.CalculateStudentStatistics(name);
        }
    }
}
