using C971MobileAppDev.Resources.Data;
using C971MobileAppDev.Resources.Models;
using C971MobileAppDev.Resources.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace C971MobileAppDev
{
    public partial class MainPage : ContentPage
    {
        DatabaseService _databaseService;
        public ObservableCollection<Term> Terms { get; set; } = new();
        public ObservableCollection<Course> Courses { get; set; } = new();

        private const int MAX_COURSES = 6;
        public bool CanAddCourse => Courses.Count < MAX_COURSES;

        private int _currentTermIndex = 0;
        public Term CurrentTerm =>
            Terms.Count > 0 && _currentTermIndex < Terms.Count
            ? Terms[_currentTermIndex]
            : null;
        public string PreviousTermText =>
            _currentTermIndex > 0 && _currentTermIndex - 1 < Terms.Count
            ? Terms[_currentTermIndex - 1].Name
            : " ";
        public string NextTermText =>
            _currentTermIndex + 1 < Terms.Count
            ? Terms[_currentTermIndex + 1].Name
            : "Add";

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _databaseService = new();
            InitDatabase();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCourses();
        }

        async void InitDatabase()
        {
            await _databaseService.Init();
            await LoadTerms();
            await LoadCourses();
        }

        // Term Functions
        async Task LoadTerms()
        {
            var terms = await _databaseService.GetTermsAsync();
            Terms.Clear();
            foreach (var term in terms)
            {
                Terms.Add(term);
            }
            if (_currentTermIndex >= Terms.Count)
            {
                _currentTermIndex = Math.Max(0, Terms.Count - 1);
            }

            UpdateTermUI();
        }

        async Task AddTerm(Term newTerm)
        {
            await _databaseService.SaveTermAsync(newTerm);
            await LoadTerms();

            _currentTermIndex = Terms.Count - 1;
            UpdateTermUI();
        }

        async Task DeleteTerm(Term deleteTerm)
        {
            if (deleteTerm == null) return;
            if (Terms.Count <= 1)
            {
                await DisplayAlert("Error", "At least one term must exist.", "OK");
                return;
            }
            if (Courses.Count > 0)
            {
                await DisplayAlert("Error", "Please delete all courses in this term before deleting the term.", "OK");
                return;
            }
            await _databaseService.DeleteTermAsync(deleteTerm);
            if (_currentTermIndex > 0) _currentTermIndex--;
            await LoadTerms();
        }

        private async void EditTermClicked(object sender, EventArgs e)
        {
            if (CurrentTerm == null) return;
            var editPage = new EditTermPage(CurrentTerm);
            editPage.OnSave = async (updateTerm) => await AddTerm(updateTerm);
            editPage.OnDelete = async (deleteTerm) => await DeleteTerm(deleteTerm);
            await Navigation.PushAsync(editPage);
        }
        void PreviousTermClicked(object sender, EventArgs e)
        {
            if (_currentTermIndex > 0)
            {
                _currentTermIndex--;
                UpdateTermUI();
            }
        }
        async void NextTermClicked(object sender, EventArgs e)
        {
            if (_currentTermIndex < Terms.Count - 1)
            {
                _currentTermIndex++;
                UpdateTermUI();
            }
            else
            {
                var newPage = new EditTermPage();
                newPage.OnSave = async (newTerm) => await AddTerm(newTerm);
                await Navigation.PushAsync(newPage);
                await LoadTerms();
            }
        }

        async void UpdateTermUI()
        {
            OnPropertyChanged(nameof(CurrentTerm));
            OnPropertyChanged(nameof(PreviousTermText));
            OnPropertyChanged(nameof(NextTermText));
            await LoadCourses();
        }

        // Course Functions
        async Task LoadCourses()
        {
            if (CurrentTerm == null) return;

            var courses = await _databaseService.GetCoursesAsync(CurrentTerm.Id);
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }

            OnPropertyChanged(nameof(CanAddCourse));
        }

        async Task AddCourse()
        {
            var newCourse = new Course
            {
                TermId = CurrentTerm.Id,
                ClassName = "New Course",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3)
            };
            await _databaseService.SaveCourseAsync(newCourse);
            await LoadCourses();
        }
        async void AddCourseClicked(object sender, EventArgs e)
        {
            var page = new CreateCoursePage(CurrentTerm.Id);

            page.OnSave = async (newCourse) =>
            {
                await _databaseService.SaveCourseAsync(newCourse);
                await LoadCourses();
            };

            await Navigation.PushAsync(page);
        }

        async void ViewCourseClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var course = button?.BindingContext as Course;
            var coursePage = new CourseDetailsPage(course);
            await Navigation.PushAsync(coursePage);
        }
    }
}
