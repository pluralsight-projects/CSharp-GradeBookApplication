using GradeBook.Enums;
using System;

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
            if (Students.Count < 5)
            {
                throw new InvalidOperationException("Ranked - grading requires a minimum of 5 students to work");
            }
                
            return 'F';
        }
    }
}
