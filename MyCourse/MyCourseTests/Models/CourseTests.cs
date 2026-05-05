using Xunit;
using System;
using MyCourseCore.Models;

namespace UnitTests
{
    public class CourseTests
    {
        [Fact]
        public void ClassName_ShouldTrimWhitespace()
        {
            var course = new Course();

            course.ClassName = "  Math 101  ";

            Assert.Equal("Math 101", course.ClassName);
        }

        [Fact]
        public void InstructorName_ShouldTrimWhitespace()
        {
            var course = new Course();

            course.InstructorName = "  John Doe  ";

            Assert.Equal("John Doe", course.InstructorName);
        }

        [Fact]
        public void DateRange_ShouldFormatCorrectly()
        {
            var course = new Course
            {
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 2, 1)
            };

            var expected = $"{course.StartDate:d} - {course.EndDate:d}";

            Assert.Equal(expected, course.DateRange);
        }

        [Fact]
        public void ToReportRow_ShouldReturnCorrectValues()
        {
            var now = DateTime.UtcNow;

            var course = new Course
            {
                Id = 1,
                TermId = 2,
                ClassName = "Math",
                StartDate = now,
                EndDate = now,
                InstructorName = "John",
                InstructorEmail = "test@test.com",
                InstructorPhone = "123",
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            var row = course.ToReportRow();

            Assert.Equal("1", row[0]);
            Assert.Equal("2", row[1]);
            Assert.Equal("Math", row[2]);
            Assert.Equal(now.ToString("o"), row[3]);
            Assert.Equal(now.ToString("o"), row[4]);
            Assert.Equal("John", row[5]);
        }

        [Fact]
        public void GetReportHeader_ShouldMatchExpected()
        {
            var course = new Course();

            Assert.Equal(
                "Id,TermId,ClassName,StartDate,EndDate,InstructorName,InstructorEmail,InstructorPhone,Status,CreatedAt,UpdatedAt",
                course.GetReportHeader()
            );
        }
    }
}