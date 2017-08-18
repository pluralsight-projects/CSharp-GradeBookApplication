# C Sharp Grade Book Application

The C Sharp Grade Book Application is a designed to allow instructors to create gradebooks, add students to those grade books, add grades to those students, and calculate statics such as GPA.

-- Gif of working Application

## Accepted Commands

### Commands when no gradebooks are open
- "Create `Name of Gradebook` `Is this Gradebook Weighted (true/false)`" : Creates a new gradebook with the provided name
- "Help" : gives you a list of all valid commands within the given context
- "Load `Name of GradeBook`"
- "Quit" : Closes the application

### Commands when a gradebook is open
- "Add `Name of Student` `Type of Student` `Type of Enrollment`" : Adds a new student to the open gradebook
- "Remove `Name of Student`" : Removes a student with the provided name from the gradebook. (if a student with that name exists in the gradebook)
- "List" : Lists all students in the open gradebook
- "AddGrade `Name of Student` `Score`" : Adds to the given value to provided student's grades.
- "RemoveGrade `Name of Student` `Score`" : Removes the given value from the provided student's grade. (if that value exists in the stundent's grades)
- "Statistics all" : Provides statistical output for all students in the open gradebook
- "Statistics `Name of Student`" : Provides statistical out put for the provided student. (if that student exists)
- "Help" : Gives you a list of all valid commands within the given context
- "Save" : Saves the currently open gradebook
- "Close" : Closes the gradebook

# Setup the Application

## If you want to use Visual Studio
If you want to use Visual Studio (highly recommended for Windows users) follow the following steps:
-	If you already have Visual Studio installed make sure you have .Net Core installed by running the "Visual Studio Installer" and making sure ".NET Core cross-platform development" is checked
-	If you need to install visual studio download it at https://www.microsoft.com/net/core#windowsvs2017 On the workloads screen make sure ".NET Core cross-platform development" is checked
-	To run the application simply press the Start Debug button (green arrow) or press F5
-	To run the tests go to "Test" and select "Run All Tests" (you might need to goto "Test" > "Windows" > "Test Explorer" to display the results if you closed the test explorer.)

## If you don't plan to use Visual studio
If you would rather use something other than Visual Studio
-	Install the .Net Core SDK from https://www.microsoft.com/net/download/core once that installation completes you're ready to roll!
-	To run the application go into the GradeBook project folder and type `dotnet run`
-	To run the tests go into the GradeBookTests project folder and type `dotnet test /verbosity:quiet`

# Features you will impliment

- Add support for Ranked Grading
- Add support for Weighted GPAs

## Tasks necessary to complete implimentation:

__Note:__ this isn't the only way to accomplish this, however; this is what the project's tests are expecting. Implimenting this in a different way will likely result in being marked as incomplete / incorrect.

