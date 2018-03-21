using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GradeBook;
using GradeBook.Enums;
using Xunit;

namespace GradeBookTests
{
    public class OverrideRankedGradeBooksGetLetterGradeTests
    {
        /// <summary>
        ///   All tests related to the "Create GetLetterGrade Override" task.
        /// </summary>
        [Fact(DisplayName = "Create GetLetterGrade Override @create-getlettergrade-override")]
        public void CreateGetLetterGradeOverrideTests()
        {
            // Setup Test
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();

            var constructor = rankedGradeBook.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");

            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            //Test if exception is thrown when there are less than 5 students.
            var exception = Record.Exception(() => method.Invoke(gradeBook, new object[] { 100 }));
            Assert.True(exception != null, "`GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade` didn't throw an exception when less than 5 students have grades.");

            //Setup successful conditions
            var students = new List<Student>
            {
                new Student("jamie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 100 }
                },
                new Student("john",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 75 }
                },
                new Student("jackie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 50 }
                },
                new Student("tom",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 25 }
                },
                new Student("tony",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 0 }
                }
            };

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, students);

            //Test if A is given when input grade is in the top 20%.
            Assert.True((char)method.Invoke(gradeBook, new object[] { 0 }) == 'F', "`GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade` didn't give an F when a student failed to earn a higher grade.");

            // Read file for to confirm GetLetterGrade has been overriden
            // Get appropriate path to file for the current operating system
            var filePath = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "GradeBook" + Path.DirectorySeparatorChar + "GradeBooks" + Path.DirectorySeparatorChar + "RankedGradeBook.cs";
            var input = File.ReadAllText(filePath);

            var pattern = @"(public\soverride\schar\sGetLetterGrade\s?[(]\s?double\saverageGrade\s?[)])";
            var rgx = new Regex(pattern);
            var matches = rgx.Matches(input);
            if (matches.Count > 0)
            {
                pattern = @"base.GetLetterGrade[(]\s?averageGrade\s?[)];";
                rgx = new Regex(pattern);
                matches = rgx.Matches(input);

                Assert.True(matches.Count == 0, "`RankedGradeBook.GetLetterGrade` is either not overiding `BaseGradeBook.GetLetterGrade` or is using `base.GetLetterGrade` within the override. If a higher grade isn't assigned `RankedGradeBook.GetLetterGrade` should return 'F'.");
            }
            else
            {
                Assert.True(false, "`RankedGradeBook.GetLetterGrade` is either not overiding `BaseGradeBook.GetLetterGrade` or is using `base.GetLetterGrade` within the override.");
            }
        }

        /// <summary>
        ///   All tests related to the "Top 20 Percent Get An A" task.
        /// </summary>
        [Fact(DisplayName = "Top 20 Percent Get An A @override-getlettergrade-a")]
        public void TopTwentyPercentGetAnATests()
        {
            // Setup Test
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();

            var constructor = rankedGradeBook.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");

            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            //Setup successful conditions
            var students = new List<Student>
            {
                new Student("jamie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 100 }
                },
                new Student("john",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 75 }
                },
                new Student("jackie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 50 }
                },
                new Student("tom",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 25 }
                },
                new Student("tony",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 0 }
                }
            };

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, students);

            //Test if A is given when input grade is in the top 20%.
            Assert.True((char)method.Invoke(gradeBook, new object[] { 100 }) == 'A', "`GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade` didn't give an A to students in the top 20% of the class.");

            // Read file for to confirm GetLetterGrade has been overriden
            // Get appropriate path to file for the current operating system
            var filePath = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "GradeBook" + Path.DirectorySeparatorChar + "GradeBooks" + Path.DirectorySeparatorChar + "RankedGradeBook.cs";
            var input = File.ReadAllText(filePath);

            var pattern = @"(public\soverride\schar\sGetLetterGrade\s?[(]\s?double\saverageGrade\s?[)])";
            var rgx = new Regex(pattern);
            var matches = rgx.Matches(input);
            if(matches.Count > 0)
            {
                pattern = @"base.GetLetterGrade[(]\s?averageGrade\s?[)];";
                rgx = new Regex(pattern);
                matches = rgx.Matches(input);

                Assert.True(matches.Count == 0, "`RankedGradeBook.GetLetterGrade` is either not overiding `BaseGradeBook.GetLetterGrade` or is using `base.GetLetterGrade` within the override. If a higher grade isn't assigned `RankedGradeBook.GetLetterGrade` should return 'F'.");
            }
            else
            {
                Assert.True(false, "`RankedGradeBook.GetLetterGrade` is either not overiding `BaseGradeBook.GetLetterGrade` or is using `base.GetLetterGrade` within the override.");
            }
        }

        /// <summary>
        ///   All tests related to the "Second 20 Percent Get A B" task.
        /// </summary>
        [Fact(DisplayName = "Second 20 Percent Get An B @override-getlettergrade-b")]
        public void SecondTwentyPercentGetABTests()
        {
            // Setup Test
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();

            var constructor = rankedGradeBook.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");

            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            //Setup successful conditions
            var students = new List<Student>
            {
                new Student("jamie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 100 }
                },
                new Student("john",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 75 }
                },
                new Student("jackie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 50 }
                },
                new Student("tom",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 25 }
                },
                new Student("tony",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 0 }
                }
            };

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, students);

            //Test if B is given when input grade is between the top 20 and 40%.
            Assert.True((char)method.Invoke(gradeBook, new object[] { 75 }) == 'B', "`GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade` didn't give an B to students between the top 20 and 40% of the class.");
        }

        /// <summary>
        ///   All tests related to the "Third 20 Percent Get A C" task.
        /// </summary>

        [Fact(DisplayName = "Third 20 Percent Get An C @override-getlettergrade-c")]
        public void ThirdTwentyPercentGetACTests()
        {
            // Setup Test
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();

            var constructor = rankedGradeBook.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");

            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            //Setup successful conditions
            var students = new List<Student>
            {
                new Student("jamie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 100 }
                },
                new Student("john",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 75 }
                },
                new Student("jackie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 50 }
                },
                new Student("tom",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 25 }
                },
                new Student("tony",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 0 }
                }
            };

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, students);

            //Test if C is given when input grade is between the top 40 and 60%.
            Assert.True((char)method.Invoke(gradeBook, new object[] { 50 }) == 'C', "`GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade` didn't give an C to students between the top 40 and 60% of the class.");
        }

        /// <summary>
        ///   All tests related to the "Fourth 20 Percent Get A D" task.
        /// </summary>

        [Fact(DisplayName = "Fourth 20 Percent Get An D @override-getlettergrade-d")]
        public void FourthTwentyPercentGetADTests()
        {
            // Setup Test
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();

            var constructor = rankedGradeBook.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");

            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            //Setup successful conditions
            var students = new List<Student>
            {
                new Student("jamie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 100 }
                },
                new Student("john",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 75 }
                },
                new Student("jackie",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 50 }
                },
                new Student("tom",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 25 }
                },
                new Student("tony",StudentType.Standard,EnrollmentType.Campus)
                {
                    Grades = new List<double>{ 0 }
                }
            };

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, students);

            //Test if D is given when input grade is between the top 60 and 80%.
            Assert.True((char)method.Invoke(gradeBook, new object[] { 25 }) == 'D', "`GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade` didn't give an D to students between the top 60 and 80% of the class.");
        }
    }
}
