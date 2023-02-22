using Xunit;

namespace Buttercup.Api;

public class ClockTests
{
    #region UtcNow

    [Fact]
    public void UtcNow_ReturnsCurrentUtcTime()
    {
        var start = DateTime.UtcNow;
        var actual = new Clock().UtcNow;
        var finish = DateTime.UtcNow;

        Assert.InRange(actual, start, finish);
    }

    #endregion
}
