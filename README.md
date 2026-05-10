# MyCourse

## Description
A basic mobile-friendly application that allows you to create a school schedule for yourself, including terms, classes, and information about said classes.

## Installation
When a new change has been made, a new build will be uploaded.
- On the right side of the page, the newest download for the MyCourse application is available. Download it on your phone.
- Install the APK file that is downloaded.
  - As this is an unofficial means of getting an APK, you may be required to disable some security features on your phone to install this application.

## Running
A user guide
- Terms
  - You can add additional terms from the term page bu clicking on the "add term" in the top right"
  - You can navigate between terms by clicking on them in the top left and top right
  - You can edit a term by clicking on the term name after it has been selected in the top middle
  - Terms can only be deleted if there are no courses
  - Term information can be exported and shared on the term page by click "Export term"
  - Terms have a maximum of 6 courses
  - Adding courses can be done by clicking "Add course"
- Courses
  - Courses can be added from the term page
  - Courses require course names, start/end dates, and instructor information. Reminders and notes are not required
  - All courses can be edited/deleted from the edit menu in the top right of the course
  - Course notes can be shared via the "Share notes" feature on the course page
  - Each course allows for one objective and one performance assessment
- Assessments
  - Assessments can be added from the course page
  - You will be presented with an option to add a performance or objective assessment
  - You can only add 1 of each assessment type
  - Reminders and notes are not required in assessments

## Development
- Clone the repository and open the solution in Visual Studio.
  - *If downloading this directly to your machine, be sure to keep in mind the 260 character limit in file paths, and maybe put this at a higher level (such as `C:\Users\username\Dev\<this-repo>`).*
- Ensure you have .NET 8.0.x installed on your system. This is not compatible with .NET 9.0.x at this time
- Upon building, you may be asked to install certain NuGet packages. Accept these changes.
- Use Visual Studio's built in building for the solution.
- Set up an android emulator to work directly with the solution. Personally, I chose the Pixel 7 for development.
  - While windows *can* be used for development purposes, it is highly recommended you use android emulation.
- UI changes can be saved with the emulator running.
- Core functionality (database services, for example), may require a restart of the application to save.

## Testing
- Tests are available in the MyCourseTests solution.
- Using Visual Studio's built in test explorer, you can run tests here.
- Tests should be run immediately after making changes, and prior to pushing changes.
- UAT testing should be done with real-life students to confirm functionality changes make sense.

## Release
- After pushing updates to the code, a github action will immediately trigger uploading a release build.
- Navigate to the successful github action, and navigate to the "Create Release and Upload APK" section.
- Click on the link at "Release ready at <link>"
- Click on edit, select pre-release, and publish the release.
- After testing on your own phone, click on "Create a new release" from the main page of github, and add the tag for the pre-release.

## Authors and acknowledgment
Jtnoble

## License
N/A
