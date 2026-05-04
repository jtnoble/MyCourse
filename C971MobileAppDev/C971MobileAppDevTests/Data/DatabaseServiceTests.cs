namespace IntegrationTests
{
    using Xunit;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using C971MobileAppDev.Resources.Data;
    using C971MobileAppDev.Resources.Models;

    public class DatabaseServiceTests
    {
        [Fact]
        public async Task Init_ShouldCreateDefaultTerm()
        {
            var service = new DatabaseService();

            var terms = await service.GetTermsAsync();

            Assert.NotEmpty(terms);
            Assert.Contains(terms, t => t.Name == "Default Term");
        }

        [Fact]
        public async Task SaveTerm_ShouldInsertAndRetrieve()
        {
            var service = new DatabaseService();

            var term = new Term
            {
                Name = "Test Term",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };

            await service.SaveTermAsync(term);

            var terms = await service.GetTermsAsync();

            Assert.Contains(terms, t => t.Name == "Test Term");
        }

        [Fact]
        public async Task SaveCourse_ShouldInsertAndRetrieve()
        {
            var service = new DatabaseService();

            var term = (await service.GetTermsAsync()).First();

            var course = new Course
            {
                TermId = term.Id,
                ClassName = "Test Course",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10)
            };

            await service.SaveCourseAsync(course);

            var courses = await service.GetCoursesAsync(term.Id);

            Assert.Contains(courses, c => c.ClassName == "Test Course");
        }

        [Fact]
        public async Task SaveAssessment_ShouldInsertAndRetrieve()
        {
            var service = new DatabaseService();

            var term = (await service.GetTermsAsync()).First();

            var course = new Course
            {
                TermId = term.Id,
                ClassName = "Course"
            };

            await service.SaveCourseAsync(course);

            var assessment = new Assessment
            {
                CourseId = course.Id,
                Name = "Assessment 1",
                DueDate = DateTime.Now
            };

            await service.SaveAssessmentAsync(assessment);

            var assessments = await service.GetAssessmentsAsync(course.Id);

            Assert.Contains(assessments, a => a.Name == "Assessment 1");
        }
    }
}