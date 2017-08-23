using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

using GradeBook;
using GradeBook.Enums;
using System.IO;
using Newtonsoft.Json;
using GradeBook.GradeBooks;

namespace GradeBookTests
{
    /* Disclaimer: The tests in this class are NOT recommended practice, they've been made far more compilated and fragile than standard tests out of the 
     need for the project to be able to compile prior to student completion of all tasks which greatly complicates this process, you should not do this 
     in real world testing. Also note calling these "unit tests" would be inaccurate as true unit tests test code in isolation, this does not. */

    public class RankedGradeBookTests
    {
        [Fact]
        public void RankedGradeBookExistsTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");
        }

        [Fact]
        public void RankedGradeBookInheritsBaseGradeBookTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            MethodInfo method = rankedGradeBook.GetMethod("Save");
            Assert.True(method != null, "GradeBook.GradeBooks.RankedGradeBook doesn't appear to be inherritting BaseGradeBook.");
        }

        [Fact]
        public void TypePropertyExistsTest()
        {
            var gradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.FullName == "GradeBook.GradeBooks.BaseGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(gradeBook != null, "GradeBook.GradeBooks.BaseGradeBook doesn't exist.");

            gradeBook.GetProperty("Type");
            
            Assert.True(gradeBook.GetProperty("Type") != null, "`Type` property wasn't found in `GradeBook.BaseGradeBook`.");
        }

        //Do not test IO in this manner in real world applications, it's fragile, false negative prone, etc (sadly in our education context it's unavoidable without adding complexity to the project)
        [Fact]
        public void LoadTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();
            Assert.True(gradebookEnum != null, "GradeBook.Enums.GradeBookType doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "RankedLoadTest", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "RankedLoadTest");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            gradeBook.GetType().GetProperty("Type").SetValue(gradeBook, Enum.Parse(gradebookEnum, "Ranked", true));

            try
            {
                using (var file = new FileStream("RankedLoadTest.gdbk", FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        var json = JsonConvert.SerializeObject(gradeBook);
                        writer.Write(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.True(ex != null, "Test for GradeBook.GradeBooks.BaseGradeBook.Load was unable to run. This is likely due to issues being able to read/write gradebook files to the local file system.");
            }

            var actual = BaseGradeBook.Load("RankedLoadTest");
            Assert.True((string)actual.GetType().GetProperty("Name").GetValue(actual) == "RankedLoadTest", "GradeBook.GradeBooks.BaseGradeBook.Load did not properly load the gradebook when called from RankedGradeBook.");
            Assert.True(actual.GetType() == rankedGradeBook, "GradeBook.GradeBooks.BaseGradeBook.Load did not instantiate the correct type of gradebook when called from RankedGradeBook.");
            Assert.True(actual.GetType().GetProperty("Type").GetValue(actual).ToString() == Enum.Parse(gradebookEnum, "Ranked", true).ToString(), "GradeBook.GradeBooks.BaseGradeBook.Load did not properly set the type of gradebook to Ranked when called from RankedGradeBook.");
        }

        //Do not test IO in this manner in real world applications, it's fragile, false negative prone, etc (sadly in our education context it's unavoidable without adding complexity to the project)
        [Fact]
        public void LoadFileNotFoundTest()
        {

            var output = string.Empty;
            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                BaseGradeBook.Load("LoadFileNotFoundTest.gdbk");
                output = consolestream.ToString().ToLower();
            }
            StreamWriter rankedOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(rankedOutput);

            Assert.True(output.Contains("gradebook could not be found"), "GradeBook.GradeBooks.BaseGradeBook.Load didn't return a 'Gradebook could not be found' message when there isn't a gradebook with that name.");
        }

        #region Constructor
        [Fact]
        public void ConstructorTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.FullName == "GradeBook.GradeBooks.RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();
            Assert.True(gradebookEnum != null, "GradeBook.Enums.GradeBookType doesn't exist.");

            Assert.True((string)gradeBook.GetType().GetProperty("Name").GetValue(gradeBook) == "Test GradeBook", "`Name` wasn't set properly by `GradeBook.RankedGradeBook` Construtor.");
            Assert.True(gradeBook.GetType().GetProperty("Type").GetValue(gradeBook).GetType() == Enum.Parse(gradebookEnum, "Ranked", true).GetType(), "`Type` wasn't set properly by the `GradeBook.RankedGradeBook` Constructor.");
        }
        #endregion

        #region CalculateStatistics
        [Fact]
        public void CalculateStatisticsNotEnoughStudentsTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.FullName == "GradeBook.GradeBooks.RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("CalculateStatistics");
            var output = string.Empty;

            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                method.Invoke(gradeBook, null);
                output = consolestream.ToString().ToLower();
            }
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(standardOutput);

            Assert.True(output.Contains("5 students") || output.Contains("five students"));
        }
        #endregion

        #region CalculateStudentStastitics
        [Fact]
        public void CalculateStudentStatisticsNotEnoughStudentTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.FullName == "GradeBook.GradeBooks.RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("CalculateStudentStatistics");
            var output = string.Empty;

            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                method.Invoke(gradeBook, new object[] { "Bob" });
                output = consolestream.ToString().ToLower();
            }
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(standardOutput);

            Assert.True(output.Contains("5 students") || output.Contains("five students"));
        }
        #endregion

        #region GetLetterGrade
        [Fact]
        public void GetLetterGradeThrowsExceptionOnNotEnoughStudentsTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");
            var exception = Record.Exception(() => method.Invoke(gradeBook, new object[] { 100 }));
            Assert.True(exception != null, "GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade didn't throw an exception when less than 5 students have grades.");
        }

        [Fact]
        public void GetLetterGradeATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

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
            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            var expected = 'A';
            var actual = (char)method.Invoke(gradeBook, new object[] { 100 });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade didn't give an A to students in the top 20%.");
        }

        [Fact]
        public void GetLetterGradeBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

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
            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            var expected = 'B';
            var actual = (char)method.Invoke(gradeBook, new object[] { 75 });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade didn't give an B to students in the top 20% to 40%.");
        }

        [Fact]
        public void GetLetterGradeCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

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
            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            var expected = 'C';
            var actual = (char)method.Invoke(gradeBook, new object[] { 50 });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade didn't give an C to students in the top 40% to 60%.");
        }

        [Fact]
        public void GetLetterGradeDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

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
            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            var expected = 'D';
            var actual = (char)method.Invoke(gradeBook, new object[] { 25 });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade didn't give an D to students in the top 60% to 80%.");
        }

        [Fact]
        public void GetLetterGradeFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

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
            MethodInfo method = rankedGradeBook.GetMethod("GetLetterGrade");

            var expected = 'F';
            var actual = (char)method.Invoke(gradeBook, new object[] { 0 });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetLetterGrade didn't give an F to students in the bottom 20%.");
        }
        #endregion

        #region BaseGradeBook.GetGPA
        [Fact]
        public void GetGPAStandardStudentIsNotWeightedATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 4;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 4 when the grade is 'A', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsNotWeightedBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 3;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'B', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 3 when the grade is 'B', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsNotWeightedCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 2;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'C', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 2 when the grade is 'C', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsNotWeightedDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 1;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'D', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 1 when the grade is 'D', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsNotWeightedFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 0;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'F', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 0 when the grade is 'F', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsWeightedATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 4;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 4 when the grade is 'A', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsWeightedBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 3;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'B', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 3 when the grade is 'B', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsWeightedCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 2;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'C', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 2 when the grade is 'C', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsWeightedDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 1;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'D', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 1 when the grade is 'D', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAStandardStudentIsWeightedFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 0;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'F', StudentType.Standard });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 0 when the grade is 'F', the Student is Standard, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsNotWeightedATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 4;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 4 when the grade is 'A', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsNotWeightedBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 3;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'B', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 3 when the grade is 'B', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsNotWeightedCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 2;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'C', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 2 when the grade is 'C', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsNotWeightedDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 1;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'D', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 1 when the grade is 'D', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsNotWeightedFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 0;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'F', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 0 when the grade is 'F', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsWeightedATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 5;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'A', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 4 when the grade is 'A', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsWeightedBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 4;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'B', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 3 when the grade is 'B', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsWeightedCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 3;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'C', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 2 when the grade is 'C', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsWeightedDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 2;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'D', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 1 when the grade is 'D', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPAHonorsStudentIsWeightedFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 1;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'F', StudentType.Honors });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 0 when the grade is 'F', the Student is Honors, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsNotWeightedATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 4;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'A', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 4 when the grade is 'A', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsNotWeightedBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 3;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'B', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 3 when the grade is 'B', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsNotWeightedCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 2;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'C', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 2 when the grade is 'C', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsNotWeightedDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 1;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'D', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 1 when the grade is 'D', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsNotWeightedFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", false);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 0;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'F', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 0 when the grade is 'F', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsWeightedATest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 5;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'A', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 4 when the grade is 'A', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsWeightedBTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 4;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'B', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 3 when the grade is 'B', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsWeightedCTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 3;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'C', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 2 when the grade is 'C', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsWeightedDTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 2;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'D', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 1 when the grade is 'D', the Student is DuelEnrolled, and grades are not weighted.");
        }

        [Fact]
        public void GetGPADuelEnrolledStudentIsWeightedFTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.RankedGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("GetGPA");

            var expected = 1;
            var actual = (double)method.Invoke(gradeBook, new object[] { 'F', StudentType.DuelEnrolled });
            Assert.True(expected == actual, "GradeBook.GradeBooks.RankedGradeBook.GetGPA didn't give a GPA of 0 when the grade is 'F', the Student is DuelEnrolled, and grades are not weighted.");
        }
        #endregion

        #region BaseGradeBook.AddStudent
        [Fact]
        public void AddStudentExceptionWhenNoNameTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("AddStudent");

            var exception = Record.Exception(() => method.Invoke(gradeBook, new object[] { new Student(string.Empty, StudentType.Standard, EnrollmentType.Campus) }));
            Assert.True(exception != null, "GradeBook.GradeBooks.BaseGradeBook.AddStudent didn't throw an exception when a student's name was empty when called through RankedGradeBook.");
        }

        [Fact]
        public void AddStudentTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("AddStudent");

            var expected = new Student("Test Student", StudentType.Standard, EnrollmentType.Campus);
            method.Invoke(gradeBook, new object[] { new Student("Test Student", StudentType.Standard, EnrollmentType.Campus) });
            Assert.True(((List<Student>)gradeBook.GetType().GetProperty("Students").GetValue(gradeBook)).FirstOrDefault(e => e.Name == "Test Student") != null, "GradeBook.GradeBooks.BaseGradeBook.AddStudent didn't successfully add a student when called through RankedGradeBook.");
        }
        #endregion

        #region BaseGradeBook.RemoveStudent
        [Fact]
        public void RemoveStudentExceptionWhenNoNameTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("RemoveStudent");

            var exception = Record.Exception(() => method.Invoke(gradeBook, new object[] { string.Empty }));
            Assert.True(exception != null, "GradeBook.GradeBooks.BaseGradeBook.RemoveStudent didn't throw an exception when a student's name was empty when used from a RankedGradeBook.");
        }

        [Fact]
        public void RemoveStudentNoStudentFoundTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.SankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("RemoveStudent");
            var output = string.Empty;

            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                method.Invoke(gradeBook, new object[] { "Test Student" });
                output = consolestream.ToString().ToLower();
            }
            StreamWriter rankedOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(rankedOutput);

            Assert.True(output.Contains("try again"), "GradeBook.GradeBooks.BaseGradeBook.RemoveStudent didn't return a 'Try Again' message when called from a RankedGradeBook.");
        }

        [Fact]
        public void RemoveStudentTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "RankedGradeBook"
                                     select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, new List<Student> { new Student("Test Student", StudentType.Standard, EnrollmentType.Campus) });

            MethodInfo method = rankedGradeBook.GetMethod("RemoveStudent");

            method.Invoke(gradeBook, new object[] { "Test Student" });
            Assert.True(((List<Student>)gradeBook.GetType().GetProperty("Students").GetValue(gradeBook)).FirstOrDefault(e => e.Name == "Test Student") == null, "GradeBook.GradeBooks.BaseGradeBook.RemoveStudent didn't successfully remove a student when called through RankedGradeBook.");
        }
        #endregion

        #region BaseGradeBook.AddGrade
        [Fact]
        public void AddGradeExceptionWhenNoNameTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("AddGrade");

            var exception = Record.Exception(() => method.Invoke(gradeBook, new object[] { string.Empty, 100 }));
            Assert.True(exception != null, "GradeBook.GradeBooks.BaseGradeBook.AddGrade didn't throw an exception when a student's name was empty when called through RankedGradeBook.");
        }

        [Fact]
        public void AddGradeNoStudentFoundTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.SankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("AddGrade");
            var output = string.Empty;

            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                method.Invoke(gradeBook, new object[] { "Test Student", 100 });
                output = consolestream.ToString().ToLower();
            }
            StreamWriter rankedOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(rankedOutput);

            Assert.True(output.Contains("try again"), "GradeBook.GradeBooks.BaseGradeBook.AddGrade didn't return a 'Try Again' message when called from a RankedGradeBook.");
        }

        [Fact]
        public void AddGradeTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, new List<Student> { new Student("Test Student", StudentType.Standard, EnrollmentType.Campus) });

            MethodInfo method = rankedGradeBook.GetMethod("AddGrade");
            
            var expected = new Student("Test Student", StudentType.Standard, EnrollmentType.Campus);
            method.Invoke(gradeBook, new object[] { "Test Student", 100 });
            Assert.True(((List<Student>)gradeBook.GetType().GetProperty("Students").GetValue(gradeBook)).FirstOrDefault(e => e.Name == "Test Student").Grades.Contains(100), "GradeBook.GradeBooks.BaseGradeBook.AddGrade didn't successfully add a grade to the student when called through RankedGradeBook.");
        }
        #endregion

        #region BaseGradeBook.RemoveGrade
        [Fact]
        public void RemoveGradeExceptionWhenNoNameTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("RemoveGrade");

            var exception = Record.Exception(() => method.Invoke(gradeBook, new object[] { string.Empty, 100 }));
            Assert.True(exception != null, "GradeBook.GradeBooks.BaseGradeBook.RemoveGrade didn't throw an exception when a student's name was empty when called through RankedGradeBook.");
        }

        [Fact]
        public void RemoveGradeNoStudentFoundTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.SankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            MethodInfo method = rankedGradeBook.GetMethod("RemoveGrade");
            var output = string.Empty;

            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                method.Invoke(gradeBook, new object[] { "Test Student", 100 });
                output = consolestream.ToString().ToLower();
            }
            StreamWriter rankedOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(rankedOutput);

            Assert.True(output.Contains("try again"), "GradeBook.GradeBooks.BaseGradeBook.RemoveGrade didn't return a 'Try Again' message when called from a RankedGradeBook.");
        }

        [Fact]
        public void RemoveGradeTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, new List<Student> { new Student("Test Student", StudentType.Standard, EnrollmentType.Campus) { Grades = new List<double> { 0, 100, 50 } } });

            MethodInfo method = rankedGradeBook.GetMethod("RemoveGrade");

            var expected = new Student("Test Student", StudentType.Standard, EnrollmentType.Campus);
            method.Invoke(gradeBook, new object[] { "Test Student", 100 });
            Assert.True(!((List<Student>)gradeBook.GetType().GetProperty("Students").GetValue(gradeBook)).FirstOrDefault(e => e.Name == "Test Student").Grades.Contains(100), "GradeBook.GradeBooks.BaseGradeBook.RemoveGrade didn't successfully remove a grade to the student when called through RankedGradeBook.");
        }
        #endregion

        #region BaseGradeBook.ListStudents
        [Fact]
        public void ListStudentsTest()
        {
            var rankedGradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from type in assembly.GetTypes()
                                   where type.Name == "RankedGradeBook"
                                   select type).FirstOrDefault();
            Assert.True(rankedGradeBook != null, "GradeBook.GradeBooks.RankedGradeBook doesn't exist.");

            var ctor = rankedGradeBook.GetConstructors().FirstOrDefault();
            Assert.True(ctor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            var parameters = ctor.GetParameters();
            object gradeBook = null;
            if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook", true);
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                gradeBook = Activator.CreateInstance(rankedGradeBook, "Test GradeBook");
            Assert.True(gradeBook != null, "The constructor for GradeBook.GradeBooks.RankedGradeBook have the expected parameters.");

            gradeBook.GetType().GetProperty("Students").SetValue(gradeBook, 
                new List<Student>
                {
                    new Student("Test Student", StudentType.Standard, EnrollmentType.Campus) { Grades = new List<double> { 0, 100, 50 } },
                    new Student("Test Student2", StudentType.Honors, EnrollmentType.State) { Grades = new List<double> { 0, 100, 50 } },
                    new Student("Test Student3", StudentType.DuelEnrolled, EnrollmentType.National) { Grades = new List<double> { 0, 100, 50 } }
                });

            MethodInfo method = rankedGradeBook.GetMethod("ListStudents");

            var output = string.Empty;

            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                method.Invoke(gradeBook, null);
                output = consolestream.ToString().ToLower();
            }
            StreamWriter rankedOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(rankedOutput);

            Assert.True(output.Contains("test student"), "GradeBook.GradeBooks.BaseGradeBook.ListStudent didn't announce the student's name.");
            Assert.True(output.Contains("standard"), "GradeBook.GradeBooks.BaseGradeBook.ListStudent didn't announce the student's type.");
            Assert.True(output.Contains("campus"), "GradeBook.GradeBooks.BaseGradeBook.ListStudent didn't announce the student's enrollment.");
            Assert.True(output.Contains("test student2"), "GradeBook.GradeBooks.BaseGradeBook.ListStudent didn't announce all students in the gradebook.");
        }
        #endregion
    }
}