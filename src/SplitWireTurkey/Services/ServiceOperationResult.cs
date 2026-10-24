namespace SplitWireTurkey.Services;

public sealed record ServiceOperationResult(string ServiceName, bool Success, ServiceState FinalState, string Message);
