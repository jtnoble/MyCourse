using C971MobileAppDev.Resources.Models;
using C971MobileAppDev.Resources.Data;
using System.Collections.ObjectModel;

namespace C971MobileAppDev.Resources.Views;

public partial class CourseDetailsPage : ContentPage
{
	DatabaseService _databaseService;
	public Course CurrentCourse { get; set; }
	public ObservableCollection<Assessment> Assessments { get; set; } = new();

	public bool HasObjectiveAssessment { get; set; }
	public bool HasPerformanceAssessment { get; set; }
	public bool CanAddObjective => !HasObjectiveAssessment;
	public bool CanAddPerformance => !HasPerformanceAssessment;

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

		HasObjectiveAssessment = Assessments.Any(a => a.Type == "Objective");
		HasPerformanceAssessment = Assessments.Any(a => a.Type == "Performance");
		OnPropertyChanged(nameof(HasObjectiveAssessment));
		OnPropertyChanged(nameof(HasPerformanceAssessment));
		OnPropertyChanged(nameof(CanAddObjective));
		OnPropertyChanged(nameof(CanAddPerformance));
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

	async Task AddAssessment(Assessment newAssessment)
	{
		var newPage = new CreateAssessmentPage(CurrentCourse.Id, newAssessment.Type, newAssessment);
		newPage.OnSave = async (newAssessment) =>
		{
			await _databaseService.SaveAssessmentAsync(newAssessment);
			LoadAssessments();
		};
		await Navigation.PushAsync(newPage);
	}

	async void AddObjectiveClicked(object sender, EventArgs e)
	{
		var newAssessment = new Assessment
		{
			CourseId = CurrentCourse.Id,
			Name = "Objective Assessment",
			DueDate = DateTime.Today,
			Type = "Objective",
			ReminderEnabled = false,
			Notes = ""
		};
        await AddAssessment(newAssessment);
    }

	async void AddPerformanceClicked(object sender, EventArgs e)
	{
		var newAssessment = new Assessment
		{
			CourseId = CurrentCourse.Id,
			Name = "Performance Assessment",
			DueDate = DateTime.Today,
			Type = "Performance",
			ReminderEnabled = false,
			Notes = ""
		};
		await AddAssessment(newAssessment);
    }

    async void AssessmentTapped(object sender, TappedEventArgs e)
	{
		var frame = sender as Frame;
		var assessment = frame?.BindingContext as Assessment;
		if (assessment == null) return;
		var editPage = new CreateAssessmentPage(CurrentCourse.Id, assessment.Type, assessment);
		editPage.OnSave = async (updatedAssessment) =>
		{
			await _databaseService.SaveAssessmentAsync(updatedAssessment);
			LoadAssessments();
		};
		editPage.OnDelete = async (assessmentToDelete) =>
		{
			await _databaseService.DeleteAssessmentAsync(assessmentToDelete);
			LoadAssessments();
		};
		await Navigation.PushAsync(editPage);
    }
}