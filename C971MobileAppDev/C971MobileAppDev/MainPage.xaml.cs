using C971MobileAppDev.Resources.Data;
using C971MobileAppDev.Resources.Models;
using System.Collections.ObjectModel;

namespace C971MobileAppDev
{
    public partial class MainPage : ContentPage
    {
        DatabaseService _databaseService;
        public ObservableCollection<Term> Term { get; set; } = new();
        public ObservableCollection<Course> Courses { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _databaseService = new();
            InitDatabase();
        }

        async void InitDatabase()
        {
            await _databaseService.Init();
            await LoadTerms();
            await LoadCourses();
        }

        async Task LoadTerms()
        {
            var terms = await _databaseService.GetTermsAsync();
            Term.Clear();
            foreach (var term in terms)
            {
                 Term.Add(term);
            }
        }

        async Task LoadCourses()
        {
            var courses = await _databaseService.GetCoursesAsync(1);
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }
        }

        async Task AddCourse()
        {
            var newCourse = new Course
            {
                TermId = 1,
                ClassName = "New Course",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3)
            };
            await _databaseService.SaveCourseAsync(newCourse);
            await LoadCourses();
        }

        // Page interactions
        async void AddCourseClicked(object sender, EventArgs e)
        {
            AddCourse();
        }
    }

}
