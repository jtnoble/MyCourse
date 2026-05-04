using Xunit;
using System;
using C971MobileAppDev.Resources.Models;

namespace UnitTests
{
    public class AssessmentTests
    {
        [Fact]
        public void Name_ShouldTrimWhitespace()
        {
            var assessment = new Assessment();

            assessment.Name = "  Test Assessment  ";

            Assert.Equal("Test Assessment", assessment.Name);
        }

        [Fact]
        public void Name_ShouldHandleNull()
        {
            var assessment = new Assessment();

            assessment.Name = null;

            Assert.Equal(string.Empty, assessment.Name);
        }

        [Fact]
        public void ToReportRow_ShouldReturnCorrectValues()
        {
            var now = DateTime.UtcNow;

            var assessment = new Assessment
            {
                Id = 1,
                CourseId = 10,
                Name = "Test",
                DueDate = now,
                Type = "Objective",
                ReminderEnabled = true,
                Notes = "Notes",
                CreatedAt = now,
                UpdatedAt = now
            };

            var row = assessment.ToReportRow();

            Assert.Equal("1", row[0]);
            Assert.Equal("10", row[1]);
            Assert.Equal("Test", row[2]);
            Assert.Equal(now.ToString("o"), row[3]);
            Assert.Equal("Objective", row[4]);
            Assert.Equal("1", row[5]);
            Assert.Equal("Notes", row[6]);
            Assert.Equal(now.ToString("o"), row[7]);
            Assert.Equal(now.ToString("o"), row[8]);
        }

        [Fact]
        public void GetReportHeader_ShouldMatchExpected()
        {
            var assessment = new Assessment();

            Assert.Equal(
                "Id,CourseId,Name,DueDate,Type,ReminderEnabled,Notes,CreatedAt,UpdatedAt",
                assessment.GetReportHeader()
            );
        }
    }
}