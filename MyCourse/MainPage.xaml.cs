using C971MobileAppDev.Resources.Data;
using C971MobileAppDev.Resources.Models;
using C971MobileAppDev.Resources.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace C971MobileAppDev
{
    public partial class MainPage : ContentPage
    {
        DatabaseService _databaseService;
        private List<Course> _allCourses = new();

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

        // Search backing
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                ApplySearch(_searchQuery);
            }
        }

        public ICommand SearchCommand { get; set; }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _databaseService = new();
            SearchCommand = new Command(() => ApplySearch(SearchQuery));
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
            _allCourses = courses;
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }

            OnPropertyChanged(nameof(CanAddCourse));
        }

        // Apply search to the current _allCourses set
        void ApplySearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Courses.Clear();
                foreach (var c in _allCourses) Courses.Add(c);
                return;
            }

            var q = query.Trim();
            var filtered = _allCourses.Where(c =>
                (!string.IsNullOrEmpty(c.ClassName) && c.ClassName.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.InstructorName) && c.InstructorName.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.InstructorEmail) && c.InstructorEmail.Contains(q, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            Courses.Clear();
            foreach (var c in filtered) Courses.Add(c);
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

        // Export report for current term
        private async void ExportReportClicked(object sender, EventArgs e)
        {
            if (CurrentTerm == null)
            {
                await DisplayAlert("Report Error", "No current term available.", "OK");
                return;
            }

            var reportService = new ReportService();
            try
            {
                var filePath = await reportService.GenerateTermCsvReport(CurrentTerm, _databaseService);
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = $"Term Report - {CurrentTerm.Name}",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Report Error", ex.Message, "OK");
            }
        }

        // Debug for creating example data
        async void CreateExampleData(object sender, EventArgs e)
        {
            // Add term
            Term term = new Term
            {
                Name = "Example Term",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6)
            };
            await _databaseService.SaveTermAsync(term);
            await LoadTerms();

            _currentTermIndex = Terms.Count - 1;
            UpdateTermUI();

            // Add course
            Course course = new Course
            {
                TermId = term.Id,
                ClassName = "Example Course",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                Status = "Planned",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "5551234567"
            };
            await _databaseService.SaveCourseAsync(course);
            await LoadCourses();

            // Add assessments
            Assessment a1 = new Assessment
            {
                CourseId = course.Id,
                Name = "Ex. PA",
                DueDate = DateTime.Today.AddDays(7),
                Type = "Performance",
                ReminderEnabled = false,
                Notes = "Example notes"
            };
            Assessment a2 = new Assessment
            {
                CourseId = course.Id,
                Name = "Ex. OA",
                DueDate = DateTime.Today.AddDays(14),
                Type = "Objective",
                ReminderEnabled = false,
                Notes = "Example notes"
            };
            await _databaseService.SaveAssessmentAsync(a1);
            await _databaseService.SaveAssessmentAsync(a2);

        }
    }
}