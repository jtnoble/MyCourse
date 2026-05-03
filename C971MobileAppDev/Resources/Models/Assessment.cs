using System;
using SQLite;

namespace C971MobileAppDev.Resources.Models
{
    public class Assessment : BaseModel, IReportable
    {
        public int CourseId { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => _name = value?.Trim() ?? string.Empty;
        }

        public DateTime DueDate { get; set; }
        public string Type { get; set; }
        public bool ReminderEnabled { get; set; }
        public string Notes { get; set; }

        public override string[] ToReportRow()
        {
            return new[]
            {
                Id.ToString(),
                CourseId.ToString(),
                Name,
                DueDate.ToString("o"),
                Type,
                ReminderEnabled ? "1" : "0",
                Notes,
                CreatedAt.ToString("o"),
                UpdatedAt.ToString("o")
            };
        }

        public override string GetReportHeader() => "Id,CourseId,Name,DueDate,Type,ReminderEnabled,Notes,CreatedAt,UpdatedAt";
    }
}