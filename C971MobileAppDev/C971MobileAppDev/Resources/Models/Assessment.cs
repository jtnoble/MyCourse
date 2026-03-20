using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971MobileAppDev.Resources.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Type { get; set; }
        public bool ReminderEnabled { get; set; }
        public string Notes { get; set; }

    }
}
