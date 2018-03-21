using System;
using System.Linq;
using System.Reflection;
using GradeBook.Enums;
using GradeBook.GradeBooks;
using Xunit;

namespace GradeBookTests
{
    public class UpdateGPACalculationsToSupportWeightedGPATests
    {
        // <summary>
        ///     All Tests related to the "Update BaseGradeBooks GetGPA Method" Task.
        ///</summary>
        [Fact(DisplayName = "Update BaseGradeBooks GetGPA Method Test @update-basegradebook-s-getgpa-method")]
        public void GetWeightedGPATest()
        {
            var standardGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "StandardGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(standardGradeBook != null, "`GradeBook.GradeBooks.StandardGradeBook` doesn't exist.");

            // Test if `StandardGradeBook` is `public`.
            Assert.True(standardGradeBook.IsPublic, "`GradeBook.GradeBooks.StandardGradeBook` isn't public");

            // Test if `StandardGradeBook` is inheritting `BaseGradeBook`.
            Assert.True(standardGradeBook.BaseType == typeof(BaseGradeBook), "`GradeBook.GradeBooks.StandardGradeBook` doesn't inherit `BaseGradeBook`");

            // Test if `StandardGradeBook`'s constructor has the proper signature (consider both this task and later signature)
            var ctor = standardGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            Assert.True(parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool), "`GradeBook.GradeBooks.BaseGradeBook`'s constructor doesn't have the correct parameters. It should be a `string` and a `bool`.");

            gradeBook = Activator.CreateInstance(standardGradeBook, "WeightedTest", true);
            MethodInfo method = standardGradeBook.GetMethod("GetGPA");

            // Test weighting works correctly for Weighted gradebooks
            Assert.True((double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Standard }) == 4, "`GradeBook.GradeBooks.BaseGradeBook`'s `GetGPA` method weighted a student's grade even when they weren't an Honors or Duel Enrolled student.");
            Assert.True((double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Honors }) == 5, "`GradeBook.GradeBooks.BaseGradeBook`'s `GetGPA` method did not weight a student's when they were an Honors student.");
            Assert.True((double)method.Invoke(gradeBook, new object[] { 'A', StudentType.DualEnrolled }) == 5, "`GradeBook.GradeBooks.BaseGradeBook`'s `GetGPA` method did not weight a student's when they were a Duel Enrolled student.");


            // Test weighting works correctly for unweighted gradebooks
            gradeBook.GetType().GetProperty("IsWeighted").SetValue(gradeBook, false);
            Assert.True((double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Standard }) == 4, "`GradeBook.GradeBooks.BaseGradeBook`'s `GetGPA` method weighted a student's grade when the gradebook was not weighted.");
            Assert.True((double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Honors }) == 4, "`GradeBook.GradeBooks.BaseGradeBook`'s `GetGPA` method weighted a student's grade when the gradebook was not weighted.");
            Assert.True((double)method.Invoke(gradeBook, new object[] { 'A', StudentType.DualEnrolled }) == 4, "`GradeBook.GradeBooks.BaseGradeBook`'s `GetGPA` method weighted a student's grade when the gradebook was not weighted.");
        }
    }
}
