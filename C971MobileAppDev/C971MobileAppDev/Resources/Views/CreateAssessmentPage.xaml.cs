using C971MobileAppDev.Resources.Models;

namespace C971MobileAppDev.Resources.Views;

public partial class CreateAssessmentPage : ContentPage
{
    public Assessment CurrentAssessment { get; set; }
    public Action<Assessment> OnSave { get; set; }
    public Action<Assessment> OnDelete { get; set; }
    public CreateAssessmentPage(int courseId)
	{
		InitializeComponent();

        CurrentAssessment = new Assessment
        {
            CourseId = courseId,
            Name = "New Course",
            DueDate = DateTime.Today.AddMonths(1),
            Type = "Performance Assessment"
        };

        BindingContext = this;
    }
}