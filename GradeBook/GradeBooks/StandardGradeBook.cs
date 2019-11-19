using System;
using GradeBook.Enums;

namespace GradeBook.GradeBooks {
    public class StandardGradeBook : BaseGradeBook {
        public StandardGradeBook(string name, bool val) : base(name, val) {
            Type = GradeBookType.Standard;
        }
    }
}