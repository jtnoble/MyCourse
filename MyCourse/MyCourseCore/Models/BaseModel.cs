using System;
using SQLite;

namespace MyCourseCore.Models
{
    public abstract class BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime _updatedAt = DateTime.UtcNow;

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set => _updatedAt = value;
        }

        public abstract string[] ToReportRow();

        public virtual string GetReportHeader() => "Id,CreatedAt,UpdatedAt";
    }
}