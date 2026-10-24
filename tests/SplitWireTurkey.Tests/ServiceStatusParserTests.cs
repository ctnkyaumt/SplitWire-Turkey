namespace SplitWireTurkey.Tests;

using SplitWireTurkey.Services;
using Xunit;

public class ServiceStatusParserTests
{
    // Real fixture from temp_work/fixtures/sc_query_running_en.txt (EventLog, exit 0)
    private const string ScQueryRunningEn = @"
SERVICE_NAME: EventLog 
        TYPE               : 30  WIN32  
        STATE              : 4  RUNNING 
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
";

    [Fact]
    public void Parse_real_running_fixture_returns_Running()
    {
        var result = new CommandResult(0, ScQueryRunningEn, string.Empty, false);
        var state = ServiceStatusParser.Parse(result);
        Assert.Equal(ServiceState.Running, state);
    }
}
