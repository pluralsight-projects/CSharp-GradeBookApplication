using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
    public class RankedGradeBook : BaseGradeBook
    {
        // the constructor
        public RankedGradeBook(string name) : base(name)
            {
                Type = GradeBookType.Ranked;
            }
        //override method
       /* public override char GetLetterGrade (double avreagegrade)
            return base.GetLetterGrade(avreagegrade);*/

    }
}
