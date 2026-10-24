namespace SplitWireTurkey.Services;

public enum ServiceState
{
    NotInstalled,
    Stopped,
    StartPending,
    StopPending,
    Running,
    ContinuePending,
    PausePending,
    Paused,
    Unknown
}
