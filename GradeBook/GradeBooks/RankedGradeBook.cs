using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GradeBook.GradeBooks
{
    class RankedGradeBook : BaseGradeBook
    {
		public RankedGradeBook(string name) : base(name)
		{
			Type = Enums.GradeBookType.Ranked;
		}
		public override char GetLetterGrade(double averageGrade)
		{
			if (Students.Count < 5)
				throw new InvalidOperationException("Ranked grading requires at leasr 5 students.");

			var threshold = (int)Math.Ceiling(Students.Count * 0.2);
			var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();

			return base.GetLetterGrade(averageGrade);
		}
	}
}
