using Xunit;
using System;
using C971MobileAppDev.Resources.Models;

namespace UnitTests
{
    public class TestModel : BaseModel
    {
        public override string[] ToReportRow() => new[] { "test" };
    }

    public class BaseModelTests
    {
        [Fact]
        public void CreatedAt_ShouldDefaultToUtcNow()
        {
            var model = new TestModel();

            Assert.True((DateTime.UtcNow - model.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void UpdatedAt_ShouldBeSettable()
        {
            var model = new TestModel();
            var time = DateTime.UtcNow.AddDays(-1);

            model.UpdatedAt = time;

            Assert.Equal(time, model.UpdatedAt);
        }
    }
}