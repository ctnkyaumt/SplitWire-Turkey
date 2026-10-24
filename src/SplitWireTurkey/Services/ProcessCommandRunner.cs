namespace SplitWireTurkey.Services;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public sealed class ProcessCommandRunner : ICommandRunner
{
    public async Task<CommandResult> RunAsync(string fileName, string arguments, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process;
        try
        {
            process = Process.Start(psi);
            if (process == null)
            {
                return new CommandResult(-1, string.Empty, "Failed to start process.", false);
            }
        }
        catch (Exception ex)
        {
            return new CommandResult(-1, string.Empty, ex.Message, false);
        }

        using (process)
        {
            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(timeout);

            try
            {
                await process.WaitForExitAsync(linkedCts.Token).ConfigureAwait(false);
                var stdout = await stdoutTask.ConfigureAwait(false);
                var stderr = await stderrTask.ConfigureAwait(false);
                return new CommandResult(process.ExitCode, stdout, stderr, false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Ignore exceptions during kill
                }

                return new CommandResult(-1, string.Empty, string.Empty, true);
            }
        }
    }
}
