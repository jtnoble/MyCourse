using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971MobileAppDev.Resources.Models;

namespace C971MobileAppDev.Resources.Data
{
    public class ReportService
    {
        // Generate a CSV that includes a title row, timestamp, headers, and rows for courses + assessments.
        public async Task<string> GenerateTermCsvReport(Term term, DatabaseService databaseService)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (databaseService == null) throw new ArgumentNullException(nameof(databaseService));

            await databaseService.Init();
            var courses = await databaseService.GetCoursesAsync(term.Id);

            var sb = new StringBuilder();

            // Title and timestamp
            sb.AppendLine($"\"Term Report: {term.Name}\"");
            sb.AppendLine($"\"Generated: {DateTime.UtcNow:o}\"");
            sb.AppendLine();

            // Course header
            sb.AppendLine("CourseId,CourseName,StartDate,EndDate,InstructorName,InstructorEmail,Status");

            foreach (var course in courses)
            {
                var courseLine = $"{EscapeCsv(course.Id.ToString())},{EscapeCsv(course.ClassName)},{EscapeCsv(course.StartDate.ToString("o"))},{EscapeCsv(course.EndDate.ToString("o"))},{EscapeCsv(course.InstructorName)},{EscapeCsv(course.InstructorEmail)},{EscapeCsv(course.Status)}";
                sb.AppendLine(courseLine);

                // assessments for each course
                var assessments = await databaseService.GetAssessmentsAsync(course.Id);
                if (assessments != null && assessments.Any())
                {
                    sb.AppendLine("  AssessmentId,AssessmentName,DueDate,Type,ReminderEnabled,Notes");
                    foreach (var a in assessments)
                    {
                        var aLine = $"{EscapeCsv(a.Id.ToString())},{EscapeCsv(a.Name)},{EscapeCsv(a.DueDate.ToString("o"))},{EscapeCsv(a.Type)},{(a.ReminderEnabled ? "1" : "0")},{EscapeCsv(a.Notes)}";
                        sb.AppendLine($"  {aLine}");
                    }
                }
            }

            // write file
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileName = $"TermReport_{term.Name.Replace(' ', '_')}_{timestamp}.csv";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
            return filePath;
        }

        static string EscapeCsv(string input)
        {
            if (input == null) return "\"\"";
            var needsQuotes = input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r');
            var escaped = input.Replace("\"", "\"\"");
            return needsQuotes ? $"\"{escaped}\"" : escaped;
        }
    }
}