using MyCourseCore.Models;

namespace MyCourse.Resources.Views;

public partial class CreateAssessmentPage : ContentPage
{
    public Assessment CurrentAssessment { get; set; }
    public Action<Assessment> OnSave { get; set; }
    public Action<Assessment> OnDelete { get; set; }
    public string TitleText { get; set; }
    public bool DeleteVisible { get; set; }

    public CreateAssessmentPage(int courseId, string type, Assessment assessment = null)
	{
		InitializeComponent();

        TitleText = $"{type} Assessment";

        CurrentAssessment = assessment ?? new Assessment
        {
            CourseId = courseId,
            Name = "New Assessment",
            DueDate = DateTime.Today,
            Type = "Performance",
            ReminderEnabled = false,
            Notes = ""
        };

        DeleteVisible = assessment != null;

        BindingContext = this;
    }

    private async void SaveAssessmentClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CurrentAssessment.Name))
        {
            await DisplayAlert("Validation Error", "Assessment has no name. Name cannot be empty.", "OK");
            return;
        }
        if (CurrentAssessment.Type != "Objective" && CurrentAssessment.Type != "Performance")
        {
            await DisplayAlert("Validation Error", "Assessment type must be either 'Objective' or 'Performance'.", "OK");
            return;
        }
        OnSave?.Invoke(CurrentAssessment);
        await Navigation.PopAsync();
    }

    private async void DeleteAssessmentClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this assessment?", "Yes", "No");
        if (!confirm) return;
        OnDelete?.Invoke(CurrentAssessment);
        await Navigation.PopAsync();
    }

    private async void ShareNotesClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CurrentAssessment.Notes))
        {
            await DisplayAlert("No Notes", "No notes to share. Add notes before sharing.", "OK");
            return;
        }

        await Share.RequestAsync(new ShareTextRequest
        {
            Text = CurrentAssessment.Notes,
            Title = $"{CurrentAssessment.Name} notes."
        });
    }
}