using C971MobileAppDev.Resources.Models;
using C971MobileAppDev.Resources.Data;
using System.Collections.ObjectModel;
using C971MobileAppDev.Resources.Services;
using System.Diagnostics;

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
			if (CurrentCourse.NotificationsEnabled)
			{
				Notify.CreateNotification(
					CurrentCourse.Id * 10 + 1,
					"Course Starting",
					$"{CurrentCourse.ClassName} starting today!",
					CurrentCourse.StartDate
					);
				Notify.CreateNotification(
					CurrentCourse.Id * 10 + 2,
					"Course Ending",
					$"{CurrentCourse.ClassName} ending today!",
					CurrentCourse.EndDate
					);
            }
			else if (!CurrentCourse.NotificationsEnabled)
			{
                Notify.DeleteNotification(CurrentCourse.Id * 10 + 1);
                Notify.DeleteNotification(CurrentCourse.Id * 10 + 2);
            }
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
            int id = newAssessment.Type.Equals("Performance") ? CurrentCourse.Id * 10 + 3 : CurrentCourse.Id * 10 + 4;
            if (newAssessment.ReminderEnabled)
			{
                Notify.CreateNotification(
					id,
					"Assessment Due",
					$"{newAssessment.Name} due today!",
					newAssessment.DueDate
					);
			}
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
            int id = updatedAssessment.Type.Equals("Performance") ? CurrentCourse.Id * 10 + 3 : CurrentCourse.Id * 10 + 4;
            if (updatedAssessment.ReminderEnabled)
            {
                Notify.CreateNotification(
                    id,
                    "Assessment Due",
                    $"{updatedAssessment.Name} due today!",
                    updatedAssessment.DueDate
                    );
            }
            else if (!updatedAssessment.ReminderEnabled)
			{
                Notify.DeleteNotification(id);
            }
            await _databaseService.SaveAssessmentAsync(updatedAssessment);
			LoadAssessments();
		};
		editPage.OnDelete = async (assessmentToDelete) =>
		{
            int id = assessmentToDelete.Type.Equals("Performance") ? CurrentCourse.Id * 10 + 3 : CurrentCourse.Id * 10 + 4;
            Notify.DeleteNotification(id);
            await _databaseService.DeleteAssessmentAsync(assessmentToDelete);
			LoadAssessments();
		};
		await Navigation.PushAsync(editPage);
    }

    private async void ShareNotesClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CurrentCourse.Notes))
        {
            await DisplayAlert("No Notes", "No notes to share. Add notes before sharing.", "OK");
            return;
        }

        await Share.RequestAsync(new ShareTextRequest
        {
            Text = CurrentCourse.Notes,
            Title = $"{CurrentCourse.ClassName} notes."
        });
    }
}