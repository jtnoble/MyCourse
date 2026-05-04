namespace SystemTests
{
    using Xunit;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using C971MobileAppDev.Resources.Data;
    using C971MobileAppDev.Resources.Models;
    using C971MobileAppDev.Resources.Services;

    public class EndToEndTests
    {
        [Fact]
        public async Task FullWorkflow_CreateTermCourseAssessment_GenerateReport()
        {
            var db = new DatabaseService();
            var reportService = new ReportService();

            var term = new Term
            {
                Name = "System Test Term",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };

            await db.SaveTermAsync(term);

            var course = new Course
            {
                TermId = term.Id,
                ClassName = "System Course",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10)
            };

            await db.SaveCourseAsync(course);

            var assessment = new Assessment
            {
                CourseId = course.Id,
                Name = "System Assessment",
                DueDate = DateTime.Now.AddDays(5)
            };

            await db.SaveAssessmentAsync(assessment);

            var path = await reportService.GenerateTermCsvReport(term, db);

            Assert.True(File.Exists(path));

            var content = await File.ReadAllTextAsync(path);

            Assert.Contains("System Test Term", content);
            Assert.Contains("System Course", content);
            Assert.Contains("System Assessment", content);
        }
    }
}