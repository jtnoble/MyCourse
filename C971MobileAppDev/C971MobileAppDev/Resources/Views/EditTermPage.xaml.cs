using C971MobileAppDev.Resources.Models;

namespace C971MobileAppDev.Resources.Views;

public partial class EditTermPage : ContentPage
{
	public Term CurrentTerm { get; set; }
	public Action<Term> OnSave { get; set; }
	public Action<Term> OnDelete { get; set; }
	public bool DeleteVisible { get; set; }
    public EditTermPage(Term term = null)
	{
		InitializeComponent();
		CurrentTerm = term ?? new Term
		{
			Name = "New Term",
			StartDate = DateTime.Now,
			EndDate = DateTime.Now.AddMonths(1)
		};
		DeleteVisible = term != null;

		BindingContext = this;
    }

	private async void SaveTermClicked(object sender, EventArgs e)
	{
		Console.WriteLine($"START: {CurrentTerm.StartDate} | END: {CurrentTerm.EndDate}");
		if (string.IsNullOrWhiteSpace(CurrentTerm.Name))
		{
			await DisplayAlert("Validation Error", "Term name cannot be empty.", "OK");
			return;
		}
		if (CurrentTerm.StartDate >= CurrentTerm.EndDate)
		{
			await DisplayAlert("Validation Error", "Start date must be before end date.", "OK");
			return;
		}
		OnSave?.Invoke(CurrentTerm);
		await Navigation.PopAsync();
    }

	private async void DeleteTermClicked(object sender, EventArgs e)
	{
		bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this term?", "Yes", "No");
		if (!confirm) return;
        OnDelete?.Invoke(CurrentTerm);
		await Navigation.PopAsync();
    }
}