using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using C971MobileAppDev.Resources.Data;
using C971MobileAppDev.Resources.Models;

namespace C971MobileAppDev.Resources.Services
{
    public class ReportService
    {
        public async Task<string> GenerateTermCsvReport(Term term, DatabaseService databaseService)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (databaseService == null) throw new ArgumentNullException(nameof(databaseService));

            var courses = await databaseService.GetCoursesAsync(term.Id).ConfigureAwait(false);
            var sb = new StringBuilder();

            var header = new[]
            {
                "Entity",
                "Id",
                "TermId",
                "TermName",
                "CourseId",
                "CourseName",
                "StartDate",
                "EndDate",
                "DueDate",
                "AssessmentName",
                "AssessmentType",
                "ReminderEnabled",
                "InstructorName",
                "InstructorEmail",
                "InstructorPhone",
                "Status",
                "Notes",
                "CreatedAt",
                "UpdatedAt"
            };
            sb.AppendLine(string.Join(",", header));

            static string Escape(object value)
            {
                if (value == null) return string.Empty;
                var s = Convert.ToString(value) ?? string.Empty;
                if (s.Contains('"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r'))
                {
                    s = s.Replace("\"", "\"\"");
                    return $"\"{s}\"";
                }
                return s;
            }

            sb.AppendLine(string.Join(",",
                Escape("Term"),
                Escape(term.Id),
                Escape(term.Id),
                Escape(term.Name),
                string.Empty, // CourseId
                string.Empty, // CourseName
                Escape(term.StartDate.ToString("o")),
                Escape(term.EndDate.ToString("o")),
                string.Empty, // DueDate
                string.Empty, // AssessmentName
                string.Empty, // AssessmentType
                string.Empty, // ReminderEnabled
                string.Empty, // InstructorName
                string.Empty, // InstructorEmail
                string.Empty, // InstructorPhone
                string.Empty, // Status
                string.Empty, // Notes
                Escape(term.CreatedAt.ToString("o")),
                Escape(term.UpdatedAt.ToString("o"))
            ));

            if (courses != null)
            {
                foreach (var course in courses)
                {
                    // Course row
                    sb.AppendLine(string.Join(",",
                        Escape("Course"),
                        Escape(course.Id),
                        Escape(course.TermId),
                        Escape(term.Name),
                        Escape(course.Id),
                        Escape(course.ClassName),
                        Escape(course.StartDate.ToString("o")),
                        Escape(course.EndDate.ToString("o")),
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        Escape(course.InstructorName),
                        Escape(course.InstructorEmail),
                        Escape(course.InstructorPhone),
                        Escape(course.Status),
                        Escape(course.Notes),
                        Escape(course.CreatedAt.ToString("o")),
                        Escape(course.UpdatedAt.ToString("o"))
                    ));

                    var assessments = await databaseService.GetAssessmentsAsync(course.Id).ConfigureAwait(false);
                    if (assessments != null)
                    {
                        foreach (var a in assessments)
                        {
                            sb.AppendLine(string.Join(",",
                                Escape("Assessment"),
                                Escape(a.Id),
                                Escape(course.TermId),
                                Escape(term.Name),
                                Escape(a.CourseId),
                                Escape(string.Empty), // CourseName (optional)
                                string.Empty, // StartDate
                                string.Empty, // EndDate
                                Escape(a.DueDate.ToString("o")),
                                Escape(a.Name),
                                Escape(a.Type),
                                Escape(a.ReminderEnabled ? "1" : "0"),
                                string.Empty, // InstructorName (not on assessment)
                                string.Empty,
                                string.Empty,
                                string.Empty, // Status
                                Escape(a.Notes),
                                Escape(a.CreatedAt.ToString("o")),
                                Escape(a.UpdatedAt.ToString("o"))
                            ));
                        }
                    }
                }
            }

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var safeTermName = string.IsNullOrWhiteSpace(term.Name) ? $"term_{term.Id}" : MakeSafeFileName(term.Name);
            var fileName = $"TermReport_{safeTermName}_{term.Id}_{timestamp}.csv";
            var path = Path.Combine(Path.GetTempPath(), fileName);

            await File.WriteAllTextAsync(path, sb.ToString()).ConfigureAwait(false);
            return path;
        }

        static string MakeSafeFileName(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(c, '_');
            }
            return s;
        }
    }
}