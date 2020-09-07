using System;
using TimeLine.Core;
using Xunit;

namespace TimeLine.Tests
{
    public class GeneralTests
    {
        [Fact]
        public void CurrentTimeToString_ReturnsWithZeroesIfNeeded() {
            Assert.Equal("15:00", Utilities.TimeToString(DateTime.Parse("15:00")));
            Assert.Equal("02:03", Utilities.TimeToString(DateTime.Parse("02:03")));
            Assert.Equal("00:00", Utilities.TimeToString(DateTime.Parse("00:00")));
        }
    }
}
