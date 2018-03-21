using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GradeBook.GradeBooks;
using Xunit;

namespace GradeBookTests
{
    /// <summary>
    ///     This class contains all tests related to the "Create StandardGradeBook and RankedGradeBook Classes" task.
    ///     Note: Do not use these tests as example of good testing practices, due to the nature of how Pluralsight projects work
    ///     we have to create tests against code that doesn't exist and changes implimentation, due to this tests are fragile,
    ///     hard to maintain, and don't don't adhere to the "test just one thing" practice commonly used in production tests.
    /// </summary>
    public class CreateStandardGradeBookandRankedGradeBookClassesTests
    {
        /// <summary>
        ///     All Tests related to the "Create the StandardGradeBook Class" task.
        /// </summary>
        [Fact(DisplayName = "Create the StandardGradeBook Class @create-the-standardgradebook-class")]
        public void StardardGradeBookExistsTest()
        {
            // Get appropriate path to file for the current operating system
            var filePath = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "GradeBook" + Path.DirectorySeparatorChar + "GradeBooks" + Path.DirectorySeparatorChar + "StandardGradeBook.cs";
            // Assert StandardGradeBook is in the GradeBooks folder
            Assert.True(File.Exists(filePath), "`StandardGradeBook.cs` was not found in the `GradeBooks` folder.");

            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();

            // Assert StandardGradeBook was found in the GradeBook.GradeBooks namespace
            Assert.True(gradebook != null, "`StandardGradeBook` wasn't found in the `GradeBooks.GradeBook` namespace.");

            // Test to make sure the enum StandardGradeBook is public.
            Assert.True(gradebook.IsPublic, "`GradeBook.GradeBooks.StandardGradeBook` exists, but isn't `public`.");

            // Assert that StandardGradeBook's BaseType is BaseGradeBook
            Assert.True(gradebook.BaseType == typeof(BaseGradeBook), "`GradeBook.GradeBooks.StandardGradeBook` doesn't inherit `BaseGradeBook`");
        }

        /// <summary>
        ///     All tests related to the "Update StandardGradeBook Type" task.
        /// </summary>
        [Fact(DisplayName = "Update StandardGradeBook Type @update-standardgradebook-type")]
        public void UpdateStandardGradeBookTypeTests()
        {
            // Get StandardGradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();

            // Get StandardGradeBook's first constructor (should be the only constructor)
            var constructor = gradebook.GetConstructors().FirstOrDefault();

            // Assert a constructor was found
            Assert.True(constructor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            // Get constructor's parameters
            var parameters = constructor.GetParameters();

            // Instantiate the StandardGradeBook
            object standardGradeBook = null;
            if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                standardGradeBook = Activator.CreateInstance(gradebook, "LoadTest");

            // GUARD CODE - Without this code this test will fail once the project is refactored to accomidate weighted grading DO NOT REMOVE!!!
            else if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                standardGradeBook = Activator.CreateInstance(gradebook, "LoadTest", true);
            // END GUARD CODE

            // Assert the Type property's value is Standard
            Assert.True(standardGradeBook.GetType().GetProperty("Type").GetValue(standardGradeBook).ToString() == Enum.Parse(gradebookEnum, "Standard", true).ToString(), "`Type` wasn't set to `GradeBookType.Standard` by the `GradeBook.GradeBooks.StandardGradeBook` Constructor.");

            // Read file for Type being set (because Standard is the default in our enum we can't test if the task was completed without regex :(
            // Get appropriate path to file for the current operating system
            var filePath = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "GradeBook" + Path.DirectorySeparatorChar + "GradeBooks" + Path.DirectorySeparatorChar + "StandardGradeBook.cs";
            var input = File.ReadAllText(filePath);

            var pattern = @"(Type\s?[=]\s?(GradeBook[.])?(Enums[.])?GradeBookType[.]Standard\s?;)";
            var rgx = new Regex(pattern);
            var matches = rgx.Matches(input);
            Assert.True(matches.Count > 0, "While `Type` was set to `GradeBookType.Standard`, it wasn't set by the `GradeBook.GradeBooks.StandardGradeBook` Constructor.");
        }

        /// <summary>
        ///     All tests related to the "Create RankedGradeBook Class" task.
        /// </summary>
        [Fact(DisplayName = "Create RankedGradeBook Class @create-the-rankedgradebook-class")]
        public void CreateRankedGradeBookClassTest()
        {
            // Get appropriate path to file for the current operating system
            var filePath = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "GradeBook" + Path.DirectorySeparatorChar + "GradeBooks" + Path.DirectorySeparatorChar + "RankedGradeBook.cs";
            // Assert RankedGradeBook is in the GradeBooks folder
            Assert.True(File.Exists(filePath), "`RankedGradeBook.cs` was not found in the `GradeBooks` folder.");
        
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.GradeBooks.RankedGradeBook"
                                 select type).FirstOrDefault();

            // Assert RankedGradeBook was found in the GradeBook.GradeBooks namespace
            Assert.True(gradebook != null, "`RankedGradeBook` wasn't found in the `GradeBooks.GradeBook` namespace.");
        
            // Test to make sure the enum RankedGradeBook is public.
            Assert.True(gradebook.IsPublic, "`GradeBook.GradeBooks.RankedGradeBook` exists, but isn't `public`.");
        
            // Assert that RankedGradeBook's BaseType is BaseGradeBook
            Assert.True(gradebook.BaseType == typeof(BaseGradeBook), "`GradeBook.GradeBooks.RankedGradeBook` doesn't inherit `BaseGradeBook`");
        }

        /// <summary>
        ///     All tests related to the "Update RankedGradeBook Type" task.
        /// </summary>
        [Fact(DisplayName = "Update RankedGradeBook Type @update-rankedgradebook-type")]
        public void UpdateRankedGradeBookTypeTest()
        {
            // Get RankedGradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.RankedGradeBook"
                             select type).FirstOrDefault();

            // Get RankedGradeBook's first constructor (should be the only constructor)
            var constructor = gradebook.GetConstructors().FirstOrDefault();

            // Assert a constructor was found
            Assert.True(constructor != null, "No constructor found for GradeBook.GradeBooks.StardardGradeBook.");

            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            // Get constructor's parameters
            var parameters = constructor.GetParameters();

            // Instantiate the RankedGradeBook
            object rankedGradeBook = null;
            if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(string))
                rankedGradeBook = Activator.CreateInstance(gradebook, "LoadTest");

            // GUARD CODE - Without this code this test will fail once the project is refactored to accomidate weighted grading DO NOT REMOVE!!!
            else if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                rankedGradeBook = Activator.CreateInstance(gradebook, "LoadTest", true);
            // END GUARD CODE

            // Assert the Type property's value is Ranked
            Assert.True(rankedGradeBook.GetType().GetProperty("Type").GetValue(rankedGradeBook).ToString() == Enum.Parse(gradebookEnum, "Ranked", true).ToString(), "`Type` wasn't set to `GradeBookType.Ranked` by the `GradeBook.GradeBooks.RankedGradeBook` Constructor.");
        }
    }
}
