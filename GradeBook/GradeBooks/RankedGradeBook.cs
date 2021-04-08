using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
    public class RankedGradeBook : BaseGradeBook
    {
        GetLetterGrade();

        public RankedGradeBook (string name) :  base(name)
            {
                Type = GradeBookType.Ranked;
            }
    }
}
