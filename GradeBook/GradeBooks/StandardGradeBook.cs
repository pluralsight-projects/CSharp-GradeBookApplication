using GradeBook.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GradeBook.GradeBooks
{
    public class StandardGradebook : BaseGradeBook
    {
        public StandardGradebook(string name) : base(name)
        {
            Type = GradeBookType.Standard;
        }
    }
}
