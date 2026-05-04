namespace IntegrationTests
{
    using Xunit;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using C971MobileAppDev.Resources.Services;
    using C971MobileAppDev.Resources.Data;
    using C971MobileAppDev.Resources.Models;

    public class ReportServiceTests
    {
        [Fact]
        public async Task GenerateTermCsvReport_ShouldCreateFile()
        {
            var db = new DatabaseService();
            var reportService = new ReportService();

            var term = new Term
            {
                Name = "Report Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };

            await db.SaveTermAsync(term);

            var path = await reportService.GenerateTermCsvReport(term, db);

            Assert.True(File.Exists(path));
        }

        [Fact]
        public async Task GenerateTermCsvReport_ShouldContainHeader()
        {
            var db = new DatabaseService();
            var reportService = new ReportService();

            var term = (await db.GetTermsAsync())[0];

            var path = await reportService.GenerateTermCsvReport(term, db);
            var content = await File.ReadAllTextAsync(path);

            Assert.Contains("Entity,Id,TermId", content);
        }

        [Fact]
        public async Task GenerateTermCsvReport_NullTerm_ShouldThrow()
        {
            var reportService = new ReportService();
            var db = new DatabaseService();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                reportService.GenerateTermCsvReport(null, db));
        }
    }
}