using MyCourseCore.Models;

namespace MyCourse.Resources.Views;

public partial class CreateCoursePage : ContentPage
{
	public Course CurrentCourse { get; set; }
	public Action<Course> OnSave { get; set; }
	public Action<Course> OnDelete { get; set; }
	public bool DeleteVisible { get; set; }
	public string TitleText => DeleteVisible ? "Edit Course" : "New Course";
	public string SaveText => DeleteVisible ? "Save Changes" : "Create Course";

    public CreateCoursePage(int termId, Course course = null)
	{
		InitializeComponent();

		CurrentCourse = course ?? new Course
		{
			TermId = termId,
			ClassName = "New Course",
            StartDate = DateTime.Today,
			EndDate = DateTime.Today.AddMonths(3),
			Status = "Planned"
		};

		DeleteVisible = course != null;

		BindingContext = this;
    }

	private async void CreateCourseClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(CurrentCourse.ClassName))
		{
			await DisplayAlert("Validation Error", "Course name cannot be empty.", "OK");
			return;
		}

		if (CurrentCourse.StartDate >= CurrentCourse.EndDate)
		{
			await DisplayAlert("Validation Error", "Start date must be before end date.", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(CurrentCourse.InstructorName)) {
			await DisplayAlert("Validation Error", "Instructor name cannot be empty.", "OK");
			return;
		}

        string phoneRegex = @"^\+?\d{10,15}$";
        if (string.IsNullOrWhiteSpace(CurrentCourse.InstructorPhone) ||
            !System.Text.RegularExpressions.Regex.IsMatch(CurrentCourse.InstructorPhone, phoneRegex))
        {
            await DisplayAlert("Validation Error", "Instructor phone must be valid (10-15 digits, optional leading +).", "OK");
            return;
        }

        string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (string.IsNullOrWhiteSpace(CurrentCourse.InstructorEmail) || 
			!System.Text.RegularExpressions.Regex.IsMatch(CurrentCourse.InstructorEmail, emailRegex))
		{
			await DisplayAlert("Validation Error", "Instructor email must be valid (something@something.something).", "OK");
			return;
        }

		

        OnSave?.Invoke(CurrentCourse);
		await Navigation.PopAsync();
    }

	private async void DeleteCourseClicked(object sender, EventArgs e)
	{
		bool confirm = await DisplayAlert(
			"Confirm Delete",
			"Delete this course?",
			"Yes",
			"No");

		if (!confirm) return;

		OnDelete?.Invoke(CurrentCourse);

		await Navigation.PopAsync();
	}
}