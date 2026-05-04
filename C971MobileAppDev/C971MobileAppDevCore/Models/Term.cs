using System;
using System.Collections.ObjectModel;
using SQLite;

namespace C971MobileAppDev.Resources.Models
{
    public class Term : BaseModel, IReportable
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => _name = value?.Trim() ?? string.Empty;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Ignore]
        public ObservableCollection<Course> Courses { get; set; }

        public override string[] ToReportRow()
        {
            return new[]
            {
                Id.ToString(),
                Name,
                StartDate.ToString("o"),
                EndDate.ToString("o"),
                CreatedAt.ToString("o"),
                UpdatedAt.ToString("o")
            };
        }

        public override string GetReportHeader() => "Id,Name,StartDate,EndDate,CreatedAt,UpdatedAt";
    }
}
