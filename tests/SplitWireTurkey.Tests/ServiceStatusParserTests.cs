namespace SplitWireTurkey.Tests;

using SplitWireTurkey.Services;
using Xunit;

public class ServiceStatusParserTests
{
    // Real fixture verbatim from temp_work/fixtures/sc_query_running_en.txt (EventLog, exit 0)
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

    // Real fixture verbatim from temp_work/fixtures/sc_query_stopped_en.txt (AppMgmt, exit 0)
    private const string ScQueryStoppedEn = @"
SERVICE_NAME: AppMgmt 
        TYPE               : 20  WIN32_SHARE_PROCESS  
        STATE              : 1  STOPPED 
        WIN32_EXIT_CODE    : 1077  (0x435)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
";

    // Real fixture verbatim from temp_work/fixtures/sc_query_missing_en.txt (exit 36)
    private const string ScQueryMissingEn = @"[SC] EnumQueryServicesStatus:OpenService FAILED 1060:

The specified service does not exist as an installed service.
";

    // Synthetic localized sample: label DURUM instead of STATE, Turkish-ish labels
    private const string ScQueryRunningTrSynthetic = @"
SERVICE_NAME: OrnekHizmet 
        TUR                : 30  WIN32  
        DURUM              : 4  RUNNING 
                                (DURDURULABILIR, DURAKLATILAMAZ, KAPANISI_KABUL_EDER)
        WIN32_CIKIS_KODU   : 0  (0x0)
        HIZMET_CIKIS_KODU  : 0  (0x0)
        KONTROL_NOKTA      : 0x0
        BEKLEME_IPUCU      : 0x0
";

    [Fact]
    public void Parse_real_running_fixture_returns_Running()
    {
        var result = new CommandResult(0, ScQueryRunningEn, string.Empty, false);
        var state = ServiceStatusParser.Parse(result);
        Assert.Equal(ServiceState.Running, state);
    }

    [Fact]
    public void Parse_real_stopped_fixture_returns_Stopped()
    {
        var result = new CommandResult(0, ScQueryStoppedEn, string.Empty, false);
        var state = ServiceStatusParser.Parse(result);
        Assert.Equal(ServiceState.Stopped, state);
    }

    [Fact]
    public void Parse_real_missing_fixture_returns_NotInstalled()
    {
        // exit=36 in fixture, contains FAILED 1060:
        var result = new CommandResult(36, ScQueryMissingEn, string.Empty, false);
        var state = ServiceStatusParser.Parse(result);
        Assert.Equal(ServiceState.NotInstalled, state);
    }

    [Fact]
    public void Parse_synthetic_localized_fixture_returns_Running()
    {
        // Synthetic fixture with DURUM: 4 RUNNING
        var result = new CommandResult(0, ScQueryRunningTrSynthetic, string.Empty, false);
        var state = ServiceStatusParser.Parse(result);
        Assert.Equal(ServiceState.Running, state);
    }

    [Fact]
    public void Parse_timed_out_result_returns_Unknown()
    {
        var result = new CommandResult(0, string.Empty, string.Empty, TimedOut: true);
        Assert.Equal(ServiceState.Unknown, ServiceStatusParser.Parse(result));
    }

    [Fact]
    public void Parse_empty_output_with_negative_exit_code_returns_Unknown()
    {
        var result = new CommandResult(-1, string.Empty, "error", TimedOut: false);
        Assert.Equal(ServiceState.Unknown, ServiceStatusParser.Parse(result));
    }

    [Fact]
    public void Parse_null_result_returns_Unknown()
    {
        Assert.Equal(ServiceState.Unknown, ServiceStatusParser.Parse(null));
    }
}
