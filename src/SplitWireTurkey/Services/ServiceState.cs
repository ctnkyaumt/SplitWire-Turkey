namespace SplitWireTurkey.Services;

using System;

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
