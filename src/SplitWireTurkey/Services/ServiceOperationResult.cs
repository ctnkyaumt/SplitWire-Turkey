namespace SplitWireTurkey.Services;

using System;

public sealed record ServiceOperationResult(string ServiceName, bool Success, ServiceState FinalState, string Message);
