using C971MobileAppDev.Resources.Models;
using C971MobileAppDev.Resources.Data;
using System.Collections.ObjectModel;

namespace C971MobileAppDev.Resources.Views;

public partial class CourseDetailsPage : ContentPage
{
	DatabaseService _databaseService;
	public Course CurrentCourse { get; set; }
	public ObservableCollection<Assessment> Assessments { get; set; } = new();
    public CourseDetailsPage(Course course)
	{
		InitializeComponent();
		_databaseService = new();
		CurrentCourse = course;
		BindingContext = this;
		LoadAssessments();
    }

	async void LoadAssessments()
	{
		var assessments = await _databaseService.GetAssessmentsAsync(CurrentCourse.Id);
		Assessments.Clear();
		foreach (var assessment in assessments)
		{
			Assessments.Add(assessment);
		}
    }

	async void EditCourseClicked(object sender, EventArgs e)
	{
		var editPage = new CreateCoursePage(CurrentCourse.TermId, CurrentCourse);
		editPage.OnSave = async (updatedCourse) =>
		{
			await _databaseService.SaveCourseAsync(updatedCourse);
			CurrentCourse = updatedCourse;
			OnPropertyChanged(nameof(CurrentCourse));
			LoadAssessments();
		};
		editPage.OnDelete = async (courseToDelete) =>
		{
			await _databaseService.DeleteCourseAsync(courseToDelete);
			await Navigation.PopAsync();
		};
		await Navigation.PushAsync(editPage);
    }

	async void AddAssessmentClicked(object sender, EventArgs e)
	{
		var newPage = new CreateAssessmentPage(CurrentCourse.Id);
		newPage.OnSave = async (newAssessment) =>
		{
			await _databaseService.SaveAssessmentAsync(newAssessment);
			LoadAssessments();
		};
		await Navigation.PushAsync(newPage);
	}
}