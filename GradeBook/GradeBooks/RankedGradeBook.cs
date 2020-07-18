using GradeBook.Enums;
namespace GradeBook.GradeBooks
{
  
         public class RankedGradeBook : BaseGradeBook{
           public RankedGradeBook(string name) : base(name)
         {
             Name = name;
              Type = GradeBookType.Ranked;
         }
       }
    }
