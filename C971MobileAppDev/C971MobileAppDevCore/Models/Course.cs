using System;
using SQLite;

namespace C971MobileAppDev.Resources.Models
{
    public class Course : BaseModel, IReportable
    {
        public int TermId { get; set; }

        private string _className = string.Empty;
        public string ClassName
        {
            get => _className;
            set => _className = value?.Trim() ?? string.Empty;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        private string _instructorName = string.Empty;
        public string InstructorName
        {
            get => _instructorName;
            set => _instructorName = value?.Trim() ?? string.Empty;
        }

        public string InstructorEmail { get; set; }
        public string InstructorPhone { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public bool NotificationsEnabled { get; set; }

        [Ignore]
        public string DateRange => $"{StartDate:d} - {EndDate:d}";

        public override string[] ToReportRow()
        {
            return new[]
            {
                Id.ToString(),
                TermId.ToString(),
                ClassName,
                StartDate.ToString("o"),
                EndDate.ToString("o"),
                InstructorName,
                InstructorEmail,
                InstructorPhone,
                Status,
                CreatedAt.ToString("o"),
                UpdatedAt.ToString("o")
            };
        }

        public override string GetReportHeader() => "Id,TermId,ClassName,StartDate,EndDate,InstructorName,InstructorEmail,InstructorPhone,Status,CreatedAt,UpdatedAt";
    }
}
