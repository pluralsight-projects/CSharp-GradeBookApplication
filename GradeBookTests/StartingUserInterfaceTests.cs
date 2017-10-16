using GradeBook.UserInterfaces;
using System;
using System.IO;
using Xunit;

namespace GradeBookTests
{
    public class StartingUserInterfaceTests
    {
        //Note all tests contained here are not good unit tests, they are interacting with the user interface which is not recommended. Normally you'd use Mocks, Fakes, etc to work around this, however; the added complexity this would create would likely be harmful for the learner.
        [Fact]
        public void CreateCommandTest()
        {
            var failed = true;
            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                Console.SetIn(new StringReader("Close"));
                StartingUserInterface.CreateCommand("Create CreateCommandTest");
                if (consolestream.ToString().ToLower().Contains("created gradebook createcommandtest."))
                    failed = false;
                if (failed)
                {
                    StartingUserInterface.CreateCommand("Create CreateCommandTest Standard");
                    if (consolestream.ToString().ToLower().Contains("created gradebook createcommandtest."))
                        failed = false;
                }
                if (failed)
                {
                    StartingUserInterface.CreateCommand("Create CreateCommandTest Standard True");
                    if (consolestream.ToString().ToLower().Contains("created gradebook createcommandtest."))
                        failed = false;
                }
            }
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(standardOutput);
            StreamReader standardInput = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(standardInput);
            
            Assert.True(!failed, "GradeBook.UserInterfaces.StartingUserInterface.CreateCommand was unable to successfully create gradebook.");
        }

        [Fact]
        public void CreateCommandErrorWhenNoTypeTest()
        {
            var failed = true;
            var output = string.Empty;
            using (var consolestream = new StringWriter())
            {
                Console.SetOut(consolestream);
                Console.SetIn(new StringReader("Close"));
                StartingUserInterface.CreateCommand("Create CreateCommandTest");
                if (!consolestream.ToString().ToLower().Contains("command not valid"))
                    failed = false;
                if (failed)
                {
                    StartingUserInterface.CreateCommand("Create CreateCommandTest Standard");
                    if (consolestream.ToString().ToLower().Contains("command not valid"))
                        failed = false;
                }
                StartingUserInterface.CreateCommand("Create CreateCommandTest Standard True");
                output = consolestream.ToString().ToLower();
            }
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(standardOutput);
            StreamReader standardInput = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(standardInput);

            Assert.True(output.Contains("created gradebook"), "GradeBook.UserInterfaces.StartingUserInterface.CreateCommand successfully created a gradebook even when no type was provided.");
        }
    }
}
