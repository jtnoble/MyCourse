using Xunit;
using System;
using MyCourseCore.Models;

namespace UnitTests
{

    public class TermTests
    {
        [Fact]
        public void Name_ShouldTrimWhitespace()
        {
            var term = new Term();

            term.Name = "  Term 1  ";

            Assert.Equal("Term 1", term.Name);
        }

        [Fact]
        public void Name_ShouldHandleNull()
        {
            var term = new Term();

            term.Name = null;

            Assert.Equal(string.Empty, term.Name);
        }

        [Fact]
        public void ToReportRow_ShouldReturnCorrectValues()
        {
            var now = DateTime.UtcNow;

            var term = new Term
            {
                Id = 1,
                Name = "Term 1",
                StartDate = now,
                EndDate = now,
                CreatedAt = now,
                UpdatedAt = now
            };

            var row = term.ToReportRow();

            Assert.Equal("1", row[0]);
            Assert.Equal("Term 1", row[1]);
            Assert.Equal(now.ToString("o"), row[2]);
            Assert.Equal(now.ToString("o"), row[3]);
        }

        [Fact]
        public void GetReportHeader_ShouldMatchExpected()
        {
            var term = new Term();

            Assert.Equal(
                "Id,Name,StartDate,EndDate,CreatedAt,UpdatedAt",
                term.GetReportHeader()
            );
        }
    }
}