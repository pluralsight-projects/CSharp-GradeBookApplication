

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
    public class StandartGradeBook : BaseGradeBook
    {
        public StandartGradeBook(string name) : base(name)
        {
            Type = GradeBookType.Standart;
        }
    }
}