- [ ] Add support for Ranked Grading
	- [ ] In the Enums directory, create a new Enum `GradeBookType` containing the types of Gradebooks to be supported (`Standard`, `Ranked`, `ESNU`, `OneToFour`, `SixPoint`)
		- [ ] Create a new Enum `GradeBookType` containing the types `Standard`, `Ranked`, `ENSU`, `OneToFour`, and `SixPoint`.
			- This should be located in the `Enums` directory.
			- This should use the `GradeBook.Enums` namespace.
		- [ ] Add a `public` property named `Type` to `BaseGradeBook` of type `GradeBookType`.

	- [ ] Create a new class `StandardGradeBook` to contain the Standard Gradebook functionality. *note once this task is started code won't compile until task completion*
		- [ ] Create a class `StandardGradeBook` that inherits the `BaseGradeBook` class.
			- This should be located in the `GradeBooks` directory.
			- This should use the `GradeBook.GradeBooks` namespace.
		- [ ] Change the constructor to set the `Type` property to `Standard` in the `StandardGradeBook` class.
		- [ ] The constructor should also call the base constructor by adding ` : base(name)` after the contsructor declaration. (this was not covered in the course, it calls the constructor of `BaseGradeBook`)

	- [ ] Create a new `RankedGradeBook` to contain the Ranked Gradebook functionality. *note once this task is started code won't compile until task completion*
		- [ ] Create a new class `RankedGradeBook` that inherits the `BaseGradeBook` class.
			- This should be located in the `GradeBooks` directory.
			- This should use the `GradeBook.GradeBooks` namespace.
		- [ ] Create a constructor that sets the `Type` property to `Ranked.		
		- [ ] The constructor should also call the base constructor by adding ` : base(name)` after the contsructor declaration. (this was not covered in the course)

	- [ ] Update the `BaseGradeBook` class's `Load` method to handle multiple types of Gradebooks
		- [ ] Create a variable inside the `Load` method to contain the gradebook.
		- [ ] Between where the variable `jobject` is set and the return statement, set a variable using `Enum.Parse(typeof(GradeBookType), jobject.GetValue("Type").ToString(), true);` to get the type of GradeBook being loaded _(this was not covered in the course, it will take the saved file, and attempt to get the type of the gradebook from it)_
		- [ ] When type is `GradeBookType.Standard` set the gradebook variable using `JsonConvert.DeserializeObject<StandardGradeBook>(json);` _(this was also not covered in the course, it will take the saved file and create a StandardGradeBook object based on that file)_
		- [ ] When type is `GradeBookType.Ranked` set the gradebook variable using `JsonConvert.DeserializeObject<RankedGradeBook>(json);` _(this was also not covered in the course, it will take the saved file and create a RankedGradeBook object based on that file)_
		- [ ] Change the return statement to return `gradebook`.

	- [ ] Impliment an override for the `RankedGradeBook`'s `GetLetterGrade` method, in ranked grading students aren't scored based on their own individual performance, instead they are graded based on how they performed compared to the rest of their class.
	*Hint: To accomplish this out you need to figure out where an average grade fits compared to all other average grades, one way to do this is to take the number of students and multiple by 0.2 (this tells you every `X` students drop one letter grade), then put all average grades in descending order, and finally figure out where the input grade fits in the list of grades, for every `X` students from the top drop the result one letter grade (with `F` being the lowest possible grade)
		- [ ] If there are less than 5 students with a grade throw an `InvalidOperationException`. (Ranked Grading requires a minimum of 5 students to work)
		- [ ] To get an A a student must have an average score that is in the top 20% of their class.
		- [ ] To get a B a student must have an average score between the top 20 and 40% of their class.
		- [ ] To get a C a student must have an average score between the top 40 and 60% of their class.
		- [ ] To get a D a student must have an average score between the top 60 and 80% of their class.
		- [ ] If a student's average score is below the top 80% of their class they get an F.

	- [ ] Impliment an override of the  `RankedGradeBook`'s `CalculateStatistics` method.
		- [ ] This override will perform a check to make sure there are at least 5 students with grades
			- [ ] If there are not 5 students with grades display the message "Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade." then escape the method.
			- [ ] If there are 5 students with grades call the `CalculateStatistics` method using `base.CalculateStatistics`

	- [ ] Impliment an override of the  `RankedGradeBook`'s `CalculateStudentStatistics` method.
		- [ ] This override will perform a check to make sure there are at least 5 students with grades
			- [ ] If there are not 5 students with grades display the message "Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade." then escape the method.
			- [ ] If there are 5 students with grades call the base `CalculateStudentStatistics` method using `base.CalculateStudentStatistics`

	- [ ] In the GradeBook project's root directory, Update the `Program` class to work with multiple types of grade books
		- [ ] Inside the `Main` method update the code within the `if` condition that checks to see if `command` starts with "create".
			- [ ] Update the validation to check the `parts.Length` is not 3.
			- [ ] Update the message written to console by this condition to say "Command not valid, Create requires a name and type.".
			- [ ] Remove where variable `gradeBook` is set to a new instance of `BaseGradeBook` and instead change it from a `var` to being of type `BaseGradeBook`.
			- [ ] Create a new variable to store a string representing the gradebook type, this will be set using the second substring of the parts array.
				- [ ] When the value is "standard" set `gradeBook` to a newly instantiated `StandardGradeBook`.
				- [ ] When the value is "rank" set `gradeBook` to a newly instantiated `RankedGradeBook`.
				- [ ] When the value doesn't match any of the above write to `Console` what they entered " is not a supported type of gradebook, please try again", then use the `continue` keyword to return to the start of the loop.
			- [ ] Change where the "help" command outlines the "create" command to say "Create 'Name' 'Type' - Creates a new gradebook where 'Name' is the name of the gradebook and 'Type' is what type of grading it should use.".

	- [ ] In the GradeBooks directory, change `BaseGradeBook` into an abstract class
		- [ ] Add the `abstract` keyword to the `BaseGradeBook` declarition

- [ ] Add support for weighted GPAs *Note: Code will stop compiling once the constructor is updated until after changes are made in `Program.Main` are complete*
	- [ ] Add a property named 'IsWeighted' of type `bool` to `BaseGradeBook` located in the `GradeBooks` directory.
		- [ ] Create a new public property `IsWeighted` of type `bool` in `BaseGradeBook`
		- [ ] Change the `BaseGradeBook` constructor to accept a `bool` for the second parameter.
		- [ ] Set the `IsWeighted` property using the `bool` parameter.
		- [ ] Update the `RankedGradeBook` and `StandardGradeBook` constructors to also have the same `bool` parameter. (Don't forget to add the bool to base constructor call after the constructor declaration!)
		- [ ] Update the `BaseGradeBook.GetGPA` method to add 1 point to GPAs of `Honors` and `DuelEnrolled` students when `IsWeighted` is true.

	- [ ] Update `Program.Main`, located in the GradeBook project's root directory, to support `IsWeighted`
		- [ ] Update `Program.Main` so that the `Create` command accepts a third value of `true` or `false` to set `IsWeighted` via the gradebook constructors.
		- [ ] Update the condition that checks the number of parts to check if the number of parts is not equal to 4.
		- [ ] Update the error message to also mention that is weighted is required as well.
		- [ ] Create a new variable to containing if or if not the gradebook should be weighted set by the last part of the command. *Hint: you should convert the provided `string` into a `bool`
		- [ ] Update both Gradebook Instantiations to use our new command value to set `IsWeighted`.
		- [ ] Change where the "help" command outlines the "create" command to say "Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).".

## What Now?

You've compeleted the tasks of this project, if you want to continue working on this project some next steps would be to add support for some of the other grading formats, set Save to run with Add/Removing students and grades, etc.

Otherwise now is a good time to continue on the C# path to expand your understanding of the C# programming language or start looking into the User Interface options of C# whether that's ASP.NET (web), XAML (applications), DirectX (Graphically intense applications), etc