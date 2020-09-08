using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GradeBook.GradeBooks
{
    public class RankedGradeBook : BaseGradeBook
    {
        public RankedGradeBook(string name) : base(name)
        {
            this.Type = Enums.GradeBookType.Ranked;
        }

        public override char GetLetterGrade(double averageGrade)
        {
            if (Students.Count < 5) throw new InvalidOperationException("Ranked-grading requires a minimum of 5 students to work");

            var sortedAverageGrades = Students.Select(student => student.AverageGrade).OrderBy(grade => grade);

            double index = sortedAverageGrades.Where(grade => grade < averageGrade).Count();

            var indexPercentage = (index / Students.Count) * 100;

            if (indexPercentage >= 80.0)
                return 'A';
            else if ((indexPercentage >= 60.0) && (indexPercentage < 80.0))
                return 'B';
            else if ((indexPercentage >= 40.0) && (indexPercentage < 60.0))
                return 'C';
            else if ((indexPercentage >= 20.0) && (indexPercentage < 40.0))
                return 'D';
            else
                return 'F';
        }
    }
}
